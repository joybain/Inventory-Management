
function jsAlert(message="Provide a text please!",color="#454343",status=1,align="left") {
    var node = document.createElement("samp");
    var imgs;
    if (status == 1) {
        imgs = "jsAlert/success.png";
    }
    else if(status == 2){
        imgs = "jsAlert/worning.png";
    }
    else if(status == 0){
        imgs = "jsAlert/wrong.png";
    }
    else if(status == 3){
        imgs = "jsAlert/information.png";
    }
  node.innerHTML = '<div class="alrtBody"><div class="alrtContent" style="color:'+color+';text-align:'+align+';"><img src="'+imgs+'">'+message+'</div><div class="alrtFoot"><button type="button" onclick="closeJsAlert()">OK</button></div></div>';
  document.getElementsByTagName("body")[0].prepend(node);
}

// close jsAlert
    function closeJsAlert()
    {
        document.getElementsByTagName("samp")[0].remove();
    }