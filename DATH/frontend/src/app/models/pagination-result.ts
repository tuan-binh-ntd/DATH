export interface PaginationResult<TArray> {
  content: Array<TArray>;
  totalCount: number;
  totalPage: number;
}
