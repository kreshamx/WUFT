function showLoading() {
    $(".loadingOverlay").fadeIn();
}
function hideLoading() {
    $(".loadingOverlay").fadeOut();
}

function toggleHighlight(o)
{
    o.closest('tr').classList.toggle('highlightRow');
}

function exportSuccess()
{
    alert('done with export!');
}

function getInternetExplorerVersion() {
    var rv = -1; // Return value assumes failure.
    if (navigator.appName == 'Microsoft Internet Explorer') {
        var ua = navigator.userAgent;
        var re = new RegExp("MSIE ([0-9]{1,}[\.0-9]{0,})");
        if (re.exec(ua) != null)
            rv = parseFloat(RegExp.$1);
    }
    return rv;
}
function checkVersion() {
    var msg = "You're not using Internet Explorer.";
    var ver = getInternetExplorerVersion();

    if (ver > -1) {
        if (ver >= 8.0)
            msg = "You're using a recent copy of Internet Explorer."
        else
            msg = "You should upgrade your copy of Internet Explorer.";
    }
    alert(msg);
}


var options = {
    valueNames: ['MRBID', 'Disposition', 'LotID', 'UnmergeLotID', 'BoxID', 'UnitQty', 'Warehouse', 'Requestor', 'RequestDate', 'RequestStatus']
};

var requestTable = new List('requestTable', options);