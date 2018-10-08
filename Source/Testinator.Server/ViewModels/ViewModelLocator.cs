using Testinator.Server.Core;

using static Testinator.Server.Core.DI;

namespace Testinator.Server
{
    /// <summary>
    /// Locates view models from the IoC for use in binding in Xaml files
    /// </summary>
    public class ViewModelLocator
    {
        #region Public Properties

        /// <summary>
        /// Singleton instance of the locator
        /// </summary>
        public static ViewModelLocator Instance { get; private set; } = new ViewModelLocator();

        /// <summary>
        /// The application view model
        /// </summary>
        public static ApplicationViewModel ApplicationViewModel => Application;

        /// <summary>
        /// The shortcut to the test host
        /// </summary>
        public static TestHost TestHost => DI.TestHost;

        /// <summary>
        /// The shortcut to the test editor
        /// </summary>
        public static TestEditor TestEditor => DI.TestEditor;


        #endregion
    }
}
