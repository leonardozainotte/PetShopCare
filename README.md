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
* **Navegação Dinâmica:** Estruturação do `MainView` atuando como contêiner principal (`ContentControl`), injetando *UserControls* (como `TutorView`) dinamicamente via comandos, sem acoplamento de múltiplas janelas.
* **Estabilização de Compilação:** Supressão de avisos de nulidade global (C# 8.0+) e resolução de conflitos de tipos, garantindo um ambiente de compilação limpo (zero erros ou *warnings*).
* **Conexão UI-Banco:** Integração do fluxo de leitura do SQLite (via Dapper) com a `DataGrid` da interface.

### ✅ Etapa 7: CRUD de Tutores e Arquitetura Desacoplada

Implementação completa do fluxo de gerenciamento de clientes/tutores aplicando boas práticas de UX e o Princípio da Responsabilidade Única (SRP).

* **Desacoplamento de Visões (SRP):** Divisão estrita do fluxo em duas janelas independentes: `CadastroTutorView` (exclusiva para inserções lógicas limpas) e `EdicaoTutorView` (focada em atualizações e exclusões), permitindo evolução independente de layouts.
* **Validação em Tempo Real:** Sincronização do estado de comandos via `CanExecute` (`PodeSalvar` e `PodeAlterar`), bloqueando os botões de ação na interface até que os campos prioritários obrigatórios (*Nome*, *CPF* e *Telefone*) estejam preenchidos.
* **Máscaras de Entrada:** Implementação de rotinas de formatação dinâmica diretamente no comportamento do input (Code-Behind visual) para os padrões de CPF (`000.000.000-00`) e Telefone (`(00) 00000-0000`).
* **Engenharia de Resiliência:** - Automação de Infraestrutura: Inicialização forçada do esquema de tabelas (`CREATE TABLE IF NOT EXISTS`) no ciclo de vida global de inicialização da aplicação (`App.xaml.cs`).
  - Tratamento de Exclusão em Cascata: Implementação de proteção defensiva (*try-catch shield*) no `ClienteService` para interceptar e mitigar exceções estruturais em cenários onde tabelas relacionais dependentes (ex: `Pets`) estão vazias ou ausentes, mantendo a operação principal funcional e sem travamentos.

### ✅ Etapa 8: Módulo de Gestão de Pets e Integridade Relacional

Conclusão do CRUD de prontuários de animais, materializando o relacionamento 1:N com a base de Tutores e aprofundando os conceitos de MVVM.

* **Mapeamento de Chaves Estrangeiras:** Implementação de `ComboBox` reativa ligada à lista de tutores no banco, forçando a integridade referencial logo na camada visual.
* **Clonagem de Estado (*Safe Editing*):** Aplicação de padrão de clonagem de instâncias no `EdicaoPetViewModel` para isolamento de dados. Protege a `DataGrid` contra corrupção visual causada pelo *Two-Way Binding* caso a edição seja cancelada pelo utilizador.
* **UX e Controles Dinâmicos:** Criação de seletores em cascata inteligentes (a seleção do atributo *Espécie* reconstrói instantaneamente a lista de *Raças* disponíveis) com suporte a *input* livre (`IsEditable="True"`).
* **Tratamento Seguro de *Parsing*:** Abstração de campos numéricos decimais (`Peso`) através de propriedades intermediárias em texto, resolvendo conflitos nativos do WPF com separadores culturais (ponto e vírgula).
* **Estética de Dados (Material Design):** Refinamento completo das `DataGrids` com alinhamento tipográfico lógico (textos à esquerda, números centralizados) e prevenção de quebras de layout usando supressão por reticências (`CharacterEllipsis`) para strings longas, como endereços e observações médicas.

### ✅ Etapa 9: Módulo de Gestão de Catálogo e Estoque

Finalização do gerenciamento de produtos com foco estrito em rastreabilidade financeira e prevenção de perdas.

* **Livro Razão de Estoque:** Arquitetura de inventário blindada contra `UPDATES` manuais. O saldo de produtos é alterado exclusivamente através de lançamentos de movimentação na tabela `EstoqueMovimentacao`, garantindo histórico auditável de entradas e saídas.
* **Sistema de Alertas Inteligentes:** Implementação de motor de varredura que cruza o "Estoque Atual" com a flag de "Estoque Mínimo", disparando alertas visuais automáticos na interface quando itens de alto giro atingem o ponto de pedido.
* **Resolução de *Type Affinity* (Dapper vs. SQLite):** Correção estratégica de quebras de tipagem (`InvalidCastException`) forçando conversões de `Int64` para ponto flutuante em tempo de execução via instrução SQL `CAST(PrecoCusto AS REAL)`.
* **Segurança Visual e Otimizações de UX:** Evolução da interface global com centralização nativa de janelas de diálogo (`CenterScreen`), exibição via `INNER JOIN` do nome do Tutor na listagem de Prontuários e replicação do padrão de Clonagem de Estado na edição de precificação.

### ✅ Etapa 10: Ponto de Venda (PDV) e Transações Atômicas

Construção do núcleo de frente de caixa comercial, com gestão de estado interativa e persistência relacional unificada.

* **Interface de Alta Performance (UX):** Construção de layout particionado (Catálogo vs. Carrinho). Implementação de busca de produtos em tempo real (Key-up Event) filtrada na memória RAM via `LINQ`, sem onerar requisições ao SQLite.
* **Gestão de Estado do Carrinho:** Lógica de manipulação de itens em memória (Adição, Redução Unitária e Exclusão) com recálculo automático de subtotais e *refresh* visual sem quebra de *Bindings*.
* **Segurança de Fluxo (`CanExecute`):** Acoplamento do botão de finalização às regras de validação do WPF, mantendo-o nativamente bloqueado caso o estado do carrinho seja vazio.
* **Transações Atômicas (ACID):** Salvamento robusto utilizando `IDbTransaction` do Dapper. Orquestração da gravação da Venda, vínculo dos Itens, subtração do Estoque Físico e lançamento no Livro Razão num único *Commit*, com *Rollback* de segurança ativado pelo bloco `catch`.
* **Globalização Sistêmica (`pt-BR`):** Injeção em nível de núcleo (`App.xaml.cs`) da cultura brasileira para formatação monetária (R$) e temporal, unificando as máscaras visuais do XAML de forma definitiva.

### ✅ Etapa 11: Dashboard Analítico e Inteligência de Negócio

Transformação da tela inicial num centro de telemetria para monitoramento tático das operações diárias.

* **Integração Gráfica (LiveCharts2):** Instalação e orquestração do motor de renderização `SkiaSharp` acoplado ao WPF para desenhar gráficos cartesianos de curva de faturamento. Ajustes finos de compatibilidade no `.csproj` e silenciamento de avisos (`<NoWarn>`) para garantir um ambiente de compilação imaculado.
* **Telemetria e Injeção de Repositórios:** Centralização do processamento de métricas na `DashboardViewModel`. Consultas estruturadas via repasse de chamadas ao `VendaRepository`, `ProdutoService` e `OrdemServicoService` para extração de indicadores em tempo real.
* **Arquitetura de Layout Fluido:** Refatoração do XAML isolando as sub-regiões em linhas definidas nativamente pela propriedade `Grid.RowDefinitions`, eliminando quebras visuais e sobreposições de renderização entre os Cards de Resumo, Gráfico e DataGrid de Alerta.
* **Prevenção de Ruptura de Estoque:** Tabela inteligente embutida no Dashboard que monitora proativamente os produtos com saldo igual ou inferior à margem de segurança configurada.
* **Inicialização Segura:** Ajuste do ciclo de vida (*startup*) no contêiner principal da aplicação (`MainViewModel`) para renderização tipada do painel de métricas usando alocação real de objetos.

### 🚧 Etapa 12: Módulo de Serviços (Próximo Passo)

* Implementação completa do CRUD administrativo de serviços prestados pelo petshop (Banho, Tosa, Consulta Veterinária, etc.).
* Parametrização dinâmica de valores base, descrições e controle de disponibilidade.
* Substituição da carga temporária de dados (Mock) pelo fluxo real integrado ao banco de dados SQLite.