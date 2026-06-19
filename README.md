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
* Uso de propriedades auto-implementadas e tipagem segura para nulidade (`DateTime?`, `decimal?`) em C# para prevenir quebras de `NullReferenceException`.
* Configuração da classe `DatabaseConfig` aplicando o padrão estático para fornecimento da *Connection String*.

### ✅ Etapa 4: Camada de Persistência (Repositories)
Isolamento do código SQL em classes especializadas, garantindo que o restante do sistema desconheça a infraestrutura do banco.
* **Segurança:** Uso de parâmetros nomeados no Dapper (`@Parametro`) eliminando riscos de *SQL Injection*.
* **Atomicidade:** Uso de transações (`IDbTransaction`) no `VendaRepository` para processamento atômico em cascata (inserção da venda, itens, baixa de estoque e auditoria).
* **Mapeamento:** Criação dos repositórios para todas as entidades principais, utilizando aliasing SQL para compatibilização de propriedades (ex: `Usuario AS Login`).

### ✅ Etapa 5: Camada de Negócios (Services)
Implementação da barreira sanitária do domínio. Nenhuma informação atinge os repositórios sem validação prévia.
* **Segurança e Autenticação:** Implementação do `CriptografiaService` (SHA-256) para *hashing* de senhas. Prevenção de duplicidade de credenciais via `UsuarioService`.
* **Validações Estruturais:** Bloqueio de CPFs duplicados no `ClienteService` e obrigatoriedade de vínculo relacional no `PetService`.
* **Blindagem Financeira e de Estoque:** `ProdutoService` impede a inserção de margens de lucro negativas. O `VendaService` inspeciona o saldo físico antes de autorizar a abertura de transações no banco, prevenindo inventário negativo.

### ✅ Etapa 6: Interface e ViewModels (Padrão MVVM)
Transição para a camada de apresentação rejeitando o uso de *Code-Behind* para lógica de negócios, adotando estritamente o *Data Binding*.
* **Infraestrutura UI:** Implementação do `ViewModelBase` (`INotifyPropertyChanged`) para reatividade assíncrona da tela e `RelayCommand` (`ICommand`) para o roteamento isolado de eventos de clique.
* **Navegação Dinâmica:** Estruturação do `MainView` atuando como contêiner principal (`ContentControl`), injetando *UserControls* (como `TutorView`) dinamicamente via comandos, sem acoplamento de múltiplas janelas.
* **Estabilização de Compilação:** Supressão estratégica de avisos de nulidade global (C# 8.0+) e resolução de conflitos de tipos, garantindo um ambiente de compilação limpo (zero erros ou *warnings*).
* **Conexão UI-Banco:** Integração do fluxo de leitura do SQLite (via Dapper) com a `DataGrid` da interface.

### 🚧 Etapa 7: Operações CRUD na Interface (Em Andamento)
* Modelagem dos formulários de entrada de dados para inserção e atualização.
* Implementação do fluxo completo de gravação (View -> ViewModel -> Service -> Repository -> SQLite).
* Sincronização de estado para atualização em tempo real das tabelas visuais após transações no banco.