using System;
using System.Windows.Forms;
using Microsoft.Extensions.DependencyInjection;
using Library.Core.Extensions;

namespace LibraryWinForms;

internal static class Program
{
    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
        // Standard WinForms bootstrap
        Application.SetHighDpiMode(HighDpiMode.SystemAware);
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);

        // ----- Modern DI container setup -----
        var services = new ServiceCollection();
        ConfigureServices(services);

        using ServiceProvider serviceProvider = services.BuildServiceProvider();

        // Resolve the form through the container so IBookService, IUserService,
        // and IUserBooksServices get injected into its constructor automatically.
        var mainForm = serviceProvider.GetRequiredService<Form1>();

        Application.Run(mainForm);
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        // Registers IBookRepository, IUserRepository, IBookService,
        // IUserService and IUserBooksServices (defined in Practice.Extensions).
        services.AddPracticeServices();

        // The form itself is resolved from the container, so it can declare
        // the services it needs as constructor parameters.
        services.AddTransient<Form1>();
    }
}
