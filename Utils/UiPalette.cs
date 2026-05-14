using PayNexus.Models;

namespace PayNexus.Utils;

public static class UiPalette
{
    public static readonly Color Background = FromHex("#F7F4FF");
    public static readonly Color Surface = Color.White;
    public static readonly Color Primary = FromHex("#7C3AED");
    public static readonly Color Secondary = FromHex("#A855F7");
    public static readonly Color PurpleSoft = FromHex("#F1E7FF");
    public static readonly Color PurpleSelection = FromHex("#EDE9FE");
    public static readonly Color Success = FromHex("#16A34A");
    public static readonly Color SuccessSoft = FromHex("#DCFCE7");
    public static readonly Color Danger = FromHex("#DC2626");
    public static readonly Color DangerSoft = FromHex("#FEE2E2");
    public static readonly Color Warning = FromHex("#D97706");
    public static readonly Color WarningSoft = FromHex("#FEF3C7");
    public static readonly Color Text = FromHex("#24143D");
    public static readonly Color MutedText = FromHex("#6D5B82");
    public static readonly Color Border = FromHex("#E7DDF8");
    public static readonly Color Shadow = Color.FromArgb(34, 91, 33, 182);

    public static Color FromHex(string hex)
    {
        return ColorTranslator.FromHtml(hex);
    }

    public static Color BadgeBackColor(StatusPagamento status)
    {
        return status switch
        {
            StatusPagamento.Aprovado => SuccessSoft,
            StatusPagamento.Pendente => WarningSoft,
            StatusPagamento.Recusado => DangerSoft,
            _ => Border
        };
    }

    public static Color BadgeForeColor(StatusPagamento status)
    {
        return status switch
        {
            StatusPagamento.Aprovado => Success,
            StatusPagamento.Pendente => Warning,
            StatusPagamento.Recusado => Danger,
            _ => MutedText
        };
    }

    public static Color Blend(Color source, Color target, double amount)
    {
        amount = Math.Clamp(amount, 0, 1);

        return Color.FromArgb(
            (int)(source.A + (target.A - source.A) * amount),
            (int)(source.R + (target.R - source.R) * amount),
            (int)(source.G + (target.G - source.G) * amount),
            (int)(source.B + (target.B - source.B) * amount));
    }

    public static Color ResolveBackColor(Control? control)
    {
        while (control is not null)
        {
            if (control.BackColor != Color.Transparent && control.BackColor.A > 0)
            {
                return control.BackColor;
            }

            control = control.Parent;
        }

        return Background;
    }
}
