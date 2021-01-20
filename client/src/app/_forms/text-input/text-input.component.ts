import { Component, Input, OnInit, Self } from '@angular/core';
import { ControlValueAccessor, NgControl } from '@angular/forms';

@Component({
  selector: 'app-text-input',
  templateUrl: './text-input.component.html',
  styleUrls: ['./text-input.component.css']
})
export class TextInputComponent implements ControlValueAccessor { // The control value accessor defines an interface that acts as a bridge between the angular forms api and the native elemetn in the DOM. This allows us to create our own text input element and use the form controls we have have on our register form component rather than having to repeat the code for the validation in the HTML over and over. the native form control within the register component html file is the formControlName 

  @Input() label: string;
  @Input() type = 'text';

  constructor(@Self() public ngControl: NgControl) { // When angular looks at dependancy injection, it's going to look inside the heirachy of things it can inject. If there's an injector that matches this that it's already got inside it's dependancy injection container. This decorator ensures that Angular will always inject what we are doing here locally into this component. This stops angular trying to get this nControl from somewhere else in the dependancy tree
    this.ngControl.valueAccessor = this;
   } 
  writeValue(obj: any): void { // We don't need to enter anything i these methods. The functions are going to be created by the control value accessor itself. 
  }
  registerOnChange(fn: any): void {
  }
  registerOnTouched(fn: any): void {
  }


 

}
