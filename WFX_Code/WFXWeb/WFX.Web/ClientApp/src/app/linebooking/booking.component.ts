import { Component, Inject, OnInit, ViewChild } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Router } from '@angular/router';
import { Observable } from 'rxjs';
import { AgGridAngular } from 'ag-grid-angular';
import { FormBuilder, FormGroup, FormsModule, Validators } from '@angular/forms';
import { environment } from '../../environments/environment';
import { bytesToSize as funcbytesToSize } from '../../models/helper';

@Component({
  selector: 'app-linebooking',
  templateUrl: './booking.component.html'
})

export class BookingComponent implements OnInit {
  selecttedFile = null;
  userID: number = 0;
  Url: string;
  UserName: string;
  isInvalid: number;
  ExcelFileName: string;
  ExcelFileSize: string;

  constructor(private router: Router, private http: HttpClient) {
    this.Url = environment.apiUrl;
    this.UserName = sessionStorage.getItem('userFirstName') + " " + sessionStorage.getItem('userLastName').replace("-", "");
    this.userID = parseInt(sessionStorage.getItem('userID'));
    this.isInvalid = 0;
  }

  ngOnInit() { }

  handleFileInput(event) {
    console.log(event);
    this.selecttedFile = <File>event.target.files[0];
    this.ExcelFileName = this.selecttedFile.name;
    this.ExcelFileSize = funcbytesToSize(this.selecttedFile.size);
    console.log(this.ExcelFileName);
    console.log(this.ExcelFileSize);
    if (!this.ExcelFileName.includes('xlsx')) {
      this.isInvalid = 1;
      this.router.navigate(['/User/Booking']);
    }
    else {
      this.isInvalid = 0;
      this.onUpload();
    }
  }

  SampleFile() {
    window.location.href = 'sample/linebookingsample.xlsx';
  }

  onUpload() {
    /*alert("upload call");*/
    const endpoint = 'Upload/Excel/UploadLineBooking/' + this.userID.toString();
    const formData: FormData = new FormData();
    formData.append('file', this.selecttedFile, this.selecttedFile.name);
    var linebookingfilesize = funcbytesToSize(this.selecttedFile.size);
    /*alert("linebookingfilesize" + linebookingfilesize);*/
    /* alert(endpoint);*/
    this.http.post<any>(endpoint, formData)
      .subscribe(res => {
        console.log(res)
        if (res.status = 200) {
          sessionStorage.setItem("linebookingfilename", res.message);
          sessionStorage.setItem("linebookingfilepath", res.path);
          sessionStorage.setItem("linebookingfilesize", linebookingfilesize);
          /* alert(res.message);*/
          this.router.navigate(['/User/BookingUpload']);
        }
      });
  }
}

