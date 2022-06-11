import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { HttpHeaders } from '@angular/common/http';
import { from, Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { ModuleModel } from '../../models/ModuleModel';

@Injectable({
  providedIn: 'root'
})

export class ModuleService {
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
 
  UpdateModule(model: ModuleModel[]) {
/*    debugger;*/
    console.log(model);
    var res = this.http.post<any>(this.Url + "Module/PutMultiModule", model, {
      headers: new HttpHeaders().set('Authorization', "Bearer " + this.auth)
    })
    return res;
   
  }

}  
