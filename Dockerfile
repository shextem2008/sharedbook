FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS base
WORKDIR /app
EXPOSE 80
# EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /src
COPY ["SharedBook.csproj", "./"]
RUN dotnet restore "SharedBook.csproj"
COPY . .
RUN dotnet build "SharedBook.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SharedBook.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SharedBook.dll"]
