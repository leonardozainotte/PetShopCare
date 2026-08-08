using System;

namespace PetShopCare.Models
{

    // O nosso novo Enum de permissões
    public enum PerfilAcesso
    {
        Administrador = 1,
        Atendimento = 2,
        Operacional = 3
    }

    public class Usuario
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Login { get; set; } // Mapeará a coluna 'Usuario' do banco
        public string SenhaHash { get; set; }
        public string Cargo { get; set; }
        public string Status { get; set; }
        public DateTime DataCadastro { get; set; }
        public PerfilAcesso Perfil { get; set; }
    }
}