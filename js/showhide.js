// show and hide a div

function showHide(obj,img,img1,img2) 
{
    if (obj.style.display == "block") {
        obj.style.display = "none";
        img.innerHTML = "<img src='"  + img1 + " '>";
    } else {
        obj.style.display = "block";
        img.innerHTML = "<img src='"  + img2 + " '>";
    }
}
