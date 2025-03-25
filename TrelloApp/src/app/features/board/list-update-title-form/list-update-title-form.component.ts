import {Component, Input} from '@angular/core';
import {FormControl, FormGroup, FormsModule, ReactiveFormsModule, Validators} from "@angular/forms";
import {List, UpdateList} from '../../../core/models/list';
import {ListHttpService} from '../../../core/services/http/list-http.service';
import {ListCommunicationService} from '../../../core/services/communication/list-communication.service';

@Component({
  selector: 'app-list-update-title-form',
    imports: [
        FormsModule,
        ReactiveFormsModule
    ],
  templateUrl: './list-update-title-form.component.html',
  styleUrl: './list-update-title-form.component.css'
})
export class ListUpdateTitleFormComponent {
  @Input() list: List | undefined = undefined;

  formUpdateListTitle: FormGroup = new FormGroup({
    title: new FormControl('', [Validators.required, Validators.maxLength(64)]),
  });

  constructor(private listHttpService: ListHttpService,
              private listCommunicationService: ListCommunicationService) {
  }

  ngOnInit(): void {
    if (this.list) {
      this.formUpdateListTitle.patchValue({
        title: this.list.title
      });
    }
    this.listCommunicationService.updateList$.subscribe((l: List | null): void => {
      if (l != null && this.list != undefined && this.list?.id == l.id) {
        this.formUpdateListTitle.patchValue({
          title: l.title
        });
      }
    })
  }

  onSubmitUpdateListTitle(): void {
    if (this.formUpdateListTitle.invalid) {
      this.formUpdateListTitle.markAllAsTouched();
      return;
    }
    if (this.list) {
      const body: UpdateList = {
        title: this.formUpdateListTitle.value.title
      }

      this.listHttpService.update(this.list.id, body).subscribe();
    }
  }
}
