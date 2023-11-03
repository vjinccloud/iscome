//需要CanvasS.js

/**
 * 用來在Canvas上產生表格個工具
 */
class CanvasTable {

    /**
     * Canvas的2dContext
     * @type {CanvasRenderingContext2D}
     */
    Context2D

    /**
     * 列的數量
     */
    RowNumber

    /**
     * 欄的數量
     */
    ColumnNumber

    /**
     * 各欄的欄寬
     * @type {number[]}
     */
    ColumnWidth

    /**
     * 各列的列高
     * @type {number[]}
     */
    RowHeight

    /**
     * 資料
     * @type {Cell[][]}
     */
    Data

    /**
     * 不繪製左上角的單元格
     * 臨時的作法，實作邊界樣式要花不少時間
     * @type {boolean}
     */
    SkipLeftTop = false

    CellPaddingTop = 4;
    CellPaddingRight = 4;
    CellPaddingBottom = 6;
    CellPaddingLeft = 6;


    /**
     * @param ctx {CanvasRenderingContext2D} Canvas的2dContext
     * @param data {Cell[][]} 表格資料
     */
    constructor(ctx, data) {
        this.Data = data;
        this.RowNumber = data.length;
        this.Context2D = ctx;
        this.#InitTable();
    }

    /**
     * 設定Cell內容與邊界的距離
     * @param top {number} 與上邊界的距離
     * @param right {number} 與右邊界的距離
     * @param bottom {number} 與下邊界的距離
     * @param left {number} 與左邊界的距離
     */
    SetCellPadding(top, right, bottom, left) {
        this.CellPaddingTop = top;
        this.CellPaddingRight = right;
        this.CellPaddingBottom = bottom;
        this.CellPaddingLeft = left;
        this.#InitTable();
    }

    /**
     * 渲染表格
     * @param startX {number} 起始位置X
     * @param startY {number} 起始位置Y
     */
    RenderTable(startX, startY) {

        // 繪製每個單元格
        let x, y = startY;
        for (let r = 0; r < this.RowNumber; r++) {
            x = startX;
            for (let c = 0; c < this.ColumnNumber; c++) {

                let w = this.ColumnWidth[c];
                let h = this.RowHeight[r];

                //畫框線
                if (!(this.SkipLeftTop && r === 0 && c === 0)) {
                    this.Context2D.rect(x, y, w, h);
                    this.Context2D.stroke();
                }

                //畫文字
                let cell = this.Data[r][c];
                if (cell) {

                    let textSet = Array.isArray(cell.Content) ? cell.Content : [cell.Content];
                    //計算文字起始位置
                    let posX;
                    switch (cell.TextAnchor) {
                        //靠左
                        case AnchorType.Left:
                            posX = x + this.CellPaddingLeft;
                            break;

                        //置中
                        case AnchorType.Middle:
                            let whiteSpace = w - this.CellPaddingLeft - this.CellPaddingRight - this.#CalculateTextBox(textSet).Width;
                            posX = x + this.CellPaddingLeft + (whiteSpace / 2);
                            break;

                        //靠右
                        case AnchorType.Right:
                            posX = x + (w - this.#CalculateTextBox(textSet).Width - this.CellPaddingRight);
                            break;
                    }

                    let posY = y + this.RowHeight[r] - this.CellPaddingBottom

                    CanvasS.FillMultiText(this.Context2D, posX, posY, textSet);
                }

                x += w;
            }


            y += this.RowHeight[r];
        }

    }

    /**
     * 初始化表格資訊
     */
    #InitTable() {
        let maxColNumber = 0;
        this.Data.forEach((row) => {
            if (row.length > maxColNumber) maxColNumber = row.length
        });
        this.ColumnNumber = maxColNumber;

        //計算各欄的欄寬
        let colWidth = [];
        let rowHeight = [];
        for (let row = 0; row < this.RowNumber; row++) {

            for (let col = 0; col < this.ColumnNumber; col++) {

                let w = 0, h = 0;
                let text = this.Data[row][col].Content;

                if (text) {
                    //計算文字的大小
                    let textBox = this.#CalculateTextBox(text);
                    w = textBox.Width;
                    h = textBox.Height;
                }

                colWidth[col] || colWidth.push([]);
                colWidth[col][row] = w + this.CellPaddingLeft + this.CellPaddingRight;

                rowHeight[row] || rowHeight.push([]);
                rowHeight[row][col] = h + +this.CellPaddingTop + this.CellPaddingBottom;
            }
        }

        //計算各欄的最大欄寬
        this.ColumnWidth = [];
        for (let i = 0; i < colWidth.length; i++) {
            this.ColumnWidth[i] = Math.max(...colWidth[i]);
        }

        //計算各列的最大列高
        this.RowHeight = [];
        for (let i = 0; i < rowHeight.length; i++) {
            this.RowHeight[i] = Math.max(...rowHeight[i]);
        }
    }

    /**
     * 計算文字大小
     * @param canvasTextSet {CanvasText|CanvasText[]} 要計算的文字
     * @return {{Height: number, Width: number}} 文字大小
     */
    #CalculateTextBox(canvasTextSet) {
        let w = 0, h = 0;
        const defaultFont = this.Context2D.font;
        if (canvasTextSet) {
            let textAry = Array.isArray(canvasTextSet) ? canvasTextSet : [canvasTextSet];
            textAry.forEach(text => {
                this.Context2D.font = text.Font || defaultFont;
                let metrics = this.Context2D.measureText(text.Text);
                let fontHeight = metrics.actualBoundingBoxAscent + metrics.actualBoundingBoxDescent;
                w += metrics.width;
                h = fontHeight > h ? fontHeight : h;
            })
        }

        return {Width: w, Height: h};

    }
}


/**
 * 單元格
 */
class Cell {

    /**
     * 單元格建構式
     * @param [content] {CanvasText|CanvasText[]} 單元格內容
     * @param [textAnchor=AnchorType.Left] {string} 文字的對齊方式
     */
    constructor(content, textAnchor = AnchorType.Left) {
        this.Content = content || new CanvasText("");
        this.TextAnchor = textAnchor;
    }

    /**
     * 單元格內容
     * @type {CanvasText|CanvasText[]}
     */
    Content

    /**
     * 文字的對齊方式
     * @type {string}
     */
    TextAnchor
}

/**
 * 文字的對齊方式列舉
 * @enum {string}
 */
class AnchorType {

    /**
     * 靠左對齊
     * @type {string}
     */
    static Left = "left";

    /**
     * 置中
     * @type {string}
     */
    static Middle = "middle";

    /**
     * 靠右對齊
     * @type {string}
     */
    static Right = "right";

}
