﻿using Dna;
using System;
using System.IO;
using System.Net;
using Testinator.Core;

namespace Testinator.Client.Core
{
    /// <summary>
    /// Provides network support for client-side
    /// </summary>
    public class ClientNetwork : ClientNetworkBase
    {
        #region Public Properties

        /// <summary>
        /// Indicates if attempting to reconnect is in progress
        /// </summary>
        public bool AttemptingToReconnect { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public ClientNetwork()
        {
            InitializeUsingConfigFile();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Sends update to the server about the user data
        /// </summary>
        public void SendClientModelUpdate()
        {
            var dataPackage = DI.Client.GetPackage();
            DI.Application.Network.SendData(dataPackage);
        }

        /// <summary>
        /// Stops reconnecting if in progress
        /// </summary>
        public void StopReconnecting()
        {
            // If connected or not attempting to reconnect dont do anything
            if (IsConnected || !AttemptingToReconnect)
                return;

            // Stop connecting, otherwise
            Disconnect();
        }

        #endregion

        #region Override Methods

        /// <summary>
        /// Fired when connection gets stoped
        /// </summary>
        protected override void OnConnectionLost()
        {
            // Log it
            DI.Logger.LogDebugSource("Network connection lost");

            // Dont'try to reconnect if in the result page, because the test result has been already sent to the server
            if (DI.TestHost.IsShowingResultPage)
                return;

            // If the test in progress
            if (DI.TestHost.IsTestInProgress)
            {
                // Notify the test host about the disconnection
                //IoCClient.DI.TestHost.NetworkDisconnected();

                // Set attempting to reconnect
                AttemptingToReconnect = true;

                // Start connecting to the server
                Connect();
            }

            // In any other case return to the login page
            else
                DI.UI.DispatcherThreadAction(() => DI.Application.GoToPage(ApplicationPage.Login));
        }

        /// <summary>
        /// Fired when server ask client to disconnect from the server
        /// </summary>
        protected override void OnDisconnected()
        {
            // Log it
            DI.Logger.LogDebugSource("Network disconnected");

            // Dont'try to reconnect if in the result page, because the test result has been already sent to the server
            if (DI.TestHost.IsShowingResultPage)
                return;

            // If not in reults page show login page
            if (!DI.TestHost.IsTestInProgress)
                DI.UI.DispatcherThreadAction(() => DI.Application.GoToPage(ApplicationPage.Login));
        }

        /// <summary>
        /// Fired when connection with the server has been established
        /// </summary>
        protected override void OnConnectionEstablished()
        {
            // Log it
            DI.Logger.LogDebugSource("Network connected");

            // Send info package with the information
            SendData(DI.Client.GetPackage());
            DI.Logger.LogDebugSource("Sending client info...");

            // Reset AttemptingToReconnect flag
            AttemptingToReconnect = false;

            // If we're in login page change page to the waiting for test page
            if (DI.Application.CurrentPage == ApplicationPage.Login)
            {
                DI.UI.DispatcherThreadAction(() => DI.Application.GoToPage(ApplicationPage.WaitingForTest));
            }
            //else
                // Notify the test host
                //IoCClient.DI.TestHost.NetworkReconnected();
            
            // Save current IP to the file, as connection was successful
            SaveNetworkConfigToFile();
        }
        
        /// <summary>
        /// Fired when data has been received from the server
        /// </summary>
        /// <param name="DataReceived"></param>
        protected override void OnDataReceived(DataPackage DataReceived)
        {
            switch (DataReceived.PackageType)
            {
                case PackageType.TestForm:
                    // Bind the newly received test
                    DI.TestHost.BindTest(DataReceived.Content as Test);
                    break;

                case PackageType.BeginTest:

                    var args = DataReceived.Content as TestStartupArgs;

                    DI.TestHost.SetupArguments(args);
                    DI.TestHost.StartTest();
                    break;

                case PackageType.StopTestForcefully:
                    DI.TestHost.AbortTest();
                    break;

            }
        }

        #endregion

        #region Config File Reading/Writing

        /// <summary>
        /// Initializes this client using config file, if config doesn't exist or is invalid default values are used
        /// </summary>
        private void InitializeUsingConfigFile()
        {
            // Get directory in appdata
            var directory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "//Testinator//";

            var ip = default(IPAddress);
            var port = 0;

            try
            {
                // Try to read data from file
                var fileContent = File.ReadAllText(directory + "ipconfig.txt").Trim();
                var separatorIndex = fileContent.IndexOf(';');

                if (separatorIndex == -1)
                    throw new Exception();

                var ipString = fileContent.Substring(0, separatorIndex);
                var portString = fileContent.Substring(separatorIndex + 1);

                ip = IPAddress.Parse(ipString);

                if (!NetworkHelpers.IsPortCorrect(portString))
                    throw new Exception();

                port = int.Parse(portString);

                Initialize(ip, port);
            }
            catch
            {
                // No need to do anything as network is already loaded with default valus
            }
            
        }

        /// <summary>
        /// Saves current IP and port number to config file
        /// </summary>
        private void SaveNetworkConfigToFile()
        {
            // Get directory in appdata
            var directory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Testinator\\";
            Directory.CreateDirectory(directory);

            // Save current state of IP
            // Using ';' as a separator
            File.WriteAllText(directory + "ipconfig.txt", $"{IPString};{Port}");
        }

        #endregion
    }
}