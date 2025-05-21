using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RestaurantComenzi.Data;
using System.Windows;

namespace Restaurant
{
    public partial class App : Application
    {
        public static IServiceProvider ServiceProvider { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            var services = new ServiceCollection();

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer("Server=TEODOR;Database=RestaurantComenzi;Trusted_Connection=True; TrustServerCertificate=True;"));

            ServiceProvider = services.BuildServiceProvider();

            base.OnStartup(e);
        }
    }
}
