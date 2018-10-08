using Dna;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Testinator.Core;
using Testinator.Server.Core;
using Testinator.Server.DataAccess;
using Testinator.Server.Repositories;
using Testinator.Server.Repositories.Settings;

namespace Testinator.Server
{
    public static class FrameworkConstrctionExtensions
    {
        // TODO: naming convetion, thease are not viewmodels
        public static FrameworkConstruction AddApplicationViewModels(this FrameworkConstruction construction)
        {
            construction.Services.AddSingleton<ApplicationViewModel>();
            construction.Services.AddSingleton<ApplicationSettingsViewModel>();

            return construction;
        }

        public static FrameworkConstruction AddApplicationServices(this FrameworkConstruction construction)
        {
            construction.Services.AddTransient<IUIManager, UIManager>();
            construction.Services.AddTransient<ServerNetwork>();
            construction.Services.AddTransient<TestHost>();
            construction.Services.AddTransient<TestEditor>();

            // Local storage
            construction.Services.AddDbContext<TestinatorAppDbContext>(options =>
            {
                // Setup connection string
                options.UseSqlite(construction.Configuration.GetConnectionString("ClientDataStoreConnection"));
            }, contextLifetime: ServiceLifetime.Transient);

            construction.Services.AddTransient<ISettingsRepository>(provider => new SettingsRepository(provider.GetService<TestinatorAppDbContext>()));

            return construction;
        }
    }
}
