# Employee Management documentation

This repository contains a VitePress guide for the Employee Management ASP.NET Core API and Angular application in `samples/`.

```powershell
npm ci
npm run docs:dev
```

The website starts with [docs/index.md](docs/index.md) and includes the important source code, SQL setup, and end-to-end testing steps. Build the static site for Vercel with `npm run docs:build`. Vercel hosts the documentation only, not the API, Angular app, or SQL Server.

The API includes `appsettings.json` with the provided SQL connection and JWT settings. These values are public in this repository; replace them before using the project with any real database or deployment. The API also has security gaps, including client-controlled registration roles and unprotected employee endpoints; do not use its authorization model in production without fixing them.
