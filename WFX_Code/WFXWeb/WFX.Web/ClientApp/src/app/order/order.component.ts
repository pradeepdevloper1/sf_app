import { Component, Inject, OnInit, ViewChild } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Router } from '@angular/router';
import { Observable } from 'rxjs';
import { AgGridAngular } from 'ag-grid-angular';
import { FormBuilder, FormGroup, FormsModule, Validators } from '@angular/forms';
import { environment } from '../../environments/environment';
import { bytesToSize as funcbytesToSize } from '../../models/helper';

@Component({
  selector: 'app-order',
  templateUrl: './order.component.html'
})

export class OrderComponent implements OnInit {
  selecttedFile = null;
  userID: number = 1;
  Url: string;
  UserName: string;
  isInvalid: number;
  OrderExcelFileName: string;
  OrderExcelFileSize: string;

  constructor(private router: Router, private http: HttpClient) {
    this.Url = environment.apiUrl;
    //this.UserName = sessionStorage.getItem('userFirstName') + " " + sessionStorage.getItem('userLastName').replace("-", "");
    //this.userID = parseInt(sessionStorage.getItem('userID'));
    this.isInvalid = 0;
  }

  ngOnInit() {

  }
  handleFileInput(event) {
    this.selecttedFile = <File>event.target.files[0];
    this.OrderExcelFileName = this.selecttedFile.name;
    if(this.OrderExcelFileName.length>25 && this.OrderExcelFileName.includes('xlsx')){
      this.OrderExcelFileName=(this.OrderExcelFileName.slice(0,16)+"...").concat(".xlsx");
    }
    else{
      this.OrderExcelFileName
    }
    this.OrderExcelFileSize = funcbytesToSize(this.selecttedFile.size);
    if (!this.OrderExcelFileName.includes('xlsx')) {
      this.isInvalid = 1;
      this.router.navigate(['/User/Order']);
    }
    else {
      this.isInvalid = 0;
      this.onUpload();
    }
  }

  SampleFile() {
    window.location.href = 'sample/ordersample.xlsx';
  }

  onUpload() {
    /*alert("upload call");*/
    const endpoint = 'Upload/Excel/UploadOrder/' + this.userID.toString();
    const formData: FormData = new FormData();
    formData.append('file', this.selecttedFile, this.OrderExcelFileName);
    var orderexcelfilesize = funcbytesToSize(this.selecttedFile.size);
    /*alert("orderexcelfilesize" + orderexcelfilesize);*/
    /*alert(endpoint);*/
    this.http.post<any>(endpoint, formData)
      .subscribe(res => {
        if (res.status = 200) {
          sessionStorage.setItem("orderexcelfilename", res.message);
          sessionStorage.setItem("orderexcelfilepath", res.path);
          sessionStorage.setItem("orderexcelfilesize", orderexcelfilesize);
          this.router.navigate(['/User/OrderUpload']);
        }
      });
  }
}

