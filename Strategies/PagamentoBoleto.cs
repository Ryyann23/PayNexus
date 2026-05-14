using PayNexus.Models;
using PayNexus.Utils;

namespace PayNexus.Strategies;

public sealed class PagamentoBoleto : IEstrategiaPagamento
{
    public string Metodo => "Boleto";

    public ResultadoPagamento Processar(Pagamento pagamento)
    {
        return new ResultadoPagamento(
            StatusPagamento.Pendente,
            "Boleto gerado e aguardando compensação.",
            TransactionCodeGenerator.Generate("BOL"));
    }
}
