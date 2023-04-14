import { Component, EventEmitter, Input, Output } from '@angular/core';
import { NzButtonType } from 'ng-zorro-antd/button';

@Component({
  selector: 'app-button-base',
  templateUrl: './button-base.component.html',
  styleUrls: ['./button-base.component.less']
})
export class ButtonBaseComponent {
  @Input() text = '';
  @Input() iconClass = '';
  @Input() loading = false;
  @Input() disabled = false;
  @Input() type: NzButtonType = 'primary';
  @Input() buttonClass = 'mr-2';
  @Output() onClick = new EventEmitter();


  handleClick(): any {
    this.onClick.emit();
  }
}
