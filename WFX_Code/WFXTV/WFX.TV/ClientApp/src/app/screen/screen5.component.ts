import { Component, Inject, OnInit } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Router } from '@angular/router';
import { DashboardService } from '../services/dashboard.service';
import { HourlyProductionModel } from '../../models/HourlyProductionModel';

@Component({
  selector: 'app-screen',
  templateUrl: './screen5.component.html'
})

export class Screen5Component implements OnInit {
  public model: HourlyProductionModel;
  UserName: string;
  CurrentDate: Date;
  Module: string;
  Line: string;
  Shift: string;
  constructor(
    private router: Router,
    http: HttpClient,
    @Inject('BASE_URL') baseUrl: string,
    private DashboardService: DashboardService
  ) {
    this.loadScripts();
    if(localStorage.length==0){
      this.router.navigate(['']);
    }
    else{
      this.UserName = localStorage.getItem('userFirstName') + " " + localStorage.getItem('userLastName').replace("-", "");
    }   
    this.CurrentDate = new Date();
    this.Module = localStorage.getItem('module');
    this.Line = localStorage.getItem('line');
    this.Shift = localStorage.getItem('shift');
  }

  ngOnInit() {
    this.GetData();
    setTimeout(() => {
      if( localStorage.getItem('module')!='' || localStorage.getItem('line')!='' || localStorage.getItem('shift')!=''){
        this.router.navigate(['/Screen1']);
      }
    }, 20000);  //20s
  }

  GetData() {
    this.model = new HourlyProductionModel();
    this.model.module = this.Module;
    this.model.line = this.Line;
    this.model.shiftname = this.Shift;
    if(this.model.module!='' || this.model.line!='' || this.model.shiftname!='')
    {
      this.DashboardService.HourlyProduction(this.model).subscribe(
        res => {
          this.model = res.data;
          console.log(res.data);
        });
    }
  }

  // Method to dynamically load JavaScript
  loadScripts() {
    const dynamicScriptsbody = [
      'assets/js/custom.js',
    ];

    for (let i = 0; i < dynamicScriptsbody.length; i++) {
      const node = document.createElement('script');
      node.src = dynamicScriptsbody[i];
      node.type = 'text/javascript';
      node.async = false;
      document.getElementsByTagName('body')[0].appendChild(node);
    }
  }
  home() {
    localStorage.setItem('module', '');
    localStorage.setItem('line', '');
    localStorage.setItem('shift', '');
    this.router.navigate(['/Dashboard']);
  };
}

