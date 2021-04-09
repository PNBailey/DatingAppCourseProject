import { Injectable } from "@angular/core";
import { ActivatedRouteSnapshot, Resolve } from "@angular/router";
import { Observable } from "rxjs";
import { Member } from "../Models/member";
import { MembersService } from "../_services/members.service";

// Using this resolver means that the member will be retrieved before the member detail component is loaded 

@Injectable({
    providedIn: 'root'
})
export class MemberDetailedResolver implements Resolve<Member> {
    constructor(private memberService: MembersService) {}

    resolve(route: ActivatedRouteSnapshot): Member | Observable<Member> | Promise<Member> {
        return this.memberService.getMember(route.paramMap.get('username'));
    }


}