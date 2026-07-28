# Report packaging proxy

A .NET 10 YARP reverse proxy prepared for the CDP build and deployment approach used by
`cdp-node-frontend-template`.

## Behaviour

- `GET /health` is handled by this service and returns `200 OK` with `{ "message": "success" }`.
- Every other path, with no HTTP-method restriction, is forwarded to the `backend` YARP cluster. This includes
  `POST` requests.
- The default backend is `http://localhost:8080/`. Override it in an environment with
  `ReverseProxy__Clusters__backend__Destinations__primary__Address`, for example:

  ```sh
  ReverseProxy__Clusters__backend__Destinations__primary__Address=https://report-packaging.example/
  ```

The container listens on `PORT` (default `8085`) and includes `curl` for the CDP platform health check.

## Run locally

```sh
dotnet restore spike-report-packaging-proxy.slnx
dotnet run --project src/Api
```

Then check the local endpoint:

```sh
curl http://localhost:8085/health
```

## Container

```sh
docker build --tag spike-report-packaging-proxy .
docker run --rm -p 8085:8085 \
  -e ReverseProxy__Clusters__backend__Destinations__primary__Address=http://host.docker.internal:8080/ \
  spike-report-packaging-proxy
```
