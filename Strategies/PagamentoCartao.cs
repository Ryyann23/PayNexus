using PayNexus.Models;
using PayNexus.Adapters;

namespace PayNexus.Strategies;

public sealed class PagamentoCartao : IEstrategiaPagamento
{
    private readonly IGatewayPagamentoAdapter _gatewayAdapter;

    public PagamentoCartao()
        : this(new CartaoGatewayAdapter())
    {
    }

    public PagamentoCartao(IGatewayPagamentoAdapter gatewayAdapter)
    {
        _gatewayAdapter = gatewayAdapter ?? throw new ArgumentNullException(nameof(gatewayAdapter));
    }

    public string Metodo => "Cartão de Crédito";

    public ResultadoPagamento Processar(Pagamento pagamento)
    {
        pagamento.AdicionarEvento($"Strategy Cartao direcionou o pagamento para o adapter {_gatewayAdapter.NomeGateway}.");
        return _gatewayAdapter.Processar(pagamento);
    }
}
