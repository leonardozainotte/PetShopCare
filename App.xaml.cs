using PetShopCare.Database;
using PetShopCare.ViewModels;
using PetShopCare.Views;
using System.Globalization;
using System.Threading;
using System.Windows;
using System.Windows.Markup;

namespace PetShopCare
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            DatabaseConfig.InitializeDatabase();

            var cultureInfo = new CultureInfo("pt-BR");
            Thread.CurrentThread.CurrentCulture = cultureInfo;
            Thread.CurrentThread.CurrentUICulture = cultureInfo;

            FrameworkElement.LanguageProperty.OverrideMetadata(
                typeof(FrameworkElement),
                new FrameworkPropertyMetadata(XmlLanguage.GetLanguage(cultureInfo.IetfLanguageTag)));

            // ====================================================================
            // MANTÉM O PROCESSO VIVO INDEPENDENTE DE QUAIS JANELAS ESTÃO ABERTAS
            // ====================================================================
            Application.Current.ShutdownMode = ShutdownMode.OnExplicitShutdown;

            bool rodandoSistema = true;

            while (rodandoSistema)
            {
                var loginViewModel = new LoginViewModel();
                var loginView = new LoginView
                {
                    DataContext = loginViewModel
                };

                // Abre sempre como Dialog (Isso corrige o erro que você encontrou)
                bool? resultadoLogin = loginView.ShowDialog();

                if (resultadoLogin == true && loginViewModel.UsuarioAutenticado != null)
                {
                    var mainViewModel = new MainViewModel(loginViewModel.UsuarioAutenticado);
                    var mainView = new MainView
                    {
                        DataContext = mainViewModel
                    };

                    Application.Current.MainWindow = mainView;
                    mainView.ShowDialog();

                    if (!mainViewModel.IsLogoutRequested)
                    {
                        rodandoSistema = false;
                    }
                }
                else
                {
                    rodandoSistema = false;
                }
            }

            // Desliga a aplicação de forma limpa e segura
            Shutdown();
        }
    }
}