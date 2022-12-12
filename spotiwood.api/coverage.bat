dotnet test Spotiwood.Api.sln --collect:"XPlat Code Coverage"
reportgenerator -reports:./**/coverage.cobertura.xml -targetdir:./coverage -reporttypes:Cobertura
reportgenerator -reports:./coverage/Cobertura.xml -targetdir:./coverage/report -reporttypes:HtmlInline