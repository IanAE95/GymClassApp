API RESTful para gerenciamento de agendamentos de aulas em academia. Desenvolvida em .NET 8 com Entity Framework e SQLServer

# Tecnologias Utilizadas
    -.NET 8 - Framework principal
    -Entity Framework Core - ORM para acesso a dados
    -SQLServer - Banco de dados
    -Swagger/OpenAPI - Documentação interativa

# Pré-requisitos:
    -.NET 8 SDK
    -SQLServer
    -Visual Studio 2022 ou VS Code
    -Insomnia (para testes da API)

# Executar aplicação:
    dotnet run --project GymClass

# Comandos para executar tests:
    Lista todos os testes disponíveis
      dotnet test --list-tests
    Executa todos os tests com output detalhado para debugging
      dotnet test -v diag
     Executa apenas um teste específico
      dotnet test --filter "DeveBloquearAgendamento_QuandoAulaInativa"

# Testando com insomnia:
    -Abra o Insomnia
    -Clique em Import/Export → Import Data → From File
    -Selecione o arquivo docs/Gym-Api-Insomnia.yaml


  


