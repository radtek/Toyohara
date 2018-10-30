var selectedRecord = '';
var showSelected = null;
var records;
 rebind = false;



//функция для перезагрзуки страницы при обновлении Грида
function GetLink() {
    var link = '/Forms/SVRItems/?form_id='+document.getElementById("form_id").value;
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
        url: '/Forms/ExcelReportSVRitems',
        type: 'POST',
        data: {
            filter: dataGrid.option("filterValue") ? JSON.stringify(dataGrid.option("filterValue")) : "",
            sort: $("#Grid").dxDataGrid("getDataSource").loadOptions().sort ? JSON.stringify($("#Grid").dxDataGrid("getDataSource").loadOptions().sort) : "",
            showSelected: showSelected,
            selectedRecord: selectedRecord,
            form_id: parseFloat(document.getElementById("form_id").value),
            show_only_for_pricing: document.getElementById('show_only_for_pricing').checked
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
        "<button title='Расширенный поиск'  class= 'btn btn_in_grid dx-button btn_pad_grid' data-toggle='modal' onclick='" + fullSearch.function + "' id='" + fullSearch.id + "' ><img src='/../../img/GridBtn/1-10.png' style='height:18px; width:auto;' alt='Расширенный поиск'></img></button>" +
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
            { function: 'SaveFullSearchFilters("FullSearch", "", ""); $("#FullSearch").modal("show");', id: 'FullSearch' },
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
        form_id: parseFloat(document.getElementById("form_id").value),
        rebind: rebind,
        storedProcedure: "OMC_SELECT_FORM_SVR_ITEMS",
        controller: controller,
        show_only_for_pricing: document.getElementById('show_only_for_pricing').checked
    };
    return JSON.stringify(obj);
}






