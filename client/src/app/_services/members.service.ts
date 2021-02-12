import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { of } from 'rxjs';
import { map, take } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { LikesParams } from '../Models/likesParams';
import { Member } from '../Models/member';
import { PaginatedResult } from '../Models/pagination';
import { User } from '../Models/user';
import { UserParams } from '../Models/userParams';
import { AccountService } from './account.service';


@Injectable({
  providedIn: 'root'
})

export class MembersService {
  baseUrl = environment.apiUrl;
  members: Member[] = [];
  memberCache = new Map(); // If we want to store something with a key and value, a good thing to use is a map. A map is like a dictionary.
  user: User; 
  userParams: UserParams;

  constructor(private http: HttpClient, private accountService: AccountService) {
    this.accountService.currentUser$.pipe(take(1)).subscribe(user => {
      this.user = user;
      this.userParams = new UserParams(user);
    })
   }

   getUserParams() {
     return this.userParams;
   }

   setUserParams(params: UserParams) {
     this.userParams = params;
   }

   resetUserParams() {
     this.userParams = new UserParams(this.user);
     return this.userParams;
   }

  getMembers(userParams: UserParams) { // The ? here means that the page can be null
    // if(this.members.length > 0) return of(this.members); // In this getMembers method, we only want to retrieve the members from the database if we don't already have the members in our database. As our client is expecting an observable from this getMembers method, we need to return an observable which is why we use the of keyword. Of just means we are going to return something of an observable. It turns the return of the members into an observable so that when our member list component subscribes to it, it still works correctly.
    var response = this.memberCache.get(Object.values(userParams).join('-')); // The Object.values we use here will create an array from the object you pass into it. We then join the arrays together with a - between them. The response here will be the key of our key value pair. So this is our key that we are trying to retrieve from the memberCache: 'Object.values(userParams).join('-')'. If the response that is retrieved here is identical to the userParams that is passed into the getMembers method then the below code will simply return the reponse as an observable because we don;t want to get our members if we have already retrieved them with the same userParams
    if(response) { // If there is a response, this means that we already have our members so therefore we don;t need to go and get them again
      return of(response); // If there is a response, we return it as an observable 
    }

    let params = this.getPaginationHeaders(userParams.pageNumber, userParams.pageSize);

    params = params.append('minAge', userParams.minAge.toString());
    params = params.append('maxAge', userParams.maxAge.toString());
    params = params.append('gender', userParams.gender);
    params = params.append('orderBy', userParams.orderBy);

    
    
    return this.getPaginatedResult<Member[]>(this.baseUrl + 'users', params).pipe(map(response => {
      this.memberCache.set(Object.values(userParams).join('-'), response); // The response from this is our paginated result. We use the response data (The members returned from the http request) here to set it in our memberCache Map property.
      return response; // The response here is our members array 
    }))
  }

  

  getMember(username: string) {
    // for(let value of this.memberCache.values()) { // This was my attempt
    //   for(let user of value.result) {
    //     if(user.userName === username) {
    //       return of(user);
    //     }
    //   }
    // }
    const member = [...this.memberCache.values()] // This is the instructors attempt. We use the spread operator to put the paginated results from the values retrieved from our memberCache
    .reduce((arr, elem) => arr.concat(elem.result), []) // We then use the reduce method (call back function) on the above member (now) array to reduce our array into something else. We just want the results of each array in a single array that we can search to find the first member that has the same username that is passed into this method. This reduce method is caled for every element in the member array. The 'arr' argument is the previous value (the previous). The 'elem' is our current value. The empty [] is the initial value. So we are concatinating the previous value (arr, which initially is set to an empty array) with the current value (elem.result). This will flatten the array into one array which is an array of member objects

    .find((member: Member) => member.userName === username); // This iterates over every member in the flattened array and returns the member if the userName is equal to the username passed into the getMember method 

    if(member) {
      return of(member);
    }


    return this.http.get<Member>(`${this.baseUrl}users/${username}`);
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

  addLike(username: string) {
    return this.http.post(this.baseUrl + 'likes/' + username, {}) // Because this is a post request, we have to add the body of the request so we add an empty object {}.
  }

  getLikes(likesParams: LikesParams) {
    let params = this.getPaginationHeaders(likesParams.pageNumber, likesParams.pageSize);

    params = params.append('predicate', likesParams.predicate);

    return this.getPaginatedResult<Partial<Member[]>>(`${this.baseUrl}likes`, params); //The predicate will either be 'liked' or 'likedBy' which will determine whether we want to retrieve the liked users or the users that have liked the logged in user
  }

  private getPaginatedResult<T>(url, params) { // We set the type of this method to a generic type using the T 
    const paginatedResult: PaginatedResult<T> = new PaginatedResult<T>();
    return this.http.get<T>(url, { observe: 'response', params }).pipe(
      map(response => {
        paginatedResult.result = response.body; // This is where our members array will be contained
        if (response.headers.get('Pagination') !== null) {
          paginatedResult.pagination = JSON.parse(response.headers.get('Pagination'));
        }
        return paginatedResult;
      })

    );
  }

  private getPaginationHeaders(pageNumber: number, pageSize: number) {
    let params = new HttpParams(); // This gives us the ability to serialize our parameters. This will take care of adding this onto our query string. 

      params = params.append('pageNumber', pageNumber.toString()); // as the page needs to be a query string we need to convert the page from a number to a string 
      params = params.append('pageSize', pageSize.toString());

      return params;
    
  }


}
