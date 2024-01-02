function formatdate(elementId) {
    var fdt;
    var dt = document.getElementById(elementId).value;
    var day;
    var mon;
    var yr;
    if (dt.length > 0) {
        dt = dt.toUpperCase();
        dt = dt.replace('/', '').replace('/', '').replace('.', '').replace('.', '').replace('-', '').replace('-', '');
        dt = dt.replace('JAN', '01').replace('FEB', '02').replace('MAR', '03').replace('APR', '04').replace('MAY', '05').replace('JUN', '06').replace('JUL', '07').replace('AUG', '08').replace('SEP', '09').replace('OCT', '10').replace('NOV', '11').replace('DEC', '12');
        day = dt.substr(0, 2);
        mon = dt.substr(2, 2);
        yr = dt.substr(4);
        if (yr.length == 2) {
            if (yr > 30) {
                yr = '19' + yr;
            }
            else {
                yr = '20' + yr;
            }
        }
        if ((dt.length < 6) || (dt.length > 10)) {
            alert("Invalid Date\nPlease Re-Enter");
            document.getElementById(elementId).value = "";
            document.getElementById(elementId).focus();
        }
        else if (parseInt(day) >= 32) {
            alert("Invalid Date\nPlease Re-Enter");
            document.getElementById(elementId).value = "";
            document.getElementById(elementId).focus();
        }

        else if (parseInt(mon) > 12) {
            alert("Invalid Date\nPlease Re-Enter");
            document.getElementById(elementId).value = "";
            document.getElementById(elementId).focus();
        }
        else {
            fdt = day + '/' + mon + '/' + yr;
            document.getElementById(elementId).value = fdt;
        }
    }
}
function formatTime(elementId) {
    var dt = document.getElementById(elementId).value.replace(':', '').replace(';', '').replace('.', '').replace(',', '').replace('-', '');
    if (dt.length == 1) {
        document.getElementById(elementId).value = '0'+ dt + ':00';
    }
    else if (dt.length == 2) {
        document.getElementById(elementId).value = dt + ':00';
    }
    else if (dt.length == 3) {
        var hr = pad(dt.substr(0, 1), 2);
        var min = dt.substr(1, 2);
        document.getElementById(elementId).value = hr + ':' + min;
    }
    else if (dt.length == 4) {
        var hr = dt.substr(0, 2);
        if (hr > 23) {
            alert('Invaild time.');
            return;
        }
        var min = dt.substr(2, 2);
        document.getElementById(elementId).value = hr + ':' + min;
    }
}
function formatDateTime(elementId) {
    var datetime = document.getElementById(elementId).value.split(" ");
    var dt = datetime[0].toUpperCase();
    var dtime = datetime[1].toUpperCase();

    var day;
    var mon;
    var yr;
    var fdt;
    if (datetime[0].length > 0) {

        dt = dt.replace('/', '').replace('/', '').replace('.', '').replace('.', '').replace('-', '').replace('-', '');
        dt = dt.replace('JAN', '01').replace('FEB', '02').replace('MAR', '03').replace('APR', '04').replace('MAY', '05').replace('JUN', '06').replace('JUL', '07').replace('AUG', '08').replace('SEP', '09').replace('OCT', '10').replace('NOV', '11').replace('DEC', '12');
        day = dt.substr(0, 2);
        mon = dt.substr(2, 2);
        yr = dt.substr(4);
        if (yr.length == 2) {
            if (yr > 30) {
                yr = '19' + yr;
            }
            else {
                yr = '20' + yr;
            }
        }
        if ((dt.length < 6) || (dt.length > 10)) {
            alert("Invalid Date\nPlease Re-Enter");
            document.getElementById(elementId).value  = "";
            document.getElementById(elementId).focus();
        }
        else if (parseInt(day) >= 32) {
            alert("Invalid Date\nPlease Re-Enter");
            document.getElementById(elementId).value = "";
            document.getElementById(elementId).focus();
        }

        else if (parseInt(mon) > 12) {
            alert("Invalid Date\nPlease Re-Enter");
            document.getElementById(elementId).value = "";
            document.getElementById(elementId).focus();
        }
        else {
            fdt = day + '/' + mon + '/' + yr;
        }
    }

    var hr;
    var min;
    if (datetime[1].length > 0) {
        dtime = dtime.replace(':', '').replace(';', '').replace('.', '').replace(',', '').replace('-', '');
        if (dtime.length == 2) {
            hr = dtime;
            min = '00';
        }
        else if (dtime.length == 3) {
            hr = pad(dtime.substr(0, 1), 2);
            min = dtime.substr(1, 2);
        }
        else if (dtime.length == 4) {
            hr = dtime.substr(0, 2);
            if (hr > 23) {
                alert('Invaild time.');
                return;
            }
            min = dtime.substr(2, 2);
        }
    }
    document.getElementById(elementId).value = fdt + ' ' + hr + ':' + min;
}
function pad(n, width, z) {
    z = z || '0';
    n = n + '';
    return n.length >= width ? n : new Array(width - n.length + 1).join(z) + n;
}            