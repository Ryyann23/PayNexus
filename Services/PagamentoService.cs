using PayNexus.Core;
using PayNexus.Models;
using PayNexus.Observers;
using PayNexus.Strategies;

namespace PayNexus.Services;

public sealed class PagamentoService
{
    private readonly SistemaPagamento _sistemaPagamento = new();
    private readonly List<Pagamento> _pagamentos = [];
    private readonly Dictionary<string, IEstrategiaPagamento> _estrategias;

    public PagamentoService()
    {
        _estrategias = new Dictionary<string, IEstrategiaPagamento>(StringComparer.OrdinalIgnoreCase)
        {
            [new PagamentoPix().Metodo] = new PagamentoPix(),
            [new PagamentoCartao().Metodo] = new PagamentoCartao(),
            [new PagamentoBoleto().Metodo] = new PagamentoBoleto()
        };

        _sistemaPagamento.RegistrarObserver(new ObserverEmail());
        _sistemaPagamento.RegistrarObserver(new ObserverLog());
        _sistemaPagamento.RegistrarObserver(new ObserverHistorico());
        _sistemaPagamento.RegistrarObserver(new ObserverStatus());
    }

    public event Action<string>? LogRegistrado;

    public event Action? PagamentosAtualizados;

    public IReadOnlyList<Pagamento> Pagamentos => _pagamentos.AsReadOnly();

    public IReadOnlyList<string> MetodosPagamento => _estrategias.Keys.ToList();

    public IReadOnlyList<string> ObserversAtivos => _sistemaPagamento.Observers.Select(observer => observer.Nome).ToList();

    public Pagamento ProcessarPagamento(string nomeCliente, decimal valor, string metodoPagamento)
    {
        if (!_estrategias.TryGetValue(metodoPagamento, out var estrategia))
        {
            throw new InvalidOperationException($"Método de pagamento não suportado: {metodoPagamento}");
        }

        var pagamento = new Pagamento(new Cliente(nomeCliente), valor);
        RegistrarLog($"Novo pagamento criado para {pagamento.Cliente.Nome}.");

        _sistemaPagamento.DefinirEstrategia(estrategia);
        var pagamentoProcessado = _sistemaPagamento.Processar(pagamento, RegistrarLog);

        _pagamentos.Insert(0, pagamentoProcessado);
        PagamentosAtualizados?.Invoke();

        return pagamentoProcessado;
    }

    public bool AdicionarObserverNotificacaoSistema()
    {
        var adicionado = _sistemaPagamento.RegistrarObserver(new ObserverNotificacaoSistema());
        RegistrarLog(adicionado
            ? "Observer Notificações do Sistema adicionado em tempo de execução."
            : "Observer Notificações do Sistema já está ativo.");

        return adicionado;
    }

    public PagamentoResumo ObterResumo()
    {
        var hoje = DateTime.Today;
        var totalProcessado = _pagamentos
            .Where(pagamento => pagamento.Status == StatusPagamento.Aprovado)
            .Sum(pagamento => pagamento.Valor);

        return new PagamentoResumo(
            totalProcessado,
            _pagamentos.Count(pagamento => pagamento.CriadoEm.Date == hoje),
            _pagamentos.Count(pagamento => pagamento.Status == StatusPagamento.Aprovado),
            _pagamentos.Count(pagamento => pagamento.Status == StatusPagamento.Pendente));
    }

    private void RegistrarLog(string mensagem)
    {
        LogRegistrado?.Invoke(mensagem);
    }
}
