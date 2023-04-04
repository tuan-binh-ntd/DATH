import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';

@Component({
  selector: 'app-button-icon',
  templateUrl: './button-icon.component.html',
  styleUrls: ['./button-icon.component.less']
})
export class ButtonIconComponent implements OnInit {
  @Input() titleTooltip: string = "";
  @Input() iconClass: string = "";
  @Input() toolTipTitle: string = "";
  @Input() placement: string = "";
  
  
  @Output() onClick = new EventEmitter();

  constructor() { }

  ngOnInit(): void {
  }

  handleClick(): any {
    this.onClick.emit();
  }

}
