﻿L.Marker.include({
    _initIcon: function () {
        this._initIconOrigin();
        var i = this._light = L.DivIcon.prototype.createIcon();
        L.DomUtil.addClass(i, "light");
        L.DomUtil.create("span", "glow", i);
        L.DomUtil.create("span", "flare", i);
        if (this.options.highlight === "permanent") {
            L.DomUtil.addClass(i, "permanent")
        } else if (this.options.highlight === "temporary") {
            L.DomUtil.addClass(i, "temporary")
        }
        this.getPane().appendChild(i);
        this.on('remove', function () {
            i.remove()
        })
    },
    _setPos: function (i) {
        this._setPosOrigin(i);
        if (this._light) { L.DomUtil.setPosition(this._light, i) }
    },
    enableTemporaryHighlight: function () {
        this.options.highlight = "temporary";
        if (this._light) { L.DomUtil.addClass(this._light, "temporary") }
    },
    disableTemporaryHighlight: function (i) {
        delete this.options.highlight;
        if (this._light) { L.DomUtil.removeClass(this._light, "temporary") }
    },
    enablePermanentHighlight: function () {
        this.options.highlight = "permanent";
        if (this._light) { L.DomUtil.addClass(this._light, "permanent") }
    },
    disablePermanentHighlight: function (i) {
        delete this.options.highlight;
        if (this._light) { L.DomUtil.removeClass(this._light, "permanent") }
    },
    _initIconOrigin: function () {
        var i = this.options,
            t = "leaflet-zoom-" + (this._zoomAnimated ? "animated" : "hide");
        var e = i.icon.createIcon(this._icon),
            o = false;
        if (e !== this._icon) {
            if (this._icon) {
                this._removeIcon()
            }
            o = true;
            if (i.title) {
                e.title = i.title
            }
            if (i.alt) {
                e.alt = i.alt
            }
        }
        L.DomUtil.addClass(e, t);
        if (i.keyboard) {
            e.tabIndex = "0"
        }
        this._icon = e;
        if (i.riseOnHover) {
            this.on({
                mouseover: this._bringToFront,
                mouseout: this._resetZIndex
            })
        }
        var s = i.icon.createShadow(this._shadow),
            n = false;
        if (s !== this._shadow) {
            this._removeShadow();
            n = true
        }
        if (s) {
            L.DomUtil.addClass(s, t)
        }
        this._shadow = s;
        if (i.opacity < 1) {
            this._updateOpacity()
        }
        if (o) {
            this.getPane().appendChild(this._icon)
        }
        this._initInteraction();
        if (s && n) {
            this.getPane("shadowPane").appendChild(this._shadow)
        }
    },
    _setPosOrigin: function (i) {
        L.DomUtil.setPosition(this._icon, i);
        if (this._shadow) {
            L.DomUtil.setPosition(this._shadow, i)
        }
        this._zIndex = i.y + this.options.zIndexOffset;
        this._resetZIndex()
    }
});