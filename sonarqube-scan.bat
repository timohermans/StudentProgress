dotnet-sonarscanner begin /k:"student-progress" /d:sonar.host.url="http://localhost:9000" /d:sonar.login="1b0641ceae46e06d159f310a37738ccb73cdf2d1"
dotnet build .\StudentProgress.sln
dotnet-sonarscanner end /d:sonar.login="1b0641ceae46e06d159f310a37738ccb73cdf2d1"