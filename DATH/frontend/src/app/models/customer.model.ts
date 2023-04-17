import { Gender } from "../enums/gender.enum";

export interface Customer {
    id: string | null,
     firstName: string | null,
     lastName: string | null,
     code: string | null,
     gender: Gender | null,
     birthday: Date | null,
     email: string | null,
     joinDate: Date | null,
     isActive: boolean | null,
}