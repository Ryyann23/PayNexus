using PayNexus.Services;
using PayNexus.Views;

namespace PayNexus;

static class Program
{
    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();
        Application.Run(new DashboardForm(new PagamentoService()));
    }
}
