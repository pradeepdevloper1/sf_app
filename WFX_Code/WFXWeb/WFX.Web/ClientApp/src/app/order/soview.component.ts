import { Component, Inject, OnInit } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Router } from '@angular/router';
import { Observable } from 'rxjs';
import { FormBuilder, FormGroup, FormsModule, Validators } from '@angular/forms';
import { environment } from '../../environments/environment';
import { OrderService } from '../services/order.service';
import { SOViewModel } from '../../models/SOViewModel';
import { ActivatedRoute } from '@angular/router';

declare function getlist(): any;

@Component({
  selector: 'app-order',
  templateUrl: './soview.component.html'
})

export class SOViewComponent implements OnInit {
  Url: string;
  UserName: string;
  auth: string;
  public soviewmodel: SOViewModel;
  sono: string;
  pono: string;
  posearchtext: string;
  public imagelist: object[];
  PassWidth: any = '0%';
  DefectWidth: any = '0%';
  RejectWidth: any = '0%';
  Totalcheckedpcs = 0;
  RejectWidthPer:any;
  DefectWidthPer:any;
  PassWidthPer:any;
  constructor(
    private router: Router,
    private http: HttpClient,
    private orderservice: OrderService,
    private _Activatedroute: ActivatedRoute
  ) {
    this.loadScripts();

    this._Activatedroute.paramMap.subscribe(params => {
      this.sono = params.get('sono');
    });

    this.Url = environment.apiUrl;
    this.auth = sessionStorage.getItem('auth');
    this.UserName = sessionStorage.getItem('userFirstName') + " " + sessionStorage.getItem('userLastName').replace("-", "");

    this.soviewmodel = new SOViewModel();
    this.soviewmodel.soNo = this.sono
    this.orderservice.SOView(this.soviewmodel).subscribe(
      res => {
        this.soviewmodel = res.data;
        this.imagelist = res.imagelist
        this.loadSearch();
        this.CheckedPcsprogressBar();
      })
  }

  loadSearch() {
    this.soviewmodel.posearchtext = this.posearchtext;
    this.soviewmodel.soNo = this.sono;
    this.orderservice.POListOfSO(this.soviewmodel).subscribe(
      res => {
        this.soviewmodel.polist = res.data;
        console.log(res.data);
      })
  }

  onChangeEvent(event: any) {
    console.log(event.target.value);
    this.posearchtext = event.target.value;
    this.loadSearch();
  }

  ngOnInit() {
  }

  GetPOListOfSO() {
    this.soviewmodel.soNo = "SO2";
    this.orderservice.GetPOListOfSO(this.soviewmodel.soNo).subscribe(
      res => {
        this.soviewmodel.polist = res.data;
        console.log(this.soviewmodel);
      })
  }

  // Method to dynamically load JavaScript
  loadScripts() {
    const dynamicScriptsbody = [
      'assets/js/custom.js'
    ];

    for (let i = 0; i < dynamicScriptsbody.length; i++) {
      const node = document.createElement('script');
      node.src = dynamicScriptsbody[i];
      node.type = 'text/javascript';
      node.async = false;
      document.getElementsByTagName('body')[0].appendChild(node);
    }
  }
  CheckedPcsprogressBar() {
    if (!this.soviewmodel.completedQty) {
      this.soviewmodel.completedQty = 0;
    }
    if (!this.soviewmodel.defectedQty) {
      this.soviewmodel.defectedQty = 0;
    }
    if (!this.soviewmodel.rejectedQty) {
      this.soviewmodel.rejectedQty = 0;
    }
    this.Totalcheckedpcs = this.soviewmodel.completedQty + this.soviewmodel.defectedQty + this.soviewmodel.rejectedQty;
    if (this.Totalcheckedpcs > 0) {
      this.PassWidth = this.soviewmodel.completedQty * 100 / this.Totalcheckedpcs;
      this.DefectWidth = this.soviewmodel.defectedQty * 100 / this.Totalcheckedpcs;
      this.RejectWidth = this.soviewmodel.rejectedQty * 100 / this.Totalcheckedpcs;
      if (this.PassWidth < 15 && this.soviewmodel.completedQty > 0) {
        this.PassWidth = 15;
      }
      if (this.DefectWidth < 15 && this.soviewmodel.defectedQty > 0) {

        this.DefectWidth = 15

      }
      if (this.RejectWidth < 15 && this.soviewmodel.rejectedQty > 0) {
        this.RejectWidth = 15;
      }
      if (this.soviewmodel.rejectedQty < 1) {
        this.RejectWidth = 0;
      }

      if (this.soviewmodel.defectedQty < 1) {
        this.DefectWidth = 0;

      }
      if (this.soviewmodel.completedQty < 1) {
        this.PassWidth = 0;

      }
      if ((this.RejectWidth + this.PassWidth + this.DefectWidth) > 100) {
        const more = (this.RejectWidth + this.PassWidth + this.DefectWidth) - 100;
        if (this.RejectWidth > 15) {
          this.RejectWidth = this.RejectWidth - more;
          if (this.RejectWidth < 15) {
            this.RejectWidth = 15;
          }
        }
        if (this.PassWidth > 15) {
          this.PassWidth = this.PassWidth - more;
          if (this.PassWidth < 15) {
            this.PassWidth = 15;
          }
        }
        if (this.DefectWidth > 15) {
          this.DefectWidth = this.DefectWidth - more;
          if (this.DefectWidth < 15) {
            this.DefectWidth = 15;
          }
        }
      }
      this.DefectWidthPer = this.DefectWidth + '%'
      this.RejectWidthPer = this.RejectWidth + '%'
      this.PassWidthPer = this.PassWidth + '%'
    }
  }
}

