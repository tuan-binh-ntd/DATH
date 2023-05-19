import { Injectable } from "@angular/core";
import { Cart } from "src/app/stores/cart/cart.model";
import { CartState, CartStore } from "./cart.store";

@Injectable({ providedIn: 'root' })
export class CartService {
  constructor(private cartStore: CartStore) {}
  insert(cart: Cart){
    this.cartStore.add(cart);
  }
  update(id: string, cart: Partial<CartState>) {
    this.cartStore.update(id, cart);
  }  

  removeAll(){
    this.cartStore.remove();
  }
}