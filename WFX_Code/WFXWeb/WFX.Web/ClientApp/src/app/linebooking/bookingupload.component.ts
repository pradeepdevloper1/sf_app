import { Component, Inject, OnInit, ViewChild } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Router } from '@angular/router';
import { Observable } from 'rxjs';
import { FormBuilder, FormGroup, FormsModule, Validators } from '@angular/forms';

import { environment } from '../../environments/environment';

@Component({
  selector: 'app-linebooking',
  templateUrl: './bookingupload.component.html'
})

export class BookingUploadComponent implements OnInit {
  selecttedFile = null;
  userId: number = 1;
  uploadResponse = { status: '', message: '', filePath: '' };
  Url: string;
  UserName: string;
  ExcelFileName: string;
  ExcelFileSize: string;
  constructor(private router: Router, private http: HttpClient) {
    this.Url = environment.apiUrl;
    this.UserName = sessionStorage.getItem('userFirstName') + " " + sessionStorage.getItem('userLastName').replace("-", "");
    this.ExcelFileName = sessionStorage.getItem("linebookingfilename");
    this.ExcelFileSize = sessionStorage.getItem("linebookingfilesize");
  }

  ngOnInit() { }

  SaveTarget() {
    this.router.navigate(['/User/SaveBooking']);
  }

  Cancel() {
    this.router.navigate(['/User/BookingList']);
  }
}

