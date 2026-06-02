# LabMS Postman Testing Kit

This folder contains:

- LabMS.postman_environment.json: Environment variables (base URL, credentials, JWT storage).
- Bootstrap.postman_collection.json: A small helper collection that logs in and demonstrates using the token. Use this with the environment to validate auth.
- Instructions to import ALL endpoints from Swagger into Postman and automate testing with Newman.

## 1) Import ALL endpoints via Swagger (recommended)

The project exposes an OpenAPI document at:

- Development: https://localhost:<port>/swagger/v1/swagger.json
- If running HTTP: http://localhost:<port>/swagger/v1/swagger.json

Steps:
1. Run the API (Debug or `dotnet run`).
2. Open Postman -> Import -> Link -> paste the swagger.json URL above.
3. Postman will generate a complete collection for all controllers and endpoints.
4. Select the `LabMS` environment (provided in this folder) for base URL and auth.

Notes:
- This ensures you always have a complete and up-to-date endpoint list without manual maintenance.

## 2) Environment setup

Import `LabMS.postman_environment.json`.

Update variables:
- baseUrl: e.g., https://localhost:5001 or http://localhost:5000
- username/password: test account to login (must exist). If none, call `Auth/register` first.
- issuer/audience: optional, used only in certain flows

## 3) Auth automation in Postman

Use the `Bootstrap` collection together with the environment. It contains a pre-request script to:
- Check if a token exists and is not expired
- If missing/expired, call `POST /api/Auth/login` with environment `username`/`password`
- Store `token` and `tokenExpiresAt` in the environment
- Set `Authorization: Bearer <token>` header automatically

To apply this behavior to your Swagger-imported collection, copy the pre-request script from Bootstrap to the collection/folder or requests.

## 4) Running automated tests with Newman

Install newman globally:

```bash
npm install -g newman
```

Run your Swagger-imported collection with the provided environment:

```bash
newman run "<Your Swagger Imported Collection>.postman_collection.json" \
  -e Postman/LabMS.postman_environment.json \
  --timeout-request 60000 \
  --insecure
```

Tips:
- Add folder-level test suites (e.g., Smoke, Regression) in Postman.
- Use data files for parameterized runs:

```bash
newman run collection.json -e env.json -d data.csv
```

## 5) Optional: GitHub Actions CI

```yaml
name: API Tests
on: [push, pull_request]
jobs:
  newman:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v3
      - uses: actions/setup-node@v4
        with:
          node-version: '20'
      - run: npm install -g newman
      - name: Run API tests
        run: |
          newman run "<Your Swagger Imported Collection>.postman_collection.json" \
            -e Postman/LabMS.postman_environment.json \
            --timeout-request 60000 --insecure
```

## 6) Common setup pitfalls

- If you see 400 with message `JWT_SECRET_KEY environment variable is required`, set the secret via User Secrets, env vars, or launchSettings and restart.
- Ensure `username/password` in environment match a real user, or run register first.
- For HTTPS self-signed dev certs, you may need `--insecure` with Newman.

## 7) Next steps

- After importing Swagger, group endpoints in Postman (Smoke, Auth-required, Inventory, Billing).
- Add Postman Tests to critical endpoints to assert status codes and payloads.

If you want, I can also export a ready-made Swagger-imported collection from your running app and check in the JSON under `Postman/` for consistency across the team.
