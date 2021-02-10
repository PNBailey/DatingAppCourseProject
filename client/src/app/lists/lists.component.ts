import { Component, OnInit } from '@angular/core';
import { Member } from '../Models/member';
import { MembersService } from '../_services/members.service';

@Component({
  selector: 'app-lists',
  templateUrl: './lists.component.html',
  styleUrls: ['./lists.component.css']
})
export class ListsComponent implements OnInit {
  members: Partial<Member[]>; // The partial type means that we can receive an array of members where not every property of the members has a value, so they are partially completed
  predicate = 'liked'; 

  constructor(private memberService: MembersService) { }

  ngOnInit() {
    this.loadLikes();
  }

  loadLikes() {
    this.memberService.getLikes(this.predicate).subscribe(response => {
      this.members = response;
    })
  }

}
