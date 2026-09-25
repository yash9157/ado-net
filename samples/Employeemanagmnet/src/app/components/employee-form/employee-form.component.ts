import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, Validators, FormGroup } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { EmployeeService } from '../../services/employee.service';
import { Department } from '../../models/department.model';
import { Employee } from '../../models/employee.model';

@Component({
  selector: 'app-employee-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './employee-form.component.html',
  styleUrls: ['./employee-form.component.css']
})
export class EmployeeFormComponent implements OnInit {
  form!: FormGroup;

  departments: Department[] = [];
  loading = false;
  error: string | null = null;
  originalEmployee: Employee | null = null;
  isEdit = false;

  constructor(
    private fb: FormBuilder,
    private service: EmployeeService,
    private router: Router,
    private route: ActivatedRoute
  ) {
    // initialize form here after fb is available
    this.form = this.fb.group({
      firstName: ['', [Validators.required, Validators.minLength(2)]],
      lastName: ['', [Validators.required, Validators.minLength(2)]],
      email: ['', [Validators.required, Validators.email]],
      // phone: optional but if present must be 10 digits
      phone: ['', [Validators.pattern('^[0-9]{10}$')]],
      // salary must be >= 0 when provided
      salary: [null, [Validators.min(0)]],
      dateOfBirth: [''],
      joiningDate: [''],
      gender: [''],
      isActive: [true],
      departmentId: [null, [Validators.required]]
    });
  }

  ngOnInit(): void {
    this.loadDepartments();

    const idParam = this.route.snapshot.paramMap.get('id');
    if (idParam) {
      this.isEdit = true;
      const id = Number(idParam);
      this.loading = true;
      this.service.getEmployeeById(id).subscribe({
        next: (e) => {
          this.originalEmployee = e;
          this.patchForm(e);
          this.loading = false;
        },
        error: (err) => {
          console.error(err);
          this.error = 'Failed to load employee.';
          this.loading = false;
        }
      });
    }
  }

  private loadDepartments(): void {
    this.service.getDepartments().subscribe({
      next: (d) => (this.departments = d || []),
      error: (err) => console.error(err)
    });
  }

  private patchForm(e: Employee) {
    this.form.patchValue({
      firstName: e.firstName,
      lastName: e.lastName,
      email: e.email,
      phone: e.phone ?? '',
      salary: e.salary ?? null,
      dateOfBirth: e.dateOfBirth ? e.dateOfBirth.split('T')[0] : '',
      joiningDate: e.joiningDate ? e.joiningDate.split('T')[0] : '',
      gender: e.gender ?? '',
      isActive: e.isActive,
      departmentId: e.departmentId
    });
  }

  cancel(): void {
    this.router.navigate(['/employees']);
  }

  submit(): void {
    this.error = null;
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    // show loading indicator while submitting
    this.loading = true;

    const values = this.form.value;

    if (!this.isEdit) {
      const payload = {
        firstName: values.firstName,
        lastName: values.lastName,
        email: values.email,
        phone: values.phone || null,
        salary: values.salary ?? null,
        dateOfBirth: values.dateOfBirth || null,
        joiningDate: values.joiningDate || null,
        gender: values.gender || null,
        isActive: values.isActive,
        departmentId: Number(values.departmentId)
      };

      this.service.createEmployee(payload as any).subscribe({
        next: () => {
          this.loading = false;
          this.router.navigate(['/employees']);
        },
        error: (err) => {
          console.error(err);
          this.error = 'Failed to create employee.';
          this.loading = false;
        }
      });

      return;
    }

    // Edit mode - build patch with only changed fields
    if (!this.originalEmployee) {
      this.error = 'Original data missing.';
      return;
    }

    const update: any = {};
    const orig = this.originalEmployee;

    if (values.firstName !== orig.firstName) update.firstName = values.firstName;
    if (values.lastName !== orig.lastName) update.lastName = values.lastName;
    if (values.email !== orig.email) update.email = values.email;

    const phoneVal = values.phone || null;
    if (phoneVal !== (orig.phone ?? null)) update.phone = phoneVal;

    const salaryVal = values.salary === null || values.salary === undefined ? null : Number(values.salary);
    if (salaryVal !== (orig.salary ?? null)) update.salary = salaryVal;

    const dobVal = values.dateOfBirth ? values.dateOfBirth : null;
    const origDob = orig.dateOfBirth ? orig.dateOfBirth.split('T')[0] : null;
    if (dobVal !== origDob) update.dateOfBirth = dobVal;

    const joinVal = values.joiningDate ? values.joiningDate : null;
    const origJoin = orig.joiningDate ? orig.joiningDate.split('T')[0] : null;
    if (joinVal !== origJoin) update.joiningDate = joinVal;

    const genderVal = values.gender || null;
    if (genderVal !== (orig.gender ?? null)) update.gender = genderVal;

    if (values.isActive !== orig.isActive) update.isActive = values.isActive;

    const deptVal = values.departmentId === null || values.departmentId === undefined ? null : Number(values.departmentId);
    if (deptVal !== orig.departmentId) update.departmentId = Number(values.departmentId);

    if (Object.keys(update).length === 0) {
      // Nothing changed
      this.router.navigate(['/employees']);
      return;
    }

    this.service.updateEmployee(orig.employeeId, update).subscribe({
      next: () => {
        this.loading = false;
        this.router.navigate(['/employees']);
      },
      error: (err) => {
        console.error(err);
        this.error = 'Failed to update employee.';
        this.loading = false;
      }
    });
  }
}
