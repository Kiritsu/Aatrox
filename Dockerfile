#Build
FROM mcr.microsoft.com/dotnet/core/sdk:3.0 AS builder
WORKDIR /app
COPY . ./
RUN dotnet publish -c Release -o /app

#Run
FROM mcr.microsoft.com/dotnet/core/runtime:3.0
WORKDIR /app
COPY --from=builder /app .

ENTRYPOINT [ "dotnet" ] 
CMD [ "Aatrox.dll" ]