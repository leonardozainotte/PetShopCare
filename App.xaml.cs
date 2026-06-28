using System.Globalization;
using System.Threading;
using System.Windows;
using System.Windows.Markup;
using PetShopCare.Database;

namespace PetShopCare
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // 1. Inicia o banco de dados (Sua lógica original mantida intacta)
            DatabaseConfig.InitializeDatabase();

            // 2. Define a cultura padrão do sistema para o Brasil (Moeda = R$, Data = dd/MM/yyyy)
            var cultureInfo = new CultureInfo("pt-BR");
            Thread.CurrentThread.CurrentCulture = cultureInfo;
            Thread.CurrentThread.CurrentUICulture = cultureInfo;

            // 3. Força o motor visual do XAML a renderizar seguindo a cultura definida acima
            FrameworkElement.LanguageProperty.OverrideMetadata(
                typeof(FrameworkElement),
                new FrameworkPropertyMetadata(XmlLanguage.GetLanguage(cultureInfo.IetfLanguageTag)));
        }
    }
}