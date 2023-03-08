dotnet test --no-build
dotnet reportgenerator -reports:.\TestCyjb\coverage.cobertura.xml -targetdir:.\CodeCoverage
