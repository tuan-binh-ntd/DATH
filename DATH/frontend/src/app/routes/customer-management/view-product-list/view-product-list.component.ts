import { Component } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-view-product-list',
  templateUrl: './view-product-list.component.html',
  styleUrls: ['./view-product-list.component.less']
})
export class ViewProductListComponent {
  constructor(private route: ActivatedRoute){}
  type: string | null = '';
  ngOnInit(){
    this.route.paramMap.subscribe(
      params => {
        this.type = params.get('type')?.toUpperCase()!;
      })
  }
  
}
