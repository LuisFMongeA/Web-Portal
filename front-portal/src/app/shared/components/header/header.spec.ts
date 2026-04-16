import { ComponentFixture, TestBed } from '@angular/core/testing';

import { Header } from './header';
import { provideRouter } from '@angular/router';

describe('Header', () => {
  let component: Header;
  let fixture: ComponentFixture<Header>;

  TestBed.configureTestingModule({
  imports: [Header],
  providers: [provideRouter([])]
});

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [Header],
    }).compileComponents();

    fixture = TestBed.createComponent(Header);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
