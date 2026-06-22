FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution + projects
COPY FitnessClub_Test.sln ./

COPY FitnessClub_Test.API/FitnessClub_Test.API.csproj FitnessClub_Test.API/
COPY FitnessClub_Test.CMS/FitnessClub_Test.CMS.csproj FitnessClub_Test.CMS/
COPY FitnessClub_Test.Core/FitnessClub_Test.Core.csproj FitnessClub_Test.Core/
COPY FitnessClub_Test.Common/FitnessClub_Test.Common.csproj FitnessClub_Test.Common/
COPY FitnessClub_Test.DTOs/FitnessClub_Test.DTOs.csproj FitnessClub_Test.DTOs/
COPY FitnessClub_Test.AIAssistant/FitnessClub_Test.AIAssistant.csproj FitnessClub_Test.AIAssistant/

# Restore
RUN dotnet restore FitnessClub_Test.API/FitnessClub_Test.API.csproj

# Copy everything
COPY . .

# Publish
RUN dotnet publish FitnessClub_Test.API/FitnessClub_Test.API.csproj -c Release -o /app/publish

FROM base AS final
WORKDIR /app

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "FitnessClub_Test.API.dll"]