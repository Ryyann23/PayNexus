using PayNexus.Models;
using PayNexus.Utils;

namespace PayNexus.Adapters;

public sealed class PixGatewayAdapter : IGatewayPagamentoAdapter
{
    private readonly BancoPixGateway _gateway;

    public PixGatewayAdapter()
        : this(new BancoPixGateway())
    {
    }

    private PixGatewayAdapter(BancoPixGateway gateway)
    {
        _gateway = gateway;
    }

    public string NomeGateway => "Banco PIX";

    public ResultadoPagamento Processar(Pagamento pagamento)
    {
        var retorno = _gateway.ConfirmarTransferencia(
            pagamento.Cliente.Id,
            pagamento.Valor);

        var status = retorno.Confirmado
            ? StatusPagamento.Aprovado
            : StatusPagamento.Recusado;

        return new ResultadoPagamento(
            status,
            $"{retorno.Mensagem} Adapter utilizado: {NomeGateway}.",
            retorno.Protocolo);
    }

    private sealed class BancoPixGateway
    {
        public PixRetorno ConfirmarTransferencia(string chaveCliente, decimal valor)
        {
            return new PixRetorno(
                true,
                TransactionCodeGenerator.Generate("PIX"),
                $"Pagamento PIX aprovado para {chaveCliente} no valor de {valor:C}.");
        }
    }

    private sealed record PixRetorno(
        bool Confirmado,
        string Protocolo,
        string Mensagem);
}
