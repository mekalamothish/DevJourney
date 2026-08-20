import { ApplicationConfig, APP_INITIALIZER, provideBrowserGlobalErrorListeners } from '@angular/core';
import { provideRouter, withComponentInputBinding } from '@angular/router';
import { provideHttpClient } from '@angular/common/http';
import { ThemeService } from './core/services/theme.service';
import { routes } from './app.routes';

export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideRouter(routes, withComponentInputBinding()),
    provideHttpClient(),
    ThemeService,
    {
      provide: APP_INITIALIZER,
      useFactory: (themeService: ThemeService) => () => {
        // Initialize theme on app start
        const theme = themeService.getTheme();
        themeService.setTheme(theme);
      },
      deps: [ThemeService],
      multi: true,
    },
  ],
};
