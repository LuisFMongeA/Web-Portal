import { Injectable, inject, signal } from "@angular/core";
import { AuthService } from "../../../core/services/auth.service";
import { HttpClient } from "@angular/common/http";
import { environment } from "../../../../environments/environment";
import { Todo } from "../models/todo.model";
import { ERROR_MESSAGES } from "../../../core/constants/error-messages";


@Injectable({
  providedIn: 'root',
})
export class TodoService {
    private authService = inject(AuthService);
    
    private readonly email = this.authService.email;
    private httpClient = inject(HttpClient);
    private readonly apiUrlBase = environment.todoApiUrl;

    private _todoList = signal<Todo[]>([]);
    private _error = signal<string | null>(null);

    readonly todoList = this._todoList.asReadonly();
    readonly error = this._error.asReadonly();

    getTodos() {
         this.httpClient.get<Todo[]>(`${this.apiUrlBase}/todo`).subscribe( {
            next: (todos) => {
                this._todoList.set(todos);
            },
            error: () => {
                this._error.set(ERROR_MESSAGES.todo.loadFailed);
            }
        });
    }
    
    addTodo(description: string) {
        const newTodo = {
            userId: this.email(),
            description
        };
        this.httpClient.post<Todo>(`${this.apiUrlBase}/todo`, newTodo).subscribe({
            next: (todo) => {
                this._todoList.update(todos => [...todos, todo]);
            },
            error: () => {
                this._error.set(ERROR_MESSAGES.todo.addFailed);
            }
        });
    }

    updateTodo(description: string, todoId: string) {
        const updatedTodo = {
            id: todoId,
            userId: this.email(),
            description,
        };
        this.httpClient.put<Todo>(`${this.apiUrlBase}/todo/${todoId}`, updatedTodo).subscribe({
            next: (todo) => {
                this._todoList.update(todos => todos.map(t => t.id === todoId ? todo : t));
            },
            error: () => {
                this._error.set(ERROR_MESSAGES.todo.updateFailed);
            }
        });
    }

    deleteTodo(todoId: string) {
        this.httpClient.delete(`${this.apiUrlBase}/todo/${todoId}`).subscribe({
            next: () => {
                this._todoList.update(todos => todos.filter(t => t.id !== todoId));
            },
            error: () => {
                this._error.set(ERROR_MESSAGES.todo.deleteFailed);
            }
        });
    }

    updateTodoDone(done: boolean, todoId: string) {
        this.httpClient.patch<Todo>(`${this.apiUrlBase}/todo/${todoId}/done`, {}, {
            params: { newValue: done },
            headers: { 'Content-Type': 'application/json' }
        }).subscribe({
            next: (todo) => {
                this._todoList.update(todos => todos.map(t => t.id === todoId ? todo : t));
            },
            error: () => {
                this._error.set(ERROR_MESSAGES.todo.updateDoneFailed);
            }
        });
    }
}