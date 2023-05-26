import { Gender } from "../enums/gender.enum";
import { EmployeeType } from "../shared/helper";
import { Cart } from "../stores/cart/cart.model";

export interface Order {
  customerName: string
  address: string
  code: string
  status: number
  phone: string
  estimateDate: string | null
  actualDate: string | null
  cost: number
  discount: number | null
  promotionId: number | null
  orderDetailInputs: Cart[]
}

