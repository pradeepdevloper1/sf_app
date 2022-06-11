import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { environment } from 'src/environments/environment';
import { ProcessDefinitionModel } from 'src/models/ProcessDefinitionModel';
import { CommonFunctions } from '../commonlibrary/commonfunctions';
import { ProcessDefinitionService } from '../services/processdefinition.service';
import { TranslationService } from '../services/translation.service';
import { GridSelectComponent } from '../commonlibrary/gridselect.component';

@Component({
  selector: 'app-processdefinitionlist',
  templateUrl: './processdefinitionlist.component.html',
})

export class ProcessdefinitionlistComponent implements OnInit {

  public columnDefs;
  public processDefinitionModel: ProcessDefinitionModel;
  Url: string;
  auth: string;
  factoryid: number;
  public rowData: any;
  errorMessage: string;
  processTypesList: any;

  public gridApi;
  public gridColumnApi;

  frameworkComponents = {
    GridSelectComponent: GridSelectComponent
  }

  constructor(
    private router: Router,
    private http: HttpClient,
    private processDefinitionService: ProcessDefinitionService,
    private _Activatedroute: ActivatedRoute,
    private cf: CommonFunctions,
    private ts: TranslationService

  ) {
    this.Url = environment.apiUrl;
    this.auth = sessionStorage.getItem('auth');
    this._Activatedroute.paramMap.subscribe(params => {
      this.factoryid = parseInt(params.get('id'));
    });

  }
  updateRowData() {
    this.rowData.forEach(element => {
      const index = this.processTypesList.findIndex(e => e.objectKey === element.processType);
      if (index === -1) {
        element.processType = '';
      }
    });
  }

  createColDef() {
    this.ts.GetData("ProcessTypes").subscribe(
      res => {
        this.processTypesList = res.obj;
        this.updateRowData();
        this.columnDefs = [
          { headerName: 'Process Code', field: "processCode" },
          { headerName: 'Process Name', field: "processName", editable: true },
          {
            headerName: 'Process Type ', field: "processType", minWidth: 100, editable: true,
            cellEditor: 'GridSelectComponent',
            cellEditorParams: {
              values: this.processTypesList,
              ddlValueText: 'translatedString',
              ddlValueCode: 'objectKey'
            },
            valueFormatter: params => {
              return this.cf.lookupValue(params, this.processTypesList, params.value);
            },
          }
        ];
        this.loadScripts();
      })
  };


  SaveProcessDefinition() {
    var list = [];
    this.gridApi.forEachNode((rowNode) => {
      var processdefinition = {
        ProcessDefinitionID: parseInt(rowNode.data.processDefinitionID),
        ProcessCode: rowNode.data.processCode,
        ProcessName: rowNode.data.processName,
        ProcessType: rowNode.data.processType,

      }
      list.push(processdefinition);
    });
    this.processDefinitionModel = new ProcessDefinitionModel();
    this.processDefinitionService.UpdateProcessDefinition(list).subscribe(
      res => {
        if (res.status == 200) {
          alert("Record saved");
          this.router.navigate(['/Admin/FactoryWizard/' + this.factoryid]);
        }
        else {
          this.errorMessage = res.message;
        }
      },
      error => {
        this.errorMessage = error.message;
      });

  }

  ngOnInit(): void {
  }
  loadScripts() {
    const dynamicScriptsbody = [
      'https://unpkg.com/@ag-grid-enterprise/all-modules@25.1.0/dist/ag-grid-enterprise.min.js',
      'assets/js/w_product.js',
      'assets/js/jquery.fancybox.js',
    ];
    for (let i = 0; i < dynamicScriptsbody.length; i++) {
      const node = document.createElement('script');
      node.src = dynamicScriptsbody[i];
      node.type = 'text/javascript';
      node.async = false;
      document.getElementsByTagName('body')[0].appendChild(node);
    }
  }
  onCellValueChanged(event) {
    event.data.modified = true;
  }
  onGridReady(params) {
    this.gridApi = params.api;
    this.gridColumnApi = params.columnApi;
    params.api.sizeColumnsToFit();

    window.setTimeout(() => {
      this.http
        .get(this.Url + "ProcessDefinition/GetProcessDefinitionList/" + this.factoryid.toString(), {
          headers: new HttpHeaders().set('Authorization', "Bearer " + this.auth)
        })
        .subscribe((data) => {
          this.rowData = data;
          this.createColDef();
        });
    }, 1)

  }
}
