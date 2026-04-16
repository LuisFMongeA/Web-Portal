import { Component, computed, inject, OnInit } from '@angular/core';
import { TodoService } from '../../services/todo.service';
import { FormGroup, FormControl, Validators, ReactiveFormsModule } from '@angular/forms';
import { TodoList } from "../../components/todo-list/todo-list";

@Component({
  selector: 'app-todo-page',
  imports: [TodoList, ReactiveFormsModule],
  templateUrl: './todo-page.html',
  styleUrl: './todo-page.scss',
})
export class TodoPage implements OnInit{
  
  private todoService = inject(TodoService);
  
  protected todoList = this.todoService.todoList;
  protected error = this.todoService.error;

  form = new FormGroup({
    description: new FormControl<string>('', {
      validators: [Validators.required, Validators.maxLength(250), Validators.minLength(3)],
      nonNullable: true,
    })
  });

  ngOnInit () {
    this.todoService.getTodos();
  }

  onAddTodo() {
    
    if (!this.form.valid) return;
    this.todoService.addTodo(this.form.value.description!);
    this.form.reset();
  }

  onUpdateTodo(todoId: string, description: string) {
    this.todoService.updateTodo(description, todoId);
  }

  onDeleteTodo(todoId: string) {
    this.todoService.deleteTodo(todoId);
  }

  onUpdateTodoDone(todoId: string, done: boolean) {
    this.todoService.updateTodoDone(done, todoId);
  }
}
