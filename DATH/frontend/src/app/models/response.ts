import { PaginationResult } from "./pagination-result";

export interface ResponseResult<TResult> {
  statusCode: number | null;
  data: PaginationResult<TResult> | Iterable<TResult> | TResult | any |null;
  message: string | null;
}
