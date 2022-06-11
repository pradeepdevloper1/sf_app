import { Routes, RouterModule } from '@angular/router';

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

import { SiteLayoutComponent } from './layout/site-layout/site-layout.component';
import { AdminLayoutComponent } from './layout/admin-layout/admin-layout.component';
import { ModuleListComponent } from './admin/modulelist.component';
import { ProcessdefinitionlistComponent } from './admin/processdefinitionlist.component';
import { ProcessTemplateComponent } from './order/processtemplate.component';
import { SettingsComponent } from './settings/settings.component';
import { UploadObComponent } from './ob/uploadob.component';
const appRoutes: Routes = [
  //Site routes goes here
  {
    path: 'User',
    component: SiteLayoutComponent,
    children: [
      { path: 'UserDashboard', component: UserDashboardComponent },
      { path: 'Order', component: OrderComponent },
      { path: 'OrderUpload', component: OrderUploadComponent },
      { path: 'SaveOrder', component: SaveOrderComponent },
      //{ path: 'OrderList/:pono', component: OrderListComponent },
      { path: 'OrderList', component: OrderListComponent },
      { path: 'SOView/:sono', component: SOViewComponent },
      { path: 'POView/:pono/:sono', component: POViewComponent },
      { path: 'POView/:sono', component: POViewComponent },

      { path: 'IMGEditor/:sono/:img_ob', component: IMGEditorComponent },
      { path: 'EditOrder/:pono', component: EditOrderComponent },

      { path: 'Reports', component: ReportsComponent },
      { path: 'Settings', component: SettingsComponent },

      { path: 'Booking', component: BookingComponent },
      { path: 'BookingUpload', component: BookingUploadComponent },
      { path: 'SaveBooking', component: SaveBookingComponent },
      { path: 'BookingList', component: BookingListComponent },

      { path: 'Target', component: TargetComponent },
      { path: 'TargetUpload', component: TargetUploadComponent },
      { path: 'SaveTarget', component: SaveTargetComponent },
      { path: 'SaveTarget/:pono', component: SaveTargetComponent },
      { path: 'TargetList', component: TargetListComponent },
      { path: 'PlanEditor/:pono', component: PlanEditorComponent },
      { path: 'TargetUpload/:pono', component: TargetUploadComponent },
      { path: 'QC', component: QCComponent },
      { path: 'ProcessTemplate/:sono', component: ProcessTemplateComponent },
      { path: 'UploadOB', component: UploadObComponent },
    ]
  },

  //Site routes goes here
  {
    path: 'Admin',
    component: AdminLayoutComponent,
    children: [
      { path: 'AdminDashboard', component: AdminDashboardComponent },
      { path: 'SaveOrganisation', component: OrganisationComponent },
      { path: 'Culster/:id', component: CulsterComponent },
      { path: 'Factory/:id', component: FactoryComponent },
      { path: 'FactoryWizard/:id', component: FactoryWizardComponent },
      { path: 'UploadProduct', component: UploadProductComponent },
      { path: 'FactoryList', component: FactoryListComponent },
      { path: 'ClusterList', component: ClusterListComponent },
      { path: 'OrganisationList', component: OrganisationListComponent },
      { path: 'ProductList/:id', component: ProductListComponent },
      { path: 'ProductFitList/:id', component: ProductFitListComponent },
      { path: 'CustomerList/:id', component: CustomerListComponent },
      { path: 'LineList/:id', component: LineListComponent },
      { path: 'ShiftList/:id', component: ShiftListComponent },
      { path: 'DefectsList/:id', component: DefectsListComponent },
      { path: 'UserList/:id', component: UserListComponent },
      { path: 'EditOrganisation/:id', component: EditOrganisationComponent },
      { path: 'EditCulster/:id', component: EditCulsterComponent },
      { path: 'EditFactory/:id', component: EditFactoryComponent },
      { path: 'ModuleList/:id', component: ModuleListComponent },
      { path: 'ProcessDefinitionList/:id', component: ProcessdefinitionlistComponent },
    ]
  },

  //no layout routes
  { path: '', component: LoginComponent },
];

/*imports: [RouterModule.forRoot(routes, {onSameUrlNavigation: 'reload'//})],*/
/*export const routing = RouterModule.forRoot(appRoutes, { onSameUrlNavigation: 'reload'});*/
export const routing = RouterModule.forRoot(appRoutes);


