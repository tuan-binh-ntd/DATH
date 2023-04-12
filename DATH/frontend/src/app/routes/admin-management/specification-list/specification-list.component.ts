import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-specification-list',
  templateUrl: './specification-list.component.html',
  styleUrls: ['./specification-list.component.less']
})
export class SpecificationListComponent implements OnInit {

  listOfData:any[] = [];
  constructor() { }

  ngOnInit(): void {
    this.listOfData = new Array(100).fill(0).map((_, index) => ({
      id: index,
      name: `Edward King ${index}`,
      age: 32,
      address: `London, Park Lane no. ${index}`,
      disabled: index % 2 === 0
    }));
  }
}
