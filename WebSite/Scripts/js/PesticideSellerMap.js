//座標轉換
proj4.defs([
    [
        'EPSG:4326',
        '+title=WGS 84 (long/lat) +proj=longlat +ellps=WGS84 +datum=WGS84 +units=degrees'],
    [
        'EPSG:3826',
        '+title=TWD97 TM2+proj=tmerc +lat_0=0 +lon_0=121 +k=0.9999 +x_0=250000 +y_0=0 +ellps=GRS80 +units=m +no_defs']
]);

var EPSG3826 = new proj4.Proj('EPSG:3826');//TWD97 121 2度分帶 (Tgos)
var EPSG4326 = new proj4.Proj('EPSG:4326');//WGS84

//---------------------------------------

//設定MarkerCluster參數
var MarkerList = L.markerClusterGroup({ disableClusteringAtZoom: 16 });
MarkerList.maxClusterRadius = 1000;


var SelectedMarker = null;
var i = 1;

function GetIcon(AuditResult) {
    switch (AuditResult) {
        case "不合格":
        case "複查不合格":
            return (Marker_err);

        case "合格":
        case "複查合格":
            return (Marker_pass);

        case "限期改善":
        case "輔導改善":
            return (Marker_calendar);


        case "移縣市辦理":
        case "其他":
            return (Marker_change);
    }
}


//顯示業者資料
function ShowPoint(PointList) {

    MarkerList.clearLayers();
    $(PointList).each(
        function (idx, item) {

            var InfoCardID = 'InfoCard_' + item.IndustryInfo.IndustryID;
            var MarkerID = 'Marker_' + item.IndustryInfo.IndustryID;

            //先把座標轉為WGS84
            var X = parseFloat(item.IndustryInfo.TGOSX);
            var Y = parseFloat(item.IndustryInfo.TGOSY);

            //LngLat格式
            var LngLat = proj4(EPSG3826, EPSG4326, [X, Y]);
            //轉成LatLng格式
            var LatLng = [LngLat[1], LngLat[0]];

            //取得稽核結果對應的Icon
            var icon = GetIcon(item.PmdsRecord.AuditResult);

            //建立標記
            var tmpMarker = L.marker(LatLng, { id: MarkerID, AuditResult: item.PmdsRecord.AuditResult, icon: icon });

            //設定zindex
            var zindex = 1000;
            var pos = map.latLngToLayerPoint(LatLng).round();
            tmpMarker.setZIndexOffset(zindex - pos.y);

            //綁定tooltip
            var Tooltip = $('<div>').loadTemplate($('#MarkerTooltip'), item);
            tmpMarker.bindTooltip(Tooltip.html()).openTooltip();

            //綁定Popup
            var Popup = $('<div>').loadTemplate($('#MarkerPopup'), item);
            tmpMarker.bindPopup(Popup.html());

            //綁定事件
            tmpMarker.on('mouseover', function () { GoToInfoCard(InfoCardID); HighlightCard(InfoCardID, true); });

            tmpMarker.on('mouseout', function () { HighlightCard(InfoCardID, false); });

            tmpMarker.on('click', function (e) { ChangeSelectedMarker(e.target) });

            //加到叢集標記中
            MarkerList.addLayer(tmpMarker);
        });

    map.addLayer(MarkerList);
    MarkerList.refreshClusters();

}


//---------------------------------------
//透過MarkerID尋找對應Marker
//回傳Marker物件
function FindMarker(MarkerID) {
    var Target;

    MarkerList.eachLayer(x => {
        if (x.options.id == MarkerID) {
            Target = x;
        }
    });

    return Target;
}

function GoToInfoCard(InfoCardID) {
    var offset = $('#' + InfoCardID)[0].offset;
    $('#sidebar').scrollTop(offset);
}

var Debug;
function ChangeSelectedMarker(Marker) {
    Debug = Marker;

    if (SelectedMarker && SelectedMarker.options.id != Marker.options.id) {
        //變回原本
        SelectedMarker.disablePermanentHighlight();
    }


    Marker.enablePermanentHighlight();
    SelectedMarker = Marker;


}



function HighlightCard(InfoCardID, SetHighlight) {

    if (SetHighlight) {
        $('#' + InfoCardID).addClass('Highlight')
    }
    else {
        $('#' + InfoCardID).removeClass("Highlight")
    }
}

//移動到行政區中心
function MoveToDist(DistName) {

    //LatLng
    var DistCentroid = {
        "中區": [24.141888013, 120.679804482],
        "北區": [24.1581428585, 120.683498903],
        "北屯區": [24.1880288155, 120.7300323],
        "南區": [24.1199976975, 120.664187697],
        "南屯區": [24.140008411, 120.614708526],
        "后里區": [24.31824236, 120.706916804],
        "和平區": [24.2744178915, 121.1586719105],
        "外埔區": [24.3308562075, 120.664206952],
        "大安區": [24.364894723, 120.5889586725],
        "大甲區": [24.377664787, 120.647017354],
        "大肚區": [24.1426289235, 120.550545064],
        "大里區": [24.096890937, 120.696084603],
        "大雅區": [24.230422522, 120.6395305685],
        "太平區": [24.104889889, 120.768589592],
        "新社區": [24.177718381, 120.832196048],
        "東勢區": [24.242211901, 120.826981867],
        "東區": [24.1372179785, 120.697305037],
        "梧棲區": [24.2531467875, 120.5244590325],
        "沙鹿區": [24.2308314645, 120.589870182],
        "清水區": [24.2963532855, 120.5775237475],
        "潭子區": [24.211336148, 120.7117449705],
        "烏日區": [24.0822314825, 120.6224202675],
        "石岡區": [24.2627186665, 120.7923585465],
        "神岡區": [24.26408336, 120.672282689],
        "西區": [24.1462138455, 120.6660924595],
        "西屯區": [24.184177944, 120.626238729],
        "豐原區": [24.2502006355, 120.735218065],
        "霧峰區": [24.0454620635, 120.7192361065],
        "龍井區": [24.208960917, 120.5272039265]
    };

    var Location = DistCentroid[DistName];

    if (Location) {
        map.setView(Location, 13)
    }

}