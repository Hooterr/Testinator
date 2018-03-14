﻿using System.Globalization;
using System.Threading.Tasks;
using System.Windows;
using Testinator.Server.Core;
using Testinator.Core;
using Testinator.UICore;
using System.Net;
using System.Diagnostics;

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
            ApplicationSetup();

            IoCServer.Logger.Log("Application starting...");

            // Show the main window
            Current.MainWindow = new MainWindow();
            Current.MainWindow.Show();

            // Check for updates
            if (await CheckUpdatesAsync())
            {
                IoCServer.Logger.Log("Running updater...");

                // Run the updater
                Process.Start(new ProcessStartInfo
                {
                    FileName = "Testinator.Updater.exe",
                    Arguments = "Server" + " " + IoCServer.Application.ApplicationLanguage + " " + "",
                    UseShellExecute = true,
                    Verb = "runas"
                });

                // Close this app
                IoCServer.Logger.Log("Main application closing...");
                Current.Shutdown();
            }
        }

        /// <summary>
        /// Configures our application ready for use
        /// </summary>
        private void ApplicationSetup()
        {
            // Setup IoC
            IoCServer.Setup();

            // Bind a logger
            IoCServer.Kernel.Bind<ILogFactory>().ToConstant(new BaseLogFactory(new[]
            {
                // Set the path from Settings
                new FileLogger(IoCServer.Settings.LogFilePath)
            }));

            // Bind a File Writer
            IoCServer.Kernel.Bind<FileManagerBase>().ToConstant(new LogsWriter());

            // Bind a UI Manager
            IoCServer.Kernel.Bind<IUIManager>().ToConstant(new UIManager());
        }

        /// <summary>
        /// Checks if there is a new version of that application
        /// </summary>
        private async Task<bool> CheckUpdatesAsync()
        {
            try
            {
                // Get current version
                var currentVersion = "1.0.0.0";

                // Set webservice's url and parameters we want to send
                var url = "http://minorsonek.pl/testinator/data/index.php";
                var parameters = $"version={ currentVersion }&type=Server";

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
                            var vm = new ResultBoxDialogViewModel
                            {
                                Title = "New update found!",
                                Message = "New version of Testinator can be downloaded, update now?",
                                AcceptText = "Sure",
                                CancelText = "Skip update"
                            };
                            await IoCServer.UI.ShowMessage(vm);

                            // Depend on the answer...
                            return vm.UserResponse;
                        }

                    case "New update IMP":
                        {
                            // An important update, inform the user and update
                            await IoCServer.UI.ShowMessage(new MessageBoxDialogViewModel
                            {
                                Title = "Important new update!",
                                Message = "New important update is up, application will close.",
                                OkText = "Ok"
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
