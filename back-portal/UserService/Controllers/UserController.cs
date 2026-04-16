using Microsoft.AspNetCore.Mvc;
using UserService.Models.DTOs;
using UserService.Models.Interfaces;

namespace UserService.Controllers;

[ApiController]
[Route("api/user")]
public class UserController(IUserService userService) : ControllerBase
{
    [Route("create")]
    [HttpPost]
    public async Task<IActionResult> CreateUser([FromBody]CreateUserDto userDto)
    {
        
        try
        {
            await userService.CreateUser(userDto);
            return Ok();
        }
        catch (ArgumentException ex)
        {
            return Conflict(ex.Message);
        }
        catch (Exception) 
        {
            return StatusCode(500);
        }
    }

    [Route("login")]
    [HttpPost]
    public async Task<IActionResult> LoginUser([FromBody] LoginUserDto userDto)
    {
        try
        {
           var result =  await userService.LoginUser(userDto);
            return Ok(result);
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
}
