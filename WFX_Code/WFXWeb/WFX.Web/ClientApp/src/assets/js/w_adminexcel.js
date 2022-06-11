// Product columns to be present
var productcolumns = {
  'A': 'ProductName'
};

// Productfit columns to be present
var producfittcolumns = {
  'A': 'FitType'
};

// Customer columns to be present
var customercolumns = {
  'A': 'CustomerName'
};
// Shift columns to be present
var shiftcolumns = {
  'A': 'ShiftName',
  'B': 'ShiftStartTime',
  'C': 'ShiftEndTime'

};

// Line columns to be present
var linecolumns = {
  'A': 'LineName',
  'B': 'ProcessType',
  'C': 'InternalLineName',
  'D': 'NoOfMachine',
  'E': 'LineCapacity',
  'F': 'LineloadingPoint',
  'G': 'TabletID',
  'H': 'ModuleName'
};

// QC Code columns to be present
var qccodecolumns = {
  'A': 'DefectCode',
  'B': 'DefectName',
  'C': 'DefectType',
  'D': 'DefectLevel'
};
// Module columns to be present
var modulecolumns = {
  'A': 'ModuleName'

};
// user columns to be present
var usercolumns = {
  'A': 'UserFirstName',
  'B': 'UserLastName',
  'C': 'UserName',
  'D': 'Password',
  'E': 'UserRole',
  'F': 'Module',
  'G': 'UserType',
  'H': 'UserEmail'

};
var processDefinitionColumns = {
  'A': 'ProcessCode',
  'B': 'ProcessName',
  'C': 'ProcessType',
};


// XMLHttpRequest in promise format
function makeRequest(method, url, success, error) {
  /*  alert("Hello in makeRequest");*/

  var httpRequest = new XMLHttpRequest();
  httpRequest.open("GET", url, true);
  httpRequest.responseType = "arraybuffer";

  httpRequest.open(method, url);
  httpRequest.onload = function () {
    /* alert(" makeRequest success");*/
    success(httpRequest.response);
  };
  httpRequest.onerror = function () {
    /*alert(" makeRequest fail");*/
    error(httpRequest.response);
  };
  httpRequest.send();
}

// read the raw data and convert it to a XLSX workbook
function convertDataToWorkbook(data) {
  /* alert("Hello in convertDataToWorkbook");*/
  /* convert data to binary string */
  var data = new Uint8Array(data);
  var arr = new Array();

  for (var i = 0; i !== data.length; ++i) {
    arr[i] = String.fromCharCode(data[i]);
  }
  //alert("Hello in convertDataToWorkbook 2");
  //alert(arr);
  var bstr = arr.join("");
  return XLSX.read(bstr, { type: "binary" });
}

// start product
function populateproductGrid(workbook) {
  var fid = parseInt(sessionStorage.getItem("factoryid"));
  /*  alert("Hello in populateGrid");*/
  // our data is in the first sheet
  var list = [];
  var res = "";

  var firstSheetName = workbook.SheetNames[0];
  var worksheet = workbook.Sheets[firstSheetName];

  var rowData = [];
  var data = {};
  // start at the 2nd row - the first row are the headers
  var rowIndex = 2;
  // iterate over the worksheet pulling out the columns we're expecting
  while (worksheet['A' + rowIndex]) {
    var row = {};
    Object.keys(productcolumns).forEach(function (column) {
      if (worksheet[column + rowIndex]) {
        row[productcolumns[column]] = worksheet[column + rowIndex].w;
      }
    });
    row["FactoryID"] = fid;
    rowData.push(row);
    rowIndex++;
  }

  //var re = JSON.stringify(rowData);
  // alert(re);

  var apiurl = sessionStorage.getItem("apiurl") + 'Product/PostMultiProduct';
  /*alert(apiurl);*/

  $.ajax({
    cache: false,
    url: apiurl,
    headers: { 'Authorization': "Bearer " + sessionStorage.getItem('auth') },
    type: "Post",
    data: JSON.stringify(rowData),
    contentType: 'application/json',
    async: false,
    success: function (data) {
      res = data;
      alert("Upload Successfully");
      sessionStorage.setItem("productexcelfilename", "");
      sessionStorage.setItem("productexcelfilepath", "");
      sessionStorage.setItem("factoryid", "");
    },
    error: function (data) {
      res = data;
      alert("error");
    },
  });
  return res;
}

function importProductExcel() {

  /*  alert("Hello in Import excel");*/
  var filepath = sessionStorage.getItem("productexcelfilepath");
  makeRequest('GET',
    filepath,
    // success
    function (data) {
      /*alert(" Import excel data" );*/
      var workbook = convertDataToWorkbook(data);
      /*alert("workbook create");*/
      populateproductGrid(workbook);
    },
    // error
    function (error) {
      alert("error found");
      throw error;
    }
  );
  //alert("Import excel final call");
}
//end product

// start product fit
function populateproductfitGrid(workbook) {
  var fid = parseInt(sessionStorage.getItem("factoryid"));
  /*alert("Hello in populateGrid");*/
  // our data is in the first sheet
  var list = [];
  var res = "";

  var firstSheetName = workbook.SheetNames[0];
  var worksheet = workbook.Sheets[firstSheetName];

  var rowData = [];
  var data = {};
  // start at the 2nd row - the first row are the headers
  var rowIndex = 2;
  // iterate over the worksheet pulling out the columns we're expecting
  while (worksheet['A' + rowIndex]) {
    var row = {};
    Object.keys(producfittcolumns).forEach(function (column) {
      if (worksheet[column + rowIndex]) {
        row[producfittcolumns[column]] = worksheet[column + rowIndex].w;
      }
    });
    row["FactoryID"] = fid;
    rowData.push(row);
    rowIndex++;
  }
  var re = JSON.stringify(rowData);
  /*   alert(re);*/

  // finally, set the imported rowData into the grid
  //gridOptions.api.setRowData(rowData);

  var apiurl = sessionStorage.getItem("apiurl") + 'ProductFit/PostMultiProductFit';
  /*alert(apiurl);*/
  /*  alert(sessionStorage.getItem('auth'));*/
  $.ajax({
    cache: false,
    url: apiurl,
    headers: { 'Authorization': "Bearer " + sessionStorage.getItem('auth') },
    type: "Post",
    data: JSON.stringify(rowData),
    contentType: 'application/json',
    async: false,
    success: function (data) {
      res = data;
      alert("Upload Successfully");
      sessionStorage.setItem("productfitexcelfilename", "");
      sessionStorage.setItem("productfitexcelfilepath", "");
      sessionStorage.setItem("factoryid", "");
    },
    error: function (data) {
      res = data;
      alert("error");
    },
  });
  return res;
}

//product Fit
function importProductFitExcel() {

  /*alert("Hello in Import excel");*/
  var filepath = sessionStorage.getItem("productfitexcelfilepath");
  makeRequest('GET',
    filepath,
    // success
    function (data) {
      /*alert(" Import excel data" );*/
      var workbook = convertDataToWorkbook(data);
      /*alert("workbook create");*/
      populateproductfitGrid(workbook);
    },
    // error
    function (error) {
      alert("error found");
      throw error;
    }
  );
  //alert("Import excel final call");
}
//end product fit

//start customer
function populatecustomerGrid(workbook) {
  var fid = parseInt(sessionStorage.getItem("factoryid"));
  /*alert("Hello in populateGrid");*/
  // our data is in the first sheet
  var list = [];
  var res = "";

  var firstSheetName = workbook.SheetNames[0];
  var worksheet = workbook.Sheets[firstSheetName];

  var rowData = [];
  var data = {};
  // start at the 2nd row - the first row are the headers
  var rowIndex = 2;
  // iterate over the worksheet pulling out the columns we're expecting
  while (worksheet['A' + rowIndex]) {
    var row = {};
    Object.keys(customercolumns).forEach(function (column) {
      if (worksheet[column + rowIndex]) {
        row[customercolumns[column]] = worksheet[column + rowIndex].w;
      }
    });
    row["FactoryID"] = fid;
    rowData.push(row);
    rowIndex++;
  }
  var re = JSON.stringify(rowData);
  /*   alert(re);*/

  // finally, set the imported rowData into the grid
  //gridOptions.api.setRowData(rowData);

  var apiurl = sessionStorage.getItem("apiurl") + 'Customer/PostMultiCustomer';
  /*  alert(apiurl);*/
  /*  alert(sessionStorage.getItem('auth'));*/
  $.ajax({
    cache: false,
    url: apiurl,
    headers: { 'Authorization': "Bearer " + sessionStorage.getItem('auth') },
    type: "Post",
    data: JSON.stringify(rowData),
    contentType: 'application/json',
    async: false,
    success: function (data) {
      res = data;
      alert("Upload Successfully");
      sessionStorage.setItem("customerexcelfilename", "");
      sessionStorage.setItem("customerexcelfilepath", "");
      sessionStorage.setItem("factoryid", "");
    },
    error: function (data) {
      res = data;
      alert("error");
    },
  });
  return res;
}

function importCustomerExcel() {

  /*alert("Hello in Import excel");*/
  var filepath = sessionStorage.getItem("customerexcelfilepath");
  makeRequest('GET',
    filepath,
    // success
    function (data) {
      /*alert(" Import excel data" );*/
      var workbook = convertDataToWorkbook(data);
      /*alert("workbook create");*/
      populatecustomerGrid(workbook);
    },
    // error
    function (error) {
      alert("error found");
      throw error;
    }
  );
  //alert("Import excel final call");
}
//end customer

//start Line
function populatelineGrid(workbook) {
  var fid = parseInt(sessionStorage.getItem("factoryid"));
  /*  alert("Hello in populateGrid");*/
  // our data is in the first sheet
  var list = [];
  var res = "";

  var firstSheetName = workbook.SheetNames[0];
  var worksheet = workbook.Sheets[firstSheetName];

  var rowData = [];
  var data = {};
  // start at the 2nd row - the first row are the headers

  var rowIndex = 2;

  // iterate over the worksheet pulling out the columns we're expecting

  while (worksheet['A' + rowIndex]) {
    var row = {};
    row["ModuleID"] = 1;
    row["LineName"] = worksheet['A' + rowIndex].w;
    row["ProcessType"] = worksheet['B' + rowIndex].w;
    row["InternalLineName"] = worksheet['C' + rowIndex].w;
    row["NoOfMachine"] = parseInt(worksheet['D' + rowIndex].w);
    row["LineCapacity"] = parseInt(worksheet['E' + rowIndex].w);
    row["LineloadingPoint"] = worksheet['F' + rowIndex].w;
    row["TabletID"] = worksheet['G' + rowIndex].w;
    row["DeviceSerialNo"] = worksheet['H' + rowIndex].w;
    row["ModuleName"] = worksheet['I' + rowIndex].w;
    row["FactoryID"] = fid;
    rowData.push(row);
    rowIndex++;
  }

  //var re = JSON.stringify(rowData);
  //alert(re);

  var apiurl = sessionStorage.getItem("apiurl") + 'Line/PostMultiLine';
  /* alert(apiurl);*/
  /*  alert(sessionStorage.getItem('auth'));*/
  $.ajax({
    cache: false,
    url: apiurl,
    headers: { 'Authorization': "Bearer " + sessionStorage.getItem('auth') },
    type: "Post",
    data: JSON.stringify(rowData),
    contentType: 'application/json',
    async: false,
    success: function (data) {
      res = data;
      alert("Upload Successfully");
      sessionStorage.setItem("lineexcelfilename", "");
      sessionStorage.setItem("lineexcelfilepath", "");
      sessionStorage.setItem("factoryid", "");
    },
    error: function (data) {
      res = data;
      alert("error");
    },
  });
  return res;
}

function importLineExcel() {

  /*  alert("Hello in Line Import excel");*/
  var filepath = sessionStorage.getItem("lineexcelfilepath");
  makeRequest('GET',
    filepath,
    // success
    function (data) {
      /*alert(" Import excel data" );*/
      var workbook = convertDataToWorkbook(data);
      /*alert("workbook create");*/
      populatelineGrid(workbook);
    },
    // error
    function (error) {
      alert("error found");
      throw error;
    }
  );
  //alert("Import excel final call");
}
//end line

//start Shift
function populateshiftGrid(workbook) {
  var fid = parseInt(sessionStorage.getItem("factoryid"));
  /*  alert("Hello in populateshiftGrid");*/
  // our data is in the first sheet
  var list = [];
  var res = "";

  var firstSheetName = workbook.SheetNames[0];
  var worksheet = workbook.Sheets[firstSheetName];

  var rowData = [];
  var data = {};
  // start at the 2nd row - the first row are the headers

  var rowIndex = 2;

  // iterate over the worksheet pulling out the columns we're expecting

  while (worksheet['A' + rowIndex]) {
    var row = {};
    Object.keys(shiftcolumns).forEach(function (column) {
      if (worksheet[column + rowIndex]) {
        row[shiftcolumns[column]] = worksheet[column + rowIndex].w;
      }
    });
    row["ModuleID"] = 1;
    row["FactoryID"] = fid;
    rowData.push(row);
    rowIndex++;
  }

  //var re = JSON.stringify(rowData);
  //alert(re);

  var apiurl = sessionStorage.getItem("apiurl") + 'Shift/PostMultiShift';
  /*alert(apiurl);*/
  /*  alert(sessionStorage.getItem('auth'));*/
  $.ajax({
    cache: false,
    url: apiurl,
    headers: { 'Authorization': "Bearer " + sessionStorage.getItem('auth') },
    type: "Post",
    data: JSON.stringify(rowData),
    contentType: 'application/json',
    async: false,
    success: function (data) {
      res = data;
      alert("Upload Successfully");
      sessionStorage.setItem("shiftexcelfilename", "");
      sessionStorage.setItem("shiftexcelfilepath", "");
    },
    error: function (data) {
      res = data;
      alert("error");
    },
  });
  return res;
}

function importShiftExcel() {

  /*  alert("Hello in Line Import excel");*/
  var filepath = sessionStorage.getItem("shiftexcelfilepath");
  makeRequest('GET',
    filepath,
    // success
    function (data) {
      /*alert(" Import excel data" );*/
      var workbook = convertDataToWorkbook(data);
      /*alert("workbook create");*/
      populateshiftGrid(workbook);
    },
    // error
    function (error) {
      alert("error found");
      throw error;
    }
  );
  //alert("Import excel final call");
}
//end Shift

//start QC Code
function populateqccodeGrid(workbook) {
  var fid = parseInt(sessionStorage.getItem("factoryid"));
  /*  alert("Hello in populateqccodeGrid");*/
  // our data is in the first sheet
  var list = [];
  var res = "";

  var firstSheetName = workbook.SheetNames[0];
  var worksheet = workbook.Sheets[firstSheetName];

  var rowData = [];
  var data = {};
  // start at the 2nd row - the first row are the headers

  var rowIndex = 2;

  // iterate over the worksheet pulling out the columns we're expecting

  while (worksheet['A' + rowIndex]) {
    var row = {};
    Object.keys(qccodecolumns).forEach(function (column) {
      if (worksheet[column + rowIndex]) {
        row[qccodecolumns[column]] = worksheet[column + rowIndex].w;
      }
    });
    row["DepartmentID"] = 1;
    row["FactoryID"] = fid;
    rowData.push(row);
    rowIndex++;
  }

  var re = JSON.stringify(rowData);
  alert(re);

  var apiurl = sessionStorage.getItem("apiurl") + 'Defects/PostMultiDefects';
  /*alert(apiurl);*/
  /*  alert(sessionStorage.getItem('auth'));*/
  $.ajax({
    cache: false,
    url: apiurl,
    headers: { 'Authorization': "Bearer " + sessionStorage.getItem('auth') },
    type: "Post",
    data: JSON.stringify(rowData),
    contentType: 'application/json',
    async: false,
    success: function (data) {
      res = data;
      alert("Upload Successfully");
      sessionStorage.setItem("qccodeexcelfilename", "");
      sessionStorage.setItem("qccodeexcelfilepath", "");
    },
    error: function (data) {
      res = data;
      alert("error");
    },
  });
  return res;
}

function importQCCodeExcel() {

  /*    alert("Hello in qccode Import excel");*/
  var filepath = sessionStorage.getItem("qccodeexcelfilepath");
  makeRequest('GET',
    filepath,
    // success
    function (data) {
      /*alert(" Import excel data" );*/
      var workbook = convertDataToWorkbook(data);
      /*alert("workbook create");*/
      populateqccodeGrid(workbook);
    },
    // error
    function (error) {
      alert("error found");
      throw error;
    }
  );
  //alert("Import excel final call");
}
//end Qc Code

//start Module Code
function populatemoduleGrid(workbook) {
  var fid = parseInt(sessionStorage.getItem("factoryid"));
  /*  alert("Hello in populatemoduleGrid");*/
  // our data is in the first sheet
  var list = [];
  var res = "";

  var firstSheetName = workbook.SheetNames[0];
  var worksheet = workbook.Sheets[firstSheetName];

  var rowData = [];
  var data = {};
  // start at the 2nd row - the first row are the headers

  var rowIndex = 2;

  // iterate over the worksheet pulling out the columns we're expecting

  while (worksheet['A' + rowIndex]) {
    var row = {};
    Object.keys(modulecolumns).forEach(function (column) {
      if (worksheet[column + rowIndex]) {
        row[modulecolumns[column]] = worksheet[column + rowIndex].w;
      }
    });
    row["DepartmentID"] = 1;
    row["FactoryID"] = fid;
    rowData.push(row);
    rowIndex++;
  }

  var re = JSON.stringify(rowData);
  alert(re);

  var apiurl = sessionStorage.getItem("apiurl") + 'Module/PostMultiModule';
  /*alert(apiurl);*/
  /*  alert(sessionStorage.getItem('auth'));*/
  $.ajax({
    cache: false,
    url: apiurl,
    headers: { 'Authorization': "Bearer " + sessionStorage.getItem('auth') },
    type: "Post",
    data: JSON.stringify(rowData),
    contentType: 'application/json',
    async: false,
    success: function (data) {
      res = data;
      alert("Upload Successfully");
      sessionStorage.setItem("moduleexcelfilename", "");
      sessionStorage.setItem("moduleexcelfilepath", "");
    },
    error: function (data) {
      res = data;
      alert("error");
    },
  });
  return res;
}

function importModuleExcel() {
  alert("Hi");
  /*    alert("Hello in module Import excel");*/
  var filepath = sessionStorage.getItem("moduleexcelfilepath");

  makeRequest('GET',
    filepath,
    // success
    function (data) {
      /*alert(" Import excel data" );*/
      var workbook = convertDataToWorkbook(data);
      /*alert("workbook create");*/
      populatemoduleGrid(workbook);
    },
    // error
    function (error) {
      alert("error found");
      throw error;
    }
  );
  //alert("Import excel final call");
}
//end Module

//start user
function populateuserGrid(workbook) {
  var fid = parseInt(sessionStorage.getItem("factoryid"));
  /*  alert("Hello in populateGrid");*/
  // our data is in the first sheet
  var list = [];
  var res = "";

  var firstSheetName = workbook.SheetNames[0];
  var worksheet = workbook.Sheets[firstSheetName];

  var rowData = [];
  var data = {};
  // start at the 2nd row - the first row are the headers
  var rowIndex = 2;

  // iterate over the worksheet pulling out the columns we're expecting

  while (worksheet['A' + rowIndex]) {
    var tbl_UserModules = [];
    var tbl_FactoryUserRoles = [];
    var row = {};
    row["UserFirstName"] = worksheet['A' + rowIndex].w;
    row["UserLastName"] = worksheet['B' + rowIndex].w;
    row["UserName"] = worksheet['C' + rowIndex].w;
    row["Password"] = worksheet['D' + rowIndex].w;
    if (worksheet['F' + rowIndex].w && worksheet['F' + rowIndex].w !== null) {
      let Module = worksheet['F' + rowIndex].w.toString();
      let arrModule = Module.split(',');
      for (let i of arrModule) {
        let moduleData = {
          FactoryID: fid,
          ModuleName: i
        }
        tbl_UserModules.push(moduleData);
      }
      row["tbl_UserModules"] = tbl_UserModules;

    }
    if (worksheet['E' + rowIndex].w && worksheet['E' + rowIndex].w !== null) {
      let UserRole = worksheet['E' + rowIndex].w.toString();
      let arrUserRole = UserRole.split(',');
      for (let i of arrUserRole) {
        let UserRoleData = {
          FactoryID: fid,
          UserRole: i
        }
        tbl_FactoryUserRoles.push(UserRoleData);
      }

      row["tbl_FactoryUserRoles"] = tbl_FactoryUserRoles;

      //Object.keys(usercolumns).forEach(function (column) {
      //  if (worksheet[column + rowIndex]) {
      //    row[usercolumns[column]] = worksheet[column + rowIndex].w;
      //  }
      //});

    }
    row["UserType"] = worksheet['G' + rowIndex].w;
    row["UserEmail"] = worksheet['H' + rowIndex].w;
    row["FactoryID"] = fid;

    rowData.push(row);
    rowIndex++;
  }

  //var re = JSON.stringify(rowData);
  //alert(re);

  var apiurl = sessionStorage.getItem("apiurl") + 'User/PostMultiUser';
  /*alert(apiurl);*/
  /*  alert(sessionStorage.getItem('auth'));*/
  $.ajax({
    cache: false,
    url: apiurl,
    headers: { 'Authorization': "Bearer " + sessionStorage.getItem('auth') },
    type: "Post",
    data: JSON.stringify(rowData),
    contentType: 'application/json',
    async: false,
    success: function (data) {
      res = data;
      alert("Upload Successfully");
      sessionStorage.setItem("userexcelfilename", "");
      sessionStorage.setItem("userexcelfilepath", "");
    },
    error: function (data) {
      res = data;
      alert("error");
    },
  });
  return res;
}

function importUserExcel() {

  /*  alert("Hello in Line Import excel");*/
  var filepath = sessionStorage.getItem("userexcelfilepath");
  makeRequest('GET',
    filepath,
    // success
    function (data) {
      /*alert(" Import excel data" );*/
      var workbook = convertDataToWorkbook(data);
      /*alert("workbook create");*/
      populateuserGrid(workbook);
    },
    // error
    function (error) {
      alert("error found");
      throw error;
    }
  );
  //alert("Import excel final call");
}

//end user

// Process Definition.

function populateProcessDefinitionGrid(workbook) {
  var fid = parseInt(sessionStorage.getItem("factoryid"));
  var res = "";
  var firstSheetName = workbook.SheetNames[0];
  var worksheet = workbook.Sheets[firstSheetName];
  var rowData = [];

  var rowIndex = 2;
  while (worksheet['A' + rowIndex]) {
    var row = {};
    Object.keys(processDefinitionColumns).forEach(function (column) {
      if (worksheet[column + rowIndex]) {
        row[processDefinitionColumns[column]] = worksheet[column + rowIndex].w;
      }
    });
    row["FactoryID"] = fid;
    rowData.push(row);
    rowIndex++;
  }

  var apiurl = sessionStorage.getItem("apiurl") + 'ProcessDefinition/PostMultiProcessDefinition';
  $.ajax({
    cache: false,
    url: apiurl,
    headers: { 'Authorization': "Bearer " + sessionStorage.getItem('auth') },
    type: "Post",
    data: JSON.stringify(rowData),
    contentType: 'application/json',
    async: false,
    success: function (data) {
      res = data;
      alert("Upload Successfully");
      sessionStorage.setItem("processdefinitionexcelfilename", "");
      sessionStorage.setItem("processdefinitionexcelfilepath", "");
    },
    error: function (data) {
      res = data;
      alert("error");
    },
  });
  return res;
}

function importProcessDefinitionExcel() {
  var filepath = sessionStorage.getItem("processdefinitionexcelfilepath");

  makeRequest('GET',
    filepath,
    function (data) {
      var workbook = convertDataToWorkbook(data);
      populateProcessDefinitionGrid(workbook);
    },
    function (error) {
      alert("error found");
      throw error;
    }
  );

}
