using Dna;
using Microsoft.Extensions.DependencyInjection;
using Testinator.Client.Core;

namespace Testinator.Client
{
    public static class FrameworkConstructionExtensions
    {
        public static FrameworkConstruction AddApplicationServices(this FrameworkConstruction construction)
        {
            construction.Services.AddSingleton<TestHost>();
            construction.Services.AddTransient<IUIManager, UIManager>();

            return construction;
        }

        public static FrameworkConstruction AddApplicationViewModels(this FrameworkConstruction construction)
        {
            construction.Services.AddSingleton<ApplicationViewModel>();
            construction.Services.AddSingleton<ClientModel>();
            construction.Services.AddSingleton<ApplicationViewModel>();


            return construction;
        }
    }
}
