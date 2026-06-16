using System.IO;

namespace PetShopCare.Database {
    public static class DatabaseConfig {
        // Como configuramos o arquivo para "Copiar se for mais novo", 
        // ele estará na mesma pasta do executável durante o desenvolvimento.
        public static string ConnectionString => "Data Source=PetShopCare.db;Version=3;";
    }
}