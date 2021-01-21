import { Component, Input, OnInit, Output, EventEmitter, resolveForwardRef } from '@angular/core';
import { AbstractControl, FormBuilder, FormControl, FormGroup, ValidatorFn, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { AccountService } from '../_services/account.service';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {

  @Output() cancelRegister = new EventEmitter();
  registerForm: FormGroup; // This is for the reactive forms 
  maxDate: Date;
  validationErrors: string[] = [];

 
  constructor(private accountService: AccountService, private toastr: ToastrService, private fb: FormBuilder, private router: Router) { }

  ngOnInit(): void {
    this.initializeForm();
    this.maxDate = new Date();
    this.maxDate.setFullYear(this.maxDate.getFullYear() - 18); // This is th property we pass into the app-date-input component selector in the register html file. This means that only over 18's can use the app
  }

  // instead of using the new FormGroup below, we use the built in Angular form builder
  initializeForm() {
    this.registerForm = this.fb.group({
      gender: ['male'], // The first parameter here is the starting value. The next parameter is our validation options. This form control will be a radio button
      username: ['', Validators.required], 
      knownAs: ['', Validators.required],
      dateOfBirth: ['', Validators.required],
      city: ['', Validators.required],
      country: ['', Validators.required],
      password: ['', [Validators.required, Validators.minLength(4), Validators.maxLength(8)]], // If we need more than 1 validator we create an array of validators
      confirmPassword: ['', [Validators.required, this.matchValues('password')]]
    })
  }

  // initializeForm() { // This is for the reactive form approach
  //   this.registerForm = new FormGroup({
  //     username: new FormControl('', Validators.required), // The first parameter here is the starting value. The next parameter is our validation options 
  //     password: new FormControl('', [Validators.required, Validators.minLength(4), Validators.maxLength(8)]), // If we need more than 1 validators we put them inside squared brackets
  //     confirmPassword: new FormControl('', [Validators.required, this.matchValues('password')])
  //   })
  // }

  matchValues(matchTo: any): ValidatorFn { // Here we create a custom validator so we can ensure that what the user entrers in our confirmPassword field matches what is in the password field 
      return (control: AbstractControl) => {
        return control?.value === control?.parent?.controls[matchTo].value ? null : {notMatching: true} // Using this syntax gives us access to all the controls in the form as we are traversing through the form controls. This gives us access to the control that we are going to add this custom validator to (our confirmPassword control). o we are comparing the controlPassword field to the password field to ensure they match. The matchTo value is the password we want to compare this value to. If the passwords do match, we return null and therefore, the validation passes. If the passwords do not match then we attach a validator error called notMatching to the control and this will fail our form validation  
      }
  }

  register() {
    this.accountService.register(this.registerForm.value).subscribe(response => {
      this.router.navigateByUrl('/members');
    }, error => {
     // this.toastr.error(error.error); // This allows us to get the error message from the http response
     // If we get an error from the form validation, we are going to get thrown the error from our interceptor
     this.validationErrors = error;
    });
  }

  cancel() {
    this.cancelRegister.emit(false);
  }
}
