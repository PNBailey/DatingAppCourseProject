using System;
using System.Threading.Tasks;
using API.Extensions;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace API.Helpers
{

    // The action filter we use here allows us to do something either before the request is executing or after the request has executed. We will use this to update our lastActive field for each user. WE NEED TO ADD THIS SERVICE TO OUR APPLICATIONSERVCEEXTENSION METHOD SO THAT IT IS INCLUDED IN OUR STARTUP FILE. WE ALSO NEED TO ADD THE ACTION FILTER TO THE CONTROLLERS WE WISH TO USE IT IN. IN THIS CASE WE WANT TO USE THE ACTION FILTER IN ALL OUR CONTROLLERS SO WE ADD IT TO OUR BASE CONTROLLER
    public class LogUserActivity : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next) // The next here is what happens next once the action is executed. We use this parameter to execute the action and then do something afterwards. The 'ActionExecutingContext' here is the context of what happens before and the 'ActionExecutionDelegate' returns a 'ActionExecutedContext' which is the context of what happens afterwards
        {
            var resultContext = await next(); // So the resultContext here is our context after the action has been executed 

            if(!resultContext.HttpContext.User.Identity.IsAuthenticated) return; // We want to see if the user is authenticated (user is logged in) as we don't want to perform these actions if there isn't a user that is logged in. If the user sent up a token and we've authenticated the user then the 'IsAuthenticated' will be true, otherwise it will be false. Here we check to see if the 'IsAuthenticated' is false 

            var userId = resultContext.HttpContext.User.GetUserId(); // This gets our username from the claims principle. The 'User' object here is a claims principle and we created the GetUserName claims principle extension method 

            var repo = resultContext.HttpContext.RequestServices.GetService<IUserRepository>();// We need to get access to our repository so to do this we can use our service locator pattern. We have to manually import the GetService method from 'using Microsoft.Extensions.DependencyInjection' as VS code doesn't auto import it 

            var user = await repo.GetUserByIdAsync(userId); 

            user.LastActive = DateTime.Now;

            await repo.SaveAllAsync(); // This updates the database with our updated user entity above



        }
    }
}