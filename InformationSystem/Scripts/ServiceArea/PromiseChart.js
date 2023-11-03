/**
 * 讓ChartJs的圖表渲染有promise效果
 */
class PromiseChart {

    #chart
    #dom
    data

    options

    /**
     * 渲染圖表
     * @param id {string} Canvas的Id
     * @param type {string} 圖表類型
     * @param data {object} 圖表資料集
     * @param options {object} 設定值
     * @return {Promise} 渲染結果的Promise
     */
    constructor(id, type, data, options) {
        this.#dom = document.getElementById(id);
        this.#chart = new Chart(this.#dom, {
            type: type,
            data: {},
            options: {}
        });

        this.data = data;
        this.options = options
    }

    get Dom() {
        return this.#dom;
    }

    /**
     * 渲染圖表
     * @return {Promise} 渲染結果的Promise
     */
    Render() {

        return new Promise((resolve) => {
            this.options = this.#AddResolve(this.options, resolve);
            this.#chart.data = this.data;
            this.#chart.options = this.options;
            this.#chart.update();
        })
    }

    Destroy() {
        this.#chart.destroy();
    }

    /**
     * 在設定值中加入resolve
     * @private
     * @param options {object} chartJs的設定值
     * @param resolve {function} promise的resolve
     * @return {object}  包含resolve的chartJs設定值
     */
    #AddResolve(options, resolve) {
        if (!options || !options.animation) {
            options = (options || {});
            options.animation = {
                duration: 0,
                onComplete: function () {
                    resolve();
                }
            }
        } else {
            let origFunc = options.animation.onComplete;
            options.animation.onComplete = (animation) => {
                origFunc(animation);
                resolve();
            }
        }

        return options;
    }

}
