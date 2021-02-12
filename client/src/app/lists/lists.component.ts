import { Component, OnInit } from '@angular/core';
import { LikesParams } from '../Models/likesParams';
import { Member } from '../Models/member';
import { Pagination } from '../Models/pagination';
import { MembersService } from '../_services/members.service';

@Component({
  selector: 'app-lists',
  templateUrl: './lists.component.html',
  styleUrls: ['./lists.component.css']
})
export class ListsComponent implements OnInit {
  members: Partial<Member[]>; // The partial type means that we can receive an array of members where not every property of the members has a value, so they are partially completed
  likesParams: LikesParams;
  pagination: Pagination;

  constructor(private memberService: MembersService) { }

  ngOnInit() {
    this.likesParams = new LikesParams();
    this.loadLikes();
    
  }

  loadLikes() {
    this.memberService.getLikes(this.likesParams).subscribe(response => {
      this.members = response.result;
      this.pagination = response.pagination;
    })
  }

  pageChanged(event: any) {
    this.likesParams.pageNumber = event.page;
    this.loadLikes();

  }

}
