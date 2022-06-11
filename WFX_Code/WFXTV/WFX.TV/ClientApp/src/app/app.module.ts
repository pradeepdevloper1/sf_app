import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { Routes,RouterModule } from '@angular/router';
import { StoreModule } from '@ngrx/store';
import { AppComponent } from './app.component';
import { HomeComponent } from './home/home.component';
import { StoreDevtoolsModule } from '@ngrx/store-devtools';
import { AgGridModule } from 'ag-grid-angular';
import { ChartModule, LineSeriesService, CategoryService, LegendService , DataLabelService,TooltipService} from '@syncfusion/ej2-angular-charts';
import { NgSelect2Module } from 'ng-select2';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';

import { DashboardComponent } from './dashboard/dashboard.component';
import { LoginComponent } from './login/login.component';
import { Screen1Component } from './screen/screen1.component';
import { Screen2Component } from './screen/screen2.component';
import { Screen3Component } from './screen/screen3.component';
import { Screen4Component } from './screen/screen4.component';
import { Screen5Component } from './screen/screen5.component';


@NgModule({
  declarations: [
    AppComponent,
    HomeComponent,
    DashboardComponent,
    LoginComponent,
    Screen1Component,
    Screen2Component,
    Screen3Component,
    Screen4Component,
    Screen5Component,
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    HttpClientModule,
    AgGridModule.withComponents([]),
    ChartModule,
    RouterModule,
    NgSelect2Module,
    NgbModule,
    FormsModule,
    ReactiveFormsModule,
    StoreDevtoolsModule.instrument({
      maxAge: 25
    }),
    RouterModule.forRoot([
      { path: '', component: LoginComponent },
      { path: 'Dashboard', component: DashboardComponent },
      { path: 'Screen1', component: Screen1Component },
      { path: 'Screen2', component: Screen2Component },
      { path: 'Screen3', component: Screen3Component },
      { path: 'Screen4', component: Screen4Component },
      { path: 'Screen5', component: Screen5Component },

    ], { relativeLinkResolution: 'legacy' })
  ],
  providers: [LineSeriesService, CategoryService, LegendService, DataLabelService, TooltipService],
  bootstrap: [AppComponent]
})
export class AppModule { }
