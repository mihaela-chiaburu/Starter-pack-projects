document.addEventListener('DOMContentLoaded', function () {
    const todoInput = document.getElementById('todo-input');
    const addTodoBtn = document.getElementById('add-todo-btn');
    const pendingTodosContainer = document.getElementById('pending-todos');
    const completedTodosContainer = document.getElementById('completed-todos');

    // Get anti-forgery token
    function getAntiForgeryToken() {
        const token = document.querySelector('input[name="__RequestVerificationToken"]');
        return token ? token.value : '';
    }

    // Get headers with anti-forgery token
    function getHeaders() {
        const headers = {
            'Content-Type': 'application/json',
        };

        const token = getAntiForgeryToken();
        if (token) {
            headers['RequestVerificationToken'] = token;
        }

        return headers;
    }

    function addTodo() {
        const text = todoInput.value.trim();
        if (!text) return;

        fetch('/api/todo', {
            method: 'POST',
            headers: getHeaders(),
            body: JSON.stringify({
                text: text,
                dueDate: new Date().toISOString()
            })
        })
            .then(response => {
                if (!response.ok) {
                    throw new Error(`HTTP error! status: ${response.status}`);
                }
                return response.json();
            })
            .then(todo => {
                addTodoToDOM(todo, false);
                todoInput.value = '';
            })
            .catch(error => {
                console.error('Error adding todo:', error);
                alert('Error adding todo. Please try again.');
            });
    }

    function toggleTodo(id, isCompleted) {
        fetch(`/api/todo/${id}/toggle`, {
            method: 'PUT',
            headers: getHeaders()
        })
            .then(response => {
                if (!response.ok) {
                    throw new Error(`HTTP error! status: ${response.status}`);
                }
                return response.json();
            })
            .then(todo => {
                const todoElement = document.querySelector(`[data-todo-id="${id}"]`);
                if (todoElement) {
                    todoElement.remove();
                    addTodoToDOM(todo, todo.isCompleted);
                }
            })
            .catch(error => {
                console.error('Error toggling todo:', error);
                alert('Error updating todo. Please try again.');
                // Revert checkbox state
                const checkbox = document.querySelector(`[data-todo-id="${id}"] .todo-checkbox`);
                if (checkbox) {
                    checkbox.checked = !checkbox.checked;
                }
            });
    }

    function deleteTodo(id) {
        if (confirm('Are you sure you want to delete this todo?')) {
            fetch(`/api/todo/${id}`, {
                method: 'DELETE',
                headers: getHeaders()
            })
                .then(response => {
                    if (!response.ok) {
                        throw new Error(`HTTP error! status: ${response.status}`);
                    }
                    const todoElement = document.querySelector(`[data-todo-id="${id}"]`);
                    if (todoElement) {
                        todoElement.remove();
                    }
                })
                .catch(error => {
                    console.error('Error deleting todo:', error);
                    alert('Error deleting todo. Please try again.');
                });
        }
    }

    function editTodo(id) {
        const todoElement = document.querySelector(`[data-todo-id="${id}"]`);
        const label = todoElement.querySelector('label');
        const currentText = label.textContent;

        const input = document.createElement('input');
        input.type = 'text';
        input.value = currentText;
        input.className = 'edit-input';
        input.style.cssText = 'background: transparent; border: 1px solid #ccc; padding: 2px; font-size: inherit; color: inherit;';

        label.style.display = 'none';
        label.parentNode.insertBefore(input, label.nextSibling);
        input.focus();
        input.select();

        function saveEdit() {
            const newText = input.value.trim();
            if (newText && newText !== currentText) {
                fetch(`/api/todo/${id}`, {
                    method: 'PUT',
                    headers: getHeaders(),
                    body: JSON.stringify({
                        text: newText
                    })
                })
                    .then(response => {
                        if (!response.ok) {
                            throw new Error(`HTTP error! status: ${response.status}`);
                        }
                        return response.json();
                    })
                    .then(todo => {
                        label.textContent = todo.text;
                        label.style.display = '';
                        input.remove();
                    })
                    .catch(error => {
                        console.error('Error updating todo:', error);
                        alert('Error updating todo. Please try again.');
                        cancelEdit();
                    });
            } else {
                cancelEdit();
            }
        }

        function cancelEdit() {
            label.style.display = '';
            input.remove();
        }

        input.addEventListener('blur', saveEdit);
        input.addEventListener('keypress', function (e) {
            if (e.key === 'Enter') {
                saveEdit();
            } else if (e.key === 'Escape') {
                cancelEdit();
            }
        });
    }

    function updateTodoDate(id) {
        const todoElement = document.querySelector(`[data-todo-id="${id}"]`);
        const dateSpan = todoElement.querySelector('.date-todo');
        const currentDateText = dateSpan.textContent.trim();

        const dateInput = document.createElement('input');
        dateInput.type = 'date';
        dateInput.className = 'date-input';
        dateInput.style.cssText = 'background: transparent; border: 1px solid #ccc; padding: 2px; font-size: 12px; color: inherit;';

        const today = new Date();
        dateInput.value = today.toISOString().split('T')[0];

        const originalContent = dateSpan.innerHTML;
        dateSpan.innerHTML = '';
        dateSpan.appendChild(dateInput);
        dateInput.focus();

        function saveDate() {
            const newDate = dateInput.value;
            if (newDate) {
                // Convert date string to ISO format for the API
                const dateObj = new Date(newDate);
                fetch(`/api/todo/${id}/date`, {
                    method: 'PUT',
                    headers: getHeaders(),
                    body: JSON.stringify({
                        dueDate: dateObj.toISOString()
                    })
                })
                    .then(response => {
                        if (!response.ok) {
                            throw new Error(`HTTP error! status: ${response.status}`);
                        }
                        return response.json();
                    })
                    .then(todo => {
                        const formattedDate = formatDate(todo.dueDate);
                        dateSpan.innerHTML = `
                        <img src="images/calendar.png" alt="calendar-icon" class="calendar-icon">
                        ${formattedDate}
                    `;
                    })
                    .catch(error => {
                        console.error('Error updating todo date:', error);
                        alert('Error updating date. Please try again.');
                        cancelDateEdit();
                    });
            } else {
                cancelDateEdit();
            }
        }

        function cancelDateEdit() {
            dateSpan.innerHTML = originalContent;
        }

        dateInput.addEventListener('blur', saveDate);
        dateInput.addEventListener('keypress', function (e) {
            if (e.key === 'Enter') {
                saveDate();
            } else if (e.key === 'Escape') {
                cancelDateEdit();
            }
        });
    }

    function addTodoToDOM(todo, isCompleted) {
        const todoDiv = document.createElement('div');
        todoDiv.className = isCompleted ? 'item-div completed-div' : 'item-div';
        todoDiv.setAttribute('data-todo-id', todo.id);

        const formattedDate = formatDate(todo.dueDate);

        if (isCompleted) {
            todoDiv.innerHTML = `
                <div class="left-part">
                    <input type="checkbox" id="completed${todo.id}" name="completed${todo.id}" class="input-todo todo-checkbox" checked />
                    <label for="completed${todo.id}">${todo.text}</label>
                </div>
                <div class="right-part">
                    <span class="date-todo">
                        <img src="images/calendar.png" alt="calendar-icon" class="calendar-icon">
                        ${formattedDate}
                    </span>
                    <img src="images/bin.png" alt="delete icon" class="edit-icon bin delete-btn">
                </div>
            `;
        } else {
            todoDiv.innerHTML = `
                <div class="left-part">
                    <input type="checkbox" id="item${todo.id}" name="item${todo.id}" class="input-todo todo-checkbox" />
                    <label for="item${todo.id}">${todo.text}</label>
                </div>
                <div class="right-part">
                    <img src="images/edit.png" alt="edit icon" class="edit-icon edit-btn">
                    <span class="date-todo">
                        <img src="images/calendar.png" alt="calendar-icon" class="calendar-icon">
                        ${formattedDate}
                    </span>
                    <img src="images/bin.png" alt="delete icon" class="edit-icon bin delete-btn">
                </div>
            `;
        }

        const checkbox = todoDiv.querySelector('.todo-checkbox');
        checkbox.addEventListener('change', function () {
            toggleTodo(todo.id, this.checked);
        });

        const deleteBtn = todoDiv.querySelector('.delete-btn');
        deleteBtn.addEventListener('click', function () {
            deleteTodo(todo.id);
        });

        const editBtn = todoDiv.querySelector('.edit-btn');
        if (editBtn) {
            editBtn.addEventListener('click', function () {
                editTodo(todo.id);
            });
        }

        const dateSpan = todoDiv.querySelector('.date-todo');
        dateSpan.addEventListener('click', function () {
            updateTodoDate(todo.id);
        });

        if (isCompleted) {
            completedTodosContainer.appendChild(todoDiv);
        } else {
            pendingTodosContainer.appendChild(todoDiv);
        }
    }

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

    function toggleSection(arrowElement) {
        const section = arrowElement.closest('.todo-items, .completed-items');
        const todoContainer = section.querySelector('#pending-todos, #completed-todos');

        if (todoContainer.style.display === 'none') {
            todoContainer.style.display = 'block';
            arrowElement.classList.remove('up');
        } else {
            todoContainer.style.display = 'none';
            arrowElement.classList.add('up');
        }
    }

    // Event listeners for main controls
    addTodoBtn.addEventListener('click', addTodo);

    todoInput.addEventListener('keypress', function (e) {
        if (e.key === 'Enter') {
            addTodo();
        }
    });

    // Event listeners for existing todos (server-rendered)
    document.querySelectorAll('.todo-checkbox').forEach(checkbox => {
        checkbox.addEventListener('change', function () {
            const todoId = this.closest('[data-todo-id]').getAttribute('data-todo-id');
            toggleTodo(parseInt(todoId), this.checked);
        });
    });

    document.querySelectorAll('.delete-btn').forEach(btn => {
        btn.addEventListener('click', function () {
            const todoId = this.closest('[data-todo-id]').getAttribute('data-todo-id');
            deleteTodo(parseInt(todoId));
        });
    });

    document.querySelectorAll('.edit-btn').forEach(btn => {
        btn.addEventListener('click', function () {
            const todoId = this.closest('[data-todo-id]').getAttribute('data-todo-id');
            editTodo(parseInt(todoId));
        });
    });

    document.querySelectorAll('.date-todo').forEach(dateSpan => {
        dateSpan.addEventListener('click', function () {
            const todoId = this.closest('[data-todo-id]').getAttribute('data-todo-id');
            updateTodoDate(parseInt(todoId));
        });
    });

    document.querySelectorAll('.arrow-down').forEach(arrow => {
        arrow.addEventListener('click', function () {
            toggleSection(this);
        });
    });
});