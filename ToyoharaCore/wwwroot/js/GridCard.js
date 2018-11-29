Globalize.locale("ru");




function InsertCard(flowWindowName, gridId, bindning, storedProcedure, additionalParams, gridType) {
    additionalParams = JSON.parse(additionalParams);
    var parameters = [];
    var err = '';
    if (gridType == "grid")
        var dataGrid = $("#" + gridId).dxDataGrid("instance");
    else
        var dataGrid = $("#" + gridId).dxTreeList("instance");


    bindning = bindning.toLowerCase();

    var str = $('#' + flowWindowName + ' .grid_field').val();
    var GridSettings = JSON.parse(str);
    var grid_settings = GridSettings.filter(function (elem, i) {
        return (elem.ui_type == "dropdown_id" | elem.ui_type == "dropdown_list_id" );
    });
    $('#' + flowWindowName + ' table tr:visible .td_value.editable').each(function () {
        if ($(this).attr('requd') == 'required') {

            if ($(this).attr('element_type') == 'dropdown_id' | $(this).attr('element_type') == 'dropdown_list_id')
                if ($(this).val() == 0) { alert('Заполните поля, помеченные звездочкой, они обязательны для заполнения.'); type_flag = true; return false; }
            if ($(this).attr('element_type') != 'checkbox' && $(this).attr('element_type') != 'date')
                if ($(this).val() == '') { alert('Заполните поля, помеченные звездочкой, они обязательны для заполнения.'); type_flag = true; return false; }
            if ($(this).attr('element_type') == 'date') {
                var dat = $(this).dxDateBox('instance').option('value');
                if (dat == "" | dat == null | dat == "undefined") { alert('Заполните поля, помеченные звездочкой, они обязательны для заполнения.'); type_flag = true; return false; }

            }
        }
    });
    if (type_flag)
        return;
    var type_flag = false;
    $('#' + flowWindowName + ' table tr:visible .td_value.textbox_int.editable').each(function () {
        if ($(this).val() != '' & !isInt($(this).val())) { alert("Поле " + $(this).attr('rus_name').toLowerCase() + ' должно быть целым числом'); type_flag = true; return false; }
    });
    if (type_flag)
        return;
    $('#' + flowWindowName + ' table tr:visible .td_value.textbox_float.editable').each(function () {
        if ($(this).val() != '' & !isFloat($(this).val())) { alert("Поле " + $(this).attr('rus_name').toLowerCase() + ' должно быть числом с плавающей точкой'); type_flag = true; return false; }
    });
    if (type_flag)
        return;

    $('#' + flowWindowName + ' table tr:visible .td_value.date.editable').each(function () {
        var isValid = $(this).dxDateBox('instance').option('isValid');
        if (!isValid) { type_flag = true; alert("Поле " + $(this).attr('rus_name').toLowerCase() + ' должно быть правильного формата даты'); return false; }
    });
    if (type_flag)
        return;

    $('#' + flowWindowName + ' table tr:visible .td_value').each(function () {
        if ($(this).attr('element_type') == 'checkbox')
            parameters.push({ Name: $(this).attr('name'), Value: $(this).prop('checked') });
        else if ($(this).attr('element_type') == 'date') {
            var dat = $(this).dxDateBox('instance').option('value');
            if (dat != "" && dat != null && dat != "undefined")
                dat = dat.toString().replace(/UTC\s/, "").replace(/GMT.+/, "");
            else
                dat = null
            parameters.push({ Name: $(this).attr('name'), Value: dat });

        } else if ($(this).attr('element_type') == 'div_html')
            parameters.push({ Name: $(this).attr('name'), Value: $(this).html().toString() });
        else
            parameters.push({ Name: $(this).attr('name'), Value: $(this).val() != null ? $(this).val().toString() : '' });
    });

    if (err != '')
        return;
    parameters.push({ Name: 'storedProcedure', Value: storedProcedure });

    var selectedData = dataGrid.getSelectedRowsData()[0];

    for (var propt in selectedData) {

        var index = -1;
        for (var i = 0; i < parameters.length; i++) {
            if (parameters[i].Name == propt) {
                index = 1;
                break;
            }
        }

        if (index == -1)
            parameters.push({ Name: propt, Value: selectedData[propt] });
    }

    if (additionalParams != null && additionalParams != "" && additionalParams != "undefined") {
        for (var k = 0; k < additionalParams.length; k++) {
            parameters = parameters.filter(function (elem, j) {
                return (elem.Name != additionalParams[k].Name)
            });
            parameters.push(additionalParams[k]);
        }
    }
    $.ajax({
        async: true,
        cache: false,
        traditional: true,
        url: '/Common/InsertCard',
        type: 'POST',
        data: {
            procedureParams: JSON.stringify(parameters)
        },
        success: function (data) {
            if (data != "")
                alert(data);
            else {
                if (bindning) {

                    if (gridType == "grid")
                        var dataGrid = $("#" + gridId).dxDataGrid("instance");
                    else
                        var dataGrid = $("#" + gridId).dxTreeList("instance");
                    try { rebind_obj.rebind = true; } catch(err) { rebind = true; }
                    dataGrid.refresh();
                    dataGrid.deselectAll();
                    $('#' + flowWindowName).modal("hide");
                }
            }
        }
    });
}

function OpenCard(AddUpdateFlag, CardId, GridId, dropdownParams, gridType) {
    var GridSettings = JSON.parse($('#' + CardId + ' .grid_field').val());

    if (gridType == "grid")
        var dataGrid = $("#" + GridId).dxDataGrid("instance");
    else
        var dataGrid = $("#" + GridId).dxTreeList("instance");


    var selectedKeys = dataGrid.getSelectedRowKeys();
    var selectedData = dataGrid.getSelectedRowsData();


    var grid_settings = GridSettings.filter(function (elem, i) {
        return (elem.ui_type == "dropdown_id" | elem.ui_type == "dropdown_list_id" | elem.ui_type == "dropdown_txt_id" | elem.ui_type == "dropdown_txt_list_id");
    });


    CompleteDropDown(AddUpdateFlag, CardId, grid_settings, null, 'нет', selectedData);
    JSON.parse(dropdownParams, function (key, value) {
        CompleteDropDown(AddUpdateFlag, CardId, grid_settings, value,  key, selectedData);
    });

    if (AddUpdateFlag) {
        $('#' + CardId + ' .text_b').each(function () { $(this).val(selectedData[0][$(this).attr("name")]); CompleteDropDown(AddUpdateFlag, CardId, grid_settings, selectedData[0][$(this).attr("name")], "grid." +$(this).attr("name"), selectedData);  });
        $('#' + CardId + ' .checkbox').each(function () { $(this).prop('checked', selectedData[0][$(this).attr("name")]);  });
        $('#' + CardId + ' .date').each(function () {
            var disable = $(this).hasClass('non_editable');//attr('disabled');
            var date = selectedData[0][$(this).attr("name")];
            if (date == "" | date == "undefined" | date == null)
                date = null;
            if (date == null) {
                $(this).dxDateBox({
                    disabled: disable
                });
            }
            else {
                $(this).dxDateBox({
                    disabled: disable,
                    value: new Date(date)
                });
            }
        });
        $('#' + CardId + ' .div_val').each(function () { $(this).html(selectedData[0][$(this).attr("name")]); CompleteDropDown(AddUpdateFlag, CardId, grid_settings, selectedData[0][$(this).attr("name")], "grid."+$(this).attr("name"), selectedData); });
        $('#' + CardId + ' .dropdown_list_description').each(function () { $(this).html(selectedData[0][$(this).attr("name")]); });
        $('#' + CardId + ' .dropdown_description').each(function () { $(this).html(selectedData[0][$(this).attr("name")]); });
        $('#' + CardId + ' .dropdown_list_description').each(function () { $(this).html(selectedData[0][$(this).attr("name")]); });
        $('#' + CardId + ' .dropdown_txt_list_description').each(function () { $(this).html(selectedData[0][$(this).attr("name")]); });
        $('#' + CardId + ' .non_editable').show();
        $('#' + CardId + ' .null_type').show();
        $('#' + CardId + ' .dropdown_list_description').show();
        $('#' + CardId + ' .dropdown_description').show();
        $('#' + CardId + ' .dropdown_txt_list_description').show();
        $('#' + CardId + ' .dropdown_txt_description').show();
    }
    else {
        $('#' + CardId + ' .date').each(function () {
            $(this).dxDateBox({
                //min: new Date(2000, 0, 1),
                //max: new Date(2029, 11, 31),
                /* value:''*//* new Date('')*/
            });
            //$(this).prop('checked', selectedData[0][$(this).attr("name")]);
        });

        $('#' + CardId + ' .text_b').each(function () { $(this).val(''); });
        $('#' + CardId + ' .checkbox').each(function () { $(this).prop('checked', false); });

        $('#' + CardId + ' .non_editable').hide();
        $('#' + CardId + ' .null_type').hide();
        $('#' + CardId + ' .dropdown_list_description').hide();
        $('#' + CardId + ' .dropdown_description').hide();
        $('#' + CardId + ' .dropdown_txt_list_description').hide();
        $('#' + CardId + ' .dropdown_txt_description').hide();
        $('#' + CardId + ' .global_non_visible').hide();
        $('#' + CardId + ' .dropdown_list_id').find($('option')).attr('selected', false);
        $('#' + CardId + ' .dropdown_id').find($('option')).attr('selected', false);
    }
    setTimeout(function () { $('#' + CardId).modal("show"); }, 500);
}

function CompleteDropDown(AddUpdateFlag, CardId, grid_settings, param, param_name, selectedData) {
    $.ajax({
        async: true,
        cache: false,
        traditional: true,
        url: '/Common/DropDownCard',
        type: 'POST',
        data: {
            grid_settings: JSON.stringify(grid_settings),
            param: param,
            param_name: param_name
        },
        success: function (data) {
            
            var json_data = JSON.parse(data);
            for (var a = 0; a < json_data.length; a++) {
                $('#card' + json_data[a].Key).html(json_data[a].Value);
                try {  $('#card' + json_data[a].Keyn).selectator('destroy'); } catch (e) { }
                if (AddUpdateFlag) {

                    try {
                        if (selectedData[0][json_data[a].Key] != "" && selectedData[0][json_data[a].Key] != null && selectedData[0][json_data[a].Key] != "undefined") {
                            if (grid_settings[a].ui_type == "dropdown_id" || grid_settings[a].ui_type == "dropdown_txt_id") { 
                                $('#card' + json_data[a].Key).val(selectedData[0][json_data[a].Key]);

                                CompleteDropDown(AddUpdateFlag, CardId, grid_settings, selectedData[0][json_data[a].Key].toString(), "grid." + json_data[a].Key, selectedData);
                            }
                            else {
                                try {
                                    var arr = [];
                                    arr = selectedData[0][json_data[a].Key].split(',');
                                    $('#card' + json_data[a].Key).val(arr)
                                }
                                catch (exc) {}
                            }
                        }
                    }
                    catch (ex) { }

                    $('#' + CardId + ' .dropdown_id.non_editable').each(function () {
                        $(this).closest('tr').hide();
                        $('#' + CardId + ' .dropdown_description[drop="' + $(this).attr('drop') + '"]').closest('tr').show();
                    });
                    $('#' + CardId + ' .dropdown_id.editable').each(function () {
                        $(this).closest('tr').show();
                        $('#' + CardId + ' .dropdown_description[drop="' + $(this).attr('drop') + '"]').closest('tr').hide();
                    });
                    $('#' + CardId + ' .dropdown_list_id.non_editable').each(function () {
                        $(this).closest('tr').hide();
                        $('#' + CardId + ' .dropdown_list_description[drop="' + $(this).attr('drop') + '"]').closest('tr').show();
                    });
                    $('#' + CardId + ' .dropdown_list_id.editable').each(function () {
                        $(this).closest('tr').show();
                        $('#' + CardId + ' .dropdown_list_description[drop="' + $(this).attr('drop') + '"]').closest('tr').hide();
                    });
                    $('#' + CardId + ' .dropdown_txt_id.non_editable').each(function () {
                        $(this).closest('tr').hide();
                        $('#' + CardId + ' .dropdown_txt_description[drop="' + $(this).attr('drop') + '"]').closest('tr').show();
                    });
                    $('#' + CardId + ' .dropdown_txt_id.editable').each(function () {
                        $(this).closest('tr').show();
                        $('#' + CardId + ' .dropdown_txt_description[drop="' + $(this).attr('drop') + '"]').closest('tr').hide();
                    });

                    $('#' + CardId + ' .dropdown_txt_list_id.non_editable').each(function () {
                        $(this).closest('tr').hide();
                        $('#' + CardId + ' .dropdown_txt_list_description[drop="' + $(this).attr('drop') + '"]').closest('tr').show();
                    });
                    $('#' + CardId + ' .dropdown_txt_list_id.editable').each(function () {
                        $(this).closest('tr').show();
                        $('#' + CardId + ' .dropdown_txt_list_description[drop="' + $(this).attr('drop') + '"]').closest('tr').hide();
                    });
                    $('#' + CardId + ' .non_editable').each(function () { $(this).attr('disabled', 'disabled'); });
                    $('#' + CardId + ' .global_non_visible').hide();


                }
                else {
                    try { $('#card' + json_data[a].Key).selectator('destroy'); } catch (e) { }
                    $('#' + CardId + ' .non_editable').each(function () { $(this).removeAttr('disabled'); });
                }


                $('#card' + json_data[a].Key).selectator({
                    showAllOptionsOnFocus: true,
                    searchFields: 'value text subtitle right'
                });
            }           
        }
    });
}

function OnChangeDD(dropdown_name, flowWindowName) {
    var GridSettings = JSON.parse($("#" + flowWindowName + " .grid_field").val());
    var grid_settings = GridSettings.filter(function (elem, i) {
        return ((elem.ui_type == "dropdown_id" | elem.ui_type == "dropdown_list_id" | elem.ui_type == "dropdown_txt_id" | elem.ui_type == "dropdown_txt_list_id") & (elem.dropdown_param == "grid." + dropdown_name));
    });

    $.ajax({
        async: true,
        cache: false,
        traditional: true,
        url: '/Common/DropDownCard',
        type: 'POST',
        data: {
            grid_settings: JSON.stringify(grid_settings),
            param: $("#card" + dropdown_name + " option:selected").val(),
            param_name: "grid." + dropdown_name
        },
        success: function (data) {
            var json_data = JSON.parse(data);
            for (var a = 0; a < json_data.length; a++) {
                $('#card' + json_data[a].Key).html(json_data[a].Value);
                try { $('#card' + json_data[a].Key).selectator('destroy'); } catch (e) { }

                OnChangeDD(json_data[a].Key, flowWindowName);

                $('#card' + json_data[a].Key).selectator({
                    showAllOptionsOnFocus: true,
                    searchFields: 'value text subtitle right'
                });
            }
        }
    });
}
