import { Component, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { NgxGalleryAnimation, NgxGalleryImage, NgxGalleryOptions } from '@kolkov/ngx-gallery';
import { TabDirective, TabsetComponent } from 'ngx-bootstrap/tabs';
import { Member } from 'src/app/Models/member';
import { Message } from 'src/app/Models/message';
import { MembersService } from 'src/app/_services/members.service';
import { MessageService } from 'src/app/_services/message.service';

@Component({
  selector: 'app-member-detail',
  templateUrl: './member-detail.component.html',
  styleUrls: ['./member-detail.component.css']
})
export class MemberDetailComponent implements OnInit {
  @ViewChild('memberTabs', {static: true}) memberTabs: TabsetComponent; // as the viewchild binds to the template reference variable in the html page, if we used ngif for the whole template using the member that is retrived from the api, the template would be available when this component loads for the view chil to get access to. 
  member: Member;
  galleryOptions: NgxGalleryOptions[];
  galleryImages: NgxGalleryImage[];
  activeTab: TabDirective;
  messages: Message[] = [];

  constructor(private memberService: MembersService, private route: ActivatedRoute, private messageService: MessageService) { }

  ngOnInit(): void {
    this.loadMember();

    this.route.queryParams.subscribe(params => {  // as wwe are passing the query params along with the route, to this page when the messages icon is clicked, we can get access to the queryparams from the injected ActivatedRoute
      params.tab ? this.selectTab(params.tab) : this.selectTab(0);

    });

    this.galleryOptions = [
      {
        width: '500px',
        height: '500px',
        imagePercent: 100,
        thumbnailsColumns: 4,
        imageAnimation: NgxGalleryAnimation.Slide,
        preview: false
      }
    ]
    
  }

  getImages(): NgxGalleryImage[] {
    const imageUrls = [];
    for(const photo of this.member.photos) {
      imageUrls.push({
        small: photo?.url,
        medium: photo?.url,
        big: photo?.url
      })
    }
    return imageUrls;
  }

  loadMember() {
    this.memberService.getMember(this.route.snapshot.paramMap.get('username')).subscribe(member => {
      this.member = member;
      this.galleryImages = this.getImages();
    })
  }

  loadMessages() {
    console.log("hi");
    this.messageService.getMessageThread(this.member.userName).subscribe(messages => {
      this.messages = messages;
    });
  }

  onTabActivated(data: TabDirective) {
    
    this.activeTab = data;
    console.log(this.activeTab.heading);
    if(this.activeTab.heading === 'Messages' && this.messages.length === 0) {
      this.loadMessages();
    }
  }

  selectTab(tabId: number) {
    this.memberTabs.tabs[tabId].active = true; // the memberTabs is a TabsetComponent. This gives us the array of tabs as we have used a template reference variable on the html file. When we pass in the index of the tab we would like to go to, it will activate that tab
  }

}
