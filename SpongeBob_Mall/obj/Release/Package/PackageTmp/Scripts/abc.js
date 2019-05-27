function validate_required(field, alerttxt) {
    with (field) {
        if (value == null || value == "") {
            placeholder = alerttxt;
            return false;
        }   
        else { return true; }
    }
}

function validate_form(thisform) {
    with (thisform) {
        var bl = true;
        bl = validate_required(username, "用户名不能为空") && bl;
        bl = validate_required(password, "密码不能为空") && bl;
        if (!bl) {
            return false;
        }
    }
}
var ranl = 0;
var useRand = 0;
images = new Array;
images[1] = new Image();
images[1].src = "../img/d1.jpg";
images[2] = new Image();
images[2].src = "../img/d2.png";
images[3] = new Image();
images[3].src = "../img/d3.png";
function swapPic() {
    var imgnum = images.length - 1;
    do {
        var randnum = Math.random();
        randl = Math.round((imgnum - 1) * randnum) + 1;
    } while (randl == useRand);
    useRand = randl;
    document.randimg.src = images[useRand].src
    setTimeout('swapPic()', 2000);
}

function subm(){

Document.charset="gb2312";

}

function ShowShop(shop,id) {
    shop.style.display = 'none';
    document.getElementById(id).style.display = 'block';
}

function HideShop(shop,id) {
    shop.style.display = 'none';
    document.getElementById(id).style.display = 'block';
}

function firm(price) {
    with (price) {
        if (confirm("你确定以￥" + goodsPrice.value + "的价格上架吗?")) {
            PutawayJudge(goodsId.value, goodsPrice.value);
            return true;
        } else {
            return false;
        }
    }
}

function PayJudge(goodsid, price) {
    if (confirm("你确定以￥" + price + "的价格购买吗?")) {
        var jsonObj = { 'type': "goodsid", 'text': goodsid };
        myAjax('/Shop/Pay', jsonObj, null, function (re) {
            $.each(re, function (i, eobj) {
                alert(eobj.message);
            })
        });
    } else {
        return false;
    }
}

function PutawayJudge(goodsid, goodsprice) {
    var jsonObj = { 'type': "putaway", 'text': goodsid + ',' + goodsprice };
    myAjax('/Bag/Putaway', jsonObj, null, function (re) {
        $.each(re, function (i, eobj) {
            alert(eobj.message);
        })
    });
}

function DownShop(goodsid) {
    var jsonObj = { 'type': "goodsid", 'text': goodsid };
    myAjax('/Bag/DownShop', jsonObj, null, function (re) {
        $.each(re, function (i, eobj) {
            alert(eobj.message);
        })
    });
}

function myAjax(action, dt, type, callback) {
    $.ajax({
        url: action,
        type: type || "POST",
        data: dt,
        cache: false,
        success: function (result) {
            callback && callback(result);
        },
        error: function (result) {
            alert('System Error, Please Contact Admin!');
        }
    });
}

function viewmypic(mypic, upfile) {
    if (upfile.files && upfile.files[0]) {
        mypic.style.display = "";
        //火狐下，直接设img属性
        //mypic.src = upfile.files[0].getAsDataURL();

        //火狐7以上版本不能用上面的getAsDataURL()方式获取，需要一下方式  
        mypic.src = window.URL.createObjectURL(upfile.files[0]);
    }
    else {
        //IE下
        if (upfile.value) {
            mypic.src = upfile.value;
            mypic.style.display = "";
            mypic.border = 1;
        }
    }
}
