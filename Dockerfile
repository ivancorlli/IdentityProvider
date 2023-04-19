ARG VERSION=7.0-alpine

FROM mcr.microsoft.com/dotnet/sdk:$VERSION AS build-env
WORKDIR /app
ADD *.csproj .
RUN dotnet restore
ADD . .
RUN dotnet publish \
  -c Release \
  -o ./build
#RUN dotnet ef migrations bundle --configuration Bundle --self-contained -r linux-x64 -o Bundle/Migration --verbose 
FROM mcr.microsoft.com/dotnet/aspnet:$VERSION
WORKDIR /app
COPY --from=build-env /app/build .
ENV DOTNET_RUNNING_IN_CONTAINER=true \
  ASPNETCORE_URLS=http://+:5005 \
  ASPNETCORE_ENVIRONMENT=Production
EXPOSE 5005
ENTRYPOINT ["dotnet", "OAuthServer.dll"]