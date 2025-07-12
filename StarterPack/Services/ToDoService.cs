using StarterPack.Models;
using StarterPack.Models.ToDo;

namespace StarterPack.Services
{
    public class ToDoService
    {
        private readonly List<ToDoItem> _todos;
        private int _nextId = 1;

        public ToDoService()
        {
            _todos = new List<ToDoItem>
            {
                new ToDoItem { Id = _nextId++, Text = "Wake Up", DueDate = DateTime.Today, IsCompleted = false },
                new ToDoItem { Id = _nextId++, Text = "Make breakfast", DueDate = DateTime.Today.AddDays(-2), IsCompleted = false },
                new ToDoItem { Id = _nextId++, Text = "Watch TikTok", DueDate = DateTime.Today.AddDays(-1), IsCompleted = false },
                new ToDoItem { Id = _nextId++, Text = "Go sleep", DueDate = DateTime.Today, IsCompleted = false },
                new ToDoItem { Id = _nextId++, Text = "Sleep", DueDate = DateTime.Today, IsCompleted = true },
                new ToDoItem { Id = _nextId++, Text = "Watch a video", DueDate = DateTime.Today.AddDays(-5), IsCompleted = true },
                new ToDoItem { Id = _nextId++, Text = "Watch a movie", DueDate = DateTime.Today.AddDays(-6), IsCompleted = true },
                new ToDoItem { Id = _nextId++, Text = "Dance", DueDate = DateTime.Today.AddDays(-9), IsCompleted = true }
            };
        }

        public IEnumerable<ToDoItem> GetAllTodos()
        {
            return _todos.OrderBy(t => t.IsCompleted).ThenBy(t => t.DueDate);
        }

        public IEnumerable<ToDoItem> GetPendingTodos()
        {
            return _todos.Where(t => !t.IsCompleted).OrderBy(t => t.DueDate);
        }

        public IEnumerable<ToDoItem> GetCompletedTodos()
        {
            return _todos.Where(t => t.IsCompleted).OrderByDescending(t => t.DueDate);
        }

        public ToDoItem AddTodo(string text, DateTime? dueDate = null)
        {
            var todo = new ToDoItem
            {
                Id = _nextId++,
                Text = text,
                DueDate = dueDate ?? DateTime.Today,
                IsCompleted = false
            };
            _todos.Add(todo);
            return todo;
        }

        public ToDoItem? ToggleTodo(int id)
        {
            var todo = _todos.FirstOrDefault(t => t.Id == id);
            if (todo != null)
            {
                todo.IsCompleted = !todo.IsCompleted;
            }
            return todo;
        }

        public bool DeleteTodo(int id)
        {
            var todo = _todos.FirstOrDefault(t => t.Id == id);
            if (todo != null)
            {
                _todos.Remove(todo);
                return true;
            }
            return false;
        }

        public ToDoItem? UpdateTodo(int id, string newText)
        {
            var todo = _todos.FirstOrDefault(t => t.Id == id);
            if (todo != null)
            {
                todo.Text = newText;
            }
            return todo;
        }

        public ToDoItem? UpdateTodoDate(int id, DateTime newDate)
        {
            var todo = _todos.FirstOrDefault(t => t.Id == id);
            if (todo != null)
            {
                todo.DueDate = newDate;
            }
            return todo;
        }
    }
}