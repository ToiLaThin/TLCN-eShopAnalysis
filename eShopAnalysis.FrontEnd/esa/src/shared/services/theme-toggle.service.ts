import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root' //ko cần import export ở shared module
})
export class ThemeToggleService {

    isDarkTheme$!: BehaviorSubject<boolean> 
    themeKey: string = 'isDarkTheme';

    constructor() { 
        //get theme state from local storage
        const themeStateJSON = window.localStorage.getItem(this.themeKey);
        if (themeStateJSON) {
            this.isDarkTheme$ = new BehaviorSubject<boolean>(JSON.parse(themeStateJSON));
        }
        else {
            this.isDarkTheme$ = new BehaviorSubject<boolean>(false);
        }
    }

    toggleTheme() { 
        const currentThemeState = this.isDarkTheme$.getValue();
        //toggle theme set next theme state to local storage
        window.localStorage.setItem(this.themeKey, JSON.stringify(!currentThemeState));
        const nextThemeState = !currentThemeState;
        this.isDarkTheme$.next(nextThemeState);
    }
}
