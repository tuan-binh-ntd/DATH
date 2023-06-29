import { Feedback } from "./feedback.model";
import { Photo } from "./photo.model";
import { Specification } from "./specification.model";

export interface Product {
  id: string | null,
  name: string | null,
  price: number | null,
  description: string | null,
  productCategoryId: number | null,
  specifications: Specification[] | null,
  specificationId: string | null
  photos: Photo[] | null,
  feedbacks: Feedback[] | null,
  star: number | null,

}
