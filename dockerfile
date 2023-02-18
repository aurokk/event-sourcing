FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /build

COPY EventSourcing.sln     .
COPY Directory.Build.props .

# Копировать руками csproj-и нужно для того, чтобы кэшировать нагет пакеты
# Чтоб быстро всё сделать: tree -i -f | grep .csproj
COPY src/Common.Abstractions/Common.Abstractions.csproj                     ./src/Common.Abstractions/
COPY src/Common.Abstractions.UnitTests/Common.Abstractions.UnitTests.csproj ./src/Common.Abstractions.UnitTests/
COPY src/Orders.Api/Orders.Api.csproj                                       ./src/Orders.Api/
COPY src/Orders.DataAccess/Orders.DataAccess.csproj                         ./src/Orders.DataAccess/
COPY src/Orders.Domain/Orders.Domain.csproj                                 ./src/Orders.Domain/
COPY src/Orders.PaymentsClient/Orders.PaymentsClient.csproj                 ./src/Orders.PaymentsClient/
COPY src/Payments.Api/Payments.Api.csproj                                   ./src/Payments.Api/
COPY src/Payments.Contracts/Payments.Contracts.csproj                       ./src/Payments.Contracts/
COPY src/Payments.DataAccess/Payments.DataAccess.csproj                     ./src/Payments.DataAccess/
COPY src/Payments.Domain/Payments.Domain.csproj                             ./src/Payments.Domain/
COPY tests/E2eTests/E2eTests.csproj                                         ./tests/E2eTests/

RUN dotnet restore ./EventSourcing.sln
# RESTORE END

COPY src/.   ./src/
COPY tests/. ./tests/

# BUILD START
RUN dotnet build ./EventSourcing.sln \
    --no-restore \
    -c Release
# BUILD END

# UNIT TESTS START
RUN dotnet test ./EventSourcing.sln \
    --filter TestCategory!=E2E \
    --no-build \
    --no-restore \
    -c Release
# UNIT TESTS END

# PUBLISH APP START
RUN dotnet publish \
    --no-build \
    --no-restore \
    -c Release \
    -o /dist/orders-api \
    ./src/Orders.Api/Orders.Api.csproj
# PUBLISH APP END

# PUBLISH ADMIN START
RUN dotnet publish \
    --no-build \
    --no-restore \
    -c Release \
    -o /dist/payments-api \
    ./src/Payments.Api/Payments.Api.csproj
# PUBLISH ADMIN END

# PUBLISH E2E-TESTS START
RUN dotnet publish \
    --no-build \
    --no-restore \
    -c Release \
    -o /dist/e2e-tests \
    ./tests/E2eTests/E2eTests.csproj
# PUBLISH E2E-TESTS END

FROM mcr.microsoft.com/dotnet/aspnet:7.0 as orders-api
WORKDIR /app
COPY --from=build dist/orders-api ./

ENV DOTNET_ReadyToRun=0
# Unset default env value "http://+:80" from aspnet image
# ENV ASPNETCORE_URLS=

ENTRYPOINT ["dotnet", "Orders.Api.dll"]

FROM mcr.microsoft.com/dotnet/aspnet:7.0 as payments-api
WORKDIR /app
COPY --from=build dist/payments-api ./

ENV DOTNET_ReadyToRun=0
# Unset default env value "http://+:80" from aspnet image
# ENV ASPNETCORE_URLS=

ENTRYPOINT ["dotnet", "Payments.Api.dll"]

FROM mcr.microsoft.com/dotnet/aspnet:7.0 as e2e-tests
WORKDIR /app
COPY --from=build dist/e2e-tests ./

ENV DOTNET_ReadyToRun=0

ENTRYPOINT ["dotnet", "E2eTests.dll"]