﻿using System.Threading.Tasks;
using Testinator.Core;

namespace Testinator.Client.Core
{
    /// <summary>
    /// The UI manager that handles any UI interaction in the application
    /// </summary>
    public interface IUIManager
    {
        /// <summary>
        /// Changes page in the application
        /// NOTE: Used as a work around thread problem in client netowrk
        /// </summary>
        /// <param name="page">The target page</param>
        /// <param name="viewmodel">The corresponding viewmodel, null by default</param>
        void ChangePage(ApplicationPage page, BaseViewModel viewmodel = null);

        /// <summary>
        /// Displays a single message box to the user
        /// </summary>
        /// <param name="viewModel">The view model</param>
        /// <param name="isAlreadyOnUIThread">Indicates if caller is on UIThread, default as true</param>
        /// <returns></returns>
        Task ShowMessage(MessageBoxDialogViewModel viewModel, bool isAlreadyOnUIThread = true);

        /// <summary>
        /// Displays a result box to the user and catch the result
        /// </summary>
        /// <param name="viewModel">The view model</param>
        /// <param name="isAlreadyOnUIThread">Indicates if caller is on UIThread, default as true</param>
        /// <returns></returns>
        Task ShowMessage(ResultBoxDialogViewModel viewModel, bool isAlreadyOnUIThread = true);
    }
}
