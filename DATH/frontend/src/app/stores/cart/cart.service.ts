import { Injectable } from "@angular/core";
import { Cart } from "src/app/stores/cart/cart.model";
import { CartState, CartStore } from "./cart.store";

@Injectable({ providedIn: 'root' })
export class CartService {
  constructor(private cartStore: CartStore) {}
  insert(cart: Cart){
    this.cartStore.setLoading(true);
    this.cartStore.add(cart);
    this.cartStore.setLoading(false);
  }
  update(id: string, cart: Partial<CartState>) {
    this.cartStore.update(id, cart);
  }  

  delete(id: string){
    this.cartStore.remove(id);
  } 

  removeAll(){
    this.cartStore.remove();
  }
}