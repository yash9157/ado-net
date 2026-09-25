# 5. Set up Angular

The second project is Angular 21. Its folder is spelled `Employeemanagmnet`; keep that spelling when running commands.

## Install and start the included app

From the repository root:

```powershell
cd samples/Employeemanagmnet
npm ci
npm start
```

Open `http://localhost:4200`. The package file defines Angular, Bootstrap, and its scripts:

**File:** `package.json` — **Use:** Defines Angular dependencies and commands.

<<< ../samples/Employeemanagmnet/package.json{json}

To build it yourself from a new Angular project, create a standalone, routed CSS application with Angular CLI 21, then copy the TypeScript, HTML, and CSS shown in these pages into the matching `src/app` locations. Install Bootstrap (`npm install bootstrap@5.3.8`) and ensure `src/styles.css` imports it. The supplied project uses this global stylesheet:

**File:** `src/styles.css` — **Use:** Loads global styles and Bootstrap.

<<< ../samples/Employeemanagmnet/src/styles.css{css}

The project has `src/app/models`, `services`, `guards`, `interceptors`, and `components/{login,register,employee-list,employee-form}`. Those are code organization folders, not separate Angular applications.

The root component contains the router outlet where each screen appears. Angular bootstraps it from `main.ts`:

**File:** `src/main.ts` — **Use:** Bootstraps the Angular app.

<<< ../samples/Employeemanagmnet/src/main.ts{ts}

**File:** `src/app/app.ts` — **Use:** Defines the root component.

<<< ../samples/Employeemanagmnet/src/app/app.ts{ts}

**File:** `src/app/app.html` — **Use:** Displays the current route.

<<< ../samples/Employeemanagmnet/src/app/app.html{html}

The root component's local stylesheet is:

**File:** `src/app/app.css` — **Use:** Styles the root component.

<<< ../samples/Employeemanagmnet/src/app/app.css{css}

## Connect to the API

The environment file points to the API's HTTPS launch profile:

**File:** `src/environments/environment.ts` — **Use:** Sets the API base URL.

<<< ../samples/Employeemanagmnet/src/environments/environment.ts{ts}

If the API uses another URL, change `apiUrl`. For local HTTPS, run `dotnet dev-certs https --trust` if the browser rejects the development certificate. The API CORS policy allows `http://localhost:4200`; change it if the UI origin changes.

## Routing and HTTP client

The routes send `/` to `/employees`, expose `/login` and `/register`, and guard employee routes:

**File:** `src/app/app.routes.ts` — **Use:** Maps URLs to screens and the guard.

<<< ../samples/Employeemanagmnet/src/app/app.routes.ts{ts}

`app.config.ts` registers the router and `HttpClient` with the auth interceptor:

**File:** `src/app/app.config.ts` — **Use:** Registers routing, HTTP, and the interceptor.

<<< ../samples/Employeemanagmnet/src/app/app.config.ts{ts}

**Next:** [Angular screens and services](angular-flow.md).
