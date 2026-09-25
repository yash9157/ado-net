import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { EmployeeService } from '../../services/employee.service';
import { AuthService } from '../../services/auth.service';
import { timeout } from 'rxjs';
import { finalize } from 'rxjs/operators';
import { Employee } from '../../models/employee.model';

@Component({
  selector: 'app-employee-list',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './employee-list.component.html',
  styleUrls: ['./employee-list.component.css']
})
export class EmployeeListComponent implements OnInit {
  employees: Employee[] = [];
  loading = false;
  error: string | null = null;

  constructor(private service: EmployeeService, private router: Router, private auth: AuthService, private cdr: ChangeDetectorRef) {}

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    this.loading = true;
    this.error = null;
    console.log('Loading employees...');
    this.service
      .getEmployees()
      .pipe(
        timeout(10000),
        finalize(() => {
          this.loading = false;
          console.log('Employees loading finished, loading set to', this.loading);
          // ensure view updates
          try {
            this.cdr.detectChanges();
          } catch {}
        })
      )
      .subscribe({
        next: (res) => {
          console.log('Employees response received', res);
          this.employees = res || [];
        },
        error: (err: any) => {
          console.error('Employees loading error', err);
          if (err && err.name === 'TimeoutError') {
            this.error = 'Request timed out. Please ensure the API is running.';
          } else if (err && err.status === 0) {
            // likely network or CORS error
            this.error = 'Network error or CORS issue. Check API server and CORS settings.';
          } else {
            this.error = 'Failed to load employees.';
          }
        }
      });
  }

  add(): void {
    this.router.navigate(['/employees/add']);
  }

  edit(id: number): void {
    this.router.navigate([`/employees/edit/${id}`]);
  }

  delete(id: number): void {
    if (!confirm('Are you sure you want to delete this employee?')) {
      return;
    }

    this.service.deleteEmployee(id).subscribe({
      next: () => this.load(),
      error: (err) => {
        console.error(err);
        this.error = 'Failed to delete employee.';
      }
    });
  }

  logout(): void {
    this.auth.logout();
    this.router.navigate(['/login']);
  }
}
