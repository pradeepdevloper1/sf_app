import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { HttpHeaders } from '@angular/common/http';
import { from, Observable } from 'rxjs';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})

export class LoginService {
  Url: string;
  token: string;
  header: any;
  constructor(private http: HttpClient) {
    this.Url = environment.apiUrl;  
  }

  Login(model: any) {
/*    debugger;*/
   var res = this.http.post<any>(this.Url + "User/Login", model);
    return res
  }
}  
