import {Component, Input} from '@angular/core';
import {BtnComponent} from "../../../shared/components/btn/btn.component";
import {FormControl, FormGroup, FormsModule, ReactiveFormsModule, Validators} from "@angular/forms";
import {InputComponent} from "../../../shared/components/input/input.component";
import {NgForOf} from "@angular/common";
import {Board} from '../../../core/models/board';
import {AddLabel, Label} from '../../../core/models/label';
import {ResourcesService} from '../../../core/services/resources.service';
import {LabelHttpService} from '../../../core/services/http/label-http.service';

@Component({
  selector: 'app-label-create-form',
    imports: [
        BtnComponent,
        FormsModule,
        InputComponent,
        NgForOf,
        ReactiveFormsModule
    ],
  templateUrl: './label-create-form.component.html',
  styleUrl: './label-create-form.component.css'
})
export class LabelCreateFormComponent {
  @Input() board: Board | undefined = undefined;
  selectedColor: number = 0;
  labelColors: string[] = [];
  title: string = "";

  formCreateLabel: FormGroup = new FormGroup({
    title: new FormControl('', [Validators.required, Validators.maxLength(32)]),
    color: new FormControl('', [Validators.required, Validators.maxLength(32)])
  });

  errorMessages: any = {
    title: {
      required: 'Title is required.',
      maxlength: 'The title must be less than 64 characters.'
    }
  };

  constructor(private resourcesService: ResourcesService,
              private labelHttpService: LabelHttpService) { }

  ngOnInit() {
    this.labelColors = this.resourcesService.labelColors;
    this.formCreateLabel.valueChanges.subscribe(values => {
      this.title = values.title;
    });
  }

  getErrorMessage(controlName: string): string {
    const control = this.formCreateLabel.get(controlName);
    if (control?.errors) {
      for (const error in control.errors) {
        if (this.errorMessages[controlName][error]) {
          return this.errorMessages[controlName][error];
        }
      }
    }
    return '';
  }

  selectColor(index: number): void {
    this.selectedColor = index;
    this.formCreateLabel.patchValue({ color: this.labelColors[index] });
  }

  onSubmitCreateLabel(): void {
    if (this.formCreateLabel.invalid) {
      this.formCreateLabel.markAllAsTouched();
      return;
    }

    if (this.board) {
      const body: AddLabel = {
        title: this.formCreateLabel.value.title,
        color: this.formCreateLabel.value.color,
      };

      this.labelHttpService.add(this.board.id, body).subscribe({
        next: (): void => {
          this.formCreateLabel.patchValue({
            color: this.labelColors[0],
            title: "",
          });
        }
      })
    }
  }
}
