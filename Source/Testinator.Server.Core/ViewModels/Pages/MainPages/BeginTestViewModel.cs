﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Timers;
using System.Windows.Input;
using Testinator.Core;

namespace Testinator.Server.Core
{
    /// <summary>
    /// The view model for the begin test page
    /// </summary>
    public class BeginTestViewModel : BaseViewModel
    {
        #region Public Properties

        /// <summary>
        /// A list of connected clients
        /// </summary>
        public ObservableCollection<ClientModel> ClientsConnected => IoCServer.Network.Clients;

        /// <summary>
        /// All clients that are currently taking the test
        /// </summary>
        public ObservableCollection<ClientModelExtended> ClientsTakingTheTest => IoCServer.TestHost.Clients;

        /// <summary>
        /// The test which is choosen by user on the list
        /// </summary>
        public Test CurrentTest => IoCServer.TestHost.Test;

        /// <summary>
        /// The number of connected clients
        /// </summary>
        public int ClientsNumber => IoCServer.Network.ConnectedClientCount;

        /// <summary>
        /// The number of the questions in the test
        /// </summary>
        public int QuestionsCount => CurrentTest.Questions.Count;

        /// <summary>
        /// A flag indicating whether server has started
        /// </summary>
        public bool IsServerStarted => IoCServer.Network.IsRunning;

        /// <summary>
        /// A flag indicating whether test has started
        /// </summary>
        public bool IsTestInProgress => IoCServer.TestHost.IsTestInProgress;

        /// <summary>
        /// The time that is left to the end of the test
        /// </summary>
        public TimeSpan TimeLeft => IoCServer.TestHost.TimeLeft;

        /// <summary>
        /// The server's ip
        /// </summary>
        public string ServerIpAddress => IoCServer.Network.Ip;

        /// <summary>
        /// The server's port
        /// </summary>
        public string ServerPort { get; set; } = IoCServer.Network.Port.ToString();

        /// <summary>
        /// Indicates if the result page should be allowed for the users
        /// </summary>
        public bool ResultPageAllowed { get; set; } = true;

        /// <summary>
        /// Indicates if the test should be held in fullscreen mode
        /// </summary>
        public bool FullScreenMode { get; set; } = false;

        #region Error flags

        /// <summary>
        /// Indicates if there is not enough clients to start the test
        /// </summary>
        public bool NotEnoughClients => ClientsNumber == 0;

        /// <summary>
        /// Indicates if the test is not selected
        /// </summary>
        public bool TestNotSelected => !TestListViewModel.Instance.IsAnySelected;

        /// <summary>
        /// Indicates if the test can be sent to the clients
        /// </summary>
        public bool CanSendTest => !NotEnoughClients && !TestNotSelected;

        #endregion

        #endregion

        #region Commands

        /// <summary>
        /// The command to start the server (allows clients to connect)
        /// </summary>
        public ICommand StartServerCommand { get; private set; }

        /// <summary>
        /// The command to stop the server (disable client connections)
        /// </summary>
        public ICommand StopServerCommand { get; private set; }

        /// <summary>
        /// The command to change subpage to test list page
        /// </summary>
        public ICommand ChangePageTestListCommand { get; private set; }

        /// <summary>
        /// The command to change subpage to test info page
        /// </summary>
        public ICommand ChangePageTestInfoCommand { get; private set; }

        /// <summary>
        /// The command to start choosen test
        /// </summary>
        public ICommand BeginTestCommand { get; private set; }

        /// <summary>
        /// The command to stop the test (test disappears completely)
        /// </summary>
        public ICommand StopTestCommand { get; private set; }
        
        /// <summary>
        /// The command to finish the test (results are collected before a timer ends)
        /// </summary>
        public ICommand FinishTestCommand { get; private set; }

        /// <summary>
        /// The command to exit from the resultpage
        /// </summary>
        public ICommand ResultPageExitCommand { get; private set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public BeginTestViewModel()
        {
            // Create commands
            StartServerCommand = new RelayCommand(StartServer);
            StopServerCommand = new RelayCommand(StopServer);
            ChangePageTestListCommand = new RelayCommand(ChangePageList);
            ChangePageTestInfoCommand = new RelayCommand(ChangePageInfo);
            BeginTestCommand = new RelayCommand(BeginTest);
            StopTestCommand = new RelayCommand(StopTest);
            FinishTestCommand = new RelayCommand(FinishTest);
            ResultPageExitCommand = new RelayCommand(ResultPageExit);

            // Load every test from files
            TestListViewModel.Instance.LoadItems();

            // Hook to timer event
            IoCServer.TestHost.OnTimerUpdated += TimerUpdated;

            // Hook to the server event
            IoCServer.Network.OnClientConnected += Network_OnClientConnected;
            IoCServer.Network.OnClientDisconnected += Network_OnClientDisconnected;

            // Hook to the test list event
            TestListViewModel.Instance.ItemSelected += TestListViewModel_TestSelected;

            // Hook to the test host evet
            IoCServer.TestHost.TestFinished += ChangePageToResults;
        }

        #endregion

        #region Command Methods

        /// <summary>
        /// Starts the server
        /// </summary>
        private void StartServer()
        {
            // If port is not valid, dont start the server
            if (!NetworkHelpers.IsPortCorrect(ServerPort))
            {
                // Show a message box with info about it
                IoCServer.UI.ShowMessage(new MessageBoxDialogViewModel
                {
                    Title = "Niepoprawne dane!",
                    Message = "Niepoprawny port.",
                    OkText = "Ok"
                });
                return;
            }

            // Set network data
            IoCServer.Network.Ip = ServerIpAddress;
            IoCServer.Network.Port = int.Parse(ServerPort);

            // Start the server
            IoCServer.Network.Start();

            UpdateView();
        }

        /// <summary>
        /// Stops the server
        /// </summary>
        private void StopServer()
        {
            // Check if any test is already in progress
            if (IoCServer.TestHost.IsTestInProgress)
            {
                // Ask the user if he wants to stop the test
                var vm = new ResultBoxDialogViewModel()
                {
                    Title = "Test w trakcie!",
                    Message = "Test jest w trakcie. Czy chcesz go przerwać?",
                    AcceptText = "Tak",
                    CancelText = "Nie",
                };
                IoCServer.UI.ShowMessage(vm);
                
                // If he agreed
                if (vm.UserResponse)
                    // Stop the test
                    StopTestForcefully();
                else
                    return;

                UpdateView();
            }
            
            // Stop the server
            IoCServer.Network.Stop();

            UpdateView();

            // Go to the initial page
            IoCServer.Application.GoToBeginTestPage(ApplicationPage.BeginTestInitial);
        }

        /// <summary>
        /// Changes the subpage to test choose page
        /// </summary>
        private void ChangePageList()
        {
            // If server isnt started, dont change the page
            if (!IsServerStarted)
            {
                // Show a message box with info about it
                IoCServer.UI.ShowMessage(new MessageBoxDialogViewModel
                {
                    Title = "Serwer nie włączony!",
                    Message = "By zmienić stronę, należy przedtem włączyć serwer.",
                    OkText = "Ok"
                });
                return;
            }

            // Simply go to target page
            IoCServer.Application.GoToBeginTestPage(ApplicationPage.BeginTestChoose);
        }

        /// <summary>
        /// Changes the subpage to test info page
        /// </summary>
        private void ChangePageInfo()
        {
            // Meanwhile lock the clients list and send them the test 
            IoCServer.TestHost.LockClients();
            
            // If there is no enough users to start the test, show the message and don't send the test
            if (IoCServer.TestHost.Clients.Count == 0)
            {
                IoCServer.UI.ShowMessage(new MessageBoxDialogViewModel
                {
                    Title = "Brak użytkowników",
                    Message = "Nie można ropocząć testu. Brak użytkowników, którzy mogą go rozpocząć.",
                    OkText = "OK"
                });
                return;
            }

            // Then go to the info page
            IoCServer.Application.GoToBeginTestPage(ApplicationPage.BeginTestInfo);

            IoCServer.TestHost.SendTest();
        }

        /// <summary>
        /// Starts the test
        /// </summary>
        private void BeginTest()
        {
            // Send the args before startting
            IoCServer.TestHost.SendTestArgs(new TestStartupArgsPackage()
            {
                FullScreenMode = FullScreenMode,
                IsResultsPageAllowed = ResultPageAllowed,
            });

            IoCServer.TestHost.TestStart();
            IoCServer.Application.GoToBeginTestPage(ApplicationPage.BeginTestInProgress);
        }

        /// <summary>
        /// Stops the test (test disappears completely, like it didn't even happened)
        /// </summary>
        private void StopTest()
        {
            // Ask the user if he wants to stop the test
            var vm = new ResultBoxDialogViewModel()
            {
                Title = "Przerywanie testu",
                Message = "Czy na pewno chcesz przerwać test?",
                AcceptText = "Tak",
                CancelText = "Nie",
            };
            IoCServer.UI.ShowMessage(vm);

            // If his will match
            if (vm.UserResponse)
                // Stop the test
                StopTestForcefully();
        }

        /// <summary>
        /// Finishes the test before a timer ends, results are collected here
        /// </summary>
        private void FinishTest()
        {
            // TODO: Get the results from users (even empty ones)
            //       Then finish the test etc.
        }

        /// <summary>
        /// Exits from the result page
        /// </summary>
        private void ResultPageExit()
        {
            if (IoCServer.Network.IsRunning)
                IoCServer.Application.GoToBeginTestPage(ApplicationPage.BeginTestChoose);
            else
                IoCServer.Application.GoToBeginTestPage(ApplicationPage.BeginTestInitial);
        }

        #endregion

        #region Private Helpers

        /// <summary>
        /// Fired when timeleft timer is updated
        /// </summary>
        private void TimerUpdated()
        {
            UpdateView();
        }

        #endregion

        #region Private Event Methods

        /// <summary>
        /// Fired when the test finishes
        /// </summary>
        private void ChangePageToResults()
        {
            // Jump on the dispatcher thread to change page
            var uiContext = SynchronizationContext.Current;
            uiContext.Send(x => IoCServer.Application.GoToBeginTestPage(ApplicationPage.BeginTestResults), null);
        }

        /// <summary>
        /// Fired when test is selected
        /// </summary>
        private void TestListViewModel_TestSelected()
        {
            UpdateView();
        }

        /// <summary>
        /// Fired when a client disconnects
        /// </summary>
        /// <param name="obj"></param>
        private void Network_OnClientDisconnected(ClientModel obj)
        {
            UpdateView();
        }

        /// <summary>
        /// Fired when a client connects
        /// </summary>
        /// <param name="obj"></param>
        private void Network_OnClientConnected(ClientModel obj)
        {
            UpdateView();
        }

        /// <summary>
        /// Updates the view and all the properties
        /// </summary>
        private void UpdateView()
        {
            OnPropertyChanged(nameof(ClientsNumber));
            OnPropertyChanged(nameof(NotEnoughClients));
            OnPropertyChanged(nameof(CanSendTest));
            OnPropertyChanged(nameof(TestNotSelected));
            OnPropertyChanged(nameof(CanSendTest));
            OnPropertyChanged(nameof(TimeLeft));
            OnPropertyChanged(nameof(TestNotSelected));
            OnPropertyChanged(nameof(IsServerStarted));
        }

        /// <summary>
        /// Stops the test forcefully
        /// </summary>
        private void StopTestForcefully()
        {
            IoCServer.TestHost.TestStopForcefully();
            IoCServer.Application.GoToBeginTestPage(ApplicationPage.BeginTestInitial);
        }

        #endregion
    }
}