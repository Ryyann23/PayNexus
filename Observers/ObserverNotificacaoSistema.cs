using PayNexus.Models;

namespace PayNexus.Observers;

public sealed class ObserverNotificacaoSistema : IObserverPagamento
{
    public string Nome => "Notificações do Sistema";

    public void Atualizar(Pagamento pagamento, Action<string> registrarLog)
    {
        pagamento.AdicionarEvento("Notificação interna enviada para o painel operacional.");
        registrarLog("Notificação do sistema enviada.");
    }
}
