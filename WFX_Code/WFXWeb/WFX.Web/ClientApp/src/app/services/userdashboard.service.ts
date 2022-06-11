import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { HttpHeaders } from '@angular/common/http';
import { from, Observable } from 'rxjs';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class UserDashboardService {
  Url: string;
  header: any;
  auth: string;

  constructor(private http: HttpClient ) {
    this.Url = environment.apiUrl;  
    this.auth = sessionStorage.getItem('auth');
  }

  GetUserDashboard(model: any) {
    //alert(model);
    /* debugger;*/
    var res = this.http.post<any>(this.Url + "UserDashboard/GetUserDashboard", model,{
      headers: new HttpHeaders().set('Authorization', "Bearer " + this.auth)
    })
    return res;
  }

  FillModule() {
    /* debugger;*/
    var res = this.http.get<any>(this.Url + "Module/FillModule", {
      headers: new HttpHeaders().set('Authorization', "Bearer " + this.auth)
    })
    return res;
  }

  OrderList(model: any) {
    /* debugger;*/
    var res = this.http.post<any>(this.Url + "Order/OrderListForUserDashboard", model, {
      headers: new HttpHeaders().set('Authorization', "Bearer " + this.auth)
    })
    return res;
  }
}  
