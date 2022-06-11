import { Component, Inject, OnInit, ViewChild } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Router } from '@angular/router';
import { Observable } from 'rxjs';
import { AgGridAngular } from 'ag-grid-angular';
import { FormBuilder, FormGroup, FormsModule, Validators } from '@angular/forms';
import { environment } from '../../environments/environment';
import { bytesToSize as funcbytesToSize } from '../../models/helper';

@Component({
  selector: 'app-linetarget',
  templateUrl: './target.component.html'
})

export class TargetComponent implements OnInit {
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
    this.selecttedFile = <File>event.target.files[0];
    this.ExcelFileName = this.selecttedFile.name;
    if (this.ExcelFileName.length>25 && this.ExcelFileName.includes('xlsx')){
      this.ExcelFileName=(this.ExcelFileName.slice(0,16)+"...").concat(".xlsx");
        }
    else {
      this.ExcelFileName
    }
    this.ExcelFileSize = funcbytesToSize(this.selecttedFile.size);
    if (!this.ExcelFileName.includes('xlsx')) {
      this.isInvalid = 1;
      this.router.navigate(['/User/Target']);  
    }
    else {
      this.isInvalid = 0;
      this.onUpload();
    }
  }

  SampleFile() {
    window.location.href = 'sample/linetargetsample.xlsx';
  }

  onUpload() {
    /*alert("upload call");*/
    const endpoint = 'Upload/Excel/UploadLineTarget/' + this.userID.toString();
    const formData: FormData = new FormData();
    formData.append('file', this.selecttedFile,this.ExcelFileName);
    var linetargetfilesize = funcbytesToSize(this.selecttedFile.size);
    /*alert("linetargetfilesize" + linetargetfilesize);*/
    /*alert(endpoint);*/
    this.http.post<any>(endpoint, formData)
      .subscribe(res => {
        if (res.status = 200) {
          sessionStorage.setItem("linetargetfilename", res.message);
          sessionStorage.setItem("linetargetfilepath", res.path);
          sessionStorage.setItem("linetargetfilesize", linetargetfilesize);
          this.router.navigate(['/User/TargetUpload']);
        }
      });
  }
}

