FROM microsoft/dotnet:2.2-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /src
COPY ["GNotes/GNotes.csproj", "GNotes/"]
RUN dotnet restore "GNotes/GNotes.csproj"
COPY . .
WORKDIR "/src/GNotes"
RUN dotnet build "GNotes.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "GNotes.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "GNotes.dll"]