# 6. Angular screens and services

The UI calls the API through two services, stores the JWT after login, and adds it to later requests. The guard prevents client-side navigation without an unexpired token; server authorization must still be enforced separately.

## Registration and login

`AuthService` posts credentials, stores the response in `localStorage`, and clears it on logout:

<<< ../samples/Employeemanagmnet/src/app/services/auth.service.ts{ts}

The auth request and response types are:

<<< ../samples/Employeemanagmnet/src/app/models/register-user.model.ts{ts}

<<< ../samples/Employeemanagmnet/src/app/models/login.model.ts{ts}

<<< ../samples/Employeemanagmnet/src/app/models/auth-response.model.ts{ts}

The forms validate email, passwords, and confirmation, then call the service:

<<< ../samples/Employeemanagmnet/src/app/components/register/register.component.ts{ts}

<<< ../samples/Employeemanagmnet/src/app/components/register/register.component.html{html}

<<< ../samples/Employeemanagmnet/src/app/components/register/register.component.css{css}

<<< ../samples/Employeemanagmnet/src/app/components/login/login.component.ts{ts}

<<< ../samples/Employeemanagmnet/src/app/components/login/login.component.html{html}

<<< ../samples/Employeemanagmnet/src/app/components/login/login.component.css{css}

## Guard and interceptor

<<< ../samples/Employeemanagmnet/src/app/guards/auth.guard.ts{ts}

<<< ../samples/Employeemanagmnet/src/app/interceptors/auth.interceptor.ts{ts}

The interceptor adds `Authorization: Bearer <token>`. It does not validate the token. Keeping tokens in `localStorage` also carries XSS risk; this is a learning example.

## List and edit employees

`EmployeeService` maps each UI operation to the API route:

<<< ../samples/Employeemanagmnet/src/app/services/employee.service.ts{ts}

The TypeScript models describe the JSON exchanged with the API:

<<< ../samples/Employeemanagmnet/src/app/models/employee.model.ts{ts}

<<< ../samples/Employeemanagmnet/src/app/models/create-employee.model.ts{ts}

<<< ../samples/Employeemanagmnet/src/app/models/update-employee.model.ts{ts}

<<< ../samples/Employeemanagmnet/src/app/models/department.model.ts{ts}

The list loads records, navigates to add/edit, confirms delete, and logs out. The form loads departments, validates fields, and sends changed fields on `PATCH`:

<<< ../samples/Employeemanagmnet/src/app/components/employee-list/employee-list.component.ts{ts}

<<< ../samples/Employeemanagmnet/src/app/components/employee-list/employee-list.component.html{html}

<<< ../samples/Employeemanagmnet/src/app/components/employee-list/employee-list.component.css{css}

<<< ../samples/Employeemanagmnet/src/app/components/employee-form/employee-form.component.ts{ts}

<<< ../samples/Employeemanagmnet/src/app/components/employee-form/employee-form.component.html{html}

<<< ../samples/Employeemanagmnet/src/app/components/employee-form/employee-form.component.css{css}

The department selector is empty until you insert a department in [step 2](database.md). The API's `COALESCE` update procedure cannot clear an existing nullable field by sending `null`.

**Next:** [Run and test the complete system](run-and-test.md).
