import { Injectable } from '@angular/core';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { ToastrService } from 'ngx-toastr';
import { environment } from 'src/environments/environment';
import { User } from '../Models/user';

@Injectable({
  providedIn: 'root'
})
export class PresenceService {
huburl = environment.hubUrl;
private hubConnection: HubConnection;

  constructor(private toastr: ToastrService) { }

  // We crwate a method to create a hub connection so that when our user does connect to the app and    // they're are authenticated, it will automatically create a hub connection that's going to connect   // them to our presence hub. We pass in our user as we need to send up our JWT token. We can't use
  // our JWT interceptor as these are not http requests, they are typically WebSockets which do not 
  // support authentication headers. 
  createHubConnection(user: User) {
    this.hubConnection = new HubConnectionBuilder()
      .withUrl(this.huburl + 'presence', { // The url we pass in here must match the url end point we created for signal (this is in the startup file)
        accessTokenFactory: () => user.token // The accessTokenFactory returns a string containing the access token. We return our users JWT token as this is the token that we need to pass as a query string to the signal r end point
      })
      .withAutomaticReconnect() // This automatically reconnects the user if there's  network problem
      .build() // This takes care of creating the hub connection

      this.hubConnection // This starts the hub connection
        .start()
        .catch(error => console.log(error)); // This catches any errors that are thrown when the connection is created

      this.hubConnection.on('UserIsOnline', username => { // The .on method listens for the UserIsOnline method and when they are triggered, the call back function we pass in as the 2nd argument is called. The UserIsOnline string must exactly match the string we provide in our OnConnectedAsync method in our presenceHub c# class.
        this.toastr.info(username + ' has connected');
      });

      this.hubConnection.on('UserIsOffline', username => { // The .on method listens for the UserIsOffline method and when they are triggered, the call back function we pass in as the 2nd argument is called. The UserIsOffline string must exactly match the string we provide in our OnDisconnectedAsync method in our presenceHub c# class
        this.toastr.warning(username + ' has disconnected');
      })

  }

  stopHubConnection() { // This is how the connectionis stopped
    this.hubConnection.stop().catch(error => console.log(error));
  }
}
