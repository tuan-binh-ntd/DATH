import { PaginationResult } from "./pagination-result";

export interface Response<TResult> {
  statusCode: number | null;
  data: PaginationResult<TResult> | TResult | any | null;
  message: string | null;
}
