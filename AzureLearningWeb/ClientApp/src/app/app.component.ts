import { Component, OnInit } from '@angular/core';
import { ConfigService } from '@services/config.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html'
})
export class AppComponent implements OnInit {
  public title = 'app';

  constructor(private readonly configService: ConfigService) {
  }

  public ngOnInit() {
  }
}
