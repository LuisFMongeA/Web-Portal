using AuthService.Models.DTOs;
using AuthService.Models.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(IAuthService authService) : ControllerBase
{
    [Route("token")]
    [HttpPost]
    public IActionResult GetToken([FromBody] TokenRequestDto reqDto)
    {
        try
        {
            return Ok(authService.GenerateToken(reqDto.Email));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception)
        {
            return StatusCode(500);
        }
    }

    [Route("refresh")]
    [HttpPost]
    public IActionResult RefreshToken([FromBody] RefreshRequestDto reqDto)
    {
        try
        {
            return Ok(authService.RefreshToken(reqDto.RefreshToken));
        }
        catch (ArgumentException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception)
        {
            return StatusCode(500);
        }
    }

    [Route("invalidate")]
    [HttpPost]
    public IActionResult InvalidateToken([FromBody] InvalidateRequestDto reqDto)
    {
        try
        {
            authService.InvalidateToken(reqDto.AccessToken);
            return Ok();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception)
        {
            return StatusCode(500);
        }
    }

    [Route("validate")]
    [HttpPost]
    public IActionResult ValidateToken([FromBody] ValidateTokenDto reqDto) 
    {

        try
        {
            return authService.ValidateToken(reqDto.AccessToken)
                ? Ok()
                : Unauthorized();
        }
        catch (Exception) 
        {
            return StatusCode(500);
        }
    }

    [Route("public-key")]
    [HttpGet]
    public IActionResult GetPublicKey()
    {
        try
        {
            return Ok(authService.GetPublicKey());
        }
        catch (Exception)
        {
            return StatusCode(500);
        }
    }
}
