function InsertCard(flowWindowName, gridId, bindning, close_window, storedProcedure, id_func, additionalParams) {
    additionalParams = JSON.parse(additionalParams);
    var parameters = [];
    var err = '';
    var dataGrid = $("#" + gridId).dxDataGrid("instance");
    bindning = bindning.toLowerCase();
    close_window = close_window.toLowerCase();

    var str = $('#' + flowWindowName + ' .grid_field').val();
    var GridSettings = JSON.parse(str);
    var grid_settings = GridSettings.filter(function (elem, i) {
        return (elem.ui_type == "dropdown_id" | elem.ui_type == "dropdown_list_id");
    });
    $('#' + flowWindowName + ' table tr:visible .td_value.editable').each(function () {
        if ($(this).attr('requd') == 'required') {
            if ($(this).attr('element_type') != 'checkbox' && $(this).attr('element_type') != 'date')
                if ($(this).val() == '') { alert('Заполните поля, помеченные звездочкой, они обязательны для заполнения.'); type_flag = true; return false; }
            if ($(this).attr('element_type') == 'date') {
                var dat = $(this).dxDateBox('instance').option('value');
                if (dat == "" || dat == null || dat == "undefined"); { alert('Заполните поля, помеченные звездочкой, они обязательны для заполнения.'); type_flag = true; return false; }

            }
        }
    });
    //$('#' + flowWindowName + ' table tr:visible .td_value').children().each(function () {
    //    if ($(this).attr('requd') == 'required') {
    //        if ($(this).attr('element_type') != 'checkbox')
    //            if ($(this).val() == '') { alert('Заполните поля, помеченные звездочкой, они обязательны для заполнения.'); err = 'some_err'; return false; }
    //    }
    //});
    if (type_flag)
        return;
    var type_flag = false;
    $('#' + flowWindowName + ' table tr:visible .td_value.textbox_int.editable').each(function () {
        if (!isInt($(this).val())) { alert("Поле " + $(this).attr('rus_name').toLowerCase() + ' должно быть целым числом'); type_flag = true; return false; }
    });
    if (type_flag)
        return;
    $('#' + flowWindowName + ' table tr:visible .td_value.textbox_float.editable').each(function () {
        if (!isFloat($(this).val())) { alert("Поле " + $(this).attr('rus_name').toLowerCase() + ' должно быть числом с плавающей точкой'); type_flag = true; return false; }
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

    parameters.push({ Name: 'id', Value: object_id[id_func]() });


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
        for (var i = 0; i < additionalParams.length; i++) {
            parameters = parameters.filter(function (elem, j) {
                return (elem.Name != additionalParams[i].Name)
            });
            parameters.push(additionalParams[i]);
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
                    var dataGrid = $("#" + gridId).dxDataGrid("instance")
                    rebind_obj.rebind = true;
                    dataGrid.refresh();
                    dataGrid.deselectAll();
                }
                if (close_window) {
                    $('#' + flowWindowName).modal("hide");
                }
            }
        }
    });
}

function OpenCard(AddUpdateFlag, CardId, GridId, ParamStr) {
    //var data = selectedItems.selectedRowsData;
    //records = [];
    //$.each(data, function (i, val) {
    //    records.push(val.id);

    //});
    //$('#' + CardId + ' tr').show();
    var str = $('#' + CardId + ' .grid_field').val();
    var GridSettings = JSON.parse(str);
    var Param = null;
    if (ParamStr != null)
        Param = ParamObj[ParamStr]();


    // selectedData[0][grid_settings[a].field_description]
    var dataGrid = $("#" + GridId).dxDataGrid("instance")
    var selectedKeys = dataGrid.getSelectedRowKeys();
    var selectedData = dataGrid.getSelectedRowsData();
    if (AddUpdateFlag) {


        $('#' + CardId + ' .text_b').each(function () {
            $(this).val(selectedData[0][$(this).attr("name")]);
        });
        $('#' + CardId + ' .checkbox').each(function () {
            $(this).prop('checked', selectedData[0][$(this).attr("name")]);
        });
        $('#' + CardId + ' .date').each(function () {
            var disable = $(this).hasClass('non_editable');//attr('disabled');
            var date = selectedData[0][$(this).attr("name")];
            if (date == "" | date == "undefined" | date == null)
                date = null;


            //if (disable == "disabled")
            //    disable = true
            //else
            //    disable = false;

            if (date == null) {
                $(this).dxDateBox({
                    //min: new Date(2000, 0, 1),
                    //max: new Date(2029, 11, 31),
                    disabled: disable
                });
            }
            else {
                $(this).dxDateBox({
                    //min: new Date(2000, 0, 1),
                    //max: new Date(2029, 11, 31),
                    disabled: disable,
                    value: new Date(date)
                });
            }
            //$(this).prop('checked', selectedData[0][$(this).attr("name")]);
        });
        $('#' + CardId + ' .div_val').each(function () {
            $(this).html(selectedData[0][$(this).attr("name")]);
        });

        $('#' + CardId + ' .dropdown_list_description').each(function () {
            $(this).html(selectedData[0][$(this).attr("name")]);
        });

        $('#' + CardId + ' .dropdown_description').each(function () {
            $(this).html(selectedData[0][$(this).attr("name")]);
        });

        $('#' + CardId + ' .dropdown_list_description').each(function () {
            $(this).html(selectedData[0][$(this).attr("name")]);
        });

        $('#' + CardId + ' .dropdown_txt_list_description').each(function () {
            $(this).html(selectedData[0][$(this).attr("name")]);
        });
        //$('#' + CardId + ' .non_editable.date').each(function () {
        //    $(this).html(selectedData[0][$(this).attr("name")]);
        //}); 
        $('#' + CardId + ' .non_editable').show();
        $('#' + CardId + ' .null_type').show();
        $('#' + CardId + ' .dropdown_list_description').show();
        $('#' + CardId + ' .dropdown_description').show();
        $('#' + CardId + ' .dropdown_txt_list_description').show();
        $('#' + CardId + ' .dropdown_txt_description').show();
        //$('#' + CardId + ' .dropdown_txt_list_description').show();
        //$('#' + CardId + ' .dropdown_txt_description').show();
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

        $('#' + CardId + ' .text_b').each(function () {
            $(this).val('');
        });

        $('#' + CardId + ' .checkbox').each(function () {
            $(this).prop('checked', false);
        });

    }
    var grid_settings = GridSettings.filter(function (elem, i) {
        return (elem.ui_type == "dropdown_id" | elem.ui_type == "dropdown_list_id" | elem.ui_type == "dropdown_txt_id" | elem.ui_type == "dropdown_txt_list_id");
    });
    //var dropdowns = [];
    //for (var i = 0; i < grid_settings.length; i++) {
    //    dropdowns.push(grid_settings[i].dropdown);
    //}
    //dropdowns: dropdowns,

    $.ajax({
        async: true,
        cache: false,
        traditional: true,
        url: '/Common/DropDownCard',
        type: 'POST',
        data: {
            grid_settings: JSON.stringify(grid_settings),
            param: Param
        },
        success: function (data) {
            var json_data = JSON.parse(data);
            for (var a = 0; a < json_data.length; a++) {
                $('#card' + json_data[a].Key).html(json_data[a].Value);
                try { $('#card' + json_data[a].Keyn).selectator('destroy'); } catch (e) { }
                if (AddUpdateFlag) {
                    try {
                        if (selectedData[0][json_data[a].Key] != "" && selectedData[0][json_data[a].Key] != null && selectedData[0][json_data[a].Key] != "undefined") {
                            if (grid_settings[a].ui_type == "dropdown_id" || grid_settings[a].ui_type == "dropdown_txt_id")
                                $('#card' + json_data[a].Key).val(selectedData[0][json_data[a].Key]);
                            else {
                                try {
                                    var arr = [];
                                    arr = selectedData[0][json_data[a].Key].split(',');
                                    $('#card' + json_data[a].Key).val(arr)
                                }
                                catch (exc) { ; }
                            }
                        }
                        //alert(selectedData[0][grid_settings[a].field_description]);
                        //alert(grid_settings[a].field_description);
                    }
                    catch (ex) { ; }
                    //$('#' + CardId + ' .dropdown_list_description').show();
                    //$('#' + CardId + ' .dropdown_description').show();

                    //$('#' + CardId + ' .non_editable').hide();
                    //$('#' + CardId + ' .null_type').show();

                    //$('#' + CardId + ' .dropdown_description').hide();
                    //$('#' + CardId + ' .dropdown_list_description').();
                    //if (selectedData[0][grid_settings[a].field_description] != "" && selectedData[0][grid_settings[a].field_description] != null && selectedData[0][grid_settings[a].field_description] != "undefined")
                    //    $('#card' + grid_settings[a].field_description).val(selectedData[0][grid_settings[a].field_description]);

                }
                else {
                    try { $('#card' + json_data[a].Key).selectator('destroy'); } catch (e) { }
                }

                // try { $('#card' + grid_settings[a].field_description).selectator('refresh'); } catch (ex) { ;}
                //$('#card' + grid_settings[a].field_description).selectator('refresh');

                $('#card' + json_data[a].Key).selectator({
                    showAllOptionsOnFocus: true,
                    searchFields: 'value text subtitle right'
                });
                //window.location = "/Common/ReturnFile?physicalPath=" + data;
                //$('#card' + grid_settings[i].field_description).html(data);
                //if (AddUpdateFlag) {
                //    try {
                //        $('#card' + grid_settings[i].field_description).val(selectedData[grid_settings[i].field_description]);
                //    }
                //    catch (ex) { ; }
                //}
                //$('#card' + grid_settings[i].field_description).selectator({
                //    showAllOptionsOnFocus: true,
                //    searchFields: 'value text subtitle right'
                //});
            }

            if (AddUpdateFlag) {
                $('#' + CardId + ' .dropdown_id.non_editable').each(function () {
                    //$('#' + CardId + ' .dropdown_id[drop="' + $(this).attr('drop') + '"]').closest('tr').hide();
                    $(this).closest('tr').hide();
                    $('#' + CardId + ' .dropdown_description[drop="' + $(this).attr('drop') + '"]').closest('tr').show();
                });
                $('#' + CardId + ' .dropdown_id.editable').each(function () {
                    //$('#' + CardId + ' .dropdown_id[drop="' + $(this).attr('drop') + '"]').closest('tr').hide();
                    $(this).closest('tr').show();
                    $('#' + CardId + ' .dropdown_description[drop="' + $(this).attr('drop') + '"]').closest('tr').hide();
                });
                $('#' + CardId + ' .dropdown_list_id.non_editable').each(function () {
                    //$('#' + CardId + ' .dropdown_list_id[drop="' + $(this).attr('drop') + '"]').closest('tr').hide();
                    $(this).closest('tr').hide();
                    $('#' + CardId + ' .dropdown_list_description[drop="' + $(this).attr('drop') + '"]').closest('tr').show();
                });
                $('#' + CardId + ' .dropdown_list_id.editable').each(function () {
                    //$('#' + CardId + ' .dropdown_id[drop="' + $(this).attr('drop') + '"]').closest('tr').hide();
                    $(this).closest('tr').show();
                    $('#' + CardId + ' .dropdown_list_description[drop="' + $(this).attr('drop') + '"]').closest('tr').hide();
                });


                $('#' + CardId + ' .dropdown_txt_id.non_editable').each(function () {
                    //$('#' + CardId + ' .dropdown_id[drop="' + $(this).attr('drop') + '"]').closest('tr').hide();
                    $(this).closest('tr').hide();
                    $('#' + CardId + ' .dropdown_txt_description[drop="' + $(this).attr('drop') + '"]').closest('tr').show();
                });
                $('#' + CardId + ' .dropdown_txt_id.editable').each(function () {
                    //$('#' + CardId + ' .dropdown_id[drop="' + $(this).attr('drop') + '"]').closest('tr').hide();
                    $(this).closest('tr').show();
                    $('#' + CardId + ' .dropdown_txt_description[drop="' + $(this).attr('drop') + '"]').closest('tr').hide();
                });

                $('#' + CardId + ' .dropdown_txt_list_id.non_editable').each(function () {
                    //$('#' + CardId + ' .dropdown_id[drop="' + $(this).attr('drop') + '"]').closest('tr').hide();
                    $(this).closest('tr').hide();
                    $('#' + CardId + ' .dropdown_txt_list_description[drop="' + $(this).attr('drop') + '"]').closest('tr').show();
                });
                $('#' + CardId + ' .dropdown_txt_list_id.editable').each(function () {
                    //$('#' + CardId + ' .dropdown_id[drop="' + $(this).attr('drop') + '"]').closest('tr').hide();
                    $(this).closest('tr').show();
                    $('#' + CardId + ' .dropdown_txt_list_description[drop="' + $(this).attr('drop') + '"]').closest('tr').hide();
                });

                //    ($('#card'+$(this).attr("name")+'_id')).closest('tr').hide();
                //});
                //$('#' + CardId + ' .non_editable').show();
                $('#' + CardId + ' .non_editable').each(function () { $(this).attr('disabled', 'disabled'); });
                $('#' + CardId + ' .global_non_visible').hide();
            }
            else {
                $('#' + CardId + ' .non_editable').each(function () { $(this).removeAttr('disabled'); });
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
        }
    });

    $('#' + CardId).modal("show");
}