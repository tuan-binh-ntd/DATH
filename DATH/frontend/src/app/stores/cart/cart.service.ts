import { Injectable } from "@angular/core";
import { Cart } from "src/app/stores/cart/cart.model";
import { CartStore } from "./cart.store";

@Injectable({ providedIn: 'root' })
export class CartService {
  constructor(private cartStore: CartStore) {}
  insert(cart: Cart){
    this.cartStore.add(cart);
  }
  update(cart: Cart) {
    this.cartStore.update(cart);
  }  
}