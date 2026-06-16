namespace PetShopCare.Models {
    public class Produto {
        public int Id { get; set; }
        public string CodigoBarras { get; set; }
        public string Nome { get; set; }
        public string Descricao { get; set; }
        public string Marca { get; set; }
        public string Categoria { get; set; }
        public decimal PrecoCusto { get; set; }
        public decimal PrecoVenda { get; set; }
        public int EstoqueAtual { get; set; }
        public int EstoqueMinimo { get; set; }
    }
}