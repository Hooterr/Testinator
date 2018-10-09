﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Testinator.Core;

namespace Testinator.Client.Core
{
    /// <summary>
    /// The view model for the "waiting for test" page
    /// </summary>
    public class WaitingForTestViewModel : BaseViewModel
    {
        #region Public Properties

        /// <summary>
        /// The current users name
        /// </summary>
        public TextEntryViewModel Name { get; set; }

        /// <summary>
        /// The current users surname
        /// </summary>
        public TextEntryViewModel Surname { get; set; }

        /// <summary>
        /// The name of the test
        /// </summary>
        public string TestName => DI.TestHost.IsTestReceived ? DI.TestHost.CurrentTest.Info.Name : "";

        /// <summary>
        /// The duration of the test
        /// </summary>
        public TimeSpan TestDuration => DI.TestHost.IsTestReceived ? DI.TestHost.CurrentTest.Info.Duration : TimeSpan.Zero;

        /// <summary>
        /// How much score can user get from this test
        /// </summary>
        public int TestPossibleScore => DI.TestHost.IsTestReceived ? DI.TestHost.CurrentTest.TotalPointScore : 0;

        /// <summary>
        /// A flag indicating if we have any test to show,
        /// to show corresponding content in the WaitingPage
        /// </summary>
        public bool IsTestReceived => DI.TestHost.IsTestReceived;

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public WaitingForTestViewModel()
        {
            // Set input data
            Name = new TextEntryViewModel { Label = "Imię", OriginalText = DI.Client.Name };
            Surname = new TextEntryViewModel { Label = "Nazwisko", OriginalText = DI.Client.LastName };

            // Listen out for test which will come from server
            DI.TestHost.OnTestReceived += Update;

            // Hook to the events
            Name.ValueChanged += Name_ValueChanged;
            Surname.ValueChanged += Surname_ValueChanged;
        }

        #endregion

        #region Private Events

        /// <summary>
        /// Name value changes
        /// </summary>
        private void Name_ValueChanged()
        {
            DI.Client.Name = Name.OriginalText;
            DI.Application.Network.SendClientModelUpdate();
        }

        /// <summary>
        /// Surname value changes
        /// </summary>
        private void Surname_ValueChanged()
        {
            DI.Client.LastName = Surname.OriginalText;
            DI.Application.Network.SendClientModelUpdate();
        }

        #endregion

        #region Public Helpers

        /// <summary>
        /// Updates this view model. As it's properties are bound to the IoC, changing 
        /// something in IoC doesn't fire PropertyChanged event in this view model.
        /// Therefore, properties need to be updated manually
        /// </summary>
        public void Update()
        {
            OnPropertyChanged(nameof(IsTestReceived));
            OnPropertyChanged(nameof(TestName));
            OnPropertyChanged(nameof(TestDuration));
            OnPropertyChanged(nameof(TestPossibleScore));
        }

        #endregion
    }
}
