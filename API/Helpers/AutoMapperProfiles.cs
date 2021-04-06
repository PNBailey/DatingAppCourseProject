using System.Linq;
using API.DTOs;
using API.Entities;
using API.Extensions;
using AutoMapper;

namespace API.Helpers
{
    public class AutoMapperProfiles : Profile // When using automapper, we must inherit from Profile base class 
    {
        public AutoMapperProfiles()
        {
            CreateMap<AppUser, MemberDto>() // This takes care of our mapping from our AppUser to our Member
            .ForMember(dest => dest.PhotoUrl, opt => opt.MapFrom(src => src.Photos.FirstOrDefault(x => x.IsMain).Url)) // As we want to populate the Photo Url property, we add some configuration. ForMember means: which proerty do we want to affect? the first parameter we pass in is the destination: What property are we looking to affect? The next part is the options. Here we can tell it where we want it to map from specifically. So this will go into the users photo collection and get the first photo or default that isMain and get the url from that. Now Automapper will add the photo url to the Member Dto when it does it's conversion rather than just returning null for the photo url
            .ForMember(dest => dest.Age, opt => opt.MapFrom(src => src.DateOfBirth.CalculateAge())); // Rather than calculate the age of the user within the Appuser class (this is not very efficient), we calculate the age here when the AppUser is converted to a Member Dto by automapper 
            CreateMap<Photo, PhotoDto>();

            CreateMap<MemberUpdateDto, AppUser>(); // As we want to map our updated member when the user updates their profile, we need to build this into our automapper

            CreateMap<RegisterDto, AppUser>();

            CreateMap<Message, MessageDto>()
                .ForMember(dest => dest.SenderPhotoUrl, opt => opt.MapFrom(src => src.Sender.Photos.FirstOrDefault(x => x.IsMain).Url)) // As the photo for the sender will not be automatically mapped, we need to add some configuration to get the url from the image which is marked as the main photo in the senders photos
                .ForMember(dest => dest.RecipientPhotoUrl, opt => opt.MapFrom(src => src.Recipient.Photos.FirstOrDefault(x => x.IsMain).Url)); 
        }

        // As we want to add this as a dependancy that we can inject, we need to add it to our applicationservice extensions
    }
}