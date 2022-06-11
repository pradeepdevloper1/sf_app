import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { HttpHeaders } from '@angular/common/http';
import { from, Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { ShiftModel } from '../../models/ShiftModel';

@Injectable({
  providedIn: 'root'
})

export class ShiftService {
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
 
  UpdateShift(model: ShiftModel[]) {
/*    debugger;*/
    console.log(model);
    var res = this.http.post<any>(this.Url + "Shift/PutMultiShift", model, {
      headers: new HttpHeaders().set('Authorization', "Bearer " + this.auth)
    })
    return res;
   
  }

}  
