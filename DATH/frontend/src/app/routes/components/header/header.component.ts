import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { Employee } from 'src/app/models/employee.model';

@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.less']
})
export class HeaderComponent implements OnInit {
  @Input() isCollapsed: boolean = false;
  @Output() emitOnCollapse = new EventEmitter();
  isVisible: boolean = false;
  employee!: Employee;
  constructor() { }

  ngOnInit(): void {
  }

  ngAfterViewInit(){
    this.employee = JSON.parse(localStorage.getItem('user')!);
  }

  showModal(){
    this.isVisible = true;
  }

  onCollapse(){
    this.isCollapsed = !this.isCollapsed;
    this.emitOnCollapse.emit(this.isCollapsed);
  }

}
