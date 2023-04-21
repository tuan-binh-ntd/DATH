import { Component } from '@angular/core';
import { Product } from 'src/app/models/product.model';
import { ProductService } from 'src/app/services/product.service';
import { checkResponseStatus } from 'src/app/shared/helper';

@Component({
  selector: 'app-customer-home',
  templateUrl: './customer-home.component.html',
  styleUrls: ['./customer-home.component.less']
})
export class CustomerHomeComponent {
  listOfData: Product[] = [];
  constructor(private productService: ProductService,){}
  ngOnInit():void{
    this.fetchData();
  }

  fetchData(){
    this.productService.getAll().subscribe(res => {
      if(checkResponseStatus(res)){
        this.listOfData = res.data;
      }
    })
  }
}
