import {
  Component, OnInit, Renderer2, ViewChild, ViewContainerRef,
} from "@angular/core";


@Component({
  selector: "GridSlideToggle",
  template: `
      <div>
      <mat-slide-toggle class="mat-primary" (change)="checkedHandler($event)" [checked]="checked"></mat-slide-toggle>
      </div>
  `,
})
export class GridSlideToggleComponent implements OnInit {
  private params: any;
checked:boolean=false;
  constructor(private rd: Renderer2) { }

  agInit(params: any): void {
    this.params = params;
    if(params.data.processEnabled==1 || params.data.processEnabled==true )
{
  this.checked=true;
}
  }
  ngOnInit() {

    }
  ngAfterViewInit(): void {
  }

  checkedHandler(event) {
    let colId = this.params.column.colId;
    this.params.node.setDataValue(colId, event.checked);
}

}
