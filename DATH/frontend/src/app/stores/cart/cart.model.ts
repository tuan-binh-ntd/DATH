
export interface Cart {
  id?: string | null,
  name: string | null,
  photo: string | null,
  specifications: any[],
  cost: number | null,
  quantity: number | null,
  productId: string,
}
