namespace PayNexus.Models;

public sealed record PagamentoResumo(
    decimal TotalProcessado,
    int TransacoesHoje,
    int PagamentosAprovados,
    int PagamentosPendentes);
