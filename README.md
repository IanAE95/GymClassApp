API RESTful para gerenciamento de agendamentos de aulas em academia. Desenvolvida em .NET 8 com Entity Framework e SQLServer

# Pré-requisitos:
    .NET 8 SDK
    SQLServer
    Visual Studio 2022/VSCode (opcional)
    Insomnia (para testes da API)

# Executar aplicação:
    dotnet run --project GymClass

# Comandos para executar tests:
     dotnet test --list-tests //Lista todos os testes disponíveis
     dotnet test -v diag  //Executa todos os tests com output detalhado para debugging
     dotnet test --filter "DeveBloquearAgendamento_QuandoAulaInativa" //Executa apenas um teste específico

# Testando com insomnia:
    Abra o Insomnia
    Clique em Import/Export → Import Data → From File
    Selecione o arquivo docs/Gym-Api-Insomnia.yaml


  


