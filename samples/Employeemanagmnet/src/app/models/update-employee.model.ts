export interface UpdateEmployee {
  firstName?: string;
  lastName?: string;
  email?: string;
  phone?: string | null;
  salary?: number | null;
  dateOfBirth?: string | null;
  joiningDate?: string | null;
  gender?: string | null;
  isActive?: boolean;
  departmentId?: number;
}
