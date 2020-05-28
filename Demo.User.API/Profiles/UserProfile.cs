using AutoMapper;
using Demo.User.Domain.Model.Dto;
using Mark.Common;

namespace Demo.User.API.Profiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<Domain.Aggregate.User, UserDto>()
                .ForMember(
                    dest => dest.Age,
                    opt => opt.MapFrom(src => src.BirthDay.ToAge()))
                .ForMember(dest => dest.Gender,
                    opt => opt.MapFrom(src => CommonMethod.GetEnumDescription(src.Gender)));
        }
    }
}
