import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { HttpHeaders } from '@angular/common/http';
import { from, Observable } from 'rxjs';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})

export class CulsterService {
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
  ClusterView(clusterid: number) {
    /* debugger;*/
    var res = this.http.get<any>(this.Url + "Cluster/CulsterView/" + clusterid, {
      headers: new HttpHeaders().set('Authorization', "Bearer " + this.auth)
    })
    return res;
  }
  SaveCulster(model: any) {
/*    debugger;*/

    var res = this.http.post<any>(this.Url + "Cluster/PostCluster", model, {
      headers: new HttpHeaders().set('Authorization', "Bearer " + this.auth)
    })
    return res;
   
  }

  FillOrganisation() {
    /* debugger;*/
    var res = this.http.get<any>(this.Url + "Organisation/FillOrganisation", {
      headers: new HttpHeaders().set('Authorization', "Bearer " + this.auth)
    })
    return res;
  }

  UpdateCulster(model: any) {
    /*    debugger;*/

    var res = this.http.post<any>(this.Url + "Cluster/PutCluster", model, {
      headers: new HttpHeaders().set('Authorization', "Bearer " + this.auth)
    })
    return res;
  }

}  
