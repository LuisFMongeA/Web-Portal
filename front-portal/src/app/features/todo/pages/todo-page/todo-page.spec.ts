import { ComponentFixture, TestBed } from '@angular/core/testing';
import { TodoPage } from './todo-page';
import { TodoService } from '../../services/todo.service';
import { signal } from '@angular/core';
import { Todo } from '../../models/todo.model';
import { vi } from 'vitest';
import { provideRouter } from '@angular/router';

describe('TodoPage', () => {
  let fixture: ComponentFixture<TodoPage>;
  let todoService: {
    getTodos: ReturnType<typeof vi.fn>;
    addTodo: ReturnType<typeof vi.fn>;
    deleteTodo: ReturnType<typeof vi.fn>;
    updateTodo: ReturnType<typeof vi.fn>;
    updateTodoDone: ReturnType<typeof vi.fn>;
    todoList: ReturnType<typeof signal<Todo[]>>;
    error: ReturnType<typeof signal<string | null>>;
  };

  const mockTodos: Todo[] = [
    { id: '1', userId: 'test@test.com', description: 'Test todo', done: false }
  ];

  beforeEach(async () => {
    todoService = {
      getTodos: vi.fn(),
      addTodo: vi.fn(),
      deleteTodo: vi.fn(),
      updateTodo: vi.fn(),
      updateTodoDone: vi.fn(),
      todoList: signal(mockTodos),
      error: signal(null)
    };

    await TestBed.configureTestingModule({
      imports: [TodoPage],
      providers: [
        { provide: TodoService, useValue: todoService },
        provideRouter([])
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(TodoPage);
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(fixture.componentInstance).toBeTruthy();
  });

  it('should call getTodos on init', () => {
    expect(todoService.getTodos).toHaveBeenCalled();
  });

  it('should display todos from service', () => {
    const items = fixture.nativeElement.querySelectorAll('app-todo-item');
    expect(items.length).toBe(mockTodos.length);
  });

  it('should call addTodo when form is submitted with valid description', () => {
    const input = fixture.nativeElement.querySelector('input');
    input.value = 'New todo description';
    input.dispatchEvent(new Event('input'));
    fixture.detectChanges();

    const form = fixture.nativeElement.querySelector('form');
    form.dispatchEvent(new Event('submit'));
    fixture.detectChanges();

    expect(todoService.addTodo).toHaveBeenCalledWith('New todo description');
  });

  it('should not call addTodo when form is invalid', () => {
    const form = fixture.nativeElement.querySelector('form');
    form.dispatchEvent(new Event('submit'));
    fixture.detectChanges();

    expect(todoService.addTodo).not.toHaveBeenCalled();
  });
});