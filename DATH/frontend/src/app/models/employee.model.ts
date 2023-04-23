import { EmployeeType } from "../enums/employee-type.enum";
import { Gender } from "../enums/gender.enum";

export interface Employee {
  id: number | null,
  username: string | null,
  shopName: string | null,
  name: string | null,
  code: string | null,
  gender: Gender | null,
  birthday: Date | null,
  type: EmployeeType | null,
  email: string | null,
  joinDate: Date | null,
  isActive: boolean | null,
}
