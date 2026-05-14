using PayNexus.Models;

namespace PayNexus.Observers;

public interface IObserverPagamento
{
    string Nome { get; }

    void Atualizar(Pagamento pagamento, Action<string> registrarLog);
}
