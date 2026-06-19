namespace PetShopCare.Models
{
    public class Cliente
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string CPF { get; set; } = string.Empty;
        public string Telefone { get; set; } = string.Empty;
        public string? Endereco { get; set; }
        public string? Email { get; set; }
        public string? DataCadastro { get; set; }
    }
}