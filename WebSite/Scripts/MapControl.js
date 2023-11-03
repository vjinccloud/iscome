//資料來源
//國土繪測中心 https://maps.nlsc.gov.tw/S09SOA/
//開放街圖OSM
//TGOS(中研院提供) http://gis.sinica.edu.tw/tgos/

var lat = 24;
var lon = 121;
var ZoomLevel = 10;

//Icon的圖檔路徑
var IconImgPath = '/Scripts/Leaflet/images/';

//設定預設的圖檔路徑
L.Icon.Default.imagePath = IconImgPath;

var map;
//Marker
var Marker_Blue;
var Marker_Red;
var Marker_Green;
var Marker_Yellow;

var Marker_pass;
var Marker_calendar;
var Marker_err;
var Marker_change;

//var Marker_pass_selected;
//var Marker_calendar_selected;
//var Marker_err_selected;
//var Marker_change_selected;

var _UserLocationMarker;

/*
window.onload = function(){
    LoadMap();
};
*/
function LoadMap() {

    map = L.map('map_container').setView([lat, lon], ZoomLevel);

    //===================================BaseLayers===========================================

    //OpenStreetMap
    var OSM = new L.tileLayer('http://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png',
        {
            maxZoom: 19,
            attribution: '&copy; <a href="https://www.openstreetmap.org/">OpenStreetMap</a>貢獻者' +
                '<a href="http://creativecommons.org/licenses/by-sa/2.0/">CC-BY-SA</a>'
        });

    //TGOS-TGOS電子地圖
    var TGOS_TGOSMAP_W = new L.tileLayer('http://gis.sinica.edu.tw/tgos/file-exists.php?img={id}-jpg-{z}-{x}-{y}',
        {
            maxZoom: 19,
            attribution: 'Map data &copy; Sinica',
            id: 'TGOSMAP_W'
        });

    //TGOS-通用版電子地圖
    var TGOS_NLSCMAP_W = new L.tileLayer('http://gis.sinica.edu.tw/tgos/file-exists.php?img={id}-jpg-{z}-{x}-{y}',
        {
            maxZoom: 19,
            attribution: 'Map data &copy; Sinica',
            id: 'NLSCMAP_W'
        });
    //TGOS-路網數值圖
    var TGOS_MOTCMAP_W = new L.tileLayer('http://gis.sinica.edu.tw/tgos/file-exists.php?img={id}-jpg-{z}-{x}-{y}',
        {
            maxZoom: 19,
            attribution: 'Map data &copy; Sinica',
            id: 'MOTCMAP_W'
        });
    //TGOS-福衛二號影像
    var TGOS_F2IMAGE_W = new L.tileLayer('http://gis.sinica.edu.tw/tgos/file-exists.php?img={id}-jpg-{z}-{x}-{y}',
        {
            maxZoom: 19,
            attribution: 'Map data &copy; Sinica',
            id: 'F2IMAGE_W'
        });

    //國土繪測中心-臺灣通用電子地圖
    var NLSC_EMAP = new L.tileLayer('https://wmts.nlsc.gov.tw/wmts/{id}/default/GoogleMapsCompatible/{z}/{y}/{x}',
        {
            maxZoom: 19,
            attribution: '&copy; <a href="https://maps.nlsc.gov.tw/">NLSC</a>',
            id: 'EMAP'
        });

    //國土繪測中心-正射影像圖(通用版)
    var NLSC_PHOTO2 = new L.tileLayer('https://wmts.nlsc.gov.tw/wmts/{id}/default/GoogleMapsCompatible/{z}/{y}/{x}',
        {
            maxZoom: 19,
            attribution: '&copy; <a href="https://maps.nlsc.gov.tw/">NLSC</a>',
            id: 'PHOTO2'
        });
    //國土繪測中心-1/5000基本地形圖
    var NLSC_B5000 = new L.tileLayer('https://wmts.nlsc.gov.tw/wmts/{id}/default/GoogleMapsCompatible/{z}/{y}/{x}',
        {
            maxZoom: 19,
            attribution: '&copy; <a href="https://maps.nlsc.gov.tw/">NLSC</a>',
            id: 'B5000'
        });


    //==================================OverLayers============================================


    //縣市界
    var NLSC_CITY_overlayer = new L.tileLayer('https://wmts.nlsc.gov.tw/wmts/{id}/default/GoogleMapsCompatible/{z}/{x}/{y}',
        {
            maxZoom: 14,
            attribution: '&copy; <a href="https://maps.nlsc.gov.tw/">NLSC</a>',
            id: 'CITY'
        });
    //鄉鎮區界
    var NLSC_TOWN_overlayer = new L.tileLayer('https://wmts.nlsc.gov.tw/wmts/{id}/default/GoogleMapsCompatible/{z}/{y}/{x}',
        {
            maxZoom: 15,
            attribution: '&copy; <a href="https://maps.nlsc.gov.tw/">NLSC</a>',
            id: 'TOWN'
        });
    //村里界
    var NLSC_Village_overlayer = new L.tileLayer('https://wmts.nlsc.gov.tw/wmts/{id}/default/GoogleMapsCompatible/{z}/{y}/{x}',
        {
            maxZoom: 18,
            attribution: '&copy; <a href="https://maps.nlsc.gov.tw/">NLSC</a>',
            id: 'Village'
        });

    //段籍圖
    var NLSC_LANDSECT_overlayer = new L.tileLayer('https://wmts.nlsc.gov.tw/wmts/{id}/default/GoogleMapsCompatible/{z}/{y}/{x}',
        {
            maxZoom: 18,
            attribution: '&copy; <a href="https://maps.nlsc.gov.tw/">NLSC</a>',
            id: 'LANDSECT'
        });

    //臺灣通用電子地圖透明
    var NLSC_EMAP2_overlayer = new L.tileLayer('https://wmts.nlsc.gov.tw/wmts/{id}/default/GoogleMapsCompatible/{z}/{y}/{x}',
        {
            maxZoom: 18,
            attribution: '&copy; <a href="https://maps.nlsc.gov.tw/">NLSC</a>',
            id: 'EMAP2'
        });


    //==============================LayerControl===============================================
    L.control.scale({ 'position': 'bottomleft', 'metric': true, 'imperial': false }).addTo(map);

    //預設的底圖
    NLSC_EMAP.addTo(map);

    var baseLayers = {
        //"OSM": OSM,
        //"TGOS-TGOS電子地圖": TGOS_TGOSMAP_W,
        //"TGOS-通用版電子地圖": TGOS_NLSCMAP_W,
        //"TGOS-路網數值圖": TGOS_MOTCMAP_W,
        //"TGOS-福衛二號影像": TGOS_F2IMAGE_W,
        "NLSC-臺灣通用電子地圖": NLSC_EMAP,
        "NLSC-正射影像圖(通用版)": NLSC_PHOTO2,
        //"NLSC-1/5000基本地形圖": NLSC_B5000
    };

    var overLayers = {
        //"縣市界": NLSC_CITY_overlayer,
        "鄉鎮區界": NLSC_TOWN_overlayer,
        "村里界": NLSC_Village_overlayer,
        "段籍圖": NLSC_LANDSECT_overlayer,
        "臺灣通用電子地圖透明": NLSC_EMAP2_overlayer
    };

    L.control.layers(baseLayers, overLayers).addTo(map);

    //===============================ICON=====================================================

    //streelight = L.icon({
    //    iconUrl: '../../Scripts/leaflet/images/streelight-new-2.png',
    //    iconSize: [20, 20],
    //    iconAnchor: [9, 10],
    //    tooltipAnchor: [13, 0],
    //    popupAnchor: [0, -13]
    //});


    //use css style sprites, available colors are
    //blue(default )
    //green
    //orange
    //yellow
    //red
    //purple
    //violet
    //Marker_Blue = L.spriteIcon('blue');
    //Marker_Red = L.spriteIcon('red');
    //Marker_Green = L.spriteIcon('green');
    //Marker_Yellow = L.spriteIcon('yellow');


    Marker_pass = L.icon({
        iconUrl: IconImgPath + "marker-icon-pass.png",
        iconAnchor: [13, 38],
        tooltipAnchor: [13, -18],
        popupAnchor: [0, -35]
    });

    Marker_calendar = L.icon({
        iconUrl: IconImgPath + "marker-icon-calendar.png",
        iconAnchor: [13, 38],
        tooltipAnchor: [13, -18],
        popupAnchor: [0, -35]
    });

    Marker_err = L.icon({
        iconUrl: IconImgPath + "marker-icon-err.png",
        iconAnchor: [13, 38],
        tooltipAnchor: [13, -18],
        popupAnchor: [0, -35]
    });

    Marker_change = L.icon({
        iconUrl: IconImgPath + "marker-icon-change.png",
        iconAnchor: [13, 38],
        tooltipAnchor: [13, -18],
        popupAnchor: [0, -35]
    });

    //Marker_pass_selected = L.icon({
    //    iconUrl: '../../Scripts/leaflet/images/marker-icon-2x-pass.png',
    //    iconAnchor: [20, 51],
    //    tooltipAnchor: [13, 0],
    //    popupAnchor: [5, -50]
    //});

    //Marker_calendar_selected = L.icon({
    //    iconUrl: '../../Scripts/leaflet/images/marker-icon-2x-calendar.png',
    //    iconAnchor: [20, 51],
    //    tooltipAnchor: [13, 0],
    //    popupAnchor: [5, -50]
    //});

    //Marker_err_selected = L.icon({
    //    iconUrl: '../../Scripts/leaflet/images/marker-icon-2x-err.png',
    //    iconAnchor: [20, 51],
    //    tooltipAnchor: [13, 0],
    //    popupAnchor: [5, -50]
    //});

    //Marker_change_selected = L.icon({
    //    iconUrl: '../../Scripts/leaflet/images/marker-icon-2x-change.png',
    //    iconAnchor: [20, 51],
    //    tooltipAnchor: [13, 0],
    //    popupAnchor: [5, -50]
    //});


    //==============================EVENT=====================================================



}


var Sidebar;
var miniMap
var currentLocation;

var sidebarLock = false; //sidebar鎖定狀態

//載入按鈕
function LoadEasyButton() {
    //定位
    L.easyButton('<i class="fas fa-crosshairs" title="取得我的定位"></i>', function () {
        map.locate();
    }).addTo(map);

    //開啟設定
    L.easyButton('<i class="fas fa-info-circle" title="顯示業者資料卡"></i>', function (e) {
        //避免短時間重複觸發，每0.1秒只能執行一次
        if (!sidebarLock) {
            sidebarLock = true;
            sidebar.toggle();
            setTimeout(function () { sidebarLock = false; }, 100);
        }
    }).addTo(map);
}

//載入側邊攔
function LoadSideBar() {
    sidebar = L.control.sidebar('sidebar').addTo(map);;
}

//定位成功事件
function onLocationFound(e) {

    if (_UserLocationMarker) {
        map.removeLayer(_UserLocationMarker)
    }

    _UserLocationMarker = L.userMarker(e.latlng, { pulsing: true, accuracy: 100, smallIcon: true });
    _UserLocationMarker.addTo(map);

    map.setView(e.latlng, 17)
}

//定位失敗事件
function onLocationError(e) {
    alert("定位失敗! 請確認您的瀏覽器有提供定位功能，並允許取得位置。");
}


