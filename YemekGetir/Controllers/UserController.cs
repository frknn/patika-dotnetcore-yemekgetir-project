using System;
using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using YemekGetir.Application.UserOperations.Commands.CreateToken;
using YemekGetir.Application.UserOperations.Commands.CreateUser;
using YemekGetir.Application.UserOperations.Commands.DeleteUser;
using YemekGetir.Application.UserOperations.Commands.RefreshToken;
using YemekGetir.Application.UserOperations.Queries.GetUserById;
using YemekGetir.DBOperations;
using YemekGetir.TokenOperations.Models;

namespace YemekGetir.Controllers
{
  [ApiController]
  [Route("[Controller]s")]
  public class UserController : ControllerBase
  {
    private readonly IYemekGetirDbContext _context;
    private readonly IMapper _mapper;
    private readonly IConfiguration _configuration;
    private readonly IHttpContextAccessor _httpContextAccessor;
    public UserController(IYemekGetirDbContext context, IMapper mapper, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
    {
      _context = context;
      _mapper = mapper;
      _configuration = configuration;
      _httpContextAccessor = httpContextAccessor;
    }

    [HttpPost]
    public IActionResult CreateUser([FromBody] CreateUserModel newUser)
    {
      CreateUserCommand command = new CreateUserCommand(_context, _mapper);
      command.Model = newUser;

      CreateUserCommandValidator validator = new CreateUserCommandValidator();
      validator.ValidateAndThrow(command);

      command.Handle();

      return Ok();
    }

    [HttpPost("connect/token")]
    public ActionResult<Token> CreateToken([FromBody] LoginModel loginInfo)
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
    public IActionResult DeleteUser(string id)
    {
      DeleteUserCommand command = new DeleteUserCommand(_context, _httpContextAccessor);
      command.Id = id;

      command.Handle();

      return Ok();
    }

    [Authorize]
    [HttpGet("{id}")]
    public IActionResult GetUserDetils(string id)
    {
      GetUserByIdQuery command = new GetUserByIdQuery(_context, _mapper, _httpContextAccessor);
      command.Id = id;

      GetUserByIdViewModel user = command.Handle();

      return Ok(user);
    }
  }
}