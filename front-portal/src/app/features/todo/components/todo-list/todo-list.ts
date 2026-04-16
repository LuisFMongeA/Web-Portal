import { Component, input, output } from '@angular/core';
import { Todo } from '../../models/todo.model';
import { TodoItem } from "../todo-item/todo-item";

@Component({
  selector: 'app-todo-list',
  imports: [TodoItem],
  templateUrl: './todo-list.html',
  styleUrl: './todo-list.scss',
})
export class TodoList {
  todoList = input<Todo[]>([]);
  deleteTodo = output<string>();
  updateTodo = output<{description: string, todoId: string}>();
  updateTodoDone = output<{done: boolean, todoId: string}>();

}
