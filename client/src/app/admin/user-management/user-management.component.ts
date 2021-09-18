import { Component, OnInit } from '@angular/core';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { RolesModalComponent } from 'src/app/Modals/roles-modal/roles-modal.component';
import { User } from 'src/app/Models/user';
import { AdminService } from 'src/app/_services/admin.service';


@Component({
  selector: 'app-user-management',
  templateUrl: './user-management.component.html',
  styleUrls: ['./user-management.component.css']
})
export class UserManagementComponent implements OnInit {
  users: Partial<User[]>;
  bsModalRef: BsModalRef;

  constructor(private adminService: AdminService, private modalService: BsModalService) { } // We inject the BsModalService here. This is from NGX Bootstrap and is a pop up box that is overlayed over the whole page

  ngOnInit(): void {
    this.getUsersWithRoles();
    
  }

  getUsersWithRoles() {
    this.adminService.getUsersWithRoles().subscribe(users => {
      this.users = users;
      console.log(this.users);
    })
  }

  openRolesModal(user: User) { // This method is required for the modal to work
    // const initialState = { // Inside this method we create some configuration for the modal
    //   list: [ // We can use a list property which will be displayed in the modal
    //     'Open a modal with component',
    //     'Pass your data',
    //     'Do something else',
    //     '...'
    //   ],
    //   title: 'Modal with component' // We can use the title property to specify what the title of the modal will be
    // };

    const config = {
      class: 'modal-dialog-centered', // This will ensure the modal appears in the centre of the browser screen
      initialState: {
        user,
        roles: this.getRolesArray(user)
      }
    }
    this.bsModalRef = this.modalService.show(RolesModalComponent, config); // When we call the modalServies .show method, we pass in our RolesModalComponent and the initialState cofig setting we created above. This then uses the RolesModalComponent to dispay the modal
    this.bsModalRef.content.updateSelectedRoles.subscribe(values => { // The values are the roles we get when the roles are updated using the event emitter in the roles.modal class
    console.log(values);
      const rolesToUpdate = { // We then create an object with a property called roles. We spread the multiple arrays that are coming from the values into one array and we filter this array to only have the roles that have been checked by the user, we then map each element of the array and return just the name of the role
        roles: [...values.filter(el => el.checked === true).map(el => el.name)]
      };
      console.log(rolesToUpdate);
      if(rolesToUpdate) {
        this.adminService.updateUserRoles(user.username, rolesToUpdate.roles).subscribe(() => { // If there are roles to update, we call the http post method to update the users roles in the database 
          user.roles = [...rolesToUpdate.roles];
        })
      }
    }); 
  }

  private getRolesArray(user) {
    const roles = [];
    const userRoles = user.roles;
    const availableRoles: any[] = [
      {name: 'Admin', value: 'Admin'},
      {name: 'Moderator', value: 'Moderator'},
      {name: 'Member', value: 'Member'}
    ];
    availableRoles.forEach(role => {
      let isMatch = false;
      for (const userRole of userRoles) {
        if(role.name === userRole) {
          isMatch = true;
          role.checked = true;
          roles.push(role);
          break; // Because we are in a loop within a loop, we need to break out of it
        }
      }
      if(!isMatch) {
        role.checked = false;
        roles.push(role);
      }
    })

    return roles;
  }
  


}
