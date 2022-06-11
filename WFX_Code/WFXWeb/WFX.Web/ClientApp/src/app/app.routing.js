"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.routing = void 0;
var router_1 = require("@angular/router");
var login_component_1 = require("./login/login.component");
var admindashboard_component_1 = require("./admin/admindashboard.component");
var userdashboard_component_1 = require("./userdashboard/userdashboard.component");
var order_component_1 = require("./order/order.component");
var orderupload_component_1 = require("./order/orderupload.component");
var saveorder_component_1 = require("./order/saveorder.component");
var orderlist_component_1 = require("./order/orderlist.component");
var soview_component_1 = require("./order/soview.component");
var poview_component_1 = require("./order/poview.component");
var imgeditor_component_1 = require("./order/imgeditor.component");
var planeditor_component_1 = require("./order/planeditor.component");
var editorder_component_1 = require("./order/editorder.component");
var reports_component_1 = require("./report/reports.component");
var booking_component_1 = require("./linebooking/booking.component");
var bookingupload_component_1 = require("./linebooking/bookingupload.component");
var savebooking_component_1 = require("./linebooking/savebooking.component");
var bookinglist_component_1 = require("./linebooking/bookinglist.component");
var target_component_1 = require("./linetarget/target.component");
var targetupload_component_1 = require("./linetarget/targetupload.component");
var savetarget_component_1 = require("./linetarget/savetarget.component");
var targetlist_component_1 = require("./linetarget/targetlist.component");
var qc_component_1 = require("./qc/qc.component");
var organisation_component_1 = require("./admin/organisation.component");
var cluster_component_1 = require("./admin/cluster.component");
var factory_component_1 = require("./admin/factory.component");
var factorywizard_component_1 = require("./admin/factorywizard.component");
var uploadproduct_component_1 = require("./admin/uploadproduct.component");
var factorylist_component_1 = require("./admin/factorylist.component");
var clusterlist_component_1 = require("./admin/clusterlist.component");
var organisationlist_component_1 = require("./admin/organisationlist.component");
var productlist_component_1 = require("./admin/productlist.component");
var productfitlist_component_1 = require("./admin/productfitlist.component");
var customerlist_component_1 = require("./admin/customerlist.component");
var linelist_component_1 = require("./admin/linelist.component");
var shiftlist_component_1 = require("./admin/shiftlist.component");
var defectslist_component_1 = require("./admin/defectslist.component");
var userlist_component_1 = require("./admin/userlist.component");
var editorganisation_component_1 = require("./admin/editorganisation.component");
var editcluster_component_1 = require("./admin/editcluster.component");
var editfactory_component_1 = require("./admin/editfactory.component");
var site_layout_component_1 = require("./layout/site-layout/site-layout.component");
var admin_layout_component_1 = require("./layout/admin-layout/admin-layout.component");
var modulelist_component_1 = require("./admin/modulelist.component");
var processdefinitionlist_component_1 = require("./admin/processdefinitionlist.component");

var appRoutes = [
    //Site routes goes here 
    {
        path: 'User',
        component: site_layout_component_1.SiteLayoutComponent,
        children: [
            { path: 'UserDashboard', component: userdashboard_component_1.UserDashboardComponent },
            { path: 'Order', component: order_component_1.OrderComponent },
            { path: 'OrderUpload', component: orderupload_component_1.OrderUploadComponent },
            { path: 'SaveOrder', component: saveorder_component_1.SaveOrderComponent },
            // { path: 'OrderList/:pono', component: orderlist_component_1.OrderListComponent },
            { path: 'OrderList', component: orderlist_component_1.OrderListComponent },
            { path: 'SOView/:sono', component: soview_component_1.SOViewComponent },
            { path: 'POView/:pono', component: poview_component_1.POViewComponent },
            { path: 'IMGEditor/:pono/:img_ob', component: imgeditor_component_1.IMGEditorComponent },
            { path: 'EditOrder/:pono', component: editorder_component_1.EditOrderComponent },
            { path: 'Reports', component: reports_component_1.ReportsComponent },
            { path: 'Booking', component: booking_component_1.BookingComponent },
            { path: 'BookingUpload', component: bookingupload_component_1.BookingUploadComponent },
            { path: 'SaveBooking', component: savebooking_component_1.SaveBookingComponent },
            { path: 'BookingList', component: bookinglist_component_1.BookingListComponent },
            { path: 'Target', component: target_component_1.TargetComponent },
            { path: 'TargetUpload', component: targetupload_component_1.TargetUploadComponent },
            { path: 'SaveTarget', component: savetarget_component_1.SaveTargetComponent },
            { path: 'TargetList', component: targetlist_component_1.TargetListComponent },
            { path: 'PlanEditor/:pono', component: planeditor_component_1.PlanEditorComponent },
            { path: 'QC', component: qc_component_1.QCComponent },
        ]
    },
    //Site routes goes here 
    {
        path: 'Admin',
        component: admin_layout_component_1.AdminLayoutComponent,
        children: [
            { path: 'AdminDashboard', component: admindashboard_component_1.AdminDashboardComponent },
            { path: 'SaveOrganisation', component: organisation_component_1.OrganisationComponent },
            { path: 'Culster/:id', component: cluster_component_1.CulsterComponent },
            { path: 'Factory/:id', component: factory_component_1.FactoryComponent },
            { path: 'FactoryWizard/:id', component: factorywizard_component_1.FactoryWizardComponent },
            { path: 'UploadProduct', component: uploadproduct_component_1.UploadProductComponent },
            { path: 'FactoryList', component: factorylist_component_1.FactoryListComponent },
            { path: 'ClusterList', component: clusterlist_component_1.ClusterListComponent },
            { path: 'OrganisationList', component: organisationlist_component_1.OrganisationListComponent },
            { path: 'ProductList/:id', component: productlist_component_1.ProductListComponent },
            { path: 'ProductFitList/:id', component: productfitlist_component_1.ProductFitListComponent },
            { path: 'CustomerList/:id', component: customerlist_component_1.CustomerListComponent },
            { path: 'LineList/:id', component: linelist_component_1.LineListComponent },
            { path: 'ShiftList/:id', component: shiftlist_component_1.ShiftListComponent },
            { path: 'DefectsList/:id', component: defectslist_component_1.DefectsListComponent },
            { path: 'UserList/:id', component: userlist_component_1.UserListComponent },
            { path: 'EditOrganisation/:id', component: editorganisation_component_1.EditOrganisationComponent },
            { path: 'EditCulster/:id', component: editcluster_component_1.EditCulsterComponent },
            { path: 'EditFactory/:id', component: editfactory_component_1.EditFactoryComponent },
            { path: 'ModuleList/:id', component: modulelist_component_1.ModuleListComponent },
            { path: 'ProcessDefinitionList/:id', component: processdefinitionlist_component_1.ProcessdefinitionlistComponent },

        ]
    },
    //no layout routes
    { path: '', component: login_component_1.LoginComponent },
];
/*imports: [RouterModule.forRoot(routes, {onSameUrlNavigation: 'reload'//})],*/
/*export const routing = RouterModule.forRoot(appRoutes, { onSameUrlNavigation: 'reload'});*/
exports.routing = router_1.RouterModule.forRoot(appRoutes);
//# sourceMappingURL=app.routing.js.map