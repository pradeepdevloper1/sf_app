import { Component, Inject, OnInit } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Router } from '@angular/router';
import { environment } from 'src/environments/environment';
import { PlanModel } from '../../models/PlanModel';
import { PlanService } from '../services/plan.service';
import * as moment from 'moment';
import { TranslateService } from '@ngx-translate/core';
import { MatDialog, MatDialogConfig } from '@angular/material/dialog';
import { TargetPlanPopupComponent } from './targetplanpopup.component';
import { bytesToSize as funcbytesToSize } from '../../models/helper';
import { LineTargetModel } from 'src/models/LineTargetModel';
import { MatSnackBar } from '@angular/material/snack-bar';
import { GridDatePickerComponent } from '../commonlibrary/griddatepicker.component';
import { GridNumericFieldComponent } from '../commonlibrary/gridnumericfield.component';
import { OrderService } from '../services/order.service';

@Component({
  selector: 'app-linebooking',
  templateUrl: './bookinglist.component.html'
})

export class BookingListComponent implements OnInit {
  UserName: string;
  Url: string;
  header: any;
  auth: string;
  rowData: any;
  target_rowData: any;
  targetstartdate: string;
  targetenddate: string;
  gridApi: any
  gridColumnApi: any;
  planmodel: PlanModel;
  LineTargetModel: LineTargetModel;
  line: string;
  module: string;
  ProcessCode: string;
  po: string;
  startdate: Date;
  enddate: Date;
  lstartdate: Date;
  lenddate: Date;
  public columnDefs;
  public target_columnDefs;
  poList: [] = [];
  lineList: [] = [];
  moduleList: [] = [];
  processList: [] = [];

  selecttedFile = null;
  isInvalid: number;
  ExcelFileName: string;
  ExcelFileSize: string;
  userID: number = 0;
  showButton: boolean = false;
  updatedRows = [];
  frameworkComponents: any;
  selectedTab='LineTarget';
  lang: any;
  gridData: any;
  constructor(
    private translate: TranslateService,
    private router: Router,
    private http: HttpClient,
    private planservice: PlanService,
    @Inject('BASE_URL') baseUrl: string,
    private msg: MatSnackBar,
    public dialog: MatDialog,
    private orderservice: OrderService,

    // private cmf: CommonFunctions
  ) {
    this.userID = parseInt(sessionStorage.getItem('userID')),
      this.isInvalid = 0;
    this.loadScripts();
    this.UserName = sessionStorage.getItem('userFirstName') + " " + sessionStorage.getItem('userLastName').replace("-", "");
    this.Url = environment.apiUrl;
    this.auth = sessionStorage.getItem('auth');
    this.lang = sessionStorage.getItem('lang');
    this.gridData = translate.store.translations[this.lang].lang;
    this.columnDefs = [
      { field: "module", headerName: this.gridData["module"] },
      { field: "line", headerName: this.gridData["line"] },
      { field: "style", headerName: this.gridData["style"] },
      { field: "soNo", headerName: this.gridData["sono"] },
      { field: "poNo", headerName: this.gridData["pono"] },
      { field: "quantity", headerName: this.gridData["quantity"] },
      { field: "smv", headerName: this.gridData["smv"] },
      { field: "plannedEffeciency", headerName: this.gridData["plannedEfficiency"] },
      {
        field: "startDate", headerName: this.gridData["startDate"],
        cellRenderer: (params) => {
          return moment.utc(params.value).format('DD/MM/YY')
        }
      },
      {
        field: "endDate", headerName: this.gridData["endDate"],
        cellRenderer: (params) => {
          return moment.utc(params.value).format('DD/MM/YY')
        }
      }
    ];

    this.frameworkComponents = {
      GridDatePickerComponent: GridDatePickerComponent,
      GridNumericFieldComponent: GridNumericFieldComponent

    }
  }

  ngOnInit() {


    this.enddate = new Date();
    this.startdate = new Date();
    this.startdate.setDate(this.startdate.getDate() - 7);

    this.lenddate = new Date();
    this.lstartdate = new Date();
    this.lstartdate.setDate(this.lstartdate.getDate() - 7);

    this.planmodel = new PlanModel();
    this.planservice.FillRunningPO().subscribe(
      res => {
        res.data.unshift({ id: -1, text: 'All' });
        this.planmodel.polist = res.data;
        this.poList = res.data
      })

    this.planservice.FillLine().subscribe(
      res => {
        res.data.unshift({ id: -1, text: 'All' });
        this.planmodel.linelist = res.data;
      })

    this.planservice.FillModule().subscribe(
      res => {
        res.data.unshift({ id: -1, text: 'All' });
        this.planmodel.modulelist = res.data;
      })
    this.orderservice.FillProcess().subscribe(
      res => {
        res.data.unshift({ id: -1, text: 'All' });
        this.planmodel.processlist = res.data;
      })
    this.planmodel.IsTargetStartDate = 0;
    this.planmodel.IsTargetEndDate = 0;

    this.LineTargetList();
    this.LineBookingList();
  this.createJsonDef();
  }

  StartDate(val: any) {
    //alert("blur1 " + val);
    this.startdate = new Date(moment(val, 'DD/MM/YYYY').format('YYYY-MM-DD'));
    this.LineTargetList();
  }

  EndDate(val: any) {
    //alert("blur1 " + val);
    this.enddate = new Date(moment(val, 'DD/MM/YYYY').format('YYYY-MM-DD'));
    this.LineTargetList();
  }

  LineTargetList() {
    //alert("lineTargetList");
    if (this.line != '-1')
      this.planmodel.Line = this.line;
    else
      this.planmodel.Line = '';
    if (this.module != '-1')
      this.planmodel.Module = this.module;
    else
      this.planmodel.Module = '';
    if (this.ProcessCode != '-1')
      this.planmodel.ProcessCode = this.ProcessCode;
    else
      this.planmodel.ProcessCode = '';
    if (this.po != '-1')
      this.planmodel.PONo = this.po;
    else
      this.planmodel.PONo = '';
    this.planmodel.TargetStartDate = moment(this.startdate, 'DD/MM/YYYY').format('YYYY-MM-DD');
    this.planmodel.TargetEndDate = moment(this.enddate, 'DD/MM/YYYY').format('YYYY-MM-DD');
    //if (this.planmodel.IsTargetStartDate == 1) {
    //  this.planmodel.TargetStartDate = moment(this.targetstartdate, 'DD/MM/YYYY').format('YYYY-MM-DD');
    //  console.log(this.planmodel.TargetStartDate);
    //}
    //if (this.planmodel.IsTargetEndDate == 1) {
    //  this.planmodel.TargetEndDate = moment(this.targetenddate, 'DD/MM/YYYY').format('YYYY-MM-DD');
    //  console.log(this.planmodel.TargetEndDate);
    //}

    this.planservice.GetLineTargetList(this.planmodel).subscribe(
      res => {
        this.target_rowData = res.data;
      })
  }

  LineBookingList() {
    if (this.line != '-1')
      this.planmodel.Line = this.line;
    else
      this.planmodel.Line = '';
    if (this.module != '-1')
      this.planmodel.Module = this.module;
    else
      this.planmodel.Module = '';
    if (this.po != '-1')
      this.planmodel.PONo = this.po;
    else
      this.planmodel.PONo = '';
    if (this.ProcessCode != '-1')
      this.planmodel.ProcessCode = this.ProcessCode;
    else
     this.planmodel.ProcessCode = '';

    this.planservice.GetLineBookingList(this.planmodel).subscribe(
      res => {
        this.rowData = res.data;
      })
  }

  poChangeHandler(event: any) {
    if (event != "") {
      this.po = event;
    }
    else {
      this.po = "";
    }
    this.LineTargetList();
  }

  lineChangeHandler(event: any) {
    if (event != "") {
      this.line = event;
    }
    else {
      this.line = "";
    }
    this.LineTargetList();
  }

  moduleChangeHandler(event: any) {
    if (event != "") {
      this.module = event;
    }
    else {
      this.module = "";
    }
    this.LineTargetList();
  }

  processChangeHandler(event: any) {
    if (event != "") {
      this.ProcessCode = event;
    }
    else {
      this.ProcessCode = "";
    }
    this.LineTargetList();
  }

  SampleFile() {
    window.location.href = 'sample/linetargetsample.xlsx';
  }

  TargetStartDate(val: any) {
    this.targetstartdate = val;
    this.planmodel.IsTargetStartDate = 1;
    this.LineTargetList();
  }

  TargetEndDate(val: any) {
    this.targetenddate = val;
    this.planmodel.IsTargetEndDate = 1;
    this.LineTargetList();
  }

  bookingpoChangeHandler(event: any) {
    if (event != "") {
      this.po = event;
    }
    else {
      this.po = "";
    }
    this.LineBookingList();
  }

  bookinglineChangeHandler(event: any) {
    if (event != "") {
      this.line = event;
    }
    else {
      this.line = "";
    }
    this.LineBookingList();
  }

  bookingmoduleChangeHandler(event: any) {
    if (event != "") {
      this.module = event;
    }
    else {
      this.module = "";
    }
    this.LineBookingList();
  }

  BrowseBookingFile() {
    this.router.navigate(['/User/Booking']);
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
  GeneratePOPlanSheet() {

    const dialogRef = this.dialog.open(TargetPlanPopupComponent, {
      maxWidth: '90vw',
      height: '70%',
      width: '90%',
      disableClose: true,
      data: {
        POlist: this.poList,
      }

    });
    dialogRef.updatePosition({ bottom: '50px', right: '32px' });
  }

  BrowseTargetFile(event) {
    this.selecttedFile = <File>event.target.files[0];
    this.ExcelFileName = this.selecttedFile.name;
    this.ExcelFileSize = funcbytesToSize(this.selecttedFile.size);
    if (!this.ExcelFileName.includes('xlsx')) {
      this.isInvalid = 1;
      alert("Invalid Format ! Please valid format")
    }
    else {
      this.isInvalid = 0;
      this.onUpload();
    }
  }


  onUpload() {
    /*alert("upload call");*/
    const endpoint = 'Upload/Excel/UploadLineTarget/' + this.userID.toString();
    const formData: FormData = new FormData();
    formData.append('file', this.selecttedFile,this.ExcelFileName);
    var linetargetfilesize = funcbytesToSize(this.selecttedFile.size);
    /*alert("linetargetfilesize" + linetargetfilesize);*/
    /*alert(endpoint);*/
    this.http.post<any>(endpoint, formData)
      .subscribe(res => {
        if (res.status = 200) {
          sessionStorage.setItem("linetargetfilename", res.message);
          sessionStorage.setItem("linetargetfilepath", res.path);
          sessionStorage.setItem("linetargetfilesize", linetargetfilesize);
          this.router.navigate(['/User/TargetUpload']);
        }
      });
  }
  cellValueChanged(event: any) {
    this.showButton = true;
    let data = event.data;
    this.LineTargetModel = data;
    const removeIndex = this.updatedRows.findIndex(item => item.lineTargetID == this.LineTargetModel.lineTargetID);
    if (removeIndex != -1) {
      this.updatedRows.splice(removeIndex, 1);
    }
    this.updatedRows.push(this.LineTargetModel);
  }
 async savechanges() {
  let FactoryID=parseInt(sessionStorage.getItem("factoryID"));
    for(let row of this.updatedRows){

    if (isNaN(row.plannedEffeciency)) {
      this.msg.open('Planned Efficiency must be positive decimal/integer', 'Error', { duration: 2000 });
      return false;

    }
    if (isNaN(row.smv)) {
      this.msg.open('SMV must be positive decimal/integer', 'Error', { duration: 2000 });
      return false;

    }
    if (isNaN(row.shiftHours)) {
      this.msg.open('Shift Hours must be positive decimal/integer', 'Error', { duration: 2000 });
      return false;

    }
    row.plannedEffeciency= +row.plannedEffeciency;
    row.smv= +row.smv;
    row.shiftHours = +row.shiftHours;
    if(row.plannedEffeciency>100){
      this.msg.open('Planned Efficiency should be less than 100', 'Error', { duration: 2000 });
      return false;
    }
    if(row.plannedEffeciency<=0){
      this.msg.open('Planned Efficiency cannot be less than 1', 'Error', { duration: 2000 });
      return false;
    }
    var lineName = {
      LineName: row.line,
    }
    var shiftName = {
      shiftName: row.shiftName,
      FactoryID:FactoryID,
    }
    const t =await this.planservice.checkLineExist(lineName).toPromise();
  if (t.status == 401) {
    this.msg.open('Line doesn’t exist in the factory', 'Error', { duration: 2000 });
    return false;
  }

  const y =await this.planservice.checkShiftNameExist(shiftName).toPromise();
    if (y.status == 401) {
      this.msg.open('Shift Name doesn’t exist in the factory', 'Error', { duration: 2000 });
      return false;
    }

  }

  this.planservice.PutEditedLineTarget(this.updatedRows).subscribe(res => {
    this.msg.open(res.message, 'info', { duration: 2000 });
  })
  }
  OnGridReady(params: any): void {
    this.gridApi = params.api;
  }
  onTabClick(event){
    var target = event.target || event.srcElement || event.currentTarget;
    var idAttr = target.id;
    if (idAttr == 'secondTab') {
      this.selectedTab = 'LineTarget';

    }else{
      this.selectedTab = 'BookingList';
    }
  }
createJsonDef(){
  const dateObj = new Date();
  dateObj.setHours(0,0,0,0);
  let currDate =moment.utc(dateObj, 'DD/MM/YYYY');

  this.target_columnDefs = [
    { field: "module", headerName: this.gridData["module"] },
    { field: "ProcessName", headerName: this.gridData["Process"] },
    { field: "section", headerName: this.gridData["section"] },
    { field: "line", headerName: this.gridData["line"], editable:function(params){
      if(moment.utc(new Date(params.node.data.date), 'DD/MM/YYYY') < currDate || params.node.data.isorderrun>0){
        return false;
      }
      return true; }
    },
    { field: "style", headerName: this.gridData["style"] },
    { field: "soNo", headerName: this.gridData["productionorder"] },
    { field: "poNo", headerName: this.gridData["workorder"] },
    { field: "part", headerName: this.gridData["part"] },
    { field: "color", headerName: this.gridData["color"] },
    { field: "smv", headerName: this.gridData["smv"], editable:function(params){
      if(moment.utc(new Date(params.node.data.date), 'DD/MM/YYYY') < currDate || params.node.data.isorderrun >0){
        return false;
      }
      return true; }
    },
    { field: "operators", headerName: this.gridData["operators"],editable:function(params){
      if(moment.utc(new Date(params.node.data.date), 'DD/MM/YYYY') < currDate || params.node.data.isorderrun >0){
        return false;
      }
      return true;
     },
     cellEditor: 'GridNumericFieldComponent', },
    { field: "helpers", headerName: this.gridData["helpers"],editable:function(params){
      if(moment.utc(new Date(params.node.data.date), 'DD/MM/YYYY') < currDate || params.node.data.isorderrun >0){
        return false;
      }
      return true;
    },
       cellEditor: 'GridNumericFieldComponent' },
    { field: "shiftName", headerName: this.gridData["shiftName"], editable:function(params){
      if(moment.utc(new Date(params.node.data.date), 'DD/MM/YYYY') < currDate || params.node.data.isorderrun >0){
        return false;
      }
      return true;
    }},
    { field: "shiftHours", headerName: this.gridData["shiftHours"], editable:function(params){
      if(moment.utc(new Date(params.node.data.date), 'DD/MM/YYYY') < currDate || params.node.data.isorderrun >0){

        return false;
      }
      return true;
    }},
    {
      field: "date", headerName: this.gridData["date"],
      editable:function(params){
        if(moment.utc(new Date(params.node.data.date), 'DD/MM/YYYY') < currDate || params.node.data.isorderrun >0){
          return false;
        }
        return true;
      }
      , cellEditor: 'GridDatePickerComponent', cellRenderer: (params) => {
        return moment.utc(params.value).format('DD/MM/YY')
      }
    },
    { field: "plannedEffeciency", headerName: this.gridData["plannedEfficiency"], editable:function(params){
    if(moment.utc(new Date(params.node.data.date), 'DD/MM/YYYY') < currDate){
        return false;
      }return true;
    }},
    { field: "plannedTarget", headerName: this.gridData["plannedTarget"] }
  ];
}
}

