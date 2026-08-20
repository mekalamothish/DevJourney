import { ChangeDetectionStrategy, Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ThemeService, type Theme } from '../../../core/services/theme.service';

@Component({
  selector: 'dj-theme-toggle',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="flex items-center gap-1 rounded-lg bg-gray-100 dark:bg-gray-800 p-1">
      @for (option of themeOptions; track option.value) {
        <button
          (click)="themeService.setTheme(option.value)"
          [class.active]="themeService.getTheme() === option.value"
          [attr.title]="option.label"
          class="px-3 py-2 rounded-md text-sm font-medium transition-all duration-200
                 text-gray-600 dark:text-gray-400 hover:text-gray-900 dark:hover:text-white
                 hover:bg-white dark:hover:bg-gray-700
                 active:bg-blue-500 active:text-white"
        >
          {{ option.icon }}
        </button>
      }
    </div>
  `,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ThemeToggle {
  themeOptions = [
    { value: 'light' as Theme, label: 'Light Theme', icon: '☀️' },
    { value: 'dark' as Theme, label: 'Dark Theme', icon: '🌙' },
    { value: 'system' as Theme, label: 'System Theme', icon: '🖥️' }
  ];

  constructor(public themeService: ThemeService) {}
}
