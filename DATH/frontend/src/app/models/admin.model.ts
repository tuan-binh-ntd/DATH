import { Gender } from "../enums/gender.enum";
import { UserType } from "../shared/helper";

export interface Admin {
  id: number | null,
  username: string | null,
  firstName: string | null,
  lastName: string | null,
  code: string | null,
  gender: Gender | null,
  birthday: Date | null,
  email: string | null,
  address: string | null
  phone: string | null
  isActive: boolean | null,
  type: UserType
}
