import { ErrorResponse } from './error-response';

export interface ResponseContent<TResponse> {
  result: TResponse;
  error: ErrorResponse;
}
