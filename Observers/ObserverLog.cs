using PayNexus.Models;

namespace PayNexus.Observers;

public sealed class ObserverLog : IObserverPagamento
{
    public string Nome => "Log";

    public void Atualizar(Pagamento pagamento, Action<string> registrarLog)
    {
        pagamento.AdicionarEvento($"Log gravado para a transação {pagamento.CodigoTransacao}.");
        registrarLog("Transação registrada com sucesso.");
    }
}
