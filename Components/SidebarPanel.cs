using PayNexus.Utils;

namespace PayNexus.Components;

public sealed class SidebarPanel : SurfacePanel
{
    private readonly Label _titleLabel;
    private readonly Label _observerLabel;
    private readonly FlowLayoutPanel _logContainer;

    public SidebarPanel()
    {
        CornerRadius = 20;
        Padding = new Padding(20);

        _titleLabel = new Label
        {
            Text = "Logs em tempo real",
            Dock = DockStyle.Top,
            Height = 30,
            Font = UiFonts.Title(13f),
            ForeColor = UiPalette.Text,
            BackColor = Color.Transparent
        };

        _observerLabel = new Label
        {
            Dock = DockStyle.Top,
            Height = 38,
            Font = UiFonts.Regular(8.7f),
            ForeColor = UiPalette.MutedText,
            BackColor = Color.Transparent
        };

        _logContainer = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            AutoScroll = true,
            WrapContents = false,
            FlowDirection = FlowDirection.TopDown,
            Padding = new Padding(0, 10, 4, 0),
            BackColor = Color.Transparent
        };

        _logContainer.Resize += (_, _) => AjustarLarguraLogs();

        Controls.Add(_logContainer);
        Controls.Add(_observerLabel);
        Controls.Add(_titleLabel);
    }

    public void SetObservers(IReadOnlyList<string> observers)
    {
        _observerLabel.Text = $"{observers.Count} observers ativos: {string.Join(", ", observers)}";
    }

    public void AddLog(string message)
    {
        _logContainer.SuspendLayout();

        var item = CreateLogItem(message);
        _logContainer.Controls.Add(item);
        _logContainer.Controls.SetChildIndex(item, 0);

        while (_logContainer.Controls.Count > 80)
        {
            var lastIndex = _logContainer.Controls.Count - 1;
            var control = _logContainer.Controls[lastIndex];
            _logContainer.Controls.RemoveAt(lastIndex);
            control.Dispose();
        }

        _logContainer.ResumeLayout();
    }

    public void ClearLogs()
    {
        foreach (Control control in _logContainer.Controls.Cast<Control>().ToList())
        {
            control.Dispose();
        }

        _logContainer.Controls.Clear();
    }

    private Panel CreateLogItem(string message)
    {
        var panel = new Panel
        {
            Width = GetLogItemWidth(),
            Height = 72,
            Margin = new Padding(0, 0, 0, 10),
            BackColor = UiPalette.PurpleSoft
        };

        panel.Paint += (_, e) =>
        {
            using var accentBrush = new SolidBrush(UiPalette.Primary);
            e.Graphics.FillRectangle(accentBrush, new Rectangle(0, 16, 4, panel.Height - 32));
        };

        var timeLabel = new Label
        {
            Text = DateTime.Now.ToString("HH:mm:ss"),
            Font = UiFonts.Medium(8f),
            ForeColor = UiPalette.Primary,
            BackColor = Color.Transparent,
            Location = new Point(16, 12),
            Size = new Size(panel.Width - 32, 18)
        };

        var messageLabel = new Label
        {
            Text = message,
            Font = UiFonts.Regular(8.6f),
            ForeColor = UiPalette.Text,
            BackColor = Color.Transparent,
            Location = new Point(16, 33),
            Size = new Size(panel.Width - 34, 34)
        };

        panel.Controls.Add(timeLabel);
        panel.Controls.Add(messageLabel);
        return panel;
    }

    private void AjustarLarguraLogs()
    {
        foreach (Control control in _logContainer.Controls)
        {
            control.Width = GetLogItemWidth();

            foreach (Control child in control.Controls)
            {
                child.Width = control.Width - 34;
            }
        }
    }

    private int GetLogItemWidth()
    {
        return Math.Max(260, _logContainer.ClientSize.Width - 12);
    }
}
