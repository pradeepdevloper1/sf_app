import { Injectable } from "@angular/core";

@Injectable()
export class CommonFunctions {
  constructor() { }

  lookupValue(params, mappings: any[], key: string) {
    let ddlValueCode = 'id';
    let ddlValueText = 'text';
    if (params.colDef.cellEditorParams) {
      if (params.colDef.cellEditorParams.ddlValueCode) {
        ddlValueCode = params.colDef.cellEditorParams.ddlValueCode;
      }
      if (params.colDef.cellEditorParams.ddlValueText) {
        ddlValueText = params.colDef.cellEditorParams.ddlValueText;
      }
    }
    if (params.colDef.cellEditorParams && params.colDef.cellEditorParams.multiple) {
      const selectedText = [];
      if (Array.isArray(key) && key.length) {
        key.forEach(e => {
          selectedText.push(this.filterLookUp(mappings, ddlValueCode, e, ddlValueText));
        });
      } else if (key) {
        selectedText.push(this.filterLookUp(mappings, ddlValueCode, key, ddlValueText));
      }
      return selectedText;
    } else if (!params.colDef.cellEditorParams.multiple && key) {
      return this.filterLookUp(mappings, ddlValueCode, key, ddlValueText);
    }
    return '';
  }

  private filterLookUp(mappings, ddlValueCode, key, ddlValueText) {
    return mappings.find(e => e[ddlValueCode] === key)[ddlValueText];
  }
}
