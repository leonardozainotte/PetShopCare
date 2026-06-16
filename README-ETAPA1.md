# PetShop ERP - Etapa 1: Engenharia de Dados e Modelagem Relacional

Este diretório contém a documentação técnica e os conceitos da camada de dados do sistema **PetShopCare**, desenvolvido em **C# (.NET 8)** com **WPF**.

## 📌 Visão Geral da Modelagem

O modelo relacional foi estruturado focando na integridade referencial, eliminação de redundâncias de dados (atendendo às Formas Normais) e capacidade de rastreabilidade financeira e de inventário.

A principal decisão arquitetural aplicada nesta etapa foi a **desacoplagem completa entre Clientes e Pets**. No modelo de negócios de um Pet Shop, a relação é de **1 para N** (Um cliente/tutor pode possuir múltiplos animais de estimação), mapeada via chave estrangeira `ClienteId` na tabela `Pets`.

## 🗄️ Estrutura do Dicionário de Dados

O banco de dados é composto pelas seguintes entidades nucleares:

1. **Usuarios**: Controle de credenciais de operadores do sistema, segregados por cargos e permissões (`Administrador`, `Atendente`, `BanhoTosa`).
2. **Clientes**: Base cadastral de tutores contendo informações de contato e validação fiscal (`CPF`).
3. **Pets**: Prontuário básico do animal, incluindo peso, espécie, raça e mapeamento para o tutor responsável.
4. **Produtos**: Cadastro de itens comerciais categorizados, com parametrização de margens e gatilho de **Estoque Mínimo**.
5. **EstoqueMovimentacao**: Ledger (livro-razão) de auditoria que registra entradas e saídas de produtos, garantindo histórico do inventário.
6. **Vendas & ItensVenda**: Acoplamento de vendas em cascata para emissão de comprovantes e controle de faturamento diário.
7. **Servicos**: Catálogo de procedimentos disponíveis no PetShop (Preço, Tempo médio).
8. **OrdensServico**: Controle operacional de fluxo de serviços e agenda (Status: `Agendado`, `Em Andamento`, `Finalizado`, `Cancelado`).

## 🛠️ Infraestrutura de Banco de Dados

O projeto utiliza o **SQLite** como motor de banco de dados embutido. 

* **Isolamento e Portabilidade:** O arquivo `PetShopCare.db` reside fisicamente na estrutura do projeto e é gerenciado de forma local.
* **Integridade:** O esquema utiliza a declaração rigorosa de `INTEGER PRIMARY KEY AUTOINCREMENT` para assegurar que IDs deletados nunca sejam reutilizados pelo motor do banco, garantindo consistência fiscal.

---
*Status do Projeto: Etapa 1 Concluída. Modelagem Relacional definida e banco de dados SQLite inicializado.*