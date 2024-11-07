import { Component, OnInit } from '@angular/core';
import {MatTabsModule} from '@angular/material/tabs';
import { Router } from '@angular/router';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrl: './app.component.css',
})

export class AppComponent implements OnInit{
  links: any[];

  title = 'web.client';
  activeLink = -1;

  constructor(private router: Router) {
    this.links = [
      { label: 'Current', link: './current', index: 0 },
      { label: 'Temperature and Humidity', link: './temperature-humidity', index: 1 },
      { label: 'Pressure', link: './pressure', index: 2 },
      { label: 'AirQuality', link: './air-quality', index: 3 }
    ];
  }
  ngOnInit(): void {
    this.router.events.subscribe((res) => {
      this.activeLink = this.links.indexOf(this.links.find(tab => tab.link === '.' + this.router.url));
    });
  }
}
