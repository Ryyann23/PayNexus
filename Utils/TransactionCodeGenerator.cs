namespace PayNexus.Utils;

public static class TransactionCodeGenerator
{
    public static string Generate(string prefix)
    {
        return $"{prefix}-{DateTime.Now:yyyyMMdd}-{Guid.NewGuid().ToString("N")[..6].ToUpperInvariant()}";
    }
}
