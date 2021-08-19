using System;
using System.Linq;
using AutoMapper;
using YemekGetir.DBOperations;
using YemekGetir.Entities;

namespace YemekGetir.Application.RestaurantOperations.Commands.CreateRestaurant
{
  public class CreateRestaurantCommand
  {
    public CreateRestaurantModel Model { get; set; }
    private readonly IYemekGetirDbContext _dbContext;
    private readonly IMapper _mapper;

    public CreateRestaurantCommand(IYemekGetirDbContext dbContext, IMapper mapper)
    {
      _dbContext = dbContext;
      _mapper = mapper;
    }

    public void Handle()
    {
      Restaurant restaurant = _dbContext.Restaurants.SingleOrDefault(restaurant => restaurant.Email == Model.Email);
      if (restaurant is not null)
      {
        throw new InvalidOperationException("Restoran zaten mevcut.");
      }

      restaurant = _mapper.Map<Restaurant>(Model);
      _dbContext.Restaurants.Add(restaurant);
      _dbContext.SaveChanges();
    }
  }

  public class CreateRestaurantModel
  {
    private string name;
    public string Name
    {
      get { return name; }
      set { name = value.Trim(); }
    }
    private string email;
    public string Email
    {
      get { return email; }
      set { email = value.Trim(); }
    }
    private string password;
    public string Password
    {
      get { return password; }
      set { password = value.Trim(); }
    }
  }
}