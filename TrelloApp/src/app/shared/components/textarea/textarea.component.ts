import {Component, forwardRef, Input} from '@angular/core';
import {NgClass, NgIf} from '@angular/common';
import {ControlValueAccessor, NG_VALUE_ACCESSOR} from '@angular/forms';

@Component({
  selector: 'app-textarea',
  imports: [
    NgClass,
    NgIf
  ],
  templateUrl: './textarea.component.html',
  styleUrl: './textarea.component.css',
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => TextareaComponent),
      multi: true
    }
  ]
})
export class TextareaComponent implements ControlValueAccessor {
  @Input() id: string = `textarea-${Math.random().toString(36).substr(2, 9)}`;
  @Input() textLabel: string = '';
  @Input() placeholder: string = '';

  value: string = '';
  disabled = false;

  onChange = (value: string) => {};
  onTouched = () => {};

  writeValue(value: string): void {
    this.value = value || '';
  }

  registerOnChange(fn: any): void {
    this.onChange = fn;
  }

  registerOnTouched(fn: any): void {
    this.onTouched = fn;
  }

  setDisabledState(isDisabled: boolean): void {
    this.disabled = isDisabled;
  }

  updateValue(event: Event) {
    const textareaElement = event.target as HTMLTextAreaElement;
    this.value = textareaElement.value;
    this.onChange(this.value);
  }
}
