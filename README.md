# 🚀 ReStart.AI – Backend em C# (.NET Web API)

### 1. 🧠 O que o backend em C# faz?

- 📲 Recebe os dados do app mobile (React Native / Expo).
- 💾 Salva e consulta informações em banco (usuários, currículos, vagas, etc.).
- 🤖 Chama o serviço de IA/IoT (FastAPI em Python) para:
  - 📝 Resumir o currículo do usuário.
  - 🎯 Sugerir um papel-alvo.
  - 📊 Calcular o percentual de match (%).
  - 💬 Gerar o texto “por que você?”.

Fluxo geral:

👤 Usuário → 📱 App Mobile → 🧩 API C# (.NET) → 🌐 Serviço IoT (Python) → 🧠 OpenAI  
                                                ↑  
                                          🔁 Resposta pronta para o app

------------------------------------------
### 2. 🛠 Tecnologias usadas (C#)

- 💻 Linguagem: **C#**
- 🌐 Framework: **ASP.NET Core Web API**
- 🧱 Arquitetura em camadas:
  - `ReStartAI.Api` – controllers e configuração da API
  - `ReStartAI.Application` – regras de negócio, parsing e matching
  - `ReStartAI.Domain` – entidades e interfaces
  - `ReStartAI.Infrastructure` – MongoDB, repositórios, logs
  - `ReStartAI.Tests` – testes automatizados
- 🗄 Banco principal: **MongoDB**
- 📜 Logs: **Entity Framework Core** em banco relacional
- 📚 Documentação: **Swagger / OpenAPI**
- 🔐 Segurança:
  - Hash de senha (não salva senha em texto puro)
  - API Key / chave interna para acessar o serviço de IA/IoT

------------------------------------------
### 3. 📌 Principais responsabilidades da API C#

- 👥 **Usuários**
  - Cadastro e login.
  - Armazena a senha como **hash**, não em texto puro. 

- 📄 **Currículos**
  - Recebe currículo do usuário.
  - Envia para o parser/serviço de IA.
  - Guarda informações estruturadas do currículo.

- 💼 **Vagas e Matching**
  - Mantém as vagas cadastradas.
  - Usa um motor de matching para comparar perfil x vaga.
  - Retorna percentuais de aderência (match). 

- 🌉 **Integração com o IoT (FastAPI)**
  - Usa `HttpClient` para chamar o serviço Python.
  - Envia `X-Internal-Key` com uma chave secreta compartilhada. 
  - Recebe um JSON com insight e resumo e devolve para o app mobile.

------------------------------------------
### 4. 🧪 Como rodar o backend localmente

Pré-requisitos:
- ✅ .NET SDK instalado
- ✅ MongoDB rodando (local ou na nuvem)
- ✅ Serviço IoT configurado e acessível (ou URL de mock)

Passos básicos:

1️⃣ Restaurar dependências:
   - `dotnet restore`

2️⃣ Configurar `appsettings.json` ou variáveis de ambiente:
   - String do MongoDB:
     - `ConnectionStrings:MongoDb = "<SUA_CONNECTION_STRING_DO_MONGODB>"`
   - URL do IoT:
     - `Iot:BaseUrl = "https://restartai-iot-web.azurewebsites.net/docs"`
   - Chave interna (para header `X-Internal-Key`):
     - `Iot:InternalKey = "minha-internal-key"`

3️⃣ Rodar a API:
   - `cd ReStartAI.Api`
   - `dotnet run`

4️⃣ Testar no navegador:
   - Swagger: `http://localhost:5000/swagger` (ou a porta configurada) 🌐

5️⃣ Testes Localmente:
   - cd ReStartAI.Tests
   - dotnet test

------------------------------------------
### 5. 🔗 Deploy e links importantes

- 🌍 **Link do deploy da API (.NET Web API):**  
  👉 https://restartai-api-001.azurewebsites.net/swagger/index.html

  - 🔑 **Chave interna do Swagger:**  
  👉 `API_KEY = "dev-swagger-key-123"`

- 🎥 **Link do vídeo:**  
  👉 https://youtu.be/t01p_cDDX38
  
------------------------------------------
### 6. 👩‍💻 Equipe:

- ⭐️ Valéria Conceição Dos Santos — RM: 557177
- ⭐️ Mirela Pinheiro Silva Rodrigues — RM: 558191

-------------------------------------------



