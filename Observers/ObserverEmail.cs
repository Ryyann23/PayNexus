using PayNexus.Models;

namespace PayNexus.Observers;

public sealed class ObserverEmail : IObserverPagamento
{
    public string Nome => "Email";

    public void Atualizar(Pagamento pagamento, Action<string> registrarLog)
    {
        pagamento.AdicionarEvento($"Comprovante enviado para {pagamento.Cliente.Nome}.");
        registrarLog("Observer de Email executado.");
    }
}
