import { Component, Input, OnInit, Output, EventEmitter, resolveForwardRef } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { AccountService } from '../_services/account.service';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {

  model: any = {};
 
  @Output() cancelRegister = new EventEmitter();
 
  constructor(private accountService: AccountService, private toastr: ToastrService) { }

  ngOnInit(): void {
  }

  register() {
    this.accountService.register(this.model).subscribe(response => {
      this.cancel();
    }, error => {
      console.log(error.error);
      this.toastr.error(error.error); // This allows us to get the error message from the http response
    });
  }

  cancel() {
    this.cancelRegister.emit(false);
  }
}
