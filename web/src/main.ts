import { bootstrapApplication } from '@angular/platform-browser';
import * as Sentry from '@sentry/angular';
import { appConfig } from './app/app.config';
import { AppComponent } from './app/app.component';

Sentry.init({
  dsn: 'https://6930e202ea0d816dd0b01ff8db953749@o1036693.ingest.us.sentry.io/4509189061148672'
});

bootstrapApplication(AppComponent, appConfig)
  .catch((err) => console.error(err));
