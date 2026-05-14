using System.ComponentModel;
using System.Drawing.Drawing2D;
using PayNexus.Utils;

namespace PayNexus.Components;

public class SurfacePanel : Panel
{
    public SurfacePanel()
    {
        SetStyle(
            ControlStyles.AllPaintingInWmPaint |
            ControlStyles.UserPaint |
            ControlStyles.OptimizedDoubleBuffer |
            ControlStyles.ResizeRedraw,
            true);

        BackColor = Color.Transparent;
        SurfaceColor = UiPalette.Surface;
        ShadowColor = UiPalette.Shadow;
    }

    [DefaultValue(18)]
    public int CornerRadius { get; set; } = 18;

    [DefaultValue(true)]
    public bool ShowShadow { get; set; } = true;

    public Color SurfaceColor { get; set; }

    public Color ShadowColor { get; set; }

    protected override void OnPaintBackground(PaintEventArgs e)
    {
        e.Graphics.Clear(UiPalette.ResolveBackColor(Parent));
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);

        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

        var surfaceRectangle = new Rectangle(0, 0, Width - 8, Height - 8);
        if (surfaceRectangle.Width <= 0 || surfaceRectangle.Height <= 0)
        {
            return;
        }

        if (ShowShadow)
        {
            using var shadowPath = new Rectangle(3, 4, Width - 9, Height - 9).ToRoundedPath(CornerRadius);
            using var shadowBrush = new SolidBrush(ShadowColor);
            e.Graphics.FillPath(shadowBrush, shadowPath);
        }

        using var surfacePath = surfaceRectangle.ToRoundedPath(CornerRadius);
        using var surfaceBrush = new SolidBrush(SurfaceColor);
        e.Graphics.FillPath(surfaceBrush, surfacePath);
    }
}
