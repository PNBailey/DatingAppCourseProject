import { User } from "./user";

export class UserParams { // This class will be passed into our getMembers method in our member service file
    gender: string;
    minAge = 18;
    maxAge = 99;
    pageNumber = 1;
    pageSize = 5;
    orderBy = 'lasActive'; // This will enable the order of the users that is returned from our htp request will be by the lastActive users property

    constructor (user: User) {
        this.gender = user.gender === 'female' ? 'male' : 'female';
    }
}