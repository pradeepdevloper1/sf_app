import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class PlanService {
  Url: string;
  header: any;
  auth: string;

  constructor(private http: HttpClient) {
    this.Url = environment.apiUrl;
    this.auth = sessionStorage.getItem('auth');
    /*alert(this.auth);*/
  }

  GetLineTargetList(model: any) {
    /* debugger;*/
    //alert("GetLineTargetList service call");
    var res = this.http.post<any>(this.Url + "LineTarget/GetLineTargetList", model, {
      headers: new HttpHeaders().set('Authorization', "Bearer " + this.auth)
    })
    return res;
  }

  GetLineBookingList(model: any) {
    /* debugger;*/
    //alert("GetLineTargetList service call");
    var res = this.http.post<any>(this.Url + "LineBooking/GetLineBookingList", model, {
      headers: new HttpHeaders().set('Authorization', "Bearer " + this.auth)
    })
    return res;
  }

  FillRunningPO() {
    /* debugger;*/
    var res = this.http.get<any>(this.Url + "Order/FillRunningPO", {
      headers: new HttpHeaders().set('Authorization', "Bearer " + this.auth)
    })
    return res;
  }

  FillLine() {
    /* debugger;*/
    var res = this.http.get<any>(this.Url + "Line/FillLine", {
      headers: new HttpHeaders().set('Authorization', "Bearer " + this.auth)
    })
    return res;
  }

  FillModule() {
    /* debugger;*/
    var res = this.http.get<any>(this.Url + "Module/FillModule",  {
      headers: new HttpHeaders().set('Authorization', "Bearer " + this.auth)
    })
    return res;
  }
  PutEditedLineTarget(model: any){
    var res = this.http.post<any>(this.Url + "LineTarget/PutEditedLineTarget", model, {
      headers: new HttpHeaders().set('Authorization', "Bearer " + this.auth)
    })
    return res;
  }
  checkLineExist(LineName: any){
    var res = this.http.post<any>(this.Url + "Line/PostIsLineExist",LineName, {
      headers: new HttpHeaders().set('Authorization', "Bearer " + this.auth)
    })
    return res;
  }
  checkShiftNameExist(ShiftName: any){
    var res = this.http.post<any>(this.Url + "Order/PostIsShiftExist",ShiftName, {
      headers: new HttpHeaders().set('Authorization', "Bearer " + this.auth)
    })
    return res;
  }
}


