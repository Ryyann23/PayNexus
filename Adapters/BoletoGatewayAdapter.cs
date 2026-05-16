using PayNexus.Models;
using PayNexus.Utils;

namespace PayNexus.Adapters;

public sealed class BoletoGatewayAdapter : IGatewayPagamentoAdapter
{
    private readonly BancoBoletoGateway _gateway;

    public BoletoGatewayAdapter()
        : this(new BancoBoletoGateway())
    {
    }

    private BoletoGatewayAdapter(BancoBoletoGateway gateway)
    {
        _gateway = gateway;
    }

    public string NomeGateway => "Banco de Boletos";

    public ResultadoPagamento Processar(Pagamento pagamento)
    {
        var boleto = _gateway.EmitirBoleto(
            pagamento.Cliente.Nome,
            pagamento.Valor,
            DateTime.Today.AddDays(3));

        return new ResultadoPagamento(
            StatusPagamento.Pendente,
            $"{boleto.Descricao} Adapter utilizado: {NomeGateway}.",
            boleto.LinhaDigitavel);
    }

    private sealed class BancoBoletoGateway
    {
        public BoletoEmitido EmitirBoleto(string sacado, decimal valor, DateTime vencimento)
        {
            return new BoletoEmitido(
                TransactionCodeGenerator.Generate("BOL"),
                $"Boleto emitido para {sacado} no valor de {valor:C}, com vencimento em {vencimento:dd/MM/yyyy}.");
        }
    }

    private sealed record BoletoEmitido(
        string LinhaDigitavel,
        string Descricao);
}
