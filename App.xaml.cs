using System.Windows;
using PetShopCare.Database;

namespace PetShopCare
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Inicia o banco de dados
            DatabaseConfig.InitializeDatabase();
        }
    }
}