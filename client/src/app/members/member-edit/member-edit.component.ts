import { Component, HostListener, OnInit, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';
import { take } from 'rxjs/operators';
import { Member } from 'src/app/Models/member';
import { User } from 'src/app/Models/user';
import { AccountService } from 'src/app/_services/account.service';
import { MembersService } from 'src/app/_services/members.service';

@Component({
  selector: 'app-member-edit',
  templateUrl: './member-edit.component.html',
  styleUrls: ['./member-edit.component.css']
})
export class MemberEditComponent implements OnInit {
  @ViewChild('editForm') editForm: NgForm; // As we have used a local reference in our html template where the form tag is, we can access this element from within this component. This allows us to reset the form in the updateMember method below
  member: Member;
  user: User;
  @HostListener('window:beforeunload', ['$event']) unloadNotification($event: any) { // This allows us to show a warning message if the user tries to close a tab (or go to google for example) and asks them if they want to leave the page as any changes made to the form will be . The hostlistener gives us access to browser events 

    if(this.editForm.dirty) {
      $event.returnValue = true;
    }

  } 

  constructor(private accountService: AccountService, private memberService: MembersService, private toastr: ToastrService) {
    this.accountService.currentUser$.pipe(take(1)).subscribe(user => this.user = user);
   }

  ngOnInit(): void {
    this.loadMember();
    
  }

  loadMember() {
    this.memberService.getMember(this.user.username).subscribe(member => {
      this.member = member;
    })
  }

  updateMember() {
    this.memberService.updateMember(this.member).subscribe(() => {
      this.toastr.success('Profile Updated Successfully');
      this.editForm.reset(this.member); // passing in the member to the reset method here (which is a built in method of the NgForm), means that the forms values will be preserved (I.e. what the user has entered will be preserved rather than the reset method just resetting the form fields to blank)
    })
    
  }


}
