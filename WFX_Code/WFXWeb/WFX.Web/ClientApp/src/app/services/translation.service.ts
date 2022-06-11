import { HttpClient, HttpHeaders } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { environment } from "src/environments/environment";

@Injectable({
    providedIn: 'root'
})
export class TranslationService {
    Url: string;
    auth: string;

    constructor(private http: HttpClient) {
        this.Url = environment.apiUrl;
        this.auth = sessionStorage.getItem('auth');
    }

    GetData(objectName: string) {
        var res = this.http.get<any>(this.Url + "Translation/GetData/" + objectName, {
            headers: new HttpHeaders().set('Authorization', "Bearer " + this.auth)
        })
        return res;
    }
}
