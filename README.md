# PetShop ERP - Etapa 2: Arquitetura MVVM e Setup de Infraestrutura

Este repositório reflete a segunda fase de desenvolvimento do **PetShopCare**, focada na criação da estrutura base em **WPF com .NET 8** e na injeção do banco de dados SQLite.

## 🏗️ Padrão Arquitetural: MVVM

O sistema foi estruturado adotando estritamente o padrão **Model-View-ViewModel (MVVM)** para garantir o baixo acoplamento entre a Interface de Usuário (UI) e as regras de negócio.

A solução está dividida nos seguintes domínios:
* **Models:** Entidades anêmicas que representam as tabelas do banco de dados em memória (`Cliente`, `Pet`, `Produto`).
* **ViewModels:** Classes responsáveis por intermediar as ações do usuário (Commands) e os dados da aplicação. Implementam a interface `INotifyPropertyChanged` para atualização de tela em tempo real.
* **Views:** Telas construídas exclusivamente com marcação XAML, sem lógica de negócios no code-behind (`MainWindow.xaml.cs` vazio).
* **Repositories:** Camada de persistência de dados. Isola todo o código SQL e acesso ao banco de dados do restante da aplicação, utilizando o padrão Repository.

## 📦 Dependências Adicionadas (NuGet)

Para a comunicação de dados, adotamos uma abordagem de **Micro-ORM** focada em velocidade e baixo consumo de memória:

1. **`Microsoft.Data.Sqlite`**: Provedor oficial ADO.NET para o SQLite.
2. **`Dapper`**: Micro-ORM responsável por mapear os resultados das consultas SQL diretamente para nossos objetos C# (Models), eliminando a necessidade de ler o banco de dados linha por linha manualmente (`DataReader`).

## ⚙️ Gerenciamento do Banco de Dados Local

O arquivo do banco de dados (`PetShopCare.db`) gerado na Etapa 1 foi acoplado ao projeto na pasta `Database`. Ele está configurado via MSBuild (`CopyToOutputDirectory="PreserveNewest"`) para ser transferido automaticamente para a pasta de compilação `bin/Debug` durante o ciclo de desenvolvimento, mantendo a integridade das tabelas criadas previamente.

---
*Status do Projeto: Etapa 2 Concluída. Pronto para prosseguir para a Etapa 3 (Camada de Dados e Modelos).*