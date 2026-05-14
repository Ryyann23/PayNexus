using System.Drawing.Drawing2D;
using PayNexus.Models;
using PayNexus.Utils;

namespace PayNexus.Components;

public sealed class StatusBadge : Control
{
    private StatusPagamento _status;

    public StatusBadge()
    {
        SetStyle(
            ControlStyles.AllPaintingInWmPaint |
            ControlStyles.UserPaint |
            ControlStyles.OptimizedDoubleBuffer |
            ControlStyles.ResizeRedraw,
            true);

        Font = UiFonts.Medium(8.5f);
        Size = new Size(92, 30);
    }

    public StatusPagamento Status
    {
        get => _status;
        set
        {
            _status = value;
            Invalidate();
        }
    }

    public static string GetStatusText(StatusPagamento status)
    {
        return status switch
        {
            StatusPagamento.Aprovado => "Aprovado",
            StatusPagamento.Pendente => "Pendente",
            StatusPagamento.Recusado => "Recusado",
            _ => "Indefinido"
        };
    }

    public static void PaintBadge(Graphics graphics, Rectangle bounds, StatusPagamento status)
    {
        graphics.SmoothingMode = SmoothingMode.AntiAlias;

        var text = GetStatusText(status);
        var textSize = TextRenderer.MeasureText(text, UiFonts.Medium(8.5f));
        var badgeWidth = Math.Min(bounds.Width - 10, Math.Max(86, textSize.Width + 28));
        var badgeHeight = 28;
        var badgeRectangle = new Rectangle(
            bounds.Left + (bounds.Width - badgeWidth) / 2,
            bounds.Top + (bounds.Height - badgeHeight) / 2,
            badgeWidth,
            badgeHeight);

        using var path = badgeRectangle.ToRoundedPath(14);
        using var backgroundBrush = new SolidBrush(UiPalette.BadgeBackColor(status));
        graphics.FillPath(backgroundBrush, path);

        TextRenderer.DrawText(
            graphics,
            text,
            UiFonts.Medium(8.5f),
            badgeRectangle,
            UiPalette.BadgeForeColor(status),
            TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        PaintBadge(e.Graphics, ClientRectangle, Status);
    }
}
