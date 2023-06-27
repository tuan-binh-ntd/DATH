import { Gender } from "../enums/gender.enum";
import { OrderStatus } from "../enums/order-status.enum";
import { EmployeeType } from "../shared/helper";
import { Cart } from "../stores/cart/cart.model";

export interface Order {
  id: string,
  customerName: string
  address: string
  code: string
  status: OrderStatus
  phone: string
  email: string
  estimateDate: string | null
  createDate: string | null
  actualDate: string | null
  cost: number
  discount: number | null
  promotionId: number | null
  orderDetailInputs: Cart[]
  shopName: string | null
}

