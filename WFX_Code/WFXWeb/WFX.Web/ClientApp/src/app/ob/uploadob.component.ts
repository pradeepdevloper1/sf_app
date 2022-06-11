import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { ProductService } from '../services/product.service';
import { ProcessDefinitionService } from '../services/processdefinition.service';
import { UploadOBService } from '../services/uploadob.service';
import { bytesToSize as funcbytesToSize } from '../../models/helper';
import { OBFileUpload } from 'src/models/OBFileUpload';
import { TranslateService } from '@ngx-translate/core';

declare function importExcel(OBFileUploadID: number): any;

@Component({
  selector: 'app-uploadob',
  templateUrl: './uploadob.component.html'
})

export class UploadObComponent implements OnInit {
  constructor(private readonly ps: ProductService,
    private readonly pd: ProcessDefinitionService,
    private readonly uob: UploadOBService,
    private router: Router, private http: HttpClient,
    private translate: TranslateService) {
    this.loadScripts();
    this.userID = parseInt(sessionStorage.getItem('userID'));

    let lang = sessionStorage.getItem('lang');
    let gridData = translate.store.translations[lang].lang
    this.colDef = [
      { headerName: gridData['sNo'], field: "sNo" },
      { headerName: gridData['operationCode'], field: "operationCode", editable: true },
      { headerName: gridData['operationName'], field: "operationName", editable: true },
      { headerName: gridData['section'], field: "section", editable: true },
      { headerName: gridData['smv'], field: "smv", editable: true },
      // { headerName: gridData['obLocation'], field: "obLocation", editable: true },
    ];
    this.defaultColDef = {
      resizable: true,
      minWidth: 80,
      flex: 1,
      filter: true,
    };
  }

  productList: any[] = [];
  filteredProductList: any[] = [];
  processDefinitionList: any[] = [];
  obFileUploadDataList: any[] = [];
  selectedProductID: number = 0;
  selectedProcessCode: string = '';
  selectedOBFileUploadID: number = 0;

  gridApi: any
  gridColumnApi: any;
  gridRowData = [];
  colDef = [];
  defaultColDef: any;

  userID: number = 0;
  selecttedFile = null;
  isInvalid: number;
  ExcelFileName: string;
  ExcelFileSize: string;

  ngOnInit(): void {
    this.ps.GetProductList().subscribe(res => {
      this.productList = res;
      if (this.productList && this.productList.length) {
        this.selectedProductID = this.productList[0].productID;
      }
      this.filteredProductList = this.productList;
      this.fillOBFileNames();
    });
    this.pd.GetProcessDefinitionList().subscribe(res => {
      this.processDefinitionList = res;
      if (this.processDefinitionList && this.processDefinitionList.length) {
        this.selectedProcessCode = this.processDefinitionList[0].processCode;
      }
      this.fillOBFileNames();
    });
  }

  onChangeEvent(e: any) {
    // this.posearchtext = e.target.value;
    // this.loadSearch();
    this.filteredProductList = this.productList.filter((p)=>{
      return p.productName.toLowerCase().indexOf(e.target.value.toLowerCase()) >= 0;
    });
  }

  fillOBFileNames() {
    if (this.selectedProductID > 0 && this.selectedProcessCode) {
      this.uob.GetOBFileUploadList({ ProductID: this.selectedProductID, ProcessCode: this.selectedProcessCode }).subscribe(res => {
        this.obFileUploadDataList = res;
        this.setSelectedOBFileUploadIdNBindGrid();
      })
    }
  }

  private setSelectedOBFileUploadIdNBindGrid() {
    if (this.obFileUploadDataList && this.obFileUploadDataList.length) {
      this.selectedOBFileUploadID = this.obFileUploadDataList[0].obFileUploadID;
      this.fillOBFileUpdateData();
    }
  }

  fillOBFileUpdateData() {
    if (this.selectedOBFileUploadID) {
      this.uob.GetOBFileUploadData(this.selectedOBFileUploadID).subscribe(res => {
        this.gridRowData = res;
        if (this.gridRowData && this.gridRowData.length) {
          this.gridRowData.forEach((e, i) => e.sNo = Number(i + 1));
        }
      })
    }
  }

  productClicked(ProductID: number) {
    this.selectedProductID = ProductID;
    this.fillOBFileNames();
    this.gridRowData = [];
  }

  processDefinitionClicked(ProcessCode: string) {
    this.selectedProcessCode = ProcessCode;
    this.fillOBFileNames();
    this.gridRowData = [];
  }

  obFileClicked(OBFileUploadID: number) {
    this.selectedOBFileUploadID = OBFileUploadID;
    this.fillOBFileUpdateData();
  }

  loadScripts() {
    const dynamicScriptsbody = [
      'assets/js/custom.js',
      'assets/js/w_uploadob.js'
    ];

    for (let i = 0; i < dynamicScriptsbody.length; i++) {
      const node = document.createElement('script');
      node.src = dynamicScriptsbody[i];
      node.type = 'text/javascript';
      node.async = false;
      document.getElementsByTagName('body')[0].appendChild(node);
    }
  }
  OnGridReady(params: any): void {
    this.gridApi = params.api;
  }

  handleFileInput(event) {
    this.selecttedFile = <File>event.target.files[0];
    this.ExcelFileName = this.selecttedFile.name;
    this.ExcelFileSize = funcbytesToSize(this.selecttedFile.size);
    if (!this.ExcelFileName.includes('xlsx')) {
      this.isInvalid = 1;
      this.router.navigate(['/User/UploadOB']);
    }
    else {
      this.isInvalid = 0;
      this.onUpload();
    }
  }

  onUpload() {
    const endpoint = 'Upload/Excel/UploadOB/' + this.userID.toString();
    const formData: FormData = new FormData();
    formData.append('file', this.selecttedFile, this.selecttedFile.name);
    var linebookingfilesize = funcbytesToSize(this.selecttedFile.size);
    this.http.post<any>(endpoint, formData)
      .subscribe(res => {
        if (res.status = 200) {
          sessionStorage.setItem("obexcelfilename", res.message);
          sessionStorage.setItem("obexcelfilepath", res.path);
          sessionStorage.setItem("obexcelfilesize", linebookingfilesize);

          const data: OBFileUpload = {
            OBFileUploadID: 0,
            ProductID: this.selectedProductID,
            ProcessCode: this.selectedProcessCode,
            FileName: res.message,
            FilePath: res.path,
            FactoryID: 0,
            UserID: this.userID,
            CreatedOn: new Date()
          }
          this.uob.PostOBFileUpload(data).subscribe(res => {
            if (res.status === 200) {
              this.obFileUploadDataList.push(data);
              importExcel(res.obFileUploadID);
              this.setSelectedOBFileUploadIdNBindGrid();
            }
          });
        }
      });
  }

  saveChanges() {
    var list = [];
    this.gridApi.forEachNode((rowNode) => {
      const data = {
        OBFileUploadDataID: parseInt(rowNode.data.obFileUploadDataID),
        OperationCode: rowNode.data.operationCode,
        OperationName: rowNode.data.operationName,
        Section: rowNode.data.section,
        SMV: rowNode.data.smv
      }
      list.push(data);
    });
    this.uob.PutMultiOBFileUploadData(list).subscribe(
      res => {
        if (res.status === 200) {
          alert("Record saved");
        }
        else {
          alert(res.message);
        }
      },
      error => {
        alert(error.message);
      });
  }
}
