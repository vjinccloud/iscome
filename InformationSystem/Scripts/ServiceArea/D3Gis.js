class D3Gis {

    D3Svg

    Projection

    #Path
    #RedDotIcon = "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAUAAAAFCAYAAACNbyblAAAAHElEQVQI12P4//8/w38GIAXDIBKE0DHxgljNBAAO9TXL0Y4OHwAAAABJRU5ErkJggg==";

    constructor(svgSelector, projection) {
        this.D3Svg = d3.select(svgSelector);
        this.Projection = projection;
        this.#Path = d3.geoPath().projection(this.Projection);
    }

    get SvgDom() {
        return this.D3Svg.node();
    }

    Clear() {
        this.D3Svg.selectAll("*").remove();
    }

    /**
     * 繪製GeoJson資料
     * @param geoJson {object} GeoJson資料
     * @param geoJson.features {object} GeoJson的特徵資料
     * @param labelConfig {LabelConfig}
     */
    DrawGeoJson(geoJson, labelConfig = null) {

        let path = this.#Path;

        this.D3Svg.append("g")
            .selectAll("path")
            .data(geoJson.features)
            .enter().append("path")
            .attr("d", path);

        if (labelConfig) {
            this.D3Svg.append("g")
                .selectAll("path")
                .data(geoJson.features)
                .enter().append("text")
                .attr('transform', d => `translate(${path.centroid(d)})`) //位置
                .style('text-anchor', labelConfig.TextAnchor) //對齊方式
                .style('font-size', labelConfig.FontSize) //文字大小
                .style('font-family', labelConfig.FontFamily) //字體
                .attr('fill', labelConfig.Fill)//填滿樣式
                .attr('stroke', labelConfig.Stroke)//外框樣式
                .text(labelConfig.LabelTextSelector);
        }
    }

    /**
     * 產生校準用的點位
     * @param markers {Marker[]} 要繪製的點位集合
     * @returns {Promise<void>}
     */
    DrawCorrectionMarker(markers) {
        return new Promise((resolve) => {
            markers.forEach(marker => {
                let xy = this.Projection([marker.X, marker.Y]);
                let x = xy[0];
                let y = xy[1];

                //產生校正用的點
                this.D3Svg.append("svg:image")
                    .attr("class", "marker")
                    .attr("xlink:href", this.#RedDotIcon)
                    .attr("x", x - 3)
                    .attr("y", y - 3)
                    .attr("width", 6)
                    .attr("height", 6);
            })
            resolve();
        })
    }

    /**
     * 產生點位
     * @param markers {Marker[]} 要繪製的點位集合
     * @returns {Promise<void>}
     */
    DrawMarker(markers) {
        return new Promise((resolve) => {
            markers.forEach(marker => {
                let xy = this.Projection([marker.X, marker.Y]);
                let x = xy[0] + marker.Icon.OffsetX;
                let y = xy[1] + marker.Icon.OffsetY;

                //產生marker
                this.D3Svg.append("svg:image")
                    .attr("class", "marker")
                    .attr("xlink:href", marker.Icon.Base64Content)
                    .attr("x", x)
                    .attr("y", y)
                    .attr("width", marker.Icon.Width)
                    .attr("height", marker.Icon.Height);

            })

            resolve();
        })
    }

    #GenerateHiddenSvg(width, height) {
        let randomId = "TestSvg";

        let divContainer = document.createElement("div");
        //divContainer.setAttribute("style", "display: none");

        let svgElement = document.createElement("svg")
        svgElement.setAttribute("id", randomId)
        svgElement.setAttribute("width", width || "600")
        svgElement.setAttribute("height", height || "600")
        svgElement.setAttribute("fill", "lightgray")
        svgElement.setAttribute("stroke", "black");

        let body = document.querySelector("body");
        divContainer.append(svgElement);
        body.append(divContainer);

        return svgElement;
    }
}

/**
 * 點位標記
 */
class Marker {

    /**
     * 點位標記
     * @param lngX {number} x座標(經度)
     * @param latY {number} y座標(緯度)
     * @param icon {Icon} 標記圖示
     */
    constructor(lngX, latY, icon) {
        this.X = lngX;
        this.Y = latY;
        this.Icon = icon;
    }
}

/**
 * 標記圖示
 */
class Icon {

    /**
     * 標記圖示
     * @param base64Content {string} 圖示的Base64格式內容
     * @param width {number} 寬度
     * @param height {number} 高度
     * @param offsetX {number} X軸校正值
     * @param offsetY {number} Y軸校正值
     */
    constructor(base64Content, width, height, offsetX = 0, offsetY = 0) {
        this.Base64Content = base64Content;
        this.Width = width;
        this.Height = height;
        this.OffsetX = offsetX;
        this.OffsetY = offsetY;
    }

    /**
     * 圖示的Base64格式內容
     * @type {string}
     */
    Base64Content

    /**
     * X軸校正值
     * @type {number}
     */
    OffsetX

    /**
     * Y軸校正值
     * @type {number}
     */
    OffsetY

    /**
     * 圖示的寬度
     * @type {number}
     */
    Width

    /**
     * 圖示的高度
     * @type {number}
     */
    Height
}

/**
 * 文字標籤的設定
 */
class LabelConfig {


    /**
     * 文字標籤的設定
     * @param labelTextSelector 標籤文字，會將每個feature帶入作為第一個參數
     */
    constructor(labelTextSelector) {
        this.LabelTextSelector = labelTextSelector;
    }

    /**
     * 填滿樣式
     * @type {string}
     * @default black
     */
    Fill = "black";

    /**
     * 外框樣式
     * @type {string}
     * @default transparent
     */
    Stroke = "transparent";

    /**
     * 對齊方式
     * @type {string}
     * @default middle
     */
    TextAnchor = "middle";

    /**
     * 文字大小
     * @type {string}
     * @default 1em
     */
    FontSize = "1em"

    /**
     * 字體
     * @type {string}
     * @default Microsoft JhengHei, Arial
     */
    FontFamily = "Microsoft JhengHei, Arial"

    /**
     * 標籤文字，會將每個feature帶入作為第一個參數
     * @return {string}
     */
    LabelTextSelector;
}