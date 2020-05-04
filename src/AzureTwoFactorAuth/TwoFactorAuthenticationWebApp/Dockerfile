FROM microsoft/dotnet:2.1-aspnetcore-runtime-nanoserver-1709 AS base
WORKDIR /app
EXPOSE 1739
EXPOSE 44325

FROM microsoft/dotnet:2.1-sdk-nanoserver-1709 AS build
WORKDIR /src
COPY TwoFactorAuthenticationWebApp/TwoFactorAuthenticationWebApp.csproj TwoFactorAuthenticationWebApp/
RUN dotnet restore TwoFactorAuthenticationWebApp/TwoFactorAuthenticationWebApp.csproj
COPY . .
WORKDIR /src/TwoFactorAuthenticationWebApp
RUN dotnet build TwoFactorAuthenticationWebApp.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish TwoFactorAuthenticationWebApp.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "TwoFactorAuthenticationWebApp.dll"]
