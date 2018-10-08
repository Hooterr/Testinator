using Dna;
using Microsoft.Extensions.Logging;
using Testinator.Core;
using Testinator.Server.DataAccess;

namespace Testinator.Server.Core
{
    /// <summary>
    /// Shortcut for the dependency Injection
    /// </summary>
    public static class DI
    {
        /// <summary>
        /// The UI service
        /// </summary>
        public static IUIManager UI => Framework.Service<IUIManager>();

        /// <summary>
        /// Application as a whole
        /// </summary>
        public static ApplicationViewModel Application => Framework.Service<ApplicationViewModel>();
        
        /// <summary>
        /// Settings
        /// </summary>
        public static ApplicationSettingsViewModel Settings => Framework.Service<ApplicationSettingsViewModel>();

        /// <summary>
        /// Network service
        /// </summary>
        public static ServerNetwork Network => Framework.Service<ServerNetwork>();
        
        /// <summary>
        /// Test host service
        /// </summary>
        public static TestHost TestHost => Framework.Service<TestHost>();

        /// <summary>
        /// TestEditor service
        /// </summary>
        public static TestEditor TestEditor => Framework.Service<TestEditor>();

        /// <summary>
        /// The whole db context of the application
        /// </summary>
        public static TestinatorAppDbContext AppDbContext => Framework.Service<TestinatorAppDbContext>();

        public static ILogger Logger => FrameworkDI.Logger;

    }
}
