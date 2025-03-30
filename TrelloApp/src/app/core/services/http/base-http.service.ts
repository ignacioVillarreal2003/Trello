import { HttpClient, HttpErrorResponse, HttpHeaders } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';

export abstract class BaseHttpService {
  protected http: HttpClient;
  protected baseUrl: string = "https://trellobackend-d7d3h2dxhwecf7az.brazilsouth-01.azurewebsites.net";
  protected httpOptions = {
    headers: new HttpHeaders({ 'Content-Type': 'application/json' }),
    withCredentials: true
  };

  constructor(http: HttpClient) {
    this.http = http;
  }

  protected handleError(error: HttpErrorResponse): Observable<never> {
    console.error('API Error:', error);

    let errorMessage = 'An unexpected error occurred. Please try again later.';

    if (error.error instanceof ErrorEvent) {
      errorMessage = `Client Error: ${error.error.message}`;
    }

    return throwError(() => new Error(errorMessage));
  }
}
