using System;
using API.Common;
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
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BadRequestError), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(BadRequestError), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<bool>> Register(RegisterRequest request)
    {
        var result = await authService.RegisterAsync(request);

        if (!result.Success)
        {
            return StatusCode((int)result.Code!, new BadRequestError { Status = (int)result.Code, Errors = result.Error! });
        }

        return Ok(true);
    }

    [HttpGet("confirm-email")]
    public async Task<ActionResult<bool>> ConfirmEmail([FromQuery] string email, [FromQuery] string token)
    {
        var result = await authService.ConfirmEmailAsync(email, token);
        if (!result)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

}
