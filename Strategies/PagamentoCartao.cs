using PayNexus.Models;
using PayNexus.Utils;

namespace PayNexus.Strategies;

public sealed class PagamentoCartao : IEstrategiaPagamento
{
    public string Metodo => "Cartão de Crédito";

    public ResultadoPagamento Processar(Pagamento pagamento)
    {
        if (pagamento.Valor > 10000)
        {
            return new ResultadoPagamento(
                StatusPagamento.Recusado,
                "Pagamento no cartão recusado pela validação antifraude simulada.",
                TransactionCodeGenerator.Generate("CRD"));
        }

        return new ResultadoPagamento(
            StatusPagamento.Aprovado,
            "Pagamento no cartão aprovado após validação simulada.",
            TransactionCodeGenerator.Generate("CRD"));
    }
}
