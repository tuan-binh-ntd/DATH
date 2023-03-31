import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';

@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.less']
})
export class HeaderComponent implements OnInit {
  @Input() isCollapsed: boolean = false;
  @Output() emitOnCollapse = new EventEmitter();
  isVisible: boolean = false;
  constructor() { }

  ngOnInit(): void {
  }

  showModal(){
    this.isVisible = true;
  }

  onCollapse(){
    this.isCollapsed = !this.isCollapsed;
    this.emitOnCollapse.emit(this.isCollapsed);
  }

}
