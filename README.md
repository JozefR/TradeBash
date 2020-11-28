# TradeBash

Docker setup for sql server on mac os

1. pull sql server 2017
2. run

        docker run -e 'ACCEPT_EULA=Y' -e 'SA_PASSWORD=Pwd12345!' -p 1433:1433 -d mcr.microsoft.com/mssql/server:2017-latest
        
### Apply migrations

1. dotnet ef migrations add InitialCreate 
2. dotnet ef database update