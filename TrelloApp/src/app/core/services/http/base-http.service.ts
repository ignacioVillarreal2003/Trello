import { HttpClient, HttpErrorResponse, HttpHeaders } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';

export abstract class BaseHttpService {
  protected http: HttpClient;
  protected baseUrl: string = 'http://localhost:5182';
  protected httpOptions = { headers: new HttpHeaders({ 'Content-Type': 'application/json' }) };

  constructor(http: HttpClient) {
    this.http = http;
  }

  protected handleError(error: HttpErrorResponse): Observable<never> {
    console.error('API Error:', error);

    let errorMessage = 'An unexpected error occurred. Please try again later.';

    if (error.error instanceof ErrorEvent) {
      errorMessage = `Client Error: ${error.error.message}`;
    } else {
      switch (error.status) {
        case 0:
          errorMessage = 'No connection to the server.';
          break;
        case 400:
          errorMessage = error.error?.message || 'Invalid request. Please check your input.';
          break;
        case 401:
          errorMessage = 'Unauthorized. Check your credentials.';
          break;
        case 404:
          errorMessage = 'Resource not found.';
          break;
        case 500:
          errorMessage = 'Server error. Try again later.';
          break;
      }
    }

    return throwError(() => new Error(errorMessage));
  }
}
