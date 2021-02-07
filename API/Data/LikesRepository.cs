using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;

// THIS REPOSITORY NEEDS TO BE ADDED TO THE APPLICATION SERVICE EXTENSION FILE

namespace API.Data
{
    public class LikesRepository : ILikesRepository
    {
        private readonly DataContext _context;
        public LikesRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<UserLike> GetUserLike(int sourceUserId, int likedUserId)
        {
            return await _context.Likes.FindAsync(sourceUserId, likedUserId); // We will retrieve the like by using the like 'key' which is the sourceUserId and the likedUserId combined
        }

        public async Task<IEnumerable<LikeDto>> GetUserLikes(string predicate, int userId)
        {
            var users = _context.Users.OrderBy(user => user.UserName).AsQueryable(); // We use the AsQueryable so that we can build up our queries below
            var likes = _context.Likes.AsQueryable(); // We need to query both the users table and the likes table

            if(predicate == "liked") // This is the users that the currently logged in user has liked
            {
                likes = likes.Where(like => like.SourceUserId == userId); // Here we are returning each like where the sourceUserId matches the userId that is passed into this method.
                users = likes.Select(like => like.LikedUser); // Here we are returning the LikedUser (AppUser) from each of the likes that we have filtered above
            }

            if(predicate == "likedBy") // This is the users that the currently logged in user has been liked by
            {
                likes = likes.Where(like => like.LikedUserId == userId); // Here we are returning each like where the LikedUserId matches the userId that is passed into this method.
                users = likes.Select(like => like.SourceUser); // Here we are returning the SourceUser (AppUser) from each of the likes that we have filtered above
            }

            return await users.Select(user => new LikeDto // We don't use automapper here. We project directly into our Like Dto
            {
                Username = user.UserName,
                KnownAs = user.KnownAs,
                Age = user.DateOfBirth.CalculateAge(),
                PhotoUrl = user.Photos.FirstOrDefault(p => p.IsMain).Url,
                City = user.City,
                Id = user.Id
            }).ToListAsync(); 

        }

        public async Task<AppUser> GetUserWithLikes(int userId) // This method gets the logged in user and also retrieves their likes as well. When we add a like, we are going to be adding it to the user we return from here
        {
            return await _context.Users
                .Include(user => user.LikedUsers)
                .FirstOrDefaultAsync(user => user.Id == userId);
        }
    }
}