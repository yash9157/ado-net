# 6. Angular screens and services

The UI calls the API through two services, stores the JWT after login, and adds it to later requests. The guard prevents client-side navigation without an unexpired token; server authorization must still be enforced separately.

## Registration and login

`AuthService` posts credentials, stores the response in `localStorage`, and clears it on logout:

**File:** `src/app/services/auth.service.ts` — **Use:** Calls register/login and stores the session.

<<< ../samples/Employeemanagmnet/src/app/services/auth.service.ts{ts}

The auth request and response types are:

**File:** `src/app/models/register-user.model.ts` — **Use:** Types a registration request.

<<< ../samples/Employeemanagmnet/src/app/models/register-user.model.ts{ts}

**File:** `src/app/models/login.model.ts` — **Use:** Types login credentials.

<<< ../samples/Employeemanagmnet/src/app/models/login.model.ts{ts}

**File:** `src/app/models/auth-response.model.ts` — **Use:** Types the login response.

<<< ../samples/Employeemanagmnet/src/app/models/auth-response.model.ts{ts}

The forms validate email, passwords, and confirmation, then call the service:

**File:** `src/app/components/register/register.component.ts` — **Use:** Validates and submits registration.

<<< ../samples/Employeemanagmnet/src/app/components/register/register.component.ts{ts}

**File:** `src/app/components/register/register.component.html` — **Use:** Displays the registration form.

<<< ../samples/Employeemanagmnet/src/app/components/register/register.component.html{html}

**File:** `src/app/components/register/register.component.css` — **Use:** Styles the registration form.

<<< ../samples/Employeemanagmnet/src/app/components/register/register.component.css{css}

**File:** `src/app/components/login/login.component.ts` — **Use:** Validates and submits login.

<<< ../samples/Employeemanagmnet/src/app/components/login/login.component.ts{ts}

**File:** `src/app/components/login/login.component.html` — **Use:** Displays the login form.

<<< ../samples/Employeemanagmnet/src/app/components/login/login.component.html{html}

**File:** `src/app/components/login/login.component.css` — **Use:** Styles the login form.

<<< ../samples/Employeemanagmnet/src/app/components/login/login.component.css{css}

## Guard and interceptor

**File:** `src/app/guards/auth.guard.ts` — **Use:** Redirects visitors without a valid session.

<<< ../samples/Employeemanagmnet/src/app/guards/auth.guard.ts{ts}

**File:** `src/app/interceptors/auth.interceptor.ts` — **Use:** Adds the JWT to HTTP requests.

<<< ../samples/Employeemanagmnet/src/app/interceptors/auth.interceptor.ts{ts}

The interceptor adds `Authorization: Bearer <token>`. It does not validate the token. Keeping tokens in `localStorage` also carries XSS risk; this is a learning example.

## List and edit employees

`EmployeeService` maps each UI operation to the API route:

**File:** `src/app/services/employee.service.ts` — **Use:** Calls employee and department endpoints.

<<< ../samples/Employeemanagmnet/src/app/services/employee.service.ts{ts}

The TypeScript models describe the JSON exchanged with the API:

**File:** `src/app/models/employee.model.ts` — **Use:** Types employee records.

<<< ../samples/Employeemanagmnet/src/app/models/employee.model.ts{ts}

**File:** `src/app/models/create-employee.model.ts` — **Use:** Types a create request.

<<< ../samples/Employeemanagmnet/src/app/models/create-employee.model.ts{ts}

**File:** `src/app/models/update-employee.model.ts` — **Use:** Types a partial update.

<<< ../samples/Employeemanagmnet/src/app/models/update-employee.model.ts{ts}

**File:** `src/app/models/department.model.ts` — **Use:** Types department records.

<<< ../samples/Employeemanagmnet/src/app/models/department.model.ts{ts}

The list loads records, navigates to add/edit, confirms delete, and logs out. The form loads departments, validates fields, and sends changed fields on `PATCH`:

**File:** `src/app/components/employee-list/employee-list.component.ts` — **Use:** Loads, deletes, and navigates employees.

<<< ../samples/Employeemanagmnet/src/app/components/employee-list/employee-list.component.ts{ts}

**File:** `src/app/components/employee-list/employee-list.component.html` — **Use:** Displays the employee table and actions.

<<< ../samples/Employeemanagmnet/src/app/components/employee-list/employee-list.component.html{html}

**File:** `src/app/components/employee-list/employee-list.component.css` — **Use:** Styles the employee list.

<<< ../samples/Employeemanagmnet/src/app/components/employee-list/employee-list.component.css{css}

**File:** `src/app/components/employee-form/employee-form.component.ts` — **Use:** Loads and submits add/edit forms.

<<< ../samples/Employeemanagmnet/src/app/components/employee-form/employee-form.component.ts{ts}

**File:** `src/app/components/employee-form/employee-form.component.html` — **Use:** Displays the add/edit form.

<<< ../samples/Employeemanagmnet/src/app/components/employee-form/employee-form.component.html{html}

**File:** `src/app/components/employee-form/employee-form.component.css` — **Use:** Styles the employee form.

<<< ../samples/Employeemanagmnet/src/app/components/employee-form/employee-form.component.css{css}

The department selector is empty until you insert a department in [step 2](database.md). The API's `COALESCE` update procedure cannot clear an existing nullable field by sending `null`.

**Next:** [Run and test the complete system](run-and-test.md).
