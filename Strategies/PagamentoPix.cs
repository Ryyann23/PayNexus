using PayNexus.Models;
using PayNexus.Adapters;

namespace PayNexus.Strategies;

public sealed class PagamentoPix : IEstrategiaPagamento
{
    private readonly IGatewayPagamentoAdapter _gatewayAdapter;

    public PagamentoPix()
        : this(new PixGatewayAdapter())
    {
    }

    public PagamentoPix(IGatewayPagamentoAdapter gatewayAdapter)
    {
        _gatewayAdapter = gatewayAdapter ?? throw new ArgumentNullException(nameof(gatewayAdapter));
    }

    public string Metodo => "PIX";

    public ResultadoPagamento Processar(Pagamento pagamento)
    {
        pagamento.AdicionarEvento($"Strategy PIX direcionou o pagamento para o adapter {_gatewayAdapter.NomeGateway}.");
        return _gatewayAdapter.Processar(pagamento);
    }
}
