// we expect the following columns to be present
var columns = {
  'A': 'Module',
  'B': 'SONo',
  'C': 'PONo',
  'D': 'Style',
  'E': 'Fit',
  'F': 'Product',
  'G': 'Season',
  'H': 'Customer',
  'I': 'PlanStDt',
  'J': 'ExFactory',
  'K': 'PrimaryPart',
  'L': 'Part',
  'M': 'Color',
  'N': 'Hexcode',
  'O': 'Fabric',

  'P': 'OrderRemark',
  'Q': 'SizeWise',

  'R': 'S',
  'S': 'S',
  'T': 'S',
  'U': 'S',
  'V': 'S',
  'W': 'S',
  'X': 'S',
  'Y': 'S',
  'Z': 'S',
};

var sizecolumns = {

};

var duplicatesize = [];

var errorlist = [];
var gridOptions = {
  columnDefs: [
    { field: "Module", minWidth: 100 },
    { field: "SONo", minWidth: 100 },
    { field: "PONo", minWidth: 100 },
    { field: "Style", minWidth: 100 },
    { field: "Fit", minWidth: 100 },
    { field: "Product", minWidth: 100 },
    { field: "Season", minWidth: 100 },
    { field: "Customer", minWidth: 100 },
    { field: "PlanStDt", minWidth: 100 },
    { field: "ExFactory", minWidth: 100 },
    { field: "PrimaryPart", minWidth: 100 },
    { field: "Part", minWidth: 100 },
    { field: "Color", minWidth: 100 },
    { field: "Hexcode", minWidth: 100 },
    { field: "Fabric", minWidth: 100 },
    { field: "OrderRemark", minWidth: 100 },
    { field: "SizeWise", minWidth: 100 }
  ],

  defaultColDef: {
    resizable: true,
    minWidth: 80,
    flex: 1,
    filter: true,
    editable: true,
  },
  pagination: true,
  paginationPageSize: 10,
  rowData: []
};
// XMLHttpRequest in promise format
function makeRequest(method, url, success, error) {
  var httpRequest = new XMLHttpRequest();
  httpRequest.open("GET", url, true);
  httpRequest.responseType = "arraybuffer";
  httpRequest.open(method, url);
  httpRequest.onload = function () {
    success(httpRequest.response);
  };
  httpRequest.onerror = function () {
    error(httpRequest.response);
  };
  httpRequest.send();
}

// read the raw data and convert it to a XLSX workbook
function convertDataToWorkbook(data) {
  var data = new Uint8Array(data);
  var arr = new Array();
  for (var i = 0; i !== data.length; ++i) {
    arr[i] = String.fromCharCode(data[i]);
  }
  var bstr = arr.join("");
  return XLSX.read(bstr, { type: "binary" });
}

// pull out the values we're after, converting it into an array of rowData
function populateGrid(workbook) {
  // our data is in the first sheet
  var firstSheetName = workbook.SheetNames[0];
  var worksheet = workbook.Sheets[firstSheetName];
  var rowData = [];
  var rowIndex = 1;
  var count = 0;
  var columnDefs = gridOptions.api.getColumnDefs();
  Object.keys(columns).forEach(function (column) {
    if (worksheet[column + rowIndex]) {
      var sizename = worksheet[column + rowIndex].w;
      count++;
      if (count > 17) {
        duplicatesize.push(sizename);
        columnDefs.push(
          {
            headerName: sizename,
            field: sizename,
          }
        );
        columns[column] = sizename;
        sizecolumns[sizename] = sizename;
      }
    }
  });
  gridOptions.api.setColumnDefs(columnDefs);
  // start at the 2nd row - the first row are the headers
  rowIndex = 2;
  // iterate over the worksheet pulling out the columns we're expecting
  while (worksheet['A' + rowIndex]) {
    var module = worksheet['A' + rowIndex].w;
    /*if (module == "undefined") { return false;}*/
    if (typeof module === "undefined") {
    }
    else {
      var row = {};
      Object.keys(columns).forEach(function (column) {
        if (worksheet[column + rowIndex]) {
          row[columns[column]] = worksheet[column + rowIndex].w;
        }
      });
      rowData.push(row);
    }
    rowIndex++;
  }

  // finally, set the imported rowData into the grid
  gridOptions.api.setRowData(rowData);
}

function importExcel() {
  //var filenamestr = "uploads/excel/" + sessionStorage.getItem("orderexcelfilepath");
  var filepath = sessionStorage.getItem("orderexcelfilepath");
  makeRequest('GET',
    filepath,
    // success
    function (data) {
      var workbook = convertDataToWorkbook(data);
      populateGrid(workbook);
      geterrorcount();
    },
    // error
    function (error) {
      alert("error found");
      throw error;
    }
  );
}

function getlist() {
  var list = [];
  var res = "";
  geterrorcount();
  var errorcount = parseInt($("#errocount").text());
  if (parseInt(errorcount) == 0) {
    gridOptions.api.forEachNode((rowNode, index) => {
      var sizestring = "";
      var totalqty = 0;
      Object.keys(sizecolumns).forEach(function (column) {
        var sizename = sizecolumns[column];
        var sizeqty = parseInt(rowNode.data[sizename]);
        console.log("sizeqty : " + sizename + "-" + sizeqty)
        if (isNaN(sizeqty)) {
          sizeqty = 0;
        }
        if (sizeqty >= 0) {
          totalqty = totalqty + sizeqty;
          if (sizestring == "") { sizestring = sizename + "-" + sizeqty; }
          else { sizestring = sizestring + "," + sizename + "-" + sizeqty; }
        }
      });

      var orderItem = {
        UserID: parseInt(sessionStorage.getItem("userID")),
        FactoryID: parseInt(sessionStorage.getItem("factoryID")),
        OrderID: index,
        Module: rowNode.data.Module,
        SONo: rowNode.data.SONo,
        PONo: rowNode.data.PONo,
        Style: rowNode.data.Style,
        Fit: rowNode.data.Fit,
        Product: rowNode.data.Product,
        Season: rowNode.data.Season,
        Customer: rowNode.data.Customer,
        //PlanStDt: new Date(rowNode.data.PlanStDt),
        //ExFactory: new Date(rowNode.data.ExFactory),
        PlanStDt: moment.utc(rowNode.data.PlanStDt, 'DD/MM/YYYY'),
        ExFactory: moment.utc(rowNode.data.ExFactory, 'DD/MM/YYYY'),
        PrimaryPart: parseInt(rowNode.data.PrimaryPart),
        Part: rowNode.data.Part,
        Color: rowNode.data.Color,
        Hexcode: rowNode.data.Hexcode,
        Fabric: rowNode.data.Fabric,
        Source: '',

        OrderRemark: rowNode.data.OrderRemark,
        IsSizeRun: parseInt(rowNode.data.SizeWise),
        OrderStatus: 1,

        POQty: totalqty,
        SizeList: sizestring
      }
      list.push(orderItem);
    });

    var apiurl = sessionStorage.getItem("apiurl") + 'Order/PostOrder';
    $.ajax({
      cache: false,
      url: apiurl,
      headers: { 'Authorization': "Bearer " + sessionStorage.getItem('auth') },
      type: "Post",
      data: JSON.stringify(list),
      contentType: 'application/json',
      async: false,
      success: function (data) {
        res = data;
        sessionStorage.setItem("orderexcelfilename", "");
        sessionStorage.setItem("orderexcelfilepath", "");
      },
      error: function (data) {
        res = data;
      },
    });
    return res;
  }
  else {
    alert("Please remove all errors before save.");
  }
}

function geterrorcount() {
  var list = [];
  var isAllOk = true;
  var errorcount = 0;
  var oneerror = "";
  $("#errortable").html('');

  // Excel points no 7

  var sizecolumns_count = 0;
  var duplicatesize_count = duplicatesize.length;
  Object.keys(sizecolumns).forEach(function (column) {
    sizecolumns_count++;
  });
  if (duplicatesize_count > sizecolumns_count) {
    errorcount = errorcount + 1;
    oneerror = "<tr><th>Col</th> <td>Duplicate sizes are not allowed .</td></tr>";
    $("#errortable").append(oneerror);
  }

  var prv_pono = "-";
  var curr_pono = "-";
  gridOptions.api.forEachNode((rowNode, index) => {
    var rowno = index + 1;
    curr_pono = rowNode.data.PONo;
    if (curr_pono != prv_pono) {
      prv_pono = curr_pono;
      var added = false;
      $.map(list, function (elementOfArray, indexInArray) {
        if (elementOfArray.PONo == curr_pono) {
          added = true;
        }
      })
      if (!added) {
        list.push({ PONo: curr_pono, RowNo: rowno })
      }
    }

    //// Excel Point No 16 and 9
    var OrderQtySum = 0;
    Object.keys(sizecolumns).forEach(function (column) {
      if (Number.isInteger(parseInt(rowNode.data[column]))) {
        OrderQtySum += parseInt(rowNode.data[column]);
      }
    });
    if (OrderQtySum == 0) {
      errorcount = errorcount + 1;
      oneerror = "<tr><th>Row " + rowno + "</th> <td>Order Quantity cannot be 0.</td></tr>";
      $("#errortable").append(oneerror);
    }
    if (rowNode.data.Module == null || rowNode.data.Module == "") {
      errorcount = errorcount + 1;
      oneerror = "<tr><th>Row " + rowno + "</th> <td>Please fill Module.</td></tr>";
      $("#errortable").append(oneerror);
    }
    if (rowNode.data.SONo == null || rowNode.data.SONo == "") {
      errorcount = errorcount + 1;
      oneerror = "<tr><th>Row " + rowno + "</th> <td>Please fill SONo.</td></tr>";
      $("#errortable").append(oneerror);
    }
    var format = /[`!@#$%^&*()+\-=\[\]{};':"\\|,.<>\/?~]/;
    if (format.test(rowNode.data.SONo)) {
      errorcount = errorcount + 1;
      oneerror = "<tr><th>Row " + rowno + "</th> <td> SONo. should not contain any special characters.</td></tr>";
      $("#errortable").append(oneerror);
    }
    if (rowNode.data.PONo == null || rowNode.data.PONo == "") {
      errorcount = errorcount + 1;
      oneerror = "<tr><th>Row " + rowno + "</th> <td>Please fill PONo.</td></tr>";
      $("#errortable").append(oneerror);
    }
    if (format.test(rowNode.data.PONo)) {
      errorcount = errorcount + 1;
      oneerror = "<tr><th>Row " + rowno + "</th> <td> PONo. should not contain any special characters. </td></tr>";
      $("#errortable").append(oneerror);
    }
    if (rowNode.data.Style == null || rowNode.data.Style == "") {
      errorcount = errorcount + 1;
      oneerror = "<tr><th>Row " + rowno + "</th> <td>Please fill Style.</td></tr>";
      $("#errortable").append(oneerror);
    }
    if (rowNode.data.Fit == null || rowNode.data.Fit == "") {
      errorcount = errorcount + 1;
      oneerror = "<tr><th>Row " + rowno + "</th> <td>Please fill Fit.</td></tr>";
      $("#errortable").append(oneerror);
    }
    if (rowNode.data.Product == null || rowNode.data.Product == "") {
      errorcount = errorcount + 1;
      oneerror = "<tr><th>Row " + rowno + "</th> <td>Please fill Product.</td></tr>";
      $("#errortable").append(oneerror);
    }
    if (rowNode.data.Season == null || rowNode.data.Season == "") {
      errorcount = errorcount + 1;
      oneerror = "<tr><th>Row " + rowno + "</th> <td>Please fill Season.</td></tr>";
      $("#errortable").append(oneerror);
    }
    if (rowNode.data.Customer == null || rowNode.data.Customer == "") {
      errorcount = errorcount + 1;
      oneerror = "<tr><th>Row " + rowno + "</th> <td>Please fill Customer.</td></tr>";
      $("#errortable").append(oneerror);
    }
    if (rowNode.data.PlanStDt == null || rowNode.data.PlanStDt == "") {
      errorcount = errorcount + 1;
      oneerror = "<tr><th>Row " + rowno + "</th> <td>Please fill PlanStDt.</td></tr>";
      $("#errortable").append(oneerror);
    }
    if (rowNode.data.ExFactory == null || rowNode.data.ExFactory == "") {
      errorcount = errorcount + 1;
      oneerror = "<tr><th>Row " + rowno + "</th> <td>Please fill ExFactory.</td></tr>";
      $("#errortable").append(oneerror);
    }
    if (rowNode.data.PrimaryPart == null || rowNode.data.PrimaryPart == "") {
      errorcount = errorcount + 1;
      oneerror = "<tr><th>Row " + rowno + "</th> <td>Please fill PrimaryPart.</td></tr>";
      $("#errortable").append(oneerror);
    }
    if (rowNode.data.Part == null || rowNode.data.Part == "") {
      errorcount = errorcount + 1;
      oneerror = "<tr><th>Row " + rowno + "</th> <td>Please fill Part.</td></tr>";
      $("#errortable").append(oneerror);
    }
    if (rowNode.data.Color == null || rowNode.data.Color == "") {
      errorcount = errorcount + 1;
      oneerror = "<tr><th>Row " + rowno + "</th> <td>Please fill Color.</td></tr>";
      $("#errortable").append(oneerror);
    }
    if (rowNode.data.Hexcode == null || rowNode.data.Hexcode == "") {
      errorcount = errorcount + 1;
      oneerror = "<tr><th>Row " + rowno + "</th> <td>Please fill Hexcode.</td></tr>";
      $("#errortable").append(oneerror);
    }
    if (rowNode.data.Fabric == null || rowNode.data.Fabric == "") {
      errorcount = errorcount + 1;
      oneerror = "<tr><th>Row " + rowno + "</th> <td>Please fill Fabric.</td></tr>";
      $("#errortable").append(oneerror);
    }
    var PlanStDtStr = rowNode.data.PlanStDt;
    if (PlanStDtStr.length != 8) {
      errorcount = errorcount + 1;
      oneerror = "<tr><th>Row " + rowno + "</th> <td>please follow dd/mm/yy.</td></tr>";
      $("#errortable").append(oneerror);
    }
    var arr = PlanStDtStr.split('/');
    if (arr.length != 3) {
      errorcount = errorcount + 1;
      oneerror = "<tr><th>Row " + rowno + "</th> <td>please follow dd/mm/yy.</td></tr>";
      $("#errortable").append(oneerror);
    }
    else {
      var month = arr[1];
      if (month > 12 || month < 1) {
        errorcount = errorcount + 1;
        oneerror = "<tr><th>Row " + rowno + "</th> <td>please follow dd/mm/yy.</td></tr>";
        $("#errortable").append(oneerror);
      }
    }
    var PlanStDt = moment.utc(rowNode.data.PlanStDt, 'DD/MM/YYYY');
    var ExFactory = moment.utc(rowNode.data.ExFactory, 'DD/MM/YYYY');
    const dateObj = new Date();
    var CurrDate = moment.utc(dateObj.getDate(), 'DD/MM/YYYY');

    // Excel Point No 1
    if (PlanStDt > ExFactory) {
      errorcount = errorcount + 1;
      oneerror = "<tr><th>Row " + rowno + "</th> <td>Ex-Factory date cannot be before plan start date.</td></tr>";
      $("#errortable").append(oneerror);
    }

    // Excel Point No 12
    if (PlanStDt < CurrDate) {
      errorcount = errorcount + 1;
      oneerror = "<tr><th>Row " + rowno + "</th> <td>Planned date can't be before than the current date.</td></tr>";
      $("#errortable").append(oneerror);
    }

    // Excel Point No 13
    if (ExFactory < CurrDate) {
      errorcount = errorcount + 1;
      oneerror = "<tr><th>Row " + rowno + "</th> <td>Ex-Factory can't be before than the current date.</td></tr>";
      $("#errortable").append(oneerror);
    }

    // Excel Point No 5
    //If quantity mentioned against size is negative
    var sizestring = "";
    var poqty = 0;
    Object.keys(sizecolumns).forEach(function (column) {
      var sizename = sizecolumns[column];
      var sizeqty = parseInt(rowNode.data[sizename])
      if (sizeqty < 0) {
        errorcount = errorcount + 1;
        oneerror = "<tr><th>Row " + rowno + "</th> <td>Size quantity can’t be negative.</td></tr>";
        $("#errortable").append(oneerror);
      }
    });


    // Excel Point No 6
    //if hex code mentioned against a color is invalid(more than 6 characters)
    if (rowNode.data.Hexcode.length > 7) {
      errorcount = errorcount + 1;
      oneerror = "<tr><th>Row " + rowno + "</th> <td>hexcode is invalid.</td></tr>";
      $("#errortable").append(oneerror);
    }
    // Excel Point No 11
    //In primary part user can only fill 1 or 0, if it fills anything other than that
    if (parseInt(rowNode.data.PrimaryPart) < 0 || parseInt(rowNode.data.PrimaryPart) > 1) {
      errorcount = errorcount + 1;
      oneerror = "<tr><th>Row " + rowno + "</th> <td>Primary part can only 1 or 0.</td></tr>";
      $("#errortable").append(oneerror);
    }

    //if i upload an order with is size as 0,
    //user should not be able to add more than one size in that particular order,
    //it's part and other colors as well
    //system should display an error message"this order can have only one size"
    var sizewise = rowNode.data.SizeWise;
    if (sizewise == 0) {
      var sizecount = 0;
      var sizefound = false;
      Object.keys(sizecolumns).forEach(function (column) {
        sizecount = sizecount + 1;
        var sizename = sizecolumns[column];
        var sizeqty = parseInt(rowNode.data[sizename])
        if (sizecount > 1) {
          if (sizeqty > 0) {
            sizefound = true;
          }
        }
      });
      if (sizefound) {
        errorcount = errorcount + 1;
        oneerror = "<tr><th>Row " + rowno + "</th> <td>this order can have only one size</td></tr>";
        $("#errortable").append(oneerror);
      }
    }
    //Close

    if (parseInt(rowNode.data.SizeWise) < 0 || parseInt(rowNode.data.SizeWise) > 1) {
      errorcount = errorcount + 1;
      oneerror = "<tr><th>Row " + rowno + "</th> <td>SizeWise can only 1 or 0.</td></tr>";
      $("#errortable").append(oneerror);
    }
    
 // Check SONO exist or not
 var data = {
  SONo: rowNode.data.SONo,
  FactoryID: parseInt(sessionStorage.getItem("factoryID"))
}
var apiurl = sessionStorage.getItem("apiurl") + 'Order/PostIsSOExist';
$.ajax({
  cache: false,
  url: apiurl,
  headers: { 'Authorization': "Bearer " + sessionStorage.getItem('auth') },
  type: "POST",
  data: JSON.stringify(data),
  contentType: 'application/json',
  async: false,
  success: function (data) {
    res = data;
  },
  error: function (data) {
    res = data;
  },
});
if (res.status == 200) {
  var Style=rowNode.data.Style;
  if(res.data.style!=Style){
    errorcount = errorcount + 1;
    oneerror = "<tr><th>Row " + rowno + " </th> <td>SONo " + data.SONo +" already exist  but style "+Style+" doesn't match.</td></tr>";
    $("#errortable").append(oneerror);
  }

  var Fit=rowNode.data.Fit;
  if(res.data.fit!=Fit){
    errorcount = errorcount + 1;
    oneerror = "<tr><th>Row " + rowno + " </th> <td>SONo "+data.SONo+" exist but Fit "+Fit+" doesn't match.</td></tr>";
    $("#errortable").append(oneerror);
  }

  var Product=rowNode.data.Product;
  if(res.data.product!=Product){
    errorcount = errorcount + 1;
    oneerror = "<tr><th>Row " + rowno + " </th> <td>SONo " + data.SONo +" already already exist but Product "+Product+" doesn't match.</td></tr>";
    $("#errortable").append(oneerror);
  }

  var Season=rowNode.data.Season;
  if(res.data.season!=Season){
    errorcount = errorcount + 1;
    oneerror = "<tr><th>Row " + rowno + " </th> <td>SONo " + data.SONo +" already already exist but Season "+Season+" doesn't match.</td></tr>";
    $("#errortable").append(oneerror);
  }

  var Customer=rowNode.data.Customer;
  if(res.data.customer!=Customer){
    errorcount = errorcount + 1;
    oneerror = "<tr><th>Row " + rowno + " </th> <td>SONo " + data.SONo +" already exist but Customer "+Customer+" doesn't match.</td></tr>";
    $("#errortable").append(oneerror);
  }
}



    // Check Module exist or not
    var data = {
      ModuleName: rowNode.data.Module,
      FactoryID: parseInt(sessionStorage.getItem("factoryID"))
    }
    var apiurl = sessionStorage.getItem("apiurl") + 'Module/PostIsModuleExist';
    $.ajax({
      cache: false,
      url: apiurl,
      headers: { 'Authorization': "Bearer " + sessionStorage.getItem('auth') },
      type: "POST",
      data: JSON.stringify(data),
      contentType: 'application/json',
      async: false,
      success: function (data) {
        res = data;
      },
      error: function (data) {
        res = data;
      },
    });
    if (res.status == 401) {
      errorcount = errorcount + 1;
      oneerror = "<tr><th>Row " + rowno + " </th> <td>Module doesn’t exist in the factory.</td></tr>";
      $("#errortable").append(oneerror);
    }
    var data = {
      ProductName: rowNode.data.Product,
      FactoryID: parseInt(sessionStorage.getItem("factoryID"))
    }
    var apiurl = sessionStorage.getItem("apiurl") + 'Order/PostIsProductExist';
    $.ajax({
      cache: false,
      url: apiurl,
      headers: { 'Authorization': "Bearer " + sessionStorage.getItem('auth') },
      type: "POST",
      data: JSON.stringify(data),
      contentType: 'application/json',
      async: false,
      success: function (data) {
        res = data;
      },
      error: function (data) {
        res = data;
      },
    });
    if (res.status == 401) {
      errorcount = errorcount + 1;
      oneerror = "<tr><th>Row " + rowno + " </th> <td>Product doesn’t exist in the factory.</td></tr>";
      $("#errortable").append(oneerror);
    }
    var data = {
      CustomerName: rowNode.data.Customer,
      FactoryID: parseInt(sessionStorage.getItem("factoryID"))
    }
    var apiurl = sessionStorage.getItem("apiurl") + 'Order/PostIsCustomerExist';
    $.ajax({
      cache: false,
      url: apiurl,
      headers: { 'Authorization': "Bearer " + sessionStorage.getItem('auth') },
      type: "POST",
      data: JSON.stringify(data),
      contentType: 'application/json',
      async: false,
      success: function (data) {
        res = data;
      },
      error: function (data) {
        res = data;
      },
    });
    if (res.status == 401) {
      errorcount = errorcount + 1;
      oneerror = "<tr><th>Row " + rowno + " </th> <td>Customer doesn’t exist in the factory.</td></tr>";
      $("#errortable").append(oneerror);
    }
    var data = {
      FitType: rowNode.data.Fit,
      FactoryID: parseInt(sessionStorage.getItem("factoryID"))
    }
    var apiurl = sessionStorage.getItem("apiurl") + 'Order/PostIsFitExist';
    $.ajax({
      cache: false,
      url: apiurl,
      headers: { 'Authorization': "Bearer " + sessionStorage.getItem('auth') },
      type: "POST",
      data: JSON.stringify(data),
      contentType: 'application/json',
      async: false,
      success: function (data) {
        res = data;
      },
      error: function (data) {
        res = data;
      },
    });
    if (res.status == 401) {
      errorcount = errorcount + 1;
      oneerror = "<tr><th>Row " + rowno + " </th> <td>Fit doesn’t exist in the factory.</td></tr>";
      $("#errortable").append(oneerror);
    }

  });

  let FactoryID = parseInt(sessionStorage.getItem("factoryID"))

  $.each(list, function (listindex, value) {
    var res = "";
    // Excel Point No 8
    var data = {
      PONo: value.PONo,
      FactoryID: FactoryID
    }
    var apiurl = sessionStorage.getItem("apiurl") + 'Order/PostIsPOExist';
    $.ajax({
      cache: false,
      url: apiurl,
      headers: { 'Authorization': "Bearer " + sessionStorage.getItem('auth') },
      type: "POST",
      data: JSON.stringify(data),
      contentType: 'application/json',
      async: false,
      success: function (data) {
        res = data;
      },
      error: function (data) {
        res = data;
      },
    });

    if (res.status == 200) {
      errorcount = errorcount + 1;
      oneerror = "<tr><th>Row " + value.RowNo + " </th> <td>" + value.PONo + " is already in the system.</td></tr>";
      $("#errortable").append(oneerror);
    }

    var noofmainpart = 0;

    var firstpoindex = 0;
    var noofporow = 0;

    var Style = "";
    var Fit = "";
    var Product = "";
    var Season = "";
    var Customer = "";
    var SONo = "";
    var Color = "";
    var PONo = " ";
    var Part = "";
    var SizeWise = 0;
    gridOptions.api.forEachNode((rowNode, index) => {
      var rowno = index + 1;

      if (value.PONo == rowNode.data.PONo) {

        // Excel Point No 3 and 14
        if (firstpoindex == 0) {
          Style = rowNode.data.Style;
          Fit = rowNode.data.Fit;
          Product = rowNode.data.Product;
          Season = rowNode.data.Season;
          Customer = rowNode.data.Customer;
          SONo = rowNode.data.SONo;
          Color = rowNode.data.Color;
          Part = rowNode.data.Part;
          PONo = rowNode.data.PONo;
          SizeWise = parseInt(rowNode.data.SizeWise);
          firstpoindex = rowno;
        }
        else {
          //alert("value.PONo " + value.PONo);
          if (Style != rowNode.data.Style || Fit != rowNode.data.Fit || Product != rowNode.data.Product || Season != rowNode.data.Season || Customer != rowNode.data.Customer || SONo != rowNode.data.SONo) {
            errorcount = errorcount + 1;
            oneerror = "<tr><th>Row " + rowno + "</th> <td>Duplicate PO's are not allowed</td></tr>";
            $("#errortable").append(oneerror);
          }

          //if user uploads a mulit color/multi part order then the issize value should be same for all the rows
          //sytem should display the error message"size wise value should be same for all rows of an order  "
          if (SizeWise != parseInt(rowNode.data.SizeWise)) {
            errorcount = errorcount + 1;
            oneerror = "<tr><th scope='row'>Row " + rowno + "</th> <td>Size wise value should be same for all rows of an order .</td></tr>";
            $("#errortable").append(oneerror);
          }
          if (Color == rowNode.data.Color && PONo == rowNode.data.PONo && Part == rowNode.data.Part) {
            errorcount = errorcount + 1;
            oneerror = "<tr><th>Row " + rowno + "</th> <td>Duplicate records not allowed for same PO, Part and Color</td></tr>";
            $("#errortable").append(oneerror);
          }
        }
      }
    });
  });
  $("#errocount").html(errorcount);
  if (errorcount > 0) {
    $(".edit-alert-primary").show();
    $(".edit-alert-green").hide();
  }
  else {
    $(".edit-alert-primary").hide();
    $(".edit-alert-green").show();
  }
  nexterror();
}

function nexterror() {
  var rowCount = $('#errortable tr').length;
  var curr_row = $('#errortable').find("tr th.activered").parent('tr').index();
  curr_row = curr_row + 1;
  if (curr_row < rowCount) {
    $('#errortable').find('tr th').each(function (index, element) {
      $(element).removeClass("activered");
    });
    $('#errortable').find('tr td').each(function (index, element) {
      $(element).removeClass("activered");
    });

    $('#errortable').find("tr:eq(" + curr_row + ") th").addClass("activered");
    $('#errortable').find("tr:eq(" + curr_row + ") td").addClass("activered");

  }
  var row = $('#errortable').find("tr:eq(" + curr_row + ")");
  var w = $('#errordiv');
  if (row.length) {
    w.scrollTop(row.offset().top - (w.height() / 2));
  }
}

function prverror() {
  var rowCount = $('#errortable tr').length;
  var curr_row = $('#errortable').find("tr th.activered").parent('tr').index();
  curr_row = curr_row - 1;
  if (curr_row >= 0) {
    $('#errortable').find('tr th').each(function (index, element) {
      $(element).removeClass("activered");
    });
    $('#errortable').find('tr td').each(function (index, element) {
      $(element).removeClass("activered");
    });

    $('#errortable').find("tr:eq(" + curr_row + ") th").addClass("activered");
    $('#errortable').find("tr:eq(" + curr_row + ") td").addClass("activered");

    var row = $('#errortable').find("tr:eq(" + curr_row + ")");
    var w = $('#errordiv');
    if (row.length) {
      w.scrollTop(row.offset().top - (w.height() / 2));
    }
  }
}

$(document).ready(function () {
  // lookup the container we want the Grid to use
  var eGridDiv = document.querySelector('#myGrid');
  // create the grid passing in the div to use together with the columns & data we want to use
  new agGrid.Grid(eGridDiv, gridOptions);
  importExcel();
});
