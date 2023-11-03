/**
 * Canvas工具
 */
class CanvasS {

    /**
     * Canvas的2dContext
     * @type {CanvasRenderingContext2D}
     */
    Context2D

    CanvasDom

    /**
     * Canvas工具
     * @param canvasSelector {string} Canvas選擇器
     * @constructor
     */
    constructor(canvasSelector) {
        this.CanvasDom = document.querySelector(canvasSelector);
        this.Context2D =  this.CanvasDom.getContext("2d");
    }

    /**
     * 設定背景顏色
     * @param color {string} 顏色名稱
     */
    set BackgroundColor(color){
        this.Context2D.fillStyle = color;
        this.Context2D.fillRect(0, 0,  this.CanvasDom.width,  this.CanvasDom.height);
    }

    /**
     * 清除畫面
     */
    Clear(){
        this.Context2D.clearRect(0, 0, this.CanvasDom.width,  this.CanvasDom.height);
    }

    /**
     * 繪製Svg
     * @param x {number} 位置x
     * @param y {number} 位置y
     * @param width {number} 寬度
     * @param height {number} 高度
     * @param svgElement {SVGElement} 來源Svg的DOM
     */
    DrawSvg(x, y, width, height, svgElement) {
        //將svg資料序列化成 string
        let svgData = new XMLSerializer().serializeToString(svgElement);
        let src = "data:image/svg+xml;base64," + window.btoa(unescape(encodeURIComponent(svgData)));
        this.FillImage(x, y, width, height, src)
    };

    /**
     * Draws a rounded rectangle using the current state of the canvas.
     * If you omit the last three params, it will draw a rectangle
     * outline with a 5 pixel border radius
     * @param {Number} x The top left x coordinate
     * @param {Number} y The top left y coordinate
     * @param {Number} width The width of the rectangle
     * @param {Number} height The height of the rectangle
     * @param {Number} [radius=5] 邊角的圓弧度,預設值為 5;
     * @param {Boolean} [fill=false] Whether to fill the rectangle. Defaults to false.
     * @param {Boolean} [stroke=true] Whether to stroke the rectangle. Defaults to true.
     */
    DrawRoundRect(x, y, width, height, radius, fill, stroke) {

        let ctx = this.Context2D;

        if (typeof stroke == "undefined") {
            stroke = true;
        }
        if (typeof radius === "undefined") {
            radius = 5;
        }
        ctx.beginPath();
        ctx.moveTo(x + radius, y);
        ctx.lineTo(x + width - radius, y);
        ctx.quadraticCurveTo(x + width, y, x + width, y + radius);
        ctx.lineTo(x + width, y + height - radius);
        ctx.quadraticCurveTo(x + width, y + height, x + width - radius, y + height);
        ctx.lineTo(x + radius, y + height);
        ctx.quadraticCurveTo(x, y + height, x, y + height - radius);
        ctx.lineTo(x, y + radius);
        ctx.quadraticCurveTo(x, y, x + radius, y);
        ctx.closePath();
        if (stroke) {
            ctx.stroke();
        }
        if (fill) {
            ctx.fill();
        }
    }

    /**
     * 繪製Canvas
     * @param x {number} 位置x
     * @param y {number} 位置y
     * @param width {number} 寬度
     * @param height {number} 高度
     * @param canvasElement {HTMLCanvasElement} 來源Canvas的DOM
     */
    DrawCanvas(x, y, width, height, canvasElement) {
        //將canvas資料序列化成 string
        let src = canvasElement.toDataURL();
        this.FillImage(x, y, width, height, src);
    }

    /**
     * 連續產生多組文字
     * @param x {number} 起始位置x
     * @param y {number} 起始位置y
     * @param textAry {CanvasText[]} 文字參數
     */
    FillMultiText(x, y, textSet) {
/*        const defaultFillStyle = this.Context2D.fillStyle;
        const defaultFont = this.Context2D.font;

        let startPos = x;

        textSet.forEach(text => {
            this.Context2D.fillStyle = text.FillStyle || defaultFillStyle;
            this.Context2D.font = text.Font || defaultFont;
            this.Context2D.fillText(text.Text, startPos, y);
            startPos += this.Context2D.measureText(text.Text).width;
        })*/

        CanvasS.FillMultiText(this.Context2D,x,y,textSet);
    }

    static FillMultiText(ctx, x, y, textSet) {
        const defaultFillStyle = ctx.fillStyle;
        const defaultFont = ctx.font;

        let startPos = x;

        textSet.forEach(text => {
            ctx.fillStyle = text.FillStyle || defaultFillStyle;
            ctx.font = text.Font || defaultFont;
            ctx.fillText(text.Text, startPos, y);
            startPos += ctx.measureText(text.Text).width;
        })
    }

    /**
     * 連續產生多組文字
     * @param x {number} 起始位置x
     * @param y {number} 起始位置y
     * @param text {CanvasText} 文字參數
     */
    FillText(x, y, text) {
        const defaultFillStyle = this.Context2D.fillStyle;
        const defaultFont = this.Context2D.font;

        this.Context2D.fillStyle = text.FillStyle || defaultFillStyle;
        this.Context2D.font = text.Font || defaultFont;
        this.Context2D.fillText(text.Text, x, y);
    }


    /**
     * 插入圖片
     * @param ctx {CanvasRenderingContext2D} Canvas的2dContext
     * @param x {number} 位置x
     * @param y {number} 位置y
     * @param width {number} 寬度
     * @param height {number} 高度
     * @param src {string} 圖檔來源路徑
     */
    FillImage(x, y, width, height, src) {
        const img = new Image();
        img.onload = () => {
            this.Context2D.drawImage(img, x, y, width, height);
        };
        img.src = src;
    }
}

/**
 * Canvas文字
 */
class CanvasText {
    /**
     * 文字
     * @type {string}
     */
    Text

    /**
     * 文字樣式
     * @type {string}
     */
    FillStyle

    /**
     * 字型
     * @type {string}
     */
    Font

    /**
     * Canvas文字
     * @param text {string} 文字
     * @param font {string} 字型
     * @param [fillStyle="black"] {string} 文字樣式
     * @constructor
     */
    constructor(text, font= "1em Microsoft JhengHei", fillStyle="black") {
        this.Text = text;
        this.Font = font;
        this.FillStyle = fillStyle;
    }
}