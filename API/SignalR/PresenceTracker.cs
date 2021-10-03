using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.SignalR
{
    public class PresenceTracker
    {
        // A dicitionary stores values in key value pairs. The List here will be the connection id that // is created by Signal R when a user creates a connection. The Dictionary here will be shared 
        // by everyone who connects to our server. The standard string value here will be the users username
        private static readonly Dictionary<string, List<string>> OnlineUsers = new Dictionary<string, List<string>>(); 

        public Task UserConnected(string username, string connectionId) 
        {
            lock (OnlineUsers) // A dicitionary is not a thread safe resource. If we had concurrent users tring to update this at the same time, this may lead to issues. As the dictionary is shared by evryone that connects to our server, users may be trying to update the dictionary at the same time. To get around this issue, we 'lock' the dictionary whilst this method is being actioned.
            {        
                // First, we need to check to see if there is an entry in the dictionary for the user. We do this because the user may be logged in on another device already, meaning that there may already be an entry for them with a connection id. If that's the case, we don't want to create another entry for them, we just want to add the new connection id to their existing entry.

                // The ContainsKey method, is a method of the Dictionary type. We pass in the key we want to find. 

                if (OnlineUsers.ContainsKey(username))  
                {
                    OnlineUsers[username].Add(connectionId); // Here we access the users entry in the dicitonary by passing in the [username] key. We then use the dicitonaries add method to add the new connectionId to the list of connection id's the user has.
                }

                else 
                {
                    OnlineUsers.Add(username, new List<string>{connectionId}); // If there is no existing entry for the user, we create a new entry, adding the username as the key and a new list with the connection id as the value
                }
            }

            return Task.CompletedTask;
                
        }
    }
}