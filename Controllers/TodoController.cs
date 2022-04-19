using Todo.Models;
using Todo.Repostories;
using Microsoft.AspNetCore.Mvc;
using Todo.DTOs;
using Microsoft.AspNetCore.Authorization;
using Todo.Utilities;
using System.Security.Claims;

namespace Todo.Controllers;

[ApiController]
[Authorize]
[Route("api/todo")]
public class TodoController : ControllerBase
{
    private readonly ILogger<TodoController> _logger;
    private readonly ITodoRepository _todo;

    public TodoController(ILogger<TodoController> logger,
    ITodoRepository todo)
    {
        _logger = logger;
        _todo = todo;
    }

    private int GetUserIdFromClaims(IEnumerable<Claim> claims)
    {
        return Convert.ToInt32(claims.Where(x => x.Type == TodoConstants.Id).First().Value);
    }

    [HttpPost]
    public async Task<ActionResult<TodoItem>> CreateTodo([FromBody] TodoCreateDTO Data)
    {
        var userId = GetUserIdFromClaims(User.Claims);

        var toCreateItem = new TodoItem
        {
            Title = Data.Title.Trim(),
            UserId = userId,
        };


        var createdItem = await _todo.Create(toCreateItem);


        return StatusCode(201, createdItem);
    }

    [HttpPut("{todo_id}")]
    public async Task<ActionResult> UpdateTodo([FromRoute] int todo_id,
    [FromBody] TodoUpdateDTO Data)
    {
        var userId = GetUserIdFromClaims(User.Claims);

        var existingItem = await _todo.GetById(todo_id);

        if (existingItem is null)
            return NotFound();

        if (existingItem.UserId != userId)
            return StatusCode(404, "You cannot update other's TODO");

        var toUpdateItem = existingItem with
        {
            Title = Data.Title is null ? existingItem.Title : Data.Title.Trim(),
            IsCompleted = !Data.IsCompleted.HasValue ? existingItem.IsCompleted : Data.IsCompleted.Value,
        };

        await _todo.Update(toUpdateItem);

        return NoContent();
    }

    [HttpDelete("{todo_id}")]
    public async Task<ActionResult> DeleteTodo([FromRoute] int todo_id)
    {
        var userId = GetUserIdFromClaims(User.Claims);

        var existingItem = await _todo.GetById(todo_id);

        if (existingItem is null)
            return NotFound();

        if (existingItem.UserId != userId)
            return StatusCode(404, "You cannot delete other's TODO");

        await _todo.Delete(todo_id);

        return NoContent();
    }

    [HttpGet]
    public async Task<ActionResult<List<TodoItem>>> GetAllTodos()
    {
        var allTodo = await _todo.GetAll();
        return Ok(allTodo);
    }
}