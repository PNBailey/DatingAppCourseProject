import { Component, OnInit } from '@angular/core';
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
  currentUser$: Observable<User>;

  constructor(private accountService: AccountService) { }

  ngOnInit() {
this.currentUser$ = this.accountService.currentUser$; // We get the observable from our account service. The observable in the account service is a replaysubject which stores the value of the user that is logged in (if there is a user logged in). Getting the observable like this allows us to use the async pipe in our html code in the header html file. 
  }

  login() {
    this.accountService.login(this.model)
    
    .subscribe(response => {
      console.log(response);
    }, error => {
      console.log(error);
    })
  }

  logout() {
    this.accountService.logout();
   
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
