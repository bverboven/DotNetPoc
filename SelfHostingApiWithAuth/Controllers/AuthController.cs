using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SelfHostingApiWithAuth.Auth.Jwt.Abstraction;
using System.Security.Claims;

namespace SelfHostingApiWithAuth.Controllers;

[ApiController]
[Route("auth")]
public class AuthController(ITokenHelper tokenHelper) : ControllerBase
{
    [AllowAnonymous]
    [HttpPost("login")]
    public IActionResult Login(string username, string password)
    {
        var token = tokenHelper.Create([new Claim("sub", username)]);
        return Ok(new
        {
            isAuthenticated = true,
            token
        });
    }
}
