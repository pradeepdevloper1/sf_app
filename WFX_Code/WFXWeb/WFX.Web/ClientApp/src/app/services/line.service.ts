import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { HttpHeaders } from '@angular/common/http';
import { from, Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { LineModel } from '../../models/LineModel';

import { POPlanTargetModel } from 'src/models/POPlanTargetModel';



@Injectable({
  providedIn: 'root'
})

export class LineService {
  Url: string;
  token: string;
  header: any;
  auth: string;
  //constructor(private http: HttpClient) {
  //  this.Url = environment.apiUrl;
  //}
  constructor(private http: HttpClient) {
    this.Url = environment.apiUrl;
    this.auth = sessionStorage.getItem('auth');
  }

  UpdateLine(model: LineModel[]) {
/*    debugger;*/
    console.log(model);
    var res = this.http.post<any>(this.Url + "Line/PutMultiLine", model, {
      headers: new HttpHeaders().set('Authorization', "Bearer " + this.auth)
    })
    return res;

  }
  GetLineTargetExcelData(model: any){
    var res = this.http.post<any>(this.Url + "LineTarget/GetLineTargetExcelData",model, {
      headers: new HttpHeaders().set('Authorization', "Bearer " + this.auth)
    })
    return res;
  }
  GetPOforLineTarget(model: POPlanTargetModel){
    var res = this.http.post<any>(this.Url + "LineTarget/GetPOforLineTarget",model, {
      headers: new HttpHeaders().set('Authorization', "Bearer " + this.auth)
    })
    return res;
  }
}
