import { ComponentFixture, TestBed } from '@angular/core/testing';
import { TodoItem } from './todo-item';
import { Todo } from '../../models/todo.model';
import { Component } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';

@Component({
  template: `<app-todo-item [todo]="todo" (deleteTodo)="onDelete($event)" />`,
  imports: [TodoItem],
  standalone: true
})
class TestHostComponent {
  todo: Todo = {
    id: '1',
    userId: 'test@test.com',
    description: 'Test todo',
    done: false
  };
  deletedId: string | null = null;
  onDelete(id: string) { this.deletedId = id; }
}

describe('TodoItem', () => {
  let fixture: ComponentFixture<TestHostComponent>;
  let host: TestHostComponent;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TestHostComponent, ReactiveFormsModule]
    }).compileComponents();

    fixture = TestBed.createComponent(TestHostComponent);
    host = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(fixture.componentInstance).toBeTruthy();
  });

  it('should show description text', () => {
    const span = fixture.nativeElement.querySelector('span');
    expect(span.textContent).toContain('Test todo');
  });

  it('should activate edit mode when edit button is clicked', () => {
    const editButton = fixture.nativeElement.querySelector('button');
    editButton.click();
    fixture.detectChanges();
    const form = fixture.nativeElement.querySelector('form');
    expect(form).toBeTruthy();
  });

  it('should call window.confirm before deleting', () => {
    vi.spyOn(window, 'confirm').mockReturnValue(false);
    const buttons = fixture.nativeElement.querySelectorAll('button');
    const deleteButton = buttons[1];
    deleteButton.click();
    expect(window.confirm).toHaveBeenCalled();
  });

  it('should emit deleteTodo when confirm is accepted', () => {
    vi.spyOn(window, 'confirm').mockReturnValue(true);
    const buttons = fixture.nativeElement.querySelectorAll('button');
    const deleteButton = buttons[1];
    deleteButton.click();
    expect(host.deletedId).toBe('1');
  });

  it('should not emit deleteTodo when confirm is cancelled', () => {
    vi.spyOn(window, 'confirm').mockReturnValue(false);
    const buttons = fixture.nativeElement.querySelectorAll('button');
    const deleteButton = buttons[1];
    deleteButton.click();
    expect(host.deletedId).toBeNull();
  });
});