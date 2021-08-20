using AutoMapper;
using YemekGetir.Application.RestaurantOperations.Commands.AddAddress;
using YemekGetir.Application.RestaurantOperations.Commands.AddProduct;
using YemekGetir.Application.RestaurantOperations.Commands.CreateRestaurant;
using YemekGetir.Application.RestaurantOperations.Queries.GetRestaurantById;
using YemekGetir.Application.RestaurantOperations.Queries.GetRestaurants;
using YemekGetir.Application.UserOperations.Commands.AddAddress;
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
        .ForMember(dest => dest.Password, opt => opt.MapFrom(src => BCrypt.Net.BCrypt.HashPassword(src.Password)));

      CreateMap<User, GetUserByIdViewModel>();

      CreateMap<Order, GetUserByIdOrderVM>()
        .ForMember(dest => dest.RestaurantName, opt => opt.MapFrom(src => src.Restaurant.Name));

      CreateMap<Order, GetRestaurantByIdOrderVM>()
        .ForMember(dest => dest.Status, opt => opt.MapFrom(src => (StatusEnum)src.StatusId));
      // .ForMember(dest => dest.User.Name, opt => opt.MapFrom(src => src.User.FirstName + " " + src.User.LastName));

      CreateMap<Cart, GetUserByIdCartVM>();

      CreateMap<Address, GetUserByIdAddressVM>();

      CreateMap<Restaurant, GetRestaurantByIdViewModel>()
        .ForMember(dest => dest.Category, opt => opt.MapFrom(src => (CategoryEnum)src.CategoryId));

      CreateMap<User, GetRestaurantByIdUserVM>()
        .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.FirstName + " " + src.LastName));

      CreateMap<Address, GetRestaurantByIdAddressVM>();

      CreateMap<Order, GetUserByIdOrderVM>()
        .ForMember(dest => dest.Status, opt => opt.MapFrom(src => (StatusEnum)src.StatusId));

      CreateMap<Product, GetRestaurantByIdProductVM>();

      CreateMap<Restaurant, GetRestaurantsVM>()
      .ForMember(dest => dest.Category, opt => opt.MapFrom(src => (CategoryEnum)src.CategoryId));

      CreateMap<Product, GetRestaurantsProductVM>();

      CreateMap<AddProductModel, Product>();

      CreateMap<AddAddressToUserModel, Address>();
      CreateMap<AddAddressToRestaurantModel, Address>();

      CreateMap<CreateRestaurantModel, Restaurant>()
        .ForMember(dest => dest.Password, opt => opt.MapFrom(src => BCrypt.Net.BCrypt.HashPassword(src.Password)));

      CreateMap<LineItem, GetUserByIdLineItemVM>();

      CreateMap<LineItem, GetRestaurantByIdLineItemVM>();
    }
  }
}