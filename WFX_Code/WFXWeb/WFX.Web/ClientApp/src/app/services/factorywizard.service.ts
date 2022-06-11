import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { HttpHeaders } from '@angular/common/http';
import { from, Observable } from 'rxjs';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})

export class FactoryWizardService {
  Url: string;
  token: string;
  header: any;
  auth: string;
 
  constructor(private http: HttpClient) {
    this.Url = environment.apiUrl;
    this.auth = sessionStorage.getItem('auth');
  }

  Save(model: any) {
/*    debugger;*/

    var res = this.http.post<any>(this.Url + "Factory/PostFactory", model, {
      headers: new HttpHeaders().set('Authorization', "Bearer " + this.auth)
    })
    return res;
   
  }

 
}  
