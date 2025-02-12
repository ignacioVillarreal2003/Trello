import {Component, EventEmitter, Input, Output} from '@angular/core';
import {AlertService} from '../../core/services/alert.service';
import {TaskHttpService} from '../../core/services/http/task-http.service';
import {FormControl, FormGroup, ReactiveFormsModule, Validators} from '@angular/forms';
import {NgForOf, NgIf, NgStyle} from '@angular/common';
import {CommunicationService} from '../../core/services/communication.service';
import {ResourcesService} from '../../core/services/resources.service';

@Component({
  selector: 'app-task-view',
  imports: [
    ReactiveFormsModule,
    NgIf,
    NgForOf,
    NgStyle
  ],
  templateUrl: './task-view.component.html',
  standalone: true,
  styleUrl: './task-view.component.css'
})
export class TaskViewComponent {
  listId: number | undefined = undefined;
  mode: 'details' | 'comments' | 'labels' | 'assignations' = 'details'

  constructor(private alertService: AlertService,
              private taskHttpService: TaskHttpService,
              private communicationService: CommunicationService,
              private resourcesService: ResourcesService) {}

  updateTaskForm: FormGroup = new FormGroup({
    title: new FormControl('', [Validators.required]),
    description: new FormControl('', [Validators.required])
  });

  onSubmit(): void {

  }

    protected readonly Date = Date;

  colors: string[] = [];

  ngOnInit(): void {
    this.colors = this.resourcesService.colors;
  }
}
