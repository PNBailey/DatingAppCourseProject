import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { User } from '../Models/user';

@Injectable({
  providedIn: 'root'
})
export class AdminService {
  baseUrl = environment.apiUrl; // This gets the url from the browser url 

  constructor(private http: HttpClient) { }

  getUsersWithRoles() {
    return this.http.get<Partial<User[]>>(this.baseUrl + 'admin/users-with-roles'); // As we will only be getting some of the properties of the User object back with this request, we use the Partial keyword
  }

  updateUserRoles(username: string, roles: string[]) {
    return this.http.post(this.baseUrl + 'admin/edit-roles/' + username + '?roles=' + roles, {}) // we are adding query strings here so we have to add the ?roles=. On the end point, we use the [FromQuery] atribute to retrieve the query strings from the URL. As this is a post request, we have to add an object as the 2nd parameter so we just add an empty object in this case


  }
}
