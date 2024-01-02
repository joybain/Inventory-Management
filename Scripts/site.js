// JScript File
function startTime()
 {
    var today=new Date();
    var h=today.getHours();
    var m=today.getMinutes();
    var s=today.getSeconds();
    var Days= ["Sun","Mon","Tue","Wed","Thu","Fri","Sat","Sun"]
    var Months= ["Jan","Feb","Mar","Apr","May","June","July","Aug","Sep","Oct","Nov","Dec"]
    var mm = today.getMonth();
    var dw = today.getDay();
    var dd = today.getDate();
    var yyyy = today.getFullYear();
    // add a zero in front of numbers<10
    m=padzero(m);
  //  dd=padzero(dd);
    dd=padzero(dd);
    s = padzero(s);
   
   if(h >= 12)
   {
        s += " PM"; 
   }
   else
   {
    s += " AM";
   }
   if(h>12)
   {
    h=h-12;
   }
   if(h==0)
   {
     h="12";
   }
  
    
    document.getElementById('displaytime').innerHTML = Days[dw] + " - " + Months[mm] + " " + dd + ", " + yyyy + " " + h+":"+m+":"+s;
    t=setTimeout("startTime()",1000);
 }
 
function padzero(i)
{
if (i<10) 
  {i="0" + i}
  return i
}

//Popup Code with Jquery.
function closeParentPopup() {
    parent.$('#Popupdialog').dialog('close');
}

function closePopup() {
    $('#Popupdialog').dialog('close');
}

$(function() {
    // Dialog
    $('#Popupdialog').dialog({
        autoOpen: false,
        modal: true,
        bgiframe: true,
        buttons: {Cancel: function() { closeParentPopup() } }
    });
});

function wait(msecs) {
    var start = new Date().getTime();
    var cur = start
    while (cur - start < msecs) {
        cur = new Date().getTime();
    }
}


function setmainframe(src, w, h, headertxt) {
    document.getElementById('ctl00_PopupUC_dialogframe').src = src;
    document.getElementById('ctl00_PopupUC_dialogframe').width = w;
    document.getElementById('ctl00_PopupUC_dialogframe').height = h;
    wait(200);
    $('#Popupdialog').dialog('option', 'width', w + 25);
    $('#Popupdialog').dialog('option', 'height', h + 78);
    $('#Popupdialog').dialog('option', 'title', headertxt);
    $('#Popupdialog').dialog('open');
}