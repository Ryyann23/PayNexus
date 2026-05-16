# PayNexus

PayNexus e um aplicativo Windows Forms para simular o processamento de pagamentos em uma dashboard operacional. A tela principal mostra resumo financeiro, transacoes recentes, logs em tempo real e observers ativos.

## Recursos

- Cadastro de novo pagamento com cliente, valor e metodo.
- Metodos simulados: PIX, Cartao de Credito e Boleto.
- Dashboard com total processado, transacoes do dia, aprovados e pendentes.
- Logs operacionais em tempo real.
- Uso do padrao Strategy para trocar dinamicamente o algoritmo de pagamento.
- Uso do padrao Adapter para integrar gateways simulados com interfaces diferentes ao fluxo interno do sistema.
- Uso do padrao Observer para notificar acoes apos o processamento.
- Animacoes visuais antes da conclusao do pagamento, usando `System.Windows.Forms.Timer` e sem bibliotecas externas.

## Padroes de projeto no sistema

### Strategy

O sistema escolhe a estrategia de pagamento em tempo de execucao conforme o metodo selecionado no formulario de novo pagamento.

- `PagamentoPix`: processa pagamentos PIX.
- `PagamentoCartao`: processa pagamentos no cartao de credito, incluindo uma validacao antifraude simulada.
- `PagamentoBoleto`: emite boleto e deixa a transacao como pendente.

A classe `SistemaPagamento` recebe a estrategia ativa por meio de `DefinirEstrategia` e executa o pagamento sem depender diretamente da regra concreta.

### Adapter

Cada estrategia usa um adapter para conversar com um gateway externo simulado:

- `PixGatewayAdapter`: adapta o gateway de banco PIX.
- `CartaoGatewayAdapter`: adapta o gateway de adquirente de cartao.
- `BoletoGatewayAdapter`: adapta o gateway de emissao de boletos.

Esses gateways possuem formatos de entrada e retorno diferentes, mas os adapters convertem tudo para o modelo interno `ResultadoPagamento`. Assim, o restante do sistema continua trabalhando com uma interface unica.

### Observer

Depois que a strategy processa o pagamento usando o adapter correspondente, o `SistemaPagamento` notifica os observers registrados:

- `ObserverEmail`
- `ObserverLog`
- `ObserverHistorico`
- `ObserverStatus`
- `ObserverNotificacaoSistema`, que pode ser adicionado em tempo de execucao pela dashboard.

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
- `Adapters/`: adapters que integram gateways simulados ao modelo interno de pagamentos.
- `Strategies/`: regras simuladas para cada metodo de pagamento.
- `Observers/`: observers executados apos o processamento.
- `Models/`: entidades e resumos usados pela aplicacao.
