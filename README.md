# 🌟 CP5.Microservices 🌟

> “Quem ouve, esquece. Quem vê, lembra. Quem faz, aprende.”  
> — Provérbio Chinês

---

## 📜 Desenvolvedores

Esta aplicação foi desenvolvida pelos devs:

* Lucas E.S. Basto — RM553771  
* Kevin Nobre — RM552590  
* Sabrina Couto — RM552728

---

## 📝 Descrição

Este projeto é o **Checkpoint 5** da disciplina de Programação de API com Microservices (FIAP).  
Ele simula um fluxo de mensagens entre cinco aplicações **.NET Console** conectadas via **RabbitMQ**:

1. **SendFrutas**: envia informações de frutas da época.  
2. **SendUsuarios**: envia dados de usuários fictícios.  
3. **Validador**: recebe, valida e reencaminha mensagens.  
4. **ReceiveFrutas**: consome e exibe frutas validadas.  
5. **ReceiveUsuarios**: consome e exibe usuários validados.

---

## 📁 Estrutura do Projeto

```
CP5.Microservices/
│   CP5.Microservices.sln
│   docker-compose.yml
│
├── SendFrutas/
│   └── Program.cs
│   └── SendFrutas.csproj
│
├── SendUsuarios/
│   └── Program.cs
│   └── SendUsuarios.csproj
│
├── Validador/
│   └── Program.cs
│   └── Validador.csproj
│
├── ReceiveFrutas/
│   └── Program.cs
│   └── ReceiveFrutas.csproj
│
└── ReceiveUsuarios/
    └── Program.cs
    └── ReceiveUsuarios.csproj
```

---

## 🚀 Pré-requisitos

- [.NET 8.0 SDK (LTS)](https://dotnet.microsoft.com/download)  
- [Visual Studio 2022](https://visualstudio.microsoft.com) (opcional)  
- [Docker Desktop](https://www.docker.com/products/docker-desktop)

---

## ⚙️ Configuração e Execução

1. **Clone**  
   ```bash
   git clone https://github.com/seu-usuario/CP5.Microservices.git
   cd CP5.Microservices
   ```
2. **Suba** o RabbitMQ  
   ```bash
   docker-compose up -d
   ```
   - AMQP em **localhost:5672**  
   - UI em **http://localhost:15672** (`guest`/`guest`)
3. **Abra** no Visual Studio  
   - `Arquivo → Abrir → Projeto/Solução…`  
   - Selecione `CP5.Microservices.sln`.
4. **Instale** RabbitMQ.Client  
   - Em cada projeto: botão direito → Gerenciar Pacotes NuGet → busque `RabbitMQ.Client` e instale (v7.x).
5. **Defina** múltiplos projetos de inicialização  
   - Botão direito na solução → Definir Projetos de Inicialização…  
   - Escolha **Vários projetos** e marque, na ordem:  
     1. Validador  
     2. ReceiveFrutas  
     3. ReceiveUsuarios  
     4. SendFrutas  
     5. SendUsuarios  
6. **Execute**  
   - F5 no Visual Studio ou `dotnet run` em cada pasta, seguindo essa ordem.

---

## 🔄 Fluxo de Mensagens

1. **Send → Validador**  
   - SendFrutas publica em `fiap.exchange` / `frutas.epoca`.  
   - SendUsuarios publica em `fiap.exchange` / `usuarios.dados`.  
2. **Validador**  
   - Consome de `frutas.validate` e `usuarios.validate`.  
   - Valida e reenvia em `frutas.validated` ou `usuarios.validated`.  
3. **Receivers**  
   - ReceiveFrutas escuta `frutas.receiver`.  
   - ReceiveUsuarios escuta `usuarios.receiver`.  

---

## ✅ Testes de Conexão

- **TCP (AMQP)**  
  ```powershell
  Test-NetConnection -ComputerName 127.0.0.1 -Port 5672
  ```  
  Deve retornar `TcpTestSucceeded : True`.

- **HTTP (API Management)**  
  ```bash
  curl -u guest:guest http://127.0.0.1:15672/api/healthchecks/node
  ```  
  Deve retornar:
  ```json
  {"status":"ok","node":"rabbit@<container_id>"}
  ```

---

## 🧪 Testes de Envio e Fluxo

🔸 **Fluxo de Frutas**  
1. Abra o terminal na pasta **Validador** e rode:  
   ```bash
   cd Validador
   dotnet run
   ```  
   Aguarde: `[Validador] Aguardando mensagens. ENTER para sair.`  

2. Em outra aba, vá para **ReceiveFrutas** e execute:  
   ```bash
   cd ReceiveFrutas
   dotnet run
   ```  
   Console: `[ReceiveFrutas] Aguardando mensagens. ENTER para sair.`  

3. Em um terceiro terminal, entre em **SendFrutas** e dispare:  
   ```bash
   cd SendFrutas
   dotnet run
   ```  
   Informe **Nome da fruta** e **Descrição** quando solicitado.

**Resultado esperado:**  
- **Validador**: `Fruta "Banana" válida` (ou inválida)  
- **ReceiveFrutas**: `[ReceiveFrutas] Banana → ✔️ (2025-04-29T14:30:00Z)`

---

🔸 **Fluxo de Usuários**  
1. (Se não estiver rodando) Inicie o **Validador**:  
   ```bash
   cd Validador
   dotnet run
   ```  
2. Em outro terminal, execute **ReceiveUsuarios**:  
   ```bash
   cd ReceiveUsuarios
   dotnet run
   ```  
   Console: `[ReceiveUsuarios] Aguardando mensagens. ENTER para sair.`  

3. Finalmente, no terceiro terminal, entre em **SendUsuarios**:  
   ```bash
   cd SendUsuarios
   dotnet run
   ```  
   Informe **Nome completo**, **Endereço**, **RG** e **CPF**.

**Resultado esperado:**  
- **Validador**: `Usuário "João Silva" válido` (ou inválido)  
- **ReceiveUsuarios**: `[ReceiveUsuarios] João Silva → ✔️ (2025-04-29T14:35:00Z)`

---
