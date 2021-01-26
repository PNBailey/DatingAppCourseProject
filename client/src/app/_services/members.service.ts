import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { of } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { Member } from '../Models/member';
import { PaginatedResult } from '../Models/pagination';


@Injectable({
  providedIn: 'root'
})

export class MembersService {
  baseUrl = environment.apiUrl;
  members: Member[] = [];
  paginatedResult: PaginatedResult<Member[]> = new PaginatedResult<Member[]>();
  constructor(private http: HttpClient) { }

  getMembers(page?: number, itemsPerPage?: number) { // The ? here means that the page can be null
    // if(this.members.length > 0) return of(this.members); // In this getMembers method, we only want to retrieve the members from the database if we don't already have the members in our database. As our client is expecting an observable from this getMembers method, we need to return an observable which is why we use the of keyword. Of just means we are going to return something of an observable. It turns the return of the members into an observable so that when our member list component subscribes to it, it still works correctly.

    let params = new HttpParams(); // This gives us the ability to serialize our parameters. This will take care of adding this onto our query string. 

    if(page !== null && itemsPerPage !== null) {
      params = params.append('pageNumber', page.toString()); // as the page needs to be a query string we need to convert the page from a number to a string 
      params = params.append('pageSize', page.toString());
    }
    
    return this.http.get<Member[]>(this.baseUrl + 'users', {observe: 'response', params}).pipe( // Adding the {observe: 'response', params} means that rather than just getting the response body back, we get the full response back. 
    map(response => {
      this.paginatedResult.result = response.body; // This is where our members array will be contained
      if(response.headers.get('Pagination') !== null) {
        this.paginatedResult.pagination = JSON.parse(response.headers.get('Pagination'));
      }
      return this.paginatedResult;
    })
      
    );
  }

  getMember(username: string) {
    const member = this.members.find(x => x.userName === username);
    if(member !== undefined) return of(member);
    return this.http.get<Member>(this.baseUrl + 'users/' + username);
  }

  updateMember(member: Member) { // In this method, we are returning the members array which is wrapped in the Observable. As we amend the member-list component, it is now expecting an Observable which returns a members array. As we are subscribing to the Observable, we are receiving the data which is wrapped in the Observable. The data that is 'mapped' in the Observable doesn't necessarily have to be data which is coming from the server, it can be data which is stored locally as well
    return this.http.put(this.baseUrl + 'users', member).pipe(
      map(() => {
        const index = this.members.indexOf(member);
        this.members[index] = member;
      })
    );
  }

  setMainPhoto(photoId: number) {

    return this.http.put(this.baseUrl + "users/set-main-photo/" + photoId, {}); // We include an empty object as the body as put requests need a body 
  
  }

  deletePhoto(photoId: number) {
    return this.http.delete(this.baseUrl + 'users/delete-photo/' + photoId);
  }


}
