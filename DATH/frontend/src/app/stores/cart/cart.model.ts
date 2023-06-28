import { Installment } from "src/app/models/installment.model";

export interface Cart {
  id?: string | null,
  name: string | null,
  photo: string | null,
  specifications: any[],
  installment: Installment | null,
  installmentId: number | null,
  paymentId: number | null,
  cost: number | null,
  quantity: number | null,
  productId: string,
}
