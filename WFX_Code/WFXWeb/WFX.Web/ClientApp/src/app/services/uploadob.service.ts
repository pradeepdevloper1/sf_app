import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { HttpHeaders } from '@angular/common/http';
import { environment } from 'src/environments/environment';
import { ProcessDefinitionModel } from 'src/models/ProcessDefinitionModel';
import { OBFileUpload } from 'src/models/OBFileUpload';

@Injectable({
  providedIn: 'root'
})

export class UploadOBService {
  Url: string;
  auth: string;

  constructor(private http: HttpClient) {
    this.Url = environment.apiUrl;
    this.auth = sessionStorage.getItem('auth');
  }

  GetOBFileUploadList(opt: { ProductID: number, ProcessCode: string }) {
    var res = this.http.post<any>(this.Url + "UploadOB/GetOBFileUploadList", opt, {
      headers: new HttpHeaders().set('Authorization', "Bearer " + this.auth)
    })
    return res;
  }

  GetOBFileUploadData(OBFileUpdateID = 0) {
    var res = this.http.get<any>(this.Url + "UploadOB/GetOBFileUpdateData/" + OBFileUpdateID, {
      headers: new HttpHeaders().set('Authorization', "Bearer " + this.auth)
    })
    return res;
  }


  PostOBFileUpload(data: OBFileUpload) {
    var res = this.http.post<any>(this.Url + "UploadOB/PostOBFileUpload", data, {
      headers: new HttpHeaders().set('Authorization', "Bearer " + this.auth)
    })
    return res;
  }

  PutMultiOBFileUploadData(data: any[]) {
    var res = this.http.post<any>(this.Url + "UploadOB/PutMultiOBFileUploadData", data, {
      headers: new HttpHeaders().set('Authorization', "Bearer " + this.auth)
    })
    return res;
  }
}
