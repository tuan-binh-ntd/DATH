import { Injectable } from '@angular/core';
import { QueryEntity } from '@datorama/akita';
import { CartState, CartStore } from './cart.store';
@Injectable({ providedIn: 'root' })
export class CartQuery extends QueryEntity<CartState> {
  constructor(protected cartStore: CartStore) {
    super(cartStore);
  }
}