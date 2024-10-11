# 🎾 Automation Tennis Service - Notificações WTA

Esta aplicação foi criada para enviar notificações diárias em um canal do **Slack** com informações sobre os torneios da **WTA (Women's Tennis Association)** do mês e os resultados ou partidas do dia que ainda não foram realizadas. As notificações são enviadas por meio de um **Webhook do Slack**, garantindo que os usuários se mantenham atualizados sobre os principais eventos do tênis feminino.

## 🛠️ Tecnologias Utilizadas

- **Backend**: Desenvolvido em **C# com .NET 8**.
- **Agendamento de Tarefas**: **Hangfire** para o agendamento e gerenciamento de tarefas em background.
- **Banco de Dados**: **SQLite** para armazenar dados de torneios e partidas.
- **Integração com Slack**: Envio das notificações via **Webhook do Slack** para um canal específico.
- **Cliente HTTP**: Utilizado para se conectar às APIs externas e obter os dados atualizados dos torneios e partidas da WTA.


## ⚙️ Como Funciona

1. **Webhook do Slack**: A aplicação se integra ao Slack utilizando um **Webhook**, que permite o envio de mensagens para um canal específico.
2. **Tarefas Diárias**: Todos os dias, uma tarefa recorrente é executada para buscar os dados mais recentes dos torneios e partidas da WTA e enviá-los ao canal de Slack configurado.
3. **Obtenção de Dados**: As informações sobre os torneios e partidas são obtidas por meio da API da WTA e armazenadas em um banco de dados **SQLite**.
4. **Notificações no Slack**: A cada dia, a aplicação envia uma mensagem ao Slack com:
   - O **nome do torneio** em andamento.
   - As **partidas do dia**, mostrando os resultados ou informando as partidas ainda por acontecer.
5. **Deploy Automatizado**: O deploy da aplicação é feito automaticamente na **Azure** por meio de **GitHub Actions**, garantindo uma integração contínua e sem complicações.


## 🌟 Funcionalidades

- **Notificações Diárias no Slack**: Envia automaticamente atualizações diárias para um canal no Slack, com os torneios do mês e as partidas do dia (resultados e partidas futuras).
- **Torneios da WTA**: Lista todos os torneios da WTA do mês em curso.
- **Resultados de Partidas**: Mostra os resultados das partidas concluídas e as que estão programadas para o dia.
- **Tarefas Recorrentes com Hangfire**: Utiliza o **Hangfire** para agendar e gerenciar tarefas recorrentes, garantindo o envio diário das notificações.
- **Banco de Dados SQLite**: A aplicação utiliza o banco de dados **SQLite** para armazenar as informações necessárias sobre os torneios e partidas.

## Listagem de torneios no canal
![image](https://github.com/user-attachments/assets/9c0018f1-e1e3-46c1-9c7a-b92170d43a14)

## Listagem de jogos do dia no canal
![image](https://github.com/user-attachments/assets/8bfca2af-8f72-4fbb-ab67-d5d950b1bbcb)


