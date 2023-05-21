import { Component } from '@angular/core';
import { Observable } from 'rxjs';
import { Cart } from 'src/app/stores/cart/cart.model';
import { CartQuery } from 'src/app/stores/cart/cart.query';

@Component({
  selector: 'app-view-cart-detail',
  templateUrl: './view-cart-detail.component.html',
  styleUrls: ['./view-cart-detail.component.less']
})
export class ViewCartDetailComponent {
  listOfData: Cart[] = [];
  constructor(private cartQuery: CartQuery){};
  cartObjects$  = this.cartQuery.selectAll();


  listOfColumn: any[] = [
    {
      name: 'Image',
      width: '100px',
      class: 'text-left',
    },
    {
      name: 'Name',
      width: 'auto',
      sortKey: 'Balance',
      sortOrder: null,
      sortDirections: ['ascend', 'descend', null],
      class: 'text-left',
    },
    {
      name: 'Name',
      width: 'auto',
      sortKey: 'Name',
      sortOrder: null,
      sortDirections: ['ascend', 'descend', null],
      class: 'text-left',
    },
    {
      name: 'Price',
      width: '20%',
      sortKey: 'Price',
      sortOrder: null,
      sortDirections: ['ascend', 'descend', null],
      class: 'text-left',
    },
    {
      name: 'Quantity',
      width: '30%',
      sortKey: 'Price',
      sortOrder: null,
      sortDirections: ['ascend', 'descend', null],
      class: 'text-left',
    }
  ];

  onSort(direction: any, column: string){}

  deleteRecord(){}
}
