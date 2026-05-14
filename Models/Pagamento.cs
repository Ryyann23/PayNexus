namespace PayNexus.Models;

public sealed class Pagamento
{
    private readonly List<string> _eventos = [];

    public Pagamento(Cliente cliente, decimal valor)
    {
        if (valor <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(valor), "O valor do pagamento deve ser maior que zero.");
        }

        Cliente = cliente;
        Valor = valor;
    }

    public Guid Id { get; } = Guid.NewGuid();

    public Cliente Cliente { get; }

    public decimal Valor { get; }

    public string MetodoPagamento { get; private set; } = "Não definido";

    public StatusPagamento Status { get; private set; } = StatusPagamento.Pendente;

    public string StatusOperacional { get; private set; } = "Aguardando processamento";

    public string CodigoTransacao { get; private set; } = string.Empty;

    public string MensagemProcessamento { get; private set; } = "Aguardando processamento.";

    public DateTime CriadoEm { get; } = DateTime.Now;

    public DateTime? ProcessadoEm { get; private set; }

    public IReadOnlyList<string> Eventos => _eventos.AsReadOnly();

    public void DefinirMetodo(string metodoPagamento)
    {
        MetodoPagamento = metodoPagamento;
    }

    public void AplicarResultado(ResultadoPagamento resultado)
    {
        Status = resultado.Status;
        CodigoTransacao = resultado.CodigoTransacao;
        MensagemProcessamento = resultado.Mensagem;
        ProcessadoEm = DateTime.Now;
        AdicionarEvento(resultado.Mensagem);
    }

    public void AtualizarStatusOperacional(string statusOperacional)
    {
        StatusOperacional = statusOperacional;
        AdicionarEvento($"Status operacional atualizado para {statusOperacional}.");
    }

    public void AdicionarEvento(string descricao)
    {
        _eventos.Add($"{DateTime.Now:HH:mm:ss} - {descricao}");
    }
}

public enum StatusPagamento
{
    Aprovado,
    Pendente,
    Recusado
}
