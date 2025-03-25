import {Component, Input} from '@angular/core';
import {NgIf} from "@angular/common";

@Component({
  selector: 'app-avatar',
    imports: [
        NgIf,
    ],
  templateUrl: './avatar.component.html',
  styleUrl: './avatar.component.css'
})
export class AvatarComponent {
  @Input() username: string | undefined = undefined;
  @Input() backgroundColor: string = "var(--color-blue-500)";
}
