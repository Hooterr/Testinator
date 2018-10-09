using Dna;
using Microsoft.Extensions.Logging;

namespace Testinator.Client.Core
{
    /// <summary>
    /// DI shortcuts
    /// </summary>
    public static class DI
    {
        public static ApplicationViewModel Application => Framework.Service<ApplicationViewModel>();
        
        /// <summary>
        /// A shortcut to access the <see cref="ClientModel"/>
        /// </summary>
        public static ClientModel Client => Framework.Service<ClientModel>();

        /// <summary>
        /// A shortcut to access the <see cref="TestHost"/>
        /// </summary>
        public static TestHost TestHost => Framework.Service<TestHost>();

        /// <summary>
        /// A shortcut to access the <see cref="IUIManager"/>
        /// </summary>
        public static IUIManager UI => Framework.Service<IUIManager>();

        /// <summary>
        /// A shortcut to access the <see cref="ILogFactory"/>
        /// </summary>
        public static ILogger Logger => FrameworkDI.Logger;
    }
}
