# Event Sourcing example app

## Запустить e2e-тесты

Эта команда соберет и запустит все сервисы и прогонит на них e2e-тесты

```
docker compose \
    run \
    --build \
    e2e-tests
```

## Локально подебажить

Эта команда запустит только инфраструктуру, затем можно запустить все приложения локально и подебажить

```
docker compose \
    up orders-eventstore orders-mongo payments-eventstore payments-mongo rabbitmq \
    --build \
    -d
```

## Остановить и удалить все сервисы

Эта команда остановит все контейнеры, удалит все виртуальные диски и прочие созданные ресурсы

```
docker compose \
    down \
    --remove-orphans \
    -v
```