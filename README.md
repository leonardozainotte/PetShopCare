# PetShopCare ERP

Sistema desktop de gestão integrada para Pet Shops, focado no controle de clientes, animais, inventário, vendas e agendamento de serviços operacionais.

## 🚀 Tecnologias e Arquitetura

* **Plataforma:** .NET 8.0 (Desktop)
* **Interface:** WPF (Windows Presentation Foundation)
* **Linguagem:** C#
* **Padrão Arquitetural:** MVVM (Model-View-ViewModel)
* **Banco de Dados:** SQLite (Embutido / Local)
* **Micro-ORM:** Dapper
* **Acesso a Dados:** ADO.NET (`Microsoft.Data.Sqlite`)

---

## 📌 Histórico de Desenvolvimento (Sprints)

### ✅ Etapa 1: Engenharia de Dados e Modelagem
A base de dados foi projetada com foco em integridade referencial e normalização (1:N entre Clientes e Pets).
* Tabelas criadas: `Usuarios`, `Clientes`, `Pets`, `Produtos`, `EstoqueMovimentacao`, `Vendas`, `ItensVenda`, `Servicos` e `OrdensServico`.
* Motor SQLite configurado com `INTEGER PRIMARY KEY AUTOINCREMENT` para rastreabilidade fiscal contínua.

### ✅ Etapa 2: Setup de Infraestrutura
* Criação da solução (`.sln`) e estruturação de diretórios no padrão MVVM (`Models`, `Views`, `ViewModels`, `Repositories`, `Services`, `Database`).
* Isolamento do arquivo `PetShopCare.db` com diretiva de build para preservação de dados durante o modo de depuração.
* Instalação das dependências NuGet (`Dapper` e provedor SQLite).

### ✅ Etapa 3: Mapeamento Objeto-Relacional (Models)
* Implementação do modelo de domínio anêmico refletindo as tabelas do banco.
* Uso de propriedades auto-implementadas e tipagem segura para nulidade (`DateTime?`, `decimal?`) em C# para prevenir quebras de `NullReferenceException` durante o mapeamento com o banco.
* Configuração da classe `DatabaseConfig` aplicando o padrão estático para fornecimento da *Connection String*.

---
*Status Atual: Preparando a camada de persistência (Repositories) e rotinas CRUD.*
