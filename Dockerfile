#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/sdk:3.1 AS build
WORKDIR /app
COPY MC.sln .
COPY ./src/ ./src
COPY ./tests/ ./tests
RUN dotnet restore MC.sln

# copy full solution.
COPY . .
RUN dotnet build -c Release -o /app/build --no-restore

# --target testrunner
FROM build AS testrunner
WORKDIR "/app/tests/Topsis.Acceptance.Tests"
CMD ["dotnet", "test", "--logger:trx"]

# USAGE: build to the test target of the Dockerfile
# docker build --target testrunner -t app:latest . # runs the unit tests
# docker run example-service-tests:latest

# --target tests
FROM build AS test

## LOCAL_BUILD
#ARG APP_ENV=Staging
#ENV ASPNETCORE_ENVIRONMENT=$APP_ENV
#RUN if [ "$APP_ENV" = "production" ] ; then echo "production env"; else echo "non-production env: $APP_ENV"; fi
#ARG GOOGLE_APPLICATION_CREDENTIALS=m6-staging-service-account.json
#ENV GOOGLE_APPLICATION_CREDENTIALS $GOOGLE_APPLICATION_CREDENTIALS
#RUN echo "service account file: $GOOGLE_APPLICATION_CREDENTIALS";

WORKDIR "/app/tests/Topsis.Acceptance.Tests"
RUN dotnet test --logger:trx

FROM build AS publish
WORKDIR "/app/src/Topsis.Web"
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:3.1 AS runtime
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM runtime AS final
WORKDIR /app

## LOCAL_BUILD
#ARG APP_ENV=Staging
#ENV ASPNETCORE_ENVIRONMENT=$APP_ENV
#RUN if [ "$APP_ENV" = "production" ] ; then echo "production env"; else echo "non-production env: $APP_ENV"; fi
#ARG GOOGLE_APPLICATION_CREDENTIALS=m6-staging-service-account.json
#ENV GOOGLE_APPLICATION_CREDENTIALS $GOOGLE_APPLICATION_CREDENTIALS
#RUN echo "service account file: $GOOGLE_APPLICATION_CREDENTIALS";

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Topsis.Web.dll"]