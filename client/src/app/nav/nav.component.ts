import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { Observable } from 'rxjs';
import { User } from '../Models/user';
import { AccountService } from '../_services/account.service';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent implements OnInit {

  model: any = {};
  // currentUser$: Observable<User>; // We do't need this property as we access the accountservice straight from our html template 

  constructor(public accountService: AccountService, private router: Router, private toastr: ToastrService) { } // By making the account service public here, we can access it in our html template of this component. This means we don't have to import the currentUser$ object as we did below and we can access it sraight from the account service in our html template. 

  // TOASTR Service. This is a package we can install 'npm install ngx-toastr' that we install in the client folder. This allows us to use notifications for our users 

  ngOnInit() {
// this.currentUser$ = this.accountService.currentUser$; // We get the observable from our account service. The observable in the account service is a replaysubject which stores the value of the user that is logged in (if there is a user logged in). Getting the observable like this allows us to use the async pipe in our html code in the header html file. 
  }

  login() {
    this.accountService.login(this.model)
    
    .subscribe(response => {
      this.router.navigateByUrl('/members');
    }, error => {
      console.log(error);
      this.toastr.error(error.error); // This allows us to get the error message from the http response
      
    })
  }

  logout() {
    this.accountService.logout();
    this.router.navigateByUrl('/');
  }

  // We can coment out the below code as we are getting the currently logged in user directly from our account service file (see NgOnInit method above)

  // getCurrentUser() { // We interogate our observable here
  //   this.accountService.currentUser$.subscribe(user => {
  //     // this.loggedIn = !!user; // This converts the object into a boolean. If the user is null = false. If the user is something = true
  //   }, error => {
  //     console.log(error);
  //   })
  // }

}
