import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { Message } from '../Models/message';
import { getPaginatedResult, getPaginationHeaders } from './paginationHelper';

@Injectable({
  providedIn: 'root'
})
export class MessageService {
  baseUrl = environment.apiUrl;
  constructor(private http: HttpClient) { }

  getMessages(pageNumber, pageSize, container) {
    let params = getPaginationHeaders(pageNumber, pageSize); // The getPaginationHeaders method is in the paginationHelpers .ts file. This function is exported from this file.
    params = params.append('Container', container); // We append the query params so that we can change what's being requested from the api
    return getPaginatedResult<Message[]>(this.baseUrl + 'messages', params, this.http);
  }
}
