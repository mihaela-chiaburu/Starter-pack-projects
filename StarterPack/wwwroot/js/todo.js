document.addEventListener('DOMContentLoaded', function () {
    const todoInput = document.getElementById('todo-input');
    const addTodoBtn = document.getElementById('add-todo-btn');
    const pendingTodosContainer = document.getElementById('pending-todos');
    const completedTodosContainer = document.getElementById('completed-todos');

    // pagination
    let pendingTodos = [];
    let completedTodos = [];
    let pendingCurrentPage = 0;
    let completedCurrentPage = 0;
    const itemsPerPage = 4;

    function getAntiForgeryToken() {
        const token = document.querySelector('input[name="__RequestVerificationToken"]');
        return token ? token.value : '';
    }

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

    function initializeTodos() {
        const pendingElements = document.querySelectorAll('#pending-todos .item-div:not(.completed-div)');
        pendingTodos = Array.from(pendingElements).map(element => {
            const id = parseInt(element.getAttribute('data-todo-id'));
            const text = element.querySelector('label').textContent;
            const dateText = element.querySelector('.date-todo').textContent.trim();
            return {
                id: id,
                text: text,
                isCompleted: false,
                element: element.cloneNode(true)
            };
        });

        const completedElements = document.querySelectorAll('#completed-todos .item-div.completed-div');
        completedTodos = Array.from(completedElements).map(element => {
            const id = parseInt(element.getAttribute('data-todo-id'));
            const text = element.querySelector('label').textContent;
            const dateText = element.querySelector('.date-todo').textContent.trim();
            return {
                id: id,
                text: text,
                isCompleted: true,
                element: element.cloneNode(true)
            };
        });

        pendingCurrentPage = Math.max(0, Math.ceil(pendingTodos.length / itemsPerPage) - 1);
        completedCurrentPage = Math.max(0, Math.ceil(completedTodos.length / itemsPerPage) - 1);

        renderPendingTodos();
        renderCompletedTodos();
        updateArrowStates();
    }

    function renderPendingTodos() {
        pendingTodosContainer.innerHTML = '';

        if (pendingTodos.length === 0) {
            return;
        }

        const startIndex = pendingCurrentPage * itemsPerPage;
        const endIndex = Math.min(startIndex + itemsPerPage, pendingTodos.length);

        for (let i = startIndex; i < endIndex; i++) {
            const todo = pendingTodos[i];
            const todoElement = todo.element.cloneNode(true);

            attachEventListeners(todoElement, todo);
            pendingTodosContainer.appendChild(todoElement);
        }
    }

    function renderCompletedTodos() {
        completedTodosContainer.innerHTML = '';

        if (completedTodos.length === 0) {
            return;
        }

        const startIndex = completedCurrentPage * itemsPerPage;
        const endIndex = Math.min(startIndex + itemsPerPage, completedTodos.length);

        for (let i = startIndex; i < endIndex; i++) {
            const todo = completedTodos[i];
            const todoElement = todo.element.cloneNode(true);

            attachEventListeners(todoElement, todo);
            completedTodosContainer.appendChild(todoElement);
        }
    }

    function attachEventListeners(todoElement, todo) {
        const checkbox = todoElement.querySelector('.todo-checkbox');
        const deleteBtn = todoElement.querySelector('.delete-btn');
        const editBtn = todoElement.querySelector('.edit-btn');
        const dateSpan = todoElement.querySelector('.date-todo');

        if (checkbox) {
            checkbox.addEventListener('change', function () {
                toggleTodo(todo.id, this.checked);
            });
        }

        if (deleteBtn) {
            deleteBtn.addEventListener('click', function () {
                deleteTodo(todo.id);
            });
        }

        if (editBtn) {
            editBtn.addEventListener('click', function () {
                editTodo(todo.id);
            });
        }

        if (dateSpan) {
            dateSpan.addEventListener('click', function () {
                updateTodoDate(todo.id);
            });
        }
    }

    function updateArrowStates() {
        const pendingSection = document.querySelector('.todo-items');
        const completedSection = document.querySelector('.completed-items');

        //  todos arrows
        const pendingUpArrow = pendingSection.querySelector('.arrow-down.up');
        const pendingDownArrow = pendingSection.querySelector('.arrow-down:not(.up)');

        if (pendingUpArrow) {
            pendingUpArrow.style.opacity = pendingCurrentPage > 0 ? '1' : '0.3';
            pendingUpArrow.style.cursor = pendingCurrentPage > 0 ? 'pointer' : 'default';
        }

        if (pendingDownArrow) {
            const maxPage = Math.max(0, Math.ceil(pendingTodos.length / itemsPerPage) - 1);
            pendingDownArrow.style.opacity = pendingCurrentPage < maxPage ? '1' : '0.3';
            pendingDownArrow.style.cursor = pendingCurrentPage < maxPage ? 'pointer' : 'default';
        }

        const completedUpArrow = completedSection.querySelector('.arrow-down.up');
        const completedDownArrow = completedSection.querySelector('.arrow-down:not(.up)');

        if (completedUpArrow) {
            completedUpArrow.style.opacity = completedCurrentPage > 0 ? '1' : '0.3';
            completedUpArrow.style.cursor = completedCurrentPage > 0 ? 'pointer' : 'default';
        }

        if (completedDownArrow) {
            const maxPage = Math.max(0, Math.ceil(completedTodos.length / itemsPerPage) - 1);
            completedDownArrow.style.opacity = completedCurrentPage < maxPage ? '1' : '0.3';
            completedDownArrow.style.cursor = completedCurrentPage < maxPage ? 'pointer' : 'default';
        }
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
                addTodoToArray(todo, false);
                todoInput.value = '';

                pendingCurrentPage = Math.max(0, Math.ceil(pendingTodos.length / itemsPerPage) - 1);
                renderPendingTodos();
                updateArrowStates();
            })
            .catch(error => {
                console.error('Error adding todo:', error);
                alert('Error adding todo. Please try again.');
            });
    }

    function addTodoToArray(todo, isCompleted) {
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

            const todoObj = {
                id: todo.id,
                text: todo.text,
                isCompleted: true,
                element: todoDiv
            };

            completedTodos.push(todoObj);
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

            const todoObj = {
                id: todo.id,
                text: todo.text,
                isCompleted: false,
                element: todoDiv
            };

            pendingTodos.push(todoObj);
        }
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
                let todoObj;
                if (isCompleted) {
                    const index = pendingTodos.findIndex(t => t.id === id);
                    if (index !== -1) {
                        todoObj = pendingTodos.splice(index, 1)[0];
                    }
                } else {
                    const index = completedTodos.findIndex(t => t.id === id);
                    if (index !== -1) {
                        todoObj = completedTodos.splice(index, 1)[0];
                    }
                }

                if (todoObj) {
                    addTodoToArray(todo, todo.isCompleted);

                    if (isCompleted) {
                        const maxPage = Math.max(0, Math.ceil(pendingTodos.length / itemsPerPage) - 1);
                        if (pendingCurrentPage > maxPage) {
                            pendingCurrentPage = maxPage;
                        }
                        completedCurrentPage = Math.max(0, Math.ceil(completedTodos.length / itemsPerPage) - 1);
                    } else {
                        const maxPage = Math.max(0, Math.ceil(completedTodos.length / itemsPerPage) - 1);
                        if (completedCurrentPage > maxPage) {
                            completedCurrentPage = maxPage;
                        }
                        pendingCurrentPage = Math.max(0, Math.ceil(pendingTodos.length / itemsPerPage) - 1);
                    }

                    renderPendingTodos();
                    renderCompletedTodos();
                    updateArrowStates();
                }
            })
            .catch(error => {
                console.error('Error toggling todo:', error);
                alert('Error updating todo. Please try again.');
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

                    let pendingIndex = pendingTodos.findIndex(t => t.id === id);
                    let completedIndex = completedTodos.findIndex(t => t.id === id);

                    if (pendingIndex !== -1) {
                        pendingTodos.splice(pendingIndex, 1);
                        const maxPage = Math.max(0, Math.ceil(pendingTodos.length / itemsPerPage) - 1);
                        if (pendingCurrentPage > maxPage) {
                            pendingCurrentPage = maxPage;
                        }
                        renderPendingTodos();
                    }

                    if (completedIndex !== -1) {
                        completedTodos.splice(completedIndex, 1);
                        const maxPage = Math.max(0, Math.ceil(completedTodos.length / itemsPerPage) - 1);
                        if (completedCurrentPage > maxPage) {
                            completedCurrentPage = maxPage;
                        }
                        renderCompletedTodos();
                    }

                    updateArrowStates();
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

                        let todoObj = pendingTodos.find(t => t.id === id) || completedTodos.find(t => t.id === id);
                        if (todoObj) {
                            todoObj.text = todo.text;
                            todoObj.element.querySelector('label').textContent = todo.text;
                        }
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
    function navigatePending(direction) {
        const maxPage = Math.max(0, Math.ceil(pendingTodos.length / itemsPerPage) - 1);

        if (direction === 'up' && pendingCurrentPage > 0) {
            pendingCurrentPage--;
        } else if (direction === 'down' && pendingCurrentPage < maxPage) {
            pendingCurrentPage++;
        }

        renderPendingTodos();
        updateArrowStates();
    }

    function navigateCompleted(direction) {
        const maxPage = Math.max(0, Math.ceil(completedTodos.length / itemsPerPage) - 1);

        if (direction === 'up' && completedCurrentPage > 0) {
            completedCurrentPage--;
        } else if (direction === 'down' && completedCurrentPage < maxPage) {
            completedCurrentPage++;
        }

        renderCompletedTodos();
        updateArrowStates();
    }

    addTodoBtn.addEventListener('click', addTodo);

    todoInput.addEventListener('keypress', function (e) {
        if (e.key === 'Enter') {
            addTodo();
        }
    });

    document.addEventListener('click', function (e) {
        if (e.target.classList.contains('arrow-down')) {
            const section = e.target.closest('.todo-items, .completed-items');
            const isUp = e.target.classList.contains('up');

            if (section.classList.contains('todo-items')) {
                navigatePending(isUp ? 'up' : 'down');
            } else if (section.classList.contains('completed-items')) {
                navigateCompleted(isUp ? 'up' : 'down');
            }
        }
    });

    initializeTodos();
});