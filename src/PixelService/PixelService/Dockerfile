# Stage 1: Build and publish the app
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /app
COPY . .
WORKDIR /app/PixelService
RUN dotnet publish -c Release -o out

# Stage 2: Run the app
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=build /app/PixelService/out .
ENTRYPOINT ["dotnet", "PixelService.dll"]