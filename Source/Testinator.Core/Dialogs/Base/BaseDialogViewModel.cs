﻿namespace Testinator.Core
{
    /// <summary>
    /// The base view model for any dialogs
    /// </summary>
    public abstract class BaseDialogViewModel : BaseViewModel
    {
        /// <summary>
        /// The title of the dialog
        /// </summary>
        public string Title { get; set; }

    }
}
