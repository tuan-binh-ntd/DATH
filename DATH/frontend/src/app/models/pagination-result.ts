export interface PaginationResult<TResult> {
  content: Iterable<TResult>;
  totalCount: number;
  totalPage: number;
}
