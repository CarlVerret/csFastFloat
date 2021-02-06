FROM mcr.microsoft.com/dotnet/sdk:5.0

RUN dotnet build "Benchmark2.csproj" -c Release 
RUN dotnet publish "Benchmark2.csproj" -c Release 

COPY bin/Release/net5.0/publish/ App/
WORKDIR /App
ENTRYPOINT ["dotnet", "Benchmark2.dll"]