# PetShopCare ERP

Sistema desktop de gestão integrada para Pet Shops, focado no controle de clientes, animais, inventário, vendas e agendamento de serviços operacionais.

## 🚀 Tecnologias e Arquitetura

* **Plataforma:** .NET 8.0 (Desktop)
* **Interface:** WPF (Windows Presentation Foundation)
* **Biblioteca de UI:** Material Design In XAML Toolkit
* **Linguagem:** C#
* **Padrão Arquitetural:** MVVM (Model-View-ViewModel)
* **Banco de Dados:** SQLite (Embutido / Local)
* **Micro-ORM:** Dapper
* **Acesso a Dados:** ADO.NET (`Microsoft.Data.Sqlite`)

---

## 📌 Histórico de Desenvolvimento (Sprints)

### ✅ Etapa 1: Engenharia de Dados e Modelagem
A base de dados foi projetada com foco em integridade referencial e normalização (1:N entre Clientes e Pets).
* Tabelas projetadas: `Usuarios`, `Clientes`, `Pets`, `Produtos`, `EstoqueMovimentacao`, `Vendas`, `ItensVenda`, `Servicos` e `OrdensServico`.
* Motor SQLite configurado com `INTEGER PRIMARY KEY AUTOINCREMENT` para rastreabilidade contínua.

### ✅ Etapa 2: Setup de Infraestrutura
* Criação da solução (`.sln`) e estruturação de diretórios no padrão MVVM (`Models`, `Views`, `ViewModels`, `Repositories`, `Services`, `Database`).
* Isolamento do arquivo `PetShopCare.db` com diretiva de build para preservação de dados durante o modo de depuração.
* Instalação das dependências NuGet (`Dapper`, `Microsoft.Data.Sqlite` e `MaterialDesignThemes`).

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
* **Navegação Dinâmica:** Estruturação do `MainView` atuando como contêiner principal (`ContentControl`), injetando *UserControls* dinamicamente via comandos, sem acoplamento de múltiplas janelas.
* **Estabilização de Compilação:** Supressão de avisos de nulidade global (C# 8.0+) e resolução de conflitos de tipos, garantindo um ambiente de compilação limpo (zero erros ou *warnings*).
* **Conexão UI-Banco:** Integração do fluxo de leitura do SQLite (via Dapper) com a `DataGrid` da interface.

### ✅ Etapa 7: CRUD de Tutores e Arquitetura Desacoplada
Implementação completa do fluxo de gerenciamento de clientes/tutores aplicando boas práticas de UX e o Princípio da Responsabilidade Única (SRP).
* **Desacoplamento de Visões:** Divisão estrita do fluxo em duas janelas independentes (`CadastroTutorView` e `EdicaoTutorView`).
* **Validação em Tempo Real:** Sincronização do estado de comandos via `CanExecute`, bloqueando ações até que os campos prioritários estejam preenchidos.
* **Máscaras de Entrada:** Formatação dinâmica (Code-Behind visual) para os padrões de CPF e Telefone.
* **Engenharia de Resiliência:** Inicialização forçada do esquema de tabelas (`CREATE TABLE IF NOT EXISTS`) e tratamento de exclusão em cascata no `ClienteService`.

### ✅ Etapa 8: Módulo de Gestão de Pets e Integridade Relacional
Conclusão do CRUD de prontuários de animais, materializando o relacionamento 1:N com a base de Tutores.
* **Mapeamento de Chaves Estrangeiras:** `ComboBox` reativa ligada à lista de tutores no banco, forçando a integridade referencial.
* **Clonagem de Estado (*Safe Editing*):** Aplicação de padrão de clonagem no `EdicaoPetViewModel` para proteger a `DataGrid` contra corrupção visual.
* **Tratamento Seguro de *Parsing*:** Abstração de campos numéricos decimais resolvendo conflitos nativos do WPF com separadores culturais.

### ✅ Etapa 9: Módulo de Gestão de Catálogo e Estoque
Finalização do gerenciamento de produtos com foco estrito em rastreabilidade financeira.
* **Livro Razão de Estoque:** Arquitetura de inventário blindada. O saldo é alterado exclusivamente através de lançamentos na tabela `EstoqueMovimentacao`.
* **Sistema de Alertas Inteligentes:** Motor de varredura que cruza o estoque atual com o estoque mínimo, disparando alertas automáticos.

### ✅ Etapa 10: Ponto de Venda (PDV) e Transações Atômicas
Construção do núcleo de frente de caixa comercial.
* **Interface de Alta Performance:** Busca de produtos em tempo real filtrada na memória RAM via `LINQ`.
* **Transações Atômicas (ACID):** Orquestração da gravação da Venda, Itens e Baixa de Estoque num único *Commit*, com *Rollback* de segurança.
* **Globalização Sistêmica (`pt-BR`):** Injeção em nível de núcleo da cultura brasileira para formatação monetária (R$) unificada.

### ✅ Etapa 11: Dashboard Analítico e Inteligência de Negócio
Transformação da tela inicial num centro de telemetria.
* **Integração Gráfica:** Motor de renderização `SkiaSharp` (LiveCharts2) para desenhar gráficos cartesianos de curva de faturamento.
* **Telemetria e Injeção de Repositórios:** Centralização do processamento de métricas na `DashboardViewModel`.
* **Prevenção de Ruptura de Estoque:** Tabela inteligente embutida no Dashboard monitorizando itens abaixo do ponto de pedido.

### ✅ Etapa 12: Módulo de Serviços e Abstração de Janelas (Pop-ups)
* **Arquitetura Desacoplada:** Separação estrutural entre listagem e formulário de dados.
* **Injeção de Ações:** Utilização de `Action` delegada (`FecharJanela`) comunicando o Code-Behind à ViewModel.
* **Tratamento numérico limpo:** Otimização do componente `MaterialDesignFloatingHintTextBox` para lidar nativamente com valores não-nulos de *Value Types*.

### ✅ Etapa 13: Módulo de Agendamentos e Orquestração Relacional
* **Layout Híbrido:** Interface particionada (Formulário e Agenda Diária).
* **Seleção em Cascata:** Filtro inteligente onde a lista de Pets reage dinamicamente ao Tutor selecionado.
* **Orquestração Tríade:** Salvamento persistindo as três pontas relacionais (Tutor + Pet + Serviço).

### ✅ Etapa 14: Segurança, RBAC e Saída Documental
Consolidação do sistema para uso no mercado real.
* **Autenticação:** Construção da tela e barreira de Login integrada ao SQLite.
* **Controle de Acesso Baseado em Cargos (RBAC):** Restrições sistêmicas dinâmicas via `MainViewModel`. Administradores, Atendentes e Operadores possuem UI e permissões de menu adaptadas aos seus perfis de segurança.
* **Gestão de Equipe:** Módulo CRUD para controle de funcionários e redefinição segura de senhas (*hash*).

### ✅ Etapa 15: Infraestrutura Produtiva, Resiliência e Deploy
Preparação final para entrega comercial e publicação em portfólio.
* **Tratamento Global de Exceções:** Configuração do `DispatcherUnhandledException` na camada central da aplicação (`App.xaml.cs`) para interceptar erros críticos não tratados, mitigar *crashes* silenciosos e gerar logs rastreáveis em disco (`error_log.txt`).
* **Empacotamento (Single File & Self-Contained):** Geração de build otimizada em arquivo executável único contendo o runtime do .NET 8 embutido, garantindo portabilidade total para testes imediatos sem dependências externas.