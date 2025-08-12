# Build and publish the gateway explicitly by csproj name
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy only csproj first to leverage Docker layer caching
COPY ApiGateway.csproj ./
RUN dotnet restore ApiGateway.csproj

# Copy the rest and publish
COPY . .
RUN dotnet publish ApiGateway.csproj -c Release -o /out

FROM base AS final
WORKDIR /app
COPY --from=build /out .
ENV ASPNETCORE_URLS=http://+:8080
# IMPORTANT: match the assembly name produced by your csproj
ENTRYPOINT ["dotnet", "ApiGateway.dll"]
