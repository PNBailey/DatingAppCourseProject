import { OnInit } from '@angular/core';
import { Input, TemplateRef } from '@angular/core';
import { Directive, ViewContainerRef } from '@angular/core';
import { take } from 'rxjs/operators';
import { User } from '../Models/user';
import { AccountService } from '../_services/account.service';

@Directive({
  selector: '[appHasRole]' // When we use this directive in the html file, we will use the syntax *appHasRole='["Admin"]'. So when we use the directive, we need to specify which roles we want the html element to allow to show and this syntax: *appHasRole='["Admin"]'. Is the way we do that
})

// We create this directive to use in our nav html file. It is a structural directive which allows us to only display the 'admin' nav bar option when the user is an admin 

export class HasRoleDirective implements OnInit {
  @Input() appHasRole: string[]; // As we will be specifying the role we want to allow access to when we use the directive in the html file, we need to get access to the role so we use the @Input decorator
  user: User;

  constructor(private viewContainerRef: ViewContainerRef, private templateRef: TemplateRef<any>, private accountService: AccountService) {  
    this.accountService.currentUser$.pipe(take(1)).subscribe(user => {
      this.user = user;
    })
  }

  ngOnInit(): void {
     // clear view if no roles

     if(!this.user?.roles || this.user == null) { // This checks if the user does not have any roles or if there is no user 
       this.viewContainerRef.clear(); // If the above condition is true, we clear the container (the container is the html container that we are adding this directive to)
       return; // we then return the method
     }

     if(this.user?.roles.some(role => this.appHasRole.includes(role))) { // This checks whether the call back function returns true for any element in the array 
        this.viewContainerRef.createEmbeddedView(this.templateRef); // If the above condition is true, then we create the view using the html tag that we will be using this structural directive on
     } 
  }

}
