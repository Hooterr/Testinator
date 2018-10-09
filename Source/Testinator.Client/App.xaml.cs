using Dna;
using System;
using System.Globalization;
using System.Windows;
using Testinator.Client.Core;
using Testinator.Core;

using static Testinator.Client.Core.DI;

namespace Testinator.Client
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Custom startup so we load our IoC immediately before anything else
        /// </summary>
        /// <param name="e"></param>
        protected override void OnStartup(StartupEventArgs e)
        {
            // Let the base application do what it needs
            base.OnStartup(e);

            // Setup the main application 
            ApplicationSetup();

            // Log that application is starting
            Logger.LogDebugSource("Application starting...");

            // Show the main window
            Current.MainWindow = new MainWindow();
            Current.MainWindow.Show();

            // Go to the first page
            DI.Application.GoToPage(ApplicationPage.Login);
        }

        /// <summary>
        /// Notify the application about closing procedure
        /// </summary>
        /// <param name="e"></param>
        protected override void OnExit(ExitEventArgs e)
        {
            DI.Application.Close();
        }

        /// <summary>
        /// Configures our application ready for use
        /// </summary>
        private void ApplicationSetup()
        {
            // Set default language
            LocalizationResource.Culture = new CultureInfo("pl-PL");

            Framework.Construct<DefaultFrameworkConstruction>()
                .AddFileLogger()
                .AddApplicationServices()
                .AddApplicationViewModels()
                .Build();
        }

        private string GetDefaultLogFileName()
        {
            return $"TestinatorClientLog-{DateTime.Now.ToString().Replace("/", "-").Replace(" ", "-").Replace(":", "-")}.txt";
        }
    }
}
