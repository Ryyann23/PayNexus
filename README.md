# PayNexus

PayNexus e um aplicativo Windows Forms para simular o processamento de pagamentos em uma dashboard operacional. A tela principal mostra resumo financeiro, transacoes recentes, logs em tempo real e observers ativos.

## Recursos

- Cadastro de novo pagamento com cliente, valor e metodo.
- Metodos simulados: PIX, Cartao de Credito e Boleto.
- Dashboard com total processado, transacoes do dia, aprovados e pendentes.
- Logs operacionais em tempo real.
- Uso dos padroes Strategy, para trocar a regra de pagamento, e Observer, para notificar acoes apos o processamento.
- Animacoes visuais antes da conclusao do pagamento, usando `System.Windows.Forms.Timer` e sem bibliotecas externas.

## Requisitos

- Windows
- Visual Studio 2022
- .NET 8 SDK com suporte a Windows Forms

## Como executar

Abra `PayNexus.sln` no Visual Studio 2022 e execute o projeto.

Tambem e possivel rodar pelo terminal:

```powershell
dotnet restore
dotnet run --project PayNexus.csproj
```

## Estrutura principal

- `Views/`: formularios da dashboard e de novo pagamento.
- `Components/`: componentes visuais reutilizaveis, incluindo a animacao de pagamento.
- `Services/`: orquestracao do fluxo de pagamento.
- `Core/`: sistema de pagamento, estrategia ativa e notificacao dos observers.
- `Strategies/`: regras simuladas para cada metodo de pagamento.
- `Observers/`: observers executados apos o processamento.
- `Models/`: entidades e resumos usados pela aplicacao.
