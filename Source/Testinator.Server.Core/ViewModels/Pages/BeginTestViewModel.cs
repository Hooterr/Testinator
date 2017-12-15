﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        public ObservableCollection<ClientModel> ClientsConnected { get; set; }

        /// <summary>
        /// The list of every test user can choose in the list
        /// </summary>
        public List<Test> TestList { get; set; }

        /// <summary>
        /// The test which is choosen by user on the list
        /// </summary>
        public Test CurrentTest { get; set; }

        /// <summary>
        /// A flag indicating whether server has started
        /// </summary>
        public bool IsServerStarted { get; set; }

        /// <summary>
        /// A flag indicating whether test has been sent to the clients
        /// </summary>
        public bool IsTestSent { get; set; }

        /// <summary>
        /// A flag indicating whether test has started
        /// </summary>
        public bool IsTestStarted { get; set; }

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
        /// The command to choose a test from the list
        /// </summary>
        public ICommand ChooseTestCommand { get; private set; }

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
            ChooseTestCommand = new RelayCommand(ChooseTest);
        }

        #endregion

        #region Command Methods

        /// <summary>
        /// Starts the server
        /// </summary>
        private void StartServer()
        {
            // Indicate that server is starting
            IsServerStarted = true;

            // Prepare for client connections
            ClientsConnected = new ObservableCollection<ClientModel>();
        }

        /// <summary>
        /// Stops the server
        /// </summary>
        private void StopServer()
        {
            // TODO: Advanced mechanic here
            // For now - simply toggle the flag
            IsServerStarted = false;
        }

        /// <summary>
        /// Changes the subpage to test choose page
        /// </summary>
        private void ChangePageList()
        {
            // Simply go to target page
            IoCServer.Application.GoToBeginTestPage(ApplicationPage.BeginTestChoose);
        }

        /// <summary>
        /// Changes the subpage to test info page
        /// </summary>
        private void ChangePageInfo()
        {
            // Check if user has choosen any test
            if (CurrentTest == null)
                return;

            // Then go to info page
            IoCServer.Application.GoToBeginTestPage(ApplicationPage.BeginTestInfo);
        }

        /// <summary>
        /// Chooses a test from the list
        /// </summary>
        private void ChooseTest()
        {

        }

        #endregion
    }
}
