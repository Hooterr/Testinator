using Testinator.Server.Core;
using Testinator.UICore;

using static Testinator.Server.Core.DI;

namespace Testinator.Server
{
    /// <summary>
    /// Interaction logic for SettingsPage.xaml
    /// </summary>
    public partial class SettingsPage : BasePage<ApplicationSettingsViewModel>
    {
        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public SettingsPage() : base()
        {
            InitializeComponent();

            // Attach the single instance of IoC view model to this page
            DataContext = Settings;
        }

        #endregion
    }
}
