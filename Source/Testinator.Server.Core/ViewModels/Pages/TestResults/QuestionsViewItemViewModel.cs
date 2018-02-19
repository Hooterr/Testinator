﻿using System.Collections.Generic;

namespace Testinator.Server.Core
{
    /// <summary>
    /// A view model for items to display in data grid in questions-view mode
    /// </summary>
    public class QuestionsViewItemViewModel
    {
        /// <summary>
        /// The name of the student
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The surname of the student
        /// </summary>
        public string Surname { get; set; }

        /// <summary>
        /// Points the user got during the test starting from first question
        /// NOTE: may be changed when the view is finished 
        /// </summary>
        public List<int> QuestionsPoints { get; set; } = new List<int>();

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public QuestionsViewItemViewModel()
        {

        }

        #endregion
    }
}