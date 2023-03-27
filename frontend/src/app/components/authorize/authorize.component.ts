import { Component, Input, OnInit } from '@angular/core';
import { FormGroup } from '@angular/forms';

@Component({
  selector: 'app-authorize',
  templateUrl: './authorize.component.html',
  styleUrls: ['./authorize.component.less']
})
export class AuthorizeComponent implements OnInit {
  @Input() isVisible: boolean = false;
  form!: FormGroup
  constructor() { }

  ngOnInit(): void {
  }

}
