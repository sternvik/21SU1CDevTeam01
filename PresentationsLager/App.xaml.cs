using AffärsLager.Controllers;
using System.Configuration;
using System.Data;
using System.Windows;

namespace PresentationsLager
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private SchemaController? _schemaController;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Starta schemalagda jobb (PDF, bokföring, kundexport)
            _schemaController = new SchemaController();
            _schemaController.StartaSchemalagdaJobb();
        }
        protected override void OnExit(ExitEventArgs e)
        {
            // Städa upp schemaläggning
            _schemaController?.Dispose();
            base.OnExit(e);
        }
    }

}
