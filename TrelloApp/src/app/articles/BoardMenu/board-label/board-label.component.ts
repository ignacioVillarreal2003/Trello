import {Component, Input} from '@angular/core';
import {InputComponent} from '../../../shared/components/input/input.component';
import {BtnComponent} from '../../../shared/components/btn/btn.component';
import {FormControl, FormGroup, FormsModule, ReactiveFormsModule, Validators} from '@angular/forms';
import {ResourcesService} from '../../../core/services/resources.service';
import {NgForOf} from '@angular/common';
import {Label} from '../../../core/models/label';
import {LabelHttpService} from '../../../core/services/http/label-http.service';
import {Board} from '../../../core/models/board';
import {AlertService} from '../../../core/services/alert.service';
import {BtnDeleteComponent} from '../../../shared/components/btn-delete/btn-delete.component';

@Component({
  selector: 'app-board-label',
  imports: [
    InputComponent,
    BtnComponent,
    ReactiveFormsModule,
    FormsModule,
    NgForOf,
    BtnDeleteComponent
  ],
  templateUrl: './board-label.component.html',
  styleUrl: './board-label.component.css'
})
export class BoardLabelComponent {
  @Input() board: Board | undefined = undefined;
  selectedColor: number = 0;
  labelColors: string[] = [];
  labels: Label[] = [];
  title: string = "";

  constructor(private resourcesService: ResourcesService,
              private labelHttpService: LabelHttpService,
              private alertService: AlertService) { }

  ngOnInit() {
    this.labelColors = this.resourcesService.labelColors;
    this.getLabels();
    this.createLabelForm.valueChanges.subscribe(values => {
      this.title = values.title;
    });
  }

  selectColor(index: number): void {
    this.selectedColor = index;
    this.createLabelForm.patchValue({ color: this.labelColors[index] });
  }

  createLabelForm: FormGroup = new FormGroup({
    title: new FormControl('', [Validators.required]),
    color: new FormControl('', [Validators.required])
  });

  onSubmitCreateLabel(): void {
    if (this.createLabelForm.invalid) {
      if (this.createLabelForm.controls['title'].errors) {
        this.alertService.ErrorMessage('Invalid title.');
      } else if (this.createLabelForm.controls['color'].errors) {
        this.alertService.ErrorMessage('Invalid color.');
      }
    } else {
      if (this.board) {
        this.labelHttpService.add(this.board.id, this.createLabelForm.value.title, this.createLabelForm.value.color).subscribe({
          next: (result: Label): void => {
            this.getLabels();
            this.alertService.SuccessMessage("Label created successfully.");
          },
          error: (error: Error): void => {
            this.alertService.ErrorMessage(error.message)
          }
        })
      } else {
        this.alertService.ErrorMessage("Error creating label.");
      }
    }
  }

  getLabels(): void {
    if (this.board) {
      this.labelHttpService.getLabelsByBoardId(this.board.id).subscribe({
        next: (result: Label[]) => {
          this.labels = result;
        },
        error: (error: Error) => {
          this.alertService.ErrorMessage(error.message)
        }
      })
    }
  }

  deleteLabel(id: number) {
    this.labelHttpService.delete(id).subscribe({
      next: (result: any) => {
        this.labels = this.labels.filter(l => l.id !== id);
      },
      error: (error: Error) => {
        this.alertService.ErrorMessage(error.message)
      }
    })
  }
}
