import { Photo } from "./photo.model";

export interface Product {
  id: string | null,
  name: string | null,
  price: number | null,
  description: string | null,
  productCategoryId: number | null,
  specificationId: string | null
  //photos: Photo[] | null,
}
