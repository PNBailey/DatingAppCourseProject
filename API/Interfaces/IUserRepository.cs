using System.Collections.Generic;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;

// We create this repository interface. Repository is layer of abstraction between our controllers and our DbContext. Instead of the controller going directly to the DbContext, it then uses a repository and executes the methods inside there. This encapsulates the logic and if a controller injects a repository, it then only has access to the methods that we need that controller to have. It also reduces duplicate query logic. E.g. if we wanted to find the user in a database and we had the same method setup in all of our controllers, we could add the method to the repository and then simply call the repository method in each of the controllers. It also promotes testability. *** We also have to add this interface and the class that dervies from the interface to the ApplicationServices extension class 

namespace API.Interfaces
{
    public interface IUserRepository
    {
        void Update(AppUser user); // This isn't async as this will just update the tracking of the entity and then the saveAllAsybc will actually update the database 

        Task<bool> SaveAllAsync();

        Task<IEnumerable<AppUser>> GetUsersAsync();

        Task<AppUser> GetUserByIdAsync(int id);

        Task<AppUser> GetUserByUsernameAsync(string username);

        Task<IEnumerable<MemberDto>> GetMembersAsync(); 

        Task<MemberDto> GetMemberAsync(string username); // We create this method as rather than getting all the users from the database and then finding the correct user, it would be more optimal to find the user at the database level and then simply returning that user as a Member Dto
    }
}