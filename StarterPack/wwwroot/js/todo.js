document.addEventListener('DOMContentLoaded', function () {
    const todoInput = document.getElementById('todo-input');
    const addTodoBtn = document.getElementById('add-todo-btn');
    const pendingTodosContainer = document.getElementById('pending-todos');
    const completedTodosContainer = document.getElementById('completed-todos');

    // Add new todo
    function addTodo() {
        const text = todoInput.value.trim();
        if (!text) return;

        fetch('/api/todo', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify({
                text: text,
                dueDate: new Date().toISOString()
            })
        })
            .then(response => response.json())
            .then(todo => {
                addTodoToDOM(todo, false);
                todoInput.value = '';
            })
            .catch(error => {
                console.error('Error adding todo:', error);
            });
    }

    // Toggle todo completion
    function toggleTodo(id, isCompleted) {
        fetch(`/api/todo/${id}/toggle`, {
            method: 'PUT'
        })
            .then(response => response.json())
            .then(todo => {
                const todoElement = document.querySelector(`[data-todo-id="${id}"]`);
                if (todoElement) {
                    todoElement.remove();
                    addTodoToDOM(todo, todo.isCompleted);
                }
            })
            .catch(error => {
                console.error('Error toggling todo:', error);
            });
    }

    // Add todo to DOM
    function addTodoToDOM(todo, isCompleted) {
        const todoDiv = document.createElement('div');
        todoDiv.className = isCompleted ? 'item-div completed-div' : 'item-div';
        todoDiv.setAttribute('data-todo-id', todo.id);

        const formattedDate = formatDate(todo.dueDate);

        todoDiv.innerHTML = `
            <div class="left-part">
                <input type="checkbox" id="item${todo.id}" name="item${todo.id}" class="input-todo todo-checkbox" ${isCompleted ? 'checked' : ''} />
                <label for="item${todo.id}">${todo.text}</label>
            </div>
            <span class="date-todo">
                <img src="images/calendar.png" alt="calendar-icon" class="calendar-icon">
                ${formattedDate}
            </span>
        `;

        // Add event listener to checkbox
        const checkbox = todoDiv.querySelector('.todo-checkbox');
        checkbox.addEventListener('change', function () {
            toggleTodo(todo.id, this.checked);
        });

        // Add to appropriate container
        if (isCompleted) {
            completedTodosContainer.appendChild(todoDiv);
        } else {
            pendingTodosContainer.appendChild(todoDiv);
        }
    }

    // Format date
    function formatDate(dateString) {
        const date = new Date(dateString);
        const today = new Date();

        if (date.toDateString() === today.toDateString()) {
            return 'due today';
        }

        return date.toLocaleDateString('en-GB', {
            day: '2-digit',
            month: '2-digit',
            year: 'numeric'
        });
    }

    // Event listeners
    addTodoBtn.addEventListener('click', addTodo);

    todoInput.addEventListener('keypress', function (e) {
        if (e.key === 'Enter') {
            addTodo();
        }
    });

    // Add event listeners to existing checkboxes
    document.querySelectorAll('.todo-checkbox').forEach(checkbox => {
        checkbox.addEventListener('change', function () {
            const todoId = this.closest('[data-todo-id]').getAttribute('data-todo-id');
            toggleTodo(parseInt(todoId), this.checked);
        });
    });
});