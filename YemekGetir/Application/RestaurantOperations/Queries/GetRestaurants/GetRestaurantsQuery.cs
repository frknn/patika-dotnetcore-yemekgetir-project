using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using YemekGetir.DBOperations;
using YemekGetir.Entities;

namespace YemekGetir.Application.RestaurantOperations.Queries.GetRestaurants
{
  public class GetRestaurantsQuery
  {
    private readonly IYemekGetirDbContext _dbContext;
    private readonly IMapper _mapper;

    public GetRestaurantsQuery(IYemekGetirDbContext dbContext, IMapper mapper)
    {
      _dbContext = dbContext;
      _mapper = mapper;
    }

    public List<GetRestaurantsVM> Handle()
    {
      List<Restaurant> restaurants = _dbContext.Restaurants
        .Include(restaurant => restaurant.Products)
          .ToList<Restaurant>();
      List<GetRestaurantsVM> restaurantsVM = _mapper.Map<List<GetRestaurantsVM>>(restaurants);
      return restaurantsVM;
    }
  }

  public class GetRestaurantsVM
  {
    public string Name { get; set; }
    public List<GetRestaurantsProductVM> Products { get; set; }
  }

  public class GetRestaurantsProductVM
  {
    public string Name { get; set; }
    public int Price { get; set; }
  }
}