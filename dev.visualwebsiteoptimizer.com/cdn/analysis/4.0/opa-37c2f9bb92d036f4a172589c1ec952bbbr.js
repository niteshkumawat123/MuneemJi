! function() {
    var e = function(e, t) {
        var n = function() {
            e() ? t() : setTimeout(n, 100)
        };
        n()
    };
    e(function() {
        return VWO && VWO._ && VWO._.libLoaded
    }, function() {
        window.VWO = window.VWO || [], VWO.v_o = "4.0.391",
            function() {
                var i = function(e, t) {
                    return (i = Object.setPrototypeOf || {
                            __proto__: []
                        }
                        instanceof Array && function(e, t) {
                            e.__proto__ = t
                        } || function(e, t) {
                            for (var n in t) t.hasOwnProperty(n) && (e[n] = t[n])
                        })(e, t)
                };

                function e(e, t) {
                    function n() {
                        this.constructor = e
                    }
                    i(e, t), e.prototype = null === t ? Object.create(t) : (n.prototype = t.prototype, new n)
                }
                var R = function() {
                    return (R = Object.assign || function(e) {
                        for (var t, n = 1, i = arguments.length; n < i; n++)
                            for (var o in t = arguments[n]) Object.prototype.hasOwnProperty.call(t, o) && (e[o] = t[o]);
                        return e
                    }).apply(this, arguments)
                };

                function u(e, t) {
                    var n = {};
                    for (var i in e) Object.prototype.hasOwnProperty.call(e, i) && t.indexOf(i) < 0 && (n[i] = e[i]);
                    if (null != e && "function" == typeof Object.getOwnPropertySymbols) {
                        var o = 0;
                        for (i = Object.getOwnPropertySymbols(e); o < i.length; o++) t.indexOf(i[o]) < 0 && Object.prototype.propertyIsEnumerable.call(e, i[o]) && (n[i[o]] = e[i[o]])
                    }
                    return n
                }

                function n() {
                    for (var e = 0, t = 0, n = arguments.length; t < n; t++) e += arguments[t].length;
                    var i = Array(e),
                        o = 0;
                    for (t = 0; t < n; t++)
                        for (var r = arguments[t], a = 0, s = r.length; a < s; a++, o++) i[o] = r[a];
                    return i
                }
                var o = parseInt(new Date / 1e3, 10),
                    t, r = function() {
                        return t || (t = VWO.data.ts || o)
                    },
                    a = Object.keys || function(e) {
                        var t, n = [];
                        for (t in e) e.hasOwnProperty(t) && n.push(t);
                        return n
                    };

                function s(e, t) {
                    for (var n in t) t.hasOwnProperty(n) && (e[n] = t[n])
                }

                function d(e, t) {
                    for (var n = [], i = 0; i < e.length; i++) n.push(t(e[i]));
                    return n
                }

                function c(e, t) {
                    for (var n = [], i = 0; i < e.length; i++) t(e[i], i) && n.push(e[i]);
                    return n
                }

                function C(e) {
                    var t = r(),
                        n = parseInt(new Date / 1e3, 10) - o;
                    return e ? t + n : 1e3 * (t + n) + +new Date % 1e3
                }

                function l(e) {
                    return 1e3 * (r() + (parseInt(e / 1e3, 10) - o)) + +new Date % 1e3
                }

                function N(i, o) {
                    var r, a = !1;
                    return function() {
                        for (var e = [], t = 0; t < arguments.length; t++) e[t] = arguments[t];
                        var n = this;
                        a && (clearTimeout(r), r = null), r = setTimeout(function() {
                            i.apply(n, e)
                        }, o), a = !0
                    }
                }

                function h(e, t, n) {
                    "Array" === e ? (this.tags = [], this.lastSent = 0) : "Hash" === e && (this.tags = {}, this.sentTags = {}), this.type = e, this.maxCount = t || 1 / 0, this.addTagCallback = n || function() {}
                }
                h.prototype.add = function(e, t) {
                    if (e) {
                        var n = this.tags;
                        "Array" === this.type ? ("[object Array]" !== Object.prototype.toString.call(e) && (e = [e]), e = d(e, function(e) {
                            return e = encodeURIComponent(e.trim())
                        }), n = c(n = (n = n.concat(e)).slice(0, this.maxCount), function(e, t) {
                            return n.indexOf(e) === t
                        }), this.tags = n) : "Hash" === this.type && (this.sentTags[e] && this.sentTags[e] === t || (this.tags[encodeURIComponent(e)] = encodeURIComponent(t))), this.addTagCallback()
                    }
                }, h.prototype.get = function() {
                    var e;
                    if (this.isTagPassed()) return "Array" === this.type ? (e = this.tags.slice(this.lastSent), this.lastSent = this.tags.length) : "Hash" === this.type && (e = this.tags, s(this.sentTags, this.tags), this.tags = {}), e
                }, h.prototype.isTagPassed = function() {
                    return "Array" === this.type ? this.tags.length > this.lastSent : "Hash" === this.type && 0 < a(this.tags).length
                }, h.prototype.reset = function() {
                    "Array" === this.type ? (this.tags = [], this.lastSent = 0) : "Hash" === this.type && (this.tags = {}, this.sentTags = {})
                }, h.prototype.refresh = function() {
                    "Array" === this.type ? this.lastSent = 0 : "Hash" === this.type && (s(this.tags, this.sentTags), this.sentTags = {})
                };
                var D = {
                        completeSnapshottingEnabled: function(e) {
                            void 0 === e && (e = window.location.href);
                            return -1 < e.indexOf("att-bundles.com") || -1 < [646513, 710789].indexOf(window._vwo_acc_id)
                        },
                        customOffsetEnabled: function() {
                            return -1 < [373511, 515823, 895609].indexOf(window._vwo_acc_id)
                        },
                        blockParamsInSnapshotting: function() {
                            return -1 < [644713, 731483, 740288, 743738, 783334, 784474, 791992, 835565, 857247, 874753, 969074, 989310, 1014514, 988428, 944335, 885778, 721532, 857208, 1042399, 856323, 853763].indexOf(window._vwo_acc_id) || 1045717 <= window._vwo_acc_id
                        },
                        deadClickInHeatmap: function() {
                            return this.data360migrated()
                        },
                        data360migrated: function() {
                            return void 0 !== VWO.phoenix
                        },
                        repeatedCSSInsertion: function() {
                            return -1 < [637746].indexOf(window._vwo_acc_id)
                        },
                        customClicks: function() {
                            return -1 < [884017, 884893].indexOf(window._vwo_acc_id) && window.VWO && void 0 !== window.VWO.phoenix
                        },
                        pageExitHandling: function() {
                            return !(-1 < [743033].indexOf(window._vwo_acc_id)) && (this.data360migrated() || this.preloadListeners())
                        },
                        marketoOptimization: function() {
                            return -1 < [646513, 891895, 844055].indexOf(window._vwo_acc_id)
                        },
                        checkFormVisibility: function() {
                            return 1006624 <= window._vwo_acc_id || 708874 == window._vwo_acc_id
                        },
                        preloadListeners: function() {
                            return -1 < [721532, 768475].indexOf(window._vwo_acc_id)
                        },
                        campaignLog: function() {
                            return !1
                        },
                        ignoredSelectors: function() {
                            return window.VWO.data.ISFI || ""
                        },
                        blobSupport: function() {
                            return -1 < [1009354, 737109].indexOf(window._vwo_acc_id)
                        }
                    },
                    f = {
                        DEAD: "11",
                        RAGE: "12",
                        ERROR: "13"
                    },
                    I = {
                        RECORDING_META: "rm",
                        CLICK_TRACK_COUNT: "ctc",
                        CLICK_TEXT_DATA: "ctd",
                        CLICKS_TRACKED: "ct",
                        DEAD_CLICK_TEXT: "dCT",
                        RAGE_CLICK_TEXT: "rCT"
                    },
                    m = {
                        JSON: {
                            stringify: function(i) {
                                function e(e) {
                                    return "string" == typeof e
                                }

                                function n(e) {
                                    return null === e && "object" == typeof e
                                }

                                function o(e) {
                                    return "number" == typeof e && isNaN(e)
                                }

                                function r(e) {
                                    return "number" == typeof e && !isFinite(e)
                                }

                                function a(e) {
                                    return "symbol" == typeof e
                                }

                                function s(e) {
                                    return void 0 === (t = e) && void 0 === t || "function" == typeof e || a(e);
                                    var t
                                }

                                function t(e) {
                                    var t = e.split("");
                                    return t.pop(), t.join("")
                                }
                                var d = this;
                                if (!s(i)) {
                                    if ("object" == typeof(c = i) && null !== c && "function" == typeof c.getMonth) return '"' + i.toISOString() + '"';
                                    var c, l;
                                    if (o(l = i) || r(l) || n(l)) return "null";
                                    if (!a(i)) {
                                        if ("number" == typeof(f = i) || e(f) || "boolean" == typeof f) {
                                            var u = void 0,
                                                h = void 0;
                                            return e(i) ? (h = '"', u = (u = (u = (u = (u = i.toString()).replace(/\\/g, "\\\\")).replace(/\n/g, "\\n")).replace(/\t/g, "\\t")).replace(/\"/g, '\\"')) : h = "", u ? "" + h + u + h : "" + h + i + h
                                        }
                                        var f, m, p;
                                        if (m = i, Array.isArray(m) && "object" == typeof m) {
                                            var g = "";
                                            return i.forEach(function(e) {
                                                var t;
                                                g += o(t = e) || r(t) || n(t) || s(t) ? d.stringify(null) : d.stringify(e), g += ","
                                            }), "[" + t(g) + "]"
                                        }
                                        if ("object" == typeof(p = i) && null !== p && !Array.isArray(p)) {
                                            var v = "";
                                            return Object.keys(i).forEach(function(e) {
                                                var t = i[e],
                                                    n = s(t);
                                                v += n ? "" : d.stringify(e) + ":" + d.stringify(t) + ","
                                            }), "{" + t(v) + "}"
                                        }
                                    }
                                }
                            }
                        }
                    },
                    p = function() {
                        function e(e, t) {
                            try {
                                Object.defineProperty(e, t, {
                                    writable: !1
                                })
                            } catch (e) {}
                        }

                        function n() {
                            if (!window.DISABLE_NATIVE_CONSTANTS) {
                                if (!document.body) return window.DISABLE_NATIVE_CONSTANTS = !0, 0;
                                i = window.document.createElement("iframe"), e(i, "src"), i.setAttribute = function(e, t) {}, i.style.display = "none", i.onload = function() {
                                    (o = i.contentWindow).onerror = function(e, t, n, i) {
                                        window.VWO && window.VWO._ && window.VWO._.customError && window.VWO._.customError({
                                            msg: e,
                                            url: t,
                                            lineno: n,
                                            colno: i,
                                            source: "nativeConstants"
                                        })
                                    }
                                }, document.body.appendChild(i), (o = i.contentWindow) && e(o.location, "href")
                            }
                        }
                        var i, o;
                        return void 0 === window.DISABLE_NATIVE_CONSTANTS ? window.DISABLE_NATIVE_CONSTANTS = !0 : !1 === window.DISABLE_NATIVE_CONSTANTS && n(), {
                            get: function(e) {
                                i && i.contentWindow || n();
                                var t = o;
                                return t && !window.DISABLE_NATIVE_CONSTANTS || (t = window), window.VWO.featureInfo && window.VWO.featureInfo.vwoNative && m[e] ? ("JSON" === e && (m[e].parse = t[e].parse), m[e]) : t[e]
                            }
                        }
                    };
                window.VWO = window.VWO || [], window.VWO._ = window.VWO._ || {}, window.VWO._.nativeConstants = window.VWO._.nativeConstants || p();
                var A = window.VWO._.nativeConstants,
                    g = {
                        create: function(e, t, n, i) {
                            var o = "";
                            if (n) {
                                var r = new(A.get("Date"));
                                r.setTime(r.getTime() + 24 * n * 60 * 60 * 1e3), o = "; expires=" + r.toGMTString()
                            } else !1 === n && (o = "; expires=Thu, 01 Jan 1970 00:00:01 GMT");
                            i = i ? "; domain=" + i : "", document.cookie = e + "=" + t + o + "; path=/" + i
                        },
                        get: function(e) {
                            for (var t = e + "=", n = document.cookie.split(";"), i = 0; i < n.length; i++) {
                                for (var o = n[i];
                                    " " === o.charAt(0);) o = o.substring(1, o.length);
                                if (0 === o.indexOf(t)) return o.substring(t.length, o.length)
                            }
                            return null
                        },
                        erase: function(e, t) {
                            this.create(e, "", !1, t)
                        }
                    };

                function Y(e, t, n, i) {
                    VWO._ && VWO._.customError && window.VWO._.customError({
                        msg: e,
                        url: "gquery.js",
                        lineno: t,
                        colno: n,
                        source: i
                    })
                }
                var v = function() {
                        var l = document,
                            i = l.documentElement,
                            o = [].slice,
                            u = [].push,
                            r = [].map,
                            t = [].filter,
                            e = l.createElement("div"),
                            a = [].indexOf,
                            n = [].splice,
                            s = [].reverse,
                            d = window,
                            c = /^data-(.+)/,
                            h = /\S+/g,
                            f = /^(\s|\u00A0)+|(\s|\u00A0)+$/g,
                            m = {
                                animationIterationCount: !0,
                                columnCount: !0,
                                flexGrow: !0,
                                flexShrink: !0,
                                fontWeight: !0,
                                lineHeight: !0,
                                opacity: !0,
                                order: !0,
                                orphans: !0,
                                widows: !0,
                                zIndex: !0
                            };

                        function p(e) {
                            return e.multiple && e.options ? function(e, t, n, i) {
                                for (var o = [], r = H(t), a = i, s = 0, d = e.length; s < d; s++)
                                    if (r) {
                                        var c = t(e[s]);
                                        c.length && u.apply(o, c)
                                    } else
                                        for (var l = e[s][t]; !(null == l || i && a(-1, l));) o.push(l), l = n ? l[t] : null;
                                return o
                            }(t.call(e.options, function(e) {
                                return e.selected && !e.disabled && !e.parentNode.disabled
                            }), "value") : e.value || ""
                        }

                        function g(e) {
                            return (g = "function" == typeof Symbol && "symbol" == typeof Symbol.iterator ? function(e) {
                                return typeof e
                            } : function(e) {
                                return e && "function" == typeof Symbol && e.constructor === Symbol && e !== Symbol.prototype ? "symbol" : typeof e
                            })(e)
                        }
                        var v = {
                                focus: "focusin",
                                blur: "focusout"
                            },
                            w = /^(?:mouse|pointer|contextmenu|drag|drop|click|dblclick)/i;
                        var _ = /\S+/g;
                        var y = {
                            focus: {
                                delegateType: "focusin"
                            },
                            blur: {
                                delegateType: "focusout"
                            },
                            mouseenter: {
                                delegateType: "mouseover",
                                bindType: "mouseover"
                            },
                            mouseleave: {
                                delegateType: "mouseout",
                                bindType: "mouseout"
                            },
                            pointerenter: {
                                delegateType: "pointerover",
                                bindType: "pointerover"
                            },
                            pointerleave: {
                                delegateType: "pointerout",
                                bindType: "pointerout"
                            }
                        };
                        Element.prototype.closest || (Element.prototype.closest = function(e) {
                            var t = this;
                            if (!document.documentElement.contains(t)) return null;
                            do {
                                if (T(t, e)) return t;
                                t = t.parentElement || t.parentNode
                            } while (null !== t && 1 === t.nodeType);
                            return null
                        });
                        var E = function e(t, n) {
                                return new e.fn.init(t, n)
                            },
                            T = E.matches = function(e, t) {
                                var n = e && (e.matches || e.webkitMatchesSelector || e.mozMatchesSelector || e.msMatchesSelector || e.oMatchesSelector);
                                return !!n && n.call(e, t)
                            },
                            S = E.isString = function(e) {
                                return g(e) === g("")
                            },
                            b = /^--/;

                        function O(e) {
                            return b.test(e)
                        }
                        var R = /-([a-z])/g;

                        function C(e, t) {
                            return t.toUpperCase()
                        }
                        var N = E.camelCase = function(e) {
                            return e.replace(R, C)
                        };

                        function D(e) {
                            return e && 1 === e.nodeType
                        }
                        var I = {},
                            A = e.style,
                            x = ["webkit", "moz", "ms", "o"];

                        function k(e, t) {
                            return parseInt(L(e, t), 10) || 0
                        }

                        function L(e, t, n) {
                            if (D(e) && t) {
                                var i = d.getComputedStyle(e, null);
                                return t ? n ? i.getPropertyValue(t) || void 0 : i[t] : i
                            }
                        }

                        function M() {}

                        function P(e) {
                            return e[U] = e[U] || {}
                        }

                        function V(e) {
                            return e && 9 === e.nodeType
                        }
                        var W, H = E.isFunction = function(e) {
                                return g(e) === g(M) && !!e.call
                            },
                            U = E.uid = "_gQ" + Date.now(),
                            z = E.isWindow = function(e) {
                                return e && e === e.window
                            },
                            F = E.isNumeric = function(e) {
                                return !isNaN(parseFloat(e)) && isFinite(e)
                            };

                        function j(e, t) {
                            for (var n = 0, i = e.length; n < i && !1 !== t.call(e[n], n, e[n]); n++);
                        }

                        function X(e, t, o) {
                            j(e, function(n, i) {
                                j(t, function(e, t) {
                                    B(i, n ? t.cloneNode(!0) : t, o, o && i.firstChild)
                                })
                            })
                        }

                        function B(e, t, n, i) {
                            if (j(t && 3 === t.nodeType ? [] : E("script", t), function(e, t) {
                                    var n = document.createElement("script");
                                    j(E(t).prop("attributes"), function() {
                                        E(n).attr(this.name, this.value)
                                    }), n.text = t.innerHTML, document.getElementsByTagName("head")[0].appendChild(n), t.parentElement.removeChild(t)
                                }), n)
                                if ("SCRIPT" === t.tagName || "STYLE" === t.tagName) {
                                    var o = document.createElement(t.tagName.toLowerCase());
                                    "SCRIPT" === t.tagName ? o.text = t.innerHTML : o.appendChild(document.createTextNode(t.innerHTML)), j(E(t).prop("attributes"), function() {
                                        E(o).attr(this.name, this.value)
                                    }), o.classList = t.classList, e.insertBefore(o, i)
                                } else e.insertBefore(t, i);
                            else if ("SCRIPT" === t.tagName || "STYLE" === t.tagName) {
                                o = document.createElement(t.tagName.toLowerCase());
                                "SCRIPT" === t.tagName ? o.text = t.innerHTML : o.appendChild(document.createTextNode(t.innerHTML)), j(E(t).prop("attributes"), function() {
                                    E(o).attr(this.name, this.value)
                                }), o.classList = t.classList, e.appendChild(o)
                            } else e.appendChild(t)
                        }
                        return E.extend = function() {
                            var e, t, n, i, o = arguments[0] || {},
                                r = 1,
                                a = arguments.length,
                                s = !1;
                            for ("boolean" == typeof o && (s = o, o = arguments[1] || {}, r = 2), "object" === g(o) || H(o) || (o = {}), a === r && (o = this, --r); r < a; r++)
                                if (null != (e = arguments[r]))
                                    for (t in e)
                                        if (n = o[t], i = e[t], "__proto__" !== t && o !== i)
                                            if (s && i && (E.isPlainObject(i) || E.isArray(i))) {
                                                var d = n && (E.isPlainObject(n) || E.isArray(n)) ? n : E.isArray(i) ? [] : {};
                                                o[t] = E.extend(s, d, i)
                                            } else void 0 !== i && (o[t] = i);
                            return o
                        }, E.isArray = Array.isArray, E.isPlainObject = function(e) {
                            if (!e || "[object Object]" !== Object.prototype.toString.call(e) || e.nodeType || e.setInterval) return !1;
                            if (e.constructor && !hasOwnProperty.call(e, "constructor") && !hasOwnProperty.call(e.constructor.prototype, "isPrototypeOf")) return !1;
                            var t;
                            for (t in e);
                            return void 0 === t || hasOwnProperty.call(e, t)
                        }, E.parseJSON = function(e) {
                            return "string" == typeof e && e ? /^[\],:{}\s]*$/.test(e.replace(/\\(?:["\\\/bfnrt]|u[0-9a-fA-F]{4})/g, "@").replace(/"[^"\\\n\r]*"|true|false|null|-?\d+(?:\.\d*)?(?:[eE][+\-]?\d+)?/g, "]").replace(/(?:^|:|,)(?:\s*\[)+/g, "")) ? JSON.parse(e) : void 0 : null
                        }, E.getJSON = function(e, t, n, i) {
                            return H(t) && (i = i || n, n = t, t = null), E.ajax({
                                url: e,
                                data: t,
                                success: n,
                                dataType: i
                            })
                        }, E.get = function(e, t, n, i) {
                            return H(t) && (i = i || n, n = t, t = null), E.ajax({
                                type: "GET",
                                url: e,
                                data: t,
                                success: n,
                                dataType: i
                            })
                        }, E.each = function() {
                            var e, t, n = arguments;
                            t = 1 === n.length && H(n[0]) ? (e = o.call(this), n[0]) : (e = n[0], n[1]);
                            for (var i = 0; i < e.length; i++) t.call(e[i], i, e[i]);
                            return this
                        }, E.ajax = function(e) {
                            if ("script" === e.dataType) {
                                var t = document.createElement("script");
                                return t.src = e.url, document.getElementsByTagName("head")[0].appendChild(t), t.onload = e.success || M, void(t.onerror = e.error || M)
                            }
                            var n = new XMLHttpRequest;
                            n.open(e.method ? e.method : "GET", e.url, !0), e.data || (e.data = null), n.onload = function() {
                                200 <= this.status && this.status < 400 && (e.dataType || (this.response = E.parseJSON(this.response)), e.success && e.success(this.response))
                            }, n.onerror = function() {
                                e.error && e.error(this.response)
                            }, n.send(e.data)
                        }, E.isEmptyObject = function(e) {
                            return e && 0 === Object.keys(e).length
                        }, E.fn = E.prototype = {
                            gQVersion: "0.0.1",
                            toArray: function() {
                                return o.call(this, 0)
                            },
                            constructor: E,
                            hasClass: function(t) {
                                return o.call(this).every(function(e) {
                                    return e && 1 === e.nodeType && e.classList.contains(t)
                                })
                            },
                            ready: function(e) {
                                return "loading" !== l.readyState ? setTimeout(e) : l.addEventListener("DOMContentLoaded", e), this
                            },
                            scrollTop: function() {
                                var e = this[0];
                                return z(e) ? e.pageYOffset : V(e) ? e.defaultView.pageYOffset : e.scrollTop
                            },
                            scrollLeft: function() {
                                var e = this[0];
                                return z(e) ? e.pageXOffset : V(e) ? e.defaultView.pageXOffset : e.scrollLeft
                            },
                            getComputedDimensionOuter: function(e, t) {
                                var n = "height" === e.toLowerCase() ? 1 : 0,
                                    i = this[0];
                                if (i) return z(i) ? window["outer" + e] : this[0]["offset" + e] + (t ? k(this[0], "margin" + (n ? "Top" : "Left")) + k(this[0], "margin" + (n ? "Bottom" : "Right")) : 0)
                            },
                            getComputedDimension: function(e) {
                                var t, n, i = this[0],
                                    o = "height" === e.toLowerCase() ? 0 : 1;
                                if (e = e.charAt(0).toUpperCase() + e.slice(1), V(i)) {
                                    var r = i.documentElement;
                                    return Math.max(i.body["scroll" + e], i.body["offset" + e], r["scroll" + e], r["offset" + e], r["client" + e])
                                }
                                if (z(i)) return "height" === e.toLowerCase() ? i.innerHeight : i.innerWidth;
                                try {
                                    return i.getBoundingClientRect()[e.toLowerCase()] - (k(t = i, "border" + ((n = o) ? "Left" : "Top") + "Width") + k(t, "padding" + (n ? "Left" : "Top")) + k(t, "padding" + (n ? "Right" : "Bottom")) + k(t, "border" + (n ? "Right" : "Bottom") + "Width"))
                                } catch (e) {
                                    Y("Error is " + e + " and elem is " + i, 529, 25, "getBoundingClientRect")
                                }
                            },
                            height: function() {
                                return this.getComputedDimension("height")
                            },
                            width: function() {
                                return this.getComputedDimension("width")
                            },
                            is: function(n) {
                                if (!n) return !1;
                                var i = !1;
                                return this.each(function(e, t) {
                                    return !(i = "string" == typeof n ? T(t, n) : t === n)
                                }), i
                            },
                            attr: function(n, i) {
                                var e;
                                if (n) {
                                    if (S(n)) return void 0 === i ? null === (e = this[0] ? this[0].getAttribute ? this[0].getAttribute(n) : this[0][n] : void 0) ? void 0 : e : this.each(function(e, t) {
                                        t.setAttribute ? t.setAttribute(n, i) : t[n] = i
                                    });
                                    for (var t in n) this.attr(t, n[t]);
                                    return this
                                }
                            },
                            removeAttr: function(t) {
                                return t = t.match(h) || [], this.each(function(e, n) {
                                    j(t, function(e, t) {
                                        n.removeAttribute(t)
                                    })
                                })
                            },
                            outerWidth: function(e) {
                                return this.getComputedDimensionOuter("Width", e)
                            },
                            outerHeight: function(e) {
                                return this.getComputedDimensionOuter("Height", e)
                            },
                            offset: function() {
                                var t = this[0];
                                if (!t) return {
                                    top: 0,
                                    left: 0
                                };
                                t.nodeType == Node.TEXT_NODE && (t = t.parentElement);
                                var e = {};
                                try {
                                    e = t.getBoundingClientRect()
                                } catch (e) {
                                    if (Y("Error is " + e + " and elem is " + t, 603, 25, "getBoundingClientRect"), t === document) return
                                }
                                var n = t.ownerDocument ? t.ownerDocument.defaultView : window;
                                return {
                                    top: e.top + n.pageYOffset - i.clientTop,
                                    left: e.left + n.pageXOffset - i.clientLeft
                                }
                            },
                            index: function(e) {
                                var t = e ? E(e)[0] : this[0],
                                    n = e ? this : E(t).parent().children();
                                return a.call(n, t)
                            },
                            each: E.each,
                            delegate: function(e, t, n, i) {
                                return this.on(e, t, n, i)
                            },
                            on: function(n, i, o, r) {
                                var a, e, s = this;
                                return H(i) && (o = i, i = null), this[0] === document && "ready" === n ? this.ready(o) : (i && (a = o, o = function(e) {
                                    for (var t = e.target; !T(t, i);) {
                                        if (t === this || !t) return !1;
                                        t = t.parentNode
                                    }
                                    t && a.call(t, e)
                                }), j(S(e = n) && e.match(_) || [], function(e, t) {
                                    y[t] && (i && y[t].delegateType ? n = y[t].delegateType : y[t].bindType && (n = y[t].bindType)), s.each(function(e, t) {
                                        t.addEventListener(n, o, !!r)
                                    })
                                })), this
                            },
                            off: function(n, i, o) {
                                return this.each(function(e, t) {
                                    t.removeEventListener(n, i, !!o)
                                })
                            },
                            isChecked: function() {
                                return null !== this[0].getAttribute("checked")
                            },
                            isFocussed: function() {
                                return this[0] === l.activeElement
                            },
                            closest: function(e) {
                                return new E(this[0].closest(e))
                            },
                            parent: function() {
                                return new E(this[0] && this[0].parentNode)
                            },
                            val: function(o) {
                                return arguments.length ? this.each(function(e, t) {
                                    var n = t.multiple && t.options;
                                    if (n || /radio|checkbox/i.test(t.type)) {
                                        var i = Array.isArray(o) ? r.call(o, String) : null === o ? [] : [String(o)];
                                        n ? j(t.options, function(e, t) {
                                            t.selected = 0 <= i.indexOf(t.value)
                                        }) : t.checked = 0 <= i.indexOf(t.value)
                                    } else t.value = null == o ? "" : o
                                }) : this[0] && p(this[0])
                            },
                            prop: function(n, i) {
                                if (n) {
                                    if (S(n)) return void 0 === i ? this[0][n] : this.each(function(e, t) {
                                        t[n] = i
                                    });
                                    for (var e in n) this.prop(e, n[e]);
                                    return this
                                }
                            },
                            data: function(o, r) {
                                var e, t, n, i = this;
                                if (!o) {
                                    if (!this[0]) return;
                                    var a = {};
                                    return j(this[0].attributes, function(e, t) {
                                        var n = t.name.match(c);
                                        n && (a[n[1]] = i.data(n[1]))
                                    }), a
                                }
                                if (S(o)) return void 0 === r ? (e = this[0], t = o, void 0 === (n = P(e)[t]) && (n = e.dataset ? e.dataset[t] : E(e).attr("data-" + t)), n) : this.each(function(e, t) {
                                    return n = o, i = r, P(t)[n] = i;
                                    var n, i
                                });
                                for (var s in o) this.data(s, o[s]);
                                return this
                            },
                            eq: function(e) {
                                return E(this.get(e))
                            },
                            get: function(e) {
                                return void 0 === e ? o.call(this) : e < 0 ? this[e + this.length] : this[e]
                            },
                            appendTo: function(e) {
                                for (var t = E(e), n = 0; n < t.length; n++) t[n].appendChild(this[0]);
                                return this
                            },
                            find: function(e) {
                                return this[0] || (e = void 0), E(e, this[0])
                            },
                            toggleClass: function(e, i, o) {
                                var r = [],
                                    a = void 0 !== i;
                                return S(e) && (r = e.match(h) || []), this.each(function(e, t) {
                                    if (t && 1 === t.nodeType)
                                        for (var n = 0; n < r.length; n++) a ? (o = i ? "add" : "remove", t.classList[o](r[n])) : t.classList.toggle(r[n])
                                })
                            },
                            addClass: function(e) {
                                return this.toggleClass(e, !0, "add"), this
                            },
                            removeClass: function(e) {
                                return e ? this.toggleClass(e, !1, "remove") : this.attr("class", ""), this
                            },
                            remove: function() {
                                return this.each(function(e, t) {
                                    t.parentNode.removeChild(t)
                                }), this
                            },
                            children: function() {
                                var n = [];
                                return this.each(function(e, t) {
                                    u.apply(n, t.children)
                                }), E(n)
                            },
                            map: function(n) {
                                return E(r.call(this, function(e, t) {
                                    return n.call(e, t, e)
                                }))
                            },
                            clone: function() {
                                return this.map(function(e, t) {
                                    return t.cloneNode(!0)
                                })
                            },
                            filter: function(n) {
                                var i = n;
                                return S(i) && (i = function(e, t) {
                                    return T(t, n)
                                }), E(t.call(this, function(e, t) {
                                    return i.call(e, t, e)
                                }))
                            },
                            parents: function(t) {
                                var i = [];
                                return this.each(function(e, t) {
                                    for (var n = t.parentNode; n && 9 !== n.nodeType;) i.push(n), n = n.parentNode
                                }), i = i.filter(function(e, t) {
                                    return i.indexOf(e) === t
                                }), t && (i = i.filter(function(e) {
                                    return T(e, t)
                                })), E(i)
                            },
                            append: function() {
                                var n = this;
                                return j(arguments, function(e, t) {
                                    X(n, E(t))
                                }), this
                            },
                            prepend: function() {
                                var n = this;
                                return j(arguments, function(e, t) {
                                    X(n, E(t), !0)
                                }), this
                            },
                            html: function(n) {
                                return void 0 === n ? this[0] && this[0].innerHTML : this.each(function(e, t) {
                                    t.innerHTML = n
                                })
                            },
                            css: function(n, i) {
                                if (S(n)) {
                                    var o = O(n);
                                    return (n = function(n, e) {
                                        if (void 0 === e && (e = O(n)), e) return n;
                                        if (!I[n]) {
                                            var t = N(n),
                                                i = "" + t.charAt(0).toUpperCase() + t.slice(1);
                                            j((t + " " + x.join(i + " ") + i).split(" "), function(e, t) {
                                                if (t in A) return I[n] = t, !1
                                            })
                                        }
                                        return I[n]
                                    }(n, o), arguments.length < 2) ? this[0] && L(this[0], n, o) : n ? (e = n, t = i, void 0 === (r = o) && (r = O(e)), i = r || m[e] || !F(t) ? t : t + "px", this.each(function(e, t) {
                                        D(t) && (o ? t.style.setProperty(n, i) : t.style[n] = i)
                                    })) : this
                                }
                                var e, t, r;
                                for (var a in n) this.css(a, n[a]);
                                return this
                            },
                            hashchange: function(e) {
                                window.addEventListener("hashchange", e)
                            },
                            replaceWith: function(o) {
                                return this.each(function(e, t) {
                                    var n = t.nextSibling,
                                        i = t.parentNode;
                                    E(t).remove(), n ? E(n).before(o) : E(i).append(o)
                                })
                            },
                            before: function() {
                                var n = this;
                                return j(arguments, function(e, t) {
                                    E(t).insertBefore(n)
                                }), this
                            },
                            after: function() {
                                var n = this;
                                return j(s.apply(arguments), function(e, t) {
                                    s.apply(E(t).slice()).insertAfter(n)
                                }), this
                            },
                            insertBefore: function(e) {
                                var t = this;
                                return E(e).each(function(n, i) {
                                    var o = i.parentNode;
                                    o && t.each(function(e, t) {
                                        B(o, n ? t.cloneNode(!0) : t, !0, i)
                                    })
                                }), this
                            },
                            insertAfter: function(e) {
                                var t = this;
                                return E(e).each(function(n, i) {
                                    var o = i.parentNode;
                                    o && t.each(function(e, t) {
                                        B(o, n ? t.cloneNode(!0) : t, !0, i.nextSibling)
                                    })
                                }), this
                            },
                            trigger: function(e, t) {
                                var n, i;
                                if (S(e)) {
                                    var o = [(i = e.split("."))[0], i.slice(1).sort()],
                                        r = o[0],
                                        a = o[1],
                                        s = w.test(r) ? "MouseEvents" : "HTMLEvents";
                                    (n = l.createEvent(s)).initEvent(r, !0, !0), n.namespace = a.join(".")
                                } else n = e;
                                n.data = t;
                                var d = n.type in v;
                                return this.each(function(e, t) {
                                    d && H(t[n.type]) ? t[n.type]() : t.dispatchEvent(n)
                                })
                            },
                            contents: function() {
                                return this[0] ? E(this[0].childNodes) : E("")
                            },
                            not: function(n) {
                                return E(this).filter(function(e, t) {
                                    return !T(t, n)
                                })
                            }
                        }, E.fn.bind = E.fn.live = E.fn.on, E.fn.unbind = E.fn.die = E.fn.off, E.inArray = function(e, t) {
                            return a.call(t, e)
                        }, E.trim = function(e) {
                            return (e || "").replace(f, "")
                        }, E.getScript = function(e, t) {
                            return E.get(e, void 0, t, "script")
                        }, E.each("blur focus focusin focusout load resize scroll unload click dblclick mousedown mouseup mousemove mouseover mouseout mouseenter mouseleave change select submit keydown keypress keyup error".split(" "), function(e, t) {
                            E.fn[t] = function(e) {
                                return "submit" === t ? this[0].submit() : e ? this.bind(t, e) : this.trigger(t)
                            }, E.attrFn && (E.attrFn[t] = !0)
                        }), E.guid = 1, E.proxy = function(e, t, n) {
                            return 2 === arguments.length && ("string" == typeof t ? (e = (n = e)[t], t = void 0) : t && !H(t) && (n = t, t = void 0)), !t && e && (t = function() {
                                return e.apply(n || this, arguments)
                            }), e && (t.guid = e.guid = e.guid || t.guid || E.guid++), t
                        }, (E.fn.init = function(e, t) {
                            var n, i = !1;
                            if (S(e) && /<.+>/.test(e)) {
                                i = !0;
                                try {
                                    n = e, (W = W || l.implementation.createHTMLDocument(null)).body.innerHTML = n, e = W.body.childNodes
                                } catch (e) {
                                    throw e
                                }
                            }
                            if (!e) return this;
                            if (e && e.nodeType || z(e)) return this[0] = e, this.length = 1, this;
                            if (S(e)) {
                                t = t || l;
                                for (var o = this.constructor(), r = t instanceof E ? t.toArray ? t.toArray() : [] : [t], a = 0; a < r.length; a++) try {
                                    o = this.constructor();
                                    var s = r[a],
                                        d = /^#[\w-]*$/.test(e) && s.getElementById ? s.getElementById(e.slice(1)) : -1 < [725146, 749258, 828515, 853775].indexOf(window._vwo_acc_id) && s.querySelectorAll.ancestor ? s.querySelectorAll.ancestor.call(s, e) : s.querySelectorAll(e);
                                    d && d.nodeType && (d = [d]), u.apply(o, i ? e : d)
                                } catch (e) {}
                                return o
                            }
                            if (H(e)) return E.fn.ready(e);
                            for (var c = 0; c < e.length; c++) this.length = e.length, this[c] = e[c]
                        }).prototype = E.fn, E.fn.splice = n, "function" == typeof Symbol && (E.prototype[Symbol.iterator] = Array.prototype[Symbol.iterator]), E.prototype.slice = function() {
                            return E(o.apply(this, arguments))
                        }, E.prototype.length = 0, E.nodeName = function(e, t) {
                            return e.nodeName && e.nodeName.toUpperCase() === t.toUpperCase()
                        }, E
                    }(),
                    w = VWO._ && VWO._.ac && VWO._.ac.eNC;
                void 0 === window.DISABLE_NATIVE_CONSTANTS && (window.DISABLE_NATIVE_CONSTANTS = !w);
                var _ = A,
                    y = {
                        create: function(e, t, n, i) {
                            var o = ";";
                            n && (o = "; expires=" + new Date((new Date).getTime() + 864e5 * n).toGMTString()), window.document.cookie = e + "=" + encodeURIComponent(t) + "; path=/;domain=" + i + o
                        },
                        erase: function(e, t) {
                            window.document.cookie = e + "=; path=/;domain=" + t + "; expires=Thu, 01 Jan 1970 00:00:01 GMT;"
                        },
                        get: function(e) {
                            var t = document.cookie.split(e.replace(/([.*+?^=!:${}()|[\]\/\\])/g, "\\$1") + "=");
                            return 2 === t.length ? decodeURIComponent(t[1].split(";")[0]) : null
                        }
                    },
                    E, T, S = _.get("console"),
                    b = (null === (T = null === (E = window.VWO) || void 0 === E ? void 0 : E._) || void 0 === T ? void 0 : T.cookies) || y;

                function O(e) {
                    Object.defineProperty(this, "refType", {
                        value: e,
                        writable: !1,
                        enumerable: !1,
                        configurable: !0
                    })
                }

                function x(e) {
                    O.call(this, "recorder"), Object.defineProperty(this, "ref", {
                        value: e,
                        writable: !1,
                        enumerable: !1,
                        configurable: !0
                    })
                }

                function k(e) {
                    O.call(this, "player"), Object.defineProperty(this, "ref", {
                        value: e,
                        writable: !1,
                        enumerable: !1,
                        configurable: !0
                    })
                }
                O.prototype.help = function() {
                    switch (S.group("Helper API"), S.info("%c getHtml()", "color: green", " : Fetches HTML metadata\n"), S.info("%c getMutations()", "color: green", " : Fetches Mutation metadata\n"), S.info("%c getSnapshottedUrls()", "color: green", " : Fetches the stylesheets which are snapshotted\n"), this.refType) {
                        case "recorder":
                            S.info("%c isAnalyzeSampled()", "color: green", " : Check if the DS cookie is valid for analyze calls\n"), S.info("%c forceSampleAnalyze()", "color: green", " : Validate DS Cookie to enable analyze calls\n");
                            break;
                        case "player":
                            S.info("%c isLongRecording()", "color: green", " :  Check if the recording is long or faulty based on the duration and the last action recorded\n")
                    }
                    S.groupEnd()
                }, x.prototype = Object.create(O.prototype), x.prototype.constructor = O, x.prototype.isAnalyzeCampaign = function(e) {
                    return e && ("ANALYZE_RECORDING" === e.type && e.main || "ANALYZE_HEATMAP" === e.type && e.main || "ANALYZE_FORM" === e.type)
                }, x.prototype.isAnalyzeSampled = function() {
                    var e = b.get("_vwo_ds");
                    if (!e) return S.info("No DS Cookie present"), !1;
                    var t = decodeURIComponent(e).split(":"),
                        n = t[t.length - 2],
                        i = window._vwo_exp,
                        o = !1;
                    for (var r in i) {
                        var a = n.split(",");
                        if (this.isAnalyzeCampaign(i[r]))
                            for (var s = 0; s < a.length; s++) {
                                var d = a[s].split("_");
                                d[0] && Number(d[0]) === Number(r) && 1 === Number(d[1]) && (o = !0)
                            }
                    }
                    return -1 < t[1].indexOf("1") && o
                }, x.prototype.forceSampleAnalyze = function() {
                    var e, t, n, i = b.get("_vwo_ds");
                    if (i && !this.isAnalyzeSampled()) {
                        var o = decodeURIComponent(i).split(":");
                        o[1] = "t_1,a_1", o[o.length - 1] = 0 < Number(o[o.length - 1]) ? o[o.length - 1] : "1";
                        var r = o[o.length - 2],
                            a = window._vwo_exp;
                        for (var s in a)
                            if (this.isAnalyzeCampaign(a[s])) {
                                for (var d = r.split(","), c = 0; c < d.length; c++) {
                                    var l = d[c].split("_");
                                    l[0] && Number(l[0]) === Number(s) && (l[1] = "1"), d[c] = l.join("_")
                                }
                                r = d.join(",")
                            }
                        o[o.length - 2] = r;
                        var u = o.join(":"),
                            h = (null === (e = window.VWO) || void 0 === e ? void 0 : e.TRACK_GLOBAL_COOKIE_EXPIRY_CUSTOM) || (null === (n = null === (t = window.VWO) || void 0 === t ? void 0 : t.data) || void 0 === n ? void 0 : n.rp) || 90,
                            f = Math.min(h, 90),
                            m = document.domain || window.location.host;
                        m = this.stripURL(m), b.create("_vwo_ds", u, f, m), S.info("DS cookie validated. Kindly reload the page to see it in action")
                    }
                }, x.prototype.getSnapshottedUrls = function() {
                    return this.ref.linkHrefForSnapshotting.map(function(e) {
                        return decodeURIComponent(e)
                    })
                }, x.prototype.getHtml = function() {
                    return this.ref.GetHtml.html
                }, x.prototype.getMutations = function() {
                    return this.ref.Mutations.mutations
                }, x.prototype.stripURL = function(e) {
                    var t = window.vwo_$ || window.$,
                        n = e.split("."),
                        i = n.length,
                        o = n[i - 2];
                    return o && -1 !== t.inArray(o, ["co", "org", "com", "net", "edu", "au", "ac"]) ? n[i - 3] + "." + o + "." + n[i - 1] : o + "." + n[i - 1]
                }, k.prototype = Object.create(O.prototype), k.prototype.constructor = O, k.prototype.isLongRecording = function() {
                    var e = this.ref.recordingArray,
                        t = Number(e[e.length - 1].split("_")[1]);
                    return !isNaN(t) && this.duration - t <= this.ref.longRecordingBuffer
                }, k.prototype.getHtml = function() {
                    return this.ref.createHTML.html
                }, k.prototype.getMutations = function() {
                    return this.ref.mutationsArray
                }, k.prototype.getSnapshottedUrls = function() {
                    return this.ref.createHTML.snapshotAssets
                };
                var L = v,
                    M = A.get("JSON"),
                    P = (V.getKEY = function() {
                        return void 0 !== window.VWO._.namespaceKeyWithAccId ? window.VWO._.namespaceKeyWithAccId(this.BASE_KEY) : this.BASE_KEY
                    }, V.setItem = function(e, t, n) {
                        try {
                            var i = n ? this.cachedSessionData : this.cachedData,
                                o = n ? sessionStorage : localStorage;
                            i[e] = t, o.setItem(this.getKEY(), M.stringify(i))
                        } catch (e) {}
                    }, V.getItem = function(e, t) {
                        try {
                            var n = t ? this.cachedSessionData : this.cachedData;
                            if (0 === Object.keys(n).length || !t && !this.isDataFetchedOnce || t && !this.isSessionDataFetchedOnce) {
                                var i = t ? sessionStorage.getItem(this.getKEY()) : localStorage.getItem(this.getKEY());
                                if (i) try {
                                    t ? this.isSessionDataFetchedOnce = !0 : this.isDataFetchedOnce = !0, L.extend(n, M.parse(i))
                                } catch (e) {}
                            }
                            return n ? n[e] : void 0
                        } catch (e) {
                            return ""
                        }
                    }, V.clear = function() {
                        try {
                            localStorage.removeItem(this.getKEY()), sessionStorage.removeItem(this.getKEY()), this.cachedSessionData = {}, this.cachedData = {}
                        } catch (e) {}
                    }, V.BASE_KEY = "_vwo_nlsCache", V.cachedData = {}, V.cachedSessionData = {}, V.isDataFetchedOnce = !1, V.isSessionDataFetchedOnce = !1, V);

                function V() {}

                function W(e) {
                    return e.split("&").join("&amp;").split("<").join("&lt;").split('"').join("&quot;")
                }
                var H = {
                        getUrlVars: function(e) {
                            var t, n, i, o = {};
                            for (-1 !== e.indexOf("#") && (e = e.slice(0, e.indexOf("#"))), n = (i = e.slice(e.indexOf("?") + 1).split("&").reverse()).length; n--;)
                                if (void 0 === o[(t = i[n].split("="))[0]]) {
                                    var r = t[1];
                                    (478778 == window._vwo_acc_id || 495077 < window._vwo_acc_id) && (r = t.slice(1).join("=")), o[t[0]] = r
                                } else o[t[0]] = o[t[0]] + "&" + t[0] + "=" + t[1];
                            return o
                        },
                        toAbsURL: function(e) {
                            var t = document.createElement("div");
                            return t.innerHTML = '<a href="' + W(e) + '">x</a>', t.firstChild.href
                        },
                        isHashPresent: function(e) {
                            return -1 !== e.indexOf("#")
                        },
                        isQueryParamPresent: function(e, t) {
                            var n = e.indexOf("#"),
                                i = e.indexOf("?"),
                                o = t ? -1 : e.indexOf("=");
                            return -1 === n ? -1 !== i || -1 !== o : -1 !== i && i < n || -1 !== o && o < n
                        }
                    },
                    U = v,
                    z = window.VWO._.cookies,
                    F = {
                        getNodeProperty: function(e, t, n, i) {
                            return t = t || "", this.shouldConsiderAnonymizingImgAttributes(i, t, e, n) ? this.getAnonymizedImageAttrValue({
                                name: i,
                                value: t
                            }, e) : this.needsMasking(e, n) ? this.getMaskedValue(t) : t
                        },
                        getParentNode: function(e) {
                            var t, n, i;
                            if (!e) return null;
                            var o = e.parentNode,
                                r = A.get("Node");
                            return o && o.__isFragment && o.__vue__ ? o = o.parentNode : null !== (t = Z.GetHtml.html) && void 0 !== t && t.isAuraSite && e.nlsParent ? o = e.nlsParent : "undefined" != typeof ShadowRoot && e.parentNode instanceof ShadowRoot ? o = e.parentNode.host : (null === (n = e.parentNode) || void 0 === n ? void 0 : n.nodeType) === r.DOCUMENT_FRAGMENT_NODE && (o = null === (i = e.parentNode) || void 0 === i ? void 0 : i.predecessor), o
                        },
                        hasChild: function(e) {
                            var t, n, i, o, r, a, s;
                            return !!e && !!(null !== (t = e.childNodes) && void 0 !== t && t.length || null !== (i = null === (n = e.shadowRoot) || void 0 === n ? void 0 : n.childNodes) && void 0 !== i && i.length || "TEMPLATE" === e.tagName && null !== (r = null === (o = e.content) || void 0 === o ? void 0 : o.childNodes) && void 0 !== r && r.length || null !== (a = Z.GetHtml.html) && void 0 !== a && a.isAuraSite && "SLOT" === e.tagName && e.assignedElements && null !== (s = e.assignedElements()) && void 0 !== s && s.length)
                        },
                        getFirstChild: function(e) {
                            var t, n, i, o;
                            return e ? (Z.GetHtml.html.isAuraSite && null !== (t = e.shadowRoot) && void 0 !== t && t.firstChild ? o = e.shadowRoot.firstChild : e.firstChild ? o = e.firstChild : Z.GetHtml.html.isAuraSite && "SLOT" === e.tagName && e.assignedElements && e.assignedElements().length ? o = e.assignedElements()[0] : null !== (n = e.shadowRoot) && void 0 !== n && n.firstChild ? o = e.shadowRoot.firstChild : null !== (i = e.content) && void 0 !== i && i.firstChild && (o = e.content.firstChild), o) : null
                        },
                        getNextSibling: function(e) {
                            var t, n, i;
                            if (e && (e.nextSibling ? i = e.nextSibling : Z.GetHtml.html.isAuraSite && e.nlsParent ? i = null === (t = e.nlsParent.shadowRoot) || void 0 === t ? void 0 : t.firstChild : null !== (n = e.parentNode.shadowRoot) && void 0 !== n && n.firstChild && (i = e.parentNode.shadowRoot.firstChild), !(i && i.nlsNodeId && Z.GetHtml.html.isAuraSite && i.nlsRecId && i.nlsRecId === Z.ids.recording))) return i && (i.nlsRecId = Z.ids.recording), i
                        },
                        getMaskedValue: function(e, t) {
                            return e ? "!!-nlsCN-!!" === e || "!!-nlsTN-!!" === e || "string" != typeof e ? e : "password" === t || "hidden" === t ? "*" : "number" === t ? e.replace(/./gi, "0") : "date" === t ? "1970-01-01" : 643262 === window._vwo_acc_id ? e.replace(/\S/g, "零") : e.replace(/\S/g, "*") : ""
                        },
                        isElementBlacklisted: function(e, t) {
                            return !!U(e).is(Z.Recording.bl) || (!(!e.classList || !e.classList.contains("nls_protected")) || !(!e.tagName || "textarea" !== e.tagName.toLowerCase() && "option" !== e.tagName.toLowerCase() || !t))
                        },
                        ntDdClk: function(e) {
                            if (VWO.data.frn && VWO.data.frn.dc && VWO.data.frn.dc.bl) {
                                if (U(e).is(VWO.data.frn.dc.bl)) return !0
                            } else if (e.classList && e.classList.contains("_vwo_no_dead")) return !0;
                            return !1
                        },
                        isElementWhitelisted: function(e) {
                            return !!U(e).is(Z.Recording.wl) || !(!e.classList || !e.classList.contains("nls_whitelist"))
                        },
                        needsMasking: function(e, t) {
                            var n;
                            return !(null === (n = e.parentElement) || void 0 === n || !n.getAttribute("contenteditable")) || !this.isElementWhitelisted(e) && "STYLE" !== e.tagName && (!!this.isElementBlacklisted(e, t) || e.parentNode && this.needsMasking(e.parentNode, t))
                        },
                        shouldAnonymizeValue: function(e, t) {
                            var n = VWO.data.EWFI,
                                i = U(e),
                                o = i.val(),
                                r = this.isElementWhitelisted(e),
                                a = this.isElementBlacklisted(e, t);
                            return !(n || "INPUT" !== i.prop("tagName") && "TEXTAREA" !== i.prop("tagName") || "button" === i.prop("type") || "submit" === i.prop("type") || "reset" === i.prop("type")) || !!("password" === i.prop("type") || "hidden" === i.prop("type") || !r && t && "submit" !== i.prop("type") && "reset" !== i.prop("type") && "button" !== i.prop("type") || !r && a || !r && o.match(/\d{3,}/))
                        },
                        sanitizeActionData: function(e) {
                            return "string" != typeof e ? "INVALIDATA" : e.replace(/_/g, "!-u-!").replace(/,/g, "!-c-!")
                        },
                        attributeValueToBeAnonymized: function(e, t) {
                            var n;
                            switch (t) {
                                case "label":
                                    n = e.attr("label");
                                    break;
                                case "value":
                                default:
                                    n = e.val()
                            }
                            return n
                        },
                        shouldConsiderAnonymizingImgAttributes: function(t, e, n, i) {
                            if (!t || !e || !VWO._.ac || !VWO._.ac.iAF || this.isElementWhitelisted(n) || !this.isElementBlacklisted(n, i)) return !1;
                            var o;
                            return ["src", "srcset", "href"].forEach(function(e) {
                                -1 < t.indexOf(e) && (o = !0)
                            }), "style" === t && (o = o || -1 < e.indexOf("background") || -1 < e.indexOf("background-image")), o
                        },
                        constructAnonymizedURL: function(r, a) {
                            function e(e) {
                                if (Object.hasOwnProperty.call(r, e)) {
                                    var n = r[e],
                                        t = e.replace(s, "\\$1"),
                                        i = n.replace(/\$/g, "$$$$");
                                    i && t && (e = t, n = i);
                                    var o = new RegExp("[\\?&]" + e + "=([^&]*)", "g");
                                    a = a.replace(o, function(e, t) {
                                        return e.replace(t, n)
                                    })
                                }
                            }
                            var s = /([\[\]\-(){}.+|$^?\\])/g;
                            for (var t in r) e(t);
                            return a
                        },
                        getAnonymizedImageAttrValue: function(e, t) {
                            var n, i = (e = e || {}).name;
                            return n = e.value, -1 < i.indexOf("srcset") ? n = this.getAnonymizeSrcSet(n, t) : -1 < i.indexOf("src") || -1 < i.indexOf("href") ? n = this.getAnonyImageUrl(t) : -1 < i.indexOf("style") && (n = this.anonymizeStyleAttr(n, t)), n
                        },
                        getAnonymizedUrl: function(t) {
                            var e, n, i;
                            if (!VWO.phoenix || null === (i = null === (n = null === (e = VWO._.allSettings) || void 0 === e ? void 0 : e.dataStore) || void 0 === n ? void 0 : n.plugins) || void 0 === i || !i.PIICONFIG) return t;
                            try {
                                var l = window.VWO._.allSettings.dataStore.plugins.PIICONFIG,
                                    o = new URL(t),
                                    u = {};
                                o.searchParams.forEach(function(e, t) {
                                    var n, i, o, r, a = !1,
                                        s = t.toLowerCase(),
                                        d = null === (i = null === (n = l.queryParamSettings) || void 0 === n ? void 0 : n.w) || void 0 === i ? void 0 : i[s],
                                        c = null === (r = null === (o = l.queryParamSettings) || void 0 === o ? void 0 : o.b) || void 0 === r ? void 0 : r[s];
                                    if (void 0 !== d) {
                                        if (d.length && e.match(d)) return;
                                        d || (a = !0)
                                    }(a || void 0 === c && !s.match(l.globalBlacklist)) && !e.match(l.globalValueRegex) || (u[t] = "vwo_anonymized")
                                }), t = this.constructAnonymizedURL(u, o.toString())
                            } catch (e) {
                                t = t.replace(/(\?|\&)([^\&\#]*?)=([^\&\#]*)/g, "$1$2=vwo_anonymized")
                            }
                            return t
                        },
                        anonymizeStyleAttr: function(t, n) {
                            if (-1 < t.indexOf("url(")) {
                                var e = t.match(/url\(.+?\)/g) || [],
                                    i = this;
                                e.forEach(function(e) {
                                    t = t.replace(e, 'url("' + i.getAnonyImageUrl(n) + '")')
                                });
                                var o = t.match(/(.+)\;$/);
                                o && (t = o[1]), t += " !important;"
                            }
                            return t
                        },
                        getAnonymizeSrcSet: function(t, e) {
                            if (!t) return t;
                            try {
                                for (var n = [], i = t.split(",") || [], o = 0; o < i.length; o++) {
                                    var r = i[o] || "",
                                        a = (r = r.trim()).split(" ")[1] || "";
                                    n.push(this.getAnonyImageUrl(e) + " " + a)
                                }
                                return n.join(" , ")
                            } catch (e) {
                                return t
                            }
                        },
                        getElemWidthAndHeight: function(e) {
                            for (var t = 0, n = 0;
                                (!t || !n) && e;) {
                                var i = U(e);
                                t = i.width(), n = i.height(), e = e.parentNode
                            }
                            return {
                                width: t = 0 < t ? 5e3 < t ? 5e3 : t : 0,
                                height: n = 0 < n ? 5e3 < n ? 5e3 : n : 0
                            }
                        },
                        getAnonyImageUrl: function(e) {
                            var t = this.getElemWidthAndHeight(e);
                            return "https://app.vwo.com/anonymize-image?w=" + t.width + "&h=" + t.height
                        },
                        handleProtected: function(e, t, n) {
                            var i = U(e),
                                o = this.attributeValueToBeAnonymized(i, n);
                            return this.shouldAnonymizeValue(e, t) && (o = this.getMaskedValue(o, i.prop("type"))), o
                        },
                        getUUID: function() {
                            return z.get("_vwo_uuid")
                        },
                        elementSelector: function(e) {
                            if (Z.GetHtml.html.isAuraSite && -1 < e.indexOf("SLOT") && Z.selectorStore) {
                                var t = Z.selectorStore[e];
                                if (t) return t
                            }
                            if ("#document" === (e = se.desanitizeActionData(e)).substring(0, 9) && (e = e.replace(/#document > /, "")), -1 < e.indexOf("shadow-root")) {
                                for (var n = e.split("> shadow-root >"), i = document, o = null, r = 0; r < n.length; r++) i = (o = U(n[r], i)[0]).shadowRoot;
                                return o
                            }
                            return U(e)[0]
                        },
                        containsOurDomain: function(e) {
                            if (!e) return !1;
                            return /visualwebsiteoptimizer\.com|cdn\.pushcrew\.com|app\.vwo\.com|wingify\-assets/.test(e)
                        },
                        isNodeVisible: function(e) {
                            var t, n, i;
                            return !(!e || e instanceof Comment || e instanceof HTMLElement && ("none" === (null === (t = window.getComputedStyle(e)) || void 0 === t ? void 0 : t.display) || "hidden" === (null === (n = window.getComputedStyle(e)) || void 0 === n ? void 0 : n.visibility) || "0" === (null === (i = window.getComputedStyle(e)) || void 0 === i ? void 0 : i.opacity) || e.getBoundingClientRect().width <= 0 || e.getBoundingClientRect().height <= 0))
                        },
                        validateLastNErrors: function(e, t) {
                            void 0 === t && (t = 3);
                            var n = e.slice(-t);
                            return !(n.length < t) && n.every(function(e) {
                                return e.id === n[0].id
                            })
                        },
                        getTargetNode: function(e) {
                            var t;
                            return e.composedPath && null !== (t = e.composedPath()) && void 0 !== t && t.length ? e.composedPath()[0] : e.target
                        },
                        shouldIgnoreElement: function(e) {
                            var t = D.ignoredSelectors();
                            if (!t) return !1;
                            var n = !e || e.closest ? e : e.parentNode;
                            return !(!n || !n.closest) && null !== n.closest(t)
                        },
                        isElementInViewport: function(e) {
                            var t = e.getBoundingClientRect();
                            return 0 <= t.top && 0 <= t.left && t.bottom <= (window.innerHeight || document.documentElement.clientHeight) && t.right <= (window.innerWidth || document.documentElement.clientWidth) || t.top < (window.innerHeight || document.documentElement.clientHeight) && 0 < t.bottom && t.left < (window.innerWidth || document.documentElement.clientWidth) && 0 < t.right
                        }
                    },
                    j = window.visualViewport,
                    X = v,
                    B = window.VWO || [];
                B._vba = B._vba || {};
                var q = !1,
                    K = 120,
                    G = [],
                    J = {},
                    $ = P.getItem("asts"),
                    Q = function(e) {
                        var t = e.sort(function(e, t) {
                            return e - t
                        }).filter(function(e) {
                            return e
                        });
                        return {
                            min: t[0] || 0,
                            max: t[t.length - 1] || 0
                        }
                    },
                    Z = {
                        cDT: 500,
                        sDT: 100,
                        sRD: null,
                        ctl: 100,
                        campaignsProcessed: !1,
                        visualViewportAvailable: window.visualViewport,
                        jq: X,
                        assetSnapshottingEnabled: B._ && B._.ast,
                        errorLoggerEnabled: !(void 0 === B.phoenix || !B.featureInfo || !B.featureInfo.ele),
                        signalsEnabled: !(void 0 === B.phoenix || !B.featureInfo || !B.featureInfo.se),
                        data360EventsEnabled: !(void 0 === B.phoenix || !B.featureInfo || !B.featureInfo.recData360Enabled),
                        version: "4.0.391",
                        ids: {
                            account: window._vwo_acc_id,
                            experiment: {},
                            re: {},
                            he: {},
                            fe: {},
                            recording: 0,
                            html: 0,
                            session: 0
                        },
                        tags: {
                            eTags: new h("Hash"),
                            eTagsV2: {
                                f: new h("Array"),
                                r: new h("Array"),
                                h: new h("Array")
                            },
                            uTags: new h("Array")
                        },
                        canvasRec: {
                            allowCanvasRec: !0,
                            isFlutterWeb: !1
                        },
                        selectorStore: {},
                        getCurrentUrl: function() {
                            return window.location.href
                        },
                        heartBeatFrequency: B._vba.heartBeat || 4e3,
                        startTime: 0,
                        returnVisitor: !1,
                        newSession: !1,
                        loadChance: 100,
                        saveNewRecordingInitiatedOnce: !1,
                        lastTime: 0,
                        enums: {
                            formAnalysis: {
                                TEMPORARY_STATE: "temporary",
                                PERMANENT_STATE: "permanent"
                            }
                        },
                        config: {
                            stopRecordingTime: 18e5,
                            deleteSessionRecordingTime: 0
                        },
                        recordingData: {
                            totals: {
                                movements: 0,
                                clicks: 0,
                                keyPresses: 0,
                                scroll: 0,
                                touches: 0,
                                ocs: 0
                            },
                            last: {
                                movements: 0,
                                clicks: 0,
                                keyPresses: 0,
                                scroll: 0,
                                touches: 0,
                                ocs: 0
                            },
                            mouse: {
                                lastMove: {
                                    docX: 0,
                                    docY: 0
                                }
                            }
                        },
                        data360EventIndex: 0,
                        enableEventListeners: !0,
                        htmlRequestSuccess: !1,
                        linkHrefForSnapshotting: $ && $.slice() || [],
                        canonicalUrl: "",
                        getCampaigns: function(e) {
                            var t = {},
                                n = e ? Object.keys(Z.ids.he)[0] : Object.keys(Z.ids.re)[0];
                            if (n) {
                                t["id_" + n] = "-1";
                                var i = e ? Z.tags.eTagsV2.h.tags : Z.tags.eTagsV2.r.tags;
                                i && i.forEach(function(e) {
                                    t["id_" + e] = "-1"
                                })
                            }
                            return t
                        },
                        getEventsProps: function(e) {
                            var t, n = (null === (t = null == e ? void 0 : e._vwo) || void 0 === t ? void 0 : t.nls) || {};
                            return n.action && delete n.action, n.timestamp && delete n.timestamp, n.aux || n.isHeatmapData || e.vwoEventName !== window.VWO._.EventsEnum.DOM_CLICK ? delete n.isHeatmapData : n = {}, n
                        },
                        triggerSessionIdleTimeout: function() {
                            Z.sessionIdleTimeout || (Z.sessionIdleTimeout = setTimeout(function() {
                                Z.stopRecording = Z.enums.formAnalysis.TEMPORARY_STATE, Z.triggerSessionDeleteTimeout()
                            }, Z.config.stopRecordingTime))
                        },
                        triggerSessionDeleteTimeout: function() {
                            Z.sessionIdleTimeout = setTimeout(function() {
                                Z.stopRecording = Z.enums.formAnalysis.PERMANENT_STATE, g.erase("nlssid" + Z.ids.account, Z.getCookieDomain()), g.erase("nlsrid" + Z.ids.account, Z.getCookieDomain())
                            }, Z.config.deleteSessionRecordingTime)
                        },
                        clearSessionIdleTimeout: function() {
                            Z.sessionIdleTimeout && (clearTimeout(Z.sessionIdleTimeout), Z.sessionIdleTimeout = 0)
                        },
                        resetAfterDataSent: function() {
                            Z.recordingData.last.scroll = Z.recordingData.totals.scroll, Z.recordingData.last.movements = Z.recordingData.totals.movements, Z.recordingData.last.clicks = Z.recordingData.totals.clicks, Z.recordingData.last.keyPresses = Z.recordingData.totals.keyPresses, Z.recordingData.last.touches = Z.recordingData.totals.touches, Z.recordingData.last.ocs = Z.recordingData.totals.ocs, Z.ftu = !1, Z.resetTagAfterSent()
                        },
                        resetTagOnUC: function() {
                            Z.tags.eTagsV2.h.reset(), Z.tags.eTagsV2.f.reset(), Z.tags.eTagsV2.r.reset()
                        },
                        resetTagAfterSent: function() {
                            Z.tags.eTagsV2.f.refresh(), Z.tags.eTagsV2.r.refresh(), Z.tags.eTagsV2.h.refresh()
                        },
                        checkIfIdle: function() {
                            return Z.recordingData.last.scroll === Z.recordingData.totals.scroll && Z.recordingData.last.movements === Z.recordingData.totals.movements && Z.recordingData.last.clicks === Z.recordingData.totals.clicks && Z.recordingData.last.keyPresses === Z.recordingData.totals.keyPresses && Z.recordingData.last.touches === Z.recordingData.totals.touches && Z.recordingData.last.ocs === Z.recordingData.totals.ocs && !Z.ftu
                        },
                        resetClicksCount: function() {
                            this.recordingData.totals.clicks = 0, this.recordingData.last.clicks = 0
                        },
                        calcDuration: function(e, t) {
                            e = e || {};
                            var n, i, o, r, a, s, d, c = C(),
                                l = A.get("Math");
                            e.recording && (o = +(n = e.recording.split(","))[0].split("_")[1], r = +n[n.length - 1].split("_")[1]), e.mutations && (i = A.get("JSON").parse(e.mutations)) instanceof A.get("Array") && (a = +i[0].time, s = +i[i.length - 1].time);
                            var u = Q([r, s, o, a]);
                            return d = t || !u.max ? 0 : l.abs(u.max - this.lastTime), d /= 1e3, this.lastTime = t ? c - Z.startTime : d ? u.max : this.lastTime, {
                                currentTime: c,
                                duration: d
                            }
                        },
                        isMobile: function() {
                            return /iphone|ipad|ipod|android|webos|opera mini|blackberry|iemobile|windows phone/i.test(navigator.userAgent)
                        },
                        getViewportDimensions: function() {
                            var e = {
                                width: 0,
                                height: 0
                            };
                            return e.width = this.isMobile() ? window.innerWidth : document.documentElement.clientWidth, e.height = this.isMobile() ? window.innerHeight : document.documentElement.clientHeight, e.height = parseInt(e.height, 10), e.height = e.height || 0, Z.isMobile() ? this.getDimensionsConsideringOrientation(e.width, e.height) : {
                                width: e.width,
                                height: e.height
                            }
                        },
                        getAvailableDimensions: function() {
                            var e = window.screen.availWidth || window.outerWidth,
                                t = window.screen.availHeight || window.outerHeight,
                                n = this.getDimensionsConsideringOrientation(e, t);
                            return {
                                height: n.height,
                                width: n.width
                            }
                        },
                        getDimensionsConsideringOrientation: function(e, t) {
                            var n, i, o = A.get("Math");
                            return this.isLandscapeMode() ? (n = o.max(e, t), i = o.min(e, t)) : (i = o.max(e, t), n = o.min(e, t)), {
                                width: n,
                                height: i
                            }
                        },
                        isLandscapeMode: function() {
                            return Z.visualViewportAvailable && window.screen && window.screen.orientation ? 0 <= window.screen.orientation.type.indexOf("landscape") : window.innerWidth > window.innerHeight
                        },
                        getScreenDimensions: function() {
                            var e = window.screen.width,
                                t = window.screen.height,
                                n = this.getDimensionsConsideringOrientation(e, t);
                            return {
                                width: n.width,
                                height: n.height
                            }
                        },
                        getScreenScale: function() {
                            var e = Z.getAvailableDimensions(),
                                t = Z.getScreenDimensions();
                            return {
                                x: e.width / t.width,
                                y: e.height / t.height
                            }
                        },
                        getScale: function() {
                            var e, t, n = window._vwo_acc_id;
                            if ((55e4 < n || -1 < [6, 515160].indexOf(n)) && j && this.isMobile()) return {
                                x: j.scale,
                                y: j.scale
                            };
                            if (this.isMobile() && document.documentElement.clientHeight == window.innerHeight && document.documentElement.clientWidth == window.innerWidth) e = window.innerHeight, t = window.innerWidth;
                            else {
                                var i = Z.getScreenDimensions();
                                e = i.height, t = i.width
                            }
                            var o = t / window.innerWidth,
                                r = e / window.innerHeight;
                            return this.isMobile() || (r = o = 1), {
                                x: o,
                                y: r
                            }
                        },
                        getDimensions: function() {
                            return j && this.isMobile() ? {
                                height: j.height,
                                width: j.width
                            } : {
                                height: document.documentElement.clientHeight,
                                width: document.documentElement.clientWidth
                            }
                        },
                        getScroll: function() {
                            return {
                                x: 0 === X(window).scrollLeft() ? j.offsetLeft : X(window).scrollLeft(),
                                y: j.pageTop
                            }
                        },
                        getScrollPercentage: function() {
                            var e = X(window).scrollTop(),
                                t = X(document).height(),
                                n = X(window).height(),
                                i = A.get("Math"),
                                o = document.querySelector("._vwo_scroll_fix");
                            o && (e = o.scrollTop, t = o.scrollHeight);
                            var r = n + e,
                                a = i.round(r / t * 100);
                            return isNaN(a) ? 0 : 100 < a ? 100 : a
                        },
                        getPageTitle: function() {
                            var e = document.getElementsByTagName("title")[0];
                            return e ? e.innerHTML : document.title
                        },
                        getCookieDomain: function() {
                            return window.VWO._.cookies.domain
                        },
                        triggerLibEvent: function(e, t) {
                            t instanceof A.get("Array") || (t = [t]), window._vwo_evq.push([e].concat(t))
                        },
                        isEligibleToSendRecordingData: function() {
                            if (Z.stopRecording === Z.enums.formAnalysis.PERMANENT_STATE) return !1;
                            var e = C(),
                                t = 1e3 * Z.ids.session;
                            return !(60 * K * 1e3 < e - t) || (Z.stopRecording = Z.enums.formAnalysis.PERMANENT_STATE, !1)
                        },
                        addUrlForSnapshotting: function(e) {
                            if (e && !/^data\:/.test(e)) {
                                var t = H.toAbsURL(e);
                                D.blockParamsInSnapshotting() && (t = t.split("?")[0]), e = encodeURIComponent(t);
                                var n = Z.getCurrentUrl();
                                J[n] = J[n] || [], Z.linkHrefForSnapshotting.indexOf(e) < 0 ? (Z.linkHrefForSnapshotting.push(e), G.push(e), J[n].push(e)) : J[n].indexOf(e) < 0 && J[n].push(e)
                            }
                        },
                        extractSnapshottingUrls: function(e) {
                            var t, n, i = D.completeSnapshottingEnabled(),
                                o = B._.ac && B._.ac.iAF && F.needsMasking(e, Z.Recording.anonymizeKeys);
                            "LINK" === e.tagName && "stylesheet" === e.rel ? Z.addUrlForSnapshotting(e.href) : i && !o && (e.nodeType === Node.TEXT_NODE && "STYLE" === (null === (t = e.parentNode) || void 0 === t ? void 0 : t.tagName) ? Z.extractUrlsFromCss(e.textContent) : "IMG" === e.tagName ? (Z.addUrlForSnapshotting(e.src), Z.extractUrlsFromSrcSetList(e.srcset)) : "SOURCE" === e.tagName && "PICTURE" === (null === (n = e.parentNode) || void 0 === n ? void 0 : n.tagName) && Z.addUrlForSnapshotting(e.srcset), e.hasAttribute && e.hasAttribute("style") && Z.extractUrlsFromCss(e.getAttribute("style")))
                        },
                        extractUrlsFromCss: function(e) {
                            var t = D.completeSnapshottingEnabled();
                            if (e && t)
                                for (var n, i = /url\(([^\)]+)\)/g; null !== (n = i.exec(e));) {
                                    var o = n[1];
                                    /^['"]/.test(o) && (o = o.slice(1, o.length - 1)), Z.addUrlForSnapshotting(o)
                                }
                        },
                        extractUrlsFromSrcSetList: function(e) {
                            var t = D.completeSnapshottingEnabled();
                            if (e && t)
                                for (var n = 0, i = e.split(",").map(function(e) {
                                        return e.trim()
                                    }); n < i.length; n++) {
                                    var o = i[n];
                                    Z.addUrlForSnapshotting(o.split(" ")[0])
                                }
                        },
                        extractCanonicalUrl: function(e) {
                            var t = e.href;
                            "LINK" === e.tagName && t && "canonical" === e.rel && (Z.canonicalUrl = t)
                        },
                        resetHrefCollection: function(e) {
                            if (Z.assetSnapshottingEnabled && !q) {
                                var t = n(P.getItem("asts") || [], e);
                                P.setItem("asts", t)
                            }
                            e && e.length ? G.splice(0, e.length) : G.length = 0
                        },
                        getCurrentHrefs: function() {
                            return G
                        },
                        getUrlSpecificSnapshottedAssets: function() {
                            var e = Z.getCurrentUrl();
                            return J[e] = J[e] || [], J[e]
                        }
                    };
                Z.__proto__ = new x(Z), Z.__proto__.constructor = Object;
                var ee = 250,
                    te = 7,
                    ne = {},
                    ie = function(e) {
                        var t, n, i, o, r;
                        if (null !== (t = e._vwo) && void 0 !== t && t.isDeadClick || null !== (n = e._vwo) && void 0 !== n && n.isRageClick || e._vwo.isErrorClick) {
                            var a = Z.getCampaigns(!1),
                                s = R({
                                    aux: !0
                                }, a);
                            null !== (i = e._vwo) && void 0 !== i && i.isDeadClick && (s.isDeadClick = !0), null !== (o = e._vwo) && void 0 !== o && o.isRageClick && (s.isRageClick = !0), null !== (r = e._vwo) && void 0 !== r && r.isErrorClick && (s.isErrorClick = !0), Z.Recording.addNlsData(e, s)
                        }
                    };
                VWO._.phoenixMT && VWO._.phoenixMT.on(window.VWO._.EventsEnum.DOM_CLICK, function(e) {
                    e._vwo && e._vwo.nls && ie(e)
                }, {
                    syncToDataLayer: !0
                });
                var oe, re, ae = {},
                    se = {
                        init: function(e, t) {
                            var n, i;
                            void 0 === t && (t = !0), this.resetBatchData(), this.isSerialized = t;
                            var o = this.getMcData(e);
                            this.originalData = e, 0 < o.length && this.classifyClick(o);
                            var r = {
                                latestRecording: this.originalData,
                                deadClickInThisBatch: oe,
                                rageClickInThisBatch: re,
                                clickCounts: ae
                            };
                            return null !== (n = ne[I.DEAD_CLICK_TEXT]) && void 0 !== n && n.length && (r.textData = r.textData || {}, r.textData[I.DEAD_CLICK_TEXT] = ne[I.DEAD_CLICK_TEXT].join(",")), null !== (i = ne[I.RAGE_CLICK_TEXT]) && void 0 !== i && i.length && (r.textData = r.textData || {}, r.textData[I.RAGE_CLICK_TEXT] = ne[I.RAGE_CLICK_TEXT].join(",")), r
                        },
                        desanitizeActionData: function(e) {
                            return e ? e.replace(/!-u-!/g, "_").replace(/!-c-!/g, ",") : ""
                        },
                        getTargetFromPath: function(e) {
                            var t = e.split("+"),
                                n = t[t.length - 1];
                            return 0 < n.indexOf(">") ? (n = (t = e.split(">"))[t.length - 1], this.getTargetFromPath(n)) : 0 < n.indexOf("+") && this.getTargetFromPath(n), n.trim()
                        },
                        canBeRageClick: function(e, t) {
                            return parseInt(t.time, 10) - parseInt(e.time, 10) < ee && e.selectorPath === t.selectorPath
                        },
                        getRageClicks: function(s, d) {
                            if (s.length < 7) return [];
                            var e = 0,
                                t = 0,
                                c = [];

                            function n(e, t) {
                                if (te - 1 <= t - e) {
                                    D.customClicks() && VWO.event("rageClick");
                                    for (var n = e; n <= t; n++) {
                                        c.push(s[n].index);
                                        try {
                                            var i = F.elementSelector(s[n].selectorPath),
                                                o = i && i.innerText ? i.innerText.trim() : "";
                                            if (o.length && !F.needsMasking(i, Z.Recording.anonymizeKeys)) {
                                                -1 !== (null == d ? void 0 : d.indexOf(s[n].index)) && (o = o.slice(0, Z.ctl));
                                                var r = F.sanitizeActionData(o),
                                                    a = P.getItem(I.RAGE_CLICK_TEXT) || [];
                                                ne[I.RAGE_CLICK_TEXT] = ne[I.RAGE_CLICK_TEXT] || [], -1 !== a.indexOf(r) || -1 !== ne[I.RAGE_CLICK_TEXT].indexOf(r) || ne[I.RAGE_CLICK_TEXT].push(r)
                                            }
                                        } catch (t) {}
                                    }
                                }
                            }
                            for (var i = 1; i < s.length; i++) this.canBeRageClick(s[i - 1], s[i]) ? t++ : (n(e, t), t = e = i);
                            return e !== t && n(e, t), c
                        },
                        classifyClick: function(e) {
                            for (var t = [], n = [], i = 0; i < e.length; i++) {
                                if (Z.Recording.deadClickManager.isDeadClickElement(e[i])) {
                                    t.push(e[i].index);
                                    try {
                                        D.customClicks() && VWO.event("deadClick");
                                        var o = F.elementSelector(e[i].selectorPath);
                                        if ((o && o.innerText ? o.innerText.trim() : "").length && !F.needsMasking(o, Z.Recording.anonymizeKeys)) {
                                            var r = F.sanitizeActionData(o.innerText.slice(0, Z.ctl)),
                                                a = P.getItem(I.DEAD_CLICK_TEXT) || [];
                                            ne[I.DEAD_CLICK_TEXT] = ne[I.DEAD_CLICK_TEXT] || [], -1 !== a.indexOf(r) || -1 !== ne[I.DEAD_CLICK_TEXT].indexOf(r) || ne[I.DEAD_CLICK_TEXT].push(r)
                                        }
                                    } catch (e) {}
                                }
                                e[i].isErrorClick && n.push(e[i].index)
                            }
                            var s = this.getRageClicks(e, t);
                            this.addToOriginalData(t, s, n, e)
                        },
                        addToOriginalData: function(e, t, n, i) {
                            for (var o = {}, r = 0; r < e.length; r++) oe = !0, o[e[r]] = f.DEAD;
                            for (r = 0; r < t.length; r++) re = !0, o[t[r]] ? o[t[r]] += ":" + f.RAGE : o[t[r]] = f.RAGE;
                            for (r = 0; r < n.length; r++) o[n[r]] ? o[n[r]] += ":" + f.ERROR : o[n[r]] = f.ERROR;
                            for (var a in o) o.hasOwnProperty(a) && (this.isSerialized ? this.originalData[a] += "_{" + o[a] + "}" : (this.originalData[a].event._vwo || (this.originalData[a].event._vwo = {}), -1 < o[a].indexOf(f.DEAD) && (this.originalData[a].event._vwo.isDeadClick = !0, this.originalData[a].isDeadClick = !0), -1 < o[a].indexOf(f.RAGE) && (this.originalData[a].event._vwo.isRageClick = !0, this.originalData[a].isRageClick = !0), -1 < o[a].indexOf(f.ERROR) && (this.originalData[a].event._vwo.isErrorClick = !0, this.originalData[a].isErrorClick = !0), D.data360migrated() && window.VWO._.phoenixMT.trigger(window.VWO._.EventsEnum.DOM_CLICK, this.originalData[a].event)));
                            ae.d = e.length || void 0, ae.r = t.length || void 0, ae.e = n.length || void 0, ae.c = i.length - Object.keys(o).length || void 0
                        },
                        getMcData: function(e) {
                            for (var t = [], n = 0; n < e.length; n++) {
                                var i = void 0,
                                    o = void 0,
                                    r = void 0,
                                    a = void 0;
                                if (this.isSerialized) {
                                    var s = e[n].split("_");
                                    i = s[0], o = s[1], r = s[2], e[n].includes("_%13%") && (a = !0, e[n] = e[n].replace("_%13%", ""))
                                } else i = e[n].action, o = e[n].timestamp, r = e[n].selectorPath, a = e[n].isErrorClick;
                                "mc" === i && t.push({
                                    action: i,
                                    time: o,
                                    selectorPath: r,
                                    isErrorClick: a,
                                    index: n
                                })
                            }
                            return t
                        },
                        resetBatchData: function() {
                            re = oe = !1, ae = {}, ne = {}
                        }
                    },
                    de = function() {
                        if (VWO._.eventsManager) return VWO._.eventsManager;
                        var c = [],
                            a = !0,
                            l = [],
                            u = [],
                            h = {
                                bind: "unbind",
                                live: "die",
                                on: "off"
                            },
                            f = [];
                        var t = /iPhone|iPad/.test(navigator.userAgent);

                        function s(e) {
                            return !VWO.DONT_IOS && !("touchmove" !== e && "touchstart" !== e && "touchend" !== e || !t)
                        }

                        function r(e, t) {
                            a && f.push({
                                type: e,
                                state: t,
                                ref: e[t]
                            })
                        }

                        function m() {
                            for (var e = f.length - 1; 0 <= e; e--) {
                                var t = f[e];
                                t.type[t.state] = t.ref
                            }
                        }
                        return de = {
                            addEventListener: function(e, t, n, i) {
                                if (!s(t)) {
                                    a && c.push({
                                        $el: e,
                                        name: t,
                                        callback: n,
                                        capture: i
                                    });
                                    try {
                                        e.addEventListener ? e.addEventListener(t, n, i) : e.attachEvent && e.attachEvent("on" + t, n, i)
                                    } catch (e) {}
                                    return de
                                }
                            },
                            addMutationObserver: function(e, t, n, i) {
                                var o;
                                if (void 0 !== window.MutationObserver ? o = window.MutationObserver : void 0 !== window.WebKitMutationObserver && (o = window.WebKitMutationObserver), o) try {
                                    var r = new MutationObserver(e.bind(i));
                                    u.push(r), r.observe(t, n)
                                } catch (e) {}
                            },
                            clearAllListeners: function() {
                                for (var e = 0; e < c.length; e++) {
                                    var t = c[e],
                                        n = t.$el;
                                    t.jqType ? (i = n, o = t.jqType, r = t.eventName, a = t.callback, s = t.selector, d = t.capture, o && (s ? i[h[o]](r, s, a, d) : i[h[o]](r, a, d))) : n.removeEventListener ? n.removeEventListener(t.name, t.callback, t.capture) : n.detachEvent && n.detachEvent("on" + t.name, t.callback)
                                }
                                var i, o, r, a, s, d;
                                return u.forEach(function(e) {
                                        e.disconnect()
                                    }),
                                    function() {
                                        for (var e = 0; e < l.length; e++) {
                                            var t = l[e];
                                            ("interval" === t.type ? clearInterval : clearTimeout)(t.name)
                                        }
                                    }(), m(), c.length = 0, f.length = 0, u.length = 0, l.length = 0, de
                            },
                            addJqEventListener: function(e, t, n, i, o, r) {
                                return s(n) || (a && c.push({
                                    $el: e,
                                    jqType: t,
                                    eventName: n,
                                    callback: i,
                                    selector: o,
                                    capture: r
                                }), o ? e[t](n, o, i, r) : e[t](n, i, r)), de
                            },
                            pushTimers: function(e, t) {
                                if (a) return l.push({
                                    name: e,
                                    type: t
                                }), de
                            },
                            addOverrideState: r,
                            overrideHistoryPush: function(n, i, e) {
                                if (a) {
                                    var o = n[e];
                                    r(n, e), n[e] = function(e) {
                                        var t = o.apply(n, [].slice.call(arguments));
                                        try {
                                            i({
                                                state: e
                                            })
                                        } catch (e) {}
                                        return t
                                    }
                                }
                            },
                            revertOverriddenStates: m,
                            init: function(e) {
                                a = e.shouldPushToQueue
                            }
                        }, VWO.destroy = de.clearAllListeners, VWO._.eventsManager = de
                    }();

                function ce(e, t) {
                    if (e) {
                        var n, i = "." + e,
                            o = window.vwo_$;
                        if (!(t = t || {})[e]) {
                            try {
                                n = o(i)
                            } catch (e) {
                                n = ""
                            }
                            if (1 === n.length) return 1;
                            t[e] = !0
                        }
                    }
                }

                function le(e) {
                    if (e) {
                        var t, n = window.vwo_$;
                        try {
                            t = n("#" + e)
                        } catch (e) {
                            t = ""
                        }
                        return t.length
                    }
                }

                function ue(e) {
                    if (!e) return null;
                    if (e.previousElementSibling) return e.previousElementSibling;
                    for (; e = e.previousSibling;)
                        if (1 === e.nodeType) return e
                }

                function he(e, t) {
                    if (!e) return null;
                    if (e === document) return "#document";
                    t = t || {};
                    var n, i, o, r, a, s = e,
                        d = [],
                        c = e.tagName,
                        l = window.vwo_$;
                    if (e === document.body || e === document.head) return c;
                    for (; e;)
                        if (VWO._.ac && VWO._.ac.hFCVJ && e.__vue__ && e.__isFragment) e = ue(e);
                        else {
                            n = (c = "undefined" != typeof ShadowRoot && e instanceof ShadowRoot ? "shadow-root" : e.tagName) && c.match(/^((?:[\w\u00c0-\uFFFF\*-]|\\.)+)/), c && n && (n && n[0]) === c || (c = "*");
                            var u = -1 < ["INPUT", "SELECT"].indexOf(e.tagName);
                            try {
                                i = l(e).attr("id")
                            } catch (s) {
                                i = e.id
                            }
                            u && e.name ? c = c + '[name="' + e.name + '"]' : i && "string" == typeof i && le(i) && (c = i.match(/^\d/) ? c + '[id="' + i + '"]' : c + "#" + i), o = (o = e.getAttribute && e.getAttribute("class")) ? o.split(/\s+/) : [];
                            for (var h = 0; h < o.length; h++)
                                if (a = "." + (r = o[h]), ce(r, t)) {
                                    c += a;
                                    break
                                }
                            d.unshift(c), e = ue(e)
                        }
                    var f = s.nlsParent || s.parentNode;
                    return !d.length || -1 !== d[0].indexOf("#") || f && "HEAD" === f.nodeName || s.host || (d[0] += ":first-child"), he("undefined" != typeof ShadowRoot && s instanceof ShadowRoot && s.host ? s.host : f, t) + (d.length ? " > " + d.join(" + ") : "")
                }
                var fe = {
                        getTrueWidth: function(e) {
                            var t = window.vwo_$ || window.jQuery;
                            return t && t(e).width() || e.clientWidth || e.offsetWidth || e.scrollWidth
                        },
                        getTrueHeight: function(e) {
                            var t = window.vwo_$ || window.jQuery;
                            return t && t(e).height() || e.clientHeight || e.offsetHeight || e.scrollHeight
                        },
                        getOuterWidth: function(e) {
                            var t = window.vwo_$ || window.jQuery;
                            return t && t(e).outerWidth() || e.offsetWidth || this.getTrueWidth(e)
                        },
                        getOuterHeight: function(e) {
                            var t = window.vwo_$ || window.jQuery;
                            return t && t(e).outerHeight() || e.offsetHeight || this.getTrueHeight(e)
                        },
                        getRelativeStats: function(e, t, n, i, o, r) {
                            var a = e.offset().top - t.offset().top,
                                s = e.offset().left - t.offset().left,
                                d = 0,
                                c = 0;
                            return o && r && (d = o - s, c = r - a), {
                                correctedTargetHeight: this.getTrueHeight(t[0]),
                                correctedTargetWidth: this.getTrueWidth(t[0]),
                                correctedTargetOffsetX: n + s,
                                correctedTargetOffsetY: i + a,
                                correctedTargetPageX: d,
                                correctedTargetPageY: c
                            }
                        },
                        postProcessMutations: function(e) {
                            var i = this;
                            return e.forEach(function(e) {
                                var t, n;
                                null !== (t = e.addedNodes) && void 0 !== t && t.length && (e.addedNodes = i.deduplicateNodes(e.addedNodes)), null !== (n = e.removedNodes) && void 0 !== n && n.length && (e.removedNodes = i.deduplicateNodes(e.removedNodes))
                            }), e
                        },
                        deduplicateNodes: function(e) {
                            var t = [];
                            if (t.push(e[0]), 1 < e.length) {
                                var n = [],
                                    i = JSON.parse(JSON.stringify(e[0]));
                                i.id && delete i.id, i.parentNode && delete i.parentNode, n.push(JSON.stringify(i));
                                for (var o = 1; o < e.length; o++) {
                                    var r = JSON.parse(JSON.stringify(e[o]));
                                    delete r.id, delete r.parentNode, -1 === n.indexOf(JSON.stringify(r)) && (t.push(e[o]), n.push(JSON.stringify(r)))
                                }
                            }
                            return t
                        }
                    },
                    me = ["html", "mutations", "recording", "error", "jsonEn"],
                    pe = {
                        log: function() {
                            for (var e = [], t = 0; t < arguments.length; t++) e[t] = arguments[t];
                            if (window.console && -1 !== document.cookie.indexOf("vwo_log_mode")) return window.console.log.apply(window, e)
                        }
                    },
                    ge = 3,
                    ve = 0,
                    we = null,
                    _e = function(e, t, n) {
                        D.pageExitHandling() || setTimeout(function() {
                            n.retries = n.retries || 0, n.retries++, n.retries <= ge ? Te(e, t, n) : VWO._.customError({
                                msg: "Failed to send Analyze " + n.type + " request after " + ge + " retries: " + JSON.stringify({
                                    uuid: F.getUUID(),
                                    sId: window.VWO._.sessionInfoService.getSessionId()
                                }),
                                url: "nls/ajax.ts"
                            })
                        }, 50)
                    };

                function ye(e, t) {
                    if (e instanceof Blob) return e.size;
                    "string" != typeof e && (e = JSON.stringify(e));
                    try {
                        (t = t || {}).lineBreaks = (null == t ? void 0 : t.lineBreaks) || 1, t.ignoreWhitespace = (null == t ? void 0 : t.ignoreWhitespace) || !1;
                        var n = 0,
                            i = null == e ? void 0 : e.length,
                            o = null == e || e.replace(/[\u0100-\uFFFF]/g, function() {
                                return n++
                            }).length,
                            r = i - (null == e ? void 0 : e.replace(/(\r?\n|\r)/g, "").length);
                        return o = 2 * n, t.ignoreWhitespace ? (null == (e = null == e ? void 0 : e.replace(/(\r?\n|\r|\s+)/g, "")) ? void 0 : e.length) + o : i + o + Math.max(0, t.lineBreaks * (r - 1))
                    } catch (e) {}
                }

                function Ee(e) {
                    var t = A.get("Object").entries(e).sort(function(e, t) {
                        return ye(e[1]) - ye(t[1])
                    });
                    return t[t.length - 1]
                }
                var Te = function(t, n, i) {
                        var e = i.url,
                            o = new(A.get("XMLHttpRequest"));
                        o.open("POST", e, !0), "JSON" === t ? o.setRequestHeader("Content-Type", "application/x-www-form-urlencoded; charset=UTF-8") : o.formData = n, o.send(n), o.onload = function() {
                            if (200 <= this.status && this.status < 400)
                                if (this.response) try {
                                    var e = A.get("JSON");
                                    i.success(e.parse(this.response))
                                } catch (e) {} else i.success();
                                else i.formData = this.formData, i.error(), _e(t, n, i);
                            i.complete()
                        }, o.onerror = function() {
                            i.formData = this.formData, i.error(), i.complete(), _e(t, n, i)
                        }
                    },
                    Se = {
                        workerMessages: {},
                        useBeacon: !1,
                        workerUrl: "",
                        isWorkerAvailable: function() {
                            return this.workerUrl
                        },
                        isMultipartSupported: function() {
                            return !!window.FormData
                        },
                        isWorkerRequired: function(e) {
                            for (var t = 0; t < me.length; t++) {
                                if (e[me[t]]) return !0
                            }
                        },
                        ajax: function(e) {
                            var t = void 0 !== (e = e || {}).url ? e.url : "",
                                n = void 0 !== e.type ? e.type : "AUTO",
                                i = e.data = void 0 !== e.data ? R({}, e.data) : {},
                                o = e.success = "function" == typeof e.success ? e.success : function() {},
                                r = e.error = "function" == typeof e.error ? e.error : function() {},
                                a = e.complete = "function" == typeof e.complete ? e.complete : function() {},
                                s = this.workerUrl = e.workerUrl,
                                d = e.formData,
                                c = this.useBeacon,
                                l = this.workerMessages,
                                u = A.get("Object");
                            if (u.keys(i).length) {
                                var h = this.createQueryString(i);
                                if (c && D.pageExitHandling()) {
                                    var f = new FormData;
                                    for (var m in i)
                                        if (u.prototype.hasOwnProperty.call(i, m))
                                            if ("object" == typeof i[m]) {
                                                var p = JSON.stringify(i[m]);
                                                f.append(m, p)
                                            } else f.append(m, i[m]);
                                    var g = e.url + "?_a=" + i.a + "&_u=" + encodeURIComponent(i.url);
                                    navigator.sendBeacon(g, f), o(), a()
                                } else if ("GET" === n.toUpperCase() || "AUTO" === n.toUpperCase() && h.length <= 1800) {
                                    e.type = "GET";
                                    var v = new(A.get("Image"));
                                    v.src = t + "?" + h, v.onload = function() {
                                        o(), a()
                                    }, v.onerror = function() {
                                        r(), a()
                                    }
                                } else if ("POST" === n.toUpperCase() || "AUTO" === n.toUpperCase() && 1800 < h.length) {
                                    e.url = e.url + "?_a=" + i.a + "&_u=" + encodeURIComponent(i.url), e.type = "POST";
                                    var w = [];
                                    try {
                                        s && (we = we || new Worker(s))
                                    } catch (e) {}
                                    if (d) Te("FormData", d, e);
                                    else if (e.dataLength = e.dataLength || h.length, this.isWorkerRequired(i) && this.isWorkerAvailable() && this.isMultipartSupported() && we) {
                                        for (var _ = 0; _ < me.length; _++) w[_] = i[me[_]];
                                        l[++ve] = {
                                            data: i,
                                            options: e
                                        }, we.postMessage({
                                            id: ve,
                                            action: "compress",
                                            strings: w
                                        }), we.onmessage = we.onmessage || function(e) {
                                            var t = e.data,
                                                n = t.strings;
                                            if (l[t.id]) {
                                                var i, o = l[t.id].data,
                                                    r = l[t.id].options;
                                                if ("compressed" === t.action) {
                                                    for (var a = 0; a < n.length; a++) i = me[a], n[a] && (o["c_" + i] = new Blob([n[a]]), pe.log("Original Size: " + o[i].length + ", Compressed Size: " + o["c_" + i].size), delete o[i]);
                                                    var s = new FormData;
                                                    for (var d in o) u.prototype.hasOwnProperty.call(o, d) && s.append(d, o[d]);
                                                    try {
                                                        if (1e7 < ye(JSON.stringify(o)) + n.reduce(function(e, t) {
                                                                return t && t.byteLength ? e + t.byteLength : e
                                                            }, 0)) {
                                                            var c = Ee(o);
                                                            window.VWO._.customError && window.VWO._.customError({
                                                                msg: "callStack size exceeded 10mb, data : " + JSON.stringify({
                                                                    uuid: F.getUUID(),
                                                                    sId: window.VWO._.sessionInfoService.getSessionId(),
                                                                    key: c[0]
                                                                }),
                                                                url: "nls/ajax.ts"
                                                            })
                                                        }
                                                    } catch (e) {}
                                                    Te("FormData", s, r), delete l[t.id]
                                                }
                                            }
                                        }
                                    } else Te("JSON", h, e)
                                }
                            }
                        },
                        createQueryString: function(e) {
                            var t = "";
                            for (var n in e) Object.prototype.hasOwnProperty.call(e, n) && (t += t.length ? "&" : "", t += n + "=", t += encodeURIComponent(e[n]));
                            return t
                        },
                        sendError: function(e, t, n, i) {
                            window.VWO._.customError && window.VWO._.customError({
                                msg: e,
                                url: t,
                                lineno: n || 0,
                                colno: i || 0,
                                source: encodeURIComponent(window.location.href)
                            })
                        }
                    },
                    be = _.get("JSON"),
                    Oe = window.VWO._.localStorageService,
                    Re = {
                        localStorageKey: "_vwo_nls_q_" + window._vwo_acc_id,
                        init: function() {
                            try {
                                if (329635 === window._vwo_acc_id || 5e5 < window._vwo_acc_id) {
                                    var e = Oe.get(this.localStorageKey) || be.stringify([]);
                                    Oe.set(this.localStorageKey, e)
                                }
                            } catch (e) {
                                window.console.info(e)
                            }
                        },
                        isRequestQueueable: function() {
                            try {
                                var e = 329635 === window._vwo_acc_id || 5e5 < window._vwo_acc_id;
                                return e ? Oe.get(Re.localStorageKey) : e
                            } catch (e) {
                                return !1
                            }
                        },
                        queueRequest: function(t, n) {
                            if (Re.isRequestQueueable()) {
                                var e = be.parse(Oe.get(Re.localStorageKey)),
                                    i = {
                                        data: t,
                                        options: n
                                    };
                                e.unshift(be.stringify(i));
                                try {
                                    Oe.set(Re.localStorageKey, be.stringify(e))
                                } catch (e) {
                                    at.sendData(t, n)
                                }
                            } else at.sendData(t, n)
                        },
                        deQueueRequest: function() {
                            if (Re.isRequestQueueable()) {
                                var e = be.parse(Oe.get(Re.localStorageKey)),
                                    t = null == e ? void 0 : e.length;
                                if (t)
                                    for (var n = 0; n < t; n++) {
                                        var i = be.parse(e.pop());
                                        at.sendData(i.data, i.options)
                                    }
                                Oe.set(Re.localStorageKey, be.stringify(e))
                            }
                        }
                    },
                    Ce = {
                        TRACK_GLOBAL_COOKIE_NAME: "_vwo_ds",
                        TRACK_SESSION_COOKIE_NAME: "_vwo_sn",
                        TRACK_SESSION_COOKIE_EXPIRY: window.VWO.TRACK_SESSION_COOKIE_EXPIRY_CUSTOM || 1 / 48
                    },
                    Ne = A.get("Array"),
                    De = A.get("JSON"),
                    Ie = 20,
                    Ae = 40,
                    xe = 10,
                    ke = (Le.prototype.handleBase64Attr = function(e, t, n) {
                        var i = {
                            value: n
                        };
                        if (Z.r && ("IMG" === e.tagName && ("__nls-src" === t || "src" === t) || "INPUT" === e.tagName && "value" === t) && n.startsWith("data:")) {
                            var o = this.recordBase64(n),
                                r = o.base64Id,
                                a = o.compressed;
                            i.value = r, i.options = {
                                isBase64Tag: a
                            }
                        }
                        return i
                    }, Le.prototype.recordBase64 = function(e, t) {
                        void 0 === t && (t = !1);
                        var n = e.length > Ae;
                        if (!n) return {
                            base64Id: e,
                            compressed: n
                        };
                        var i = Le.generateBase64Id(e),
                            o = P.getItem("b64") || [];
                        return -1 === o.indexOf(i) && (o.push(i), P.setItem("b64", o), Z.htmlRequestSuccess ? t ? (this.queuedData.push({
                            id: i,
                            value: e
                        }), this.checkAndSendBatchData(t)) : Le.sendBatch([{
                            id: i,
                            value: e
                        }]) : this.queuedData.push({
                            id: i,
                            value: e
                        })), {
                            base64Id: i,
                            compressed: n
                        }
                    }, Le.generateBase64Id = function(e) {
                        for (var t = Math.floor(e.length / Ie), n = "", i = 0; i < e.length; i += t) n += e[i];
                        return n
                    }, Le.prototype.sendQueuedData = function() {
                        this.checkAndSendBatchData(!1), this.queuedData = new Ne
                    }, Le.prototype.sendQueuedDataForced = function() {
                        this.checkAndSendBatchData(!0, !0), this.queuedData = new Ne
                    }, Le.prototype.checkAndSendBatchData = function(e, t) {
                        if (void 0 === e && (e = !1), void 0 === t && (t = !1), !(e && this.queuedData.length < xe) || t)
                            for (; 0 < this.queuedData.length;) {
                                var n = e ? this.queuedData.splice(0, xe) : this.queuedData.splice(0, 1);
                                Le.sendBatch(n)
                            }
                    }, Le.sendBatch = function(e) {
                        at.sendB64(De.stringify({
                            b64: e.map(function(e) {
                                return {
                                    key: e.id,
                                    value: e.value
                                }
                            })
                        }))
                    }, Le);

                function Le() {
                    var e = this;
                    this.queuedData = new Ne, setInterval(function() {
                        0 < e.queuedData.length && e.queuedData.length < xe && (Le.sendBatch(e.queuedData), e.queuedData = new Ne)
                    }, 3e3)
                }
                var Me = new ke,
                    Pe = (Ve.prototype.hideWithCoordinates = function(e, t, n, i, o) {
                        var r = {
                            x0: Math.round(t),
                            y0: Math.round(n),
                            x1: Math.round(i),
                            y1: Math.round(o)
                        };
                        this.maskedCoordinates[e] = r
                    }, Ve.prototype.pauseCanvasRec = function() {
                        ze.sendNotifier(), Me.sendQueuedDataForced(), this.localStorageService.setItem("_vwo_recPaused", "true"), Z.canvasRec.allowCanvasRec = !1
                    }, Ve.prototype.resumeCanvasRec = function() {
                        this.localStorageService.setItem("_vwo_recPaused", "false"), Z.canvasRec.allowCanvasRec = !0
                    }, Ve.prototype.sendCustomEvent = function(e, t) {
                        VWO.event = VWO.event || function() {
                            VWO.push(["event"].concat([].slice.call(arguments)))
                        }, VWO.event(e, "string" == typeof t ? JSON.parse(t) : t)
                    }, Ve.prototype.sendCustomAttribute = function(e) {
                        VWO.visitor = VWO.visitor || function() {
                            VWO.push(["visitor"].concat([].slice.call(arguments)))
                        }, VWO.visitor("string" == typeof e ? JSON.parse(e) : e)
                    }, Ve.prototype.removeCoordinatesFor = function(e) {
                        delete this.maskedCoordinates[e]
                    }, Ve.prototype.getMaskedCoordinates = function() {
                        return this.maskedCoordinates
                    }, Ve);

                function Ve() {
                    this.maskedCoordinates = {}, this.localStorageService = window.VWO._.localStorageService
                }
                window.VWOInsights = window.VWOInsights || new Pe;
                var We = window.VWOInsights,
                    He = (Ue.prototype.getCanvasData = function(d) {
                        try {
                            var e = window.getComputedStyle(d),
                                t = d.width / d.height,
                                c = d.width,
                                l = d.height;
                            l > this.MAX_RESOLUTION && (l = this.MAX_RESOLUTION, c = Math.round(l * t));
                            var n = document.createElement("canvas");
                            n.width = c, n.height = l, n.style.width = d.style.width, n.style.height = d.style.height;
                            var u = n.getContext("2d");
                            if (!u) return null;
                            u.drawImage(d, 0, 0, c, l);
                            var h = We.getMaskedCoordinates();
                            Object.keys(h).forEach(function(e) {
                                var t = h[e],
                                    n = t.x0,
                                    i = t.y0,
                                    o = t.x1,
                                    r = t.y1,
                                    a = c / d.width,
                                    s = l / d.height;
                                u.rect(n * a, i * s, (o - n) * a, (r - i) * s), u.fillStyle = "rgba(0, 0, 0, 1)", u.fill()
                            });
                            var i = n.toDataURL("image/png");
                            return n.remove(), {
                                imageUrl: i,
                                width: c,
                                height: l,
                                actualWidth: e.width,
                                actualHeight: e.height
                            }
                        } catch (e) {
                            return null
                        }
                    }, Ue.prototype.init = function(e) {
                        Z.Recording.cnv && (this.context = e, this.FPS = Z.Recording.cnv.fps, this.MAX_RESOLUTION = Z.Recording.cnv.res, this.startCanvasRecording(), document.querySelector("flutter-view") && (Z.canvasRec.isFlutterWeb = !0))
                    }, Ue.prototype.startCanvasRecording = function() {
                        var e = this;
                        this.timerId && this.stopCanvasRecording(), Ue.shouldRecordCanvas() && this.recordCanvasData(), this.timerId = setInterval(function() {
                            Ue.shouldRecordCanvas() && e.recordCanvasData()
                        }, 1e3 / this.FPS)
                    }, Ue.shouldRecordCanvas = function() {
                        return Z.stopRecording !== Z.enums.formAnalysis.PERMANENT_STATE && !(document.hidden || Z.canvasRec.isFlutterWeb && !document.hasFocus()) && Z.canvasRec.allowCanvasRec
                    }, Ue.prototype.sendNotifier = function() {
                        this.recordCanvasData(!0)
                    }, Ue.prototype.recordCanvasData = function(c) {
                        var l = this;
                        void 0 === c && (c = !1), this.trackedNodes.forEach(function(e) {
                            var t = e.nodeId,
                                n = e.node;
                            try {
                                if (!F.isElementInViewport(n)) return;
                                var i = l.getCanvasData(n);
                                if (!i) return;
                                var o = i.imageUrl,
                                    r = u(i, ["imageUrl"]),
                                    a = Me.recordBase64(c ? "" : o, !0),
                                    s = a.base64Id,
                                    d = a.compressed;
                                l.context.addMutation({
                                    time: C() - Z.startTime,
                                    canvasData: R({
                                        nodeId: t,
                                        imageUrl: s,
                                        compressed: d
                                    }, r)
                                })
                            } catch (e) {}
                        })
                    }, Ue.prototype.stopCanvasRecording = function() {
                        this.trackedNodes = [], this.timerId && (clearInterval(this.timerId), this.timerId = null)
                    }, Ue.prototype.trackCanvasNode = function(e) {
                        "CANVAS" !== e.tagName || F.needsMasking(e, Z.Recording.anonymizeKeys) || this.trackedNodes.push({
                            nodeId: e.nlsNodeId,
                            node: e
                        })
                    }, Ue);

                function Ue() {
                    this.trackedNodes = []
                }
                var ze = new He;
                Z.canvasRecorder = ze;
                var Fe = v,
                    je, Xe, Be = "nls_ajax.php",
                    Ye = ["eTags", "eTagsV2"],
                    qe = window.VWO.data,
                    Ke = function() {},
                    Ge = {},
                    Je, $e = 5e3,
                    Qe = 100,
                    Ze = 0,
                    et = 1,
                    tt = function() {
                        return !Z.faultyWorker && Z.workerUrl
                    },
                    nt = function(e) {
                        var t = Object.keys(e);
                        if (t.length > Ye.length) return !0;
                        for (var n = 0; n < t.length; n++)
                            if (-1 === Ye.indexOf(t[n])) return !0;
                        return !1
                    };

                function it() {
                    var e = {},
                        t = A.get("JSON");
                    return Z.r && (e.re = t.stringify(Z.ids.re)), Z.hs && (e.he = t.stringify(Z.ids.he)), Z.fae && (e.fe = t.stringify(Z.ids.fe)), e
                }

                function ot(e) {
                    return 607307 === e
                }

                function rt(e, t) {
                    if (t) {
                        var n = (P.getItem(e) || []).concat(t.split(","));
                        P.setItem(e, n)
                    }
                }
                var at = {
                        formSubmitCallbacks: [],
                        saveNewRecording: function(e) {
                            var t, n, i, o, r = Z.getViewportDimensions(),
                                a = Z.getScrollPercentage(),
                                s = Z.formAnalysis ? Z.formAnalysis.forms : {},
                                d = {},
                                c = Z.calcDuration(null, !0);
                            if (e = e || Ke, Z.isEligibleToSendRecordingData()) {
                                var l = VWO._.cookies,
                                    u = A.get("JSON");
                                Z.recordingData.totals.scroll = a < 1 ? 10 : a;
                                var h, f = Z.getScreenDimensions(),
                                    m = l.get(Ce.TRACK_SESSION_COOKIE_NAME),
                                    p = "referrer=",
                                    g = m.indexOf(p) + p.length;
                                h = p.length <= g ? m.substring(g) : "", atob && (h = atob(h));
                                var v = {
                                    codedo: "set_html_and_recording",
                                    a: Z.ids.account,
                                    e: u.stringify(Z.ids.experiment),
                                    title: Z.getPageTitle(),
                                    url: window.location.href,
                                    referring_url: h,
                                    session_id: Z.ids.session,
                                    recording_id: Z.ids.recording,
                                    return_visitor: Z.returnVisitor,
                                    ins: Z.newSession,
                                    start_time: Z.startTime,
                                    end_time: c.currentTime,
                                    window_width: r.width,
                                    window_height: r.height,
                                    sh: f.height,
                                    sw: f.width,
                                    vn: Z.version,
                                    rand: Math.random()
                                };
                                if (void 0 !== VWO.phoenix && (Z.canonicalUrl ? v.cnnUrl = Z.canonicalUrl : v.cnnUrl = v.url), VWO.featureInfo && VWO.featureInfo.ipv6Ancdn) try {
                                    var w = VWO._.allSettings.dataStore.plugins.IP,
                                        _ = VWO._.allSettings.dataStore.plugins.GEO;
                                    v.geo = u.stringify(R({
                                        ip: w
                                    }, _))
                                } catch (e) {}
                                VWO._ && (v.eTime = VWO._.commonUtil.getCurrentTimestamp());
                                var y = Z.getCurrentHrefs();
                                if ((VWO._.ast && !Z.assetSnapshottingEnabled || Z.newSession) && (y = Z.getUrlSpecificSnapshottedAssets()), Z.assetSnapshottingEnabled = VWO._.ast, Z.r && Z.assetSnapshottingEnabled && y.length && (v.asts = y.slice(0, Qe), Je = Date.now()), Z.newSession = !1, Z.hs && (i = {
                                        scroll_percentage: Z.recordingData.totals.scroll
                                    }), Z.fae) {
                                    var E = Z.formAnalysis ? Z.formAnalysis.f : {};
                                    n = 0 === Object.keys(E).length ? {
                                        forms: u.stringify(E),
                                        f: u.stringify(E)
                                    } : {
                                        forms: u.stringify(s),
                                        f: u.stringify(E)
                                    }
                                }
                                Z.r && (t = {
                                    duration: c.duration,
                                    clicks: Z.recordingData.totals.clicks,
                                    movements: Z.recordingData.totals.movements,
                                    end_time: c.currentTime
                                }, Z.Recording && Z.Recording.addInitialHTML(v)), Z.analyze && (Be = "analyze", o = it(), d = Z.getTags(), Z.resetTagAfterSent()), Fe.extend(v, t, n, i, o, d), this.sendData(v, {
                                    callback: e
                                }), Z.resetClicksCount(), et = 1, "true" !== window.VWO._.localStorageService.getItem("_vwo_recPaused") && Z.canvasRec.allowCanvasRec || (ze.sendNotifier(), Z.canvasRec.allowCanvasRec = !1)
                            }
                        },
                        sendRecordingData: function(e, t, n) {
                            if (void 0 === n && (n = null), !Z.htmlRequestSuccess && !D.preloadListeners()) return !1;
                            var i, o, r, a, s, d, c, l = Z.checkIfIdle(),
                                u = A.get("JSON");
                            if (!e) {
                                if (!Z.isEligibleToSendRecordingData()) return;
                                switch (Z.stopRecording) {
                                    case Z.enums.formAnalysis.TEMPORARY_STATE:
                                        return void(l || (Z.resetAfterDataSent(), Z.clearSessionIdleTimeout(), Z.triggerSessionDeleteTimeout()));
                                    case Z.enums.formAnalysis.PERMANENT_STATE:
                                        return
                                }
                                if (l) {
                                    if (Z.triggerSessionIdleTimeout(), l) return;
                                    Z.clearSessionIdleTimeout()
                                }
                                o = {
                                    a: Z.ids.account,
                                    e: u.stringify(Z.ids.experiment),
                                    url: t || window.location.href,
                                    session_id: Z.ids.session,
                                    recording_id: Z.ids.recording,
                                    vn: Z.version,
                                    rand: Math.random()
                                }, void 0 !== VWO.phoenix && (Z.canonicalUrl ? o.cnnUrl = Z.canonicalUrl : o.cnnUrl = o.url), VWO._ && (o.eTime = VWO._.commonUtil.getCurrentTimestamp());
                                var h = Z.getCurrentHrefs();
                                VWO._.ast && !Z.assetSnapshottingEnabled && (h = Z.getUrlSpecificSnapshottedAssets()), Z.assetSnapshottingEnabled = VWO._.ast;
                                var f = Date.now(),
                                    m = D.completeSnapshottingEnabled();
                                for (var p in Z.r && Z.assetSnapshottingEnabled && h.length && (!m || $e < f - Je) && (o.asts = h.slice(0, Qe), Je = f), a = {}, this.formSubmitCallbacks) Object.prototype.hasOwnProperty.call(this.formSubmitCallbacks, p) && (i = this.formSubmitCallbacks[p]()) && Fe.extend(a, i);
                                if (r = Z.calcDuration(a), Z.r && (d = {
                                        movements: Z.recordingData.totals.movements,
                                        clicks: Z.recordingData.totals.clicks,
                                        duration: r.duration,
                                        start_time: Z.startTime,
                                        end_time: r.currentTime
                                    }, s = !0), nt(a) && (s = !0, Fe.extend(o, a)), !s) return void Z.resetTagAfterSent();
                                Z.analyze && (Be = "analyze", c = it(), o.fRS = Z.htmlRequestSuccess, delete o.e), n ? (Fe.extend(n, o, c, d), D.data360migrated() && delete n.mc, D.pageExitHandling() && Z.htmlRequestSuccess ? (Se.useBeacon = !0, this.sendData(n), Se.useBeacon = !1) : Re.queueRequest(n)) : (Fe.extend(o, d, c), Re.queueRequest(o)), Z.resetAfterDataSent(), Z.resetClicksCount(), et = 1
                            }
                        },
                        sendData: function(t, e, n) {
                            if (void 0 === n && (n = !1), !window.VWO.consentMode || !window.VWO.consentMode.dT) {
                                je = "https://dev.visualwebsiteoptimizer.com/", Xe = Z.analyze && qe.asn && "https://" + qe.asn + "/", e = e || {};
                                var i = A.get("JSON"),
                                    o = e.callback || function() {},
                                    r = Z.analyze || window.VWO._vba.forceWorker ? tt() : null,
                                    a = (Xe || je) + Be,
                                    s = Z.ids.session;
                                if (Ge[s] ? Ge[s] !== a && (Se.sendError("Recording url is not matching __ previous url: " + Ge[s] + " __ new url: " + a + " __ sessionId: " + s + "__ uuid: " + F.getUUID(), "ajax-nls.js", 33), Ge[s] = a) : Ge[s] = a, n) {
                                    var d = new URL(a),
                                        c = {
                                            a: t.a,
                                            url: t.url,
                                            session_id: t.session_id,
                                            he: t.he,
                                            rand: Math.random().toString(),
                                            eTime: VWO._.commonUtil.getCurrentTimestamp()
                                        };
                                    for (var l in D.data360migrated() || (c.mc = t.mc), void 0 !== VWO.phoenix && (c.cnnUrl = t.cnnUrl), c) c[l] && d.searchParams.append(l, c[l]);
                                    fetch(d.toString(), {
                                        keepalive: !0
                                    }).then()
                                } else {
                                    var u;
                                    if (D.data360migrated() && (t.v2 = !0, delete t.pEr, delete t.mc), t.count = t.count || ++Ze, t[I.RECORDING_META]) try {
                                        u = i.parse(t[I.RECORDING_META])[I.CLICK_TEXT_DATA]
                                    } catch (e) {}
                                    Se.ajax({
                                        url: a,
                                        type: e.method,
                                        data: t,
                                        workerUrl: ot(t.a) ? null : r,
                                        success: function(e) {
                                            o(e), "object" == typeof u && (rt(I.DEAD_CLICK_TEXT, u[I.DEAD_CLICK_TEXT]), rt(I.RAGE_CLICK_TEXT, u[I.RAGE_CLICK_TEXT])), (Array.isArray(t.asts) && 0 < t.asts.length || !VWO._.ast) && Z.resetHrefCollection(t.asts)
                                        },
                                        error: function() {
                                            if (D.pageExitHandling()) {
                                                e.retries = e.retries || 0, e.retries++, e.retries <= 3 ? Re.queueRequest(t, e) : VWO._.customError({
                                                    msg: "Failed to send Analyze request after 3 retries: " + i.stringify({
                                                        uuid: F.getUUID(),
                                                        sId: window.VWO._.sessionInfoService.getSessionId()
                                                    }),
                                                    url: "nls/ajax-nls.ts"
                                                })
                                            }
                                        }
                                    })
                                }
                            }
                        },
                        sendB64: function(e) {
                            if (Z.r && Z.htmlRequestSuccess && Z.analyze) {
                                var t = it(),
                                    n = Z.calcDuration(null, !1),
                                    i = {
                                        a: Z.ids.account,
                                        url: window.location.href,
                                        duration: n.duration,
                                        session_id: Z.ids.session,
                                        recording_id: Z.ids.recording,
                                        vn: Z.version,
                                        rand: Math.random(),
                                        end_time: n.currentTime + et,
                                        jsonEn: e,
                                        eTime: void 0,
                                        fRS: Z.htmlRequestSuccess
                                    };
                                VWO._ && (i.eTime = VWO._.commonUtil.getCurrentTimestamp() + et), Be = "analyze", Fe.extend(i, t), this.sendData(i), et++
                            }
                        }
                    },
                    st = "_",
                    dt = {
                        mc: "Click",
                        mm: "Mouse Move",
                        mu: "Mouse Up",
                        dc: "Dead Click",
                        rc: "Rage Click",
                        rcdc: "Rage & Dead Click",
                        sS: "Screen Scale",
                        tc: "Touch",
                        is: "Scale Information",
                        rs: "Resize and Scale",
                        re: "Resize",
                        bl: "Blur",
                        fo: "Focus",
                        rb: "Radio Button",
                        cb: "Checkbox",
                        sb: "Selectbox",
                        sc: "Scroll",
                        es: "Element Scroll",
                        ku: "Key Up",
                        url: "Page Load",
                        oc: "Orientation Change",
                        mr: "Click",
                        vwomt: "vwo Meta event",
                        ko: "Keyboard Open",
                        kc: "Keyboard Close",
                        ic: "Input Change",
                        ct: "Cursor Thrashers",
                        pr: "Page Refresh",
                        qb: "Quick Back",
                        cp: "Copy",
                        sl: "Selection",
                        rsl: "Repeated Scrolled",
                        rh: "Repeated Hovered",
                        to: "Tab out",
                        ti: "Tab in",
                        li: "Leave Intent",
                        vS: "Variation Shown",
                        mT: "Metric Converted",
                        cE: "Custom Event",
                        pU: "Page Unload",
                        er_: "Error Captured"
                    },
                    ct = function(e) {
                        return e ? e.replace(/!-u-!/g, "_").replace(/!-c-!/g, ",") : ""
                    },
                    lt = {
                        parseEventString: function(e) {
                            var t, n = e.split(st),
                                i = n[0];
                            if (i in dt) {
                                var o = void 0,
                                    r = void 0,
                                    a = parseInt(n[1], 10),
                                    s = "";
                                if ("mc" !== i && "mr" !== i || (10 === n.length ? o = "{11}" === n[9] ? dt.dc : "{12}" === n[9] ? dt.rc : "{11:12}" === n[9] ? dt.rcdc : dt.mc : 9 === n.length && (o = dt[i]), r = n[2], t = {
                                        action: i,
                                        label: o,
                                        timeMomentRelativeToPage: a,
                                        selectorPath: s = ct(r),
                                        sanitizedSelectorPath: r,
                                        elWidth: n[3],
                                        elHeight: n[4],
                                        relX: n[5],
                                        relY: n[6],
                                        docX: n[7],
                                        docY: n[8],
                                        innerText: ""
                                    }), "mm" !== i && "mu" !== i || (o = dt[i], r = n[2], t = {
                                        action: i,
                                        label: o,
                                        timeMomentRelativeToPage: a,
                                        selectorPath: s = ct(r),
                                        sanitizedSelectorPath: r,
                                        elWidth: n[3],
                                        elHeight: n[4],
                                        relX: n[5],
                                        relY: n[6],
                                        docX: n[7],
                                        docY: n[8]
                                    }), "sS" === i && (t = {
                                        action: i,
                                        label: o = dt[i],
                                        timeMomentRelativeToPage: a,
                                        screenScaleX: n[2],
                                        screenScaleY: n[3]
                                    }), "tc" !== i && "is" !== i || (t = {
                                        action: i,
                                        label: o = dt[i],
                                        timeMomentRelativeToPage: a,
                                        scaleX: n[2],
                                        scaleY: n[3],
                                        scrollX: n[4],
                                        scrollY: n[5]
                                    }), "rs" === i && (t = {
                                        action: i,
                                        label: o = dt[i],
                                        timeMomentRelativeToPage: a,
                                        width: n[2],
                                        height: n[3],
                                        scrollX: n[4],
                                        scrollY: n[5],
                                        scaleX: n[6],
                                        scaleY: n[7],
                                        screenScaleX: n[8],
                                        screenScaleY: n[9],
                                        pageWidth: n[10],
                                        pageHeight: n[11]
                                    }), "re" === i && (t = {
                                        action: i,
                                        label: o = dt[i],
                                        timeMomentRelativeToPage: a,
                                        width: n[2],
                                        height: n[3],
                                        scrollX: n[4],
                                        scrollY: n[5]
                                    }), "bl" !== i && "sb" !== i && "ku" !== i && "ic" !== i || (o = dt[i], r = n[2], t = {
                                        action: i,
                                        label: o,
                                        timeMomentRelativeToPage: a,
                                        selectorPath: s = ct(r),
                                        sanitizedSelectorPath: r,
                                        value: n[3]
                                    }), "fo" === i && (o = dt[i], r = n[2], t = {
                                        action: i,
                                        label: o,
                                        timeMomentRelativeToPage: a,
                                        selectorPath: s = ct(r),
                                        sanitizedSelectorPath: r
                                    }), "rb" !== i && "cb" !== i || (o = dt[i], r = n[2], t = {
                                        action: i,
                                        label: o,
                                        timeMomentRelativeToPage: a,
                                        selectorPath: s = ct(r),
                                        sanitizedSelectorPath: r,
                                        isChecked: n[3]
                                    }), "sc" === i && (t = {
                                        action: i,
                                        label: o = dt[i],
                                        timeMomentRelativeToPage: a,
                                        scaleX: n[2],
                                        scaleY: n[3],
                                        scrollX: n[4],
                                        scrollY: n[5],
                                        width: n[6],
                                        height: n[7],
                                        pageWidth: n[8],
                                        pageHeight: n[9]
                                    }), "es" === i && (o = dt[i], r = n[2], t = {
                                        action: i,
                                        label: o,
                                        timeMomentRelativeToPage: a,
                                        selectorPath: s = ct(r),
                                        sanitizedSelectorPath: r,
                                        scrollX: n[3],
                                        scrollY: n[4]
                                    }), "url" === i && (t = {
                                        action: i,
                                        label: o = dt[i],
                                        timeMomentRelativeToPage: a,
                                        url: n[2],
                                        referrerUrl: n[3]
                                    }), "oc" === i && (t = {
                                        action: i,
                                        label: o = dt[i],
                                        timeMomentRelativeToPage: a,
                                        width: n[2],
                                        height: n[3]
                                    }), "vwomt" === i) t = {
                                    action: i,
                                    label: o = dt[i],
                                    timeMomentRelativeToPage: a,
                                    data: e.substring(e.indexOf(n[1]) + 1, e.length)
                                };
                                return "ko" !== i && "kc" !== i || (t = {
                                    action: i,
                                    label: o = dt[i],
                                    timeMomentRelativeToPage: a
                                }), "ct" !== i && "ti" !== i && "to" !== i && "rsl" !== i && "li" !== i && "esl" !== i && "rh" !== i || (t = {
                                    action: i,
                                    label: o = dt[i],
                                    timeMomentRelativeToPage: a
                                }), "cp" !== i && "sl" !== i || (o = dt[i], s = ct(n[2]), t = {
                                    action: i,
                                    label: o,
                                    timeMomentRelativeToPage: a,
                                    baseNodeSelector: ct(n[3]),
                                    baseOffset: n[4],
                                    extentNodeSelector: ct(n[5]),
                                    extentOffset: n[6],
                                    innerText: n[7],
                                    selectorPath: s
                                }), "vS" !== i && "cE" !== i && "mT" !== i || (t = {
                                    action: i,
                                    label: o = dt[i],
                                    timeMomentRelativeToPage: a
                                }), i.startsWith("er_") && (t = {
                                    action: i,
                                    label: o = dt.er_,
                                    timeMomentRelativeToPage: a
                                }), t
                            }
                        },
                        serializeEvent: function(e) {
                            if (e.action in dt) {
                                var t = "";
                                if ("mc" === e.action || "mr" === e.action || "mm" === e.action || "mu" === e.action) {
                                    t = [e.action, e.timestamp, e.selectorPath, e.elWidth, e.elHeight, e.relX, e.relY, e.docX, e.docY].join(st);
                                    var n = [];
                                    e.isDeadClick && n.push(f.DEAD), e.isRageClick && n.push(f.RAGE), e.isErrorClick && n.push(f.ERROR), n.length && (t += "_{" + n.join(":") + "}")
                                } else "sS" === e.action ? t = [e.action, e.timestamp, e.screenScaleX, e.screenScaleX].join(st) : "tc" === e.action || "is" === e.action ? t = [e.action, e.timestamp, e.scaleX, e.scaleY, e.scrollX, e.scrollY].join(st) : "rs" === e.action ? t = [e.action, e.timestamp, e.width, e.height, e.scrollX, e.scrollY, e.scaleX, e.scaleY, e.screenScaleX, e.screenScaleY, e.pageWidth, e.pageHeight].join(st) : "re" === e.action ? t = [e.action, e.timestamp, e.width, e.height, e.scrollX, e.scrollY].join(st) : "bl" === e.action || "sb" === e.action || "ku" === e.action || "ic" === e.action ? t = [e.action, e.timestamp, e.selectorPath, e.value].join(st) : "fo" === e.action ? t = [e.action, e.timestamp, e.selectorPath].join(st) : "rb" === e.action || "cb" == e.action ? t = [e.action, e.timestamp, e.selectorPath, e.isChecked].join(st) : "sc" === e.action ? t = [e.action, e.timestamp, e.scaleX, e.scaleY, e.scrollX, e.scrollY, e.width, e.height, e.pageWidth, e.pageHeight].join(st) : "es" === e.action ? t = [e.action, e.timestamp, e.selectorPath, e.scrollX, e.scrollY].join(st) : "url" === e.action ? t = [e.action, e.timestamp, e.url, e.referrerUrl].join(st) : "oc" === e.action ? t = [e.action, e.timestamp, e.width, e.height].join(st) : "vwomt" === e.action ? t = [e.action, e.timestamp, e.data].join(st) : "ct" === e.action || "li" === e.action || "rsl" === e.action || "pr" === e.action || "ti" === e.action || "to" === e.action || "ko" === e.action || "kc" === e.action ? t = [e.action, e.timestamp].join(st) : "cp" === e.action || "sl" === e.action ? t = [e.action, e.timestamp, e.path, e.baseNodeSelector, e.baseOffset, e.extentNodeSelector, e.extentOffset, e.innerText].join(st) : "qb" === e.action ? t = [e.action, e.timestamp, e.previousUrl].join(st) : "rh" === e.action ? t = [e.action, e.timestamp, e.innerText, e.path].join(st) : "vS" === e.action || "mT" === e.action || "cE" === e.action ? t = [e.action, e.timestamp, e.value].join(st) : "pU" === e.action && (t = [e.action, e.timestamp].join(st));
                                return t
                            }
                        }
                    },
                    ut = {
                        RECORDING_INITIATED: "rI",
                        PAGE_EXIT: "pageExitEvent",
                        VARIATION_SHOWN: "vS",
                        PAGE_UNLOAD: "pU",
                        METRIC_CONVERTED: "mT",
                        DOM_SUBMIT: "dS",
                        CUSTOM_EVENT: "cE",
                        VWO_VARIATION_SHOWN: "vwo_variationShown",
                        PAGE_VIEW: "vwo_pageView"
                    },
                    ht = function() {},
                    ft, mt = {
                        copy: "cp",
                        cursorThrashed: "ct",
                        pageRefreshed: "pr",
                        quickBack: "qb",
                        selection: "sl",
                        repeatedScrolled: "rsl",
                        repeatedHovered: "rh",
                        leaveIntent: "li",
                        tabIn: "ti",
                        tabOut: "to"
                    },
                    pt = (ft = {}, ft[mt.tabIn] = {
                        action: mt.tabIn,
                        image: "tab-in",
                        name: "Tab In"
                    }, ft[mt.tabOut] = {
                        action: mt.tabOut,
                        image: "tab-out",
                        name: "Tab Out"
                    }, ft[mt.cursorThrashed] = {
                        action: mt.cursorThrashed,
                        image: "cursor-thrasher",
                        name: "Cursor Thrashes"
                    }, ft[mt.repeatedScrolled] = {
                        action: mt.repeatedScrolled,
                        image: "excessive-scroll",
                        name: "Repeated Scroll"
                    }, ft[mt.repeatedHovered] = {
                        action: mt.repeatedHovered,
                        image: "repeated-hovered",
                        name: "Repeated Hover"
                    }, ft[mt.leaveIntent] = {
                        action: mt.leaveIntent,
                        image: "exit-intent",
                        name: "Leave Intent"
                    }, ft.deadClick = {
                        action: "dc",
                        image: "deadclick",
                        value: "11",
                        name: "Dead Click"
                    }, ft.rageClick = {
                        action: "rc",
                        image: "rageclick",
                        value: "12",
                        name: "Rage Click"
                    }, ft.errorClick = {
                        action: "ec",
                        image: "error-click",
                        value: "13",
                        name: "Error Click"
                    }, ft),
                    gt = (vt = ht, e(wt, vt), wt.prototype.process = function(e, t) {
                        var n = this,
                            i = C() - Z.startTime;
                        Math.sqrt(Math.pow(e.movementX, 2) + Math.pow(e.movementY, 2)) < this.MIN_DISTANCE || (this.recentMovements.push({
                            dx: e.movementX,
                            dy: e.movementY,
                            time: i
                        }), this.recentMovements = this.recentMovements.filter(function(e) {
                            return i - e.time <= n.THRASH_INTERVAL
                        }), this.detectOscillations() >= this.THRESHOLD_MOVEMENT && !this.thrashDetected && (this.thrashDetected = !0, t({
                            action: mt.cursorThrashed,
                            timestamp: i
                        }), setTimeout(function() {
                            n.thrashDetected = !1
                        }, this.COOLDOWN_PERIOD)))
                    }, wt.prototype.detectOscillations = function() {
                        for (var e = 0, t = 0, n = 1; n < this.recentMovements.length; n++) {
                            var i = this.recentMovements[n - 1],
                                o = this.recentMovements[n],
                                r = Math.atan2(i.dy, i.dx),
                                a = Math.atan2(o.dy, o.dx) - r;
                            a > Math.PI && (a -= 2 * Math.PI), a < -Math.PI && (a += 2 * Math.PI), t += Math.abs(a), Math.abs(a) > Math.PI / 2 && e++
                        }
                        return t > 4 * Math.PI ? 10 : e
                    }, wt),
                    vt;

                function wt() {
                    var e = null !== vt && vt.apply(this, arguments) || this;
                    return e.recentMovements = [], e.thrashDetected = !1, e.COOLDOWN_PERIOD = 5e3, e.THRASH_INTERVAL = 500, e.THRESHOLD_MOVEMENT = 7, e.MIN_DISTANCE = 60, e
                }
                var _t = (yt = ht, e(Et, yt), Et.prototype.process = function(e, t) {
                        if (void 0 !== VWO.phoenix) {
                            this.event = e;
                            var n = performance.getEntriesByType("navigation")[0],
                                i = window.location.href,
                                o = P.getItem("pr", !0) || "";
                            "reload" === n.type && i === o && t({
                                action: mt.pageRefreshed,
                                timestamp: C() - Z.startTime
                            }), P.setItem("pr", i, !0)
                        }
                    }, Et),
                    yt;

                function Et() {
                    return null !== yt && yt.apply(this, arguments) || this
                }
                var Tt = (St = ht, e(bt, St), bt.createQuickUnit = function(e) {
                        return {
                            url: e,
                            timestamp: C()
                        }
                    }, bt.prototype.process = function(e, t) {
                        if (void 0 !== VWO.phoenix) {
                            var n = window.location.href,
                                i = P.getItem("qb", !0) || [];
                            if (2 === i.length) {
                                var o = i[0],
                                    r = i[1],
                                    a = bt.createQuickUnit(n),
                                    s = a.timestamp - r.timestamp;
                                o.url === a.url && r.url !== n && s < this.threshold && t({
                                    action: mt.quickBack,
                                    previousUrl: F.sanitizeActionData(F.getAnonymizedUrl(r.url)),
                                    timestamp: C() - Z.startTime
                                }), i[0] = R({}, i[1]), i[1] = R({}, a)
                            } else i.push(bt.createQuickUnit(n));
                            P.setItem("qb", i, !0)
                        }
                    }, bt),
                    St;

                function bt() {
                    var e = St.call(this) || this;
                    return e.threshold = 7e3, e
                }
                var Ot = (Rt = ht, e(Ct, Rt), Ct.prototype.process = function(e, t) {
                        this.event = e;
                        var n = window.getSelection();
                        if (n.toString().trim()) {
                            for (var i = n.getRangeAt(0), o = n.baseNode, r = n.baseOffset, a = n.extentOffset, s = n.extentNode, d = F.sanitizeActionData(he(o)), c = F.sanitizeActionData(he(s)), l = [], u = document.createTreeWalker(i.commonAncestorContainer, NodeFilter.SHOW_ELEMENT, {
                                    acceptNode: function(e) {
                                        return i.intersectsNode(e) ? NodeFilter.FILTER_ACCEPT : NodeFilter.FILTER_REJECT
                                    }
                                }), h = n.getRangeAt(0).commonAncestorContainer, f = h.nodeType === Node.TEXT_NODE ? h.parentElement : h; u.nextNode();) {
                                var m = u.currentNode;
                                i.intersectsNode(m) && l.push(m)
                            }
                            l.length || l.push(f);
                            var p = F.sanitizeActionData(he(f)),
                                g = l.filter(function(e) {
                                    return !F.needsMasking(e, Z.Recording.anonymizeKeys)
                                }),
                                v = "";
                            g.forEach(function(e) {
                                0 === e.children.length && (v += e.textContent.trim() + "\n")
                            }), t({
                                action: mt.selection,
                                path: p,
                                baseNodeSelector: d,
                                baseOffset: r,
                                extentNodeSelector: c,
                                extentOffset: a,
                                timestamp: C() - Z.startTime,
                                innerText: F.sanitizeActionData(v)
                            })
                        }
                    }, Ct),
                    Rt;

                function Ct() {
                    return null !== Rt && Rt.apply(this, arguments) || this
                }
                var Nt = (Dt.prototype.getScrollPercent = function() {
                    this.count = 0;
                    var e = document.documentElement.scrollTop || document.body.scrollTop,
                        t = document.documentElement.scrollHeight - document.documentElement.clientHeight;
                    return t <= 0 ? 0 : e / t * 100
                }, Dt.prototype.process = function(e, t) {
                    var n = this.getScrollPercent(),
                        i = C() - Z.startTime;
                    n > this.maxPercentThreshold && "down" !== this.direction ? this.direction = "down" : n < this.minPercentThreshold && "down" === this.direction && (this.direction = "up", this.scrollCount += 1, this.scrollCount >= this.repeatedScrollThreshold && (null === this.lastTriggered || i - this.lastTriggered >= this.cooldownPeriod) && (t({
                        action: mt.repeatedScrolled,
                        timestamp: i
                    }), this.lastTriggered = i, this.resetScrollState()))
                }, Dt.prototype.resetScrollState = function() {
                    this.scrollCount = 0, this.direction = null
                }, Dt);

                function Dt(e, t, n) {
                    void 0 === e && (e = 2), void 0 === t && (t = 10), void 0 === n && (n = 85), this.repeatedScrollThreshold = e, this.minPercentThreshold = n - t, this.maxPercentThreshold = n, this.scrollCount = 0, this.direction = null, this.count = 0, this.cooldownPeriod = 5e3, this.lastTriggered = null
                }
                var It = (At = ht, e(xt, At), xt.prototype.process = function(e, t) {
                        this.event = e, void 0 !== VWO.phoenix && (document.hidden || t({
                            action: mt.tabIn,
                            timestamp: C() - Z.startTime
                        }))
                    }, xt),
                    At;

                function xt() {
                    return null !== At && At.apply(this, arguments) || this
                }
                var kt = (Lt = ht, e(Mt, Lt), Mt.prototype.process = function(e, t) {
                        this.event = e, void 0 === VWO.phoenix || document.hidden && t({
                            action: mt.tabOut,
                            timestamp: C() - Z.startTime
                        })
                    }, Mt),
                    Lt;

                function Mt() {
                    return null !== Lt && Lt.apply(this, arguments) || this
                }

                function Pt(e) {
                    return F.sanitizeActionData(he(e))
                }
                var Vt = (Wt.prototype.process = function(e, t) {
                    var n, i, o, r = this,
                        a = e.composedPath && null !== (n = e.composedPath()) && void 0 !== n && n.length ? e.composedPath()[0] : e.target;
                    if (a && a !== document.body && a !== document.documentElement && a instanceof HTMLElement) {
                        var s = Pt(a);
                        this.hoverDataMap[s] || (this.hoverDataMap[s] = {
                            hoverCount: 0,
                            timer: null,
                            startTime: null,
                            lastTriggered: null,
                            innerText: (null === (o = null === (i = a.innerText) || void 0 === i ? void 0 : i.trim()) || void 0 === o ? void 0 : o.slice(0, Z.ctl)) || ""
                        });
                        var d = this.hoverDataMap[s];
                        d.hoverCount++, 1 === d.hoverCount && (d.startTime = C() - Z.startTime), d.timer || (d.timer = setTimeout(function() {
                            r.resetHoverCount(s)
                        }, this.timeThreshold));
                        var c = C() - Z.startTime,
                            l = c - (d.startTime || 0),
                            u = d.lastTriggered || 0;
                        if (d.hoverCount >= this.hoverThreshold && l >= this.minTimeSpent && c - u >= this.cooldownPeriod) {
                            var h = {
                                action: mt.repeatedScrolled,
                                timestamp: c,
                                innerText: F.sanitizeActionData(d.innerText),
                                path: s
                            };
                            t(h, R(R({}, h), {
                                path: s
                            })), d.lastTriggered = c, this.resetHoverCount(s)
                        }
                    }
                }, Wt.prototype.resetHoverCount = function(e) {
                    var t = this.hoverDataMap[e];
                    t && (t.hoverCount = 0, t.startTime = null, t.timer && clearTimeout(t.timer), t.timer = null)
                }, Wt);

                function Wt(e, t, n, i) {
                    void 0 === e && (e = 5), void 0 === t && (t = 1e4), void 0 === n && (n = 4e3), void 0 === i && (i = 2e4), this.hoverThreshold = e, this.timeThreshold = t, this.minTimeSpent = n, this.cooldownPeriod = i, this.hoverDataMap = {}
                }
                var Ht = (Ut.prototype.attachEventListener = function() {
                    Z.signalsEnabled && VWO._.phoenixMT && this.events.forEach(function(n) {
                        VWO._.phoenixMT.on(n.name, function(e) {
                            var t = R(R({}, e), Z.getCampaigns(!1));
                            e.vwoEventName = n.name, Z.Recording.addNlsData(e, t)
                        }, {
                            syncToDataLayer: !0
                        })
                    })
                }, Ut.prototype.onInit = function() {
                    var t = this;
                    Z.signalsEnabled && (this.pageRefreshed.process(null, function(e) {
                        t.recording.push(lt.serializeEvent(e)), setTimeout(function() {
                            VWO._.phoenixMT.trigger(window.VWO._.EventsEnum.PAGE_REFRESHED, R({}, e))
                        }, 0)
                    }), this.quickBack.process(null, function(e) {
                        t.recording.push(lt.serializeEvent(e)), setTimeout(function() {
                            VWO._.phoenixMT.trigger(window.VWO._.EventsEnum.QUICK_BACK, R({}, e))
                        }, 0)
                    }))
                }, Ut.prototype.onMouseMove = function(e) {
                    var t = this;
                    Z.signalsEnabled && this.cursorThrashed.process(e, function(e) {
                        VWO._.phoenixMT.trigger(window.VWO._.EventsEnum.CURSOR_THRASHED, R({}, e)), t.recording.push(lt.serializeEvent(e))
                    })
                }, Ut.prototype.onMouseOver = function(e) {
                    var n = this;
                    Z.signalsEnabled && this.repeatedHovered.process(e, function(e, t) {
                        n.recording.push(lt.serializeEvent(R(R({}, t || e), {
                            action: mt.repeatedHovered
                        }))), VWO._.phoenixMT.trigger(window.VWO._.EventsEnum.REPEATED_HOVERED, R({}, e))
                    })
                }, Ut.prototype.onCopy = function(e) {
                    var t = this;
                    Z.signalsEnabled && this.copy.process(e, function(e) {
                        t.recording.push(lt.serializeEvent(R(R({}, e), {
                            action: mt.copy
                        }))), VWO._.phoenixMT.trigger(window.VWO._.EventsEnum.COPY, R({}, e))
                    })
                }, Ut.prototype.onSelection = function(e) {
                    var t = this;
                    Z.signalsEnabled && this.selection.process(e, function(e) {
                        t.recording.push(lt.serializeEvent(e)), VWO._.phoenixMT.trigger(window.VWO._.EventsEnum.SELECTION, R({}, e))
                    })
                }, Ut.prototype.onScroll = function() {
                    var t = this;
                    Z.signalsEnabled && this.repeatedScrolled.process(void 0, function(e) {
                        t.recording.push(lt.serializeEvent(e)), VWO._.phoenixMT.trigger(window.VWO._.EventsEnum.REPEATED_SCROLLED, R({}, e))
                    })
                }, Ut.prototype.onVisibilityChange = function(e) {
                    var t = this;
                    Z.signalsEnabled && (this.tabIn.process(e, function(e) {
                        t.recording.push(lt.serializeEvent(e)), setTimeout(function() {
                            VWO._.phoenixMT.trigger(window.VWO._.EventsEnum.TAB_IN, R({}, e))
                        }, 500)
                    }), this.tabOut.process(e, function(e) {
                        t.recording.push(lt.serializeEvent(e)), setTimeout(function() {
                            VWO._.phoenixMT.trigger(window.VWO._.EventsEnum.TAB_OUT, R({}, e))
                        }, 1e3)
                    }))
                }, Ut);

                function Ut(e) {
                    var i = this;
                    if (this.recording = e, this.events = [{
                            name: window.VWO._.EventsEnum.CURSOR_THRASHED
                        }, {
                            name: window.VWO._.EventsEnum.PAGE_REFRESHED
                        }, {
                            name: window.VWO._.EventsEnum.QUICK_BACK
                        }, {
                            name: window.VWO._.EventsEnum.COPY
                        }, {
                            name: window.VWO._.EventsEnum.SELECTION
                        }, {
                            name: window.VWO._.EventsEnum.REPEATED_SCROLLED
                        }, {
                            name: window.VWO._.EventsEnum.REPEATED_HOVERED
                        }, {
                            name: window.VWO._.EventsEnum.LEAVE_INTENT
                        }, {
                            name: window.VWO._.EventsEnum.TAB_OUT
                        }, {
                            name: window.VWO._.EventsEnum.TAB_IN
                        }], this.cursorThrashed = new gt, this.pageRefreshed = new _t, this.quickBack = new Tt, this.copy = new Ot, this.selection = new Ot, this.repeatedScrolled = new Nt, this.repeatedHovered = new Vt, this.tabIn = new It, this.tabOut = new kt, this.disableFeature = !0, void 0 !== VWO.phoenix) {
                        var o = 0;
                        VWO.phoenix('on("${{1}}", "${{2}}")', null, {
                            captureGroups: ["vwo_leaveIntent", function(e) {
                                if (Z.signalsEnabled) {
                                    var t = C();
                                    if (!(t - o < 5e3)) {
                                        o = t;
                                        var n = {
                                            action: mt.leaveIntent,
                                            timestamp: t - Z.startTime
                                        };
                                        VWO._.phoenixMT.trigger(window.VWO._.EventsEnum.LEAVE_INTENT, R({}, e)), i.recording.push(lt.serializeEvent(n))
                                    }
                                }
                            }]
                        })
                    }
                }
                var zt = {
                        AB_TESTING: "ABTesting",
                        SPLIT_URL: "SplitURL",
                        MVT: "MVT",
                        PERSONALIZE: "Personalize",
                        WEB_ROLLOUT: "WebRollout",
                        HEATMAP: "ANALYZE_HEATMAP",
                        RECORDING: "ANALYZE_RECORDING"
                    },
                    Ft = (jt.prototype.pushFunnelData = function() {
                        var e = window.VWO._.phoenixMT.eventHistory.vwo_insightsFunnel;
                        if (e) {
                            for (var t = 0; t < e.length; t++) {
                                var n = e[t].data.split("_"),
                                    i = n[0],
                                    o = n[2];
                                this.addMTEventToRecording(i, o, !0)
                            }
                            e.length = 0
                        }
                    }, jt.prototype.attachEventListener = function() {
                        var t = this;
                        VWO._.phoenixMT.on(window.VWO._.EventsEnum.DOM_SUBMIT, function(e) {
                            t.data360EventsCaptured(e)
                        }, {
                            syncToDataLayer: !0
                        }), VWO._.phoenixMT.on(window.VWO._.EventsEnum.PAGE_UNLOAD, function(e) {
                            t.data360EventsCaptured(e)
                        }, {
                            syncToDataLayer: !0
                        })
                    }, jt.prototype.addMTEventToRecording = function(e, t, n) {
                        var i, o = {
                                name: window._vwo_exp[e].name,
                                id: e,
                                goalId: t,
                                campaignType: "TESTING"
                            },
                            r = null === (i = window._vwo_exp[e].metrics.find(function(e) {
                                return e.id == t
                            })) || void 0 === i ? void 0 : i.metricId;
                        if (void 0 !== r && 0 !== r && (o.globalMetricID = r), "INSIGHTS_METRIC" === window._vwo_exp[e].type && (o.campaignType = "INSIGHTS_METRIC"), "INSIGHTS_FUNNEL" !== window._vwo_exp[e].type || (o.campaignType = "INSIGHTS_FUNNEL", n)) {
                            var a = JSON.stringify(o);
                            a = F.sanitizeActionData(a);
                            var s = {
                                action: ut.METRIC_CONVERTED,
                                timestamp: C() - Z.startTime,
                                value: a
                            };
                            0 === Z.startTime && (s.timestamp = 0), Z.data360EventIndex++, this.recording.push(lt.serializeEvent(s))
                        }
                    }, jt.prototype.addVSEventToRecording = function(e, t) {
                        var n = window._vwo_exp[e],
                            i = window._vwo_exp[e].type;
                        if (-1 !== this.AllCampaignsType.indexOf(window._vwo_exp[e].type)) {
                            i = window._vwo_exp[e].iType ? zt.PERSONALIZE : "DEPLOY" === window._vwo_exp[e].orgType ? zt.WEB_ROLLOUT : "VISUAL" === window._vwo_exp[e].type ? zt.MVT : "SPLIT_URL" === window._vwo_exp[e].type ? zt.SPLIT_URL : zt.AB_TESTING;
                            var o = {
                                    name: n.name,
                                    id: e,
                                    variationName: n.comb_n[t],
                                    variation: t,
                                    campaignType: i
                                },
                                r = JSON.stringify(o);
                            r = F.sanitizeActionData(r);
                            var a = {
                                action: ut.VARIATION_SHOWN,
                                timestamp: C() - Z.startTime,
                                value: r
                            };
                            0 === Z.startTime && (a.timestamp = 0), this.recording.push(lt.serializeEvent(a))
                        }
                    }, jt.prototype.addPUEventToRecording = function() {
                        var e = {
                            action: ut.PAGE_UNLOAD,
                            timestamp: C() - Z.startTime
                        };
                        0 === Z.startTime && (e.timestamp = 0), this.recording.push(lt.serializeEvent(e))
                    }, jt.prototype.addDOMSUBMITEventToRecording = function() {
                        var e = {
                            action: ut.DOM_SUBMIT,
                            timestamp: C() - Z.startTime
                        };
                        0 === Z.startTime && (e.timestamp = 0), this.recording.push(lt.serializeEvent(e))
                    }, jt.prototype.data360EventsCaptured = function(o) {
                        var e, r = this,
                            t = null === (e = o._vwo) || void 0 === e ? void 0 : e.eventDataConfig;
                        if (t && t[Object.keys(t)[0]].vwoMeta) {
                            var n = o._vwo.eventDataConfig[Object.keys(o._vwo.eventDataConfig)[0]].vwoMeta.metric;
                            0 < Object.keys(n).length && Object.keys(n).forEach(function(e) {
                                var i = e.split("_")[1];
                                n[e].forEach(function(e) {
                                    var t, n = e.split("_")[1];
                                    (null === (t = o._vwo) || void 0 === t ? void 0 : t.syncEventData.vwoEventName) === window.VWO._.EventsEnum.PAGE_UNLOAD ? r.addPUEventToRecording() : o.vwoEventName === window.VWO._.EventsEnum.DOM_SUBMIT && r.addDOMSUBMITEventToRecording(), r.addMTEventToRecording(i, n)
                                })
                            })
                        }
                    }, jt.prototype.onInit = function() {
                        var o = this;
                        Z.enableEventListeners = !0, window.VWO.phoenix('store.getters.getHistoryEvents("${{1}}")', null, {
                            captureGroups: ["vwo_goalConverted"]
                        }).then(function(e) {
                            for (var t = Z.data360EventIndex; t < e.length; t++) {
                                var n = e[t].props.campaignId,
                                    i = e[t].goalId;
                                o.addMTEventToRecording(n, i)
                            }
                            Z.data360EventIndex = e.length
                        }), this.pushFunnelData()
                    }, jt);

                function jt(e) {
                    var f = this;
                    if (this.AllCampaignsType = ["VISUAL_AB", "DEPLOY", "VISUAL", "SPLIT_URL"], this.recording = e, void 0 !== VWO.phoenix) {
                        var t = window._vwo_exp_ids || [];
                        VWO._.phoenixMT && VWO._.phoenixMT.on("vwo_insightsFunnel", function() {
                            f.pushFunnelData()
                        }, {
                            syncToDataLayer: !0
                        }), 0 < t.length && t.forEach(function(e) {
                            var t = window._vwo_exp[e];
                            t.ready && t.combination_chosen && t.isFirst && f.addVSEventToRecording(e, t.combination_chosen)
                        }), VWO.phoenix('on("${{1}}", "${{2}}")', null, {
                            captureGroups: ["*", function(e) {
                                var t, n, i;
                                if (Z.enableEventListeners)
                                    if (e.name === ut.VWO_VARIATION_SHOWN) {
                                        if (!e.props.isFirst) return;
                                        var o = e.props.id;
                                        f.addVSEventToRecording(o, e.props.variation)
                                    } else if ("vwo_goalConverted" === e.name) {
                                    o = e.props.campaignId;
                                    var r = e.goalId,
                                        a = VWO._.goalsToBeConvertedSynchronously;
                                    if (null != a && 0 !== Object.keys(a).length || (a = VWO._.libUtils.updateGoalsKind(window._vwo_exp)), a && a[o] && a[o][r]) return;
                                    f.addMTEventToRecording(o, r)
                                } else if (e.isCustomEvent) {
                                    if ("vwo_debugLogs" === e.name) return;
                                    var s = {},
                                        d = ["isCustomEvent", "page", "name", "VWO"],
                                        c = null === (i = null === (n = null === (t = window.VWO._) || void 0 === t ? void 0 : t.allSettings) || void 0 === n ? void 0 : n.dataStore) || void 0 === i ? void 0 : i.events;
                                    if (!c || void 0 === c[e.name]) return;
                                    for (var l in e) - 1 === d.indexOf(l) && (s[l] = e[l]);
                                    var u = JSON.stringify(s);
                                    u = F.sanitizeActionData(u);
                                    var h = {
                                        action: ut.CUSTOM_EVENT,
                                        timestamp: C() - Z.startTime,
                                        value: F.sanitizeActionData(e.name) + "_" + u
                                    };
                                    0 === Z.startTime && (h.timestamp = 0), f.recording.push(lt.serializeEvent(h))
                                }
                            }]
                        })
                    }
                }
                var Xt = v,
                    Bt = _.get("Array");

                function Yt(e) {
                    if (e instanceof HTMLAnchorElement && e.hasAttribute("href")) return 1;
                    var t = e.parentElement;
                    return !!(t instanceof HTMLAnchorElement && t.hasAttribute("href"))
                }
                var qt = (Kt.prototype.add = function(e, t, n) {
                    Yt(e) || this.deadQueue.push(R({
                        index: n,
                        target: e
                    }, t))
                }, Kt.prototype.isDeadClickElementNode = function(e) {
                    this.count++;
                    var t = e;
                    if (!t || !t.nodeName || !t.nodeName.toUpperCase) return !1;
                    if (t._vwo_nd && !t.disabled) return !1;
                    switch (t.nodeName.toUpperCase()) {
                        case "LABEL":
                            if (t.hasAttribute("for")) {
                                var n = t.getAttribute("for");
                                if (document.getElementById(n)) return !1
                            } else if (0 < Xt(t).find("input,select,textarea,option").length) return !1
                    }
                    return !0
                }, Kt.prototype.isDeadClickElement = function(e) {
                    var t = this.popElementByIndex(e);
                    if (t) return this.isDeadClickElementNode(t.target)
                }, Kt.prototype.process = function(e) {
                    var t = (e || {}).mutatedNode;
                    if (!t || F.isElementInViewport(t))
                        for (var n = this.deadQueue, i = C() - Z.startTime, o = n.length - 1; 0 <= o && n[o].timestamp >= i - 500; o--) n.splice(o, 1)
                }, Kt.prototype.popElementByIndex = function(e) {
                    var t;
                    if (e.index === (null === (t = this.deadQueue[0]) || void 0 === t ? void 0 : t.index)) {
                        var n = this.deadQueue[0];
                        return this.deadQueue.shift(), n
                    }
                }, Kt);

                function Kt() {
                    this.deadQueue = new Bt, this.count = 0
                }
                var Gt = v,
                    Jt, $t, Qt;

                function Zt(i) {
                    var o, r;
                    window.history && (o = window.history, r = o.pushState || function() {}, de.addOverrideState(o, "pushState"), o.pushState = function() {
                        for (var e = [], t = 0; t < arguments.length; t++) e[t] = arguments[t];
                        var n = r.apply(o, e);
                        return i({
                            state: e[0]
                        }), n
                    }, window.addEventListener ? de.addEventListener(window, "popstate", i, !1) : window.attachEvent && window.attachEvent("onpopstate", i))
                }

                function en() {
                    Z.recordingData.totals.ocs++;
                    var e = Z.getScreenDimensions(),
                        t = {
                            action: "oc",
                            timestamp: C() - Z.startTime,
                            width: e.width,
                            height: e.height
                        };
                    this.recording.push(lt.serializeEvent(t))
                }

                function tn(e) {
                    var t = setTimeout(en.bind(e), 10);
                    de.pushTimers(t, "timeout")
                }

                function nn(e) {
                    if (this.isMobile = Z.isMobile(), this.isMobile) {
                        var t = {
                            action: "focusin" == e.type ? "ko" : "kc",
                            timestamp: C() - Z.startTime
                        };
                        this.recording.push(lt.serializeEvent(t))
                    }
                }

                function on(e) {
                    if (document.contains(e)) return document;
                    for (var t = e; t.parentNode;) t = t.parentNode;
                    return t
                }

                function rn() {
                    var e = A.get("Array");
                    this.heatmap = {
                        eventsLength: 0,
                        maxClicksRecorded: window._vwo_clicks || 10,
                        clicks: new e
                    }, this.recording = new e, this.signal = new Ht(this.recording), Z.data360EventsEnabled && (this.data360Events = new Ft(this.recording)), this.deadClickManager = new qt, this.index = {
                        recording: 0,
                        clicks: 0,
                        heatmap: 0
                    }, this.intervals = new e, this.anonymizeKeys = !0, this.clickDelay = {
                        page: 0,
                        link: 380
                    }, this.tags = new h("Array"), this.totals = {
                        movements: 0,
                        clicks: 0,
                        keyPresses: 0,
                        scroll: 0
                    }, this.window = {
                        width: 0,
                        height: 0
                    }, this.mouse = {
                        curMove: {
                            el: {},
                            width: 0,
                            height: 0,
                            relX: 0,
                            relY: 0,
                            docX: 0,
                            docY: 0
                        },
                        lastMove: {
                            docX: 0,
                            docY: 0
                        },
                        curDown: {
                            tag: "",
                            index: 0
                        }
                    }, this.isMobile = Z.isMobile(), this.devicePixelRatio = 0, this.lastClickData = null
                }

                function an() {
                    for (var e = document.cookie.split(/; ?/), t = {}, n = 0; n < e.length; n++) {
                        var i = e[n].split("="),
                            o = i[0],
                            r = i[1];
                        try {
                            0 !== o.indexOf("_vwo") && 0 !== o.indexOf("_vis_opt") || (t[o] = r)
                        } catch (e) {}
                    }
                    return t
                }

                function sn() {
                    var e = window._vwo_evq;
                    if (e) {
                        var t = Z.Recording.index.evq ? Z.Recording.index.evq - 1 : 0,
                            n = e.slice(t);
                        return Z.Recording.index.evq = e.length, n
                    }
                }
                rn.prototype.addInitialHTML = function(e) {
                    if (Z.r) {
                        var t = A.get("JSON"),
                            n = Z.GetHtml.html;
                        n.lvs = {
                            c: VWO.v,
                            o: VWO.v_o,
                            s: VWO.v_s
                        }, n.idleToAction = Z.saveNewRecordingInitiatedOnce, e.html = t.stringify(n)
                    }
                }, rn.prototype.getRelativeCoord = function(e) {
                    if (this.isMobile) {
                        var t = (Jt = Jt || Gt('<span style="position:absolute;top:0;left:-100px"></span>').appendTo("body")).offset();
                        e.pageY += t.top, e.pageX += t.left - -100
                    }
                    return {
                        x: e.pageX - e.offsetX,
                        y: e.pageY - e.offsetY
                    }
                }, rn.prototype.addElementScrollData = N(function(e) {
                    this.recording.push(e)
                }, Z.sDT), rn.prototype.sendElementScrollData = function(e) {
                    var t = F.getTargetNode(e),
                        n = {
                            action: "es",
                            selectorPath: F.sanitizeActionData(he(t)),
                            timestamp: C() - Z.startTime,
                            scrollX: Gt(t).scrollLeft(),
                            scrollY: Gt(t).scrollTop()
                        };
                    this.deadClickManager.process(), this.addElementScrollData(lt.serializeEvent(n))
                }, rn.prototype.loadEventListeners = function() {
                    var d = this,
                        i = this;
                    de.addJqEventListener(Gt(document), "on", "focusin", nn.bind(this), "select, textarea, input[type=text], input[type=date], input[type=password], input[type=email], input[type=number]"), de.addJqEventListener(Gt(document), "on", "focusout", nn.bind(this), "select, textarea, input[type=text], input[type=date], input[type=password], input[type=email], input[type=number]");
                    var e = window.navigator.userAgent;
                    e.match(/safari/i) && !e.match(/chrome/i) || window._vwo_acc_id < 5e5 ? de.addEventListener(Gt(window), "on", "orientationchange", tn.bind(this, i)) : de.addEventListener(window.screen.orientation, "change", tn.bind(this, i)), de.addJqEventListener(Gt(document), "on", "mouseleave", at.sendRecordingData.bind(at));

                    function a() {
                        var e = Z.getScale(),
                            t = Z.getScroll(),
                            n = {
                                action: "tc",
                                timestamp: C() - Z.startTime,
                                scaleX: e.x,
                                scaleY: e.y,
                                scrollX: t.x,
                                scrollY: t.y
                            };
                        i.recording.push(lt.serializeEvent(n))
                    }

                    function c(e, t) {
                        void 0 === t && (t = !1);
                        var n = F.getTargetNode(e),
                            i = "touchend" === e.type,
                            o = window.vwo_$(n);
                        if (!F.shouldIgnoreElement(n)) {
                            var r, a, s = Gt(n).offset(),
                                d = window.vwo_$(n);
                            if ("undefined" != typeof ShadowRoot && (n.getRootNode ? n.getRootNode() instanceof ShadowRoot : on(n) instanceof ShadowRoot) || Z.GetHtml.html.isAuraSite && n.nlsParent) {
                                var c = n.getBoundingClientRect();
                                s = {
                                    top: c.top + document.body.scrollTop,
                                    left: c.left + document.body.scrollLeft
                                }
                            }
                            if (s) {
                                if (this.mouse.curDown.tag = n.nodeName, this.mouse.curDown.index = Gt(n.nodeName).index(n), i) {
                                    var l = e.changedTouches[0];
                                    l && (r = l.pageX, a = l.pageY)
                                } else r = e.pageX, a = e.pageY;
                                for (var u = A.get("Math"), h = this.getRelativeCoord({
                                        pageX: r,
                                        offsetX: s.left,
                                        pageY: a,
                                        offsetY: s.top
                                    }), f = C() - Z.startTime, m = n, p = {
                                        eventUuid: e.eventUuid,
                                        event: e,
                                        action: "mc",
                                        timestamp: f,
                                        selectorPath: F.sanitizeActionData(he(n)),
                                        elWidth: fe.getTrueWidth(n),
                                        elHeight: fe.getTrueHeight(n),
                                        relX: u.floor(h.x),
                                        relY: u.floor(h.y),
                                        docX: this.mouse.curMove.docX,
                                        docY: this.mouse.curMove.docY
                                    }; d.width() <= 0 && d.height() <= 0 && d.length;) d = d.parent();
                                if (d && o && d.length && o.length) {
                                    var g = u.floor(h.x),
                                        v = u.floor(h.y),
                                        w = this.mouse.curMove.docX,
                                        _ = this.mouse.curMove.docY,
                                        y = fe.getRelativeStats(o, d, g, v, w, _);
                                    p = {
                                        eventUuid: e.eventUuid,
                                        event: e,
                                        action: "mc",
                                        timestamp: f,
                                        selectorPath: F.sanitizeActionData(he(d[0])),
                                        elWidth: y.correctedTargetWidth,
                                        elHeight: y.correctedTargetHeight,
                                        relX: u.floor(y.correctedTargetOffsetX),
                                        relY: u.floor(y.correctedTargetOffsetY),
                                        docX: y.correctedTargetPageX,
                                        docY: y.correctedTargetPageY
                                    }, m = d[0]
                                }
                                var E = m && m.innerText ? m.innerText.trim() : "";
                                if (E.length && !F.needsMasking(e.target, Z.Recording.anonymizeKeys) && (E = F.sanitizeActionData(E.slice(0, Z.ctl)), p.innerText = E), Z.GetHtml.html.isAuraSite && -1 < p.selectorPath.indexOf("SLOT") && (Z.selectorStore = Z.selectorStore || {}, Z.selectorStore[p.selectorPath] = m), 1 == e.which || "touchend" === e.type) {
                                    var T = R({}, p);
                                    if ((85e4 < window._vwo_acc_id || 708874 === window._vwo_acc_id) && (T = R(R({}, p), {
                                            elWidth: fe.getOuterWidth(m),
                                            elHeight: fe.getOuterHeight(m)
                                        })), VWO._ && (T.eTime = VWO._.commonUtil.getCurrentTimestamp()), this.lastClickData = p, t || D.data360migrated()) {
                                        if (t) {
                                            O = this.heatmap.clicks.length - Z.Recording.index.heatmap;
                                            this.deadClickManager.add(m, p, O), this.heatmap.clicks.push(T), this.recording.push(T);
                                            var S = Z.Recording.getClickProps(T, !0),
                                                b = Z.Recording.getClickProps(T);
                                            (S || b) && Z.Recording.addNlsData(e, R(R({}, S), b))
                                        }
                                    } else {
                                        var O = this.heatmap.clicks.length - Z.Recording.index.heatmap;
                                        this.deadClickManager.add(m, p, O), this.heatmap.clicks.push(T), this.recording.push(T)
                                    }
                                }
                                D.pageExitHandling() && m && "A" === m.nodeName ? (at.sendRecordingData(!1, null, {
                                    mc: lt.serializeEvent(p)
                                }), this.lastClickData = null) : (Z.sRD = Z.sRD || N(at.sendRecordingData.bind(at), Z.cDT), Z.sRD())
                            }
                        }
                    }
                    de.addEventListener(document, "touchstart", function(e) {
                        2 === e.touches.length && a()
                    }.bind(this), !0), de.addEventListener(document, "touchmove", function(e) {
                        2 === e.touches.length && a();
                        var t = F.getTargetNode(e);
                        if (t) {
                            t.vwoVbaTm = 1;
                            var n = setTimeout(function() {
                                t.vwoVbaTm = 0
                            }, 1e3);
                            de.pushTimers(n, "timeout")
                        }
                    }.bind(this), !0), de.addEventListener(document, "touchend", function() {
                        for (var e = [], t = 0; t < arguments.length; t++) e[t] = arguments[t];
                        var n = e[0];
                        if (2 === n.touches.length) {
                            var i = setTimeout(a, 600);
                            de.pushTimers(i, "timeout")
                        }
                        var o = F.getTargetNode(n);
                        if (o) {
                            o.vwoVbaTe = 1;
                            var r = setTimeout(function() {
                                o.vwoVbaTe = 0
                            }, 1e3);
                            de.pushTimers(r, "timeout"), o.vwoVbaTm || c.apply(this, e.concat(0)), o.vwoVbaTm = 0
                        }
                    }.bind(this), !0), D.data360migrated() && (VWO._.phoenixMT.on(window.VWO._.EventsEnum.DOM_CLICK, function(e) {
                        var t, n, i, o, r, a, s;
                        e._vwo && e._vwo.nls || null !== (t = e._vwo) && void 0 !== t && t.isDeadClick || null !== (n = e._vwo) && void 0 !== n && n.isRageClick || null !== (i = e._vwo) && void 0 !== i && i.isErrorClick || ((o = e).eventUuid = o.eventUuid || (null === (s = null === (a = null === (r = window.VWO.modules) || void 0 === r ? void 0 : r.utils) || void 0 === a ? void 0 : a.libUtils) || void 0 === s ? void 0 : s.generateUUID()), d.data360Events && d.data360Events.data360EventsCaptured(e), c.apply(d, [e, !0]))
                    }, {
                        syncToDataLayer: !0
                    }), this.signal.attachEventListener(), this.data360Events && this.data360Events.attachEventListener());

                    function t() {
                        Z.Recording.deadClickManager.process(), i.lastClickData && Z.htmlRequestSuccess && (Se.useBeacon = !0, D.pageExitHandling() ? (Re.deQueueRequest(), Object.keys(Se.workerMessages).forEach(function(e) {
                            var t = Se.workerMessages[e],
                                n = t.data,
                                i = t.options;
                            at.sendData(n, i)
                        })) : at.sendRecordingData(!1, null, {
                            mc: lt.serializeEvent(i.lastClickData)
                        }), i.lastClickData = null, Se.useBeacon = !1)
                    }
                    var n = N(function(e) {
                            d.signal.onMouseOver(e)
                        }, 200),
                        o = N(function() {
                            this.signal.onSelection()
                        }, 300),
                        r = N(function() {
                            var e = Z.getScale(),
                                t = Z.getScroll(),
                                n = Z.getDimensions(),
                                i = {
                                    action: "sc",
                                    timestamp: C() - Z.startTime,
                                    scaleX: e.x,
                                    scaleY: e.y,
                                    scrollX: t.x,
                                    scrollY: t.y,
                                    width: n.width,
                                    height: n.height,
                                    pageWidth: Gt("html").width(),
                                    pageHeight: Math.max(Gt("html").height(), Gt("html")[0].scrollHeight)
                                };
                            this.recording.push(lt.serializeEvent(i))
                        }.bind(i), Z.sDT);
                    de.addEventListener(window, "resize", function() {
                        if (this.devicePixelRatio === window.devicePixelRatio) {
                            var e = Z.getScale(),
                                t = Z.getScreenScale(),
                                n = Z.getDimensions(),
                                i = Z.getScroll(),
                                o = {
                                    action: this.isMobile ? "rs" : "re",
                                    timestamp: C() - Z.startTime,
                                    width: n.width,
                                    height: n.height,
                                    scrollX: i.x,
                                    scrollY: i.y,
                                    scaleX: this.isMobile ? e.x : "",
                                    scaleY: this.isMobile ? e.y : "",
                                    screenScaleX: this.isMobile ? t.x : "",
                                    screenScaleY: this.isMobile ? t.y : "",
                                    pageWidth: this.isMobile ? Gt("html").width() : "",
                                    pageHeight: this.isMobile ? Gt("html").height() : ""
                                };
                            this.recording.push(lt.serializeEvent(o))
                        } else this.devicePixelRatio = window.devicePixelRatio
                    }.bind(this)), de.addEventListener(document, "mousemove", function(e) {
                        var t = F.getTargetNode(e),
                            n = Gt(t).offset();
                        if ("undefined" != typeof ShadowRoot && (t.getRootNode ? t.getRootNode() instanceof ShadowRoot : on(t) instanceof ShadowRoot) || Z.GetHtml.html.isAuraSite && t.nlsParent) {
                            var i = t.getBoundingClientRect();
                            n = {
                                top: i.top + document.body.scrollTop,
                                left: i.left + document.body.scrollLeft
                            }
                        }
                        if (n) {
                            var o = this.getRelativeCoord({
                                    pageX: e.pageX,
                                    offsetX: n.left,
                                    pageY: e.pageY,
                                    offsetY: n.top
                                }),
                                r = A.get("Math");
                            this.mouse.curMove.el = t, this.mouse.curMove.width = fe.getTrueWidth(t), this.mouse.curMove.height = fe.getTrueHeight(t), this.mouse.curMove.relX = r.floor(o.x), this.mouse.curMove.relY = r.floor(o.y), this.mouse.curMove.docX = e.pageX, this.mouse.curMove.docY = e.pageY, this.signal.onMouseMove(e)
                        }
                    }.bind(this)), de.addEventListener(document, "mouseover", n.bind(this)), de.addEventListener(document, "copy", function(e) {
                        d.signal.onCopy(e)
                    }.bind(this)), de.addEventListener(document, "scroll", function() {
                        d.signal.onScroll(), d.deadClickManager.process()
                    }.bind(this)), de.addEventListener(document, "selectionchange", o.bind(this)), de.addEventListener(document, "visibilitychange", function(e) {
                        d.signal.onVisibilityChange(e)
                    }.bind(this)), de.addJqEventListener(Gt(window), "on", "scroll", r), de.addJqEventListener(Gt(window), "on", "beforeunload", t), D.pageExitHandling() && window.VWO._.phoenixMT.on(ut.PAGE_EXIT, t), "function" == typeof document.addEventListener ? de.addEventListener(document.body, "scroll", i.sendElementScrollData.bind(i), !0) : de.addJqEventListener(Gt("*"), "on", "scroll", i.sendElementScrollData.bind(i)), de.addEventListener(document, "focus", function(e) {
                        var t = F.getTargetNode(e),
                            n = {
                                action: "fo",
                                timestamp: C() - Z.startTime,
                                selectorPath: F.sanitizeActionData(he(t))
                            };
                        this.recording.push(lt.serializeEvent(n))
                    }.bind(this), !0), de.addEventListener(document, "blur", function(e) {
                        var t = F.getTargetNode(e),
                            n = this.handleProtected(t),
                            i = {
                                action: "bl",
                                timestamp: C() - Z.startTime,
                                selectorPath: F.sanitizeActionData(he(t)),
                                value: n
                            };
                        this.recording.push(lt.serializeEvent(i))
                    }.bind(this), !0), Zt(function() {
                        Z.Recording.deadClickManager.process(), Z.formAnalysis && Z.formAnalysis.resetFormObservers();
                        var e = {
                            action: "url",
                            timestamp: C() - Z.startTime,
                            url: encodeURIComponent(location.href),
                            referrerUrl: encodeURIComponent(this.currentUrl)
                        };
                        this.recording.push(lt.serializeEvent(e)), this.previousUrl = this.currentUrl, this.currentUrl = location.href, Z.enableEventListeners = !1
                    }.bind(this)), de.addEventListener(document.body, "mousedown", function() {
                        for (var e = [], t = 0; t < arguments.length; t++) e[t] = arguments[t];
                        var n = e[0],
                            i = n.target;
                        2 !== n.button ? i.vwoVbaTe ? i.vwoVbaTe = 0 : c.apply(this, e) : function(e) {
                            var t = F.getTargetNode(e),
                                n = Gt(t).offset();
                            if ("undefined" != typeof ShadowRoot && (t.getRootNode ? t.getRootNode() instanceof ShadowRoot : on(t) instanceof ShadowRoot) || Z.GetHtml.html.isAuraSite && t.nlsParent) {
                                var i = t.getBoundingClientRect();
                                n = {
                                    top: i.top + document.body.scrollTop,
                                    left: i.left + document.body.scrollLeft
                                }
                            }
                            if ("HTML" !== t.nodeName && n) {
                                var o = this.getRelativeCoord({
                                        pageX: e.pageX,
                                        offsetX: n.left,
                                        pageY: e.pageY,
                                        offsetY: n.top
                                    }),
                                    r = A.get("Math"),
                                    a = {
                                        action: "mr",
                                        timestamp: C() - Z.startTime,
                                        selectorPath: F.sanitizeActionData(he(t)),
                                        elWidth: fe.getTrueWidth(t),
                                        elHeight: fe.getTrueHeight(t),
                                        relX: r.floor(o.x),
                                        relY: r.floor(o.y),
                                        docX: this.mouse.curMove.docX,
                                        docY: this.mouse.curMove.docY
                                    };
                                if (this.recording.push(lt.serializeEvent(a)), "submit" !== Gt(t).attr("type")) {
                                    setTimeout(function() {
                                        at.sendRecordingData()
                                    }, 1);
                                    for (var s = "A" === t.nodeName ? this.clickDelay.link : this.clickDelay.page, d = A.get("Date"), c = new d, l = c.getTime() + s; c.getTime() < l;) c = new d
                                }
                            }
                        }.bind(this)(n)
                    }.bind(this), !0), de.addEventListener(document, "mouseup", function(e) {
                        var t = F.getTargetNode(e),
                            n = Gt(t).offset(),
                            i = Gt(t.nodeName).index(t);
                        if ("HTML" !== t.nodeName && n) {
                            var o;
                            if (t.nodeName === this.mouse.curDown.tag && i === this.mouse.curDown.index) switch (e.button) {
                                case 0:
                                case 1:
                                default:
                                    o = "mc";
                                    break;
                                case 2:
                                    o = "mr"
                            } else o = "mu";
                            if ("mc" !== o && "mr" !== o) {
                                var r = this.getRelativeCoord({
                                        pageX: e.pageX,
                                        offsetX: n.left,
                                        pageY: e.pageY,
                                        offsetY: n.top
                                    }),
                                    a = A.get("Math"),
                                    s = {
                                        action: o,
                                        timestamp: C() - Z.startTime,
                                        selectorPath: F.sanitizeActionData(he(t)),
                                        elWidth: fe.getTrueWidth(t),
                                        elHeight: fe.getTrueHeight(t),
                                        relX: a.floor(r.x),
                                        relY: a.floor(r.y),
                                        docX: this.mouse.curMove.docX,
                                        docY: this.mouse.curMove.docY
                                    };
                                if (this.recording.push(lt.serializeEvent(s)), "submit" !== Gt(t).attr("type")) {
                                    setTimeout(function() {
                                        at.sendRecordingData()
                                    }, 1);
                                    for (var d = "A" === t.nodeName ? this.clickDelay.link : this.clickDelay.page, c = A.get("Date"), l = new c, u = l.getTime() + d; l.getTime() < u;) l = new c
                                }
                            }
                        }
                    }.bind(this)), de.addEventListener(document, "keyup", function(e) {
                        var t = F.getTargetNode(e),
                            n = this.handleProtected(t),
                            i = {
                                action: "ku",
                                timestamp: C() - Z.startTime,
                                selectorPath: F.sanitizeActionData(he(t)),
                                value: n
                            };
                        this.recording.push(lt.serializeEvent(i))
                    }.bind(this)), de.addJqEventListener(Gt(document), "on", "change", function(e) {
                        var t = F.getTargetNode(e);
                        this.sendRadioCheckboxData(t)
                    }.bind(this), "input[type=radio],input[type=checkbox]"), de.addJqEventListener(Gt(document), "on", "change", function(e) {
                        var t, n = F.getTargetNode(e),
                            i = Gt(n).val();
                        if (Gt.isArray(i)) {
                            for (var o = 0; o < i.length; o++) i[o] = F.sanitizeActionData(i[o]);
                            t = i.join("!-ac-!")
                        } else t = i;
                        t = this.handleProtected(n);
                        var r = {
                            action: "sb",
                            timestamp: C() - Z.startTime,
                            selectorPath: F.sanitizeActionData(he(n)),
                            value: t
                        };
                        this.recording.push(lt.serializeEvent(r))
                    }.bind(this), "select"), de.addJqEventListener(Gt(document), "on", "change", function(e) {
                        var t = this,
                            n = F.getTargetNode(e);
                        !n || "INPUT" === n.tagName && -1 !== ["radio", "checkbox", "file"].indexOf(n.type) || setTimeout(function() {
                            t.sendInputElementData(n)
                        }, 10)
                    }.bind(this), "input,textarea")
                }, rn.prototype.sendInputElementData = function(e) {
                    var t;
                    if (e) {
                        "OPTION" === e.tagName && "SELECT" === (null === (t = e.parentNode) || void 0 === t ? void 0 : t.tagName) && (e = e.parentNode);
                        var n = this.handleProtected(e),
                            i = {
                                action: "ic",
                                timestamp: C() - Z.startTime,
                                selectorPath: F.sanitizeActionData(he(e)),
                                value: n
                            },
                            o = lt.serializeEvent(i);
                        this.recording.push(o)
                    }
                }, rn.prototype.sendRadioCheckboxData = function(e) {
                    if (e) {
                        var t = Gt(e).is(":checked"),
                            n = {
                                action: "rb",
                                timestamp: C() - Z.startTime,
                                selectorPath: F.sanitizeActionData(he(e)),
                                isChecked: t
                            },
                            i = lt.serializeEvent(n);
                        this.recording.push(i)
                    }
                }, rn.prototype.patchNativeProperty = function(e, t, n) {
                    if (t in e) {
                        for (var i, o = e; null !== o && !(i = Object.getOwnPropertyDescriptor(o, t));) o = Object.getPrototypeOf(o);
                        i && Object.defineProperty(o, t, {
                            configurable: !0,
                            get: function() {
                                return i.get.call(this)
                            },
                            set: function(e) {
                                var t = {
                                    oldValue: i.get.call(this),
                                    newValue: e
                                };
                                i.set.call(this, t.newValue), t.oldValue !== t.newValue && document.body.contains(this) && n(this)
                            }
                        })
                    }
                }, rn.prototype.patchNativeProperties = function() {
                    Qt || (this.patchNativeProperty(HTMLInputElement.prototype, "value", this.sendInputElementData.bind(this)), this.patchNativeProperty(HTMLSelectElement.prototype, "value", this.sendInputElementData.bind(this)), this.patchNativeProperty(HTMLOptionElement.prototype, "selected", this.sendInputElementData.bind(this)), this.patchNativeProperty(HTMLInputElement.prototype, "checked", this.sendRadioCheckboxData.bind(this)), Qt = !0)
                }, rn.prototype.storeMouseMove = function() {
                    if ("HTML" !== this.mouse.curMove.el.nodeName && (this.mouse.lastMove.docX !== this.mouse.curMove.docX || this.mouse.lastMove.docY !== this.mouse.curMove.docY)) {
                        var e = {
                            action: "mm",
                            timestamp: C() - Z.startTime,
                            selectorPath: F.sanitizeActionData(he(this.mouse.curMove.el)),
                            elWidth: this.mouse.curMove.width,
                            elHeight: this.mouse.curMove.height,
                            relX: this.mouse.curMove.relX,
                            relY: this.mouse.curMove.relY,
                            docX: this.mouse.curMove.docX,
                            docY: this.mouse.curMove.docY
                        };
                        this.recording.push(lt.serializeEvent(e)), this.mouse.lastMove.docX = this.mouse.curMove.docX, this.mouse.lastMove.docY = this.mouse.curMove.docY
                    }
                }, rn.prototype.handleProtected = function(e) {
                    var t = F.handleProtected(e, this.anonymizeKeys);
                    return t = F.sanitizeActionData(t)
                }, rn.prototype.addNlsData = function(e, t) {
                    e._vwo = e._vwo || {}, e._vwo.nls = e._vwo.nls || {}, e._vwo.nls = R(R({}, e._vwo.nls), t)
                }, rn.prototype.getClickProps = function(e, t) {
                    var n, i;
                    if ((!t || Z.hs && !Z.hsStopped) && (t || Z.r)) {
                        var o = {};
                        t && (o = Z.getCampaigns(t));
                        var r, a = null === (i = null === (n = VWO.modules) || void 0 === n ? void 0 : n.utils) || void 0 === i ? void 0 : i.heatmapUtils,
                            s = 0,
                            d = 0;
                        if (a) {
                            var c = a.getTargetPathInfo(e.event);
                            s = c.x_percent, d = c.y_percent, r = c.xpath
                        } else e.elWidth && (s = Math.round(1e3 * e.relX / e.elWidth) / 1e3), e.elHeight && (d = Math.round(1e3 * e.relY / e.elHeight) / 1e3);
                        var l = R({
                            path: r || e.selectorPath,
                            x: s,
                            y: d
                        }, o);
                        if (e.innerText && (l.innerText = e.innerText), t) {
                            var u = Z.Recording.heatmap.maxClicksRecorded - Z.Recording.heatmap.eventsLength;
                            Z.Recording.heatmap.eventsLength++, e && 0 < u && (l.isHeatmapData = !0)
                        }
                        return l
                    }
                }, rn.prototype.getHeatmapData = function(e) {
                    if (Z.hs && !Z.hsStopped) {
                        var t = e,
                            n = {},
                            i = Z.Recording.heatmap.maxClicksRecorded - Z.Recording.heatmap.eventsLength;
                        return 0 < (t = t.slice(0, i)).length && !D.data360migrated() && (Z.Recording.heatmap.eventsLength += t.length, t = t.map(function(e) {
                            return lt.serializeEvent(e)
                        }), Gt.extend(n, {
                            mc: t.join(",")
                        })), Z.Recording.hasScrollChanged() && Gt.extend(n, {
                            scroll_percentage: Z.recordingData.totals.scroll
                        }), n
                    }
                }, rn.prototype.resetHeatmapData = function() {
                    var e = A.get("Array");
                    Z.Recording.heatmap.eventsLength = 0, Z.hsStopped = !1, Z.Recording.heatmap.clicks = new e, Z.Recording.index.heatmap = 0
                }, rn.prototype.resetData = function() {
                    Z.tags.eTags.refresh(), Z.lastSendTime = 0, Z.Recording.resetHeatmapData()
                }, rn.prototype.hasScrollChanged = function() {
                    var e = Z.getScrollPercentage();
                    return e > Z.recordingData.totals.scroll && 0 < e && (Z.recordingData.totals.scroll = e), Z.recordingData.totals.scroll !== Z.recordingData.last.scroll
                }, rn.prototype.getHeatmapScroll = function() {
                    var e = Z.getScrollPercentage();
                    return e > Z.recordingData.totals.scroll && 0 < e && (Z.recordingData.totals.scroll = e), Z.recordingData.totals.scroll
                }, rn.prototype.addTag = function(e) {
                    Z.analyze || (Z.Recording.tags.add(e), Z.Recording.tags.isTagPassed() && at.sendRecordingData(!0))
                }, rn.prototype.getTagData = function() {
                    if (Z.r && Z.Recording.tags.isTagPassed()) return {
                        tags: Z.Recording.tags.get()
                    }
                }, rn.prototype.getRecordingData = function(e, t, n) {
                    var i = {};
                    if (Z.r) {
                        if (e.length && (e = e.map(function(e) {
                                return "object" != typeof e ? e : lt.serializeEvent(e)
                            }), i.recording = e.join(",")), t.length) {
                            var o = A.get("JSON");
                            i.mutations = o.stringify(t)
                        }
                        return Gt.extend(i, n), i
                    }
                }, rn.prototype.sendData = function() {
                    var e, t, n, i, o, r, a, s, d, c, l = Z.getScreenScale(),
                        u = {};
                    if (this.oldSettings = this.oldSettings || {}, this.isMobile && (this.oldSettings.screenScaleX !== l.x || this.oldSettings.screenScaleY !== l.y)) {
                        var h = {
                            action: "sS",
                            timestamp: C() - Z.startTime,
                            screenScaleX: l.x,
                            screenScaleY: l.y
                        };
                        Z.Recording.recording.push(lt.serializeEvent(h)), this.oldSettings.screenScaleX = l.x, this.oldSettings.screenScaleY = l.y
                    }
                    var f = sn();
                    if (!(Z.Recording.index.recording >= Z.Recording.recording.length && Z.Recording.index.heatmap >= Z.Recording.heatmap.clicks.length && Z.Mutations.index >= Z.Mutations.mutations.length) || Z.Recording.hasScrollChanged() || Z.Recording.tags.isTagPassed() || f.length) {
                        var m = Z.Recording.recording.slice(Z.Recording.index.recording),
                            p = Z.Mutations.mutations.slice(Z.Mutations.index),
                            g = Z.Recording.heatmap.clicks.slice(Z.Recording.index.heatmap);
                        Z.Recording.index.recording = Z.Recording.recording.length, Z.Recording.index.heatmap = Z.Recording.heatmap.clicks.length, Z.Mutations.index = Z.Mutations.mutations.length;
                        var v = A.get("JSON"),
                            w = se.init(g, !1);
                        D.deadClickInHeatmap() && (Z.hs || Z.r) && (g = w.latestRecording);
                        var _ = w.deadClickInThisBatch ? 11 : 0,
                            y = w.rageClickInThisBatch ? 12 : 0,
                            E = {};
                        if (_ || y) {
                            var T = [];
                            T.push(_, y), E[I.RECORDING_META] = ((e = {})[I.CLICKS_TRACKED] = T, e), void 0 !== VWO.phoenix && Gt.extend(E[I.RECORDING_META], R(((t = {})[I.CLICK_TRACK_COUNT] = w.clickCounts, t), D.data360migrated() ? {} : ((n = {})[I.CLICK_TEXT_DATA] = w.textData, n))), E[I.RECORDING_META] = v.stringify(E[I.RECORDING_META])
                        } else void 0 !== VWO.phoenix && w.clickCounts && w.clickCounts.c && (E[I.RECORDING_META] = ((i = {})[I.CLICK_TRACK_COUNT] = w.clickCounts, i), E[I.RECORDING_META] = v.stringify(E[I.RECORDING_META]));
                        var S = Z.Recording.getRecordingData(m, p, E),
                            b = Z.Recording.getHeatmapData(g),
                            O = Z.Recording.getTagData();
                        VWO._ && VWO._.ac && VWO._.ac.rdbg && S && S.recording && (S.recording = S.recording + ",vwomt_" + (C() - Z.startTime) + "_" + encodeURIComponent(v.stringify({
                            evq: f
                        })));
                        try {
                            null !== (a = null === (r = null === (o = window.VWO) || void 0 === o ? void 0 : o._) || void 0 === r ? void 0 : r.ac) && void 0 !== a && a.cRecJS && null !== (c = null === (d = null === (s = window.VWO) || void 0 === s ? void 0 : s._) || void 0 === d ? void 0 : d.ac) && void 0 !== c && c.cRecJS() ? Gt.extend(u, b, O) : Gt.extend(u, S, b, O)
                        } catch (e) {
                            window.VWO._.customError && window.VWO._.customError({
                                msg: "Error in cRecJS " + VWO._.ac.cRecJS,
                                url: document.URL
                            })
                        }
                        return u
                    }
                }, rn.prototype.pushScaleInformation = function() {
                    var e = Z.getScale(),
                        t = e.x,
                        n = e.y,
                        i = {
                            action: "is",
                            timestamp: C() - Z.startTime,
                            scaleX: t,
                            scaleY: n,
                            scrollX: Gt(window).scrollLeft(),
                            scrollY: Gt(window).scrollTop()
                        };
                    this.recording.push(lt.serializeEvent(i))
                }, rn.prototype.init = function(e) {
                    if (e && rn.bind(this)(), this.currentUrl = location.href, at.formSubmitCallbacks.push(this.sendData.bind(this)), this.signal.onInit(), this.data360Events && this.data360Events.onInit(), this.devicePixelRatio = window.devicePixelRatio, this.initialClientHeight = Z.getViewportDimensions().height, this.isMobile) {
                        this.oldSettings = this.oldSettings || {};
                        var t = Z.getScreenScale(),
                            n = t.x,
                            i = t.y,
                            o = {
                                action: "sS",
                                timestamp: C() - Z.startTime,
                                screenScaleX: n,
                                screenScaleY: i
                            };
                        Z.Recording.recording.push(lt.serializeEvent(o)), this.oldSettings.screenScaleX = n, this.oldSettings.screenScaleY = i, this.pushScaleInformation();
                        for (var r = 500; r <= 2e3; r += 500) {
                            var a = setTimeout(this.pushScaleInformation.bind(this), r);
                            de.pushTimers(a, "timeout")
                        }
                    }
                    var s = Z.getScale(),
                        d = Z.getScroll(),
                        c = Z.getDimensions(),
                        l = {
                            action: "sc",
                            timestamp: C() - Z.startTime,
                            scaleX: s.x,
                            scaleY: s.y,
                            scrollX: d.x,
                            scrollY: d.y,
                            width: c.width,
                            height: c.height,
                            pageWidth: Gt("html").width(),
                            pageHeight: Math.max(Gt("html").height(), Gt("html")[0].scrollHeight)
                        };
                    if (this.recording.push(lt.serializeEvent(l)), !$t) {
                        this.loadEventListeners(), this.patchNativeProperties();
                        var u = !1;
                        document.addEventListener("vwo.meta", function(e) {
                            var t, n = e.data,
                                i = A.get("JSON");
                            n && "object" == typeof n && 519176 === window._vwo_acc_id && (n.snapShottedUrl = i.stringify(Z.linkHrefForSnapshotting), u || (n.sessionId = Z.ids.session, n.uuid = F.getUUID(), n.aST = VWO._.ast), u = !0), n && "object" == typeof n && 555774 === window._vwo_acc_id && (n.vvps = window.visualViewport && window.visualViewport.scale), n && "object" == typeof n && 373511 === window._vwo_acc_id && window.vwo_iehack_queue.length && (n.gifCalls = [], null !== (t = window.vwo_iehack_queue) && void 0 !== t && t.forEach(function(e) {
                                n.gifCalls.push(e.src)
                            })), n && "object" == typeof n && 566821 === window._vwo_acc_id && window._vwo_geo && (u || (n.location = window._vwo_geo), u = !0), Z.Recording.recording.push("vwomt_" + (C() - Z.startTime) + "_" + encodeURIComponent(i.stringify(n)))
                        }), VWO._.customEvent("meta", {
                            cks: an()
                        });
                        var h = setInterval(this.storeMouseMove.bind(this), 300);
                        de.pushTimers(h, "interval"), $t = !0
                    }
                }, rn.prototype.processErrorClick = function(e) {
                    var t = e.map(function(e) {
                        var t = e.split(":"),
                            n = t[0],
                            i = t[1];
                        return {
                            timestamp: parseInt(n, 10),
                            id: parseInt(i, 10)
                        }
                    });
                    if (!F.validateLastNErrors(t, 3)) {
                        var n = C() - Z.startTime,
                            i = this.heatmap.clicks[this.heatmap.clicks.length - 1];
                        i && i.timestamp >= n - 500 && (i.isErrorClick = !0);
                        for (var o = this.recording, r = o.length - 1; 0 < r; r--) {
                            var a = o[r];
                            if ("string" == typeof a) {
                                if ("mc" === [a[0], a[1]].join("")) {
                                    var s = a.split("_");
                                    n - 500 <= parseInt(s[1], 10) && (o[r].endsWith("_%13%") || (o[r] += "_%13%"));
                                    break
                                }
                            } else if ("mc" === a.action) {
                                a.timestamp >= n - 500 && (a.isErrorClick = !0);
                                break
                            }
                        }
                    }
                }, Z.Recording = new rn;
                var dn = Z.Recording,
                    cn = {
                        EXCEPTION: "ex",
                        PROMISE_REJECTION: "pr",
                        CSP: "csp"
                    },
                    ln = {
                        FETCH: "fetch",
                        XHR: "xmlhttprequest"
                    },
                    un = A.get("Array"),
                    hn = "_",
                    fn = ",",
                    mn = (pn.prototype.getDataToSend = function() {
                        if (this.shouldSendData()) {
                            var e = this.processedData.slice(this.nextErrorIndex).join(fn),
                                t = this.pageErrors.slice(this.nextPageErrorIndex).join(fn);
                            return this.nextErrorIndex = this.processedData.length, this.nextPageErrorIndex = this.pageErrors.length, {
                                er: e,
                                pEr: t
                            }
                        }
                    }, pn.prototype.shouldSendData = function() {
                        return this.nextErrorIndex !== this.processedData.length
                    }, pn.prototype.resetData = function() {
                        this.processedData = new un, this.pageErrors = new un, this.nextErrorIndex = 0, this.nextPageErrorIndex = 0
                    }, pn.prototype.processErrorData = function(e, t, n) {
                        if (void 0 === t && (t = []), "string" != typeof e[1] || e[1].toLowerCase() !== "Script error.".toLowerCase()) {
                            t = t || [];
                            var i = e.join(hn),
                                o = t.join(hn),
                                r = n || Math.floor(C() - Z.startTime);
                            Z.Recording.processErrorClick(this.processedData);
                            var a = this.pageErrors.indexOf(i);
                            0 <= a ? this.processedData.push(r + ":" + a) : (a = this.pageErrors.length, this.pageErrors.push(i), t.length ? this.processedData.push([r + ":" + a, i, o].join(hn)) : this.processedData.push([r + ":" + a, i].join(hn)))
                        }
                    }, pn);

                function pn() {
                    this.processedData = new un, this.pageErrors = new un, this.nextErrorIndex = 0, this.nextPageErrorIndex = 0, this.truncationLenth = 100
                }
                var gn = (vn.patchFetchAPI = function(s) {
                    var d = window.fetch;
                    window.fetch = function(e, t) {
                        var n = d.call(this, e, t);
                        try {
                            var i = H.toAbsURL(vn.getFetchURL(e));
                            if (Cn.isUrlAllowed(i)) {
                                var o = F.getAnonymizedUrl(i),
                                    r = F.sanitizeActionData(o),
                                    a = t && t.method || "GET";
                                vn.handleResponsePromise(s, n, r, a)
                            }
                        } catch (e) {}
                        return n
                    }
                }, vn.handleResponsePromise = function(t, e, n, i) {
                    e.then(function(e) {
                        "status" in e && 400 <= e.status && t.processErrorData([ln.FETCH, n, e.status || 0, i])
                    }).catch(function(e) {
                        t.processErrorData([ln.FETCH, n, 0, i], [F.sanitizeActionData(e.message)])
                    })
                }, vn.getFetchURL = function(e) {
                    return "object" == typeof e ? e.url ? e.url : e.toString() : e
                }, vn);

                function vn() {}
                var wn = (_n.patchXHRAPI = function(t) {
                    var r = XMLHttpRequest.prototype.open,
                        e = XMLHttpRequest.prototype.send;
                    XMLHttpRequest.prototype.open = function(e, t) {
                        try {
                            var n = H.toAbsURL(t),
                                i = F.getAnonymizedUrl(n),
                                o = F.sanitizeActionData(i);
                            this._url = o
                        } catch (e) {}
                        this._method = e, r.apply(this, arguments)
                    }, XMLHttpRequest.prototype.send = function() {
                        try {
                            Cn.isUrlAllowed(this._url) && (this.addEventListener("error", function(e) {
                                t.processErrorData([ln.XHR, this._url, this.status || 0, this._method], [F.sanitizeActionData(e.message)])
                            }), this.addEventListener("load", function(e) {
                                400 <= this.status && t.processErrorData([ln.XHR, this._url, this.status || 0, this._method], [F.sanitizeActionData(e.message)])
                            }))
                        } catch (e) {}
                        e.apply(this, arguments)
                    }
                }, _n);

                function _n() {}
                window._vwo_evq = window._vwo_evq || [];
                var yn = "jI",
                    En = window._vwo_evq,
                    Tn = window._vwo_ev = window._vwo_ev || function() {
                        arguments[0] !== yn ? En.push([].slice.call(arguments)) : En.unshift([yn])
                    };
                window.VWO._.triggerEvent = window._vwo_ev;
                var Sn = {
                        PARSE_TLD: "pTLD"
                    },
                    bn = ["co", "org", "com", "net", "edu", "au", "ac"],
                    On = window.vwo_$ || window.$;

                function Rn(e) {
                    var t, n = e.split("."),
                        i = n.length,
                        o = n[i - 2];
                    return t = o && -1 !== On.inArray(o, bn) ? n[i - 3] + "." + o + "." + n[i - 1] : o + "." + n[i - 1], Tn(Sn.PARSE_TLD, e, t), t
                }
                var Cn = (Nn = mn, e(Dn, Nn), Dn.prototype.processXHRRequests = function() {
                        var r = this;
                        performance.getEntriesByType("resource").forEach(function(e) {
                            if (!([ln.FETCH, ln.XHR].indexOf(e.initiatorType) < 0) && Dn.isUrlAllowed(e.name) && 400 <= e.responseStatus) {
                                var t = Math.floor(l(performance.timeOrigin + e.responseEnd) - Z.startTime),
                                    n = F.getAnonymizedUrl(e.name),
                                    i = F.sanitizeActionData(n),
                                    o = [e.initiatorType, i, e.responseStatus];
                                r.processErrorData(o, null, t)
                            }
                        })
                    }, Dn.prototype.addPerformanceObserver = function() {
                        var r = this,
                            e = new PerformanceObserver(function(e) {
                                e.getEntries().forEach(function(e) {
                                    if (!(-1 < [ln.FETCH, ln.XHR].indexOf(e.initiatorType)) && Dn.isUrlAllowed(e.name) && 400 <= e.responseStatus) {
                                        var t = Math.floor(l(performance.timeOrigin + e.responseEnd) - Z.startTime),
                                            n = F.getAnonymizedUrl(e.name),
                                            i = F.sanitizeActionData(n),
                                            o = [e.initiatorType, i, e.responseStatus];
                                        r.processErrorData(o, null, t)
                                    }
                                })
                            });
                        e.observe({
                            type: "resource",
                            buffered: !0
                        }), window.addEventListener("beforeunload", function() {
                            e.disconnect()
                        })
                    }, Dn.isUrlAllowed = function(e) {
                        var t = Z.getCookieDomain(),
                            n = !0;
                        try {
                            n = !/^$/g.test(e) && !F.containsOurDomain(e) && Rn(new URL(e).host) === t
                        } catch (e) {}
                        return n
                    }, Dn),
                    Nn;

                function Dn() {
                    var e = Nn.call(this) || this;
                    return e.processXHRRequests(), e.addPerformanceObserver(), wn.patchXHRAPI(e), gn.patchFetchAPI(e), e
                }
                var In = (An = mn, e(xn, An), xn.prototype.addErrorListener = function() {
                        var r = this;
                        window.addEventListener("error", function(e) {
                            if (e && e.message) {
                                var t = F.sanitizeActionData(e.message.slice(0, r.truncationLenth));
                                return r.processErrorData([cn.EXCEPTION, t]), !0
                            }
                        }), window.addEventListener("unhandledrejection", function(e) {
                            if (e && e.reason) {
                                var t = "Uncaught (in promise) " + e.reason,
                                    n = F.sanitizeActionData(t.slice(0, r.truncationLenth));
                                return r.processErrorData([cn.PROMISE_REJECTION, n]), !0
                            }
                        }), document.addEventListener("securitypolicyviolation", function(e) {
                            if (e && e.blockedURI && Cn.isUrlAllowed(e.blockedURI)) {
                                var t = F.getAnonymizedUrl(e.blockedURI),
                                    n = F.sanitizeActionData(t),
                                    i = [cn.CSP, n, F.sanitizeActionData(e.violatedDirective)],
                                    o = [F.sanitizeActionData(e.originalPolicy)];
                                r.processErrorData(i, o)
                            }
                        })
                    }, xn),
                    An;

                function xn() {
                    var e = An.call(this) || this;
                    return e.addErrorListener(), e
                }
                var kn = A.get("JSON"),
                    Ln = (Mn = mn, e(Pn, Mn), Pn.prototype.patchConsoleAPI = function() {
                        var n = console.error,
                            i = this;
                        console.error = function(e) {
                            try {
                                var t = e;
                                "string" != typeof t && (t = kn.stringify(e)), t = F.sanitizeActionData(t.slice(0, i.truncationLenth)), i.processErrorData([t])
                            } catch (e) {}
                            n.apply(console, arguments)
                        }
                    }, Pn),
                    Mn;

                function Pn() {
                    var e = Mn.call(this) || this;
                    return e.patchConsoleAPI(), e
                }
                var Vn = !1,
                    Wn = (Hn.prototype.init = function() {
                        Z.errorLoggerEnabled && Z.ready && !Vn && (this.exceptionRecorder = new In, this.consoleRecorder = new Ln, this.networkRecorder = new Cn, Vn = !0, at.formSubmitCallbacks.push(this.sendData.bind(this)), Z.r && VWO._.phoenixMT && VWO._.phoenixMT.on(window.VWO._.EventsEnum.ERROR_ONPAGE, function(e) {
                            var t = R(R({}, e), Z.getCampaigns(!1));
                            e.vwoEventName = window.VWO._.EventsEnum.ERROR_ONPAGE, Z.Recording.addNlsData(e, t)
                        }, {
                            syncToDataLayer: !0
                        }))
                    }, Hn.prototype.sendData = function() {
                        if (Z.r && this.isNewDataAvailable()) {
                            var e = this.exceptionRecorder.getDataToSend(),
                                t = this.consoleRecorder.getDataToSend(),
                                n = this.networkRecorder.getDataToSend();
                            return D.data360migrated() && Hn.sendErrorOnPageAsEvent(e, t, n), Hn.parseErrorLoggerData(e, t, n)
                        }
                    }, Hn.sendErrorOnPageAsEvent = function(e, t, n) {
                        var i = {};
                        e && e.pEr && (i.e = e.pEr), t && t.pEr && (i.c = t.pEr), n && n.pEr && (i.n = n.pEr), 0 < Object.keys(i).length && VWO._.phoenixMT && VWO._.phoenixMT.trigger(window.VWO._.EventsEnum.ERROR_ONPAGE, i)
                    }, Hn.parseErrorLoggerData = function(e, t, n) {
                        var i = _.get("JSON"),
                            o = {
                                error: {
                                    e: null == e ? void 0 : e.er,
                                    c: null == t ? void 0 : t.er,
                                    n: null == n ? void 0 : n.er
                                }
                            };
                        return null != e && e.pEr && (o.pEr = o.pEr || {}, o.pEr.e = e.pEr), null != t && t.pEr && (o.pEr = o.pEr || {}, o.pEr.c = t.pEr), null != n && n.pEr && (o.pEr = o.pEr || {}, o.pEr.n = n.pEr), o.pEr && (o.pEr = i.stringify(o.pEr)), o.error = i.stringify(o.error), o
                    }, Hn.prototype.isNewDataAvailable = function() {
                        return this.exceptionRecorder.shouldSendData() || this.consoleRecorder.shouldSendData() || this.networkRecorder.shouldSendData()
                    }, Hn.prototype.resetData = function() {
                        Vn && (this.exceptionRecorder.resetData(), this.consoleRecorder.resetData(), this.networkRecorder.resetData())
                    }, Hn);

                function Hn() {}
                Z.ErrorLogger = new Wn;
                var Un = function() {};

                function zn() {
                    this.focusedNode = null, this.selectedLiIndex = -1
                }
                zn.prototype.getFocusedNode = function() {
                    return this.focusedNode
                }, zn.prototype.setFocusOnNode = function(e) {
                    this.focusedNode = e
                }, zn.prototype.resetFocusedNode = function() {
                    this.focusedNode = null
                }, zn.prototype.resetSelectedLiIndex = function() {
                    this.selectedLiIndex = -1
                }, zn.prototype.getSelectedLiIndex = function() {
                    return this.selectedLiIndex
                }, zn.prototype.setSelectedLiIndex = function(e, t) {
                    t = t || Un, this.selectedLiIndex !== e && (this.selectedLiIndex = e, t())
                }, zn.prototype.doBlurOnNode = function(e, t) {
                    t = t || Un, this.focusedNode && this.focusedNode !== e && (t(), this.resetFocusedNode(), this.resetSelectedLiIndex())
                }, zn.prototype.doFocusOnNode = function(e, t) {
                    t = t || Un, this.focusedNode && this.focusedNode === e || t()
                };
                var Fn = new zn,
                    jn = {
                        createEvent: function(e, t) {
                            try {
                                var n = e.createEvent("Event");
                                return n.initEvent(t, !1, !1), n
                            } catch (e) {
                                window.VWO._.customError && window.VWO._.customError({
                                    msg: e && e.stack || e + ".",
                                    url: document.URL,
                                    source: "cEF"
                                })
                            }
                        },
                        dispatchEvent: function(e, t) {
                            e.dispatchEvent(t)
                        }
                    },
                    Xn = v,
                    Bn;

                function Yn(e) {
                    if (e.vwoCustEl && e.vwoCustEl.length) {
                        for (var t = e.vwoCustEl, n = 0; n < t.length; n++) delete t[n].vwoNatEl, ti(t[n]);
                        delete e.vwoCustEl
                    }
                }

                function qn(e) {
                    for (var t = e.customElements, n = e.nativeElement, i = [], o = 0; o < t.length; o++)
                        if (t[o]) {
                            if (t[o].vwoNatEl) continue;
                            t[o].vwoNatEl = n, ei(t[o]), i.push(t[o])
                        }
                    n.vwoCustEl = i
                }

                function Kn(e) {
                    var t = Xn(e),
                        n = t.attr("type");
                    if (t.is("input") && ("checkbox" === n || "radio" === n)) {
                        var i = Xn('label[for="' + t.attr("id") + '"]');
                        if (i.length <= 0) {
                            var o = t.parent();
                            "label" === o.get(0).tagName.toLowerCase() && (i = o)
                        }
                        return i.get(0)
                    }
                }

                function Gn(e) {
                    var t = e.currentTarget.vwoNatEl,
                        n = Xn(e.target);
                    if (t.tagName && "SELECT" === t.tagName && (n.is("li") || n.parent().is("li") || n.parent().parent().is("li"))) {
                        var i = n.closest("li").index();
                        return t.chStatsProcessed = 0, Fn.setSelectedLiIndex(i, Bn.changeHandler.bind({}, {
                            target: t
                        }, !0)), void(t.chStatsProcessed = 1)
                    }
                    t.mdStatsProcessed = 0, Bn.mouseDownHandler({
                        target: t
                    }), t.mdStatsProcessed = 1, t.fcStatsProcessed = 0, Fn.doFocusOnNode(t, Bn.focusHandler.bind({}, {
                        target: t
                    })), t.fcStatsProcessed = 1
                }

                function Jn(e) {
                    var t = Xn(e.target),
                        n = e.currentTarget.vwoNatEl;
                    n.tagName && "SELECT" === n.tagName && (t.parent().is("li") || t.parent().parent().is("li")) || (n.muStatsProcessed = 0, Bn.mouseUpHandler({
                        target: n
                    }), n.muStatsProcessed = 1, n.chStatsProcessed = 0, "INPUT" === n.tagName && "checkbox" === n.type ? Bn.changeHandler({
                        target: n
                    }) : "INPUT" === n.tagName && "radio" === n.type && Fn.doFocusOnNode(n, Bn.changeHandler.bind({}, {
                        target: n
                    })), n.chStatsProcessed = 1, Fn.setFocusOnNode(n))
                }

                function $n(e) {
                    var t = e.currentTarget.vwoNatEl;
                    t.meStatsProcessed = 0, Bn.mouseEnterHandler({
                        target: t
                    }), t.meStatsProcessed = 1
                }

                function Qn(e) {
                    var t = e.currentTarget.vwoNatEl;
                    t.mlStatsProcessed = 0, Bn.mouseLeaveHandler({
                        target: t
                    }), t.mlStatsProcessed = 1
                }

                function Zn() {
                    document.addEventListener("vwo_element_added", function(e) {
                        if (Bn._mapElements) {
                            var t = e.data;
                            t.constructor !== NodeList && (t = [t]);
                            for (var n = 0; n < t.length; n++) {
                                var i = [];
                                t[n].tagName && "input" === t[n].tagName.toLowerCase() && i.push(Kn(t[n])), i.push(Bn._mapElements(t[n]));
                                var o = {
                                    customElements: i.reduce(function(e, t) {
                                        return e.concat(t)
                                    }, []),
                                    nativeElement: t[n]
                                };
                                Yn(t[n]), qn(o)
                            }
                        }
                    }, !1)
                }

                function ei(e) {
                    e.addEventListener("mousedown", Gn, !0), e.addEventListener("mouseup", Jn, !0), e.addEventListener("mouseenter", $n, !0), e.addEventListener("mouseleave", Qn, !0)
                }

                function ti(e) {
                    e.removeEventListener("mousedown", Gn, !0), e.removeEventListener("mouseup", Jn, !0), e.removeEventListener("mouseenter", $n, !0), e.removeEventListener("mouseleave", Qn, !0)
                }
                var ni = {
                        init: function(e) {
                            Bn = e, Zn()
                        }
                    },
                    ii = v,
                    oi = window._vwo_exp,
                    ri = window._vwo_exp_ids,
                    ai = "ANALYZE_FORM",
                    si = 0,
                    di;

                function ci(e, t) {
                    var n, i = ii(e.target),
                        o = i.get(0);
                    switch (t) {
                        case "mouseenter":
                            n = o.meStatsProcessed;
                            break;
                        case "mouseleave":
                            n = o.mlStatsProcessed;
                            break;
                        case "mousedown":
                            n = o.mdStatsProcessed;
                            break;
                        case "mouseup":
                            n = o.muStatsProcessed;
                            break;
                        case "focus":
                            n = o.fcStatsProcessed;
                            break;
                        case "blur":
                            n = o.blStats;
                            break;
                        case "change":
                            n = o.chStatsProcessed;
                            break;
                        case "keydown":
                            n = o.kdStats
                    }
                    if (!n && li.handleTracking(i)) return i
                }
                var li = {
                    f: {},
                    forms: {},
                    lastSentForms: void 0,
                    changes: {},
                    marketoListenerAdded: !1,
                    formSelector: "form,[nls_fa_name]:not(form)",
                    formObservers: [],
                    formInputSelector: "form input, form select, form textarea, [nls_fa_name]:not(form) input, [nls_fa_name]:not(form) select, [nls_fa_name]:not(form) textarea",
                    getFormName: function(e) {
                        if (Z.analyze) {
                            var t = e.data("id");
                            return t ? t : (si++, e.data("id", si), si)
                        }
                        return void 0 !== e.attr("nls_fa_name") && !1 !== e.attr("nls_fa_name") ? e.attr("nls_fa_name") : void 0 !== e.attr("id") && !1 !== e.attr("id") ? e.attr("id") : void 0 !== e.attr("name") && !1 !== e.attr("name") && e.attr("name")
                    },
                    getFormElementName: function(e) {
                        return void 0 !== e.attr("nls_fa_el_name") && !1 !== e.attr("nls_fa_el_name") && null !== e.attr("nls_fa_el_name") ? e.attr("nls_fa_el_name") : void 0 !== e.attr("id") && !1 !== e.attr("id") && null !== e.attr("id") ? e.attr("id") : void 0 !== e.attr("name") && !1 !== e.attr("name") && null !== e.attr("name") && e.attr("name")
                    },
                    addForm: function(t) {
                        var n = this;
                        if (D.checkFormVisibility() && "undefined" != typeof IntersectionObserver) {
                            var i = new IntersectionObserver(function(e) {
                                e.forEach(function(e) {
                                    e.isIntersecting && .1 <= e.intersectionRatio && (n.processAddedForm(t), i.disconnect())
                                })
                            }, {
                                threshold: .1
                            });
                            i.observe(t[0]), this.formObservers.push(i)
                        } else this.processAddedForm(t)
                    },
                    resetFormObservers: function() {
                        this.formObservers.forEach(function(e) {
                            e.disconnect()
                        }), this.formObservers = []
                    },
                    processAddedForm: function(n) {
                        var i, o, r, a, s, e = ri.length,
                            d = this.getFormName(n);
                        if (d) {
                            for (; e--;)
                                if (o = ri[e], (i = oi[o]).type === ai && i.ready) {
                                    var t = void 0;
                                    try {
                                        t = ii(i.forms[0])
                                    } catch (e) {
                                        return
                                    }
                                    t.eq(0).each(l)
                                }
                            if (D.marketoOptimization() && this.addMarketoListeners(), (!Z.analyze || r) && 1 !== n.data("nls_fa_tracked") && !this.forms[d]) {
                                a = {
                                    hiddenInputs: 0,
                                    submit: 0,
                                    total: 0
                                }, n.find("input, select, textarea").each(function(e, t) {
                                    var n = ii(t);
                                    "hidden" === n.prop("type").toLowerCase() && (a.hiddenInputs += 1), "submit" === n.prop("type").toLowerCase() && (a.submit += 1), a.total += 1
                                }), s = a.hiddenInputs + a.submit === a.total;
                                var c = A.get("Array");
                                d ? s ? (n.data("nls_fa_tracked", 0), n.data("nls_fa_active", 0)) : (n.data("nls_fa_tracked", 1), n.data("nls_fa_active", 1), this.forms[d] = {
                                    name: d,
                                    fields: new c,
                                    interacted: 0,
                                    submitted: 0
                                }, n.data("nls_fa_name", d)) : (n.data("nls_fa_tracked", 1), n.data("nls_fa_active", 0)), n.find("input, select, textarea").each(function(e, t) {
                                    var n = ii(t),
                                        i = li.getFormElementName(n),
                                        o = n.prop("type"),
                                        r = 1 === n.data("nls_fa_tracked") && 1 === n.data("nls_fa_active");
                                    if (!d || !i || "hidden" === n.prop("type").toLowerCase() || s) return r || (n.data("nls_fa_tracked", 1), n.data("nls_fa_active", 0)), !0;
                                    var a = li.forms[d].fields.length;
                                    li.forms[d].fields.push({
                                        name: i,
                                        type: o,
                                        interacted: 0,
                                        filledOut: 0,
                                        refilled: 0,
                                        timeHesitation: 0,
                                        timeInteraction: 0,
                                        fip: a
                                    }), n.data("nls_fa_tracked", 1), n.data("nls_fa_active", 1), n.data("nls_fa_name", d), n.data("nls_fa_field_pos", a)
                                }), Object.prototype.hasOwnProperty.call(this.forms, d) && this.addChanges("add_forms", this.forms[d], d)
                            }
                        }

                        function l(e, t) {
                            t === n[0] && (r = !0, li.f[d] = li.f[d] || {}, li.f[d][o] || (li.f[d][o] = i.forms[0]))
                        }
                    },
                    addFormElement: function(e) {
                        if (1 !== e.data("nls_fa_tracked")) {
                            var t = e.closest(this.formSelector);
                            if (1 === t.data("nls_fa_tracked")) {
                                if (1 === t.data("nls_fa_tracked") && 1 !== t.data("nls_fa_active")) return e.data("nls_fa_tracked", 1), void e.data("nls_fa_active", 0);
                                var n = t.data("nls_fa_name"),
                                    i = this.getFormElementName(e),
                                    o = e.prop("type");
                                if (!i || "hidden" === e.prop("type").toLowerCase()) return e.data("nls_fa_tracked", 1), void e.data("nls_fa_active", 0);
                                var r = this.forms[n].fields.length;
                                this.forms[n].fields.push({
                                    name: i,
                                    type: o,
                                    interacted: 0,
                                    filledOut: 0,
                                    refilled: 0,
                                    timeHesitation: 0,
                                    timeInteraction: 0,
                                    fip: r
                                }), e.data("nls_fa_tracked", 1), e.data("nls_fa_active", 1), e.data("nls_fa_name", n), e.data("nls_fa_field_pos", r), Object.prototype.hasOwnProperty.call(this.forms[n].fields, r) && this.addChanges("edit", {
                                    a: "add",
                                    t: "element",
                                    d: this.forms[n].fields[r]
                                }, n)
                            } else this.addForm(t)
                        }
                    },
                    _addForm: function(e, t) {
                        li.addForm(ii(t))
                    },
                    loadForms: function() {
                        ii("form").each(this._addForm), ii("[nls_fa_name]:not(form)").each(this._addForm)
                    },
                    handleTracking: function(e) {
                        if (1 !== e.data("nls_fa_tracked") && this.addFormElement(e), e.data("nls_fa_tracked")) return 1 !== e.data("nls_fa_tracked") || 1 === e.data("nls_fa_active")
                    },
                    addChanges: function(e, t, n) {
                        if ("add_forms" === e) void 0 === this.changes.add_forms && (this.changes.add_forms = {}), this.changes.add_forms[n] = t;
                        else if ("edit" === e) {
                            var i = A.get("Array");
                            void 0 === this.changes.edit && (this.changes.edit = {}), void 0 === this.changes.edit[n] && (this.changes.edit[n] = new i), this.changes.edit[n].push(t)
                        }
                    },
                    staggerChanges: function() {
                        var e = {},
                            t = A.get("Array");
                        for (var n in this.changes)
                            if (Object.prototype.hasOwnProperty.call(li.changes, n))
                                if ("add_forms" === n) e.add_forms = li.changes.add_forms;
                                else if ("edit" === n)
                            for (var i in e.edit = {}, li.changes.edit)
                                if (Object.prototype.hasOwnProperty.call(li.changes.edit, i)) {
                                    e.edit[i] = new t;
                                    for (var o = 0; o < li.changes.edit[i].length; o++) {
                                        if (o + 1 !== li.changes.edit[i].length) {
                                            var r = li.changes.edit[i][o],
                                                a = li.changes.edit[i][o + 1];
                                            if ("edit" === r.a && r.fip === a.fip && r.d.pn === a.d.pn) continue
                                        }
                                        e.edit[i].push(li.changes.edit[i][o])
                                    }
                                }
                        return this.changes = {}, e
                    },
                    changeHandler: function(e) {
                        var t = ci(e, "change");
                        if (t) {
                            var n = t.data("nls_fa_name"),
                                i = t.data("nls_fa_field_pos"),
                                o = li.forms[n].fields[i].name;
                            0 < t.data("nls_fa_focus_time") && (li.forms[n].fields[i].timeHesitation += C() - t.data("nls_fa_focus_time"), li.addChanges("edit", {
                                a: "edit",
                                t: "element",
                                fip: i,
                                fname: o,
                                d: {
                                    pn: "timeHesitation",
                                    v: li.forms[n].fields[i].timeHesitation
                                }
                            }, n), t.data("nls_fa_focus_time", 0)), 0 < t.data("nls_fa_keydown_time") && (li.forms[n].fields[i].timeInteraction += C() - t.data("nls_fa_keydown_time"), li.addChanges("edit", {
                                a: "edit",
                                t: "element",
                                fip: i,
                                fname: o,
                                d: {
                                    pn: "timeInteraction",
                                    v: li.forms[n].fields[i].timeInteraction
                                }
                            }, n), t.data("nls_fa_keydown_time", 0)), t.data("nls_fa_mouseenter_time", 0), t.data("nls_fa_mousedown_time", 0), 1 === li.forms[n].fields[i].filledOut && (li.forms[n].fields[i].refilled = 1, li.addChanges("edit", {
                                a: "edit",
                                t: "element",
                                fip: i,
                                fname: o,
                                d: {
                                    pn: "refilled",
                                    v: li.forms[n].fields[i].refilled
                                }
                            }, n)), t.val() || 1 !== li.forms[n].fields[i].filledOut ? t.val() && 0 === li.forms[n].fields[i].filledOut && (li.forms[n].fields[i].filledOut = 1, li.addChanges("edit", {
                                a: "edit",
                                t: "element",
                                fip: i,
                                fname: o,
                                d: {
                                    pn: "filledOut",
                                    v: li.forms[n].fields[i].filledOut
                                }
                            }, n)) : (li.forms[n].fields[i].filledOut = 0, li.addChanges("edit", {
                                a: "edit",
                                t: "element",
                                fip: i,
                                fname: o,
                                d: {
                                    pn: "filledOut",
                                    v: li.forms[n].fields[i].filledOut
                                }
                            }, n))
                        }
                    },
                    blurHandler: function(e) {
                        var t = ci(e, "blur");
                        if (t) {
                            var n = t.data("nls_fa_name"),
                                i = t.data("nls_fa_field_pos"),
                                o = li.forms[n].fields[i].name;
                            0 < t.data("nls_fa_focus_time") && (li.forms[n].fields[i].timeHesitation += C() - t.data("nls_fa_focus_time"), li.addChanges("edit", {
                                a: "edit",
                                t: "element",
                                fip: i,
                                fname: o,
                                d: {
                                    pn: "timeHesitation",
                                    v: li.forms[n].fields[i].timeHesitation
                                }
                            }, n), t.data("nls_fa_focus_time", 0)), 0 < t.data("nls_fa_keydown_time") && (li.forms[n].fields[i].timeInteraction += C() - t.data("nls_fa_keydown_time"), li.addChanges("edit", {
                                a: "edit",
                                t: "element",
                                fip: i,
                                fname: o,
                                d: {
                                    pn: "timeInteraction",
                                    v: li.forms[n].fields[i].timeInteraction
                                }
                            }, n), t.data("nls_fa_keydown_time", 0)), t.data("nls_fa_mouseenter_time", 0), t.data("nls_fa_mousedown_time", 0)
                        }
                    },
                    focusHandler: function(e) {
                        var t = ci(e, "focus");
                        if (t) {
                            var n = t.data("nls_fa_name"),
                                i = t.data("nls_fa_field_pos"),
                                o = li.forms[n].fields[i].name;
                            0 === li.forms[n].interacted && (li.forms[n].interacted = 1, li.addChanges("edit", {
                                a: "edit",
                                t: "form",
                                d: {
                                    pn: "interacted",
                                    v: 1
                                }
                            }, n)), 0 === li.forms[n].fields[i].interacted && (li.forms[n].fields[i].interacted = 1, li.addChanges("edit", {
                                a: "edit",
                                t: "element",
                                fip: i,
                                fname: o,
                                d: {
                                    pn: "interacted",
                                    v: 1
                                }
                            }, n)), t.data("nls_fa_focus_time", C()), 0 < t.data("nls_fa_mouseenter_time") && (li.forms[n].fields[i].timeHesitation += C() - t.data("nls_fa_mouseenter_time"), li.addChanges("edit", {
                                a: "edit",
                                t: "element",
                                fip: i,
                                fname: o,
                                d: {
                                    pn: "timeHesitation",
                                    v: li.forms[n].fields[i].timeHesitation
                                }
                            }, n), t.data("nls_fa_mouseenter_time", 0))
                        }
                    },
                    mouseUpHandler: function(e) {
                        var t = ci(e, "mouseup");
                        if (t) {
                            var n = t.data("nls_fa_name"),
                                i = t.data("nls_fa_field_pos"),
                                o = li.forms[n].fields[i].name;
                            0 < t.data("nls_fa_mousedown_time") && (li.forms[n].fields[i].timeInteraction += C() - t.data("nls_fa_mousedown_time"), li.addChanges("edit", {
                                a: "edit",
                                t: "element",
                                fip: i,
                                fname: o,
                                d: {
                                    pn: "timeInteraction",
                                    v: li.forms[n].fields[i].timeInteraction
                                }
                            }, n), t.data("nls_fa_mousedown_time", 0))
                        }
                    },
                    mouseDownHandler: function(e) {
                        var t = ci(e, "mousedown");
                        t && (Fn.doBlurOnNode(t.get(0), li.blurHandler.bind({}, {
                            target: Fn.getFocusedNode()
                        })), t.data("nls_fa_mousedown_time", C()))
                    },
                    mouseLeaveHandler: function(e) {
                        var t = ci(e, "mouseleave");
                        if (!(!t || t.isFocussed && t.isFocussed() || t.is(":focus"))) {
                            var n = t.data("nls_fa_name"),
                                i = t.data("nls_fa_field_pos"),
                                o = li.forms[n].fields[i].name;
                            0 < t.data("nls_fa_mouseenter_time") && (li.forms[n].fields[i].timeHesitation += C() - t.data("nls_fa_mouseenter_time"), li.addChanges("edit", {
                                a: "edit",
                                t: "element",
                                fip: i,
                                fname: o,
                                d: {
                                    pn: "timeHesitation",
                                    v: li.forms[n].fields[i].timeHesitation
                                }
                            }, n), t.data("nls_fa_mouseenter_time", 0))
                        }
                    },
                    mouseEnterHandler: function(e) {
                        var t = ci(e, "mouseenter");
                        !t || t.isFocussed && t.isFocussed() || t.is(":focus") || t.data("nls_fa_mouseenter_time", C())
                    },
                    onFormSubmit: function() {
                        var e = ii(this);
                        li.trackFormSubmission(e)
                    },
                    keyDownHandler: function(e) {
                        var t = ci(e, "keydown");
                        if (t) {
                            var n = t.data("nls_fa_name"),
                                i = t.data("nls_fa_field_pos"),
                                o = li.forms[n].fields[i].name;
                            0 < t.data("nls_fa_focus_time") && (li.forms[n].fields[i].timeHesitation += C() - t.data("nls_fa_focus_time"), li.addChanges("edit", {
                                a: "edit",
                                t: "element",
                                fip: i,
                                fname: o,
                                d: {
                                    pn: "timeHesitation",
                                    v: li.forms[n].fields[i].timeHesitation
                                }
                            }, n), t.data("nls_fa_focus_time", 0)), 0 < t.data("nls_fa_keydown_time") && (li.forms[n].fields[i].timeInteraction += C() - t.data("nls_fa_keydown_time"), li.addChanges("edit", {
                                a: "edit",
                                t: "element",
                                fip: i,
                                fname: o,
                                d: {
                                    pn: "timeInteraction",
                                    v: li.forms[n].fields[i].timeInteraction
                                }
                            }, n)), t.data("nls_fa_keydown_time", C())
                        }
                    },
                    addMarketoListeners: function() {
                        window.MktoForms2 && window.MktoForms2.whenReady && !this.marketoListenerAdded && (this.marketoListenerAdded = !0, window.MktoForms2.whenReady(function(n) {
                            var t = li.f,
                                i = n.getFormElem();
                            i && Object.prototype.hasOwnProperty.call(i, "0") && Object.keys(t).map(function(e) {
                                return t[e]
                            }).some(function(e) {
                                for (var t in e)
                                    if (Object.prototype.hasOwnProperty.call(e, t) && document.querySelector(e[t]) === i[0]) return n.onSubmit(li.onFormSubmit.bind(i[0])), !0;
                                return !1
                            })
                        }))
                    },
                    loadFormEventListeners: function() {
                        var e = this;
                        setTimeout(function() {
                            if (D.marketoOptimization()) e.addMarketoListeners();
                            else if (window.MktoForms2 && window.MktoForms2.whenReady) {
                                var t = li.f,
                                    n = Object.keys(t).map(function(e) {
                                        return t[e]
                                    }).map(function(e) {
                                        for (var t in e)
                                            if (Object.prototype.hasOwnProperty.call(e, t)) return e[t]
                                    });
                                window.MktoForms2.whenReady(function(e) {
                                    var t = e.getFormElem();
                                    t && Object.prototype.hasOwnProperty.call(t, "0") && n.some(function(e) {
                                        return document.querySelector(e) == t[0]
                                    }) && e.onSubmit(li.onFormSubmit.bind(t[0]))
                                })
                            }
                        }, 0), de.addJqEventListener(ii(document), "on", "focus", li.focusHandler, this.formInputSelector), de.addJqEventListener(ii(document), "on", "blur", li.blurHandler, this.formInputSelector), de.addJqEventListener(ii(document), "on", "change", li.changeHandler, this.formInputSelector), de.addJqEventListener(ii(document), "on", "keydown", li.keyDownHandler, this.formInputSelector), de.addJqEventListener(ii(document), "on", "mouseenter", li.mouseEnterHandler, this.formInputSelector), de.addJqEventListener(ii(document), "on", "mouseleave", li.mouseLeaveHandler, this.formInputSelector), de.addJqEventListener(ii(document), "on", "mousedown", li.mouseDownHandler, this.formInputSelector), de.addJqEventListener(ii(document), "on", "mouseup", li.mouseUpHandler, this.formInputSelector), de.addJqEventListener(ii(document), "on", "submit", this.onFormSubmit, "form")
                    },
                    markSuccess: function(e, t) {
                        var n = ii(e),
                            i = li.getFormName(n);
                        i && (e = li.forms[i]) && (e.success = t, Z.ftu = !0, li.trackFormSubmission(n))
                    },
                    isFormSuccess: function(e) {
                        var t = li.forms[e];
                        return t && (0 === t.success || !1 === t.success) ? 0 : 1
                    },
                    trackFormSubmission: function(e) {
                        if (1 !== e.data("nls_fa_tracked") && this.addForm(e), 1 !== e.data("nls_fa_tracked") || 1 === e.data("nls_fa_active")) {
                            var t = e.data("nls_fa_name");
                            if (t && li.forms[t] && 1 !== li.forms[t].submitted) {
                                0 === li.forms[t].interacted && (li.forms[t].interacted = 1, li.addChanges("edit", {
                                    a: "edit",
                                    t: "form",
                                    d: {
                                        pn: "interacted",
                                        v: 1
                                    }
                                }, t)), li.isFormSuccess(t) ? (li.forms[t].submitted = 1, li.addChanges("edit", {
                                    a: "edit",
                                    t: "form",
                                    d: {
                                        pn: "submitted",
                                        v: 1
                                    }
                                }, t)) : li.forms[t].failed || (li.forms[t].failed = 1, li.addChanges("edit", {
                                    a: "edit",
                                    t: "form",
                                    d: {
                                        pn: "failed",
                                        v: 1
                                    }
                                }, t)), at.sendRecordingData();
                                for (var n = A.get("Date"), i = new n, o = i.getTime() + 300; i.getTime() < o;) i = new n
                            }
                        }
                    },
                    sendData: function() {
                        if (Z.fae) {
                            var e, t, n, i, o, r = li.lastSentForms,
                                a = li.staggerChanges();
                            for (var s in r || (li.lastSentForms = li.forms), a.edit)
                                if (Object.prototype.hasOwnProperty.call(a.edit, s))
                                    for (var d in n = a.edit[s], o = li.lastSentForms[s], n) Object.prototype.hasOwnProperty.call(n, d) && ("timeHesitation" !== (e = n[d]).d.pn && "timeInteraction" !== e.d.pn || (i = o && o.fields[e.fip] ? o.fields[e.fip][e.d.pn] : 0, (t = e.d.v - i) <= 0 || (a.edit[s][d].d.v = t)));
                            if (li.lastSentForms = ii.extend(!0, {}, li.forms), !ii.isEmptyObject(a)) {
                                var c = A.get("JSON"),
                                    l = {};
                                return Z.analyze && (li.f = li.f || {}, l.f = c.stringify(li.f), 0 === Object.keys(li.f).length ? l.fa_changes = c.stringify(li.f) : l.fa_changes = c.stringify(a)), l
                            }
                        }
                    },
                    init: function() {
                        var e = this;
                        at.formSubmitCallbacks.push(this.sendData.bind(this)), this.loadForms(), di || (this.loadFormEventListeners(), window.addEventListener("beforeunload", function() {
                            e.resetFormObservers()
                        }), di = !0), this.changes = {}, ni.init(li)
                    }
                };

                function ui() {
                    var e = document.querySelectorAll("select, input, label");
                    if (e.length) {
                        var t = jn.createEvent(document, "vwo_element_added");
                        t.data = e, jn.dispatchEvent(document, t)
                    }
                }

                function hi(e) {
                    e && (li._mapElements = e, ui())
                }
                Z.formAnalysis = li, Z.mapElements = hi;
                var fi = window.VWO = window.VWO || [],
                    mi = window.console || {
                        log: function() {},
                        error: function() {}
                    },
                    pi = {
                        processEvent: function(t, e) {
                            try {
                                var n = t[0],
                                    i = t.slice(1),
                                    o = -1 !== n.indexOf(".");
                                if (o && 0 === n.indexOf(e) || !o) {
                                    n = "VWO." + n;
                                    var r = eval(n),
                                        a = void 0;
                                    return a = n.split("."), a.splice(-1), a = eval(a.join(".")), r ? (r.apply(a, i), 1) : 0
                                }
                                return 0
                            } catch (e) {
                                return mi.error("Error occured in VWO Process Event (" + (t && t[0]) + "): ", e), 0
                            }
                        },
                        addPushListener: function(i) {
                            var o, r = fi.push;
                            fi.push = function() {
                                for (var e = [], t = 0; t < arguments.length; t++) e[t] = arguments[t];
                                var n = e[0];
                                return r.apply(fi, e), fi[fi.length - 1] === n && (o = pi.processEvent(n, i), fi[fi.length - 1] === n && o && fi.splice(-1)), fi.length
                            }
                        },
                        init: function(e) {
                            for (var t = 0; t < fi.length;) 1 === pi.processEvent(fi[t], e) ? fi.splice(t, 1) : t++;
                            pi.addPushListener(e)
                        }
                    };

                function gi(e, t) {
                    if (e.parentRule) {
                        var n = Array.apply(void 0, e.parentRule.cssRules).indexOf(e); - 1 < n ? t.unshift(n) : t.length = 0, gi(e.parentRule, t)
                    } else {
                        var i = Array.apply(void 0, e.parentStyleSheet.cssRules).indexOf(e); - 1 < i ? t.unshift(i) : t.length = 0
                    }
                }
                var vi = [],
                    wi = {
                        overRideAllInsertRules: function(e) {
                            wi.overRideInsertRule(e, CSSStyleSheet), "undefined" != typeof CSSGroupingRule ? wi.overRideInsertRule(e, CSSGroupingRule) : wi.overRideInsertRule(e, CSSMediaRule)
                        },
                        overRideInsertRule: function(s, e) {
                            var d = e.prototype.insertRule;
                            e.prototype.insertRule = function() {
                                for (var e = [], t = 0; t < arguments.length; t++) e[t] = arguments[t];
                                var n, i = [];
                                try {
                                    n = d.apply(this, e)
                                } catch (e) {
                                    throw new Error(e)
                                } finally {
                                    var o = this;
                                    if (this instanceof CSSRule && (o = this.parentStyleSheet), o.ownerNode && o.ownerNode.ownerDocument === document) {
                                        var r = {
                                            parentSelector: he(o.ownerNode),
                                            rule: e[0],
                                            index: e[1]
                                        };
                                        if (this instanceof CSSRule) try {
                                            var a = [];
                                            gi(this, a), r.ruleSelector = a
                                        } catch (e) {}
                                        i.push(r), Z.extractUrlsFromCss(r.rule), Z.Recording.deadClickManager.process(), D.repeatedCSSInsertion() ? vi.find(function(e) {
                                            return e.rule === r.rule && e.parentSelector === r.parentSelector
                                        }) || (vi.push(r), s.addMutation({
                                            time: C() - Z.startTime,
                                            insertedRules: [{
                                                addedStyles: i
                                            }]
                                        })) : s.addMutation({
                                            time: C() - Z.startTime,
                                            insertedRules: [{
                                                addedStyles: i
                                            }]
                                        })
                                    }
                                }
                                return n
                            }
                        },
                        processInsertRules: function() {
                            function e(e) {
                                var o = t[e];
                                [].forEach.call(o.cssRules, function(e, t) {
                                    var n = he(o.ownerNode),
                                        i = [];
                                    i.push({
                                        parentSelector: n,
                                        rule: e.cssText,
                                        index: t,
                                        shouldReplace: !0
                                    }), Z.extractUrlsFromCss(e.cssText), r.html.insertedRules.push({
                                        addedStyles: i
                                    })
                                })
                            }
                            for (var t = wi.getCurrentInsertedRules(), r = this, n = 0; n < t.length; n++) e(n)
                        },
                        getCurrentInsertedRules: function() {
                            var o = [];
                            return [].forEach.call(document.styleSheets, function(e) {
                                try {
                                    if (e.href || !e.cssRules || 0 === e.cssRules.length) return;
                                    var t = void 0;
                                    void 0 !== e.ownerNode.innerText ? t = e.ownerNode.innerText : void 0 !== e.ownerNode.innerHTML && (t = e.ownerNode.innerHTML);
                                    var n = t.match(/{/g) || [],
                                        i = wi.getTotalCSSRulesInStylesheet(e) || 0;
                                    n.length < i && o.push(e)
                                } catch (e) {}
                            }), o
                        },
                        getTotalCSSRulesInStylesheet: function(e) {
                            var t = 0;
                            try {
                                var n = e.cssRules;
                                if (n) {
                                    for (var i = 0; i < n.length; i++) t += wi.getTotalCSSRulesInStylesheet(n[i]);
                                    t += n.length
                                }
                            } catch (e) {}
                            return t
                        },
                        init: function(e) {
                            vi = [], this.overRideAllInsertRules(e)
                        }
                    },
                    _i = "http://www.w3.org/1999/xhtml",
                    yi = {
                        INPUT: 1,
                        TEXTAREA: 1,
                        SELECT: 1,
                        OPTION: 1,
                        CANVAS: 1,
                        "FLUTTER-VIEW": 1
                    },
                    Ei = A.get("Array"),
                    Ti = A.get("JSON"),
                    Si = 20,
                    bi = (Oi.containsBlobURL = function(e) {
                        return !!("STYLE" === e.tagName && -1 < e.innerHTML.indexOf("blob:") || "LINK" === e.tagName && e.href && -1 < e.href.indexOf("blob:"))
                    }, Oi.getBlobId = function(e) {
                        var t = null;
                        if ("STYLE" === e.tagName) {
                            var n = e.innerText.match(/@import\s+url\(['"]?(blob:[^'")\s]+)['"]?\)/g);
                            n && (t = n[0])
                        } else if ("LINK" === e.tagName)
                            for (var i = 0, o = ["href"]; i < o.length; i++) {
                                var r = o[i];
                                if (e[r] && "string" == typeof e[r] && -1 < e[r].indexOf("blob:")) {
                                    t = e[r];
                                    break
                                }
                            }
                        return t ? t.slice(-Si) : null
                    }, Oi.prototype.handleBlob = function(t) {
                        if (D.blobSupport() && Z.r && (this.shouldSendData && (this.sendPreloadedData(), this.shouldSendData = !1), Oi.containsBlobURL(t))) {
                            var n = this;
                            t.onload = function() {
                                var e = Oi.getData(t);
                                e && n.sendData(t, e)
                            }
                        }
                    }, Oi.prototype.sendPreloadedData = function() {
                        var i = this,
                            o = document.styleSheets;
                        Object.keys(o).forEach(function(e) {
                            var t = o[e];
                            if (Oi.containsBlobURL(t.ownerNode)) {
                                var n = Oi.getData(t.ownerNode);
                                if (!n) return;
                                i.sendData(t.ownerNode, n)
                            }
                        })
                    }, Oi.prototype.sendData = function(e, t) {
                        var n = Oi.getBlobId(e);
                        if (n) {
                            var i = P.getItem("blob") || []; - 1 === i.indexOf(n) && (i.push(n), P.setItem("blob", i), Z.htmlRequestSuccess ? at.sendB64(Ti.stringify({
                                blob: {
                                    key: n,
                                    value: t
                                }
                            })) : (this.queuedData || (this.queuedData = []), this.queuedData.push({
                                id: n,
                                value: t
                            })))
                        }
                    }, Oi.getData = function(e) {
                        var t, n, i = [],
                            o = null === (t = e.sheet) || void 0 === t ? void 0 : t.cssRules;
                        if (o)
                            if ("STYLE" === e.tagName)
                                for (var r = 0, a = o; r < a.length; r++) {
                                    var s = a[r];
                                    s instanceof CSSImportRule && null !== (n = s.styleSheet) && void 0 !== n && n.cssRules && -1 < s.href.indexOf("blob:") && Oi.pushData(s.styleSheet.cssRules, i)
                                } else "LINK" === e.tagName && Oi.pushData(o, i);
                        return 0 < i.length ? i.join("\n") : ""
                    }, Oi.pushData = function(n, i) {
                        Object.keys(n).forEach(function(e) {
                            var t = n[e];
                            t instanceof CSSStyleRule && null != t && t.cssText && i.push(t.cssText)
                        })
                    }, Oi.prototype.sendQueuedData = function() {
                        this.queuedData.forEach(function(e) {
                            var t = e.id,
                                n = e.value;
                            at.sendB64(Ti.stringify({
                                blob: {
                                    key: t,
                                    value: n
                                }
                            }))
                        }), this.queuedData = new Ei
                    }, Oi);

                function Oi() {
                    this.queuedData = new Ei, this.shouldSendData = !0
                }
                var Ri = new bi;

                function Ci() {
                    var e = A.get("Array");
                    this.nextId = 1, this.nodes = new e, this.ID_PROP = "nlsNodeId"
                }

                function Ni(e) {
                    var t = A.get("Array");
                    this.target = e || document, this.mutations = new t, this.index = 0, this.stagger = !0, this.knownNodes = new Ci
                }
                Ci.prototype.add = function(e, t, n) {
                    var i = 0;
                    if (Z.baseAdjustment.afterEl === e && (i = Z.baseAdjustment.offset), this.nextId = this.nextId + i, e[this.ID_PROP] = this.nextId, (n || F.ntDdClk(e) || 1 === yi[e.nodeName && e.nodeName.toUpperCase && e.nodeName.toUpperCase()]) && (e._vwo_nd = 1, n = !0), this.nodes[this.nextId] = e, this.nextId++, t && e.childNodes.length)
                        for (var o = e.firstChild; o; o = o.nextSibling) this.add(o, !0, n);
                    return e[this.ID_PROP]
                }, Ci.prototype.delete = function(e) {
                    var t = this.getId(e);
                    delete this.nodes[t], delete e[this.ID_PROP]
                }, Ci.prototype.getId = function(e) {
                    return e[this.ID_PROP]
                }, Ci.prototype.getNode = function(e) {
                    return this.nodes[e]
                }, Z.observerCallback = function(e) {
                    Z.Mutations.initialised && Z.Mutations.mutationProcessing(e)
                }, Ni.prototype.mutationProcessing = function(e, t) {
                    if (Z.GetHtml.domTraverCompl) {
                        var n = new(A.get("Array"));
                        e.forEach(function(e) {
                            (e = this.processMutation(e)) && n.push(e)
                        }, this), n.length && this.addMutation({
                            time: t || C() - Z.startTime,
                            mutations: n
                        })
                    } else Z.GetHtml.tempMutations.push({
                        time: C() - Z.startTime,
                        mutations: e
                    })
                }, Ni.prototype.processMutation = function(e) {
                    if (F.shouldIgnoreElement(e.target)) return null;
                    switch (e.type) {
                        case "childList":
                            return this.processChildList(e);
                        case "attributes":
                            return this.processAttributes(e);
                        case "characterData":
                            return this.processCharacterData(e);
                        default:
                            return
                    }
                }, Ni.prototype.processChildList = function(e) {
                    var t, n = A.get("Array"),
                        i = new n,
                        o = new n;
                    for (t = 0; t < e.addedNodes.length; t++) {
                        var r = e.addedNodes[t];
                        i = this.processAddedNode(r, i, e.target), r.namespaceURI === _i && r._vwo_vba_vis && Z.Recording.deadClickManager.process({
                            mutatedNode: r
                        })
                    }
                    for (t = 0; t < e.removedNodes.length; t++) {
                        if ((r = e.removedNodes[t]).namespaceURI === _i && r._vwo_vba_vis && Z.Recording.deadClickManager.process({
                                mutatedNode: r
                            }), (!r.parentNode || !r.parentElement) && this.knownNodes.getId(r)) {
                            var a = this.serializeNode(r);
                            a && o.push(a), this.knownNodes.delete(r)
                        }
                    }
                    return i.length || o.length ? {
                        type: "childList",
                        addedNodes: i,
                        removedNodes: o
                    } : null
                }, Ni.prototype.processAddedNode = function(t, e, n) {
                    var i, o = A.get("Array");
                    if (e = e || new o, t.matches && t.matches(Z.formAnalysis.formSelector) && Z.formAnalysis._addForm(null, t), t.tagName && ("input" === t.tagName.toLowerCase() || "select" === t.tagName.toLowerCase())) {
                        var r = jn.createEvent(document, "vwo_element_added");
                        r.data = t, jn.dispatchEvent(document, r)
                    }
                    Z.GetHtml.html.isAuraSite && "SLOT" === t.tagName && t.assignedElements && t.assignedElements().length && t.assignedElements().forEach(function(e) {
                        return e.nlsParent = t
                    });
                    var a = this.serializeNode(t);
                    if (!a) return e;
                    if (a.previousSibling = this.serializeNode(t.previousSibling), "undefined" != typeof ShadowRoot && t.parentNode instanceof ShadowRoot) a.parentNode = this.serializeNode(t.parentNode.host), a.isFragChild = !0, de.addEventListener(t, "scroll", Z.Recording.sendElementScrollData.bind(Z.Recording), !0);
                    else {
                        try {
                            Z.GetHtml.html.isAuraSite && "SLOT" === n.tagName && 0 < n.assignedElements().length && (t.nlsParent = n)
                        } catch (e) {}
                        a.parentNode = this.serializeNode(Z.GetHtml.html.isAuraSite && t.nlsParent || t.parentNode)
                    }
                    if (t.shadowRoot) {
                        a.hasShadow = !0, _o.observe(t.shadowRoot, yo);
                        var s = A.get("Array");
                        a.constructedSheets = new s, null !== (i = t.shadowRoot.adoptedStyleSheets) && void 0 !== i && i.forEach(function(e) {
                            for (var t, n = new s, i = 0; i < e.cssRules.length; i++) {
                                var o = null === (t = e.cssRules[i]) || void 0 === t ? void 0 : t.cssText;
                                Z.extractUrlsFromCss(o), n.push(o)
                            }
                            a.constructedSheets.push(n)
                        })
                    }
                    e.push(a);
                    for (var d = F.getFirstChild(t); d; d = F.getNextSibling(d)) e = this.processAddedNode(d, e);
                    return e
                }, Ni.prototype.processAttributes = function(e) {
                    var t = e.target,
                        n = e.attributeName,
                        i = F.getParentNode(t);
                    if (D.marketoOptimization() && t.tagName && "form" === t.tagName.toLowerCase() && "id" == n && Z.formAnalysis._addForm(null, t), !i || e.oldValue === t.getAttribute(n)) return null;
                    var o, r = !!t._vwo_vba_vis !== F.isNodeVisible(t); - 1 < ["style", "id", "class"].indexOf(n) && (t._vwo_vba_vis || r) && Z.Recording.deadClickManager.process({
                        mutatedNode: t
                    }), this.knownNodes.getId(i) || (o = "undefined" != typeof ShadowRoot && i instanceof ShadowRoot ? this.serializeNodeWrapper(i.host) : this.serializeNodeWrapper(i));
                    var a = this.serializeNode(t);
                    return a.type = "attributes", a.name = n, o && (a.parentData = o), a.value = "value" === n || "label" === n ? F.handleProtected(t, Z.Recording.anonymizeKeys, n) : t.getAttribute(n), F.shouldConsiderAnonymizingImgAttributes(n, a.value, t, Z.Recording.anonymizeKeys) && (a.value = F.getAnonymizedImageAttrValue({
                        name: n,
                        value: a.value
                    }, t) || a.value), "jetzoom-blank" !== t.className && "jetzoom-lens" !== t.className || 142952 !== window._vwo_acc_id && 230507 !== window._vwo_acc_id || (a.value = "", a.name = "vwo-" + n), a
                }, Ni.prototype.processCharacterData = function(e) {
                    var t = e.target,
                        n = F.getParentNode(t);
                    if (!n || !this.knownNodes.getId(n)) return null;
                    var i = this.serializeNode(t);
                    return i.type = "characterData", i.textContent = F.getNodeProperty(t, t.textContent, Z.Recording.anonymizeKeys, e.attributeName), i
                }, Ni.prototype.serializeNode = function(e) {
                    var t = A.get("Node"),
                        n = A.get("Array");
                    if (null === e || F.shouldIgnoreElement(e)) return null;
                    Z.extractSnapshottingUrls(e), F.isNodeVisible(e) ? e._vwo_vba_vis = !0 : delete e._vwo_vba_vis;
                    var i = this.knownNodes.getId(e);
                    if (i) return {
                        id: i
                    };
                    var o = {
                            nodeType: e.nodeType,
                            id: this.knownNodes.add(e)
                        },
                        r = !1;
                    switch (o.nodeType) {
                        case t.DOCUMENT_TYPE_NODE:
                            o.name = e.name, o.publicId = e.publicId, o.systemId = e.systemId;
                            break;
                        case t.COMMENT_NODE:
                        case t.TEXT_NODE:
                            o.textContent = F.getNodeProperty(e, e.textContent, Z.Recording.anonymizeKeys);
                            break;
                        case t.ELEMENT_NODE:
                            o.tagName = e.tagName, o.attributes = new n;
                            for (var a = 0; a < e.attributes.length; a++) {
                                var s = e.attributes[a];
                                "value" === s.name && (r = !0);
                                var d = "value" === s.name || "label" === s.name ? F.handleProtected(e, Z.Recording.anonymizeKeys, s.name) : s.value;
                                F.shouldConsiderAnonymizingImgAttributes(s.name, s.value, e, Z.Recording.anonymizeKeys) && (d = F.getAnonymizedImageAttrValue(s, e));
                                var c = Me.handleBase64Attr(e, s.name, d);
                                Ri.handleBlob(e), d = c.value;
                                var l = {
                                    name: s.name,
                                    value: d
                                };
                                c.options && (l.attributeOptions = c.options), o.attributes.push(l)
                            }
                            e.namespaceURI !== _i && (o.nonHtmlNameSpaceURI = e.namespaceURI), "INPUT" !== e.tagName || r || void 0 === e.value || "" === e.value || (o.value = F.handleProtected(e, Z.Recording.anonymizeKeys)), ze.trackCanvasNode(e)
                    }
                    return o
                }, Ni.prototype.serializeNodeWrapper = function(e) {
                    if (null !== e) {
                        var t = this.serializeNode(e);
                        return t && t.nodeType ? (e.parentNode && !this.knownNodes.getId(e.parentNode) && ("undefined" != typeof ShadowRoot && e.parentNode instanceof ShadowRoot ? t.parentData = this.serializeNodeWrapper(e.parentNode.host) : t.parentData = this.serializeNodeWrapper(e.parentNode)), t) : void 0
                    }
                }, Ni.prototype.addMutation = function(e) {
                    this.mutations.push(e)
                }, Ni.prototype.init = function(e, t) {
                    e && Ni.bind(this)();
                    var n = Z.GetHtml.muts.knownNodes;
                    if ("htmlTraversal" === t || n && n.nodes && n.nodes.length) this.knownNodes.nodes = n.nodes, this.knownNodes.nextId = n.nextId, this.knownNodes.ID_PROP = n.ID_PROP;
                    else {
                        Z.GetHtml.domTraverCompl = !0, this.knownNodes.add(this.target);
                        for (var i = this.target.firstChild; i; i = i.nextSibling) this.knownNodes.add(i, !0)
                    }
                    wi.init(this), ze.init(this), this.initialised = !0
                }, Z.Mutations = new Ni(document);
                var Di = Z.Mutations,
                    Ii = 0,
                    Ai;
                Ai = D.customOffsetEnabled() && window.Zone && window.Zone.__symbol__ ? window[window.Zone.__symbol__("setTimeout")] : window.setTimeout;
                var xi = location.search.match(/.*nodes=(\d*).*/),
                    ki = VWO._.ac && VWO._.ac.dNdOfst || 1e3,
                    Li;
                D.customOffsetEnabled() && (ki = 300), xi && (ki = parseInt(xi[1]));
                var Mi = [HTMLDivElement],
                    Pi = [],
                    Vi = {
                        MAINTAIN: "maintain",
                        SET: "set"
                    },
                    Wi;

                function Hi(e) {
                    return e.html.isAuraSite || 718021 <= window._vwo_acc_id
                }

                function Ui(e) {
                    return "undefined" != typeof customElements && "function" == typeof customElements.get && customElements.get(e)
                }

                function zi(e) {
                    if (!e) return e;
                    var t = e.nodeName && e.nodeName.toLowerCase();
                    if ("video" === t) {
                        var n = document.createElement("__nls-video");
                        return Wi.setAttributes(n, e.attributes), n
                    }
                    if ("img" !== t) {
                        if (Ui(t)) {
                            var i = document.createElement("__nls-custom-" + t);
                            return Wi.setAttributes(i, e.attributes), i
                        }
                        return e.cloneNode(!1)
                    }
                    var o = document.createElement("img");
                    return Wi.setAttributes(o, e.attributes, Wi.getAttributeName), o
                }

                function Fi(e) {
                    delete e.parentData, delete e.noDeadEnabled, delete e.shouldAnonymizeContent, delete e.whiteListedElementFound
                }

                function ji(e) {
                    if (!D.customOffsetEnabled())
                        if (e === Vi.MAINTAIN) {
                            for (var t = 0; t < Mi.length; t++) Pi.push(Mi[t].prototype.addEventListener), Mi[t].prototype.addEventListener = function() {};
                            window.$ && window.$.fn && (Li = window.$.fn.click, window.$.fn.click = function() {})
                        } else if (e === Vi.SET) {
                        window.$ && window.$.fn && Li && (window.$.fn.click = Li);
                        for (t = 0; t < Mi.length; t++) Mi[t].prototype.addEventListener = Pi[t];
                        Pi = []
                    }
                }

                function Xi(e, t) {
                    try {
                        t = Wi.add(e, t)
                    } catch (e) {
                        if (window.VWO._.customError) {
                            var n = e && e.stack || e + ".";
                            window.VWO._.customError({
                                msg: "DomConfig serializeNode : " + (Wi.serializeNode, !0) + ". Html Data on DomConfig: " + (Wi.html, !0) + ". " + n,
                                url: document.URL,
                                lineno: 131,
                                colno: 29,
                                source: "dIA"
                            })
                        }
                    }
                    return t
                }

                function Bi(r, e) {
                    var t, a = A.get("Array");
                    r.constructedSheets = new a, null !== (t = e.adoptedStyleSheets) && void 0 !== t && t.forEach(function(e) {
                        for (var t, n = new a, i = 0; i < e.cssRules.length; i++) {
                            var o = null === (t = e.cssRules[i]) || void 0 === t ? void 0 : t.cssText;
                            Z.extractUrlsFromCss(o), n.push(o)
                        }
                        r.constructedSheets.push(n)
                    })
                }

                function Yi(e, t, n, i, o, r, a) {
                    var s, d, c, l, u = e;
                    if (F.shouldIgnoreElement(e) && (e = e.nextSibling, u = e), Ii++, !e) return e;
                    if (e.nlsNodeId && Wi.html.isAuraSite && e.nlsRecId && e.nlsRecId === Z.ids.recording) return "false";
                    e.nlsRecId = Z.ids.recording;
                    var h = Wi;
                    Wi.offset = ki;
                    for (var f = ""; u;) {
                        if (!Hi(Wi) && (null != u && u.shadowRoot || "TEMPLATE" === u.tagName) && !u.childNodes.length) {
                            var m = document.createComment("Comment of VWO");
                            u.appendChild(m)
                        }
                        n = Xi(u, n), ji(Vi.MAINTAIN);
                        var p = zi(u);
                        ji(Vi.SET);
                        var g = A.get("Array"),
                            v = void 0;
                        if (t = t || "body" === u.nodeName.toLowerCase()) {
                            var w = Wi.handleTextNodes(p, u);
                            w && !w.nodeType ? v = Wi.serializeNode(w.newCommentNode, r, a, void 0, u) : p = w
                        }
                        var _ = Wi.serializeNode(p, r, a, u === document.doctype, u),
                            y = _ && _.data;
                        if (null === y) return null;
                        if (y.parentData = o, u.shadowRoot ? (y.hasShadow = !0, _o.observe(u.shadowRoot, yo), Bi(y, u.shadowRoot)) : "TEMPLATE" === u.tagName && u.content ? (y.hasFragment = !0, u.content.predecessor = u) : Z.GetHtml.html.isAuraSite && "SLOT" === u.tagName && u.assignedElements && u.assignedElements().forEach(function(e) {
                                e.nlsParent = u
                            }), (null == u ? void 0 : u.parentNode) instanceof DocumentFragment && (y.isFragChild = !0, de.addEventListener(u, "scroll", Z.Recording.sendElementScrollData.bind(Z.Recording), !0)), u.namespaceURI !== _i && (y.nonHtmlNameSpaceURI = u.namespaceURI), F.isNodeVisible(u) ? u._vwo_vba_vis = !0 : delete e._vwo_vba_vis, v && y.parentData.childNodes.push(v.data), r = _.shouldAnonymizeContent, a = _.whiteListedElementFound, y.noDeadEnabled = n, y.shouldAnonymizeContent = r, y.whiteListedElementFound = a, F.hasChild(u)) {
                            if (y.childNodes = new g, u = F.getFirstChild(u), o = y, Ii % ki == 0) {
                                Ai(function() {
                                    Yi(u, t, n, i, o, r, a)
                                }, 0), f = "false";
                                break
                            }
                            if ("false" == (f = Yi(u, t, n, i, o, r, a)) || "end" == f) break
                        }
                        for (var E = "", T = u; T; T = F.getParentNode(T)) {
                            y.parentData && y.parentData.childNodes && y.parentData.childNodes.push(y);
                            var S = y,
                                b = Z.GetHtml.html.isAuraSite && T.nlsParent || T.parentNode;
                            if (T.nextSibling) {
                                E = T.nextSibling, r = y.parentData.shouldAnonymizeContent, a = y.parentData.whiteListedElementFound, Fi(S);
                                break
                            }
                            if ("TEMPLATE" === (null == b ? void 0 : b.tagName) && null !== (s = null == b ? void 0 : b.content) && void 0 !== s && s.firstChild) {
                                E = null === (d = null == b ? void 0 : b.content) || void 0 === d ? void 0 : d.firstChild, b.content.predecessor = b, y.parentData.hasFragment = !0, r = y.parentData.shouldAnonymizeContent, a = y.parentData.whiteListedElementFound, Fi(S);
                                break
                            }
                            if (null != b && b.shadowRoot && null !== (c = null == b ? void 0 : b.shadowRoot) && void 0 !== c && c.firstChild) {
                                Hi(Wi) || (n = Xi(b.shadowRoot, n)), E = null === (l = null == b ? void 0 : b.shadowRoot) || void 0 === l ? void 0 : l.firstChild, r = y.parentData.shouldAnonymizeContent, a = y.parentData.whiteListedElementFound, Fi(S);
                                break
                            }
                            T === document ? h = y : (y = y.parentData, o = y.parentData, n = y.parentData.noDeadEnabled, r = y.parentData.shouldAnonymizeContent, a = y.parentData.whiteListedElementFound), Fi(S)
                        }
                        if (!(u = E)) {
                            f = "endComplete";
                            break
                        }
                    }
                    return "endComplete" === f ? (Ai(function() {
                        i(h)
                    }, 0), "end") : f
                }

                function qi(e, t, n, f) {
                    var m = !!(Wi = t).headEl,
                        p = !!Wi.bodyEl,
                        g = !1;
                    Wi.domTraverCompl = !1, Di.init(n, "htmlTraversal");
                    var v = Wi;
                    Ai(function() {
                        Yi(e, void 0, void 0, function(e) {
                            if (Wi = v, e && e.childNodes) {
                                for (var t = e.childNodes, n = 0; n < t.length; n++) {
                                    var i = Wi.html.html.present,
                                        o = Wi.html.doctype.present,
                                        r = t[n],
                                        a = r.tagName && "html" === r.tagName.toLowerCase();
                                    if (i) Wi.html.afterHtml.push(t[n]), Wi.html.afterHtml.reverse();
                                    else if ("doctype" === t[n].elem) Wi.html.doctype.present = !0, Wi.html.doctype.name = document.doctype.name, Wi.html.doctype.publicId = Wi.doctype.publicId, Wi.html.doctype.systemId = Wi.doctype.systemId, g = !0;
                                    else if (a) {
                                        for (var s = 0; s < r.childNodes.length; s++) {
                                            var d = r.childNodes[s].tagName,
                                                c = d && d.toLowerCase(),
                                                l = r.childNodes[s],
                                                u = Wi.html.body.present,
                                                h = Wi.html.head.present;
                                            p && u ? (Wi.html.afterBody.push(l), Wi.html.afterBody.reverse()) : m && !h && "head" !== c ? Wi.html.beforeHead.push(l) : !p || m && !h || "body" === c ? "head" !== c && "body" !== c || (Wi.html[c].nodes = l, Wi.html[c].attributes = Wi.html[c].nodes.attributes, Wi.html[c].present = !0) : Wi.html.beforeBody.push(l)
                                        }
                                        delete r.childNodes, Wi.html.html = r, Wi.html.html.present = !0
                                    } else i || (o || !g ? Wi.html.beforeHtml.push(t[n]) : o || Wi.html.beforeDoctype.push(t[n]))
                                }
                                f(!0)
                            }
                        }, {})
                    }, 0)
                }
                var Ki = v;

                function Gi(n, e, i) {
                    var o;
                    i = i || function() {
                        for (var e = [], t = 0; t < arguments.length; t++) e[t] = arguments[t];
                        return e[0]
                    }, Ki.each(e, function(e, t) {
                        o = i(t.name);
                        try {
                            n.setAttribute(o, t.value)
                        } catch (e) {}
                    })
                }

                function Ji(e) {
                    switch (e) {
                        case "src":
                            e = "__nls-src";
                            break;
                        case "srcset":
                            e = "__nls-srcset"
                    }
                    return e
                }

                function $i() {
                    var e, t, n;
                    this.doctype = {}, this.htmlEl = {}, this.headEl = {}, this.bodyEl = {};
                    var i = A.get("Array"),
                        o = window._vwo_code;
                    this.html = {
                        version: 3,
                        timedout: o && o.lT ? 1 : o && o.sT ? 2 : 0,
                        idleToAction: Z.saveNewRecordingInitiatedOnce,
                        beforeDoctype: new i,
                        insertedRules: new i,
                        isAuraSite: -1 < (null === (t = null === (e = window.Aura) || void 0 === e ? void 0 : e.app) || void 0 === t ? void 0 : t.indexOf("siteforce")) || "function" == typeof(null === (n = window.LWR) || void 0 === n ? void 0 : n.define) || void 0 !== window.embedded_svc,
                        doctype: {
                            present: !1,
                            name: "",
                            publicId: "",
                            systemId: ""
                        },
                        beforeHtml: new i,
                        html: {
                            present: !1,
                            attributes: {}
                        },
                        beforeHead: new i,
                        head: {
                            present: !1,
                            attributes: {},
                            html: ""
                        },
                        beforeBody: new i,
                        body: {
                            present: !1,
                            attributes: {},
                            html: ""
                        },
                        afterBody: new i,
                        afterHtml: new i
                    };
                    try {
                        this.html.lsAllowed = "undefined" != typeof Storage && !!window.localStorage
                    } catch (e) {
                        this.html.lsAllowed = !1
                    }
                    this.muts = {
                        knownNodes: {
                            nextId: 1,
                            nodes: new i,
                            ID_PROP: "nlsNodeId",
                            noDeadEnabled: !1
                        }
                    };
                    var r = D.ignoredSelectors();
                    r && (this.html.ISFI = r), this.tempNextId = -1, this.tempNodes = [], this.tempMutations = new i, this.setAttributes = Gi, this.getAttributeName = Ji
                }
                $i.prototype.processMutsOccrdPrvsly = function() {
                    var e = this.tempMutations;
                    if (Z.Mutations.knownNodes.nextId = this.muts.knownNodes.nextId, e.length)
                        for (var t = 0; t < e.length; t++) Z.Mutations.mutationProcessing(e[t].mutations, e[t].time)
                }, $i.prototype.add = function(e, t) {
                    var n = 0;
                    Z.baseAdjustment.afterEl === e && (n = Z.baseAdjustment.offset);
                    var i = this.muts.knownNodes;
                    return i.nextId = i.nextId + n, e[i.ID_PROP] = i.nextId, (t || F.ntDdClk(e) || 1 === yi[e.nodeName && e.nodeName.toUpperCase && e.nodeName.toUpperCase()]) && (e._vwo_nd = 1, t = !0), i.nodes[i.nextId] = e, i.nextId++, t
                }, $i.prototype.serializeNode = function(e, t, n, i, o) {
                    var r, a = {
                            data: {},
                            shouldAnonymizeContent: t,
                            whiteListedElementFound: n
                        },
                        s = A.get("Node");
                    if (null === e) return null;
                    var d = {
                        nodeType: e.nodeType
                    };
                    switch (Z.extractSnapshottingUrls(o), Z.extractCanonicalUrl(o), d.nodeType) {
                        case s.COMMENT_NODE:
                        case s.TEXT_NODE:
                            d.textContent = e.textContent, (a.shouldAnonymizeContent = t) && d.textContent && d.textContent.trim() && "STYLE" !== (null === (r = null == o ? void 0 : o.parentNode) || void 0 === r ? void 0 : r.tagName) && (643262 === window._vwo_acc_id && o.parentElement && o.parentElement.classList && o.parentElement.classList.contains("label_hold") && (o.parentElement.style.width = "auto"), d.textContent = F.getMaskedValue(d.textContent));
                            break;
                        case s.ELEMENT_NODE:
                            F.isElementWhitelisted(o) ? n = !(t = !1) : !n && F.isElementBlacklisted(o, Z.Recording.anonymizeKeys) && (t = !0), d.tagName = e.tagName, d.attributes = {};
                            for (var c = 0; c < e.attributes.length; c++) {
                                var l = e.attributes[c];
                                if ("OPTION" === e.tagName || "INPUT" === e.tagName) {
                                    if ("value" === l.name)(h = vwo_$(e, this.bodyEl)).attr("value", F.handleProtected(o, Z.Recording.anonymizeKeys)), e.attributes[c].value = h.attr("value");
                                    if ("label" === l.name)(h = vwo_$(e, this.bodyEl)).attr("label", F.handleProtected(o, Z.Recording.anonymizeKeys, "label")), e.attributes[c].value = h.attr("label")
                                }
                                F.shouldConsiderAnonymizingImgAttributes(l.name, l.value, e, Z.Recording.anonymizeKeys) && (e.attributes[c].value = F.getAnonymizedImageAttrValue(l, o));
                                var u = Me.handleBase64Attr(e, l.name, l.value);
                                Ri.handleBlob(e), d.attributes[l.name] = u.value, u.options && (d.attributeOptions = d.attributeOptions || {}, d.attributeOptions[l.name] = u.options)
                            }
                            var h;
                            if ("INPUT" === e.tagName && void 0 === d.attributes.value && void 0 !== e.value && "" !== e.value)(h = vwo_$(e, this.bodyEl)).attr("value", F.handleProtected(o, Z.Recording.anonymizeKeys)), d.value = h.attr("value");
                            "INPUT" !== e.tagName || "radio" !== e.type && "checkbox" !== e.type || (d.isChecked = Ki(e).isChecked && Ki(e).isChecked() || Ki(e).is(":checked")), ze.trackCanvasNode(o), a.shouldAnonymizeContent = t, a.whiteListedElementFound = n
                    }
                    return a.data = d, i && (a.data.elem = "doctype"), a
                }, $i.prototype.handleBaseElement = function() {
                    for (var e, t = document.getElementsByTagName("base"), n = 0; n < t.length; n++)
                        if ((e = t[n]).hasAttribute("href")) {
                            var i = e.getAttribute("href");
                            null != i && (this.html.base = 1)
                        }
                    this.html.base ? Z.baseAdjustment = {
                        offset: 0
                    } : this.headEl ? Z.baseAdjustment = {
                        offset: 1,
                        afterEl: this.headEl.childNodes[0]
                    } : Z.baseAdjustment = {
                        offset: 2,
                        afterEl: this.bodyEl
                    }
                }, $i.prototype.handleTextNodes = function(e, t) {
                    var n = A.get("Node");
                    if (e.nodeType === n.TEXT_NODE && "" === e.textContent) {
                        if (t.previousSibling && "" === t.previousSibling.textContent) return e;
                        e = document.createComment("!!-nlsTN-!!")
                    } else if (e.nodeType === n.TEXT_NODE && t.previousSibling && t.previousSibling.nodeType === n.TEXT_NODE && "" !== t.previousSibling.textContent) return {
                        nodeRet: e,
                        newCommentNode: document.createComment("!!-nlsCN-!!")
                    };
                    return e
                }, $i.prototype.processBaseElement = function() {
                    this.handleBaseElement()
                }, $i.prototype.init = function(e, t) {
                    e && $i.bind(this)(), Z && !D.preloadListeners() && (Z.startTime = C()), this.doctype = document.doctype, this.htmlEl = document.getElementsByTagName("html")[0], this.headEl = document.getElementsByTagName("head")[0], this.bodyEl = document.getElementsByTagName("body")[0], this.headEl && this.handleBaseElement(), wi.processInsertRules.call(this), qi(document, this, e, t)
                }, Z.GetHtml = new $i, Z.GetHtml.__ = 1;
                var Qi = Z.GetHtml,
                    Zi = {
                        INIT: "nls.init"
                    },
                    eo = v;

                function to() {
                    Z.recordingData.totals.touches++
                }

                function no() {
                    Z.recordingData.totals.keyPresses++
                }

                function io(e) {
                    "HTML" !== e.target.nodeName && Z.recordingData.totals.clicks++
                }

                function oo(e) {
                    var t = e.target.nodeName,
                        n = e.pageX,
                        i = e.pageY;
                    "HTML" !== t && (Z.recordingData.mouse.lastMove.docX === n && Z.recordingData.mouse.lastMove.docY === i || (Z.recordingData.totals.movements++, Z.recordingData.mouse.lastMove.docX = n, Z.recordingData.mouse.lastMove.docY = i))
                }
                var ro = N(at.sendRecordingData.bind(at), 100);

                function ao() {
                    var e = Z.getScrollPercentage();
                    e > Z.recordingData.totals.scroll && 0 < e && (Z.recordingData.totals.scroll = e, ro())
                }

                function so() {
                    de.addJqEventListener(eo(window), "on", "scroll", ao), de.addEventListener(document, "mouseup", io), de.addEventListener(document, "keyup", no), de.addEventListener(document, "mousemove", oo), de.addEventListener(document, "touchstart", to)
                }
                var co = {
                    init: so,
                    __: 1
                };
                Z.EventListeners = co, co.__ = 1;
                var lo = function() {
                        var e = g.get("nlssid" + Z.ids.account);
                        e ? (Z.ids.session = parseInt(e, 10), Z.returnVisitor = !0) : (Z.ids.session = window._vwo_pa.sId, g.create("nlssid" + Z.ids.account, Z.ids.session, null, Z.getCookieDomain()), Z.returnVisitor = !1)
                    },
                    uo = function() {
                        var e = window.VWO._.cookies,
                            t = e.get(Ce.TRACK_SESSION_COOKIE_NAME);
                        if (t) {
                            var n = ":referrer=",
                                i = (t = decodeURIComponent(t)).split(n),
                                o = "http:" === document.location.protocol && document.location.href === document.referrer ? "" : document.referrer;
                            btoa && (o = btoa(o)), 1 < i.length && (t = i[0] + "", o = i[i.length - 1] + ""), e.create(Ce.TRACK_SESSION_COOKIE_NAME, t + n + o, Ce.TRACK_SESSION_COOKIE_EXPIRY, Z.getCookieDomain())
                        }
                    },
                    ho = function() {
                        var e = parseInt(g.get("nlsrid" + Z.ids.account), 10);
                        e = isNaN(e) ? 1 : e + 1, Z.ids.recording = e, g.create("nlsrid" + Z.ids.account, e, 365, Z.getCookieDomain())
                    },
                    fo = !1,
                    mo = function() {
                        var e = navigator.userAgent.toLowerCase();
                        return -1 !== e.indexOf("msie") && parseInt(e.split("msie")[1], 10)
                    },
                    po = function() {
                        var e = {},
                            t = Z.tags.eTags.get(),
                            n = A.get("JSON"),
                            i = {
                                f: Z.tags.eTagsV2.f.get(),
                                r: Z.tags.eTagsV2.r.get(),
                                h: Z.tags.eTagsV2.h.get()
                            },
                            o = Z.tags.uTags.get();
                        return t && (e.eTags = n.stringify(t)), (i.f || i.r || i.h) && (e.eTagsV2 = n.stringify(i)), o && (e.uTags = n.stringify(o)), e
                    };
                Z.getTags = po, Z.campaignsProcessed = !1;
                var go = null;
                D.data360migrated() && VWO.phoenix && window.fetcher && (window.fetcher.getValue("VWO._.auxPageViewFired").then(function(e) {
                    e && (Z.campaignsProcessed = !0, go && go(), go = null)
                }), VWO.phoenix('on("${{1}}", "${{2}}")', null, {
                    captureGroups: [ut.PAGE_VIEW, function(e) {
                        e.aux && (Z.campaignsProcessed = !0, go && go(), go = null)
                    }]
                }));
                var vo = function(i, o) {
                        var e, t, n;
                        /bot|googlebot|crawler|spider|robot|crawling/i.test(navigator.userAgent) || (Z.ids.session || (lo(), ho()), uo(), void 0 !== Z.ids.session && void 0 !== Z.returnVisitor && (mo() ? (i.push("Mutations"), r(!1)) : (Z.GetHtml.html.isAuraSite = -1 < (null === (t = null === (e = window.Aura) || void 0 === e ? void 0 : e.app) || void 0 === t ? void 0 : t.indexOf("siteforce")) || "function" == typeof(null === (n = window.LWR) || void 0 === n ? void 0 : n.define) || void 0 !== window.embedded_svc, D.preloadListeners() && (Z.startTime = C(), Z.Recording.init(o), i = i.filter(function(e) {
                            return "Recording" !== e
                        })), Z.GetHtml.init(o, r))));

                        function r(e) {
                            for (var t in fo || (Z.EventListeners.init(), fo = !0), at.formSubmitCallbacks.push(po), i) Object.prototype.hasOwnProperty.call(i, t) && Z[i[t]].init(o);
                            e && (Z.GetHtml.domTraverCompl = !0, Z.GetHtml.processMutsOccrdPrvsly()), Z.triggerLibEvent(Zi.INIT);

                            function n() {
                                at.saveNewRecording(function() {
                                    Z.saveNewRecordingInitiatedOnce = !0, Z.sRD = Z.sRD || N(at.sendRecordingData.bind(at), Z.cDT);
                                    var e = setInterval(Z.sRD, Z.heartBeatFrequency);
                                    de.pushTimers(e, "interval"), Z.htmlRequestSuccess = !0, Me.sendQueuedData(), Ri.sendQueuedData()
                                })
                            }
                            D.data360migrated() && !Z.campaignsProcessed ? go = n : (go = null, setTimeout(n, 0)), Re.isRequestQueueable() && setInterval(Re.deQueueRequest, 1e3)
                        }
                    },
                    wo = {
                        init: vo
                    },
                    _o, yo = {
                        childList: !0,
                        attributes: !0,
                        attributeOldValue: !0,
                        characterData: !0,
                        subtree: !0
                    },
                    Eo = v;
                Qi.__ = 1;
                var To = window._vwo_exp || {},
                    So = window._vwo_exp_ids || [],
                    bo = "ANALYZE_HEATMAP",
                    Oo = "ANALYZE_RECORDING",
                    Ro = "ANALYZE_FORM",
                    Co = {},
                    No = window.VWO,
                    Do = No.data && No.data.mrp || 20,
                    Io, Ao = !0,
                    xo = !1,
                    ko = !1,
                    Lo = !1,
                    Mo = !1;
                No._.customEvent = No._.customEvent || function() {};
                var Po = function() {
                        for (var e in No._) {
                            if (Object.prototype.hasOwnProperty.call(No._, e) && "function" == typeof No._[e])
                                if (0 <= No._[e].toString().indexOf('unshift(["jI"]')) return No._[e]
                        }
                    },
                    Vo = No._.triggerEvent || Po() || function() {};
                Co[Ro] = "fe", Co[bo] = "he", Co[Oo] = "re";
                var Wo = function(e) {
                        return window._vwo_pa = window._vwo_pa || {}, !(!window._vwo_pa || !window._vwo_pa[e] || 1 !== window._vwo_pa[e].r && 1 !== window._vwo_pa[e].fae && 1 !== window._vwo_pa[e].hs)
                    },
                    Ho = function(e) {
                        if (To[e]) {
                            var t = To[e].type;
                            return 0 <= [bo, Oo].indexOf(t) ? (To[e].main && (Z.hs = Z.hs || t === bo, Z.r = Z.r || t === Oo), !0) : t === Ro && (Z.fae = !0)
                        }
                    },
                    Uo = function() {
                        for (var e, t = So.length; t--;)
                            if (e = To[So[t]].type, 0 <= [bo, Oo, Ro].indexOf(e)) return !0;
                        return !1
                    },
                    zo = function() {
                        Z.stopRecording = Z.enums.formAnalysis.PERMANENT_STATE
                    },
                    Fo = function() {
                        ko = !1, Z.saveNewRecordingInitiatedOnce = !1, Lo = !1, Z.htmlRequestSuccess = !1, Z.lastSendTime = 0, Z.formAnalysis && (Z.formAnalysis.f = {}), Z.ErrorLogger.resetData(), Z.formAnalysis && Z.formAnalysis.resetFormObservers(), Z.enableEventListeners = !1, ze.stopCanvasRecording(), Z.campaignsProcessed = !1
                    },
                    jo = function(e, t, n, i, o) {
                        var r = g.get("nlssid" + Z.ids.account);
                        if (!Uo() && r) Z.ids.session = parseInt(r, 10), Z.ids.recording = parseInt(g.get("nlsrid" + Z.ids.account) || 1, 10), Z.returnVisitor = !0;
                        else {
                            var a = o || t;
                            if (Do < a) return void zo();
                            i && (P.clear(), Z.stopRecording = ""), Z.ids.session = e, Z.ids.recording = a, Z.returnVisitor = n, Z.newSession = !!i, Z.newSession && (Z.startTime = C()), n && Fo(), ko = !0, Lo || Io(), Ao && r && (g.erase("nlssid" + Z.ids.account, Z.getCookieDomain()), g.erase("nlsrid" + Z.ids.account, Z.getCookieDomain())), Ao = !1
                        }
                    },
                    Xo = function() {
                        Z.ready = !1, Z.r = !1, Z.hs = !1, Z.fae = !1, Z.resetTagOnUC(), Z.ids.experiment = {}, ko = !1, Z.saveNewRecordingInitiatedOnce = !1, Lo = !1, Z.htmlRequestSuccess = !1, Z.Recording.resetData(), Z.ids.re = {}, Z.formAnalysis && (Z.formAnalysis.f = {}), Z.ids.he = {}, Z.ids.fe = {}, Z.canonicalUrl = "", Z.ErrorLogger.resetData(), Z.enableEventListeners = !1, Z.formAnalysis && Z.formAnalysis.resetFormObservers(), ze.stopCanvasRecording(), Z.campaignsProcessed = !1
                    },
                    Bo = function(e) {
                        var t, n, i, o, r = e[0];
                        if ("rH" === r || "vS" === r) {
                            if (t = e[1], n = +e[2] || 1, i = Ho(t), !Z.ids.experiment[t] && (Wo(t) || i))
                                if (i) {
                                    if (D.campaignLog()) try {
                                        window.VWO._.customError({
                                            msg: "Analyze Campaign started",
                                            url: "nls/init.ts",
                                            lineno: 228,
                                            colno: 25,
                                            source: JSON.stringify({
                                                uuid: F.getUUID(),
                                                sn: window.VWO._.cookies.get(Ce.TRACK_SESSION_COOKIE_NAME),
                                                ds: window.VWO._.cookies.get(Ce.TRACK_GLOBAL_COOKIE_NAME),
                                                sId: window.VWO._.sessionInfoService.getSessionId(),
                                                campaignId: t
                                            })
                                        })
                                    } catch (e) {}
                                    if (o = To[t].type, To[t].main || o === Ro) Z.ids[Co[o]][t] = F.getUUID(), Z.ready = !0, Z.analyze = !0, o === Ro ? (Z.tags.eTags.add(t, n), Z.tags.eTagsV2.f.add(t)) : o === Oo && (Z.Recording.anonymizeKeys = void 0 !== To[t].aK ? To[t].aK : Z.Recording.anonymizeKeys, Z.Recording.bl = To[t].bl, Z.Recording.wl = To[t].wl, Z.Recording.cnv = To[t].cnv), Lo || Io();
                                    else switch (Z.tags.eTags.add(t, n), o) {
                                        case bo:
                                            Z.tags.eTagsV2.h.add(t);
                                            break;
                                        case Oo:
                                            Z.tags.eTagsV2.r.add(t)
                                    }
                                } else Z.Recording.anonymizeKeys = Z.Recording.anonymizeKeys || window._vwo_pa[t].aK, Z.fae = Z.fae || window._vwo_pa[t].fae, Z.hs = Z.hs || window._vwo_pa[t].hs, Z.r = Z.r || window._vwo_pa[t].r, Z.ids.experiment[t] = F.getUUID(), Z.ready = !0, Lo || Io()
                        } else "tSC" === r ? jo(e[1], e[2], e[3], e[4], e[5]) : "tSE" === r ? zo() : "uC" === r && (at.sendRecordingData(!1, Z.Recording && Z.Recording.previousUrl), Xo())
                    },
                    Yo = function(e) {
                        Eo.each(e, function(e, t) {
                            Bo(t)
                        })
                    },
                    qo = function() {
                        window._vwo_evq = window._vwo_evq || [], Yo(window._vwo_evq);
                        var n = window._vwo_evq.push,
                            i = window._vwo_evq.unshift;
                        window._vwo_evq.push = function() {
                            for (var e = [], t = 0; t < arguments.length; t++) e[t] = arguments[t];
                            return Bo(e[0]), n.apply(window._vwo_evq, e)
                        }, window._vwo_evq.unshift = function() {
                            for (var e = [], t = 0; t < arguments.length; t++) e[t] = arguments[t];
                            return Bo(e[0]), i.apply(window._vwo_evq, e)
                        }
                    },
                    Ko = function() {
                        return window.Worker && window.URL && window.Blob
                    },
                    Go = function() {
                        return !!Z.ready && (!(Z.analyze && !ko) && (!(!Z.faultyWorker && !Z.ignoreWorker) || !!Ko() && !!Z.workerUrl))
                    };

                function Jo(e) {
                    Z.observerCallback(e)
                }

                function $o() {
                    if (!Z.__mutObserverAtcd) {
                        Z.__mutObserverAtcd = 1;
                        var e = void 0;
                        if ("undefined" != typeof MutationObserver ? e = MutationObserver : "undefined" != typeof WebKitMutationObserver && (e = window.WebKitMutationObserver), !e) return;
                        D.customOffsetEnabled() && window.Zone && window.Zone.__symbol__ && (e = window[window.Zone.__symbol__("MutationObserver")]), (_o = new e(Jo.bind(this))).observe(document, yo), window.addEventListener("beforeunload", function() {
                            _o.disconnect()
                        })
                    }
                }
                var Qo = function(e) {
                        $o(), !Lo && Go() && Mo && (Lo = !0, Z.ready && Z.r && Vo(ut.RECORDING_INITIATED), wo.init(e, xo), xo = !0)
                    },
                    Zo = function(e, t, n) {
                        var i = new XMLHttpRequest;
                        i.open("GET", e, !0), i.onload = function() {
                            200 === this.status || 304 === this.status ? t && t(this.response) : window.VWO._.gcpfb && window.VWO._.gcpfb(e, Zo, this.status, t, n) || 200 <= this.status && this.status < 400 && t && t(this.response)
                        }, i.onerror = function() {
                            window.VWO._.gcpfb && window.VWO._.gcpfb(e, Zo, this.status, t, n) || n && n(this.response)
                        }, i.send(null)
                    },
                    er = function(n) {
                        var e, t, i, o, r, a;
                        D.preloadListeners() && (Z.ignoreWorker = !0, Mo = !0), Io = N(function() {
                            Qo(n)
                        }, 50), qo();
                        var s = (null === (a = null === (r = null === (o = null === (i = null === (t = null === (e = window.VWO._) || void 0 === e ? void 0 : e.allSettings) || void 0 === t ? void 0 : t.dataStore) || void 0 === i ? void 0 : i.plugins) || void 0 === o ? void 0 : o.LIBINFO) || void 0 === r ? void 0 : r.WORKER) || void 0 === a ? void 0 : a.HASH) || window._vwo_worker_cb,
                            d = s ? "-" + s : "";
                        if (!No.nls) {
                            No.nls = Z;
                            var c = window._vwo_worker_url || window._vwo_cdn + "analysis/4.0/worker" + d + ".js";
                            return Ko() && Zo(c, function(e) {
                                try {
                                    var t = new window.Blob([e], {
                                        type: "text/javascript"
                                    });
                                    Z.workerUrl = window.URL.createObjectURL(t)
                                } catch (e) {
                                    Z.faultyWorker = !0
                                }
                                Qo(n)
                            }), Eo(document).ready(function() {
                                setTimeout(function() {
                                    Mo = !0, Qo(n), pi.init("nls")
                                }, 500)
                            }), window._vwo_evq = window._vwo_evq || [], Z.ajax = Se, Re.init(), window.__nls = Z
                        }
                    },
                    tr = Object.freeze({
                        __proto__: null,
                        get mutationObserver() {
                            return _o
                        },
                        mutationConfig: yo,
                        init: er
                    });
                dn.__es6hack = 1, li.__es6hack = 1, er(["Recording", "formAnalysis", "ErrorLogger"])
            }()
    })
}();