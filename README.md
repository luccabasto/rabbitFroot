# 🌟 CP5.Microservices 🌟

> “Quem ouve, esquece. Quem vê, lembra. Quem faz, aprende.”  
> — Provérbio Chinês

---

## 📜 Desenvolvedores

Está aplicação foi desenvolvida pelos devs:

* Lucas E.S. Basto - RM553771
* Kevin Nobre - RM552590

---

## 📝 Descrição

Este projeto é o **Checkpoint 5** da disciplina de Programação de API com Microservices (FIAP).  
Ele simula um fluxo de mensagens entre cinco aplicações **.NET Console** conectadas via **RabbitMQ**:

1. **SenderFrutas**: envia informações de frutas da época.  
2. **SenderUsuarios**: envia dados de usuários fictícios.  
3. **Validador**: recebe, valida e reencaminha mensagens.  
4. **ReceiverFrutas**: consome e exibe frutas validadas.  
5. **ReceiverUsuarios**: consome e exibe usuários validados.

---

## 📁 Estrutura do Projeto

```
CP5.Microservices/
│   CP5.Microservices.sln
│   docker-compose.yml
│
├── SenderFrutas/
│   └── Program.cs
│   └── SenderFrutas.csproj
│
├── SenderUsuarios/
│   └── Program.cs
│   └── SenderUsuarios.csproj
│
├── Validador/
│   └── Program.cs
│   └── Validador.csproj
│
├── ReceiverFrutas/
│   └── Program.cs
│   └── ReceiverFrutas.csproj
│
└── ReceiverUsuarios/
    └── Program.cs
    └── ReceiverUsuarios.csproj
```

---

## 🚀 Pré-requisitos

- [.NET 8.0 SDK (LTS)](https://dotnet.microsoft.com/download)  
- [Visual Studio 2022](https://visualstudio.microsoft.com) (opcional)  
- [Docker Desktop](https://www.docker.com/products/docker-desktop)

---

## ⚙️ Configuração e Execução

1. **Clone** este repositório:
   ```bash
   git clone https://github.com/seu-usuario/CP5.Microservices.git
   cd CP5.Microservices
   ```

2. **Suba** o RabbitMQ via Docker Compose:
   ```bash
   docker-compose up -d
   ```
   - AMQP em **localhost:5672**  
   - Painel Web em **http://localhost:15672** (usuário/senha: `guest`/`guest`)

3. **Abra** a solução no Visual Studio:
   - `Arquivo → Abrir → Projeto/Solução…`  
   - Selecione `CP5.Microservices.sln`.

4. **Instale** o pacote NuGet **RabbitMQ.Client** em cada projeto:
   - Clique com o botão direito em cada projeto → **Gerenciar Pacotes NuGet…**  
   - Procure por `RabbitMQ.Client` e instale (versão 7.x).

5. **Defina** múltiplos projetos de inicialização:
   - Clique com o botão direito na solução → **Definir Projetos de Inicialização…**  
   - Escolha **Vários projetos** e ordene:
     1. Validador  
     2. ReceiverFrutas  
     3. ReceiverUsuarios  
     4. SenderFrutas  
     5. SenderUsuarios  

6. **Inicie** com **F5** ou **▶️**.  
   Cinco consoles abrirão em sequência!

---

## 🔄 Fluxo de Mensagens

1. **Sender → Validador**  
   - SenderFrutas publica em `exchange=fiap.exchange`, `routing-key=frutas.epoca`.  
   - SenderUsuarios publica em `exchange=fiap.exchange`, `routing-key=usuarios.dados`.

2. **Validador**  
   - Consome de `frutas.validation` e `usuarios.validation`.  
   - Valida formato (3 campos para frutas, 5 para usuários).  
   - Re-publica em `frutas.validated` ou `usuarios.validated`.

3. **Validador → Receivers**  
   - ReceiverFrutas escuta `frutas.receiver` (bind `frutas.validated`).  
   - ReceiverUsuarios escuta `usuarios.receiver` (bind `usuarios.validated`).  
   - Exibe no console as mensagens aprovadas.

---

## ✅ Testes de Conexão

- **TCP**:  
  ```powershell
  Test-NetConnection -ComputerName 127.0.0.1 -Port 5672
  ```
- **HTTP**:  
  ```bash
  curl -u guest:guest http://127.0.0.1:15672/api/healthchecks/node
  ```

Se ambos retornarem sucesso, o RabbitMQ está no ar! 🎉

---
