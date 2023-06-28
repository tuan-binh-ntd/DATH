import { Gender } from "../enums/gender.enum";
import { EmployeeType, UserType } from "../shared/helper";

export interface Account {
  username: string | null,
  firstName: string | null,
  lastName: string | null,
  gender: Gender | null,
  address: string | null,
  idNumber: string | null,
  phone: string | null,
  birthday: Date | null,
  email: string | null,
  isActive: boolean | null,
  token: string | null,
  avatarUrl: string | null,
  code: string | null,
  joinDate: Date | null,
  employeeType: EmployeeType | null,
  type: UserType | null,
  shopId: number | null,
}
