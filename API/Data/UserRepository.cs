using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class UserRepository : IUserRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        public UserRepository(DataContext context, IMapper mapper) // As this class is going to be communicating with the database, we need to inject the context
        {
            _mapper = mapper;
            _context = context;

        }

        public async Task<MemberDto> GetMemberAsync(string username) // We create this method as rather than getting all the users from the database and then finding the correct user, it would be more optimal to find the user at the database level and then simply returning that user as a Member Dto
        {
            return await _context.Users
            .Where(x => x.UserName == username) // We use the Where linq method as this will return the user which has the matched username that we pass into this method. This is more effecient thatn just getting all the users from the database and then finding each user 
            // .Select(user => new MemberDto { // The select method allows us to select a user from the database and then create a new class using the user when it comes back from our database
            //     Id = user.Id,
            //     UserName = user.UserName
            //     .....
            // })
            // ***** Rather than do the above with .Select which returns all the users data even though we don;t need all of it, we can use automapper. Automapper gives us the equivalent of doing the above for every single property and it allows us to project inside of our repository and it's onyl going to select the properties that we actually need.
            .ProjectTo<MemberDto>(_mapper.ConfigurationProvider) // To get the configuration we provided in our AutoMapper profiles class which has the create map and details the conversions we want to do. This ProjectTo will take the user (AppUser) type at database level and convert it to a Member Dto when it comes back from the database
            .SingleOrDefaultAsync(); // This is where we execute the query. This then goes to our database once this has been called


        }

        public async Task<PagedList<MemberDto>> GetMembersAsync(UserParams userParams)
        {
            var query = _context.Users.AsQueryable(); // The AsQueryable gives us an oppurtunity to do something with this query (e.g. what we want to filter by for instance)
            // .ProjectTo<MemberDto>(_mapper.ConfigurationProvider)  
            // .ToListAsync(); // This actually executes the database query and coverts it to a list 
            // .AsNoTracking() // In entity framework, when we go an get entities, Entity framework applies tracking to these entities. Becuase this is going to be a list that we only ever read from, we use the AsNoTracking to prevent the tracking on the entity 
            // .AsQueryable(); // This gives us an oppurtunity to do something with this query (e.g. what we want to filter by for instance)

            query = query.Where(user => user.UserName != userParams.CurrentUserName); // This filters out 
            query = query.Where(user => user.Gender == userParams.Gender);

            var minDob = DateTime.Today.AddYears(-userParams.MaxAge - 1); // We use the -1 so that we give the accurate year. So here we minus the MaxAge (which is just a number) from Todays date. The gives us the minDob date that we need to specify in our Where method below
            var maxDob = DateTime.Today.AddYears(-userParams.MinAge);

            query = query.Where(user => user.DateOfBirth >= minDob && user.DateOfBirth <= maxDob);

            //The code below in the switch statement enables sorting of the data being retrieved from the database
            
            query = userParams.OrderBy switch // This is the way we create a switch statement in C#.
            {
                "created" => query.OrderByDescending(u => u.Created),
                _ => query.OrderByDescending(u => u.LastActive)// The _ here is the way we state tha the default value will be used 
            };


            return await PagedList<MemberDto>.CreateAsync(query.ProjectTo<MemberDto>(_mapper.ConfigurationProvider).AsNoTracking(), userParams.PageNumber, userParams.PageSize);  // We specify what type of PageList we want with the <MemberDto>. This means we want to return a PageList of MemberDto's. The query is the source data that the CreateAsync static method expects. So we get a PagedList ad then we create a PagedList using the parameters. This method
        }   

        public async Task<AppUser> GetUserByIdAsync(int id) // This method is more efficient when retrievig the user as the Id is indexed and if we don't need the photos, we should use this method to retrieve the users rather that getting user by username as we do below
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<AppUser> GetUserByUsernameAsync(string username)
        {
            return await _context.Users
            .Include(p => p.Photos)
            .SingleOrDefaultAsync(x => x.UserName == username);
        }

        public async Task<IEnumerable<AppUser>> GetUsersAsync()
        {
            return await _context.Users
            .Include(p => p.Photos) // We can use eager loading here to get a related collection back with the request (users photos in this case). As the AppUser entity has a photos property and the photos entity has an AppUser property, this is classed as circular reference and will throw a 500 error. We resolve this by shaping our data before we return it with a DTO
            .ToListAsync();
        }

        public async Task<bool> SaveAllAsync() // On this methid, we are returning a boolean to say our changes have been saved 
        {
            return await _context.SaveChangesAsync() > 0; // Using the > 0 here returns true if there have been greater than 0 changes have been saved to our database. SaveChangesAsync returns an integer. Hence why we can use this syntax
        }

        public void Update(AppUser user)
        {
            _context.Entry(user).State = EntityState.Modified; // This method marks the entity to say it has been modified. It adds a modified flag to the entity. This is so the SaveAllAsync method knows which entities have been amended  
        }
    }
}