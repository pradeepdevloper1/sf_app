import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { Routes,RouterModule } from '@angular/router';
import { StoreModule } from '@ngrx/store';
import { AppComponent } from './app.component';
import { NavMenuComponent } from './nav-menu/nav-menu.component';
import { HomeComponent } from './home/home.component';
import { StoreDevtoolsModule } from '@ngrx/store-devtools';
import { AgGridModule } from 'ag-grid-angular';


import { Injector, APP_INITIALIZER } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { CommonModule, LOCATION_INITIALIZED } from '@angular/common';

/*import { AccumulationChartModule } from '@syncfusion/ej2-angular-charts';*/
//import { ChartModule, PieSeriesService,LineSeriesService, CategoryService, LegendService , DataLabelService,TooltipService} from '@syncfusion/ej2-angular-charts';

import { ChartModule, LineSeriesService, CategoryService, LegendService, DataLabelService, TooltipService, ChartAnnotationService, DateTimeService } from '@syncfusion/ej2-angular-charts';
import { BarSeriesService, StackingBarSeriesService } from '@syncfusion/ej2-angular-charts';
import { AccumulationChartModule } from '@syncfusion/ej2-angular-charts';
import {  PieSeriesService, AccumulationLegendService, AccumulationTooltipService, AccumulationAnnotationService,  AccumulationDataLabelService} from '@syncfusion/ej2-angular-charts';

import { NgSelect2Module } from 'ng-select2';

// import ngx-translate and the http loader
import { TranslateLoader, TranslateModule } from '@ngx-translate/core';
import { TranslateHttpLoader } from '@ngx-translate/http-loader';
import { HttpClient } from '@angular/common/http';

import { LoginComponent } from './login/login.component';
import { AdminDashboardComponent } from './admin/admindashboard.component';
import { UserDashboardComponent } from './userdashboard/userdashboard.component';

import { OrderComponent } from './order/order.component';
import { OrderUploadComponent } from './order/orderupload.component';
import { SaveOrderComponent } from './order/saveorder.component';
import { OrderListComponent } from './order/orderlist.component';
import { SOViewComponent } from './order/soview.component';
import { POViewComponent } from './order/poview.component';
import { IMGEditorComponent } from './order/imgeditor.component';
import { PlanEditorComponent } from './order/planeditor.component';
import { CellCustomComponent } from './order/cell-custom.component';
import { EditOrderComponent } from './order/editorder.component';
import { CellPOViewComponent } from './order/cell-poview.component';

import { ReportsComponent } from './report/reports.component';
import { SettingsComponent } from './settings/settings.component';

import { BookingComponent } from './linebooking/booking.component';
import { BookingUploadComponent } from './linebooking/bookingupload.component';
import { SaveBookingComponent } from './linebooking/savebooking.component';
import { BookingListComponent } from './linebooking/bookinglist.component';

import { TargetComponent } from './linetarget/target.component';
import { TargetUploadComponent } from './linetarget/targetupload.component';
import { SaveTargetComponent } from './linetarget/savetarget.component';
import { TargetListComponent } from './linetarget/targetlist.component';


import { QCComponent } from './qc/qc.component';

import { OrganisationComponent } from './admin/organisation.component';
import { CulsterComponent } from './admin/cluster.component';
import { FactoryComponent } from './admin/factory.component';
import { FactoryWizardComponent } from './admin/factorywizard.component';
import { UploadProductComponent } from './admin/uploadproduct.component';
import { FactoryListComponent } from './admin/factorylist.component';
import { ClusterListComponent } from './admin/clusterlist.component';
import { OrganisationListComponent } from './admin/organisationlist.component';
import { ProductListComponent } from './admin/productlist.component';
import { ProductFitListComponent } from './admin/productfitlist.component';
import { CustomerListComponent } from './admin/customerlist.component';
import { LineListComponent } from './admin/linelist.component';
import { ShiftListComponent } from './admin/shiftlist.component';
import { DefectsListComponent } from './admin/defectslist.component';
import { UserListComponent } from './admin/userlist.component';
import { EditOrganisationComponent } from './admin/editorganisation.component';
import { EditCulsterComponent } from './admin/editcluster.component';
import { EditFactoryComponent } from './admin/editfactory.component';

import { SiteHeaderComponent } from './layout/site-header/site-header.component';
import { SiteFooterComponent } from './layout/site-footer/site-footer.component';
import { SiteLeftComponent } from './layout/site-left/site-left.component';
import { SiteLayoutComponent } from './layout/site-layout/site-layout.component';

import { AdminHeaderComponent } from './layout/admin-header/admin-header.component';
import { AdminLeftComponent } from './layout/admin-left/admin-left.component';
import { AdminLayoutComponent } from './layout/admin-layout/admin-layout.component';
import { ModuleListComponent } from './admin/modulelist.component';
import { routing } from './app.routing';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { SharedModule } from './commonlibrary/shared.module';
import { TargetPlanPopupComponent } from './linebooking/targetplanpopup.component';
import { GridDatePickerComponent } from './commonlibrary/griddatepicker.component';
import { OrderListProgressBarComponent } from './order/orderlistprogressbar.component';
import { MatTooltipModule } from '@angular/material/tooltip';
import { ProcessdefinitionlistComponent } from './admin/processdefinitionlist.component';
import { ProcessTemplateComponent } from './order/processtemplate.component';
import { UploadObComponent } from './ob/uploadob.component';


@NgModule({
  declarations: [
    AppComponent,
    NavMenuComponent,
    HomeComponent,

    LoginComponent,
    AdminDashboardComponent,
    UserDashboardComponent,

    OrderComponent,
    OrderUploadComponent,
    SaveOrderComponent,
    OrderListComponent,
    SOViewComponent,
    POViewComponent,
    IMGEditorComponent,
    CellCustomComponent,
    EditOrderComponent,
    CellPOViewComponent,

    ReportsComponent,
    SettingsComponent,

    BookingComponent,
    BookingUploadComponent,
    SaveBookingComponent,
    BookingListComponent,

    TargetComponent,
    TargetUploadComponent,
    SaveTargetComponent,
    TargetListComponent,
    PlanEditorComponent,

    QCComponent,

    OrganisationComponent,
    CulsterComponent,
    FactoryComponent,
    FactoryWizardComponent,
    UploadProductComponent,
    FactoryListComponent,
    ClusterListComponent,
    OrganisationListComponent,
    ProductListComponent,
    ProductFitListComponent,
    CustomerListComponent,
    LineListComponent,
    ShiftListComponent,
    DefectsListComponent,
    UserListComponent,
    EditOrganisationComponent,
    EditCulsterComponent,
    EditFactoryComponent,

    SiteHeaderComponent,
    SiteFooterComponent,
    SiteLeftComponent,
    SiteLayoutComponent,
    AdminHeaderComponent,
    AdminLeftComponent,
    AdminLayoutComponent,
    ModuleListComponent,
    TargetPlanPopupComponent,
    OrderListProgressBarComponent,
    ProcessTemplateComponent,
    ProcessdefinitionlistComponent,
    UploadObComponent
  ],
  imports: [
    routing,SharedModule,
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    HttpClientModule,
    CommonModule,
    // ngx-translate and the loader module
    TranslateModule.forRoot({
      loader: {
        provide: TranslateLoader,
        useFactory: HttpLoaderFactory,
        deps: [HttpClient]
      }
    }),
    AgGridModule.withComponents([GridDatePickerComponent]),
    AccumulationChartModule,
    ChartModule,
    RouterModule,
    NgSelect2Module,
    FormsModule,
    ReactiveFormsModule,
    StoreDevtoolsModule.instrument({
      maxAge: 25
    }),
    BrowserAnimationsModule,MatTooltipModule,
    //RouterModule.forRoot([
    //  { path: '', component: LoginComponent },
    //  { path: 'AdminDashboard', component: AdminDashboardComponent },
    //  { path: 'UserDashboard', component: UserDashboardComponent },

    //  { path: 'Order', component: OrderComponent },
    //  { path: 'OrderUpload', component: OrderUploadComponent },
    //  { path: 'SaveOrder', component: SaveOrderComponent },
    //  { path: 'OrderList/:pono', component: OrderListComponent },
    //  { path: 'SOView/:sono', component: SOViewComponent },
    //  { path: 'POView/:pono', component: POViewComponent },
    //  { path: 'IMGEditor/:pono', component: IMGEditorComponent },
    //  { path: 'EditOrder/:pono', component: EditOrderComponent },

    //  { path: 'Reports', component: ReportsComponent },

    //  { path: 'Booking', component: BookingComponent },
    //  { path: 'BookingUpload', component: BookingUploadComponent },
    //  { path: 'SaveBooking', component: SaveBookingComponent },
    //  { path: 'BookingList', component: BookingListComponent },

    //  { path: 'Target', component: TargetComponent },
    //  { path: 'TargetUpload', component: TargetUploadComponent },
    //  { path: 'SaveTarget', component: SaveTargetComponent },
    //  { path: 'TargetList', component: TargetListComponent },
    //  { path: 'PlanEditor/:pono', component: PlanEditorComponent },

    //  { path: 'QC', component: QCComponent },
    //  { path: 'Chart', component: LineChartComponent },

    //  { path: 'SaveOrganisation', component: OrganisationComponent },
    //  { path: 'Culster/:id', component: CulsterComponent },
    //  { path: 'Factory/:id', component: FactoryComponent },
    //  { path: 'FactoryWizard/:id', component: FactoryWizardComponent },
    //  { path: 'UploadProduct', component: UploadProductComponent },
    //  { path: 'FactoryList', component: FactoryListComponent },
    //  { path: 'ClusterList', component: ClusterListComponent },
    //  { path: 'OrganisationList', component: OrganisationListComponent },
    //  { path: 'ProductList/:id', component: ProductListComponent },
    //  { path: 'ProductFitList/:id', component: ProductFitListComponent },
    //  { path: 'CustomerList/:id', component: CustomerListComponent },
    //  { path: 'LineList/:id', component: LineListComponent },
    //  { path: 'ShiftList/:id', component: ShiftListComponent },
    //  { path: 'DefectsList/:id', component: DefectsListComponent },
    //  { path: 'UserList/:id', component: UserListComponent },
    //  { path: 'EditOrganisation/:id', component: EditOrganisationComponent },
    //  { path: 'EditCulster/:id', component: EditCulsterComponent },
    //  { path: 'EditFactory/:id', component: EditFactoryComponent },


    //], { relativeLinkResolution: 'legacy' })
  ],
  providers: [LineSeriesService, CategoryService, LegendService,
    DataLabelService, TooltipService, PieSeriesService,
    AccumulationLegendService, AccumulationTooltipService,
    AccumulationDataLabelService, AccumulationAnnotationService,
    BarSeriesService, StackingBarSeriesService, ChartAnnotationService,
    DateTimeService,
    {
      provide: APP_INITIALIZER,
      useFactory: appInitializerFactory,
      deps: [TranslateService, Injector],
      multi: true
    },

  ],
  entryComponents: [CellCustomComponent],
  bootstrap: [AppComponent]
})

export class AppModule { }

// required for AOT compilation
export function HttpLoaderFactory(http: HttpClient): TranslateHttpLoader {
  return new TranslateHttpLoader(http);
}


//export function HttpLoaderFactory(http: Http) {
//  return new TranslateHttpLoader(http, "./assets/i18n/", ".json");
//}

export function appInitializerFactory(translate: TranslateService, injector: Injector) {
  return () => new Promise<any>((resolve: any) => {
    const locationInitialized = injector.get(LOCATION_INITIALIZED, Promise.resolve(null));
    locationInitialized.then(() => {
      let langToSet = 'en'
      if(sessionStorage.getItem('lang')){
        langToSet = sessionStorage.getItem('lang');
      }
      translate.setDefaultLang(langToSet);
      translate.use(langToSet).subscribe(() => {
        console.info(`Successfully initialized '${langToSet}' language.'`);
      }, err => {
        console.error(`Problem with '${langToSet}' language initialization.'`);
      }, () => {
        resolve(null);
      });
    });
  });
}
