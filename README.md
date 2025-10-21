<a id="readme-top"></a>

# ?? SafeScribe API - ASP.NET Core Web API

![Static Badge](https://img.shields.io/badge/build-passing-brightgreen) ![Static Badge](https://img.shields.io/badge/Version-1.0.0-black) ![License](https://img.shields.io/badge/license-MIT-lightgrey)

## ????? Informa��es do Contribuinte

| Nome | Matr�cula | Turma |
| :------------: | :------------: | :------------: |
| Pedro Henrique Vasco Antonieti | 556253 | 2TDSPH |
<p align="right"><a href="#readme-top">Voltar ao topo</a></p>

---

## ?? Sobre o Projeto

**SafeScribe API** � uma aplica��o desenvolvida em **ASP.NET Core 8 Web API** com foco em **autentica��o e autoriza��o seguras utilizando JWT (JSON Web Tokens)**.  
A plataforma simula um sistema corporativo de gest�o de **notas e documentos sens�veis**, garantindo que apenas usu�rios autenticados e com permiss�es adequadas tenham acesso aos recursos dispon�veis.

---

## ?? Funcionalidades

- ?? **Autentica��o com JWT** � Login seguro com tokens e claims.  
- ?? **Autoriza��o por Roles** � Controle de acesso com permiss�es `Admin`, `Editor` e `Leitor`.  
- ?? **CRUD de Notas** � Cria��o, leitura, atualiza��o e exclus�o de notas vinculadas ao usu�rio.  
- ?? **Blacklist de Tokens** � Tokens s�o invalidados no logout e listados para auditoria.  
- ?? **Swagger Documentation** � Toda a API documentada com exemplos de requisi��o e resposta.  

<p align="right"><a href="#readme-top">Voltar ao topo</a></p>

---

## ??? Tecnologias Utilizadas

![.NET](https://img.shields.io/badge/.NET%209.0-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![C#](https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=c-sharp&logoColor=white)
![Swagger](https://img.shields.io/badge/Swagger-85EA2D?style=for-the-badge&logo=swagger&logoColor=white)
![JWT](https://img.shields.io/badge/JWT-000000?style=for-the-badge&logo=jsonwebtokens&logoColor=white)

<p align="right"><a href="#readme-top">Voltar ao topo</a></p>

---

## ?? Estrutura do Projeto

```
SafeScribeAPI/
?? Controllers/
?  ?? AuthController.cs
?  ?? NotesController.cs
?  ?? BlacklistController.cs
?
?? Data/
?  ?? AppDbContext.cs
?
?? DTOs/
?  ?? LoginRequestDto.cs
?  ?? NoteCreateDto.cs
?  ?? NoteUpdateDto.cs
?  ?? UserRegisterDto.cs
?
?? Models/
?  ?? User.cs
?  ?? Note.cs
?  ?? Role.cs
?
?? Services/
?  ?? ITokenService.cs
?  ?? TokenService.cs
?  ?? ITokenBlacklistService.cs
?  ?? InMemoryTokenBlacklistService.cs
?
?? Middleware/
?  ?? JwtBlacklistMiddleware.cs
?
?? Configuration/
?  ?? OptionalAuthOperationFilter.cs
?
?? Program.cs
?? appsettings.json
```

<p align="right"><a href="#readme-top">Voltar ao topo</a></p>

---

## ?? Pr�-requisitos

Antes de iniciar, voc� precisar� ter instalado em sua m�quina:

- ? [.NET SDK 8.0+](https://dotnet.microsoft.com/download)
- ? [Visual Studio 2022](https://visualstudio.microsoft.com/) ou [VS Code](https://code.visualstudio.com/)
- ? [Git](https://git-scm.com/)

---

## ?? Pacotes Necess�rios

Para rodar a aplica��o corretamente, instale os pacotes abaixo (se ainda n�o estiverem no projeto):

```bash
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer
dotnet add package Swashbuckle.AspNetCore
dotnet add package BCrypt.Net-Next
dotnet add package Microsoft.EntityFrameworkCore.InMemory
```

<p align="right"><a href="#readme-top">Voltar ao topo</a></p>

---

## ?? Como Rodar o Projeto

1. Clone este reposit�rio:
   ```bash
   git clone https://github.com/seu-usuario/SafeScribeAPI.git
   ```

2. Acesse a pasta do projeto:
   ```bash
   cd SafeScribeAPI
   ```

3. Restaure as depend�ncias:
   ```bash
   dotnet restore
   ```

4. Execute o projeto:
   ```bash
   dotnet run
   ```

5. Acesse a documenta��o Swagger:
   ```
   https://localhost:7058/swagger/index.html
   ```

<p align="right"><a href="#readme-top">Voltar ao topo</a></p>

---

## ?? Fluxo de Teste

1. **Registrar usu�rio:** `POST /api/v1/auth/registrar`  
2. **Login:** `POST /api/v1/auth/login`  
3. **Criar nota:** `POST /api/v1/notes` *(Editor ou Admin)*  
4. **Listar notas:** `GET /api/v1/notes/{id}`  
5. **Logout:** `POST /api/v1/auth/logout`  
6. **Ver tokens inv�lidos:** `GET /api/v1/blacklist` *(somente Admin)*


<p align="right"><a href="#readme-top">Voltar ao topo</a></p>
