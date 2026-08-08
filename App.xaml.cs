using PetShopCare.Database;
using PetShopCare.ViewModels;
using PetShopCare.Views;
using System;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Threading;

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

        // ===============================================
        // REDE DE SEGURANÇA: CAPTURA DE EXCEÇÕES GLOBAIS
        // ===============================================
        private void Application_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            // 1. Prepara a mensagem amigável para o utilizador
            string mensagemAmigavel = "Ops! Ocorreu um erro inesperado no sistema.\n" +
                                      "O problema foi registado no nosso arquivo de logs.\n\n" +
                                      $"Detalhe técnico: {e.Exception.Message}";

            MessageBox.Show(mensagemAmigavel, "Falha no Sistema", MessageBoxButton.OK, MessageBoxImage.Warning);

            // 2. Grava o erro detalhado num ficheiro de texto oculto na pasta do executável
            try
            {
                string caminhoLog = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "error_log.txt");
                string detalheLog = $"[{DateTime.Now:dd/MM/yyyy HH:mm:ss}] ERRO CRÍTICO:\n" +
                                    $"Mensagem: {e.Exception.Message}\n" +
                                    $"Rastro (Stack Trace): {e.Exception.StackTrace}\n" +
                                    new string('-', 50) + "\n";

                File.AppendAllText(caminhoLog, detalheLog);
            }
            catch
            {
                // Se a gravação do log falhar (por ex: falta de permissão na pasta), ignoramos para não travar a aplicação
            }

            // 3. Diz ao WPF que o erro foi tratado e a aplicação NÃO deve fechar
            e.Handled = true;
        }
    }
}