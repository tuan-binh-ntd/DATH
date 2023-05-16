import { EntityState, EntityStore, StoreConfig } from '@datorama/akita';
import { Injectable } from '@angular/core';
import { Cart } from 'src/app/stores/cart/cart.model';

export interface CartState extends EntityState<Cart, any>{
  id: string | null,
  name: string | null,
  photo: string | null,
  specifications: any[],
  price: string | null,
  quantity: number | null,
}

@Injectable({ providedIn: 'root' })
@StoreConfig({ name: 'cart' })

export class CartStore extends EntityStore<CartState, Cart> {
  constructor() {
    super();
  }
}