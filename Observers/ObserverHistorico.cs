using PayNexus.Models;

namespace PayNexus.Observers;

public sealed class ObserverHistorico : IObserverPagamento
{
    public string Nome => "Histórico";

    public void Atualizar(Pagamento pagamento, Action<string> registrarLog)
    {
        pagamento.AdicionarEvento("Histórico financeiro atualizado.");
        registrarLog("Histórico atualizado.");
    }
}
