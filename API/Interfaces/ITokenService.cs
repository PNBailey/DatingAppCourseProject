using System.Threading.Tasks;
using API.Entities;

// To accommodate the concept of single responsibility principle, we need to create a service that is solely responsible for the creation of JWT tokens. It’s not going to issue the tokens, that’s going to be the job of the account controller. This service is simply going to receive a user (AppUser in our project) and it’s going to create a token for that user and return it to the account controller. We create this interface so that our Tokenservice class can implement it.



namespace API.Interfaces
{
    public interface ITokenService
    {
         Task<string> CreateToken(AppUser user);
    }
}