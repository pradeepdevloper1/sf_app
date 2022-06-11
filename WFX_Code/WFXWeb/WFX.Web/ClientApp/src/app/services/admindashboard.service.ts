import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { HttpHeaders } from '@angular/common/http';
import { from, Observable } from 'rxjs';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class AdminDashboardService {
  Url: string;
  header: any;
  auth: string;

  constructor(private http: HttpClient ) {
    this.Url = environment.apiUrl;  
    this.auth = sessionStorage.getItem('auth');
  }

  GetAdminDashboard() {
  /*    debugger;*/
    var res = this.http.get<any>(this.Url + "Organisation/AdminDashboard", {
      headers: new HttpHeaders().set('Authorization', "Bearer " + this.auth)
    })
    return res;
  }
}  
