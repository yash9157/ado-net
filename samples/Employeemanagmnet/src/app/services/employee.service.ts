import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { Employee } from '../models/employee.model';
import { CreateEmployee } from '../models/create-employee.model';
import { UpdateEmployee } from '../models/update-employee.model';
import { Department } from '../models/department.model';

@Injectable({
  providedIn: 'root'
})
export class EmployeeService {
  private api = environment.apiUrl;

  constructor(private http: HttpClient) {}

  getEmployees(): Observable<Employee[]> {
    // Backend controller is named 'Employee' (see Swagger) — use exact path
    return this.http.get<Employee[]>(`${this.api}/Employee`);
  }

  getEmployeeById(id: number): Observable<Employee> {
    return this.http.get<Employee>(`${this.api}/Employee/${id}`);
  }

  createEmployee(employee: CreateEmployee): Observable<string> {
    return this.http.post(`${this.api}/Employee`, employee, { responseType: 'text' }) as Observable<string>;
  }

  updateEmployee(id: number, employee: UpdateEmployee): Observable<string> {
    return this.http.patch(`${this.api}/Employee/${id}`, employee, { responseType: 'text' }) as Observable<string>;
  }

  deleteEmployee(id: number): Observable<string> {
    return this.http.delete(`${this.api}/Employee/${id}`, { responseType: 'text' }) as Observable<string>;
  }

  getDepartments(): Observable<Department[]> {
    // Backend controller is named 'Departments' (see Swagger)
    return this.http.get<Department[]>(`${this.api}/Departments`);
  }
}
