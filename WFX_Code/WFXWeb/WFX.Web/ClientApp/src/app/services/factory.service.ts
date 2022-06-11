import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { HttpHeaders } from '@angular/common/http';
import { from, Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { FactoryModel } from '../../models/FactoryModel';
@Injectable({
  providedIn: 'root'
})

export class FactoryService {
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

  FillOrganisation() {
    /* debugger;*/
    var res = this.http.get<any>(this.Url + "Organisation/FillOrganisation", {
      headers: new HttpHeaders().set('Authorization', "Bearer " + this.auth)
    })
    return res;
  }

  FillCulster(orgid: number) {
    /* debugger;*/
    var res = this.http.get<any>(this.Url + "Cluster/FillCulster/" + orgid.toString(), {
      headers: new HttpHeaders().set('Authorization', "Bearer " + this.auth)
    })
    return res;
  }
  FillCountry() {
    /* debugger;*/
    var res = this.http.get<any>(this.Url + "Country/FillCountry", {
      headers: new HttpHeaders().set('Authorization', "Bearer " + this.auth)
    })
    return res;
  }
  FillFactoryType() {
    /* debugger;*/
    var res = this.http.get<any>(this.Url + "FactoryType/FillFactoryType", {
      headers: new HttpHeaders().set('Authorization', "Bearer " + this.auth)
    })
    return res;
  }
  FillLinkedwithERP() {
    var res = [{id:"YES",text:"YES"},{id:"NO",text:"NO"}];
    return res;
  }
  FillTimeZone(countryid: number) {
    /* debugger;*/
    var res = this.http.get<any>(this.Url + "TimeZone/FillTimeZone/" + countryid.toString(), {
      headers: new HttpHeaders().set('Authorization', "Bearer " + this.auth)
    })
    return res;
  }
  DeleteFactory(model: any) {
    /* debugger;*/
    var res = this.http.post<any>(this.Url + "Factory/DeleteFactory", model, {
      headers: new HttpHeaders().set('Authorization', "Bearer " + this.auth)
    })
    return res;
  }

  FactoryView(model: any) {
    /* debugger;*/
    var res = this.http.post<any>(this.Url + "Factory/FactoryView", model, {
      headers: new HttpHeaders().set('Authorization', "Bearer " + this.auth)
    })
    return res;
  }
  CheckProduct(model: any) {
    /* debugger;*/
    var res = this.http.post<any>(this.Url + "Product/FactoryView", model, {
      headers: new HttpHeaders().set('Authorization', "Bearer " + this.auth)
    })
    return res;
  }
  Update(model: any) {
    /*    debugger;*/
    var res = this.http.post<any>(this.Url + "Factory/PutFactory", model, {
      headers: new HttpHeaders().set('Authorization', "Bearer " + this.auth)
    })
    return res;

  }

  GetFactory(factoryid: number) {
    /* debugger;*/
    var res = this.http.get<any>(this.Url + "Factory/FactoryView/" + factoryid, {
      headers: new HttpHeaders().set('Authorization', "Bearer " + this.auth)
    })
    return res;
  }

  GetOrganisationID(clusterid: number) {
    /* debugger;*/
    var res = this.http.get<any>(this.Url + "Cluster/GetOrganisationID/" + clusterid.toString(), {
      headers: new HttpHeaders().set('Authorization', "Bearer " + this.auth)
    })
    return res;
  }
  GetExcelUpload(factoryid: number) {
    /* debugger;*/
    var res = this.http.get<any>(this.Url + "Factory/ExcelUpload/" + factoryid, {
      headers: new HttpHeaders().set('Authorization', "Bearer " + this.auth)
    })
    return res;
  }
}  
