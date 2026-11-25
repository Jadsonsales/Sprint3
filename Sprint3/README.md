# Desafio Sprint 3 - Desenvolvimento de APIs .NET

## Sobre o Projeto
API REST funcional desenvolvida em **.NET 10** para gerenciamento de produtos com autenticação e autorização baseada em **JWT (JSON Web Token)**, persistência em banco de dados relacional **MySQL** e documentação interativa via **Swagger**.

## Tecnologias Utilizadas
- **ASP.NET Core Web API (.NET 10)**
- **Entity Framework Core** (com provedor **Pomelo.EntityFrameworkCore.MySql**)
- **MySQL** (Banco de dados relacional)
- **Autenticação JWT** (`Microsoft.AspNetCore.Authentication.JwtBearer`)
- **Swagger / OpenAPI** (`Swashbuckle.AspNetCore`)

## Arquitetura do Projeto
O projeto segue o padrão de arquitetura em camadas para garantir a separação de responsabilidades, manutenibilidade e segurança:
- **`Models`**: Entidades do domínio (`Usuario` e `Produto`) mapeadas para o banco de dados.
- **`DTOs`**: Objetos de transferência de dados (`UsuarioRegistroDto`, `UsuarioLoginDto`, `ProdutoRegistroDto`, `ProdutoResponseDto`) com validações de entrada via *Data Annotations*.
- **`Data`**: Contexto do Entity Framework (`AppDbContext`) e configurações de acesso ao MySQL.
- **`Repositories`**: Interfaces e implementações responsáveis pelas operações de persistência no banco de dados.
- **`Services`**: Interfaces e implementações contendo as regras de negócio, criptografia de senhas e geração de tokens JWT.
- **`Controllers`**: Endpoints REST (`AuthController` e `ProdutosController`) com proteção de rotas via `[Authorize]`.

## Endpoints Principais
### Autenticação (`/api/Auth`)
- `POST /api/Auth/registrar` — Cadastro de novo usuário.
- `POST /api/Auth/login` — Autenticação e geração do token JWT.

### Produtos (`/api/Produtos`) — *Requer Bearer Token*
- `GET /api/Produtos` — Lista todos os produtos cadastrados.
- `GET /api/Produtos/{id}` — Busca um produto específico por ID.
- `POST /api/Produtos` — Cadastra um novo produto.
- `PUT /api/Produtos/{id}` — Atualiza os dados de um produto existente.
- `DELETE /api/Produtos/{id}` — Remove um produto pelo ID.

## Como Rodar o Projeto
1. Clone o repositório:
   ```bash
   git clone <URL_DO_SEU_REPOSITORIO>
   cd Sprint3/Sprint3