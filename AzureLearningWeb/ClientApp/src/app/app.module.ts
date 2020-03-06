import { BrowserModule } from '@angular/platform-browser';
import { NgModule, APP_INITIALIZER } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { RouterModule } from '@angular/router';

import { MsalModule, MsalInterceptor, MsalGuard, MsalConfig } from '@azure/msal-angular';

import { AppComponent } from './app.component';
import { HomeComponent } from '@components/home/home.component';
import { CounterComponent } from '@components/counter/counter.component';
import { FetchDataComponent } from '@components/fetch-data/fetch-data.component';
import { NavMenuComponent } from '@components/nav-menu/nav-menu.component';
import { ConfigService as SettingsService } from '@app/services/config.service';
import { ProtectedComponent } from './components/protected/protected.component';

const AD_CONFIG = {
  clientID: 'bf91ad4e-2eeb-4201-9a4f-a7e8889cc5f5',
  authority: null,
  tokenReceivedCallback: null,
  validateAuthority: null,
  cacheLocation: null,
  storeAuthStateInCookie: null,
  redirectUri: null,
  postLogoutRedirectUri: null,
  logger: null,
  loadFrameTimeout: null,
  navigateToLoginRequestUrl: null,
  popUp: null,
  consentScopes: null,
  isAngular: null,
  unprotectedResources: null,
  protectedResourceMap: null,
  extraQueryParameters: null,
  correlationId: null,
  level: null,
  piiLoggingEnabled: null
};

@NgModule({
  declarations: [
    AppComponent,
    NavMenuComponent,
    HomeComponent,
    CounterComponent,
    FetchDataComponent,
    ProtectedComponent
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    HttpClientModule,
    FormsModule,
    MsalModule.forRoot({
      clientID: 'bf91ad4e-2eeb-4201-9a4f-a7e8889cc5f5'
    }),
    //    MsalModule.forRoot(AD_CONFIG),
    RouterModule.forRoot([
      { path: '', component: HomeComponent, pathMatch: 'full' },
      { path: 'counter', component: CounterComponent },
      { path: 'fetch-data', component: FetchDataComponent },
      { path: 'protected', component: ProtectedComponent, canActivate: [MsalGuard] }
    ])
  ],
  providers: [
    {
      provide: APP_INITIALIZER,
      useFactory: (settings: SettingsService) => () => settings.appSettings.subscribe(() => { }),
      deps: [SettingsService],
      multi: true
    },
    {
      provide: HTTP_INTERCEPTORS,
      useClass: MsalInterceptor,
      multi: true
    }],
  bootstrap: [
    AppComponent]
})
export class AppModule { }
