using Microsoft.AspNetCore.Mvc;
using StarterPack.Models.ToDo;
using StarterPack.Services;

namespace StarterPack.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ToDoController : ControllerBase
    {
        private readonly ToDoService _toDoService;

        public ToDoController(ToDoService toDoService)
        {
            _toDoService = toDoService;
        }

        [HttpGet]
        public ActionResult<IEnumerable<ToDoItem>> GetTodos()
        {
            return Ok(_toDoService.GetAllTodos());
        }

        [HttpPost]
        public ActionResult<ToDoItem> AddTodo([FromBody] CreateToDoRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Text))
            {
                return BadRequest("Todo text cannot be empty");
            }

            var todo = _toDoService.AddTodo(request.Text, request.DueDate);
            return CreatedAtAction(nameof(GetTodos), new { id = todo.Id }, todo);
        }

        [HttpPut("{id}/toggle")]
        public ActionResult<ToDoItem> ToggleTodo(int id)
        {
            var todo = _toDoService.ToggleTodo(id);
            if (todo == null)
            {
                return NotFound();
            }
            return Ok(todo);
        }

        [HttpDelete("{id}")]
        public ActionResult DeleteTodo(int id)
        {
            var success = _toDoService.DeleteTodo(id);
            if (!success)
            {
                return NotFound();
            }
            return NoContent();
        }
    }

    public class CreateToDoRequest
    {
        public string Text { get; set; } = string.Empty;
        public DateTime? DueDate { get; set; }
    }
}
