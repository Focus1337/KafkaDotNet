FROM mcr.microsoft.com/dotnet/sdk:7.0 AS dev
WORKDIR /app

COPY ["Consumer/Consumer.csproj", "Consumer/"]
RUN dotnet restore "Consumer/Consumer.csproj"

COPY ["Consumer/", "/"]
COPY ["KafkaDotNet.sln", "/"]

WORKDIR "/app/Consumer"

ENTRYPOINT ["dotnet", "watch", "run"]
