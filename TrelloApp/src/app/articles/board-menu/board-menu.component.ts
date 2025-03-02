import {Component, EventEmitter, Input, Output} from '@angular/core';
import {NgForOf, NgIf, NgStyle} from '@angular/common';
import {AlertService} from '../../core/services/alert.service';
import {BoardHttpService} from '../../core/services/http/board-http.service';
import {FormControl, FormGroup, Validators} from '@angular/forms';
import {BoardLabelHttpService} from '../../core/services/http/board-label-http.service';
import {ResourcesService} from '../../core/services/resources.service';
import {CommunicationService} from '../../core/services/communication.service';
import {BtnComponent} from '../../shared/btn/btn.component';
import {BtnBackComponent} from '../../shared/btn-back/btn-back.component';
import {BtnCloseComponent} from '../../shared/btn-close/btn-close.component';

@Component({
  selector: 'app-board-menu',
  imports: [
    NgIf,
    BtnComponent,
    BtnBackComponent,
    BtnCloseComponent
  ],
  templateUrl: './board-menu.component.html',
  standalone: true,
  styleUrl: './board-menu.component.css'
})
export class BoardMenuComponent {
  @Input() isOpen = false;
  @Output() public isClose = new EventEmitter<void>();

  onClose() {
    this.isOpen = false;
    this.isClose.emit();
  }

  mode: 'menu' | 'background' | 'label' | 'friend' = 'menu';
  backgrounds: string[] = [];
  colors: string[] = [];
  @Input() boardId: number | undefined = undefined;

  constructor(private resourcesService: ResourcesService,
              private alertService: AlertService,
              private boardHttpService: BoardHttpService,
              private boardLabelHttpService: BoardLabelHttpService,
              private communicationService: CommunicationService) {}

  ngOnInit(): void {
    this.backgrounds = this.resourcesService.backgrounds;
    this.colors = this.resourcesService.colors;
  }

  themeBoardForm: FormGroup = new FormGroup({
    theme: new FormControl('', [Validators.required])
  });

  selectBtnBoardBackground(btn: string): void {
    const buttons: NodeListOf<HTMLElement> = document.querySelectorAll('.btn-board-background') as NodeListOf<HTMLElement>;
    buttons.forEach((button: HTMLElement): void => {
      button.classList.remove('btn-board-background-selected');
    })
    const selectedButton: HTMLElement = document .querySelector(".board-background-list.ts ." + btn) as HTMLElement;
    if (selectedButton) {
      selectedButton.classList.add('btn-board-background-selected');
    }
    this.themeBoardForm.patchValue({ theme: btn });
  }

  onSubmitThemeBoard(): void {
    if (this.themeBoardForm.valid && this.boardId) {
      this.boardHttpService.putBoard(this.themeBoardForm.value.theme, this.boardId).subscribe(
        (response: any): void => {
          this.alertService.SuccessMessage('Successfully created team.');
          this.communicationService.triggerRefreshBoards();
        },
        (error: any): void => {
          this.alertService.ErrorMessage('Error in the server.')
        }
      );
    } else {
      this.alertService.ErrorMessage('Please verify the data entered.')
    }
  }

  createLabelForm: FormGroup = new FormGroup({
    title: new FormControl('', [Validators.required]),
    color: new FormControl('', [Validators.required]),
  });

  selectBtnLabel(btn: string): void {
    const buttons: NodeListOf<HTMLElement> = document.querySelectorAll('.btn-label') as NodeListOf<HTMLElement>;
    buttons.forEach((button: HTMLElement): void => {
      button.classList.remove('btn-label-selected');
    })
    const selectedButton: HTMLElement = document .querySelector(".label-list.ts ." + btn) as HTMLElement;
    if (selectedButton) {
      selectedButton.classList.add('btn-label-selected');
    }
    this.createLabelForm.patchValue({ color: btn });
  }

  onSubmitLabelBoard(): void {
    if (this.createLabelForm.valid && this.boardId) {
      this.boardLabelHttpService.PostBoardLabel(this.createLabelForm.value.title, this.createLabelForm.value.color, this.boardId).subscribe(
        (response: any): void => {
          this.alertService.SuccessMessage('Successfully created team.');
          this.communicationService.triggerRefreshBoardLabels();
        },
        (error: any): void => {
          this.alertService.ErrorMessage('Error in the server.')
        }
      );
    } else {
      this.alertService.ErrorMessage('Please verify the data entered.')
    }
  }
}
