using System.Drawing.Drawing2D;
using System.Drawing.Text;
using PayNexus.Utils;

namespace PayNexus.Components;

public sealed class PaymentAnimationView : Control
{
    private const int StepDurationMs = 900;
    private const int FinalHoldMs = 550;

    private readonly System.Windows.Forms.Timer _timer;
    private readonly Font _titleFont = UiFonts.Title(18.5f);
    private readonly Font _subtitleFont = UiFonts.Regular(9.5f);
    private readonly Font _messageFont = UiFonts.Medium(10.6f);
    private readonly Font _pixFont = UiFonts.Title(13f);
    private AnimationDescriptor _descriptor = AnimationDescriptor.ForPix();
    private int _elapsedMs;

    public PaymentAnimationView()
    {
        SetStyle(
            ControlStyles.AllPaintingInWmPaint |
            ControlStyles.UserPaint |
            ControlStyles.OptimizedDoubleBuffer |
            ControlStyles.ResizeRedraw |
            ControlStyles.SupportsTransparentBackColor,
            true);

        BackColor = Color.Transparent;
        Font = UiFonts.Regular(10f);

        _timer = new System.Windows.Forms.Timer { Interval = 16 };
        _timer.Tick += HandleTimerTick;
    }

    public event EventHandler? AnimationCompleted;

    private int TotalDurationMs => StepDurationMs * _descriptor.Messages.Length + FinalHoldMs;

    private float Progress => Math.Clamp(_elapsedMs / (float)TotalDurationMs, 0f, 1f);

    private int CurrentStep => Math.Min(_elapsedMs / StepDurationMs, _descriptor.Messages.Length - 1);

    public void Start(string metodoPagamento)
    {
        _descriptor = AnimationDescriptor.FromMethod(metodoPagamento);
        _elapsedMs = 0;
        _timer.Start();
        Invalidate();
    }

    protected override void OnPaintBackground(PaintEventArgs e)
    {
        e.Graphics.Clear(Color.White);
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);

        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
        e.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

        var contentBounds = new Rectangle(0, 0, Width, Height);
        if (contentBounds.Width <= 0 || contentBounds.Height <= 0)
        {
            return;
        }

        PaintHeader(e.Graphics, contentBounds);
        PaintAnimationStage(e.Graphics, contentBounds);
        PaintProgress(e.Graphics, contentBounds);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _timer.Dispose();
            _titleFont.Dispose();
            _subtitleFont.Dispose();
            _messageFont.Dispose();
            _pixFont.Dispose();
        }

        base.Dispose(disposing);
    }

    private void HandleTimerTick(object? sender, EventArgs e)
    {
        _elapsedMs += _timer.Interval;

        if (_elapsedMs >= TotalDurationMs)
        {
            _elapsedMs = TotalDurationMs;
            _timer.Stop();
            Invalidate();
            AnimationCompleted?.Invoke(this, EventArgs.Empty);
            return;
        }

        Invalidate();
    }

    private void PaintHeader(Graphics graphics, Rectangle bounds)
    {
        var titleBounds = new Rectangle(10, 16, bounds.Width - 20, 34);
        var subtitleBounds = new Rectangle(10, 48, bounds.Width - 20, 28);

        TextRenderer.DrawText(
            graphics,
            "Processando pagamento",
            _titleFont,
            titleBounds,
            UiPalette.Text,
            TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);

        TextRenderer.DrawText(
            graphics,
            _descriptor.Title,
            _subtitleFont,
            subtitleBounds,
            UiPalette.MutedText,
            TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
    }

    private void PaintAnimationStage(Graphics graphics, Rectangle bounds)
    {
        var stageWidth = Math.Min(360, Math.Max(260, bounds.Width - 44));
        var stageHeight = 190;
        var stage = new Rectangle(
            (bounds.Width - stageWidth) / 2,
            Math.Max(92, bounds.Height / 2 - 122),
            stageWidth,
            stageHeight);

        using var shadowPath = new Rectangle(stage.X + 3, stage.Y + 6, stage.Width, stage.Height).ToRoundedPath(24);
        using var shadowBrush = new SolidBrush(Color.FromArgb(22, _descriptor.AccentColor));
        graphics.FillPath(shadowBrush, shadowPath);

        using var stagePath = stage.ToRoundedPath(24);
        using var stageBrush = new LinearGradientBrush(
            stage,
            Color.White,
            UiPalette.Blend(_descriptor.SoftColor, Color.White, 0.34),
            LinearGradientMode.ForwardDiagonal);
        using var borderPen = new Pen(UiPalette.Blend(_descriptor.AccentColor, Color.White, 0.62), 1.4f);

        graphics.FillPath(stageBrush, stagePath);
        graphics.DrawPath(borderPen, stagePath);

        switch (_descriptor.Kind)
        {
            case AnimationKind.Pix:
                PaintPix(graphics, stage);
                break;
            case AnimationKind.CreditCard:
                PaintCreditCard(graphics, stage);
                break;
            default:
                PaintBoleto(graphics, stage);
                break;
        }

        PaintStepMessage(graphics, bounds, stage.Bottom + 18);
    }

    private void PaintPix(Graphics graphics, Rectangle stage)
    {
        var center = new PointF(stage.Left + stage.Width / 2f, stage.Top + 78);
        var card = new RectangleF(center.X - 108, center.Y - 52, 216, 104);
        using var cardPath = card.ToRoundedPath(20);
        using var cardBrush = new SolidBrush(Color.White);
        using var cardPen = new Pen(UiPalette.Blend(_descriptor.AccentColor, Color.White, 0.42), 1.5f);

        graphics.FillPath(cardBrush, cardPath);
        graphics.DrawPath(cardPen, cardPath);

        var diamond = new[]
        {
            new PointF(center.X, center.Y - 34),
            new PointF(center.X + 34, center.Y),
            new PointF(center.X, center.Y + 34),
            new PointF(center.X - 34, center.Y)
        };

        using var diamondBrush = new SolidBrush(_descriptor.AccentColor);
        using var diamondPen = new Pen(Color.FromArgb(80, Color.White), 2f);
        graphics.FillPolygon(diamondBrush, diamond);
        graphics.DrawPolygon(diamondPen, diamond);

        TextRenderer.DrawText(
            graphics,
            "PIX",
            _pixFont,
            Rectangle.Round(new RectangleF(center.X - 28, center.Y - 13, 56, 26)),
            Color.White,
            TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);

        var arcBounds = new RectangleF(center.X - 78, center.Y - 78, 156, 156);
        using var basePen = new Pen(Color.FromArgb(34, _descriptor.AccentColor), 6f);
        using var arcPen = new Pen(_descriptor.AccentColor, 6f)
        {
            StartCap = LineCap.Round,
            EndCap = LineCap.Round
        };

        graphics.DrawArc(basePen, arcBounds, 0, 360);
        graphics.DrawArc(arcPen, arcBounds, _elapsedMs * 0.28f, 96);
    }

    private void PaintCreditCard(Graphics graphics, Rectangle stage)
    {
        var reader = new Rectangle(stage.Right - 94, stage.Top + 47, 44, 96);
        using (var readerPath = reader.ToRoundedPath(16))
        using (var readerBrush = new SolidBrush(UiPalette.FromHex("#111827")))
        {
            graphics.FillPath(readerBrush, readerPath);
        }

        using (var slotPen = new Pen(Color.FromArgb(120, Color.White), 3f)
        {
            StartCap = LineCap.Round,
            EndCap = LineCap.Round
        })
        {
            graphics.DrawLine(slotPen, reader.Left + 10, reader.Top + 24, reader.Right - 10, reader.Top + 24);
        }

        var passProgress = EaseInOut(Math.Clamp(_elapsedMs / (StepDurationMs * 2.35f), 0f, 1f));
        var cardWidth = 176;
        var cardHeight = 104;
        var cardX = Lerp(stage.Left - 54, reader.Left - cardWidth + 16, passProgress);
        var cardY = stage.Top + 48 + (float)Math.Sin(_elapsedMs / 190d) * 3f;
        var cardBounds = new RectangleF(cardX, cardY, cardWidth, cardHeight);

        var state = graphics.Save();
        graphics.TranslateTransform(cardBounds.X + cardBounds.Width / 2, cardBounds.Y + cardBounds.Height / 2);
        graphics.RotateTransform(Lerp(-7f, -1.5f, passProgress));
        graphics.TranslateTransform(-(cardBounds.X + cardBounds.Width / 2), -(cardBounds.Y + cardBounds.Height / 2));

        using (var shadowPath = new RectangleF(cardBounds.X + 4, cardBounds.Y + 7, cardBounds.Width, cardBounds.Height).ToRoundedPath(18))
        using (var shadowBrush = new SolidBrush(Color.FromArgb(36, 17, 24, 39)))
        {
            graphics.FillPath(shadowBrush, shadowPath);
        }

        using (var cardPath = cardBounds.ToRoundedPath(18))
        using (var cardBrush = new LinearGradientBrush(cardBounds, _descriptor.AccentColor, UiPalette.FromHex("#312E81"), LinearGradientMode.ForwardDiagonal))
        {
            graphics.FillPath(cardBrush, cardPath);
        }

        using (var chipBrush = new SolidBrush(UiPalette.FromHex("#FACC15")))
        using (var chipPath = new RectangleF(cardBounds.X + 18, cardBounds.Y + 23, 34, 24).ToRoundedPath(7))
        {
            graphics.FillPath(chipBrush, chipPath);
        }

        using (var linePen = new Pen(Color.FromArgb(120, Color.White), 2f)
        {
            StartCap = LineCap.Round,
            EndCap = LineCap.Round
        })
        {
            graphics.DrawLine(linePen, cardBounds.X + 18, cardBounds.Bottom - 28, cardBounds.X + 88, cardBounds.Bottom - 28);
            graphics.DrawLine(linePen, cardBounds.X + 18, cardBounds.Bottom - 17, cardBounds.X + 132, cardBounds.Bottom - 17);
        }

        graphics.Restore(state);

        using var signalPen = new Pen(_descriptor.AccentColor, 2.3f)
        {
            StartCap = LineCap.Round,
            EndCap = LineCap.Round
        };

        var signalCenter = new PointF(reader.Left - 8, reader.Top + 50);
        graphics.DrawArc(signalPen, signalCenter.X - 18, signalCenter.Y - 18, 36, 36, -45, 90);
        graphics.DrawArc(signalPen, signalCenter.X - 30, signalCenter.Y - 30, 60, 60, -45, 90);
    }

    private void PaintBoleto(Graphics graphics, Rectangle stage)
    {
        var doc = new Rectangle(stage.Left + stage.Width / 2 - 115, stage.Top + 26, 230, 138);
        using var shadowPath = new Rectangle(doc.X + 4, doc.Y + 5, doc.Width, doc.Height).ToRoundedPath(18);
        using var shadowBrush = new SolidBrush(Color.FromArgb(24, _descriptor.AccentColor));
        graphics.FillPath(shadowBrush, shadowPath);

        using var docPath = doc.ToRoundedPath(18);
        using var docBrush = new SolidBrush(Color.White);
        using var docPen = new Pen(UiPalette.Blend(_descriptor.AccentColor, Color.White, 0.48), 1.4f);
        graphics.FillPath(docBrush, docPath);
        graphics.DrawPath(docPen, docPath);

        using var accentBrush = new SolidBrush(_descriptor.AccentColor);
        graphics.FillRectangle(accentBrush, new Rectangle(doc.Left, doc.Top, doc.Width, 24));

        using var dottedPen = new Pen(UiPalette.Border, 1f) { DashPattern = [2f, 4f] };
        graphics.DrawLine(dottedPen, doc.Left + 16, doc.Top + 42, doc.Right - 16, doc.Top + 42);

        using var textPen = new Pen(UiPalette.FromHex("#CBD5E1"), 3f)
        {
            StartCap = LineCap.Round,
            EndCap = LineCap.Round
        };

        graphics.DrawLine(textPen, doc.Left + 22, doc.Top + 60, doc.Right - 34, doc.Top + 60);
        graphics.DrawLine(textPen, doc.Left + 22, doc.Top + 78, doc.Right - 58, doc.Top + 78);
        graphics.DrawLine(textPen, doc.Left + 22, doc.Top + 96, doc.Right - 44, doc.Top + 96);

        var barcodeX = doc.Left + 24;
        var barcodeTop = doc.Bottom - 28;
        var barcodeHeights = new[] { 22, 14, 20, 12, 24, 16, 21, 13, 22, 18, 24, 14, 20, 12 };
        using var barcodeBrush = new SolidBrush(UiPalette.Text);

        for (var i = 0; i < barcodeHeights.Length; i++)
        {
            var width = i % 3 == 0 ? 4 : 2;
            graphics.FillRectangle(barcodeBrush, barcodeX, barcodeTop + 24 - barcodeHeights[i], width, barcodeHeights[i]);
            barcodeX += width + 5;
        }
    }

    private void PaintStepMessage(Graphics graphics, Rectangle bounds, int top)
    {
        var message = _descriptor.Messages[CurrentStep];
        var messageBounds = new Rectangle(24, top, bounds.Width - 48, 34);
        var dotBounds = new Rectangle(bounds.Width / 2 - 38, top + 42, 76, 14);

        TextRenderer.DrawText(
            graphics,
            message,
            _messageFont,
            messageBounds,
            UiPalette.Text,
            TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);

        for (var index = 0; index < _descriptor.Messages.Length; index++)
        {
            var dot = new Rectangle(dotBounds.Left + index * 31, dotBounds.Top + 3, 9, 9);
            using var dotBrush = new SolidBrush(index <= CurrentStep ? _descriptor.AccentColor : UiPalette.Border);
            graphics.FillEllipse(dotBrush, dot);
        }
    }

    private void PaintProgress(Graphics graphics, Rectangle bounds)
    {
        var barWidth = Math.Min(300, Math.Max(190, bounds.Width - 112));
        var bar = new Rectangle((bounds.Width - barWidth) / 2, bounds.Height - 42, barWidth, 9);
        using var backgroundPath = bar.ToRoundedPath(5);
        using var backgroundBrush = new SolidBrush(UiPalette.FromHex("#F1F5F9"));
        graphics.FillPath(backgroundBrush, backgroundPath);

        var progressWidth = Math.Max(9, (int)(bar.Width * Progress));
        var progress = new Rectangle(bar.Left, bar.Top, progressWidth, bar.Height);
        using var progressPath = progress.ToRoundedPath(5);
        using var progressBrush = new LinearGradientBrush(
            bar,
            _descriptor.AccentColor,
            UiPalette.Blend(_descriptor.AccentColor, Color.White, 0.25),
            LinearGradientMode.Horizontal);

        graphics.FillPath(progressBrush, progressPath);
    }

    private static float EaseInOut(float value)
    {
        return value < 0.5f
            ? 2f * value * value
            : 1f - MathF.Pow(-2f * value + 2f, 2f) / 2f;
    }

    private static float Lerp(float start, float end, float amount)
    {
        return start + (end - start) * amount;
    }

    private enum AnimationKind
    {
        Pix,
        CreditCard,
        Boleto
    }

    private sealed record AnimationDescriptor(
        AnimationKind Kind,
        string Title,
        string[] Messages,
        Color AccentColor,
        Color SoftColor)
    {
        public static AnimationDescriptor FromMethod(string method)
        {
            if (method.Contains("PIX", StringComparison.OrdinalIgnoreCase))
            {
                return ForPix();
            }

            if (method.Contains("Boleto", StringComparison.OrdinalIgnoreCase))
            {
                return ForBoleto();
            }

            return ForCreditCard();
        }

        public static AnimationDescriptor ForPix()
        {
            return new AnimationDescriptor(
                AnimationKind.Pix,
                "PIX",
                [
                    "Conectando ao banco...",
                    "Validando chave PIX...",
                    "Pagamento aprovado via PIX."
                ],
                UiPalette.FromHex("#00B8A9"),
                UiPalette.FromHex("#E0F7F5"));
        }

        private static AnimationDescriptor ForCreditCard()
        {
            return new AnimationDescriptor(
                AnimationKind.CreditCard,
                "Cart\u00e3o de Cr\u00e9dito",
                [
                    "Lendo dados do cart\u00e3o...",
                    "Autorizando pagamento...",
                    "Compra aprovada."
                ],
                UiPalette.FromHex("#2563EB"),
                UiPalette.FromHex("#DBEAFE"));
        }

        private static AnimationDescriptor ForBoleto()
        {
            return new AnimationDescriptor(
                AnimationKind.Boleto,
                "Boleto",
                [
                    "Gerando boleto...",
                    "Aguardando compensa\u00e7\u00e3o simulada...",
                    "Boleto pago com sucesso."
                ],
                UiPalette.FromHex("#F59E0B"),
                UiPalette.FromHex("#FEF3C7"));
        }
    }
}
