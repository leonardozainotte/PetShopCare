using System;

namespace PetShopCare.Models
{
    public class OrdemServico
    {
        // Propriedades reais do Banco de Dados
        public int Id { get; set; }
        public int ClienteId { get; set; }
        public int PetId { get; set; }
        public int ServicoId { get; set; }
        public int UsuarioResponsavelId { get; set; }
        public DateTime DataHoraAgendamento { get; set; }
        public decimal Valor { get; set; }
        public string Status { get; set; }
        public string Observacoes { get; set; }

        // Propriedades estendidas (Usadas exclusivamente para exibir texto na Interface Gráfica)
        public string ClienteNome { get; set; } = string.Empty;
        public string PetNome { get; set; } = string.Empty;
        public string ServicoNome { get; set; } = string.Empty;
    }
}