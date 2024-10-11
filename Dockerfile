# 1) Define base image from which the final runtime environment will be built:
# Includes needed dependencies to run an ASP.NET Core app but doesn't contain the SDK for compiling the code:
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base

# 2) Define the working directory in the base image to /app:
WORKDIR /app

# 3) Expose the ports that will be used (port 80 for HTTP) (port 443 for HTTPS):
EXPOSE 80
EXPOSE 443

# 4) Build stage: 
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["ManageFinance.csproj", "./"]
RUN dotnet restore "./ManageFinance.csproj"

# 5) Copy the application code and build in the docker container in release mode:
COPY . .
WORKDIR "/src"
RUN dotnet build "ManageFinance.csproj" -c Release -o /app/build

# 6) Publish stage
FROM build AS publish
RUN dotnet publish "ManageFinance.csproj" -c Release -o /app/publish

# 7) Final stage
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .


ENTRYPOINT ["dotnet", "ManageFinance.dll"]
