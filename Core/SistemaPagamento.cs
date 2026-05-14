using PayNexus.Models;
using PayNexus.Observers;
using PayNexus.Strategies;

namespace PayNexus.Core;

public sealed class SistemaPagamento
{
    private readonly List<IObserverPagamento> _observers = [];

    public IEstrategiaPagamento? EstrategiaAtual { get; private set; }

    public IReadOnlyList<IObserverPagamento> Observers => _observers.AsReadOnly();

    public void DefinirEstrategia(IEstrategiaPagamento estrategia)
    {
        EstrategiaAtual = estrategia ?? throw new ArgumentNullException(nameof(estrategia));
    }

    public bool RegistrarObserver(IObserverPagamento observer)
    {
        ArgumentNullException.ThrowIfNull(observer);

        if (_observers.Any(item => item.GetType() == observer.GetType()))
        {
            return false;
        }

        _observers.Add(observer);
        return true;
    }

    public Pagamento Processar(Pagamento pagamento, Action<string> registrarLog)
    {
        ArgumentNullException.ThrowIfNull(pagamento);
        ArgumentNullException.ThrowIfNull(registrarLog);

        if (EstrategiaAtual is null)
        {
            throw new InvalidOperationException("Nenhuma estratégia de pagamento foi definida.");
        }

        pagamento.DefinirMetodo(EstrategiaAtual.Metodo);
        registrarLog($"Strategy selecionada: {EstrategiaAtual.Metodo}.");

        var resultado = EstrategiaAtual.Processar(pagamento);
        pagamento.AplicarResultado(resultado);
        registrarLog(resultado.Mensagem);

        NotificarObservers(pagamento, registrarLog);
        return pagamento;
    }

    private void NotificarObservers(Pagamento pagamento, Action<string> registrarLog)
    {
        foreach (var observer in _observers)
        {
            observer.Atualizar(pagamento, registrarLog);
        }
    }
}
