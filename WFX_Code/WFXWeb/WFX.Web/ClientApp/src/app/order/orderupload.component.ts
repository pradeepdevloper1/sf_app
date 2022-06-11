import { Component, Inject, OnInit, ViewChild } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Router } from '@angular/router';
import { Observable } from 'rxjs';
import { AgGridAngular } from 'ag-grid-angular';
import { FormBuilder, FormGroup, FormsModule, Validators } from '@angular/forms';
import { environment } from '../../environments/environment';

@Component({
  selector: 'app-order',
  templateUrl: './orderupload.component.html'
})

export class OrderUploadComponent implements OnInit {
  Url: string;
  UserName: string;
  OrderExcelFileName: string;
  OrderExcelFileSize: string;


  constructor(private router: Router, private http: HttpClient) {
    this.Url = environment.apiUrl;
    this.UserName = sessionStorage.getItem('userFirstName') + " " + sessionStorage.getItem('userLastName').replace("-", "");
    this.OrderExcelFileName = sessionStorage.getItem("orderexcelfilename");
  this.OrderExcelFileSize = sessionStorage.getItem("orderexcelfilesize");
  
  }

  ngOnInit() {

  }

  Saveorder() {
    this.router.navigate(['/User/SaveOrder']);
  }

  Cancel() {
    this.router.navigate(['/User/Order']);
  }
}

