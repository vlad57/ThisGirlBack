using System;
using API.Models;
using API.Models.DTOs.Auth;
using API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controller;



[Route("api/[controller]")]
[ApiController]
public class AuthController(IAuthService authService) : ControllerBase
{
    [HttpPost("register")]
    public async Task<ActionResult<User>> Register(RegisterRequest request)
    {
        var result = await authService.RegisterAsync(request);
        if (!result.Success)
        {
            return BadRequest(result.Error);
        }

        return Ok(result.Data);
    }

}
