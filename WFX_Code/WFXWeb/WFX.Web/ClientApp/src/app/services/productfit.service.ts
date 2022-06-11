import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { HttpHeaders } from '@angular/common/http';
import { from, Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { ProductFitModel } from '../../models/ProductfitModel';

@Injectable({
  providedIn: 'root'
})

export class ProductFitService {
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
 
  UpdateProductFit(model: ProductFitModel[]) {
/*    debugger;*/
    console.log(model);
    var res = this.http.post<any>(this.Url + "ProductFit/PutMultiProductFit", model, {
      headers: new HttpHeaders().set('Authorization', "Bearer " + this.auth)
    })
    return res;
   
  }

}  
