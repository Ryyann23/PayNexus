using PayNexus.Models;
using PayNexus.Utils;

namespace PayNexus.Adapters;

public sealed class CartaoGatewayAdapter : IGatewayPagamentoAdapter
{
    private readonly AdquirenteCartaoGateway _gateway;

    public CartaoGatewayAdapter()
        : this(new AdquirenteCartaoGateway())
    {
    }

    private CartaoGatewayAdapter(AdquirenteCartaoGateway gateway)
    {
        _gateway = gateway;
    }

    public string NomeGateway => "Adquirente de Cartao";

    public ResultadoPagamento Processar(Pagamento pagamento)
    {
        var valorCentavos = decimal.ToInt32(pagamento.Valor * 100);
        var autorizacao = _gateway.AutorizarCompra(
            pagamento.Cliente.Nome,
            valorCentavos);

        var status = autorizacao.Aprovada
            ? StatusPagamento.Aprovado
            : StatusPagamento.Recusado;

        return new ResultadoPagamento(
            status,
            $"{autorizacao.Descricao} Adapter utilizado: {NomeGateway}.",
            autorizacao.CodigoAutorizacao);
    }

    private sealed class AdquirenteCartaoGateway
    {
        public CartaoAutorizacao AutorizarCompra(string titular, int valorCentavos)
        {
            if (valorCentavos > 1_000_000)
            {
                return new CartaoAutorizacao(
                    false,
                    TransactionCodeGenerator.Generate("CRD"),
                    $"Pagamento no cartao de {titular} recusado pela validacao antifraude simulada.");
            }

            return new CartaoAutorizacao(
                true,
                TransactionCodeGenerator.Generate("CRD"),
                $"Pagamento no cartao de {titular} aprovado apos autorizacao simulada.");
        }
    }

    private sealed record CartaoAutorizacao(
        bool Aprovada,
        string CodigoAutorizacao,
        string Descricao);
}
