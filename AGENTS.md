# Agents Guidelines

## Coding conventions

- Do not use the `Async` suffix for asynchronous methods.
- Add a blank line before a return statement.
- Prefer typed `Results.[method]()` helpers over `Results.Problem()` for endpoint responses; reserve `Results.Problem()` for cases without an appropriate typed helper.
- Use constants for values that are used more than once; inline values that are only used once.
- Use camelCase for constants declared within methods.
- Lint files changed or created using `dotnet csharpier format .`.
- Name expressions with `x => x.` syntax where possible.
- Use collection expressions and object initializers where possible.
- Merge related conditionals where doing so keeps the condition clear.
- Prefer `??` directly in a return statement when it clearly expresses a null fallback or exception.
- Use `_camelCase` for private instance fields.

## Change iterations

- Before adding an endpoint or changing proxy behaviour, compare the nearest existing implementation. If the change needs a one-off request, validation, error-response, or documentation pattern, pause and ask the user before introducing it.
- Keep `GET /health` local to this service. It is a CDP platform health-check contract and must continue to return HTTP 200 with `{ "message": "success" }`.
- Preserve forwarding for all HTTP methods unless a route explicitly restricts them. In particular, do not accidentally exclude `POST` requests.
- Keep `unconfigured.invalid` as a fail-closed destination placeholder. Startup validation must reject it in every configured YARP destination.
- Check work has been successful by building the solution.

## Build guidance

- In the sandbox environment, avoid plain `dotnet build` because it can hang or take significantly longer due to workload notification and build-server delays.
- Build with `DOTNET_CLI_WORKLOAD_UPDATE_NOTIFY_DISABLE=1 dotnet build report-packaging-proxy-spike.slnx --no-restore -m:1 -nodeReuse:false --disable-build-servers -v:minimal`.
- If a build is unexpectedly slow, stop it, run `dotnet build-server shutdown`, and retry the sandbox build command above.

## Test guidance

- Run unit tests with `DOTNET_CLI_WORKLOAD_UPDATE_NOTIFY_DISABLE=1 dotnet test tests/ReverseProxy.Tests/ReverseProxy.Tests.csproj --no-restore -m:1 -nodeReuse:false --disable-build-servers -v:minimal`.
- Keep integration tests focused on real integration boundaries. This service's routing tests run against the Docker Compose proxy and WireMock downstream.
- Start the local environment with `docker compose up --build -d --wait`.
- Run the integration tests with `DOTNET_CLI_WORKLOAD_UPDATE_NOTIFY_DISABLE=1 dotnet test tests/ReverseProxy.IntegrationTests/ReverseProxy.IntegrationTests.csproj --no-restore -m:1 -nodeReuse:false --disable-build-servers -v:minimal`.
- Stop the local environment with `docker compose down -v --remove-orphans`.
- In the sandbox environment, integration tests need escalation because VSTest binds a local socket and the tests access Docker Compose services.
