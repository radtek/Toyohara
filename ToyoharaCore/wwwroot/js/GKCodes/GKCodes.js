var selectedRecord = '';
var showSelected = null;


function ReturnData(controller) {
   var obj={
        showSelected: showSelected,
        selectedRecord: selectedRecord,
        only_new: document.getElementById('only_new').checked,
        rebind: rebind,
        ekk_guid_list: ReturnEkkNodes('ekk_code_name').selected_node.toString(),
        storedProcedure: "PRC_SELECT_ORDER_ITEMS_GKI",
        controller: controller,
        show_classified: null
    };
    return JSON.stringify(obj);
}

function FiltersBeforeGridString(reloading, clearFilters, fullSearch, excel, showSelected, settings, otherFilters) {

    return $("<button class='btn btn_in_grid dx-button btn_pad_grid' title='Искать' onclick='" + reloading.function + "' id='" + reloading.id + "'><img src='/../../img/GridBtn/1-5.png' style='height:18px; width:auto;' alt='Искать' ></img></button>" +
        "<button title='Показать выбранные' class='btn btn_in_grid dx-button btn_pad_grid' onclick='" + showSelected.function + "' id='" + showSelected.id + "'><img src='/../../img/GridBtn/1-7.png' style='height:18px; width:auto;' alt='Показать выбранные'></img></button>" +
        "<button title='Очистить фильтры' onclick='" + clearFilters.name + "();' class='btn btn_in_grid dx-button btn_pad_grid' id='" + clearFilters.id + "'><img src='/../../img/GridBtn/1-9.png' style='height:18px; width:auto;'  alt='Очистить фильтры'></img></button>" +
        "<button title='Расширенный поиск'  class= 'btn btn_in_grid dx-button btn_pad_grid' data-toggle='modal' onclick='" + fullSearch.function + "' id='" + fullSearch.id + "' ><img src='/../../img/GridBtn/1-10.png' style='height:18px; width:auto;' alt='Расширенный поиск'></img></button>" +
        "<button title='Выгрузить Excel' class='btn btn_in_grid dx-button btn_pad_grid' onclick='" + excel.name + "();' id='" + excel.id + "'><img src='/../../img/GridBtn/1-4.png' style='height:18px; width:auto;' alt='Выгрузить Excel'></img></button>" +
        "<button title='Настройки' class= 'btn btn_in_grid dx-button btn_pad_grid' data-toggle='modal' data-target='#" + settings.name + "' id='" + settings.id + "' ><img src='/../../img/GridBtn/1-6.png' style='height:18px; width:auto;' alt='Настройки'></img></button>" +
        otherFilters);
}
function onToolbarPreparing(e) {
    var dataGrid = e.component;
    e.toolbarOptions.items.unshift({
        location: "after",
        template: FiltersBeforeGridString(
            { function: 'Reloading("Grid")', id: 'Reloading', grid: 'Grid' },
            { name: 'ClearFilters', id: 'ClearFilters' },
            { function: 'SaveFullSearchFilters("FullSearch", "ekk_code_name", "ekk_text"); $("#FullSearch").modal("show");', id: 'FullSearch' },
            { name: 'ExportExcel', id: 'ExportExcel' },
            { function: 'showSelectedPicture(this,"Grid")', id: 'showSelectedPicture', grid: 'Grid' },
            { name: 'UserSettings', id: 'UserSettings' },
            ''
        )
    });
}
function contentReady() {
    rebind = false;
}
function ClearFilters() {
    var dataGrid = $("#Grid").dxDataGrid("instance");
    dataGrid.clearFilter();

}
function onBeforeSend(method, ajaxOptions) {
    if (method === "update") {
        var obj = {
            showSelected: showSelected,
            selectedRecord: selectedRecord,
            only_new: document.getElementById('only_new').checked,
            rebind: rebind,
            ekk_guid_list: ReturnEkkNodes('ekk_code_name').selected_node.toString(),
            storedProcedureSelect: "PRC_SELECT_ORDER_ITEMS_GKI",
            storedProcedureInsert: "PRC_UPDATE_ORDER_ITEM_GKI",
            controller: controller,
            show_classified: null
        };
       // return ;
        rebind = true;
        ajaxOptions.data.additionalParams = JSON.stringify(obj);

        //rebind = true;
        //ajaxOptions.data.showSelected = showSelected;
        //ajaxOptions.data.only_new = document.getElementById('only_new').checked;
        //ajaxOptions.data.ekk_guid_list = ReturnEkkNodes('ekk_code_name').selected_node.toString();
    }
}
rebind = false;
function getOnRowPrepared(e) {
    if (e.rowType === 'data') {
        try {
            if (e.data.color.toString() !== '') {
                e.rowElement.css('background-color', '#' + e.data.color.toString());
            }
        }
        catch (err) { ; }
    }
}
function ExportExcel() {
    var dataGrid = $("#Grid").dxDataGrid("instance");
    $.ajax({
        async: true,
        cache: false,
        url: '/GKCodes/ExcelReport',
        type: 'POST',
        data: {
            filter: dataGrid.option("filterValue") ? JSON.stringify(dataGrid.option("filterValue")) : "",
            sort: $("#Grid").dxDataGrid("getDataSource").loadOptions().sort ? JSON.stringify($("#Grid").dxDataGrid("getDataSource").loadOptions().sort) : "",
            showSelected: showSelected,
            selectedRecord: selectedRecord,
            only_new: document.getElementById('only_new').checked,
            ekk_guid_list: ReturnEkkNodes('ekk_code_name').selected_node.toString()

        },
        success: function (data) {
            window.location = "/Common/ReturnFile?physicalPath=" + data;
        }
    });
}
function ShowSettings() {

}
var records;
function selection_changed(selectedItems) {
    var data = selectedItems.selectedRowsData;
    records = [];
    $.each(data, function (i, val) {
        records.push(val.id);

    });
    selectedRecord = records.toString();
}


var gridDataSource = new DevExpress.data.DataSource({
    load: function (loadOptions) {

        var d = $.Deferred();
        $.getJSON('/GKCodes/Get', {
            skip: loadOptions.skip,
            take: loadOptions.take,
            sort: loadOptions.sort ? JSON.stringify(loadOptions.sort) : "",
            filter: loadOptions.filter ? JSON.stringify(loadOptions.filter) : "",
            requireTotalCount: loadOptions.requireTotalCount,
            totalSummary: loadOptions.totalSummary ? JSON.stringify(loadOptions.totalSummary) : "",
            group: loadOptions.group ? JSON.stringify(loadOptions.group) : "",
            groupSummary: loadOptions.groupSummary ? JSON.stringify(loadOptions.groupSummary) : "",
            requireGroupCount: loadOptions.requireGroupCount,
            some: 1
        }).done(function (result) {
            d.resolve(result.data, {
                totalCount: result.totalCount,
                summary: result.summary
            });
        });
        return d.promise();
    }
});

function GetLink() {
    var link = '/GKCodes/Index';
    return link;
}