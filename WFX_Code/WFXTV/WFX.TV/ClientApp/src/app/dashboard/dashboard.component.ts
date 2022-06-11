import { Component, Inject, OnInit } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Router } from '@angular/router';
import { DashboardService } from '../services/dashboard.service';
import { DashboardModel } from '../../models/DashboardModel';
import { Select2OptionData } from 'ng-Select2';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html'
})

export class DashboardComponent implements OnInit {
  public dashboardmodel: DashboardModel;
  UserName: string;
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

  }

  ngOnInit() {
    this.GetDashboard();
  }

  public moduleChangeHandler(event: string) {
    console.log('module changed: ' + event);
  }

  public lineChangeHandler(event: string) {
    console.log('line changed: ' + event);
  }

  public shiftChangeHandler(event: string) {
    console.log('shift changed: ' + event);
  }

  Play() {
    if (this.Module == undefined){
      alert("Please enter module")
      return;
    }
    if (this.Line == undefined){
      alert("Please enter Line")
      return;
    }
    if (this.Shift == undefined){
      alert("Please enter Shift")
      return;
    }
    localStorage.setItem('module', this.Module);
    localStorage.setItem('line', this.Line);
    localStorage.setItem('shift', this.Shift);
    this.router.navigate(['/Screen1']);
  }

  GetDashboard() {
    this.dashboardmodel = new DashboardModel();
    this.DashboardService.FillFactoryModule().subscribe(
      res => {
        this.dashboardmodel.modulelist = res.data;
        console.log(res.data);
      })
    this.DashboardService.FillFactoryLine().subscribe(
      res => {
        this.dashboardmodel.linelist = res.data;
        console.log(res.data);
      })
    this.DashboardService.FillFactoryShift().subscribe(
      res => {
        this.dashboardmodel.shiftlist = res.data;
        console.log(res.data);
      })
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
  
  logout() {
    sessionStorage.clear();
    localStorage.clear();
    this.router.navigate(['']);
  };
}

