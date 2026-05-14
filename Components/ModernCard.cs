using System.Drawing.Drawing2D;
using PayNexus.Utils;

namespace PayNexus.Components;

public sealed class ModernCard : SurfacePanel
{
    private readonly string _title;
    private readonly string _iconText;
    private string _value;

    public ModernCard(string title, string value, string caption, Color accentColor, string iconText)
    {
        _title = title;
        _value = value;
        _iconText = iconText;

        AccentColor = accentColor;
        SurfaceColor = accentColor;
        ShadowColor = Color.FromArgb(42, accentColor);
        CornerRadius = 22;
        MinimumSize = new Size(240, 150);
        Margin = new Padding(0, 0, 16, 0);
    }

    public Color AccentColor { get; set; }

    public void UpdateValue(string value, string? caption = null)
    {
        _value = value;

        Invalidate();
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);

        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
        e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

        var cardBounds = new Rectangle(0, 0, Width - 8, Height - 8);
        if (cardBounds.Width <= 0 || cardBounds.Height <= 0)
        {
            return;
        }

        PaintCardSurface(e.Graphics, cardBounds);
        PaintIcon(e.Graphics);
        PaintText(e.Graphics, cardBounds);
    }

    private void PaintCardSurface(Graphics graphics, Rectangle cardBounds)
    {
        using var path = cardBounds.ToRoundedPath(CornerRadius);
        using var brush = new LinearGradientBrush(
            cardBounds,
            AccentColor,
            UiPalette.Secondary,
            LinearGradientMode.ForwardDiagonal);

        graphics.FillPath(brush, path);
    }

    private void PaintIcon(Graphics graphics)
    {
        var iconBounds = new Rectangle(24, 30, 46, 46);
        using var path = iconBounds.ToRoundedPath(14);
        using var brush = new SolidBrush(Color.FromArgb(44, Color.White));
        using var borderPen = new Pen(Color.FromArgb(72, Color.White));

        graphics.FillPath(brush, path);
        graphics.DrawPath(borderPen, path);

        TextRenderer.DrawText(
            graphics,
            _iconText,
            UiFonts.Medium(10.5f),
            iconBounds,
            Color.White,
            TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
    }

    private void PaintText(Graphics graphics, Rectangle cardBounds)
    {
        var rightLimit = Math.Max(140, cardBounds.Width - 28);
        var titleBounds = new Rectangle(84, 24, Math.Max(120, rightLimit - 84), 58);
        var valueBounds = new RectangleF(24, 92, rightLimit - 24, 52);

        TextRenderer.DrawText(
            graphics,
            _title,
            UiFonts.Medium(10f),
            titleBounds,
            Color.FromArgb(238, Color.White),
            TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.WordBreak | TextFormatFlags.EndEllipsis);

        using var valueBrush = new SolidBrush(Color.White);
        using var valueFont = UiFonts.Title(22f);
        using var valueFormat = new StringFormat
        {
            Alignment = StringAlignment.Near,
            LineAlignment = StringAlignment.Center,
            Trimming = StringTrimming.EllipsisCharacter,
            FormatFlags = StringFormatFlags.NoWrap
        };

        graphics.DrawString(_value, valueFont, valueBrush, valueBounds, valueFormat);
    }
}
