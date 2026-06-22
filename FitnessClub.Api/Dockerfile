FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution + projects
COPY FitnessClub_Test.sln ./

COPY FitnessClub.Api/FitnessClub.Api.csproj FitnessClub.Api/
COPY FitnessClub.CMS.MVC/FitnessClub.CMS.MVC.csproj FitnessClub.CMS.MVC/
COPY FitnessClub.Core/FitnessClub.Core.csproj FitnessClub.Core/
COPY FitnessClub.Common/FitnessClub.Common.csproj FitnessClub.Common/
COPY FitnessClub.Dtos/FitnessClub.Dtos.csproj FitnessClub.Dtos/
COPY FitnessClub.AIAssistant/FitnessClub.AIAssistant.csproj FitnessClub.AIAssistant/

# Restore
RUN dotnet restore FitnessClub.Api/FitnessClub.Api.csproj

# Copy everything
COPY . .

# Publish
RUN dotnet publish FitnessClub.Api/FitnessClub.Api.csproj -c Release -o /app/publish

FROM base AS final
WORKDIR /app

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "FitnessClub_Test.Api.dll"]