using Testinator.Client.Core;
using Testinator.Core;

namespace Testinator.Client
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
        public static ApplicationViewModel ApplicationViewModel => DI.Application;

        /// <summary>
        /// The test host
        /// </summary>
        public static TestHost TestHost => DI.TestHost;

        /// <summary>
        /// The client (user) model
        /// </summary>
        public static ClientModel ClientModel => DI.Client;

        #endregion
    }
}
