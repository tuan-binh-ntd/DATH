import { Gender } from "../enums/gender.enum";

export interface Employee {
    id: string | null,
     firstName: string | null,
     lastName: string | null,
     code: string | null,
     gender: Gender | null,
     birthday: Date | null,
     type: Employee | null,
     email: string | null,
     joinDate: Date | null,
     isActive: boolean | null,
}