namespace PayNexus.Models;

public sealed class Cliente
{
    public Cliente(string nome)
    {
        if (string.IsNullOrWhiteSpace(nome))
        {
            throw new ArgumentException("O nome do cliente é obrigatório.", nameof(nome));
        }

        Nome = nome.Trim();
    }

    public string Id { get; } = $"CLI-{Guid.NewGuid().ToString("N")[..8].ToUpperInvariant()}";

    public string Nome { get; }
}
