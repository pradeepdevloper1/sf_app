import { Component, Inject, OnInit } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { ActivatedRoute, Router } from '@angular/router';
import { Observable } from 'rxjs';
import { AgGridAngular } from 'ag-grid-angular';
import { FormBuilder, FormGroup, FormsModule, Validators } from '@angular/forms';
import { environment } from '../../environments/environment';

declare function getlist(): any;
declare function nexterror(): any;
declare function prverror(): any;

@Component({
  selector: 'app-linetarget',
  templateUrl: './savetarget.component.html'
})

export class SaveTargetComponent implements OnInit {
  Url: string;
  UserName: string;
  ExcelFileName: string;
  auth: string;
  saveRes: string;
pono:string;
  constructor(private router: Router, private http: HttpClient, private route: ActivatedRoute) {
    this.loadScripts();
    this.Url = environment.apiUrl;
    this.auth = sessionStorage.getItem('auth');

    this.UserName = sessionStorage.getItem('userFirstName') + " " + sessionStorage.getItem('userLastName').replace("-", "");
    this.ExcelFileName = sessionStorage.getItem("linetargetfilename");
    this.pono = this.route.snapshot.params.pono;
  }

  async Save() {
    var res = getlist();
    //alert(res.message);
    if (res.status == 200) {
      if(this.pono){
        this.router.navigate(['/User/PlanEditor',this.pono]);
      } else{
      this.router.navigate(['/User/BookingList']);
      }
    }
  }

  Cancel() {

      if(this.pono){
        this.router.navigate(['/User/PlanEditor',this.pono]);
      } else{
      this.router.navigate(['/User/BookingList']);
      }
  }

  ngOnInit() {
  }

  next() {
    nexterror();
  }

  previous() {
    prverror();
  }

  // Method to dynamically load JavaScript
  loadScripts() {
     const dynamicScriptsbody = [
    'https://unpkg.com/@ag-grid-enterprise/all-modules@25.1.0/dist/ag-grid-enterprise.min.js',
      'assets/js/w_linetarget.js',
    ];

    for (let i = 0; i < dynamicScriptsbody.length; i++) {
      const node = document.createElement('script');
      node.src = dynamicScriptsbody[i];
      node.type = 'text/javascript';
      node.async = false;
      document.getElementsByTagName('body')[0].appendChild(node);
    }
  }
}

