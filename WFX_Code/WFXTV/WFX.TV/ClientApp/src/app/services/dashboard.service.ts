import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { HttpHeaders } from '@angular/common/http';
import { from, Observable } from 'rxjs';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class DashboardService {
  Url: string;
  header: any;
  auth: string;

  constructor(private http: HttpClient) {
    this.Url = environment.apiUrl;
    this.auth = localStorage.getItem('auth');
  }

  FillFactoryLine() {
    var res = this.http.get<any>(this.Url + "Line/FillFactoryLine", {
      headers: new HttpHeaders().set('Authorization', "Bearer " + this.auth)
    })
    return res;
  }

  FillFactoryModule() {
    var res = this.http.get<any>(this.Url + "Module/FillFactoryModule", {
      headers: new HttpHeaders().set('Authorization', "Bearer " + this.auth)
    })
    return res;
  }

  FillFactoryShift() {
    var res = this.http.get<any>(this.Url + "Shift/FillFactoryShift", {
      headers: new HttpHeaders().set('Authorization', "Bearer " + this.auth)
    })
    return res;
  }

  HourlyProduction(model: any) {
    var res = this.http.post<any>(this.Url + "TV/HourlyProduction", model, {
      headers: new HttpHeaders().set('Authorization', "Bearer " + this.auth)
    })
    return res;
  }

  DefectHotspot(model: any) {
    var res = this.http.post<any>(this.Url + "TV/DefectHotspot", model, {
      headers: new HttpHeaders().set('Authorization', "Bearer " + this.auth)
    })
    return res;
  }
}  
