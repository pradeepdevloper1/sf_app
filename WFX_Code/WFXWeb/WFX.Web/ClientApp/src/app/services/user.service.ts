import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { HttpHeaders } from '@angular/common/http';
import { from, Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { UserModel } from '../../models/UserModel';

@Injectable({
  providedIn: 'root'
})

export class UserService {
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

  UpdateUser(model: UserModel[]) {
/*    debugger;*/
    console.log(model);
    var res = this.http.post<any>(this.Url + "User/PutMultiUser", model, {
      headers: new HttpHeaders().set('Authorization', "Bearer " + this.auth)
    })
    return res;

  }

  FillUserRole() {
    /* debugger;*/
    var res = this.http.get<any>(this.Url + "User/FillUserRole", {
      headers: new HttpHeaders().set('Authorization', "Bearer " + this.auth)
    })
    return res;
  }
  FillUserListModule(factoryid: number) {
    /* debugger;*/
    var res = this.http.get<any>(this.Url + "User/FillUserListModule/"+factoryid,{
      headers: new HttpHeaders().set('Authorization', "Bearer " + this.auth)
    })
    return res;
  }
}
