FROM microsoft/dotnet:2.2-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /src
COPY ["TAOAuth/TAOAuth.csproj", "TAOAuth/"]
RUN dotnet restore "TAOAuth/TAOAuth.csproj"
COPY . .
WORKDIR "/src/TAOAuth"
RUN dotnet build "TAOAuth.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "TAOAuth.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "TAOAuth.dll"]
