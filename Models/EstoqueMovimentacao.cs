using System;

namespace PetShopCare.Models {
    public class EstoqueMovimentacao {
        public int Id { get; set; }
        public int ProdutoId { get; set; }
        public int Quantidade { get; set; }
        public string TipoMovimentacao { get; set; } // 'Entrada' ou 'Saida'
        public string Origem { get; set; }
        public DateTime DataMovimentacao { get; set; }
        public int UsuarioId { get; set; }
    }
}