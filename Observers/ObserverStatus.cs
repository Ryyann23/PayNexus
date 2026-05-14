using PayNexus.Models;

namespace PayNexus.Observers;

public sealed class ObserverStatus : IObserverPagamento
{
    public string Nome => "Status";

    public void Atualizar(Pagamento pagamento, Action<string> registrarLog)
    {
        var statusOperacional = pagamento.Status switch
        {
            StatusPagamento.Aprovado => "Concluído",
            StatusPagamento.Pendente => "Aguardando compensação",
            StatusPagamento.Recusado => "Revisar pagamento",
            _ => "Indefinido"
        };

        pagamento.AtualizarStatusOperacional(statusOperacional);
        registrarLog("Observer de Status executado.");
    }
}
