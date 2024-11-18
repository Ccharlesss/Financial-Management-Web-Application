# # 1) Define base image from which the final runtime environment will be built:
# # Includes needed dependencies to run an ASP.NET Core app but doesn't contain the SDK for compiling the code:
# FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base

# # 2) Define the working directory in the base image to /app:
# WORKDIR /app

# EXPOSE 80
# EXPOSE 443


# # 4) Build stage: 
# FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
# WORKDIR /src
# COPY ["ManageFinance.csproj", "./"]
# RUN dotnet restore "./ManageFinance.csproj"

# # 5) Copy the application code and build in the docker container in release mode:
# COPY . .
# WORKDIR "/src"
# RUN dotnet build "ManageFinance.csproj" -c Release -o /app/build

# # 6) Publish stage
# FROM build AS publish
# RUN dotnet publish "ManageFinance.csproj" -c Release -o /app/publish

# # 7) Final stage
# FROM base AS final
# WORKDIR /app
# COPY --from=publish /app/publish .


# ENTRYPOINT ["dotnet", "ManageFinance.dll"]





# ============================================================
# ============================
# 1) Build Stage (SDK Image)
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Set working directory inside the container for build operations
WORKDIR /src

# Copy the project file(s) to restore dependencies
COPY *.csproj ./
RUN dotnet restore

# Copy all source code (after restoring dependencies to use Docker cache effectively)
COPY . ./

# Publish the app in Release configuration to the 'out' folder
# Specifies the output directory (/app/out) where the built application and its dependencies will be placed.
# Contains: Compiled Code, Dependencies, Configuration Files, Static Files (if applicable):
RUN dotnet publish -c Release -o /app/out


# ============================
# 2) Runtime Stage (Runtime Image)
# During the runtime stage, only the /app/out directory is copied to the final image:
# This ensures the runtime image includes: None of the source code or SDK tools from the build stage. Only what is required to run app
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime

# Set the working directory inside the container for runtime
WORKDIR /app

# Copy the published output from the build stage
COPY --from=build /app/out .

# Expose ports for HTTP and HTTPS
EXPOSE 80
EXPOSE 443

# Specify the entry point to run the app
ENTRYPOINT ["dotnet", "ManageFinance.dll"]
