using PayNexus.Models;

namespace PayNexus.Strategies;

public interface IEstrategiaPagamento
{
    string Metodo { get; }

    ResultadoPagamento Processar(Pagamento pagamento);
}
