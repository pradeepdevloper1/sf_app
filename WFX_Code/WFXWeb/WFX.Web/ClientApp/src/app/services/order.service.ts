import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class OrderService {
  Url: string;
  header: any;
  auth: string;

  constructor(private http: HttpClient) {
    this.Url = environment.apiUrl;
    this.auth = sessionStorage.getItem('auth');
    /*alert(this.auth);*/
  }

  OrderList(model: any) {
    /* debugger;*/
/*    alert("OrderList in order service");*/
    var res = this.http.post<any>(this.Url + "Order/OrderList", model, {
      headers: new HttpHeaders().set('Authorization', "Bearer " + this.auth)
    })
    return res;
  }

  RequestStatusList() {
    var res = this.http.get<any>(this.Url + "Order/RequestStatusList",  {
      headers: new HttpHeaders().set('Authorization', "Bearer " + this.auth)
    })
    return res;
  }
  DeleteRequestStatus(model: any) {
    var res = this.http.post<any>(this.Url + "Order/DeleteRequestStatus", model, {
      headers: new HttpHeaders().set('Authorization', "Bearer " + this.auth)
    })
    return res;
  }
  SyncData(model: any) {
    var res = this.http.post<any>(this.Url + "Order/SyncData", model, {
      headers: new HttpHeaders().set('Authorization', "Bearer " + this.auth)
    })
    return res;
  }

  SOView(model: any) {
   /* debugger;*/
    var res = this.http.post<any>(this.Url + "Order/SOView", model, {
      headers: new HttpHeaders().set('Authorization', "Bearer " + this.auth)
    })
    return res;
  }

  POView(model: any) {
    /* debugger;*/
    var res = this.http.post<any>(this.Url + "Order/POView", model, {
      headers: new HttpHeaders().set('Authorization', "Bearer " + this.auth)
    })
    return res;
  }

  POColorSizeList(model: any) {
    /* debugger;*/
    var res = this.http.post<any>(this.Url + "Order/POColorSizeList", model, {
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

  GetPOListOfSO(SONo: string) {
    /* debugger;*/
    var res = this.http.get<any>(this.Url + "Order/GetPOListOfSO?SONo=" + SONo , {
      headers: new HttpHeaders().set('Authorization', "Bearer " + this.auth)
    })
    return res;
  }

  GetOrderStatusList() {
    /* debugger;*/

    var res = this.http.get<any>(this.Url + "Order/GetOrderStatusList", {
      headers: new HttpHeaders().set('Authorization', "Bearer " + this.auth)
    })
    return res;
  }

  DeletePO(model: any) {
    /* debugger;*/
    var res = this.http.post<any>(this.Url + "Order/DeletePO", model, {
      headers: new HttpHeaders().set('Authorization', "Bearer " + this.auth)
    })
    return res;
  }

  CompletePO(model: any) {
    /* debugger;*/
    var res = this.http.post<any>(this.Url + "Order/CompletePO", model, {
      headers: new HttpHeaders().set('Authorization', "Bearer " + this.auth)
    })
    return res;
  }

  GetPOLineList(PONo: string) {
    /* debugger;*/
    var res = this.http.get<any>(this.Url + "Order/GetPOLineList?PONo=" + PONo, {
      headers: new HttpHeaders().set('Authorization', "Bearer " + this.auth)
    })
    return res;
  }

  GetPOModuleList(PONo: string) {
    /* debugger;*/
    var res = this.http.get<any>(this.Url + "Order/GetPOModuleList?PONo=" + PONo, {
      headers: new HttpHeaders().set('Authorization', "Bearer " + this.auth)
    })
    return res;
  }

  GetPOImages(SONo: string) {
    /* debugger;*/
    var res = this.http.get<any>(this.Url + "Order/GetPOImages?SONo=" + SONo, {
      headers: new HttpHeaders().set('Authorization', "Bearer " + this.auth)
    })
    return res;
  }

  PostPOImages(model: any) {
    /* debugger;*/
    /*alert("PostPOImages");*/
    //alert(model);
    var res = this.http.post<any>(this.Url + "Order/PostPOImages", model, {
      headers: new HttpHeaders().set('Authorization', "Bearer " + this.auth)
    })
    return res;
  }

  PutPOImages(model: any) {
    /* debugger;*/
    /*alert("PostPOImages");*/
    //alert(model);
    var res = this.http.post<any>(this.Url + "Order/PutPOImages", model, {
      headers: new HttpHeaders().set('Authorization', "Bearer " + this.auth)
    })
    return res;
  }
  DeletePOImage(model: any) {
    var res = this.http.post<any>(this.Url + "Order/DeletePOImage", model, {
      headers: new HttpHeaders().set('Authorization', "Bearer " + this.auth)
    })
    return res;
  }

  POPartsList(model: any) {
    /* debugger;*/
    var res = this.http.post<any>(this.Url + "Order/POPartsList", model, {
      headers: new HttpHeaders().set('Authorization', "Bearer " + this.auth)
    })
    return res;
  }

  FillModule() {
    /* debugger;*/
    var res = this.http.get<any>(this.Url + "Module/FillModule",  {
      headers: new HttpHeaders().set('Authorization', "Bearer " + this.auth)
    })
    return res;
  }

    FillLine() {
    /* debugger;*/
    var res = this.http.get<any>(this.Url + "Line/FillLine", {
      headers: new HttpHeaders().set('Authorization', "Bearer " + this.auth)
    })
    return res;
  }

FillSO() {
    /* debugger;*/
    var res = this.http.get<any>(this.Url + "Order/FillSO",  {
      headers: new HttpHeaders().set('Authorization', "Bearer " + this.auth)
    })
    return res;
  }

  FillProcess() {
    /* debugger;*/
    var res = this.http.get<any>(this.Url + "Order/FactoryProcessList",  {
      headers: new HttpHeaders().set('Authorization', "Bearer " + this.auth)
    })
    return res;
  }

FillPO() {
    /* debugger;*/
    var res = this.http.get<any>(this.Url + "Order/FillPO",  {
      headers: new HttpHeaders().set('Authorization', "Bearer " + this.auth)
    })
    return res;
  }

  POOB(model: any) {
    /* debugger;*/
    var res = this.http.post<any>(this.Url + "OB/POOB", model, {
      headers: new HttpHeaders().set('Authorization', "Bearer " + this.auth)
    })
    return res;
  }

  //edit order section
  PurchaseOrder(model: any) {
    /* debugger;*/
    var res = this.http.post<any>(this.Url + "Order/PurchaseOrderView", model, {
      headers: new HttpHeaders().set('Authorization', "Bearer " + this.auth)
    })
    return res;
  }
  PurchaseOrderColorSizeList(model: any) {
    /* debugger;*/
    var res = this.http.post<any>(this.Url + "Order/PurchaseOrderColorSizeList", model, {
      headers: new HttpHeaders().set('Authorization', "Bearer " + this.auth)
    })
    return res;
  }

  PurchaseOrderColorSize(pono: string) {
    /* debugger;*/
    var res = this.http.get<any>(this.Url + "Order/PurchaseOrderColorSize/" + pono.toString(), {
      headers: new HttpHeaders().set('Authorization', "Bearer " + this.auth)
    })
    return res;
  }
  Update(model: any) {
    /*    debugger;*/
    var res = this.http.post<any>(this.Url + "Order/PutOrder", model, {
      headers: new HttpHeaders().set('Authorization', "Bearer " + this.auth)
    })
    return res;

  }
  PutOB(model: any){
    var res = this.http.post<any>(this.Url + "Order/PutOB", model, {
      headers: new HttpHeaders().set('Authorization', "Bearer " + this.auth)
    })
    return res;
  }
  PostIsFitExist(model: any){
    var res = this.http.post<any>(this.Url + "Order/PostIsFitExist", model, {
      headers: new HttpHeaders().set('Authorization', "Bearer " + this.auth)
    })
    return res;
  }
  PostIsProductExist(model: any){
    var res = this.http.post<any>(this.Url + "Order/PostIsProductExist", model, {
      headers: new HttpHeaders().set('Authorization', "Bearer " + this.auth)
    })
    return res;
  }
  PostIsCustomerExist(model: any){
    var res = this.http.post<any>(this.Url + "Order/PostIsCustomerExist", model, {
      headers: new HttpHeaders().set('Authorization', "Bearer " + this.auth)
    })
    return res;
  }
  PostIsPOExist(model: any){
    var res = this.http.post<any>(this.Url + "Order/PostIsPOExist", model, {
      headers: new HttpHeaders().set('Authorization', "Bearer " + this.auth)
    })
    return res;
  }
  FactoryProcessList(model: any){
    var res = this.http.post<any>(this.Url + "Order/FactoryProcessList", model, {
      headers: new HttpHeaders().set('Authorization', "Bearer " + this.auth)
    })
    return res;
  }
  GetProcessTemplate(model:any){
    var res = this.http.post<any>(this.Url + "Order/GetProcessTemplate", model, {
      headers: new HttpHeaders().set('Authorization', "Bearer " + this.auth)
    })
    return res;
  }
  PutProcessTemplate(model:any){
    var res = this.http.post<any>(this.Url + "Order/PostProcessTemplate", model, {
      headers: new HttpHeaders().set('Authorization', "Bearer " + this.auth)
    })
    return res;
  }
}


