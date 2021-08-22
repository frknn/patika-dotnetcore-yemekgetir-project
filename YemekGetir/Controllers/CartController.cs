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

namespace YemekGetir.Controllers
{
  [ApiController]
  [Route("[Controller]s")]
  public class CartController : ControllerBase
  {
    private readonly IYemekGetirDbContext _context;
    private readonly IMapper _mapper;
    private readonly IConfiguration _configuration;
    private readonly IHttpContextAccessor _httpContextAccessor;
    public CartController(IYemekGetirDbContext context, IMapper mapper, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
    {
      _context = context;
      _mapper = mapper;
      _configuration = configuration;
      _httpContextAccessor = httpContextAccessor;
    }

    [Authorize]
    [HttpPost("{id}")]
    public IActionResult AddProduct(int id, [FromBody] AddProductToCartModel newItem)
    {
      AddProductCommand command = new AddProductCommand(_context, _mapper, _httpContextAccessor);
      command.Id = id;
      command.Model = newItem;

      AddProductCommandValidator validator = new AddProductCommandValidator();
      validator.ValidateAndThrow(command);

      command.Handle();

      return Ok();
    }

    [Authorize]
    [HttpPut("{cartId}/items/{itemId}")]
    public IActionResult UpdateProduct(string cartId, string itemId, [FromBody] UpdateProductInCartModel itemQuantity)
    {
      UpdateProductCommand command = new UpdateProductCommand(_context, _mapper, _httpContextAccessor);
      command.CartId = cartId;
      command.LineItemId = itemId;
      command.Model = itemQuantity;

      UpdateProductCommandValidator validator = new UpdateProductCommandValidator();
      validator.ValidateAndThrow(command);

      command.Handle();

      return Ok();
    }

    [Authorize]
    [HttpDelete("{id}/items")]
    public IActionResult EmptyCart(string id)
    {
      EmptyCartCommand command = new EmptyCartCommand(_context, _httpContextAccessor);
      command.Id = id;

      EmptyCartCommandValidator validator = new EmptyCartCommandValidator();
      validator.ValidateAndThrow(command);

      command.Handle();

      return Ok();
    }
  }
}