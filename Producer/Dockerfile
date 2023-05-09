FROM mcr.microsoft.com/dotnet/sdk:7.0 AS dev
WORKDIR /app

COPY ["Producer/Producer.csproj", "Producer/"]
RUN dotnet restore "Producer/Producer.csproj"

COPY ["Producer/", "/"]
COPY ["KafkaDotNet.sln", "/"]

WORKDIR "/app/Producer"

ENTRYPOINT ["dotnet", "watch", "run", "--urls=http://+:5002"]
