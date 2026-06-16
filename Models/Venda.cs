using System;

namespace PetShopCare.Models {
    public class Venda {
        public int Id { get; set; }
        public DateTime DataVenda { get; set; }
        public int UsuarioId { get; set; }
        public int? ClienteId { get; set; } // Anulável, pois pode ser venda rápida sem cadastro
        public decimal ValorTotal { get; set; }
        public decimal Desconto { get; set; }
        public string FormaPagamento { get; set; }
        public string Status { get; set; }
    }
}