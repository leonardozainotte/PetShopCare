using System;

namespace PetShopCare.Models {
    public class Usuario {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Login { get; set; } // Mapeará a coluna 'Usuario' do banco
        public string SenhaHash { get; set; }
        public string Cargo { get; set; }
        public string Status { get; set; }
        public DateTime DataCadastro { get; set; }
    }
}