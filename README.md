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

### ✅ Etapa 4: Camada de Persistência (Repositories)
Desenvolvimento das rotinas de acesso a dados (CRUD) isolando o código SQL da regra de negócios da aplicação.
* **Segurança:** Utilização de parâmetros nomeados (`@Parametro`) com Dapper para blindagem total contra *SQL Injection*.
* **Integridade Transacional:** Implementação do padrão ACID na gerência de fluxos operacionais através de transações gerenciadas (`IDbTransaction`). O processamento de vendas realiza a inserção em cascata dos itens, atualização de saldos no inventário e log de auditoria de forma atômica.
* **Repositórios Implementados:**
  * `ClienteRepository`: Gestão cadastral de tutores.
  * `PetRepository`: Prontuários animais e mapeamento relacional 1:N com `BuscarPorClienteId`.
  * `UsuarioRepository`: Controle de credenciais e subsídio à autenticação operacional.
  * `ProdutoRepository`: Gestão de inventário e emissão de gatilhos para alertas de estoque mínimo.
  * `ServicoRepository`: Catálogo e precificação de procedimentos operacionais.
  * `EstoqueMovimentacaoRepository`: Ledger histórico para auditorias de entradas e saídas físicas.
  * `VendaRepository`: Processamento unificado de vendas rápidas ou nominais com controle transacional rigoroso.
  * `OrdemServicoRepository`: Centralização de agendamentos, ordens operacionais e controle de fluxo da agenda.

---
*Status Atual: Desenvolvimento contínuo da camada de repositórios.*