FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /app
COPY MangosVB.props MangosVB.props
COPY ./Source/ ./Source/
RUN dotnet publish /app/Source/WorldCluster/WorldCluster.vbproj -c Release -o bin -r linux-x64

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
WORKDIR /app
COPY --from=build /app/bin .
COPY ./.docker/configs ./configs
COPY ./.docker/dbc ./dbc
EXPOSE 50001
ENTRYPOINT ["./WorldCluster"]