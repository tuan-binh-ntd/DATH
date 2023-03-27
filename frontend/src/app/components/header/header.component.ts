import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.less']
})
export class HeaderComponent implements OnInit {
  isVisible: boolean = false;
  constructor() { }

  ngOnInit(): void {
  }

  showModal(){
    this.isVisible = true;
  }

}
