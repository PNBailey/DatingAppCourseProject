import { HttpClient, HttpParams } from "@angular/common/http";
import { map } from "rxjs/operators";
import { PaginatedResult } from "../Models/pagination";

// As this is not a class, we neeed to export the functions so that we can use them elsewhere

export function getPaginatedResult<T>(url, params, http: HttpClient) { // We set the type of this method to a generic type using the T 
    const paginatedResult: PaginatedResult<T> = new PaginatedResult<T>();
    return http.get<T>(url, { observe: 'response', params }).pipe(
      map(response => {
        paginatedResult.result = response.body; // This is where our members array will be contained
        if (response.headers.get('Pagination') !== null) {
          paginatedResult.pagination = JSON.parse(response.headers.get('Pagination'));
        }
        return paginatedResult;
      })

    );
  }

  export function  getPaginationHeaders(pageNumber: number, pageSize: number) {
    let params = new HttpParams(); // This gives us the ability to serialize our parameters. This will take care of adding this onto our query string. 

      params = params.append('pageNumber', pageNumber.toString()); // as the page needs to be a query string we need to convert the page from a number to a string 
      params = params.append('pageSize', pageSize.toString());

      return params;
    
  }
