using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace TodoService.Controllers;

public class BaseController : ControllerBase
{
    protected string CurrentUserEmail =>
        User.FindFirstValue(ClaimTypes.Email)
        ?? throw new UnauthorizedAccessException("Email not found in token");
}
