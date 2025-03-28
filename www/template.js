mySiteMap.style.display = "none";
function ShowHide(x) {
  x.classList.toggle("change");
  var mySiteMap = document.getElementById("mySiteMap");
  if (mySiteMap.style.display === "none") {
    mySiteMap.style.display = "block";
  } else {
    mySiteMap.style.display = "none";
  }
}