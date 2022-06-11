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
  selector: 'app-linebooking',
  templateUrl: './savebooking.component.html'
})

export class SaveBookingComponent implements OnInit {
  Url: string;
  UserName: string;
  ExcelFileName: string;
  auth: string;
  saveRes: string;

  constructor(private router: Router, private http: HttpClient) {
    this.loadScripts();
    this.Url = environment.apiUrl;
    this.auth = sessionStorage.getItem('auth');

    this.UserName = sessionStorage.getItem('userFirstName') + " " + sessionStorage.getItem('userLastName').replace("-", "");
    this.ExcelFileName = sessionStorage.getItem("linebookingfilename");
  }

  async Save() {
   /* alert("Save Call");*/
    var res = getlist();
    alert(res.message);
    if (res.status == 200) {
      this.router.navigate(['/User/BookingList']);
    } 
  }

  Cancel() {
    this.router.navigate(['/User/BookingList']);
  }

  ngOnInit() {
  }

  next() {
    nexterror();
  }

  previous() {
    prverror();
  }

  // Method to dynamically load JavaScript
  loadScripts() {
     const dynamicScriptsbody = [
    'https://unpkg.com/@ag-grid-enterprise/all-modules@25.1.0/dist/ag-grid-enterprise.min.js',
      'assets/js/w_linebooking.js',
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

