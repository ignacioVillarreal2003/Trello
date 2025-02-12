import { Injectable } from '@angular/core';
import Swal from 'sweetalert2'

@Injectable({
  providedIn: 'root'
})
export class AlertService {

  constructor() { }

  ErrorMessage(text: string): void {
    Swal.fire({
      title: text,
      icon: 'error',
      confirmButtonText: 'Cool',
      timer: 3000,
      timerProgressBar: true
    })
  }

  SuccessMessage(text: string){
    Swal.fire({
      title: text,
      icon: 'success',
      confirmButtonText: 'Cool',
      timer: 3000,
      timerProgressBar: true
    })
  }
}
