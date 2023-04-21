import { Gender } from "../enums/gender.enum";

export interface Register {
  username: string | null,
  password: string | null,
  firstName: string | null,
  lastName: string | null,
  gender: Gender | null,
  address: string | null,
  idNumber: string | null,
  phone: string | null,
  birthday: Date | null,
  email: string | null,
  isActive: boolean | null,
  shopId: number | null,
}
