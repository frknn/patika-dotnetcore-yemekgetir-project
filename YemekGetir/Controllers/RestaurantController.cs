using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using YemekGetir.Application.RestaurantOperations.Commands.CreateToken;
using YemekGetir.Application.RestaurantOperations.Commands.CreateRestaurant;
using YemekGetir.Application.RestaurantOperations.Commands.DeleteRestaurant;
using YemekGetir.Application.RestaurantOperations.Commands.RefreshToken;
using YemekGetir.Application.RestaurantOperations.Queries.GetRestaurantById;
using YemekGetir.DBOperations;
using YemekGetir.TokenOperations.Models;
using YemekGetir.Application.RestaurantOperations.Queries.GetRestaurants;
using System.Collections.Generic;
using YemekGetir.Application.RestaurantOperations.Commands.AddProduct;
using YemekGetir.Application.RestaurantOperations.Commands.AddAddress;
using YemekGetir.Application.RestaurantOperations.Commands.UpdateAddress;
using YemekGetir.Application.RestaurantOperations.Commands.UpdateProduct;

namespace YemekGetir.Controllers
{
  [ApiController]
  [Route("[Controller]s")]
  public class RestaurantController : ControllerBase
  {
    private readonly IYemekGetirDbContext _context;
    private readonly IMapper _mapper;
    private readonly IConfiguration _configuration;
    private readonly IHttpContextAccessor _httpContextAccessor;
    public RestaurantController(IYemekGetirDbContext context, IMapper mapper, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
    {
      _context = context;
      _mapper = mapper;
      _configuration = configuration;
      _httpContextAccessor = httpContextAccessor;
    }

    [HttpPost]
    public IActionResult CreateRestaurant([FromBody] CreateRestaurantModel newRestaurant)
    {
      CreateRestaurantCommand command = new CreateRestaurantCommand(_context, _mapper);
      command.Model = newRestaurant;

      CreateRestaurantCommandValidator validator = new CreateRestaurantCommandValidator();
      validator.ValidateAndThrow(command);

      command.Handle();

      return Ok();
    }

    [HttpPost("connect/token")]
    public ActionResult<Token> CreateToken([FromBody] RestaurantLoginModel loginInfo)
    {
      CreateTokenCommand command = new CreateTokenCommand(_context, _configuration);
      command.Model = loginInfo;

      CreateTokenCommandValidator validator = new CreateTokenCommandValidator();
      validator.ValidateAndThrow(command);

      Token token = command.Handle();

      return token;
    }

    [HttpGet("refreshToken")]
    public ActionResult<Token> RefreshToken([FromQuery] string token)
    {
      RefreshTokenCommand command = new RefreshTokenCommand(_context, _configuration);
      command.RefreshToken = token;
      Token resultAccessToken = command.Handle();
      return resultAccessToken;
    }

    [Authorize]
    [HttpDelete("{id}")]
    public IActionResult DeleteRestaurant(string id)
    {
      DeleteRestaurantCommand command = new DeleteRestaurantCommand(_context, _httpContextAccessor);
      command.Id = id;

      command.Handle();

      return Ok();
    }

    [Authorize]
    [HttpGet("{id}")]
    public IActionResult GetRestaurantDetils(string id)
    {
      GetRestaurantByIdQuery command = new GetRestaurantByIdQuery(_context, _mapper, _httpContextAccessor);
      command.Id = id;

      GetRestaurantByIdViewModel user = command.Handle();

      return Ok(user);
    }

    [Authorize]
    [HttpPost("{id}/address")]
    public IActionResult AddAddress(string id, [FromBody] AddAddressToRestaurantModel newAddress)
    {
      AddAddressCommand command = new AddAddressCommand(_context, _mapper, _httpContextAccessor);
      command.Id = id;
      command.Model = newAddress;

      AddAddressCommandValidator validator = new AddAddressCommandValidator();
      validator.ValidateAndThrow(command);

      command.Handle();

      return Ok();
    }

    [Authorize]
    [HttpPut("{id}/address")]
    public IActionResult UpdateAddress(string id, [FromBody] UpdateRestaurantAddressModel updatedAddress)
    {
      UpdateAddressCommand command = new UpdateAddressCommand(_context, _mapper, _httpContextAccessor);
      command.Id = id;
      command.Model = updatedAddress;

      command.Handle();

      return Ok();
    }

    [HttpGet]
    public IActionResult GetRestaurants()
    {
      GetRestaurantsQuery query = new GetRestaurantsQuery(_context, _mapper);

      List<GetRestaurantsVM> restaurants = query.Handle();

      return Ok(restaurants);
    }

    [Authorize]
    [HttpPost("{id}/products")]
    public IActionResult AddProduct(string id, [FromBody] AddProductModel newProduct)
    {
      AddProductCommand command = new AddProductCommand(_context, _mapper, _httpContextAccessor);
      command.Id = id;
      command.Model = newProduct;

      AddProductCommandValidator validator = new AddProductCommandValidator();
      validator.ValidateAndThrow(command);

      command.Handle();

      return Ok();
    }

    [Authorize]
    [HttpPut("{restaurantId}/products/{productId}")]
    public IActionResult UpdateProduct(string restaurantId, string productId,[FromBody] UpdateProductModel updatedProduct)
    {
      UpdateProductCommand command = new UpdateProductCommand(_context, _mapper, _httpContextAccessor);
      command.RestaurantId = restaurantId;
      command.ProductId = productId;
      command.Model = updatedProduct;

      UpdateProductCommandValidator validator = new UpdateProductCommandValidator();
      validator.ValidateAndThrow(command);

      command.Handle();

      return Ok();
    }
  }
}