using System;

namespace PetShopCare.Models {
    public class Pet {
        public int Id { get; set; }
        public int ClienteId { get; set; } // Chave Estrangeira
        public string Nome { get; set; }
        public string Especie { get; set; }
        public string Raca { get; set; }
        public string Sexo { get; set; }
        public DateTime? DataNascimento { get; set; } // A interrogação permite que a data seja nula
        public decimal? Peso { get; set; }
        public string Cor { get; set; }
        public string Observacoes { get; set; }
    }
}