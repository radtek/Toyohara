function ReturnData(controller) {
    var obj = {
        showSelected: null,
        selectedRecord: null,
        //only_new: document.getElementById('only_new').checked,
        rebind: rebind_obj.rebind,
        //ekk_guid_list: ReturnEkkNodes('ekk_code_name').selected_node.toString(),
        storedProcedure: "APL_SELECT_PROJECT_LIST_INFO2",
        controller: controller,
        hide_closed: $('#hide_closed').prop("checked"),
        id: null,
        show_mine: $('#show_mine').prop("checked")
    };
    return JSON.stringify(obj);
}

function contentReady() {
    rebind_obj.rebind = false;
}

//перечень кнопок для грида
function onToolbarPreparing(e) {
    var dataGrid = e.component;
    e.toolbarOptions.items.unshift(
        {
            location: "before",
            template: function (e) {
                return $(
                    "<button title='Пункты маршрутов' style='padding:6px; margin-right: 5px;' class='btn btn_in_grid dx-button btn_pad_grid btn_color' onclick='ShowRoads();' >Пункты маршрутов</button>" 
                )
            }
        },
        {
            location: "after",
            template: function (e) {
                return $(
                    "<button title='Добавить' class='btn btn_in_grid dx-button btn_pad_grid' onclick='AddItem();' id='AddItem'><img src='/../../img/GridBtn/1-1.png' style='height:18px; width:auto;' alt='Добавить'></img></button>" +
                    "<button title='Редактировать' class='btn btn_in_grid dx-button btn_pad_grid' onclick='UpdateItem();' id='UpdateItem'><img src='/../../img/GridBtn/1-2.png' style='height:18px; width:auto;' alt='Редактировать'></img></button>" +                    
                    "<button class='btn btn_in_grid dx-button btn_pad_grid' title='Обновить' onclick='Reloading(\"Grid\")' id='Reloading'><img src='/../../img/GridBtn/1-5.png' style='height:18px; width:auto;' alt='Искать' ></img></button>" +
                    "<button title='Очистить фильтры' onclick='ClearFilters(\"Grid\")' class='btn btn_in_grid dx-button btn_pad_grid' id='ClearFilters'><img src='/../../img/GridBtn/1-9.png' style='height:18px; width:auto;'  alt='Очистить фильтры'></img></button>" +
                    "<button title='Расширенный поиск'  class= 'btn btn_in_grid dx-button btn_pad_grid' data-toggle='modal' onclick='SaveFullSearchFilters(\"FullSearch\", \"\", \"\"); $(\"#FullSearch\").modal(\"show\");' id='FullSearch' ><img src='/../../img/GridBtn/1-10.png' style='height:18px; width:auto;' alt='Расширенный поиск'></img></button>" +
                    "<button title='Выгрузить Excel' class='btn btn_in_grid dx-button btn_pad_grid' onclick='ExportExcel();' id='ExportExcel'><img src='/../../img/GridBtn/1-4.png' style='height:18px; width:auto;' alt='Выгрузить Excel'></img></button>" +
                    "<button title='Настройки' class= 'btn btn_in_grid dx-button btn_pad_grid' data-toggle='modal' data-target='#UserSettings' id='UserSettings' ><img src='/../../img/GridBtn/1-6.png' style='height:18px; width:auto;' alt='Настройки'></img></button>" +
                    "<button title='История изменения' class='btn btn_in_grid dx-button btn_pad_grid' onclick='ObjectLog()' id='ObjectLog'><img src='/../../img/GridBtn/1-12.png' style='height:18px; width:auto;' alt='Групповое обновление'></img></button>"
                )
            }
        });
}

var rebind_obj = new Object();
rebind_obj.rebind = true;

function onRowClick(e) {
    var component = e.component,
        prevClickTime = component.lastClickTime;
    component.lastClickTime = new Date();
    if (prevClickTime && (component.lastClickTime - prevClickTime < 300)) {
        //Double click code
        //console.log('double click');
        var dataGrid = $("#Grid").dxDataGrid("instance");
        dataGrid.deselectAll();
        dataGrid.selectRows(e.data.id);
        window.location = "/Projects/Objects?project_id=" + e.data.id + "&link_information_param=" + e.data.id ;
    }
    else {
        //Single click code
        //console.log('single click');
    }
}

function Reloading(grid) { rebind_obj.rebind = true; var dataGrid = $("#" + grid).dxDataGrid("getDataSource"); dataGrid.reload(); }
//function ClearFilters() {
//    var dataGrid = $("#Grid").dxDataGrid("instance");
//    dataGrid.clearFilter();
//}

var object_id = new Object();
object_id.find_row_id = FindRowId;
object_id.find_row_idPrjLogNet = PrjLogNetRowId;
object_id.without_id = NullRowId;
var ParamObj = new Object();
ParamObj.paramFunc = ParamFunc;

function NullRowId() {
    return null;
}
function PrjLogNetRowId() {
    var dataGrid = $("#GridPrjLogNet").dxDataGrid("instance");
    var selectedData = dataGrid.getSelectedRowsData();
    return selectedData[0]["id"];
}

function ParamFunc() {
    var dataGrid = $("#Grid").dxDataGrid("instance");
    var selectedData = dataGrid.getSelectedRowsData();
    return selectedData[0]["id"];
}

function FindRowId() {
    var dataGrid = $("#Grid").dxDataGrid("instance");
    var selectedData = dataGrid.getSelectedRowsData();
    return selectedData[0]["id"];
}

function ExportExcel() {
    var dataGrid = $("#Grid").dxDataGrid("instance");
    $.ajax({
        async: true,
        cache: false,
        url: '/Projects/ExcelReport',
        type: 'POST',
        data: {
            filter: dataGrid.option("filterValue") ? JSON.stringify(dataGrid.option("filterValue")) : "",
            sort: $("#Grid").dxDataGrid("getDataSource").loadOptions().sort ? JSON.stringify($("#Grid").dxDataGrid("getDataSource").loadOptions().sort) : "",
            showSelected: null,
            selectedRecord: null,
            hide_closed: $('#hide_closed').prop('checked'),
            show_mine: $('#show_mine').prop("checked")

        },
        success: function (data) {
            window.location = "/Common/ReturnFile?physicalPath=" + data + "&fileDownloadName" + encodeURIComponent("Проекты");
        }
    });
}


function AddItem() {
    //$.ajax({
    //    async: true,
    //    cache: false,
    //    url: "/Common/GridCardPartialUpdate",
    //    type: "Get",
    //    data: {
    //        param_id: null,
    //        selectProc: "APL_SELECT_PROJECT_LIST_INFO2",
    //        flowWindowName: "GridCard",
    //        flowWindowRussianName: "Создание нового проекта",
    //        gridId: "Grid",
    //        binding: true,
    //        //close_Window: true,
    //        updateProc: "APL_UPDATE_PROJECT2",
    //        id_func: "without_id"/*selectedData[0]['id']*/
    //    },
    //    success: function (partialViewResult) {
    //        $("#part").html(partialViewResult);
    //        OpenCard(false, "GridCard", "Grid", null);
    //    }
    //});

    var ParamsForInsert = {id: null};
    $.ajax({
        async: true,
        cache: false,
        url: "/Common/GridCardPartialUpdate",
        type: "Get",
        data: {
            flowWindowName: "GridCard",
            flowWindowRussianName: "Создание нового проекта",
            selectProc: "APL_SELECT_PROJECT_LIST_INFO2",
            grid_setting_param_id: null,
            //нужно для того, что взять данные из грида 
            gridId: "Grid",
            binding: true,
            updateProc: "APL_UPDATE_PROJECT2",
            additionalParams: JSON.stringify(ParamsForInsert),
            gridType: "grid"
        },
        success: function (partialViewResult) {
            var dropdownParams = {};
            $("#part").html(partialViewResult);
            OpenCard(false, "GridCard", "Grid", JSON.stringify(dropdownParams), "grid");
        }
    });

}

function UpdateItem() {
    var dataGrid = $("#Grid").dxDataGrid("instance");
    var selectedData = dataGrid.getSelectedRowsData();
    var selectedKeys = dataGrid.getSelectedRowKeys();
    if (selectedKeys.length == 0)
        return alert("Выберите строку!");
    //$("#GridCard").load('/Common/GridCardPartialUpdate?param_id=' + selectedData[0]['id'] +
    //    '&selectProc=APL_SELECT_PROJECT_LIST_INFO2&flowWindowName=GridCard&flowWindowRussianName=Карточка&gridId=Grid&binding=false' +
    //    '&close_Window=false&updateProc=APL_UPDATE_PROJECT2&id_func=' + selectedData[0]['id']);




    var ParamsForInsert = { id: selectedData[0]['id']};
    $.ajax({
        async: true,
        cache: false,
        url: "/Common/GridCardPartialUpdate",
        type: "Get",
        data: {
            flowWindowName: "GridCard",
            flowWindowRussianName: selectedData[0]['project_short_description'],
            selectProc: "APL_SELECT_PROJECT_LIST_INFO2",
            grid_setting_param_id: null,
            //нужно для того, что взять данные из грида 
            gridId: "Grid",
            binding: true,
            updateProc: "APL_UPDATE_PROJECT2",
            additionalParams: JSON.stringify(ParamsForInsert),
            gridType: "grid"
        },
        success: function (partialViewResult) {
            //var dropdownParams = { "grid.id": selectedData[0]['id'] };
            var dropdownParams = { };
            $("#part").html(partialViewResult);
            OpenCard(true, "GridCard", "Grid", JSON.stringify(dropdownParams), "grid");
        }
    });
        
    //$.ajax({
    //    async: true,
    //    cache: false,
    //    url: "/Common/GridCardPartialUpdate",
    //    type: "Get",
    //    data: {
    //        param_id: selectedData[0]['id'],
    //        selectProc: "APL_SELECT_PROJECT_LIST_INFO2",
    //        flowWindowName: "GridCard",
    //        flowWindowRussianName: selectedData[0]['project_short_description'],
    //        gridId: "Grid",
    //        binding: true,
    //        close_Window: true,
    //        updateProc: "APL_UPDATE_PROJECT2",
    //        id_func: "find_row_id"
    //    },
    //    success: function (partialViewResult) {
    //        $("#part").html(partialViewResult);
    //        //OpenCard(true, "GridCard", "Grid", "paramFunc");
    //        OpenCard(true, "GridCard", "Grid", selectedData[0]['id']);
    //    }
    //});
}

