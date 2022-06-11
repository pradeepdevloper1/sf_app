import { Component, Inject, OnInit } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Router } from '@angular/router';
import { Observable } from 'rxjs';
import { AgGridAngular } from 'ag-grid-angular';
import { FormBuilder, FormGroup, FormsModule, Validators } from '@angular/forms';
import { environment } from '../../environments/environment';

declare function getlist(): any;
declare function nexterror(): any;
declare function prverror(): any;

@Component({
  selector: 'app-order',
  templateUrl: './saveorder.component.html'
})

export class SaveOrderComponent implements OnInit {
  Url: string;
  UserName: string;
  OrderExcelFileName: string;
  auth: string;
  isInvalid: number;
  selectedIndex: number;
  items: any[];
  constructor(private router: Router, private http: HttpClient) {
    this.loadScripts();
    this.Url = environment.apiUrl;
    this.auth = sessionStorage.getItem('auth');
    this.UserName = sessionStorage.getItem('userFirstName') + " " + sessionStorage.getItem('userLastName').replace("-", "");
    this.OrderExcelFileName = sessionStorage.getItem("orderexcelfilename");
    this.isInvalid = 0;
  }

  getOrderList() {
    var res = getlist();
    console.log("getOrderList res - " + res.message);
     alert("Order Uploaded Successfully");
    if (res.status == 200) {
      this.router.navigate(['/User/OrderList']);
    }   
  }

  Cancel() {
    this.router.navigate(['/User/Order']);
  }

  ngOnInit() {
  }

  next() {
    console.log("next click call");
    nexterror();
  }

  previous() {
    console.log("previous click call");
    prverror();
  }

  // Method to dynamically load JavaScript
  loadScripts() {
    const currTime=new Date().getTime();
     const dynamicScriptsbody = [
    'https://unpkg.com/@ag-grid-enterprise/all-modules@25.1.0/dist/ag-grid-enterprise.min.js',
       'assets/js/w_order.js?lm='+currTime,
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

