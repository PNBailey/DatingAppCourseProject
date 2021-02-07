using System.Collections.Generic;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;

namespace API.Interfaces
{
    public interface ILikesRepository
    {
        Task<UserLike> GetUserLike(int sourceUserId, int likedUserId); // We will retrieve the like by using the like key which is the sourceUserId and the likedUserId combined

        Task<AppUser> GetUserWithLikes(int userId);

        Task<IEnumerable<LikeDto>> GetUserLikes(string predicate, int userId); // This couild be a list of users that have been liked or liked by 

        
     
    }
}