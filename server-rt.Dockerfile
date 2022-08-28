FROM mcr.microsoft.com/dotnet/nightly/sdk:6.0 AS build
WORKDIR /app
EXPOSE 3355
WORKDIR /src
COPY ./server-rt.sln ./
COPY ./server-rt/*.csproj ./server-rt/
# COPY ./ServiceRunner/*.csproj ./ServiceRunner/

RUN dotnet restore ; exit 0

# Copy everything else and build
COPY . ./

WORKDIR /src/server-rt
RUN dotnet build -c Release -o /app
# WORKDIR /src/ServiceRunner
# RUN dotnet build -c Release -o /app
RUN dotnet publish -c Release -o out

#WORKDIR /app
#RUN echo $(ls)

# Build runtime image
FROM mcr.microsoft.com/dotnet/nightly/runtime:6.0 as desination
WORKDIR /app
#RUN mkdir /app
#RUN mkdir /app/out
#WORKDIR /app/out
COPY --from=build /app .
#RUN echo $(ls)
ENTRYPOINT ["dotnet", "server-rt.dll"]



