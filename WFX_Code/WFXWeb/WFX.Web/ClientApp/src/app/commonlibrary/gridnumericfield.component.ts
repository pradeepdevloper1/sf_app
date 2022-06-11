
import {
  AfterViewInit,
  Component,
  ViewChild,
  ViewContainerRef,
} from '@angular/core';
import { ICellEditorAngularComp } from 'ag-grid-angular';
import { ICellEditorParams } from 'ag-grid-community';

const KEY_BACKSPACE = 'Backspace';
const KEY_DELETE = 'Delete';
const KEY_F2 = 'F2';
const KEY_ENTER = 'Enter';
const KEY_TAB = 'Tab';

@Component({
  selector: 'numeric-cell',
  template: `<input
    #input
    (keydown)="onKeyDown($event)"
    [(ngModel)]="value"
    style="width: 100%;"
  />`,
  styles:[`input:focus {outline:none!important;};
 `]
})
export class GridNumericFieldComponent implements ICellEditorAngularComp, AfterViewInit {
  private params: any;
  public   value: any;
  decimalPlaces=0;
  public highlightAllOnFocus = true;
  private cancelBeforeStart = false;

  @ViewChild('input', { read: ViewContainerRef })
  public input!: ViewContainerRef;

  agInit(params: ICellEditorParams): void {
    this.params = params;
    this.setInitialState(this.params);

    // only start edit if key pressed is a number, not a letter
    this.cancelBeforeStart = !!(
      params.charPress && '1234567890'.indexOf(params.charPress) < 0
    );
  }

  setInitialState(params: any) {
    let startValue;
    let highlightAllOnFocus = true;

    if (params.key === KEY_BACKSPACE || params.key === KEY_DELETE) {
      // if backspace or delete pressed, we clear the cell
      startValue = '';
    } else if (params.charPress) {
      // if a letter was pressed, we start with the letter
      startValue = params.charPress;
      highlightAllOnFocus = false;
    } else {
      // otherwise we start with the current value
      startValue = params.value;
      if (params.key === KEY_F2) {
        highlightAllOnFocus = false;
      }
    }

    this.value = startValue;
    this.highlightAllOnFocus = highlightAllOnFocus;
  }

  getValue(): number {
    if (this.value) {
      return Number(this.value);
    } else {
      if (isNaN(this.value) || !this.value) {
        this.value = 0;
      }
      this.value = this.value ? Number(this.value).toFixed(this.decimalPlaces) : this.value;
    }
    return Number(this.value);
  }


  isCancelBeforeStart(): boolean {
    return this.cancelBeforeStart;
  }

  // will reject the number if it greater than 1,000,000
  // not very practical, but demonstrates the method.
  // isCancelAfterEnd(): boolean {
  //   return this.value > 1000000;
  // }

  onKeyDown(event: any): void {
    if (this.isLeftOrRight(event) || this.deleteOrBackspace(event)) {
      event.stopPropagation();
      return;
    }

    if (
      !this.finishedEditingPressed(event) &&
      !this.isKeyPressedNumeric(event)
    ) {
      if (event.preventDefault) event.preventDefault();
    }
  }

  // dont use afterGuiAttached for post gui events - hook into ngAfterViewInit instead for this
  ngAfterViewInit() {
    window.setTimeout(() => {
      this.input.element.nativeElement.focus();
      if (this.highlightAllOnFocus) {
        this.input.element.nativeElement.select();

        this.highlightAllOnFocus = false;
      } else {
        // when we started editing, we want the caret at the end, not the start.
        // this comes into play in two scenarios:
        //   a) when user hits F2
        //   b) when user hits a printable character
        const length = this.input.element.nativeElement.value
          ? this.input.element.nativeElement.value.length
          : 0;
        if (length > 0) {
          this.input.element.nativeElement.setSelectionRange(length, length);
        }
      }

      this.input.element.nativeElement.focus();
    });
  }

  private isCharNumeric(charStr: string): boolean {
    return !!/\d/.test(charStr);
  }

  private isKeyPressedNumeric(event: any): boolean {
    const charStr = event.key;
    return this.isCharNumeric(charStr);
  }

  private deleteOrBackspace(event: any) {
    return [KEY_DELETE, KEY_BACKSPACE].indexOf(event.key) > -1;
  }

  private isLeftOrRight(event: any) {
    return ['ArrowLeft', 'ArrowRight'].indexOf(event.key) > -1;
  }

  private finishedEditingPressed(event: any) {
    const key = event.key;
    return key === KEY_ENTER || key === KEY_TAB;
  }
}
