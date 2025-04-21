import { bootstrapApplication } from '@angular/platform-browser';
import * as Sentry from '@sentry/angular';
import { appConfig } from './app/app.config';
import { AppComponent } from './app/app.component';
import { environment } from './environments/environment';

Sentry.init({
  dsn: environment.sentryDsn
});

bootstrapApplication(AppComponent, appConfig)
  .catch((err) => console.error(err));
