var ranl = 0;
    var useRand = 0;
    images = new Array;
	images[1] = new Image();
    images[1].src ="../img/d1.jpg";
	images[2] = new Image();
	images[2].src = "../img/d2.png";
	images[3] = new Image();
	images[3].src = "../img/d3.png";
	function swapPic(){
	   var imgnum = images.length-1;
       do{
	    var randnum = Math.random();
		randl = Math.round((imgnum-1)*randnum)+1;
	   }while(randl==useRand);
        useRand = randl;
        document.randimg.src = images[useRand].src		
		setTimeout('swapPic()',2000);
}

function validate_required(field, _alerttxt) {
    with (field) {
        if (value == null || value == "") { alert(alerttxt); return false }
        else { return true }
    }
}

function validate_form(thisform) {
    with (thisform) {
        if (validate_required(username, "Username must be filled out!") == false) { username.focus(); return false }
    }
}