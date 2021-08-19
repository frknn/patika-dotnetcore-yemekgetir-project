using System.Linq;
using AutoMapper;
using YemekGetir.Application.RestaurantOperations.Queries.GetRestaurantById;
using YemekGetir.Application.RestaurantOperations.Queries.GetRestaurants;
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

      CreateMap<Order, GetUserByIdOrderVM>()
        .ForMember(dest => dest.RestaurantName, opt => opt.MapFrom(src => src.Restaurant.Name));

      CreateMap<Cart, GetUserByIdCartVM>();

      CreateMap<Address, GetUserByIdAddressVM>();

      CreateMap<Restaurant, GetRestaurantByIdViewModel>();

      CreateMap<User, GetRestaurantByIdUserVM>()
        .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.FirstName + " " + src.LastName));

      CreateMap<Address, GetRestaurantByIdAddressVM>();

      CreateMap<Order, GetUserByIdOrderVM>()
        .ForMember(dest => dest.Status, opt => opt.MapFrom(src => (StatusEnum)src.StatusId));

      CreateMap<Product, GetRestaurantByIdProductVM>();

      CreateMap<Restaurant, GetRestaurantsVM>();

      CreateMap<Product, GetRestaurantsProductVM>();

    }
  }
}