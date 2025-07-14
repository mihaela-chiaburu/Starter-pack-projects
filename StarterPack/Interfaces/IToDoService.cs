using StarterPack.Models.ToDo;

namespace StarterPack.Interfaces
{
    public interface IToDoService
    {
        IEnumerable<ToDoItem> GetAllTodos();
        IEnumerable<ToDoItem> GetPendingTodos();
        IEnumerable<ToDoItem> GetCompletedTodos();
        ToDoItem AddTodo(string text, DateTime? dueDate = null);
        ToDoItem? ToggleTodo(int id);
        bool DeleteTodo(int id);
        ToDoItem? UpdateTodo(int id, string newText);
        ToDoItem? UpdateTodoDate(int id, DateTime newDate);
    }
}
