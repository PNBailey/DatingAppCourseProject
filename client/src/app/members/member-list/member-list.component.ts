import { Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs';
import { take } from 'rxjs/operators';
import { Member } from 'src/app/Models/member';
import { Pagination } from 'src/app/Models/pagination';
import { User } from 'src/app/Models/user';
import { UserParams } from 'src/app/Models/userParams';
import { AccountService } from 'src/app/_services/account.service';
import { MembersService } from 'src/app/_services/members.service';

@Component({
  selector: 'app-member-list',
  templateUrl: './member-list.component.html',
  styleUrls: ['./member-list.component.css']
})
export class MemberListComponent implements OnInit {
  members: Member[];
  pagination: Pagination;
  userParams: UserParams;
  user: User;
  genderList = [{value: 'male', display: 'Males'}, {value: 'female', display: 'Females'}]; // We create an array of objects which will be used in our html file of this component 

  constructor(private memberService: MembersService, private accountService: AccountService) { 
    this.accountService.currentUser$.pipe(take(1)).subscribe(user => {
      this.user = user;
      this.userParams = new UserParams(user);
    })
  }

  ngOnInit(): void {

    // this.members = this.memberService.getMembers(); // He we get the observable which returns a members array from our member service getMembers method 

    this.loadMembers();
  }

  loadMembers() {
    this.memberService.getMembers(this.userParams).subscribe(response => {
      this.members = response.result; // As we now include the full response with the http request, we have to access the result from the response
      this.pagination = response.pagination;
    })
  }

  resetFilters() { // This ill reset our filters and will reload our members based on our default userParams (the default userParam values are set in our UserParams class in our API)
    this.userParams = new UserParams(this.user);
    this.loadMembers();
  }

  pageChanged(event: any){
     this.userParams.pageNumber = event.page;
     this.loadMembers();
  }

  

}
