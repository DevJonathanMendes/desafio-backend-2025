# Bankount API

API desenvolvida com .NET 8 e PostgreSQL para gerenciamento bancário de empresas por CNPJ.\
*Projeto utilizando Docker para facilitar o desenvolvimento e deploy.*

Infelizmente, tive alguns contratempos, mas compreendo que foi oferecido muito tempo para o dev.

**Não há absolutamente nada de "mais" neste projeto**, embora eu tenha experiência com .NET C#\
é a minha primeira API e é baseada na documentação oficial.

## Projeto Hospedado em VPS com Docker Compose

**Por tempo LIMITADO**!

Para testar a API, basta acessar: <http://212.85.2.149:5120/swagger/>

## Collection Postman

Se preferir testar manualmente a API com um cliente HTTP:

- [Postman Collection](./postman/Bankount.postman_collection.json)

## 🚀 Tecnologias

- **Backend**:
  - .NET 8: Framework principal para construção da API.
  - Entity Framework: Para gerenciamento de banco de dados e migrações.
  - PostgreSQL: Banco de dados relacional utilizado para persistência.
- **Docker**: Containers para backend e banco de dados PostgreSQL.

## 📦 Pré-requisitos

- .NET 8.
- [Docker](https://www.docker.com/) e [Docker Compose](https://docs.docker.com/compose/) (para rodar em produção)
- Banco de dados **PostgreSQL** (utilizado no Docker Compose ou localmente)

## 🖥️ Como Rodar Localmente

1. PostgreSQL rodando via docker, local ou servidor. Com as configurações em mãos.

2. Acesse o arquivo [appsettings.json](./appsettings.json) e configure `DefaultConnection` com as suas credencias (ou do docker, tirando service do bankount):\
`Host=localhost;Port=5621;Database=postgres;Username=Bankount;Password=Bankount`

3. Execute os seguintes passos:

```bash
# Restaurar dependências
dotnet restore

# Executar a aplicação
dotnet run
```

Acesse no navegador o endereço fornecido no terminal (geralmente `http://localhost:5120`).

## 🐳 Como Rodar com Docker

1. PostgreSQL rodando via docker, local ou servidor. Com as configurações em mãos.

2. Acesse o arquivo [appsettings.json](./appsettings.json) e configure `DefaultConnection` com:\
`Host=bankount_postgres;Port=5432;Database=postgres;Username=Bankount;Password=Bankount`

3. Execute os seguintes passos:

```bash
docker compose up --build
```

O Docker Compose irá criar os seguintes containers:

- PostgreSQL (porta 5432)
- Backend API (porta 5120)

Após a construção, a API estará disponível em `http://localhost:5120`.

## ⚙️ Observações

- As migrações são executadas automaticamente em [Program.cs](./Program.cs).
