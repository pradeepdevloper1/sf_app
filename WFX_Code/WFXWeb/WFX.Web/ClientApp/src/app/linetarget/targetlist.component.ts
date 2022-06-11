import { Component, Inject, OnInit } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Router } from '@angular/router';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-linetarget',
  templateUrl: './targetlist.component.html'
})

export class TargetListComponent implements OnInit {
  columnDefs = [
    { field: "module", minWidth: 100 },
    { field: "section", minWidth: 100 },
    { field: "line", minWidth: 100 },
    { field: "style", minWidth: 100 },
    { field: "soNo", minWidth: 100 },
    { field: "poNo", minWidth: 100 },
    { field: "part", minWidth: 100 },
    { field: "color", minWidth: 100 },
    { field: "smv", minWidth: 100 },
    { field: "operators", minWidth: 100 },
    { field: "helpers", minWidth: 100 },
    { field: "shiftName", minWidth: 100 },
    { field: "shiftHours", minWidth: 100 },
    { field: "date", minWidth: 100 },
    { field: "plannedEffeciency", minWidth: 100 },
    { field: "plannedTarget", minWidth: 100 }
  ];

  rowData: any;

  UserName: string;
  Url: string;
  header: any;
  auth: string;

  constructor(private http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    this.loadScripts();
    this.UserName = sessionStorage.getItem('userFirstName') + " " + sessionStorage.getItem('userLastName').replace("-", "");
    this.Url = environment.apiUrl;
    this.auth = sessionStorage.getItem('auth');
  }

  ngOnInit() {
    this.rowData = this.http.get(this.Url + "LineTarget/GetLineTargetList", {
      headers: new HttpHeaders().set('Authorization', "Bearer " + this.auth)
    })
  }


  // Method to dynamically load JavaScript
  loadScripts() {
    const dynamicScriptsbody = [
      'assets/js/custom.js',
    ];

    for (let i = 0; i < dynamicScriptsbody.length; i++) {
      const node = document.createElement('script');
      node.src = dynamicScriptsbody[i];
      node.type = 'text/javascript';
      node.async = false;
      document.getElementsByTagName('body')[0].appendChild(node);
    }
  }
}

