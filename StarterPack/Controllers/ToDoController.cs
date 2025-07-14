using Microsoft.AspNetCore.Mvc;
using StarterPack.Models.ToDo;
using StarterPack.Services;

namespace StarterPack.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [IgnoreAntiforgeryToken]
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
            return Ok(todo);
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
            return Ok(); 
        }

        [HttpPut("{id}")]
        public ActionResult<ToDoItem> UpdateTodo(int id, [FromBody] UpdateToDoRequest request) 
        {
            if (string.IsNullOrWhiteSpace(request.Text))
            {
                return BadRequest("Todo text cannot be empty");
            }
            var todo = _toDoService.UpdateTodo(id, request.Text);
            if (todo == null)
            {
                return NotFound();
            }
            return Ok(todo);
        }

        [HttpPut("{id}/date")]
        public ActionResult<ToDoItem> UpdateTodoDate(int id, [FromBody] UpdateToDoDateRequest request) 
        {
            var todo = _toDoService.UpdateTodoDate(id, request.DueDate);
            if (todo == null)
            {
                return NotFound();
            }
            return Ok(todo);
        }
    }

}