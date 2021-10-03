using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace API.SignalR
{
    // We inherit from the Hub class which is provided by asp.net Signal R. asp.net comes with Signalr
    // so there is no need to install it via Nuget. 

    [Authorize]
    public class PresenceHub : Hub
    {
        private readonly PresenceTracker _presenceTracker;
        public PresenceHub(PresenceTracker presenceTracker)
        {
            _presenceTracker = presenceTracker;

        }

        // The Hub class we inherited above has a few virtual methods which we can override (placing
        // the virtual keyword before the method name enables you to overrride the method elsewhere)
        // We use the override keyword so that we can override the method from the class we have 
        // inherited.

        // Inside the Hub, we have access to the Clients. These are the clients that are 
            // connected to this hub. In this method, we send a message to all the 'others' (everybody  // except the connection that triggered the current invocation). 'Others' is a property we  // access on the Clients object. The "UserIsOnline" is the name of the method we 
            // will use on the client. We have accesss to the context with our token inside here
            // so we can access the currently logged in users username. This is what we will pass
            // back to the other users. The Context here is off type HubCallerContext. HubCallerContext // has a property which is off type UserClaimsPrincipal, this is why we get access to the 
            // User object of the claims principal
        public override async Task OnConnectedAsync()
        {
            // 1. When a client connects, we are going to update our presence tracker
            await _presenceTracker.UserConnected(Context.User.GetUsername(), Context.ConnectionId);
            
            // 2. We trigger the "UserIsOnline" method which is triggered in the client (this displays a toastr message). This sends a message to all the'other' users (all the other users that are currently logged in)
            await Clients.Others.SendAsync("UserIsOnline", Context.User.GetUsername());

            // 3. We get the list of currently logged in users
            var currentUsers = await _presenceTracker.GetOnlineUsers();

            // 4. We trigger the "GetOnlineUsers" method which is trigered on the client
            await Clients.All.SendAsync("GetOnlineUsers", currentUsers);
        }


        // The below method has a required parameter which is an error Exception
        public override async Task OnDisconnectedAsync(Exception exception)
        {

            await _presenceTracker.UserDisconnected(Context.User.GetUsername(), Context.ConnectionId);

            await Clients.Others.SendAsync("UserIsOffline", Context.User.GetUsername());

            // We get the list of currently logged in users
            var currentUsers = await _presenceTracker.GetOnlineUsers();

            // We trigger the "GetOnlineUsers" method which is trigered on the client
            await Clients.All.SendAsync("GetOnlineUsers", currentUsers);

            // Using the 'base' keyword here passes the exception up to the parent class (Hub class)
            // if there is an exception. So this will call the OnDisconnectedAsync method on the 
            // base class itself
            await base.OnDisconnectedAsync(exception);
        }
    }
}