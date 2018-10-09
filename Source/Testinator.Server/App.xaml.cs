using Dna;
using System;
using System.Globalization;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using Testinator.Core;
using Testinator.Server.Core;

using static Testinator.Server.Core.DI;

namespace Testinator.Server
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Custom startup so we load our IoC and Updater immediately before anything else
        /// </summary>
        /// <param name="e"></param>
        protected override async void OnStartup(StartupEventArgs e)
        {
            // Let the base application do what it needs
            base.OnStartup(e);

            // Setup the main application 
            await ApplicationSetupAsync();
            
            Logger.LogCriticalSource("Application starting...");

            // Show the main window
            Current.MainWindow = new MainWindow();
            Current.MainWindow.Show();

            // TODO: restore check for updates
            // Check for updates
            /*if (await CheckUpdatesAsync())
            {
                Logger.LogDebugSource("Running updater...");

                // Run the updater
                Process.Start(new ProcessStartInfo
                {
                    FileName = "Testinator.Updater.exe",
                    Arguments = "Server" + " " + Application.ApplicationLanguage + " " + "",
                    UseShellExecute = true,
                    Verb = "runas"
                });

                // Close this app
                Logger.LogDebugSource("Main application closing...");
                Current.Shutdown();
            }*/
        }

        private async Task ApplicationSetupAsync()
        {
            Framework.Construct<DefaultFrameworkConstruction>()
                .AddFileLogger(GetLogFileName())
                .AddApplicationServices()
                .AddApplicationViewModels()
                .Build();

            await AppDbContext.Database.EnsureCreatedAsync();
        }

        private string GetLogFileName()
        {
            return $"ServerLog{DateTime.Now.ToString().Replace("/", "-").Replace(" ", "-").Replace(":", "-")}.txt";
        }

        /// <summary>
        /// Configures our application ready for use
        /// </summary>
        private void ApplicationSetup()
        {
            // Default language
            LocalizationResource.Culture = new CultureInfo("pl-PL");
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
        /// Checks if there is a new version of that application
        /// </summary>
        private async Task<bool> CheckUpdatesAsync()
        {
            try
            {
                // Set webservice's url and parameters we want to send
                var url = "http://minorsonek.pl/testinator/data/index.php";
                var parameters = $"version={ DI.Application.Version.ToString() }&type=Server";

                // Catch the result
                var result = string.Empty;

                // Send request to webservice
                using (var wc = new WebClient())
                {
                    wc.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                    result = wc.UploadString(url, parameters);
                }

                // Return the statement based on result...
                switch (result)
                {
                    case "New update":
                        {
                            // There is new update, but not important one
                            // Ask the user if he wants to update
                            var vm = new DecisionDialogViewModel
                            {
                                Title = LocalizationResource.NewUpdate,
                                Message = LocalizationResource.NewVersionCanDownload,
                                AcceptText = LocalizationResource.Sure,
                                CancelText = LocalizationResource.SkipUpdate
                            };
                            await UI.ShowMessage(vm);

                            // Depending on the answer...
                            return vm.UserResponse;
                        }

                    case "New update IMP":
                        {
                            // An important update, inform the user and update
                            await UI.ShowMessage(new MessageBoxDialogViewModel
                            {
                                Title = LocalizationResource.NewImportantUpdate,
                                Message = LocalizationResource.NewImportantUpdateInfo,
                                OkText = LocalizationResource.Ok
                            });
                            return true;
                        }

                    default:
                        // No updates
                        return false;
                }
            }
            catch
            {
                // Cannot connect to the web, no updates
                return false;
            }
        }
    }
}
