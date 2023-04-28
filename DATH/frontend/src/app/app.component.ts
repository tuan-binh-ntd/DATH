import { Component, HostListener, ViewEncapsulation } from '@angular/core';
import { ThemeService, ThemeType } from './services/theme.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.less'],

})
export class AppComponent {
  constructor(private themeService: ThemeService){}
  title = 'frontend';
  ngOnInit(){
    // this.themeService.currentTheme = window.localStorage.getItem("theme") as ThemeType;
    // if (this.themeService.currentTheme) {
    //   this.themeService.loadTheme(true);
    // }
  }
  
  @HostListener('window:unload', ['$event'])
  unloadHandler(event:any) {
    if (this.themeService.currentTheme) {
      window.localStorage.setItem("theme", this.themeService.currentTheme);
    }
  }
}


