namespace PayNexus.Models;

public sealed record ResultadoPagamento(
    StatusPagamento Status,
    string Mensagem,
    string CodigoTransacao);
