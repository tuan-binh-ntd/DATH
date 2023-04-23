import { Gender } from "../enums/gender.enum";

export interface Customer {
  id: number | null,
  username: string | null,
  firstName: string | null,
  lastName: string | null,
  code: string | null,
  gender: Gender | null,
  birthday: Date | null,
  email: string | null,
  address: string | null
  isActive: boolean | null,
}
