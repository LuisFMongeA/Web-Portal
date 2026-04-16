using Microsoft.AspNetCore.Mvc;
using TodoService.Models.DTOs;
using TodoService.Models.Interfaces;

namespace TodoService.Controllers;

[ApiController]
[Route("api/todo")]
public class TodoController(ITodoService todoService) : BaseController
{
    [Route("")]
    [HttpGet]
    public async Task<IActionResult> GetAllTodos()
    {
        try
        {
            var all = await todoService.GetAll(CurrentUserEmail);
            return Ok(all ?? new List<ResponseTodoDto>());
        }
        catch (ArgumentException)
        {
            return Ok(new List<ResponseTodoDto>());
        }
        catch (Exception) 
        {
            return StatusCode(500);
        }
    }

    [Route("{id}")]
    [HttpGet]
    public async Task<IActionResult> GetTodoById([FromRoute] Guid id)
    {
        try
        {
            var todo = await todoService.GetTodoById(id, CurrentUserEmail);
            return Ok (todo);
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

    [Route("")]
    [HttpPost]
    public async Task<IActionResult> CreateTodo([FromBody] CreateTodoDto todoDto)
    {
        try
        {
            todoDto.UserId = CurrentUserEmail;
            var todo = await todoService.CreateTodo(todoDto);
            return CreatedAtAction(nameof(GetTodoById), new { id = todo.Id, email = todoDto.UserId }, todo);
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

    [Route("{id}")]
    [HttpPut]
    public async Task<IActionResult> UpdateTodo([FromRoute] Guid id,[FromBody] UpdateTodoDto todoDto)
    {

        try
        {
            todoDto.UserId = CurrentUserEmail;
            if (id != todoDto.Id)
                return BadRequest();

            var todo = await todoService.UpdateTodo(todoDto);
            return Ok(todo);
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

    [Route("{id}")]
    [HttpDelete]
    public async Task<IActionResult> DeleteTodo([FromRoute] Guid id)
    {
        try
        {
            await todoService.DeleteTodo(id, CurrentUserEmail);
            return NoContent();
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

    [Route("{id}/done")]
    [HttpPatch]
    public async Task<IActionResult> UpdateTodoDone([FromRoute] Guid id, [FromQuery] bool newValue)
    {
        try
        {
            var todo = await todoService.UpdateDone(id, CurrentUserEmail, newValue);
            return Ok(todo);
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
