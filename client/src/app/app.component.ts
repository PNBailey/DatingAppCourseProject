import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { User } from './Models/user';
import { AccountService } from './_services/account.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  title = 'Dating app';
  users: any;

  constructor(private http: HttpClient, private accountService: AccountService) {}

  ngOnInit() {
   this.getUsers(); // We call the getusers method when this component is created
   this.setCurrentUser();
  }

  setCurrentUser() {
    const user: User = JSON.parse(localStorage.getItem('user'));
    this.accountService.setCurrentUser(user);
  }

  getUsers() {
    this.http.get('https://localhost:5001/api/users') // We use the end point from our users controller class within c#
  
    .subscribe(response => { // We have to subscribe to the observable for the get request to work. the first argument of the subscribe method tells the subscription what to do next. The second argument is telliong the subscription what to do if an error occurs and the third argument tells the subscription what to do once it has completed       

        this.users = response;
    }, error => {
      console.log(error);
    }) 
  }
  

}


