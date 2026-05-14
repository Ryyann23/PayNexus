using System.ComponentModel;
using System.Drawing.Drawing2D;
using PayNexus.Utils;

namespace PayNexus.Components;

public sealed class ModernButton : Button
{
    private readonly System.Windows.Forms.Timer _animationTimer;
    private Color _currentColor;
    private Color _startColor;
    private Color _targetColor;
    private Color _fillColor = UiPalette.Primary;
    private Color _hoverColor = UiPalette.Secondary;
    private Color _disabledColor = UiPalette.Border;
    private double _animationProgress;

    public ModernButton()
    {
        SetStyle(
            ControlStyles.AllPaintingInWmPaint |
            ControlStyles.UserPaint |
            ControlStyles.OptimizedDoubleBuffer |
            ControlStyles.ResizeRedraw,
            true);

        FlatStyle = FlatStyle.Flat;
        FlatAppearance.BorderSize = 0;
        Cursor = Cursors.Hand;
        Height = 44;
        Font = UiFonts.Medium(9.5f);
        ForeColor = Color.White;
        _currentColor = _fillColor;
        _targetColor = _fillColor;

        _animationTimer = new System.Windows.Forms.Timer { Interval = 12 };
        _animationTimer.Tick += HandleAnimationTick;
    }

    [DefaultValue(14)]
    public int BorderRadius { get; set; } = 14;

    public Color FillColor
    {
        get => _fillColor;
        set
        {
            _fillColor = value;

            if (!_animationTimer.Enabled)
            {
                _currentColor = value;
                _targetColor = value;
            }

            Invalidate();
        }
    }

    public Color HoverColor
    {
        get => _hoverColor;
        set => _hoverColor = value;
    }

    public Color DisabledColor
    {
        get => _disabledColor;
        set
        {
            _disabledColor = value;

            if (!Enabled)
            {
                _currentColor = value;
                Invalidate();
            }
        }
    }

    public Color BorderColor { get; set; } = Color.Transparent;

    [DefaultValue(false)]
    public bool Outline { get; set; }

    protected override void OnMouseEnter(EventArgs e)
    {
        base.OnMouseEnter(e);
        AnimateTo(HoverColor);
    }

    protected override void OnMouseLeave(EventArgs e)
    {
        base.OnMouseLeave(e);
        AnimateTo(FillColor);
    }

    protected override void OnMouseDown(MouseEventArgs e)
    {
        base.OnMouseDown(e);
        AnimateTo(UiPalette.Blend(FillColor, Color.Black, 0.12));
    }

    protected override void OnMouseUp(MouseEventArgs e)
    {
        base.OnMouseUp(e);
        AnimateTo(ClientRectangle.Contains(PointToClient(Cursor.Position)) ? HoverColor : FillColor);
    }

    protected override void OnEnabledChanged(EventArgs e)
    {
        base.OnEnabledChanged(e);
        _currentColor = Enabled ? FillColor : DisabledColor;
        Invalidate();
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
        e.Graphics.Clear(UiPalette.ResolveBackColor(Parent));

        var rectangle = new Rectangle(0, 0, Width - 1, Height - 1);
        using var path = rectangle.ToRoundedPath(BorderRadius);
        using var backgroundBrush = new SolidBrush(Enabled ? _currentColor : DisabledColor);
        using var borderPen = new Pen(BorderColor);

        e.Graphics.FillPath(backgroundBrush, path);

        if (Outline || BorderColor != Color.Transparent)
        {
            e.Graphics.DrawPath(borderPen, path);
        }

        TextRenderer.DrawText(
            e.Graphics,
            Text,
            Font,
            ClientRectangle,
            Enabled ? ForeColor : UiPalette.MutedText,
            TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _animationTimer.Dispose();
        }

        base.Dispose(disposing);
    }

    private void AnimateTo(Color color)
    {
        if (!Enabled)
        {
            return;
        }

        _startColor = _currentColor;
        _targetColor = color;
        _animationProgress = 0;
        _animationTimer.Start();
    }

    private void HandleAnimationTick(object? sender, EventArgs e)
    {
        _animationProgress += 0.18;
        _currentColor = UiPalette.Blend(_startColor, _targetColor, _animationProgress);
        Invalidate();

        if (_animationProgress >= 1)
        {
            _currentColor = _targetColor;
            _animationTimer.Stop();
        }
    }
}
