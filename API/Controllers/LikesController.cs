using System.Collections.Generic;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize]
    public class LikesController : BaseApiController
    {
        private readonly IUserRepository _userRepository;
        private readonly ILikesRepository _likesRepository;
        public LikesController(IUserRepository userRepository, ILikesRepository likesRepository)
        {
            _likesRepository = likesRepository;
            _userRepository = userRepository;
        }

        [HttpPost("{username}")] // We need to extract the username from the route parameters 

        public async Task<ActionResult> AddLike(string username)
        {
        
            var sourceUserId = User.GetUserId();
            var likedUser = await _userRepository.GetUserByUsernameAsync(username);
            var sourceUser = await _likesRepository.GetUserWithLikes(sourceUserId);

            if(likedUser == null) return NotFound();

            if(sourceUser.UserName == username) return BadRequest("you cannot like yourself");

            var userLike = await _likesRepository.GetUserLike(sourceUserId, likedUser.Id); // if the userlike is not found here then that means the user hasn't already been liked by the logged user 

            if(userLike != null) return BadRequest("You already liked this user");

            userLike = new UserLike
            {
                SourceUserId = sourceUserId, // We only need to add this two properties to our UserLike entity as this is the only two columns we have 
                LikedUserId = likedUser.Id
            };

            sourceUser.LikedUsers.Add(userLike);

            if(await _userRepository.SaveAllAsync()) return Ok();

            return BadRequest("Failed to like user");

        }

        [HttpGet]

        public async Task<ActionResult<IEnumerable<LikeDto>>> GetUserLikes(string predicate) 
        {
            // return await _likesRepository.GetUserLikes(predicate, User.GetUserId()); // The action result doesn't work so well with an interface like IEnumerable so we use the below instead

            var users = await _likesRepository.GetUserLikes(predicate, User.GetUserId());

            return Ok(users); 
        }

    }
}