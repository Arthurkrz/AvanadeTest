# AvanadeTest

Sistema desenvolvido em .NET 8.0 para gerenciamento de operações comerciais envolvendo catálogo de produtos, identidade de usuários e processamento de vendas, estruturado em quatro APIs independentes protegidas por um Proxy Reverso e em comunicação por meio de RabbitMQ e Clients.

## 🌐 Funcionalidades e Características Compartilhadas
Todas as APIs as seguintes características:
- Controllers com rotas organizadas, utilizando tipos HTTP adequados;
- Utilização de Middlewares e Exceptions customizadas para gerenciamento de erros;
- Aderência aos princípios de RESTful APIs;
- Desacoplamento de serviços durante comunicações com HttpClient e RabbitMQ;
- Cobertura completa com testes unitários e de integração, utilizando BDs de testes;
- Avaliações de cobertura com Fine Code Coverage;
- Persistência de dados e gerenciamento de banco de dados com Entity Framework Core;
- Aplicação rigorosa de todos os princípios SOLID;
- Arquitetura em camadas com diferentes projetos (Controller-Service-Repository-Core);
- Utilização de DTOs nas solicitações recebidas pela camada Controller;
- Projeções de entidades a partir de uma ou mais propriedades em comum (consultas);
- Injeção de Dependência.

## 📌 Funcionalidades e Atuações Específicas
### 🚪 GatewayAPI
- Roteamento de requisições para outras APIs com autorização e Roles;
- Autenticação JWT;
- Limitação de número de requisições (RateLimiter);
- Retry de requisições e bloqueio em erros recorrentes (Circuit Breaker);
- Validação básica de requisições (Middleware).

### 🔐 IdentityAPI
- Recebimento de solicitações de registro e login de Administradores e Compradores;
- Criptografia de dados sensíveis (senhas) com Argon2id, Salt e SHA-256;
- Geração e envio de token JWT para GatewayAPI;
- Bloqueio de entrada em contas por 1 dia após 10 tentativas;
- Validações de solicitações de registro e login com FluentValidation;
- Armazenamento de perfis em banco de dados relacional;

### 💸 SalesAPI
- Recebimento de solicitações de compra pela Controller;
- Validação da existência do produto e comprador em outras APIs por IdentityClient;
- Validação da quantidade em estoque na StockAPI por StockClient;
- Criação da entidade Sale com status Pending;
- Envio da solicitação de venda por RabbitMQ;
- Criação da exchange, routing key e canais Main, Retry e DeadLetter;
- Uso de arquivos de configuração de RabbitMQ compartilhados entre Consumer e Producer;
- Recebimento da resposta de status de venda pela StockAPI por RabbitMQ;
- Atualização de vendas sem resposta por 10 segundos para Expired (BackgroundService);
- Armazenamento de solicitações de compra em banco de dados relacional;

### 📦 StockAPI
- Realização de operações CRUD para todos os produtos;
- Criação da exchange, routing key e canais Main, Retry e DeadLetter;
- Recebimento de solicitações de compra por RabbitMQ pela SalesAPI;
- Envio de resposta com status de venda por RabbitMQ para SalesAPI;
- Uso de arquivos de configuração de RabbitMQ compartilhados entre Consumer e Producer;
- Armazenamento de produtos em banco de dados relacional;
- Validação de novos produtos com FluentValidation;
- Envio de catálogo para SalesAPI.

## ⚙️ Utilização
### Pré-requisitos e Dependências
- .NET SDK 8.0;
- SQL Server;
- RabbitMQ (local ou via Docker);

### Etapas de inicialização
- Configuração de BD com EF Core Migrations pelo comando "dotnet ef database update";
- Subida de RabbitMQ ou instalação local com acesso ao Client;
- Execução de todas as 4 APIs na ordem IdentityAPI, GatewayAPI, StockAPI e SalesAPI;
- Realização de registro e login com envio de token JWT em cada funcionalidade.
