$(document).ready(function () {
    $("#reportForm").on('submit', function (e) {
        var ids = [];
        var rows = $("#requestTable tr.Row input[type=checkbox]:checked").closest("tr");
        $.each(rows, function (i, o) {
            ids.push($(o).data('requestid'));
        });
        $("#IDs").val(ids.join("V<3:)"));
        return true;
    });

    $("#reportSelectAll").on('change', function () {
        var checkboxes = $("input.selectRequest");
        if ($("#reportSelectAll").is(':checked') == true) {
            //checkboxes.each(function (i, o) {
            //    if($(o).closest('tr').style.display != 'none')
            //        $(o).prop('checked', true);
            //});
            checkboxes.prop('checked', true);
        } else {
            checkboxes.prop('checked', false);
        }

    });
    $('#startDate').change(function () {
        $(this).addClass("orange-border");
    });
    $('#endDate').change(function () {
        $(this).addClass("orange-border");
    });
    $('#reportWarehouse').change(function () {
        $(this).addClass("orange-border");
    });

    updateReport = function(){
        $('#reportWarehouse').attr('disabled', 'disabled');
        $('#endDate').attr('disabled', 'disabled');
        $("#startDate").attr('disabled', 'disabled');

        var _warehouse = $("#reportWarehouse").val();
        var _endDate = new Date($("#endDate").val());
        var _startDate = new Date($("#startDate").val());

        $("#reportRequestTable").find('.Row').hide();
        var _rows = $("#reportRequestTable").find('.Row');
        for(var i = 0; i < _rows.length; i++)
        {
            var _date = new Date($(_rows[i]).data('date'));
            if((_warehouse == "0" || _warehouse == $(_rows[i]).data('warehouse')) &&
                _startDate < _date &&
                _endDate > _date)
            {
                $(_rows[i]).show();
            }
        }
        
        //UI Clean up
        $("#reportWarehouse").removeAttr('disabled');
        $("#endDate").removeAttr('disabled');
        $("#startDate").removeAttr('disabled');
        $('#startDate').removeClass("orange-border");
        $('#endDate').removeClass("orange-border");
        $('#reportWarehouse').removeClass("orange-border");
        var requestTable = new List('requestTable', options);
    }

});
