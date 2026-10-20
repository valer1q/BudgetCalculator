Модульные тесты на xUnit для контроллера `BudgetController`.

Тесты:

```bash
dotnet test
```

Тесты на версии до исправления бага:

```bash
git checkout 05db6d3
dotnet test
git checkout main
```

Покрытие кода:

```bash
dotnet test --collect:"XPlat Code Coverage"
```

HTML-отчёт о покрытии:

```bash
dotnet tool install -g dotnet-reportgenerator-globaltool
reportgenerator -reports:"BudgetCalculator.Tests/TestResults/*/coverage.cobertura.xml" -targetdir:coveragereport -reporttypes:Html
```

Запуск API:

```bash
dotnet run --project BudgetCalculator --launch-profile http
```
