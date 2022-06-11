import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { HttpHeaders } from '@angular/common/http';
import { from, Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { ProductModel } from '../../models/ProductModel';

@Injectable({
  providedIn: 'root'
})

export class ProductService {
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


  GetProductList(pm?: ProductModel) {
    var res = this.http.post<any>(this.Url + "Product/GetProductList", (pm ? pm : {}), {
      headers: new HttpHeaders().set('Authorization', "Bearer " + this.auth)
    })
    return res;
  }

  UpdateProduct(model: ProductModel[]) {
    var res = this.http.post<any>(this.Url + "Product/PutMultiProduct", model, {
      headers: new HttpHeaders().set('Authorization', "Bearer " + this.auth)
    })
    return res;
  }
}
