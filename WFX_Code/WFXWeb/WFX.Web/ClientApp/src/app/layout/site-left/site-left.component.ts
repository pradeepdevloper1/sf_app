import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'site-left',
  templateUrl: './site-left.component.html',
  styleUrls: ['./site-left.component.css']
})
export class SiteLeftComponent implements OnInit {
  selectedItem: string;
  constructor(private router: Router) { }

  ngOnInit(): void {
    const a = this.router.url.split('/');
    this.selectedItem = a[2];
    if (a[2] === 'POView' || a[2] === 'IMGEditor' || a[2] === 'PlanEditor' || a[2] === 'EditOrder' || a[2] === 'ProcessTemplate') {
      this.selectedItem = 'OrderList'
    }
  }
  onclick(e: string) {
    this.selectedItem = e;
  }
}
