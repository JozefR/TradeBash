# TradeBash

1. First IEXCloud api data provider accounts need's to be created and injected into appsettings.jcon in web project
    - step to step process how to create api can be found on https://iexcloud.io/
    - for test purpose i already setted one, it should work 


    "IEXConnection" : "https://cloud.iexapis.com/stable/stock/{0}/chart/{1}?token=pk_b05d7db530fb4f9da669428e49ea940c"

2. Apply migration for creating new database
    - execute 
      - "dotnet ef migrations add InitialCreate"
      - "dotnet ef database update"


3. open api http://localhost:5001/swagger/index.html
    - try to play with testing strategies
