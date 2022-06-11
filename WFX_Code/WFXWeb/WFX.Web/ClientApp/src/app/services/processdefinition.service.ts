import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { HttpHeaders } from '@angular/common/http';
import { environment } from 'src/environments/environment';
import { ProcessDefinitionModel } from 'src/models/ProcessDefinitionModel';

@Injectable({
  providedIn: 'root'
})

export class ProcessDefinitionService {
  Url: string;
  auth: string;

  constructor(private http: HttpClient) {
    this.Url = environment.apiUrl;
    this.auth = sessionStorage.getItem('auth');
  }

  GetProcessDefinitionList(factoryId: number = 0) {
    var res = this.http.get<any>(this.Url + "ProcessDefinition/GetProcessDefinitionList/" + factoryId, {
      headers: new HttpHeaders().set('Authorization', "Bearer " + this.auth)
    })
    return res;
  }

  UpdateProcessDefinition(model: ProcessDefinitionModel[]) {
    var res = this.http.post<any>(this.Url + "ProcessDefinition/PutMultiProcessDefinition", model, {
      headers: new HttpHeaders().set('Authorization', "Bearer " + this.auth)
    })
    return res;
  }
}
