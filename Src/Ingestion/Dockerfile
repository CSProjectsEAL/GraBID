FROM microsoft/dotnet:2.1-sdk AS build
COPY . ./build
WORKDIR /build/
RUN dotnet restore
RUN dotnet build --no-restore -c Release -o /app

FROM build AS publish
RUN dotnet publish --no-restore -c Release -o /app

FROM microsoft/dotnet:2.1-runtime-alpine

COPY --from=publish /app /var/app
COPY entrypoint.sh /var/app
COPY ./crontab /etc/crontabs
RUN chmod -R 644 /etc/crontabs
COPY wait-for-it.sh /var/app
RUN apk add bash 
CMD ["bash", "/var/app/wait-for-it.sh", "rabbit.docker:5672", "--timeout=30","--", "/var/app/entrypoint.sh"]
