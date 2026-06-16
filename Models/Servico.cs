namespace PetShopCare.Models {
    public class Servico {
        public int Id { get; set; }
        public string Nome { get; set; }
        public decimal Preco { get; set; }
        public int TempoEstimadoMinutos { get; set; }
    }
}