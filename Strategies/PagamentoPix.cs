using PayNexus.Models;
using PayNexus.Utils;

namespace PayNexus.Strategies;

public sealed class PagamentoPix : IEstrategiaPagamento
{
    public string Metodo => "PIX";

    public ResultadoPagamento Processar(Pagamento pagamento)
    {
        return new ResultadoPagamento(
            StatusPagamento.Aprovado,
            "Pagamento PIX aprovado.",
            TransactionCodeGenerator.Generate("PIX"));
    }
}
