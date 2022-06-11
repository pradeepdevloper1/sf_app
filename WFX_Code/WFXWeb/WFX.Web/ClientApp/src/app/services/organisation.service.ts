import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { HttpHeaders } from '@angular/common/http';
import { from, Observable } from 'rxjs';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})

export class OrganisationService {
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

  Organisation(model: any) {
/*    debugger;*/

    var res = this.http.post<any>(this.Url + "Organisation/PostOrganisation", model, {
      headers: new HttpHeaders().set('Authorization', "Bearer " + this.auth)
    })
    return res;
   
  }
  OrganisationView(orgid: number) {
    /* debugger;*/
    var res = this.http.get<any>(this.Url + "Organisation/OrganisationView/"+ orgid, {
      headers: new HttpHeaders().set('Authorization', "Bearer " + this.auth)
    })
    return res;
  }

  UpdateOrganisation(model: any) {
    /*    debugger;*/

    var res = this.http.post<any>(this.Url + "Organisation/PutOrganisation", model, {
      headers: new HttpHeaders().set('Authorization', "Bearer " + this.auth)
    })
    return res;

  }
}  
