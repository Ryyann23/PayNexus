using PayNexus.Components;
using PayNexus.Utils;

namespace PayNexus.Views;

public sealed class NovoPagamentoForm : Form
{
    private readonly TextBox _clienteTextBox;
    private readonly NumericUpDown _valorInput;
    private readonly ComboBox _metodoComboBox;
    private readonly Label _errorLabel;
    private bool _dragging;
    private Point _dragStart;

    public NovoPagamentoForm(IReadOnlyList<string> metodosPagamento)
    {
        _clienteTextBox = CreateTextBox();
        _valorInput = CreateValueInput();
        _metodoComboBox = CreateMethodComboBox(metodosPagamento);
        _errorLabel = new Label
        {
            Dock = DockStyle.Fill,
            Font = UiFonts.Regular(8.8f),
            ForeColor = UiPalette.Danger,
            BackColor = Color.Transparent,
            TextAlign = ContentAlignment.MiddleLeft
        };

        ConfigureWindow();
        BuildLayout();
    }

    public string ClienteNome => _clienteTextBox.Text.Trim();

    public decimal Valor => _valorInput.Value;

    public string MetodoPagamento => _metodoComboBox.SelectedItem?.ToString() ?? string.Empty;

    protected override void OnShown(EventArgs e)
    {
        base.OnShown(e);
        _clienteTextBox.Focus();
    }

    private void ConfigureWindow()
    {
        Text = "Novo Pagamento";
        ClientSize = new Size(520, 500);
        MinimumSize = new Size(520, 500);
        MaximumSize = new Size(520, 500);
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.None;
        BackColor = UiPalette.Background;
        Font = UiFonts.Regular(10f);
        AutoScaleMode = AutoScaleMode.Dpi;
        DoubleBuffered = true;
    }

    private void BuildLayout()
    {
        var shell = new SurfacePanel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(30, 26, 38, 38),
            CornerRadius = 24
        };

        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.Transparent,
            ColumnCount = 1,
            RowCount = 10
        };

        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 76));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 24));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 62));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 24));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 62));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 24));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 62));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 32));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 54));

        var header = CreateHeader();
        EnableDrag(header);

        layout.Controls.Add(header, 0, 0);
        layout.Controls.Add(CreateFieldLabel("Nome do cliente"), 0, 1);
        layout.Controls.Add(CreateInputHost(_clienteTextBox), 0, 2);
        layout.Controls.Add(CreateFieldLabel("Valor"), 0, 3);
        layout.Controls.Add(CreateInputHost(_valorInput), 0, 4);
        layout.Controls.Add(CreateFieldLabel("Método de pagamento"), 0, 5);
        layout.Controls.Add(CreateInputHost(_metodoComboBox), 0, 6);
        layout.Controls.Add(_errorLabel, 0, 7);
        layout.Controls.Add(CreateActions(), 0, 9);

        shell.Controls.Add(layout);
        Controls.Add(shell);
    }

    private Control CreateHeader()
    {
        var header = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.Transparent,
            ColumnCount = 2,
            RowCount = 1
        };

        header.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        header.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 42));

        var textPanel = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.Transparent
        };

        var title = new Label
        {
            Text = "Novo pagamento",
            Font = UiFonts.Title(19f),
            ForeColor = UiPalette.Text,
            BackColor = Color.Transparent,
            Location = new Point(0, 0),
            Size = new Size(330, 36)
        };


        textPanel.Controls.Add(title);


        var closeButton = new ModernButton
        {
            Text = "x",
            Width = 38,
            Height = 38,
            FillColor = Color.White,
            HoverColor = UiPalette.FromHex("#F3F4F6"),
            ForeColor = UiPalette.MutedText,
            BorderColor = UiPalette.Border,
            Outline = true,
            Margin = new Padding(0)
        };

        closeButton.Click += (_, _) =>
        {
            DialogResult = DialogResult.Cancel;
            Close();
        };

        header.Controls.Add(textPanel, 0, 0);
        header.Controls.Add(closeButton, 1, 0);

        return header;
    }

    private Control CreateActions()
    {
        var actions = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.RightToLeft,
            WrapContents = false,
            BackColor = Color.Transparent
        };

        var confirmButton = new ModernButton
        {
            Text = "Processar Pagamento",
            Width = 178,
            Height = 44,
            FillColor = UiPalette.Primary,
            HoverColor = UiPalette.Secondary,
            ForeColor = Color.White,
            Margin = new Padding(10, 0, 0, 0)
        };

        var cancelButton = new ModernButton
        {
            Text = "Cancelar",
            Width = 112,
            Height = 44,
            FillColor = Color.White,
            HoverColor = UiPalette.FromHex("#F3F4F6"),
            ForeColor = UiPalette.MutedText,
            BorderColor = UiPalette.Border,
            Outline = true,
            Margin = new Padding(10, 0, 0, 0)
        };

        confirmButton.Click += (_, _) => ConfirmPayment();
        cancelButton.Click += (_, _) =>
        {
            DialogResult = DialogResult.Cancel;
            Close();
        };

        AcceptButton = confirmButton;
        CancelButton = cancelButton;

        actions.Controls.Add(confirmButton);
        actions.Controls.Add(cancelButton);

        return actions;
    }

    private static Label CreateFieldLabel(string text)
    {
        return new Label
        {
            Text = text,
            Dock = DockStyle.Fill,
            Font = UiFonts.Medium(9.3f),
            ForeColor = UiPalette.Text,
            BackColor = Color.Transparent,
            TextAlign = ContentAlignment.BottomLeft
        };
    }

    private static Panel CreateInputHost(Control input)
    {
        var host = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.Transparent,
            Padding = new Padding(14, 12, 14, 8),
            Margin = new Padding(0, 6, 0, 10)
        };

        host.Paint += (_, e) =>
        {
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            var rectangle = new Rectangle(0, 0, host.Width - 1, host.Height - 1);
            using var path = rectangle.ToRoundedPath(14);
            using var backgroundBrush = new SolidBrush(UiPalette.FromHex("#F9FAFB"));
            using var borderPen = new Pen(UiPalette.Border);

            e.Graphics.FillPath(backgroundBrush, path);
            e.Graphics.DrawPath(borderPen, path);
        };

        input.Dock = DockStyle.Fill;
        host.Controls.Add(input);
        return host;
    }

    private static TextBox CreateTextBox()
    {
        return new TextBox
        {
            BorderStyle = BorderStyle.None,
            BackColor = UiPalette.FromHex("#F9FAFB"),
            ForeColor = UiPalette.Text,
            Font = UiFonts.Regular(10.5f),
            PlaceholderText = "Ex.: Ana Martins"
        };
    }

    private static NumericUpDown CreateValueInput()
    {
        return new NumericUpDown
        {
            BorderStyle = BorderStyle.None,
            BackColor = UiPalette.FromHex("#F9FAFB"),
            ForeColor = UiPalette.Text,
            Font = UiFonts.Regular(10.5f),
            DecimalPlaces = 2,
            Minimum = 1,
            Maximum = 1_000_000,
            Increment = 10,
            Value = 199.90m,
            ThousandsSeparator = true
        };
    }

    private static ComboBox CreateMethodComboBox(IReadOnlyList<string> methods)
    {
        var comboBox = new ComboBox
        {
            DropDownStyle = ComboBoxStyle.DropDownList,
            FlatStyle = FlatStyle.Flat,
            BackColor = UiPalette.FromHex("#F9FAFB"),
            ForeColor = UiPalette.Text,
            Font = UiFonts.Regular(10.5f)
        };

        foreach (var method in methods)
        {
            comboBox.Items.Add(method);
        }

        if (comboBox.Items.Count > 0)
        {
            comboBox.SelectedIndex = 0;
        }

        return comboBox;
    }

    private void ConfirmPayment()
    {
        if (string.IsNullOrWhiteSpace(ClienteNome))
        {
            _errorLabel.Text = "Informe o nome do cliente para continuar.";
            _clienteTextBox.Focus();
            return;
        }

        if (string.IsNullOrWhiteSpace(MetodoPagamento))
        {
            _errorLabel.Text = "Selecione um método de pagamento.";
            _metodoComboBox.Focus();
            return;
        }

        _errorLabel.Text = string.Empty;
        DialogResult = DialogResult.OK;
        Close();
    }

    private void EnableDrag(Control control)
    {
        control.MouseDown += (_, e) =>
        {
            if (e.Button != MouseButtons.Left)
            {
                return;
            }

            _dragging = true;
            _dragStart = e.Location;
        };

        control.MouseMove += (_, e) =>
        {
            if (!_dragging)
            {
                return;
            }

            Location = new Point(
                Location.X + e.X - _dragStart.X,
                Location.Y + e.Y - _dragStart.Y);
        };

        control.MouseUp += (_, _) => _dragging = false;

        foreach (Control child in control.Controls)
        {
            EnableDrag(child);
        }
    }
}
