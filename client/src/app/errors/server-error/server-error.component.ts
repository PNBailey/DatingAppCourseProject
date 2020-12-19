import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-server-error',
  templateUrl: './server-error.component.html',
  styleUrls: ['./server-error.component.css']
})
export class ServerErrorComponent implements OnInit {

  error: any;

  constructor(private router: Router) { 
    const navigation = this.router.getCurrentNavigation(); // We can only access the navigation state inside the constructor. We need this navvigation state as for the 500 error (see error interceptor), we need to have the navigation extras so that the details of the error can be dispayed in this compnents html
    this.error = navigation?.extras?.state?.error; // The ? is an optional chaning operators. We use this as there is a chance the error could be lost if the user refreshes the page as this does not persist

   }

  ngOnInit(): void {
  }

}
