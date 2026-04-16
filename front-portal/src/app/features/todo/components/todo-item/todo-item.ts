import { Component, input, output, signal } from '@angular/core';
import { Todo } from '../../models/todo.model';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';

@Component({
  selector: 'app-todo-item',
  imports: [ReactiveFormsModule],
  templateUrl: './todo-item.html',
  styleUrl: './todo-item.scss',
})
export class TodoItem {

  todo = input.required<Todo>();
  deleteTodo = output<string>();
  updateTodo = output<{description: string, todoId: string, done: boolean}>();
  updateTodoDone = output<{done: boolean, todoId: string}>();

  protected isEditing = signal(false);

  form = new FormGroup({
    description: new FormControl<string>('', {
      validators: [Validators.required, Validators.maxLength(250), Validators.minLength(3)],
      nonNullable: true,
    })
  });

  onStartEdit() {
    this.form.controls.description.setValue(this.todo().description);
    this.isEditing.set(true);
  }

  onUpdate(){
    this.isEditing.set(false);
    this.updateTodo.emit({ todoId: this.todo().id, description: this.form.value.description!, done: this.todo().done });
  }
  onUpdateTodoDone(done: boolean) {
    this.updateTodoDone.emit({ todoId: this.todo().id, done });
  }

  onDelete() {
  if (window.confirm('Are you sure you want to delete this todo?')) {
    this.deleteTodo.emit(this.todo().id);
  }
}
}
