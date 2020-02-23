import { Component, OnInit } from '@angular/core';
import { ConfigService as ConfigService1 } from '@services/config.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html'
})
export class AppComponent implements OnInit {
  public title = 'app';

  constructor(private readonly configService: ConfigService1) {
  }

  public ngOnInit() {
    this.configService.appSettings
      .subscribe(result => console.log(result));
    ;
  }
}
