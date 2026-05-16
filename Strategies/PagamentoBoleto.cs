using PayNexus.Models;
using PayNexus.Adapters;

namespace PayNexus.Strategies;

public sealed class PagamentoBoleto : IEstrategiaPagamento
{
    private readonly IGatewayPagamentoAdapter _gatewayAdapter;

    public PagamentoBoleto()
        : this(new BoletoGatewayAdapter())
    {
    }

    public PagamentoBoleto(IGatewayPagamentoAdapter gatewayAdapter)
    {
        _gatewayAdapter = gatewayAdapter ?? throw new ArgumentNullException(nameof(gatewayAdapter));
    }

    public string Metodo => "Boleto";

    public ResultadoPagamento Processar(Pagamento pagamento)
    {
        pagamento.AdicionarEvento($"Strategy Boleto direcionou o pagamento para o adapter {_gatewayAdapter.NomeGateway}.");
        return _gatewayAdapter.Processar(pagamento);
    }
}
