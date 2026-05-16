using PayNexus.Models;

namespace PayNexus.Adapters;

public interface IGatewayPagamentoAdapter
{
    string NomeGateway { get; }

    ResultadoPagamento Processar(Pagamento pagamento);
}
