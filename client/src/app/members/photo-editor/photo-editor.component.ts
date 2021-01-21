import { Component, Input, OnInit } from '@angular/core';
import { Member } from 'src/app/Models/member';
import {FileUploader} from 'ng2-file-upload';
import { environment } from 'src/environments/environment';
import { AccountService } from 'src/app/_services/account.service';
import { User } from 'src/app/Models/user';
import { take } from 'rxjs/operators';
import { MembersService } from 'src/app/_services/members.service';
import { Photo } from 'src/app/Models/photo';

@Component({
  selector: 'app-photo-editor',
  templateUrl: './photo-editor.component.html',
  styleUrls: ['./photo-editor.component.css']
})
export class PhotoEditorComponent implements OnInit {
  @Input() member: Member;
  uploader: FileUploader; // The fileUploader is avaiable as we have installed the package "npm install ng2-file-upload"
  hasBaseDropZoneOver = false;
  baseUrl = environment.apiUrl;
  user: User;

  constructor(private accountService: AccountService, private membersService: MembersService) { 
    accountService.currentUser$.pipe(take(1)).subscribe(user => this.user = user);
  }

  ngOnInit(): void {
    this.initializeUploader();
  }

  fileOverBase(e: any) {
    this.hasBaseDropZoneOver = e; // This sets the drop zone for our photo uploader. The 'e' is the event and we set the hasBaseDropZoneOver property to this event object
  }

  initializeUploader() { // This method will be called when the photo-editor component is created. We set the options for the FileUploader package below 
    this.uploader = new FileUploader({ 
      url: this.baseUrl + 'users/add-photo',
      authToken: 'Bearer ' + this.user.token,
      isHTML5: true,
      allowedFileType: ['image'],
      removeAfterUpload: true,
      autoUpload: false,
      maxFileSize: 10 * 1024 * 1024
    });

    this.uploader.onAfterAddingFile = (file) => 
    file.withCredentials = false; // If we didn't specify this, we would need to make adjustments to our CORS configuration and allow credentials to go up with our request 

    this.uploader.onSuccessItem = (item, response, status, headers) => {
      if (response) {
        const photo: Photo = JSON.parse(response); // This gets the photo from the JSON data that is retrieved from the response 
        this.member.photos.push(photo);
        // if member.photos is less than 1, call set as main photo and pass in response photo
        if(photo.isMain) {
          this.user.photoUrl = photo.url;
          this.member.photoUrl = photo.url;
          this.accountService.setCurrentUser(this.user);
      }
    }
  }
}

  deletePhoto(photoId: number) {
    this.membersService.deletePhoto(photoId).subscribe(() => {
      this.member.photos = this.member.photos.filter(x => x.id !== photoId); // This returns an array of all the photos that do not have the id of the photo we are deleting 
    })

  }

  setAsMainPhoto(photo: Photo) {
    this.membersService.setMainPhoto(photo.id).subscribe(() => {
      this.user.photoUrl = photo.url;
      this.accountService.setCurrentUser(this.user); // This updates our current user observable which will then update the users image on the nav bar
      this.member.photoUrl = photo.url; // This updates the photo url in the member variable as this is passed to our member edit component through the Input decorator and then used to display the users main photo on the edit profile page 
      this.member.photos.forEach(p => {
        if(p.isMain) p.isMain = false;
        if(p.id === photo.id) p.isMain = true;
      })

    });
  }

}
