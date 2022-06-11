import { Component, Inject, OnDestroy, OnInit } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import * as moment from 'moment';
import { Subject } from 'rxjs';
import { QCService } from '../services/qc.service';
import { BookingListComponent } from './bookinglist.component';
import { LineService } from '../services/line.service';
import { POPlanTargetModel } from 'src/models/POPlanTargetModel';
import { ExcelExportService } from '../services/excelexport.service';
import * as FileSaver from 'file-saver';
import * as XLSX from 'xlsx';
import { TranslateService } from '@ngx-translate/core';
import { OrderService } from '../services/order.service';
const EXCEL_TYPE = 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet;charset=UTF-8';
const EXCEL_EXTENSION = '.xlsx';


@Component({
  selector: 'app-linebooking',
  templateUrl: './targetplanpopup.component.html',

})

export class TargetPlanPopupComponent implements OnInit, OnDestroy {
  public rowSelection;
  disabled: boolean = true;
  columnDefs: any;
  SelectedPOforPlanTarget: object = [];
  POPlanTargetModel: POPlanTargetModel;
  soList: [] = [];
  processList:[] = []
  startdate: Date;
  enddate: Date;
  lstartdate: Date;
  lenddate: Date;
  ProcessCode: string;
  protected OnDestroy = new Subject<void>();
  public defaultColDef;
  gridApi: any;
  gridColumnApi: any;
  rowData: [] = [];
  disableheader: boolean = true;
  radiobuttonSelected : boolean = false;
  selectedRadiobutton: number=0;

  constructor(public dialogRef: MatDialogRef<BookingListComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any,
    private lineservice: LineService,
    private qcservice: QCService,
    private translate:TranslateService,
    private orderservice: OrderService,
    ) {
  }

  ngOnDestroy(): void {
    this.OnDestroy.next();
    this.OnDestroy.complete();
  }
  ngOnInit() {
    this.enddate = new Date();
    this.startdate = new Date();
    this.startdate.setDate(this.startdate.getDate() - 7);

    this.lenddate = new Date();
    this.lstartdate = new Date();
    this.lstartdate.setDate(this.lstartdate.getDate() - 7);
    this.POPlanTargetModel = new POPlanTargetModel();
    this.POPlanTargetModel.polist = this.data.POlist;
    this.POPlanTargetModel.StartDate = moment(this.startdate, 'DD/MM/YYYY').format('YYYY-MM-DD');
    this.POPlanTargetModel.EndDate = moment(this.enddate, 'DD/MM/YYYY').format('YYYY-MM-DD');
    this.getData();
    let lang = sessionStorage.getItem('lang');
      let gridData = this.translate.store.translations[lang].lang;

    this.columnDefs = [
      {
        headerName: '',
        width: 60,
        headerCheckboxSelection: true,
        checkboxSelection: true,
      },

      {
        headerName: gridData["productionorder"],
        field: 'soNo',

      },
      {
        headerName: gridData["workorder"],
        field: 'poNo',

      },
      { headerName: gridData["process"] ,
      field: 'processName',
      },
      {
        headerName: gridData["style"],
        field: 'style',

      },
      {
        field: 'planStDt',
        headerName:  gridData["receivedate"],
        cellRenderer: (params) => {
          return moment.utc(params.value).format('DD/MM/YY')
        },


      },


    ]
    this.defaultColDef = { resizable: true };

    this.qcservice.FillSO({}).subscribe(
      res => {
        res.data.unshift({ id: -1, text: 'All' });
        this.POPlanTargetModel.solist = res.data;
      })
    this.orderservice.FillProcess().subscribe(
      res => {
        res.data.unshift({ id: -1, text: 'All' });
        this.POPlanTargetModel.processlist = res.data;
      })

  }
  processChangeHandler(event: any) {
    if (event != "") {
      this.POPlanTargetModel.ProcessCode = event;
    }
    else {
      this.POPlanTargetModel.ProcessCode = "";
    }
    this.getData();

  }
  poChangeHandler(event: any) {
    if (event == '-1') {
      this.POPlanTargetModel.poNo = ''
    } else {
      this.POPlanTargetModel.poNo = event;
    }
    this.getData();
  }
  soChangeHandler(event: any) {
    if (event == '-1') {
      this.POPlanTargetModel.soNo = ''
    }
    else {
      this.POPlanTargetModel.soNo = event;
    }
    this.getData();
  }
  onClose() {
    this.dialogRef.close();
  }

  onGridReady(params) {
    this.gridApi = params.api;
    this.gridColumnApi = params.columnApi;
  }
  exportAsXLSX() {
    let list =[];
    for(let i of this.gridApi.getSelectedRows()){

    i.ExportAutoFill=this.selectedRadiobutton;
    }
    this.SelectedPOforPlanTarget = this.gridApi.getSelectedRows();
    this.lineservice.GetLineTargetExcelData(this.SelectedPOforPlanTarget).subscribe(res => {
this.exportexcel(res.orderlst, 'Line Target Sample');
    });

  }
  getData() {

    this.lineservice.GetPOforLineTarget(this.POPlanTargetModel).subscribe(
      res => {
        this.rowData = res.data;
      })
  }
  StartDate(val: any) {
    this.POPlanTargetModel.StartDate = val;
    this.getData();
  }

  EndDate(val: any) {
    this.POPlanTargetModel.EndDate = val;
    this.getData();
  }
  onRowSelected(event) {
    if (this.gridApi.getSelectedRows().length > 0) {
      if(this.radiobuttonSelected === true){
      this.disabled = false;
      console.log(this.radiobuttonSelected )

    }else{
      this.disabled = true;
    }
   } else {
console.log(this.radiobuttonSelected )
      this.disabled = true;
    }
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
  onRadioButoonChange(e: any){
    this.radiobuttonSelected = true;
    this.selectedRadiobutton= parseInt(e.target.id);
    if (this.gridApi.getSelectedRows().length > 0 && this.radiobuttonSelected) {
      this.disabled = false;
    }else{
      this.disabled = true;
    }
  }

}
