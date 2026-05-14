using System.Globalization;
using System.Reflection;
using PayNexus.Components;
using PayNexus.Models;
using PayNexus.Services;
using PayNexus.Utils;

namespace PayNexus.Views;

public sealed class DashboardForm : Form
{
    private readonly PagamentoService _pagamentoService;
    private readonly CultureInfo _culture = CultureInfo.GetCultureInfo("pt-BR");
    private readonly ModernCard _totalCard;
    private readonly ModernCard _transactionsCard;
    private readonly ModernCard _approvedCard;
    private readonly ModernCard _pendingCard;
    private readonly SidebarPanel _sidebarPanel;
    private readonly DataGridView _transactionGrid;
    private readonly Label _transactionCountLabel;

    public DashboardForm(PagamentoService pagamentoService)
    {
        _pagamentoService = pagamentoService;
        _totalCard = new ModernCard("Total Processado", "R$ 0,00", "Somente pagamentos aprovados", UiPalette.Primary, "R$");
        _transactionsCard = new ModernCard("Transações Hoje", "0", "Operações no dia atual", UiPalette.Secondary, "#");
        _approvedCard = new ModernCard("Pagamentos Aprovados", "0", "Fluxo concluído", UiPalette.FromHex("#8B5CF6"), "OK");
        _pendingCard = new ModernCard("Pagamentos Pendentes", "0", "Aguardando compensação", UiPalette.FromHex("#C084FC"), "...");
        _sidebarPanel = new SidebarPanel();
        _transactionGrid = CreateTransactionGrid();
        _transactionCountLabel = new Label
        {
            Text = "0 registros",
            Font = UiFonts.Regular(9f),
            ForeColor = UiPalette.MutedText,
            TextAlign = ContentAlignment.MiddleRight,
            Dock = DockStyle.Fill,
            BackColor = Color.Transparent
        };

        ConfigureWindow();
        BuildLayout();
        RegisterEvents();
        UpdateObserverSummary();
        RefreshDashboard();
    }

    private void ConfigureWindow()
    {
        Text = "PayNexus";
        Size = new Size(1280, 820);
        MinimumSize = new Size(1040, 700);
        StartPosition = FormStartPosition.CenterScreen;
        WindowState = FormWindowState.Normal;
        BackColor = UiPalette.Background;
        Font = UiFonts.Regular(10f);
        AutoScaleMode = AutoScaleMode.Dpi;
        DoubleBuffered = true;
    }

    private void BuildLayout()
    {
        SuspendLayout();

        var root = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(24, 14, 24, 24),
            BackColor = UiPalette.Background,
            ColumnCount = 1,
            RowCount = 3
        };

        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 88));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 184));
        root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        root.Controls.Add(CreateHeader(), 0, 0);
        root.Controls.Add(CreateCardsPanel(), 0, 1);
        root.Controls.Add(CreateContentPanel(), 0, 2);

        Controls.Add(root);
        ResumeLayout();
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

        header.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 38));
        header.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 62));

        var titlePanel = new Panel { Dock = DockStyle.Fill, BackColor = Color.Transparent };
        var titleLabel = new Label
        {
            Text = "PayNexus",
            Font = UiFonts.Title(27f),
            ForeColor = UiPalette.Text,
            BackColor = Color.Transparent,
            Location = new Point(0, 0),
            Size = new Size(380, 52)
        };

        titlePanel.Controls.Add(titleLabel);

        var actionsPanel = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.RightToLeft,
            WrapContents = false,
            BackColor = Color.Transparent,
            Padding = new Padding(0, 14, 0, 0)
        };

        var newPaymentButton = CreateActionButton("+ Novo Pagamento", 168, UiPalette.Primary, UiPalette.Secondary, Color.White);
        newPaymentButton.Click += (_, _) => OpenNewPaymentDialog();

        var addObserverButton = CreateActionButton("+ Adicionar Observer", 188, UiPalette.Secondary, UiPalette.Primary, Color.White);
        addObserverButton.Click += (_, _) => AddNotificationObserver();

        var clearLogsButton = CreateActionButton("Limpar Logs", 132, Color.White, UiPalette.PurpleSoft, UiPalette.Primary);
        clearLogsButton.BorderColor = UiPalette.Border;
        clearLogsButton.Outline = true;
        clearLogsButton.Click += (_, _) => ClearLogs();

        actionsPanel.Controls.Add(newPaymentButton);
        actionsPanel.Controls.Add(addObserverButton);
        actionsPanel.Controls.Add(clearLogsButton);

        header.Controls.Add(titlePanel, 0, 0);
        header.Controls.Add(actionsPanel, 1, 0);

        return header;
    }

    private static ModernButton CreateActionButton(string text, int width, Color fillColor, Color hoverColor, Color textColor)
    {
        return new ModernButton
        {
            Text = text,
            Width = width,
            Height = 44,
            FillColor = fillColor,
            HoverColor = hoverColor,
            ForeColor = textColor,
            Margin = new Padding(10, 0, 0, 0)
        };
    }

    private Control CreateCardsPanel()
    {
        var cards = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.Transparent,
            ColumnCount = 4,
            RowCount = 1,
            Padding = new Padding(0, 0, 0, 20)
        };

        for (var column = 0; column < 4; column++)
        {
            cards.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
        }

        AddCard(cards, _totalCard, 0);
        AddCard(cards, _transactionsCard, 1);
        AddCard(cards, _approvedCard, 2);
        AddCard(cards, _pendingCard, 3);

        return cards;
    }

    private static void AddCard(TableLayoutPanel table, ModernCard card, int column)
    {
        card.Dock = DockStyle.Fill;
        card.Margin = column == 3 ? new Padding(0) : new Padding(0, 0, 16, 0);
        table.Controls.Add(card, column, 0);
    }

    private Control CreateContentPanel()
    {
        var content = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.Transparent,
            ColumnCount = 2,
            RowCount = 1
        };

        content.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 68));
        content.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 32));

        content.Controls.Add(CreateTransactionsPanel(), 0, 0);

        _sidebarPanel.Dock = DockStyle.Fill;
        _sidebarPanel.Margin = new Padding(18, 0, 0, 0);
        content.Controls.Add(_sidebarPanel, 1, 0);

        return content;
    }

    private Control CreateTransactionsPanel()
    {
        var surface = new SurfacePanel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(22, 20, 30, 30),
            CornerRadius = 20
        };

        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.Transparent,
            RowCount = 2,
            ColumnCount = 1
        };

        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 50));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        var header = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.Transparent,
            ColumnCount = 2,
            RowCount = 1
        };

        header.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 62));
        header.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 38));

        var title = new Label
        {
            Text = "Transações recentes",
            Font = UiFonts.Title(14f),
            ForeColor = UiPalette.Text,
            Dock = DockStyle.Fill,
            BackColor = Color.Transparent,
            TextAlign = ContentAlignment.MiddleLeft
        };

        header.Controls.Add(title, 0, 0);
        header.Controls.Add(_transactionCountLabel, 1, 0);

        layout.Controls.Add(header, 0, 0);
        layout.Controls.Add(_transactionGrid, 0, 1);
        surface.Controls.Add(layout);

        return surface;
    }

    private DataGridView CreateTransactionGrid()
    {
        var grid = new DataGridView
        {
            Dock = DockStyle.Fill,
            BackgroundColor = Color.White,
            BorderStyle = BorderStyle.None,
            CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
            ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None,
            RowHeadersVisible = false,
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            AllowUserToResizeRows = false,
            ReadOnly = true,
            MultiSelect = false,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            GridColor = UiPalette.Border,
            EnableHeadersVisualStyles = false,
            ScrollBars = ScrollBars.Vertical,
            RowTemplate = { Height = 56 }
        };

        EnableDoubleBuffering(grid);

        grid.ColumnHeadersDefaultCellStyle.BackColor = Color.White;
        grid.ColumnHeadersDefaultCellStyle.ForeColor = UiPalette.MutedText;
        grid.ColumnHeadersDefaultCellStyle.Font = UiFonts.Medium(9f);
        grid.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.White;
        grid.ColumnHeadersDefaultCellStyle.SelectionForeColor = UiPalette.MutedText;
        grid.ColumnHeadersHeight = 44;
        grid.DefaultCellStyle.BackColor = Color.White;
        grid.DefaultCellStyle.ForeColor = UiPalette.Text;
        grid.DefaultCellStyle.Font = UiFonts.Regular(9.2f);
        grid.DefaultCellStyle.SelectionBackColor = UiPalette.PurpleSelection;
        grid.DefaultCellStyle.SelectionForeColor = UiPalette.Text;
        grid.AlternatingRowsDefaultCellStyle.BackColor = UiPalette.FromHex("#FCFAFF");

        grid.Columns.Add("Cliente", "Cliente");
        grid.Columns.Add("Metodo", "Método de pagamento");
        grid.Columns.Add("Valor", "Valor");
        grid.Columns.Add("Status", "Status");
        grid.Columns.Add("DataHora", "Data/Hora");

        grid.Columns["Cliente"]!.FillWeight = 24;
        grid.Columns["Metodo"]!.FillWeight = 23;
        grid.Columns["Valor"]!.FillWeight = 16;
        grid.Columns["Status"]!.FillWeight = 17;
        grid.Columns["DataHora"]!.FillWeight = 20;
        grid.Columns["Valor"]!.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
        grid.Columns["Status"]!.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

        grid.CellPainting += PaintStatusCell;
        return grid;
    }

    private void RegisterEvents()
    {
        _pagamentoService.LogRegistrado += AddLog;
        _pagamentoService.PagamentosAtualizados += RefreshDashboard;
    }

    private static void EnableDoubleBuffering(DataGridView grid)
    {
        typeof(DataGridView).InvokeMember(
            "DoubleBuffered",
            BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.SetProperty,
            null,
            grid,
            [true]);
    }

    private void OpenNewPaymentDialog()
    {
        using var form = new NovoPagamentoForm(_pagamentoService.MetodosPagamento);

        if (form.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        try
        {
            _pagamentoService.ProcessarPagamento(form.ClienteNome, form.Valor, form.MetodoPagamento);
        }
        catch (Exception exception)
        {
            _sidebarPanel.AddLog($"Falha ao processar pagamento: {exception.Message}");
        }
    }

    private void AddNotificationObserver()
    {
        _pagamentoService.AdicionarObserverNotificacaoSistema();
        UpdateObserverSummary();
    }

    private void ClearLogs()
    {
        _sidebarPanel.ClearLogs();
    }

    private void RefreshDashboard()
    {
        if (InvokeRequired)
        {
            BeginInvoke(RefreshDashboard);
            return;
        }

        UpdateTransactionGrid();
        UpdateCards();
        UpdateObserverSummary();
    }

    private void AddLog(string message)
    {
        if (InvokeRequired)
        {
            BeginInvoke(() => AddLog(message));
            return;
        }

        _sidebarPanel.AddLog(message);
    }

    private void UpdateCards()
    {
        var resumo = _pagamentoService.ObterResumo();
        _totalCard.UpdateValue(resumo.TotalProcessado.ToString("C", _culture));
        _transactionsCard.UpdateValue(resumo.TransacoesHoje.ToString(_culture));
        _approvedCard.UpdateValue(resumo.PagamentosAprovados.ToString(_culture));
        _pendingCard.UpdateValue(resumo.PagamentosPendentes.ToString(_culture));
    }

    private void UpdateTransactionGrid()
    {
        _transactionGrid.SuspendLayout();
        _transactionGrid.Rows.Clear();

        foreach (var pagamento in _pagamentoService.Pagamentos)
        {
            var processedAt = pagamento.ProcessadoEm ?? pagamento.CriadoEm;
            var rowIndex = _transactionGrid.Rows.Add(
                pagamento.Cliente.Nome,
                pagamento.MetodoPagamento,
                pagamento.Valor.ToString("C", _culture),
                StatusBadge.GetStatusText(pagamento.Status),
                processedAt.ToString("dd/MM/yyyy HH:mm", _culture));

            _transactionGrid.Rows[rowIndex].Tag = pagamento;
        }

        _transactionCountLabel.Text = $"{_pagamentoService.Pagamentos.Count} registros";
        _transactionGrid.ResumeLayout();
    }

    private void UpdateObserverSummary()
    {
        _sidebarPanel.SetObservers(_pagamentoService.ObserversAtivos);
    }

    private void PaintStatusCell(object? sender, DataGridViewCellPaintingEventArgs e)
    {
        if (e.RowIndex < 0 || _transactionGrid.Columns[e.ColumnIndex].Name != "Status")
        {
            return;
        }

        if (e.Graphics is null)
        {
            return;
        }

        var isSelected = (e.State & DataGridViewElementStates.Selected) != 0 ||
                         _transactionGrid.Rows[e.RowIndex].Selected;
        var backgroundColor = isSelected
            ? _transactionGrid.DefaultCellStyle.SelectionBackColor
            : e.RowIndex % 2 == 0
                ? _transactionGrid.DefaultCellStyle.BackColor
                : _transactionGrid.AlternatingRowsDefaultCellStyle.BackColor;

        using var backgroundBrush = new SolidBrush(backgroundColor);
        using var borderPen = new Pen(_transactionGrid.GridColor);
        e.Graphics.FillRectangle(backgroundBrush, e.CellBounds);
        e.Graphics.DrawLine(borderPen, e.CellBounds.Left, e.CellBounds.Bottom - 1, e.CellBounds.Right, e.CellBounds.Bottom - 1);

        if (_transactionGrid.Rows[e.RowIndex].Tag is Pagamento pagamento)
        {
            StatusBadge.PaintBadge(e.Graphics, e.CellBounds, pagamento.Status);
        }

        e.Handled = true;
    }
}
