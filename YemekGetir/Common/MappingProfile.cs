using System.Linq;
using AutoMapper;
using YemekGetir.Application.UserOperations.Commands.CreateUser;
using YemekGetir.Application.UserOperations.Queries.GetUserById;
using YemekGetir.Entities;

namespace YemekGetir.Common
{
  public class MappingProfie : Profile
  {
    public MappingProfie()
    {
      CreateMap<CreateUserModel, User>()
        .ForMember(dest => dest.Password, opt => opt.MapFrom(src => BCrypt.Net.BCrypt.HashPassword(src.Password))); ;

      CreateMap<User, GetUserByIdViewModel>();

      CreateMap<Order, UserDetailOrderVM>()
        .ForMember(dest => dest.RestaurantName, opt => opt.MapFrom(src => src.Restaurant.Name));

      CreateMap<Cart, UserDetailCartVM>();

      CreateMap<Address, UserDetailAddressVM>();
    }
  }
}