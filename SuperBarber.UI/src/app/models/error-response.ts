export interface ErrorResponse {
  errorMessage: string;
  statusCode: number;
  errorCode: number;
  errorsMessages?: string[];
}
