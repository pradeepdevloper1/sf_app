import { Component, Inject, OnInit, ViewChild } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { ActivatedRoute, Router } from '@angular/router';
import { Observable } from 'rxjs';
import { FormBuilder, FormGroup, FormsModule, Validators } from '@angular/forms';

import { environment } from '../../environments/environment';

@Component({
  selector: 'app-linearget',
  templateUrl: './targetupload.component.html'
})

export class TargetUploadComponent implements OnInit {
  selecttedFile = null;
  userId: number = 1;
  uploadResponse = { status: '', message: '', filePath: '' };
  Url: string;
  UserName: string;
  ExcelFileName: string;
  ExcelFileSize: string;
  pono:string;
  constructor(private router: Router, private http: HttpClient, private route: ActivatedRoute) {
    this.Url = environment.apiUrl;
    this.UserName = sessionStorage.getItem('userFirstName') + " " + sessionStorage.getItem('userLastName').replace("-", "");
    this.ExcelFileName = sessionStorage.getItem("linetargetfilename");
    this.ExcelFileSize = sessionStorage.getItem("linetargetfilesize");
    this.pono = this.route.snapshot.params.pono;
  }

  ngOnInit() { }

  SaveTarget() {

    if(this.pono){
      this.router.navigate(['/User/SaveTarget',this.pono]);
    } else{
    this.router.navigate(['/User/SaveTarget']);
    }
  }

  Cancel() {
      if(this.pono){
        this.router.navigate(['/User/PlanEditor',this.pono]);
      } else{
      this.router.navigate(['/User/BookingList']);
      }
  }
}

