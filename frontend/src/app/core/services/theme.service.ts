import { Injectable, signal } from '@angular/core';

export type Theme = 'light' | 'dark' | 'system';

@Injectable({
  providedIn: 'root'
})
export class ThemeService {
  private readonly THEME_KEY = 'devjourney-theme';
  
  protected readonly currentTheme = signal<Theme>(this.loadTheme());
  protected readonly isDarkMode = signal(this.getIsDarkMode());

  constructor() {
    // Listen for system theme changes
    if (typeof window !== 'undefined' && window.matchMedia) {
      const mediaQuery = window.matchMedia('(prefers-color-scheme: dark)');
      mediaQuery.addEventListener('change', () => {
        this.isDarkMode.set(this.getIsDarkMode());
        this.applyTheme();
      });
    }
  }

  private loadTheme(): Theme {
    if (typeof localStorage === 'undefined') return 'system';
    const saved = localStorage.getItem(this.THEME_KEY) as Theme | null;
    return saved || 'system';
  }

  private getIsDarkMode(): boolean {
    const theme = this.currentTheme();
    if (theme === 'dark') return true;
    if (theme === 'light') return false;
    if (typeof window === 'undefined') return false;
    return window.matchMedia('(prefers-color-scheme: dark)').matches;
  }

  private applyTheme(): void {
    const isDark = this.isDarkMode();
    const html = document.documentElement;
    const body = document.body;

    // Apply to both <html> and <body> to match styles that target body.dark
    if (isDark) {
      html.classList.add('dark');
      if (body) body.classList.add('dark');
    } else {
      html.classList.remove('dark');
      if (body) body.classList.remove('dark');
    }
  }

  setTheme(theme: Theme): void {
    this.currentTheme.set(theme);
    if (typeof localStorage !== 'undefined') {
      localStorage.setItem(this.THEME_KEY, theme);
    }
    this.isDarkMode.set(this.getIsDarkMode());
    this.applyTheme();
  }

  getTheme(): Theme {
    return this.currentTheme();
  }

  getIsDark(): boolean {
    return this.isDarkMode();
  }
}
