import { Component, Inject, OnInit } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Router } from '@angular/router';
import { AdminDashboardService } from '../services/admindashboard.service';
import { AdminDashboardModel } from '../../models/AdminDashboardModel';

@Component({
  selector: 'app-admin',
  templateUrl: './admindashboard.component.html'
})

export class AdminDashboardComponent implements OnInit {
  public admindashboardmodel: AdminDashboardModel;
  UserName: string;
  ngOnInit() {

  }

  constructor(private router: Router, http: HttpClient, @Inject('BASE_URL') baseUrl: string, private AdminDashboardService: AdminDashboardService) {
    this.UserName = sessionStorage.getItem('userFirstName') + " " + sessionStorage.getItem('userLastName').replace("-", "");

    this.admindashboardmodel = new AdminDashboardModel();
    this.AdminDashboardService.GetAdminDashboard().subscribe(
      res => {
        this.admindashboardmodel = res;
      })
  }

}

