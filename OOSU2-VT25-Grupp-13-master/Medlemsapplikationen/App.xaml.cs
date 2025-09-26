using Medlemsapplikationen.Navigation;
using Medlemsapplikationen.ViewModels;
using Medlemsapplikationen.Views;
using Microsoft.Extensions.DependencyInjection;
using System.Configuration;
using System.Data;
using System.Windows;

namespace Medlemsapplikationen
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static IServiceProvider ServiceProvider { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            var services = new ServiceCollection();

            services.AddSingleton<INavigationService, NavigationService>();

            services.AddTransient<AllaMedlemmarViewModel>();
            services.AddTransient<BokaTräningspassViewModel>();
            services.AddTransient<GenomfördaPassViewModel>();
            services.AddTransient<KommandePassViewmodel>();
            services.AddTransient<LoginWindowViewModel>();
            services.AddTransient<MainWindowViewModel>();
            services.AddTransient<MedlemMenyViewModel>();
            services.AddTransient<MittKontoViewModel>();
            services.AddTransient<RegistreraViewModel>();

            ServiceProvider = services.BuildServiceProvider();

            base.OnStartup(e);
        }
    }

}
