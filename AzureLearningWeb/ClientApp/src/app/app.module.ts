import { BrowserModule } from '@angular/platform-browser';
import { NgModule, APP_INITIALIZER } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpClientModule, HttpClient } from '@angular/common/http';
import { RouterModule } from '@angular/router';

import { AppComponent } from './app.component';
import { HomeComponent } from '@components/home/home.component';
import { CounterComponent } from '@components/counter/counter.component';
import { FetchDataComponent } from '@components/fetch-data/fetch-data.component';
import { NavMenuComponent } from '@components/nav-menu/nav-menu.component';
import { ConfigService as SettingsService } from '@app/services/config.service';

@NgModule({
  declarations: [
    AppComponent,
    NavMenuComponent,
    HomeComponent,
    CounterComponent,
    FetchDataComponent
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    HttpClientModule,
    FormsModule,
    RouterModule.forRoot([
      { path: '', component: HomeComponent, pathMatch: 'full' },
      { path: 'counter', component: CounterComponent },
      { path: 'fetch-data', component: FetchDataComponent },
    ])
  ],
  providers: [
    {
      provide: APP_INITIALIZER,
      useFactory: (settings: SettingsService) => () => settings.appSettings,
      deps: [HttpClient],
      multi: true
    }],
  bootstrap: [
    AppComponent]
})
export class AppModule { }
