import { Component, Inject, OnInit } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Router } from '@angular/router';
import { environment } from 'src/environments/environment';
import { PlanModel } from '../../models/PlanModel';
import { PlanService } from '../services/plan.service';
import { OrderService } from '../services/order.service';
import { Select2OptionData } from 'ng-Select2';
import * as moment from 'moment';
import { ActivatedRoute } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { bytesToSize as funcbytesToSize } from '../../models/helper';
import { LineService } from '../services/line.service';
import * as FileSaver from 'file-saver';
import * as XLSX from 'xlsx';
import { MatSnackBar } from '@angular/material/snack-bar';
import { LineTargetModel } from 'src/models/LineTargetModel';
import { GridDatePickerComponent } from '../commonlibrary/griddatepicker.component';
import { GridNumericFieldComponent } from '../commonlibrary/gridnumericfield.component';
const EXCEL_TYPE = 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet;charset=UTF-8';
const EXCEL_EXTENSION = '.xlsx';

@Component({
  selector: 'app-order',
  templateUrl: './planeditor.component.html'
})

export class PlanEditorComponent implements OnInit {
  UserName: string;
  Url: string;
  header: any;
  auth: string;
  selecttedFile = null;
  isInvalid: number;
  ExcelFileName: string;
  ExcelFileSize: string;
  userID: number = 0;
  showButton: boolean = false;
  updatedRows = [];
  LineTargetModel: LineTargetModel;
  columnDefs = [
    { field: "module" },
    { field: "line" },
    { field: "style" },
    { field: "soNo" },
    { field: "poNo" },
    { field: "quantity" },
    { field: "smv" },
    { field: "plannedEffeciency" ,headerName:"Planned Efficiency"},
    {
      field: "startDate",
      cellRenderer: (params) => {
        return moment.utc(params.value).format('DD/MM/YY')
      }
    },
    {
      field: "endDate",
      cellRenderer: (params) => {
        return moment.utc(params.value).format('DD/MM/YY')
      }
    }
  ];
  frameworkComponents = {
    GridDatePickerComponent: GridDatePickerComponent,
    GridNumericFieldComponent: GridNumericFieldComponent

  }
  rowData: any;

  target_rowData: any;
  targetstartdate: string;
  targetenddate: string;

  planmodel: PlanModel;
  line: string;
  module: string;
  po: string;

  startdate: Date;
  enddate: Date;

  lstartdate: Date;
  lenddate: Date;
  poforTargetPlan:object=[];
  public target_columnDefs;
  constructor(
    private router: Router,
    private http: HttpClient,
    private planservice: PlanService,
    private orderservice: OrderService,
    @Inject('BASE_URL') baseUrl: string,
    private _Activatedroute: ActivatedRoute,
    private translate:TranslateService,
    private lineservice: LineService,
    private msg: MatSnackBar,
  ) {
    this.loadScripts();
    this.UserName = sessionStorage.getItem('userFirstName') + " " + sessionStorage.getItem('userLastName').replace("-", "");
    this.Url = environment.apiUrl;
    this.auth = sessionStorage.getItem('auth');
    let lang = sessionStorage.getItem('lang');
    let gridData = translate.store.translations[lang].lang;



    this.target_columnDefs = [
      { field: "module",headerName: gridData["module"]},
      { field: "section",headerName: gridData["section"]  },
      { field: "line",headerName: gridData["line"], editable: true},
      { field: "style" ,headerName: gridData["style"]},
      { field: "soNo" ,headerName: gridData["sono"]},
      { field: "poNo" ,headerName: gridData["pono"]},
      { field: "part" ,headerName: gridData["part"]},
      { field: "color",headerName: gridData["color"] },
      { field: "smv",headerName: gridData["smv"] , editable: true},
      { field: "operators",headerName: gridData["operators"], editable: true, cellEditor: 'GridNumericFieldComponent' },
      { field: "helpers",headerName: gridData["helpers"],  editable: true, cellEditor: 'GridNumericFieldComponent' },
      { field: "shiftName" ,headerName: gridData["shiftName"], editable: true},
      { field: "shiftHours",headerName: gridData["shiftHours"], editable: true },
      {
        field: "date",headerName: gridData["date"],editable: true, cellEditor: 'GridDatePickerComponent',
         cellRenderer: (params) => {
          return moment.utc(params.value).format('DD/MM/YY')
        }
      },
      { field: "plannedEffeciency" ,headerName: gridData["plannedEfficiency"], editable: true},
      { field: "plannedTarget",headerName: gridData["plannedTarget"]}
    ];
  }

  ngOnInit() {
    this._Activatedroute.paramMap.subscribe(params => {
      this.planmodel = new PlanModel();

      this.planmodel.pono = params.get('pono');
      this.enddate = new Date();
      this.startdate = new Date();
      this.startdate.setDate(this.startdate.getDate() - 7);

      this.orderservice.POView(this.planmodel).subscribe(
        res => {
          //this.poviewmodel = res.data;
          this.planmodel.sono = res.data["soNo"];
          console.log(res.data);
          this.orderservice.GetPOLineList(this.planmodel.pono).subscribe(
            res => {
              this.planmodel.linelist = res.data;
              console.log(res);
            })

          this.orderservice.GetPOModuleList(this.planmodel.pono).subscribe(
            res => {
              this.planmodel.modulelist = res.data;
              console.log(res);
            })
      })

      this.planmodel.IsTargetStartDate = 0;
      this.planmodel.IsTargetEndDate = 0;

      this.LineTargetList();
    });
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
    this.planmodel.Line = this.line;
    this.planmodel.Module = this.module;
    this.planmodel.PONo = this.po;
    this.planmodel.TargetStartDate = moment(this.startdate, 'DD/MM/YYYY').format('YYYY-MM-DD');
    console.log(this.planmodel.TargetStartDate);
    this.planmodel.TargetEndDate = moment(this.enddate, 'DD/MM/YYYY').format('YYYY-MM-DD');
    console.log(this.enddate);
    console.log(this.planmodel.TargetEndDate);

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

  lineChangeHandler(event: any) {
    console.log('value changed: ' + event);
    if (event != "") {
      this.line = event;
    }
    else {
      this.line = "";
    }
    this.LineTargetList();
  }

  moduleChangeHandler(event: any) {
    console.log('value changed: ' + event);
    if (event != "") {
      this.module = event;
    }
    else {
      this.module = "";
    }
    this.LineTargetList();
  }

  TargetStartDate(val: any) {
    //alert("blur1 " + val);
    this.targetstartdate = val;
    this.planmodel.IsTargetStartDate = 1;
    this.LineTargetList();
  }

  TargetEndDate(val: any) {
    //alert("blur1 " + val);
    this.targetenddate = val;
    this.planmodel.IsTargetEndDate = 1;
    this.LineTargetList();
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

  SampleFile() {
    window.location.href = 'sample/linetargetsample.xlsx';
     }
     exportAsXLSX(){
      this.poforTargetPlan=[{
        SONo:this.planmodel.sono,PONo:this.planmodel.pono
            }]
    this.lineservice.GetLineTargetExcelData(this.poforTargetPlan).subscribe(res=>{
      this.exportexcel(res.orderlst, 'Line Target Sample');

       });

    }
     BrowseTargetFile(event) {
      console.log(event);
      this.selecttedFile = <File>event.target.files[0];
      this.ExcelFileName = this.selecttedFile.name;
      this.ExcelFileSize = funcbytesToSize(this.selecttedFile.size);
      console.log(this.ExcelFileName);
      console.log(this.ExcelFileSize);
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
            this.router.navigate(['/User/TargetUpload/',this.planmodel.pono]);
          }
        });
    }
    exportexcel(json: any[],excelFileName:string){
      const worksheet: XLSX.WorkSheet = XLSX.utils.json_to_sheet(json);
      console.log('worksheet',worksheet);
      const workbook: XLSX.WorkBook = { Sheets: { 'data': worksheet }, SheetNames: ['data'] };
      const excelBuffer: any = XLSX.write(workbook, { bookType: 'xlsx', type: 'array' });
      //const excelBuffer: any = XLSX.write(workbook, { bookType: 'xlsx', type: 'buffer' });
      this.saveAsExcelFile(excelBuffer, excelFileName);
    }
    private saveAsExcelFile(buffer: any, fileName: string): void {
      const data: Blob = new Blob([buffer], {
        type: EXCEL_TYPE
      });
      FileSaver.saveAs(data, fileName + '_export_' + new Date().getTime() + EXCEL_EXTENSION);
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
}

