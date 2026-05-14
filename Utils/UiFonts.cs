namespace PayNexus.Utils;

public static class UiFonts
{
    public static Font Regular(float size)
    {
        return new Font("Segoe UI", size, FontStyle.Regular);
    }

    public static Font Medium(float size)
    {
        return new Font("Segoe UI", size, FontStyle.Bold);
    }

    public static Font Title(float size)
    {
        return new Font("Segoe UI", size, FontStyle.Bold);
    }
}
