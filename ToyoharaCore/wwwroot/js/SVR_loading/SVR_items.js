var selectedRecord = '';
var showSelected = null;
var records;
 rebind = false;



//функция для перезагрзуки страницы при обновлении Грида
function GetLink() {
    var link = '/SVR_loading/SVR_items/?svr_id='+document.getElementById("svr_id").value;
    return link;
}

//при чтении контента положить данные в кэш
function contentReady() {
    rebind = false;
}


//функция очищения фильтров
function ClearFilters() {
    var dataGrid = $("#Grid").dxDataGrid("instance");
    dataGrid.clearFilter();
}


//выгрузка в excel грида с позициями загрузки
function ExportExcel() {
    var dataGrid = $("#Grid").dxDataGrid("instance");
    $.ajax({
        async: true,
        cache: false,
        url: '/SVR_loading/ExcelReportSVRitems',
        type: 'POST',
        data: {
            filter: dataGrid.option("filterValue") ? JSON.stringify(dataGrid.option("filterValue")) : "",
            sort: $("#Grid").dxDataGrid("getDataSource").loadOptions().sort ? JSON.stringify($("#Grid").dxDataGrid("getDataSource").loadOptions().sort) : "",
            showSelected: showSelected,
            selectedRecord: selectedRecord,
            svr_id: parseFloat(document.getElementById("svr_id").value)
        },
        success: function (data) {
            window.location = "/Common/ReturnFile?physicalPath=" + data + "&fileDownloadName=" + encodeURIComponent("Позиции СВР (код:" + document.getElementById("svr_id").value+")");
        }
    });
}

//маппинг кнопок и действия
function FiltersBeforeGridStringItems(reloading, clearFilters, excel, settings, add, otherFilters) {
    return $("<button class='btn btn_in_grid dx-button btn_pad_grid' title='Искать' onclick='" + reloading.function + "' id='" + reloading.id + "'><img src='/../../img/GridBtn/1-5.png' style='height:18px; width:auto;' alt='Искать' ></img></button>" +
        "<button title='Очистить фильтры' onclick='" + clearFilters.name + "();' class='btn btn_in_grid dx-button btn_pad_grid' id='" + clearFilters.id + "'><img src='/../../img/GridBtn/1-9.png' style='height:18px; width:auto;'  alt='Очистить фильтры'></img></button>" +
        "<button title='Выгрузить Excel' class='btn btn_in_grid dx-button btn_pad_grid' onclick='" + excel.name + "();' id='" + excel.id + "'><img src='/../../img/GridBtn/1-4.png' style='height:18px; width:auto;' alt='Выгрузить Excel'></img></button>" +
        "<button title='Настройки' class= 'btn btn_in_grid dx-button btn_pad_grid' data-toggle='modal' data-target='#" + settings.name + "' id='" + settings.id + "' ><img src='/../../img/GridBtn/1-6.png' style='height:18px; width:auto;' alt='Настройки'></img></button>" +
        otherFilters);
}

//перечень кнопок для грида
function onToolbarPreparing(e) {
    var dataGrid = e.component;
    e.toolbarOptions.items.unshift({
        location: "after",
        template: FiltersBeforeGridStringItems(
            { function: 'Reloading("Grid")', id: 'Reloading', grid: 'Grid' },
            { name: 'ClearFilters', id: 'ClearFilters' },
            { name: 'ExportExcel', id: 'ExportExcel' },
            { name: 'UserSettings', id: 'UserSettings' },
            ''
        )
    });
}


//заполнение модели для грида
function ReturnData(controller) {
    var obj = {
        showSelected: null,
        selectedRecord: '',
        id: null,
        SVR_id: parseFloat(document.getElementById("svr_id").value),
        rebind: rebind,
        storedProcedure: "OMC_SELECT_SVR_ITEMS",
        controller: controller
    };
    return JSON.stringify(obj);
}






