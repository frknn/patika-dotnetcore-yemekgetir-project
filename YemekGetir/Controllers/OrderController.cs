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
using System;
using YemekGetir.Application.CartOperations.Commands.AddProduct;
using YemekGetir.Application.RestaurantOperations.Commands.AddAddress;
using YemekGetir.Application.CartOperations.Commands.UpdateProduct;
using YemekGetir.Application.CartOperations.Commands.EmptyCart;
using YemekGetir.Application.OrderOperations.Commands.CreateOrder;
using YemekGetir.Application.OrderOperations.Commands.UpdateOrder;
using YemekGetir.Common;

namespace YemekGetir.Controllers
{
  [ApiController]
  [Route("[Controller]s")]
  public class OrderController : ControllerBase
  {
    private readonly IYemekGetirDbContext _context;
    private readonly IMapper _mapper;
    private readonly IConfiguration _configuration;
    private readonly IHttpContextAccessor _httpContextAccessor;
    public OrderController(IYemekGetirDbContext context, IMapper mapper, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
    {
      _context = context;
      _mapper = mapper;
      _configuration = configuration;
      _httpContextAccessor = httpContextAccessor;
    }

    [Authorize]
    [HttpPost]
    public IActionResult CreateOrder()
    {
      CreateOrderCommand command = new CreateOrderCommand(_context, _mapper, _httpContextAccessor);

      command.Handle();

      return Ok();
    }

    [Authorize]
    [HttpPut("{id}")]
    public IActionResult UpdateOrder(int id, [FromBody] UpdateOrderModel orderStatus)
    {
      UpdateOrderCommand command = new UpdateOrderCommand(_context, _mapper, _httpContextAccessor);
      command.Id = id;
      command.Model = orderStatus;

      UpdateOrderCommandValidator validator = new UpdateOrderCommandValidator();
      validator.ValidateAndThrow(command);
      command.Handle();

      return Ok();
    }
  }
}


