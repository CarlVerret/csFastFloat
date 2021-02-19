FROM mcr.microsoft.com/dotnet/sdk:5.0

RUN dotnet build "BenchmarkHandCoded.csproj" -c Release 
RUN dotnet publish "BenchmarkHandCoded.csproj" -c Release 

COPY bin/Release/net5.0/publish/ App/
WORKDIR /App
ENTRYPOINT ["dotnet", "BenchmarkHandCoded.dll"]