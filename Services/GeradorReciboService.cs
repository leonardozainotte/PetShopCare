using PetShopCare.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace PetShopCare.Services {
    public class GeradorReciboService {

        public void GerarEImprimirRecibo(Venda venda, List<ItemVenda> itens, Cliente? cliente) {
            var html = new StringBuilder();

            // Cabeçalho e CSS (Estilo de cupom não fiscal)
            html.AppendLine("<html><head><meta charset='UTF-8'><style>");
            html.AppendLine("body { font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; color: #333; font-size: 14px; max-width: 400px; margin: 0 auto; padding: 20px; }");
            html.AppendLine("h2 { text-align: center; margin-bottom: 5px; color: #2C3E50; }");
            html.AppendLine(".subtitle { text-align: center; font-size: 12px; color: #7F8C8D; margin-bottom: 20px; border-bottom: 1px dashed #ccc; padding-bottom: 10px; }");
            html.AppendLine("table { width: 100%; border-collapse: collapse; margin-top: 15px; }");
            html.AppendLine("th { text-align: left; border-bottom: 1px solid #ccc; padding-bottom: 5px; font-size: 12px; }");
            html.AppendLine("td { padding: 5px 0; font-size: 13px; }");
            html.AppendLine(".total-row { border-top: 1px dashed #ccc; font-weight: bold; font-size: 16px; }");
            html.AppendLine(".footer { text-align: center; margin-top: 30px; font-size: 12px; color: #7F8C8D; }");
            html.AppendLine(".text-right { text-align: right; }");
            html.AppendLine("</style></head><body>");

            // Informações da Loja
            html.AppendLine("<h2>PETSHOP CARE</h2>");
            html.AppendLine($"<div class='subtitle'>Comprovante de Venda<br>Data: {venda.DataVenda:dd/MM/yyyy HH:mm}</div>");

            // Dados do Cliente (Se houver)
            if (cliente != null) {
                html.AppendLine($"<div><strong>Cliente:</strong> {cliente.Nome}</div>");
                if (!string.IsNullOrEmpty(cliente.CPF))
                    html.AppendLine($"<div><strong>CPF:</strong> {cliente.CPF}</div>");
            }
            else {
                html.AppendLine("<div><strong>Cliente:</strong> Consumidor Final</div>");
            }
            html.AppendLine($"<div><strong>Pagamento:</strong> {venda.FormaPagamento}</div>");

            // Tabela de Itens
            html.AppendLine("<table>");
            html.AppendLine("<tr><th>Qtd</th><th>Produto</th><th class='text-right'>Subtotal</th></tr>");

            foreach (var item in itens) {
                html.AppendLine("<tr>");
                html.AppendLine($"<td>{item.Quantidade}x</td>");
                html.AppendLine($"<td>{item.ProdutoNome}</td>");
                html.AppendLine($"<td class='text-right'>R$ {item.Subtotal:N2}</td>");
                html.AppendLine("</tr>");
            }

            // Totais
            html.AppendLine($"<tr class='total-row'><td colspan='2' style='padding-top:10px;'>TOTAL GERAL</td>");
            html.AppendLine($"<td class='text-right' style='padding-top:10px;'>R$ {venda.ValorTotal:N2}</td></tr>");
            html.AppendLine("</table>");

            // Rodapé
            html.AppendLine("<div class='footer'>Obrigado pela preferência e volte sempre!<br>Volte a nos visitar com o seu pet.</div>");
            html.AppendLine("</body></html>");

            // 1. Salvar num ficheiro temporário
            string caminhoArquivo = Path.Combine(Path.GetTempPath(), $"ReciboVenda_{DateTime.Now:yyyyMMddHHmmss}.html");
            File.WriteAllText(caminhoArquivo, html.ToString());

            // 2. Abrir no navegador padrão (Permite ao usuário dar Ctrl+P para imprimir ou salvar PDF)
            AbrirNoNavegador(caminhoArquivo);
        }

        private void AbrirNoNavegador(string url) {
            try {
                Process.Start(new ProcessStartInfo {
                    FileName = url,
                    UseShellExecute = true // Necessário no .NET Core / .NET 6+ para abrir URLs ou arquivos
                });
            }
            catch (Exception ex) {
                System.Windows.MessageBox.Show("O comprovante foi gerado, mas não foi possível abrir o navegador. Erro: " + ex.Message);
            }
        }
    }
}