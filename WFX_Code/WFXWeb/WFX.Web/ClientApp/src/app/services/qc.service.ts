import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class QCService {
  Url: string;
  header: any;
  auth: string;

  constructor(private http: HttpClient) {
    this.Url = environment.apiUrl;
    this.auth = sessionStorage.getItem('auth');
  }

  Performance(model: any) {
   /* debugger;*/
    var res = this.http.post<any>(this.Url + "QC/Performance", model, {
      headers: new HttpHeaders().set('Authorization', "Bearer " + this.auth)
    })
    return res;
  }

FillProcess(model:any){
  var res = this.http.post<any>(this.Url + "Order/FactoryProcessList", model, {
    headers: new HttpHeaders().set('Authorization', "Bearer " + this.auth)
  })
  return res;
}

FillProduct(model:any) {
    /* debugger;*/
    var res = this.http.post<any>(this.Url + "Product/FillProduct", model, {
      headers: new HttpHeaders().set('Authorization', "Bearer " + this.auth)
    })
    return res;
  }

FillProductFit(model:any) {
    /* debugger;*/
    var res = this.http.post<any>(this.Url + "ProductFit/FillProductFit", model,  {
      headers: new HttpHeaders().set('Authorization', "Bearer " + this.auth)
    })
    return res;
  }

  FillStyle(model:any) {
    /* debugger;*/
    var res = this.http.post<any>(this.Url + "Order/FillStyle",model,  {
      headers: new HttpHeaders().set('Authorization', "Bearer " + this.auth)
    })
    return res;
  }

  FillSeason(model:any) {
    /* debugger;*/
    var res = this.http.post<any>(this.Url + "Order/FillSeason", model, {
      headers: new HttpHeaders().set('Authorization', "Bearer " + this.auth)
    })
    return res;
  }

  FillCustomer(model:any) {
    /* debugger;*/
    var res = this.http.post<any>(this.Url + "Customer/FillCustomer", model, {
      headers: new HttpHeaders().set('Authorization', "Bearer " + this.auth)
    })
    return res;
  }

FillSO(model:any) {
    /* debugger;*/
    var res = this.http.post<any>(this.Url + "Order/FillSO", model, {
      headers: new HttpHeaders().set('Authorization', "Bearer " + this.auth)
    })
    return res;
  }

  FillModule(model:any) {
    /* debugger;*/
    var res = this.http.post<any>(this.Url + "Module/FillModule", model, {
      headers: new HttpHeaders().set('Authorization', "Bearer " + this.auth)
    })
    return res;
  }

    FillLine(model:any) {
    /* debugger;*/
    var res = this.http.post<any>(this.Url + "Line/FillLine", model,{
      headers: new HttpHeaders().set('Authorization', "Bearer " + this.auth)
    })
    return res;
  }

  POListOfSO(model: any) {
    /* debugger;*/
    var res = this.http.post<any>(this.Url + "Order/POListOfSO", model, {
      headers: new HttpHeaders().set('Authorization', "Bearer " + this.auth)
    })
    return res;
  }

FillPO(model:any) {
    /* debugger;*/
    var res = this.http.post<any>(this.Url + "Order/FillPO", model, {
      headers: new HttpHeaders().set('Authorization', "Bearer " + this.auth)
    })
    return res;
  }

  POOperationDefect(model: any) {
    /* debugger;*/
    var res = this.http.post<any>(this.Url + "QC/POOperationDefect", model, {
      headers: new HttpHeaders().set('Authorization', "Bearer " + this.auth)
    })
    return res;
  }
}


