(function() {
    "use strict";

    function e(e) {}
    const t = self;

    function i(e, t, i, n) {
        return new(i || (i = Promise))((function(s, r) {
            function o(e) {
                try {
                    l(n.next(e))
                } catch (e) {
                    r(e)
                }
            }

            function a(e) {
                try {
                    l(n.throw(e))
                } catch (e) {
                    r(e)
                }
            }

            function l(e) {
                var t;
                e.done ? s(e.value) : (t = e.value, t instanceof i ? t : new i((function(e) {
                    e(t)
                }))).then(o, a)
            }
            l((n = n.apply(e, t || [])).next())
        }))
    }
    window.setInterval = t.setInterval.bind(t), window.setTimeout = t.setTimeout.bind(t), window.clearInterval = t.clearInterval.bind(t), window.clearTimeout = t.clearTimeout.bind(t), window.addEventListener = t.addEventListener.bind(t), window.encodeURIComponent = t.encodeURIComponent.bind(t), window.decodeURIComponent = t.decodeURIComponent.bind(t), window.JSON = self.JSON, document = window.document, window.window = window, t._onMessage = e, window.workerThread = t, window.VWO.modules = {
        vwoUtils: {
            cookies: {}
        },
        utils: {},
        tags: {},
        phoenixPlugins: {
            events: {
                predefinedEvents: {}
            }
        }
    };
    class n {
        formatErrorObject(e) {
            return "string" == typeof e && (e = {
                msg: e
            }), e
        }
        setCustomError(e) {
            const t = this;
            window.VWO._.customError = function(i) {
                i = t.formatErrorObject(i), e(i)
            }
        }
    }
    const s = e => {
        try {
            window.VWO._.customError(e)
        } catch (e) {}
    };
    let r;
    const o = {
            test: e => {
                var t;
                return r = null === (t = window.VWO) || void 0 === t ? void 0 : t.phoenix, window.workerThread && r && e === r.store.getters
            },
            transformer: function(e) {
                return e === r.store.getters.settings.campaigns || e === r.store.getters.allSettings.dataStore.campaigns ? "vwojFnGPlugCamp" : e === r.store.getters.allSettings ? "vwojFnGPlugAllSet" : e
            },
            parse: (e, t) => {
                if ("vwojFnGPlugCamp" === t) return window._vwo_exp;
                if ("vwojFnGPlugAllSet" === t) {
                    const e = Object.assign({}, window.VWO._.allSettings);
                    return delete e.triggers, delete e.tags, e
                }
                return t
            }
        },
        a = [o],
        l = {
            stringify: function(e, t, i) {
                try {
                    return JSON.stringify(e, (function(e, n) {
                        if (!i) {
                            const e = a.filter((e => e.test(n)));
                            if (e.length > 0) {
                                const i = t => e.reduce(((e, t) => t.transformer(e)), t);
                                return JSON.parse(l.stringify(n, t, i))
                            }
                        }
                        i && (n = i(n));
                        const s = e ? this : t;
                        var r;
                        return n instanceof Function || "function" == typeof n ? n.type === "vwoWrappedFn_" + (window.mainThread ? "WT" : "MT") ? "_NuPreW" + n.name.slice(0, n.name.indexOf("_") + 1) : (r = n.toString()).length < 8 || "function" !== r.substring(0, 8) ? "_NuFrRa" + window.functionWrapper.wrap(n, s) + "_" : "_NuFrNf" + window.functionWrapper.wrap(n, s) + "_" : n instanceof RegExp ? "_PxEgEr_" + n : n
                    }))
                } catch (e) {
                    return s({
                        msg: "JSONfn.stringify failed!",
                        url: "jsonFn.ts",
                        source: e
                    }), ""
                }
            },
            parse: function(e, t) {
                if (!e) return e;

                function i(e) {
                    const t = e + "_wrappedFn",
                        i = {
                            [t](...t) {
                                const i = {
                                    type: "callWrappedFunction",
                                    id: e,
                                    args: l.stringify(t)
                                };
                                return window.fetcher.request(i).send()
                            }
                        }[t];
                    return i.type = "vwoWrappedFn_" + (window.mainThread ? "WT" : "MT"), i
                }
                const n = !!t && /^(\d{4})-(\d{2})-(\d{2})T(\d{2}):(\d{2}):(\d{2}(?:\.\d*)?)Z$/;
                return JSON.parse(e, (function(e, t) {
                    for (const i of a) t = i.parse(e, t);
                    var s;
                    if ("string" != typeof t) return t;
                    if (t.length < 8) return t;
                    if (s = t.substring(0, 7), n && t.match(n)) return new Date(t);
                    if ("_NuPreW" === s) {
                        const e = t.match(/_NuPreW([0-9]*)_/)[1];
                        return window.functionWrapper.unwrap(e)
                    }
                    if ("_NuFrNf" === s) {
                        const e = t.match(/_NuFrNf([0-9]*)_/)[1];
                        return i(e)
                    }
                    if ("_PxEgEr" === s) return eval(t.slice(8));
                    if ("_NuFrRa" === s) {
                        const e = +t.match(/_NuFrRa([0-9]*)_/)[1];
                        return i(e)
                    }
                    return t
                }))
            },
            clone: function(e, t) {
                return this.parse(this.stringify(e), t)
            }
        };
    let d = 0;
    const c = {},
        u = {};

    function g(e, t, i) {
        var n;
        const s = this.postMessage.bind(this);
        if ("response" === (null === (n = e) || void 0 === n ? void 0 : n.type)) {
            const t = e;
            return {
                resolve: function(e) {
                    let i = t.encapsulatedData;
                    const n = t.isErrorPresent;
                    i && (i = "function" == typeof e ? e(t.encapsulatedData) : t.encapsulatedData), n ? u[t.twoWayCommId](i) : c[t.twoWayCommId](i)
                }
            }
        } {
            const n = {
                type: "response",
                encapsulatedData: e,
                twoWayCommId: t,
                isErrorPresent: i
            };
            return {
                send: function() {
                    try {
                        return s(n), !0
                    } catch (e) {
                        return !1
                    }
                }
            }
        }
    }

    function v(e) {
        var t;
        if (this.sendingLayer = this.postMessage, "request" === (null === (t = e) || void 0 === t ? void 0 : t.type)) {
            const t = e,
                n = t.encapsulatedData;
            return {
                resolve: e => i(this, void 0, void 0, (function*() {
                    try {
                        const i = yield e(n);
                        return g.call(this, i, t.twoWayCommId).send(), !0
                    } catch (e) {
                        const i = l.stringify(e.message);
                        return g.call(this, i, t.twoWayCommId, !0).send(), !1
                    }
                }))
            }
        } {
            const t = {
                type: "request",
                encapsulatedData: e,
                twoWayCommId: ++d
            };
            return {
                send: () => new Promise(((e, i) => {
                    try {
                        c[t.twoWayCommId] = e, u[t.twoWayCommId] = i, this.sendingLayer(t)
                    } catch (e) {
                        console.log(e), i(e)
                    }
                }))
            }
        }
    }
    var h;
    ! function(e) {
        e[e.Object = 0] = "Object", e[e.Property = 1] = "Property", e[e.Document = 2] = "Document", e[e.Variable = 3] = "Variable", e[e.OverWrite = 4] = "OverWrite", e[e.Delete = 5] = "Delete"
    }(h || (h = {}));
    class p {
        constructor() {
            this.masterObject = {}
        }
        static isObject(e) {
            return "object" == typeof e && !Array.isArray(e) && null !== e
        }
        static createProxy(e, t, i) {
            if (e.__isProxy || !this.isObject(e)) return e;
            const n = e;
            return Object.defineProperty(n, "__transferData", {
                value: !0,
                enumerable: !1,
                writable: !0
            }), new Proxy(n, {
                set: (e, n, s) => {
                    if ("__isProxy" === n || e[n] === s) return !0;
                    if (typeof e[n] == typeof s && "function" != typeof s && JSON.stringify(s) === JSON.stringify(e[n])) return !0;
                    if (this.isObject(s) ? e[n] = this.proxify(s, t, i + n.toString() + ".") : e[n] = s, "__transferData" === n || !e.__transferData) return !0;
                    const r = {
                        path: i + n.toString() + ".",
                        value: s
                    };
                    return r.value = l.stringify(s, e), t({
                        type: "sync",
                        data: r,
                        syncType: h.Object
                    }), !0
                },
                get: (e, t) => "__isProxy" === t || e[t],
                deleteProperty: (e, n) => {
                    if (n in e) {
                        if (delete e[n], !e.__transferData) return !0;
                        const s = {
                            path: i.toString(),
                            key: n
                        };
                        t({
                            type: "sync",
                            data: JSON.stringify(s),
                            syncType: h.Delete
                        })
                    }
                    return !0
                }
            })
        }
        isKey(e) {
            return e in this.masterObject
        }
        static proxify(e, t, i) {
            return this.isObject(e) ? (Object.keys(null != e ? e : {}).forEach((n => {
                this.isObject(e[n]) && (e[n] = this.proxify(e[n], t, i + n + "."))
            })), this.createProxy(e, t, i)) : e
        }
        register(e, t, i) {
            t in this.masterObject && console.error("Key already exists!"), null == e && (e = {});
            const n = p.proxify(e, i, t + ".");
            return this.masterObject[t] = {
                proxy: n
            }, n
        }
        append(e, t) {
            return t in this.masterObject || console.error("Key doesn't exist!"), JSON.stringify(e) !== JSON.stringify(this.masterObject[t].proxy) && console.error(`The object doesn't match the object registered under the key ${t}!`), this.masterObject[t].proxy
        }
        static getProxy(e, t, i) {
            return this.proxify(e, t, i + ".")
        }
        static sync(e, t, i, n, s) {
            if (null == e || !e.__isProxy) return e;
            let r = null,
                o = i + ".";
            return 1 === n.length ? (e.__transferData = !1, e[n[0]] = this.proxify(t, s, o + n[0] + "."), e.__transferData = !0, e) : (r = e[n[0]], n.forEach(((e, t) => {
                o += e + ".", 0 !== t && t !== n.length - 1 && (e in r || (r.__transferData = !1, r[e] = this.proxify({}, s, o), r.__transferData = !0), r = r[e])
            })), r.__transferData = !1, r[n.pop()] = this.proxify(t, s, o), r.__transferData = !0, e)
        }
    }
    const {
        toString: w
    } = Object.prototype;

    function E(e) {
        return "[object Object]" === w.call(e)
    }

    function f(e) {
        return "[object Array]" === w.call(e)
    }
    const _ = {
            CAMPAIGN_FLOW_START: "cFS",
            TEST_NOT_RUNNING: "tNR",
            CAMPAIGN_FLOW_END: "cFE",
            REGISTER_CONVERSION: "vwo_rC",
            CONVERT_GOAL_FOR_ALL_EXPERIMENTS: "cGFAE",
            UNHIDE_ALL_VARIATIONS: "uAV",
            DIMENSION_TAG_PUSHED: "dTP",
            CONVERT_VISIT_GOAL_FOR_EXPERIMENT: "cVGFE",
            UNHIDE_SECTION: "uS",
            EXCLUDE_URL: "eURL",
            BEFORE_REDIRECT_TO_URL: "bRTR",
            URL_CHANGED: "uC",
            HIDE_ELEMENTS: "hE",
            ELEMENT_LOAD_ERROR: "eLTTE",
            NOT_REDIRECTING: "vwo_notRedirecting",
            VISIBILITY_TRIGGERED: "vwo_visibilityTriggered",
            VARIATION_APPLIED: "vwo_vA",
            VARIATION_APPLIED_ERROR: "vwo_variationAppliedError",
            ELEMENT_LOAD_TIMER_STOP: "eLTSt",
            SEND_NEW_VISITOR_CALL: "sNVC",
            CONVERT_REVENUE_GOALS_FOR_EXPERIMENT: "cRGFE",
            CHOOSE_COMBINATION: "cC",
            START_APPLY_CHANGES: "sAC",
            END_APPLY_CHANGES: "eAC",
            CAMPAIGN_COMBI_CREATED: "cCC",
            ELEMENT_LOADED: "eL",
            ELEMENT_NOT_LOADED: "eNL",
            MATCH_WILDCARD: "mW",
            DELETE_CSS_RULE: "dCSSR",
            SPLIT_READY_TO_REDIRECT: "sURL",
            SESSION: "vwo_session",
            NEW_SESSION: "newSession",
            UNHIDE_VARIATION: "uV",
            NEW_SESSION_CREATED: "newSessionCreated",
            PAUSE: "pause",
            SPLIT_URL: "sURL",
            SHOULD_EXECUTE_LIB_ERROR: "shouldExecLib",
            UPDATE_SETTINGS_CALL: "uSC",
            EXCLUDE_GOAL_URL: "eGURL",
            HEATMAP_CLICK: "hCl",
            POST_URL_CHANGE: "hC",
            AFTER_SAMPLING_TRIGGER: "sT",
            CONVERT_ALL_VISIT_GOALS_FOR_EXPERIMENT: "cAVGFE",
            OPT_OUT: "oO",
            POST_INIT: "vwo_postInit",
            PAGE_VIEW: "vwo_pageView",
            ELEMENT_CHANGES_APPLIED: "elementChangesApplied",
            REGISTER_HIT: "registerHit",
            REDIRECT_DECISION: "rD",
            RETRACK_VISITOR: "retrackVisitor",
            CAMPAIGN_NOT_ELIGIBLE: "runCampaign.notEligible",
            UNHIDE_ELEMENT: "unhideElement",
            TOGGLE_VISIBILITY_LOCK: "runCampaign.toggleVisibilityLock",
            CAMPAIGN_READY: "runCampaign.campaignReady",
            MODIFIED_ELEMENT: "runTestCampaign.modifiedEl",
            ERROR: "error",
            SSR_COMPLETE: "vwo_mutationObserved",
            SET_ENV: "setEnvironment",
            ACTIVATED: "vwo_activated",
            _ACTIVATED: "vwo__activated",
            RECORDING_NOT_ELIGIBLE: "rNE",
            VARIATION_SHOWN: "vwo_variationShown",
            NEW_SURVEY_FOUND: "nSF",
            SYNC_VISITOR_PROP: "vwo_syncVisitorProp",
            TAG_EVALUATED: "vwo_tagEval",
            HTML_ELEMENT_LOADED: "vwo_elementLoaded",
            CAMPAIGN_UNLOADED: "vwo_campUnload",
            CAMPAIGNS_LOADED: "vwo_campaignsLoaded",
            EXECUTE_FUNNEL_FOR_GOAL_CAMPAIGN: "executeFunnelCampForGoalCampaign",
            EDITOR_APPLY_CHANGES_COMPLETE: "editorApplyChangesComplete",
            INIT_VWO_INTERNALS: "initVWOInternals",
            SET_CAMPAIGN_TO_OBSERVE: "setCampaignToObserve",
            SEGMENTATION_EVALUATED: "sE",
            ELEMENTS_SHOWN_WITHOUT_CHANGES: "eSWC",
            CUSTOM_CONVERSION: "vwo_conversion",
            REVENUE_CONVERSION: "vwo_revenue",
            DOM_SUBMIT: "vwo_dom_submit",
            DOM_CLICK: "vwo_dom_click",
            ERROR_ONPAGE: "vwo_errorOnPage",
            CURSOR_THRASHED: "vwo_cursorThrashed",
            PAGE_REFRESHED: "vwo_pageRefreshed",
            QUICK_BACK: "vwo_quickBack",
            COPY: "vwo_copy",
            SELECTION: "vwo_selection",
            LEAVE_INTENT: "vwo_leaveIntent",
            TAB_IN: "vwo_tabIn",
            TAB_OUT: "vwo_tabOut",
            REPEATED_SCROLLED: "vwo_repeatedScrolled",
            REPEATED_HOVERED: "vwo_repeatedHovered",
            GOAL_CONVERTED: "vwo_goalConverted",
            GOAL_VISIT: "vwo_goalVisit",
            EVALUATE_GOAL_PAGE_FOR_PREJS: "vwo_evalPreCampJs",
            GROUP_WINNER_CHOOSEN: "vwo_groupWinnerChosen",
            CHECK_SEGMENTATION: "checkSegmentation",
            TRACK_NEW_SESSION_CREATED: "tnSC",
            TRACK_SESSION_CREATED: "tSC",
            PAGE_UNLOAD: "vwo_pageUnload",
            SPA_VISIBILITY_SERVICE: "visibilityForSpa",
            SESSION_INIT_COMPLETE: "vwo_sessionInitComplete",
            TIB_DONE: "vwo_topInitializeBeginDone",
            TOGGLE_MUT_OBSERVER: "toggleMutationObserver",
            DOM_CONTENTLOADED: "vwo_dom_DOMContentLoaded",
            SPLIT_VARIATION_SHOWN: "splitVariationShown",
            VWO_EXECUTED: "vE",
            ACTIVATE_API_TRIGGERED: "aAT",
            CAMPAIGN_TAG_EXECUTED: "cTE",
            RUN_REVERT_TAGS: "runrT",
            VARIATION_SHOWN_SENT: "vwo_variationShownSent",
            PAGE_EXIT: "pageExitEvent",
            COOKIE_CONSENT_ACCEPTED: "cCA",
            COOKIE_CONSENT_REJECTED: "cCR",
            COOKIE_CONSENT_TIMEOUT: "cCT",
            LOAD_SURVEY_LIB: "loadSurveyLib",
            NATIVE_DOM_CONTENT_LOADED: "vwo_domReady",
            RECOM_BLOCK_SHOWN: "vwo_recommendation_block_shown",
            SYNC_EVENTS_COMPLETED: "vwo_syncEventsCallCompleted",
            SEND_SYNC_CALL: "vwo_sendSyncCall",
            LOAD_SETTINGS: "vwo_loadSettings",
            DEBUG_EVENT: "vwo_debugLogs"
        },
        m = {
            SET_COOKIE: "sC",
            GET_COOKIE: "gC",
            ERASE_COOKIE: "eC",
            SET_THIRD_PARTY_COOKIE: "sTPC",
            SET_THIRD_PARTY_COOKIE_ERROR: "sTPCE"
        };
    let O = "",
        S = () => "",
        y = e => e,
        T = e => e,
        I = e => !0;
    window.VWO._.namespaceKeyWithAccId = y;
    class C {
        constructor() {
            this.handleEmptyValue = e => "" === e ? "~" : e, this.revertEmptyValue = e => "~" === e ? "" : e, this.encodeData = e => {
                const t = Object.entries(e);
                let i = "";
                for (let e = 0; e < t.length; e++) {
                    const [n, s] = t[e], {
                        sId: r,
                        mId: o,
                        p: a,
                        id: l
                    } = s, d = `p.rU:${encodeURIComponent(this.handleEmptyValue(a.rU))},p.t:${encodeURIComponent(this.handleEmptyValue(a.t))},p.u:${encodeURIComponent(this.handleEmptyValue(a.u))}`;
                    i += `${n}:${this.handleEmptyValue(r)},${this.handleEmptyValue(o)},${d},${this.handleEmptyValue(l)}|`
                }
                return i.slice(0, -1)
            }, this.decodeData = e => {
                if ("~" === e) return;
                const t = {},
                    i = e.split("|");
                for (let e = 0; e < i.length; e++) {
                    const [n, ...s] = i[e].split(":"), [r, o, ...a] = s.join(":").split(","), l = this.revertEmptyValue(a.pop() || ""), d = {};
                    for (let e = 0; e < a.length; e++) {
                        const t = a[e],
                            [i, ...n] = t.split(":");
                        if (i.startsWith("p.")) {
                            d[i.slice(2)] = this.revertEmptyValue(decodeURIComponent(n.join(":")))
                        }
                    }
                    t[n] = {
                        sId: this.revertEmptyValue(r),
                        mId: this.revertEmptyValue(o),
                        p: d,
                        id: l
                    }
                }
                return t
            }, this.consentMode = window.VWO.consentMode || !1, this.goalCookieStore = {}, this.ccN = "_vwo_consent"
        }
        processQueue() {
            var e;
            const t = this.consentMode.deferredQueue || [];
            for (; t.length > 0;) {
                const i = t.shift();
                null === (e = i.payload) || void 0 === e || e.call(i)
            }
        }
        extractSavedCalls() {
            const e = this.getSyncDataFromConsentCookie();
            if (e) return this.decodeData(e)
        }
        overrideCookies(e) {
            const t = e._create;
            e._create = (...i) => {
                if (!this.consentMode.dT) return this.consentMode.hT && i[0].includes("_goal") ? (this.setGoalCookie(i[0], i[1]), void this.consentMode.deferredQueue.push({
                    method: "fn",
                    payload: () => t.apply(e, i)
                })) : t.apply(e, i)
            };
            const i = e.createThirdParty;
            e.createThirdParty = function(...t) {
                const n = window.VWO.consentMode;
                if (!n.dT) {
                    if (!n.hT) return i.apply(e, t); {
                        const [s, r, o, a] = t;
                        if (window.VWO.modules.utils.consentModeUtils.triggerEvent(m.SET_COOKIE, s, r, o, a, !0), "_vwo" !== s && this._create(s, r, o, a), "_combi_choose" === s.slice(-13)) return;
                        n.deferredQueue.push({
                            method: "fn",
                            payload: () => i.apply(e, t)
                        })
                    }
                }
            };
            const n = e.get;
            e.get = (...t) => {
                if (!this.consentMode.dT || "_vis_opt_test_cookie" !== t[0]) {
                    if (this.consentMode.hT) {
                        const e = this.getGoalCookie(t[0]);
                        if (e) return e
                    }
                    return n.apply(e, t)
                }
            };
            const s = e.waitForThirdPartySync;
            e.waitForThirdPartySync = function(t) {
                return window.VWO.consentMode.hT ? t() : s.apply(e, t)
            }
        }
        initConsentMode() {
            const e = this.consentMode || {};
            if (e.goalLogs = [], window.VWO.consentMode.deferredQueue = window.VWO.consentMode.deferredQueue || [], e.timeOut && (this.consentMode.wFC = !1, this.consentMode.dT = !0, this.triggerEvent(_.COOKIE_CONSENT_TIMEOUT)), "P" === e.cConfig.cPB && this.handlePartiallyBlocked(e), e.preview) return this.handlePreviewMode(e);
            this.handleConsentRejected()
        }
        handlePartiallyBlocked(e) {
            if (e.savedCalls = this.extractSavedCalls(), e.hT && this.setupConsentAcceptedListener(e), e.cCA && e.savedCalls && window.VWO._.phoenixMT.on("vwo_phoenixInitialized", (() => {
                    this.syncSaved(e.savedCalls), this.updateConsentCookie("~"), delete e.savedCalls
                })), !1 === e.hT && e.preview && !e.dT && !e.cCA) {
                let e;
                for (const t in window._vwo_exp) {
                    e = window._vwo_exp[t];
                    break
                }
                const t = window.VWO._.cookies.get("_vis_opt_exp_" + e.id + "_combi");
                if (e.multiple_domains && t) {
                    const i = "SPLIT_URL" === e.type || null,
                        n = {
                            id: e.id,
                            mId: ""
                        };
                    this.syncTpc(n, t, i, e, !0)
                }
            }
        }
        setupConsentAcceptedListener(e) {
            const t = window.VWO._.phoenixMT.on(_.COOKIE_CONSENT_ACCEPTED, (() => {
                e.savedCalls && (this.syncSaved(e.savedCalls), delete e.savedCalls), this.processQueue(), !e.preview && this.triggerEvent(_.COOKIE_CONSENT_ACCEPTED), this.updateConsentCookie("~"), window.VWO._.phoenixMT.off(t)
            }))
        }
        queueGoalLogs(e, t, i, n) {
            const s = window.VWO.consentMode;
            if (!s || !s.preview) return !0;
            if (s.dT) return !1;
            if (!s.hT) return !0;
            if (!window.mainThread) return window.fetcher.getValue('VWO.modules.utils.consentModeUtils.queueGoalLogs("${{1}}","${{2}}", "${{3}}", "${{4}}")', null, {
                captureGroups: [e, t, i, n]
            }), !1;
            let {
                goalLogs: r
            } = s;
            return r.push({
                expId: e,
                goalId: t,
                revenue: i,
                success: n
            }), !1
        }
        triggerGoalLogs() {
            const e = window.VWO.consentMode.goalLogs;
            for (; e.length > 0;) {
                const t = e.shift(),
                    {
                        expId: i,
                        goalId: n,
                        revenue: s,
                        success: r
                    } = t;
                window.VWO.modules.tags.wildCardCallback({
                    oldArgs: [i, n, s, r],
                    campaignId: i,
                    goalId: n
                }, _.REGISTER_CONVERSION)
            }
        }
        handlePreviewMode(e) {
            e.hT && window.VWO.phoenix && window.VWO.phoenix('on("${{1}}", "${{2}}")', null, {
                captureGroups: [_.URL_CHANGED, () => {
                    window.fetcher.setValue("VWO.consentMode.goalLogs", [])
                }]
            }), this.setupConsentTimeoutListener(e), this.setupConsentAcceptedListenerForPreview(e), this.setupConsentRejectedListenerForPreview(e)
        }
        setupConsentTimeoutListener(e) {
            window.VWO._.phoenixMT.on(_.COOKIE_CONSENT_TIMEOUT, (() => {
                this.triggerEvent(_.COOKIE_CONSENT_TIMEOUT), e.wFC && window.fetcher.setValue("VWO.consentMode.wFC", !1), window.fetcher.setValue("VWO.consentMode.dT", !0)
            }))
        }
        setupConsentAcceptedListenerForPreview(e) {
            window.VWO._.phoenixMT.on(_.COOKIE_CONSENT_ACCEPTED, (() => {
                this.triggerEvent(_.COOKIE_CONSENT_ACCEPTED), this.triggerGoalLogs(), e.wFC && window.fetcher.setValue("VWO.consentMode.wFC", !1), !e.dT && window.fetcher.setValue("VWO.consentMode.dT", !1)
            }))
        }
        setupConsentRejectedListenerForPreview(e) {
            window.VWO._.phoenixMT.on(_.COOKIE_CONSENT_REJECTED, (() => {
                this.triggerEvent(_.COOKIE_CONSENT_REJECTED), window.fetcher.setValue("VWO.consentMode.dT", !0)
            }))
        }
        handleConsentRejected() {
            window.VWO._.phoenixMT.on(_.COOKIE_CONSENT_REJECTED, (() => {
                window.fetcher.setValue("VWO.consentMode.dT", !0)
            }))
        }
        triggerEvent(e) {
            window.VWO.phoenix && window.VWO.phoenix('trigger("${{1}}")', null, {
                captureGroups: [e]
            })
        }
        getGoalCookie(e) {
            return this.goalCookieStore[e]
        }
        setGoalCookie(e, t) {
            return window.mainThread && window.fetcher.getValue('VWO.modules.utils.consentModeUtils.setGoalCookie("${{1}}","${{2}}")', null, {
                captureGroups: [e, t]
            }), this.goalCookieStore[e] = t
        }
        deferOnConsent(e, t, i, n, s, r, ...o) {
            if (!this.consentMode) return;
            const {
                dT: a,
                hT: l,
                deferredQueue: d
            } = this.consentMode;
            if (a) return !0;
            if (l) {
                if (["applySyncRequest", "handlerForReqFromWT"].includes(e)) {
                    if (!s.includes("_goal")) return !1;
                    if (this.setGoalCookie(s, r), "handlerForReqFromWT" === e) return d.push({
                        method: e,
                        payload: () => document.cookie = o[0]
                    })
                }
                return s && s.name === _.VARIATION_SHOWN && this.saveForSync(r.d), i && i(n || {}), d.push({
                    method: e,
                    payload: () => t[e].apply(t, o)
                }), !0
            }
        }
        prepareDataForSync(e, t, i) {
            const n = {
                d: {}
            };
            n.d.msgId = e.mId, n.d.visId = e.mId.split("-")[0], n.d.sessionId = e.sId;
            const s = {
                title: e.p.t,
                url: e.p.u,
                referrerUrl: e.p.rU
            };
            return this.consentMode.customParams = s, n.d.event = {
                props: {
                    page: s,
                    id: e.id,
                    variation: t,
                    isFirst: 1
                },
                name: _.VARIATION_SHOWN,
                time: Date.now()
            }, null != i && (n.d.event.props.isSplitVariation = i), n
        }
        addCustomParams(e) {
            const t = this.consentMode;
            return !t || (!t.customParams || (!e.includes(_.VARIATION_SHOWN) && !e.includes("l.gif") || "P" !== t.cConfig.cPB || !("P" === t.cConfig.cPB && !t.hT)))
        }
        syncSaved(e) {
            const t = {
                VWO: {
                    firedTime: Date.now()
                },
                executingTagTrigger: null,
                name: _.VARIATION_SHOWN,
                props: {},
                time: Date.now()
            };
            Object.keys(e).map((i => {
                const n = e[i],
                    s = window._vwo_exp[n.id];
                let r = null,
                    o = null;
                if ("SPLIT_URL" === s.type && (r = !0, o = "1" != i), !window.VWO._.cookies.get("_vis_opt_exp_" + n.id + "_combi")) return;
                const a = this.prepareDataForSync(n, i, o);
                window.VWO.modules.tags.dataSync.utils.addDataFromMTAndSend(null, null, a, null, !0, null, t, +n.id), this.syncImg(n, i, s), this.syncTpc(n, i, r, s)
            }))
        }
        syncTpc(e, t, i, n, s = !1) {
            if (!n.multiple_domains) return;
            const r = [`_vwo_uuid_${e.id}`, e.mId.split("-")[0], 3650, void 0, e.id, void 0, n];
            !s && window.VWO._.cookies.createThirdParty(...r), r[0] = `_vis_opt_exp_${e.id}_combi`, r[1] = t, r[3] = 100, window.VWO._.cookies.createThirdParty(...r), null != i && (r[0] = `_vis_opt_exp_${e.id}_split`, window.VWO._.cookies.createThirdParty(...r))
        }
        syncImg(e, t, i) {
            let n = window.VWO.modules.utils.libUtils.extraData2();
            const s = encodeURIComponent(n);
            n = i.ps || void 0 === i.ps ? "&ed=" + s : "";
            const r = "l.gif?experiment_id=" + e.id + "&account_id=" + window._vwo_acc_id + "&cu=" + encodeURIComponent(e.p.u) + "&combination=" + t + "&s=1&sId=" + e.sId + "&u=" + e.mId.split("-")[0] + n;
            window.VWO.modules.tags.dataSync.utils.sendCall(null, {
                url: r
            }, null, null, !0)
        }
        saveForSync(e) {
            let t = this.getSyncDataFromConsentCookie(),
                i = t ? this.decodeData(t) : {};
            const n = {
                    rU: e.event.props.page.referrerUrl,
                    u: e.event.props.page.url,
                    t: e.event.props.page.title
                },
                s = {
                    sId: e.sessionId,
                    mId: e.msgId,
                    p: n,
                    id: e.event.props.id
                },
                r = Object.assign(Object.assign({}, i), {
                    [e.event.props.variation]: s
                });
            let o = this.encodeData(r);
            this.updateConsentCookie(o)
        }
        getSyncDataFromConsentCookie() {
            const e = `${this.ccN}=`,
                t = document.cookie.split("; ").find((t => t.startsWith(e)));
            if (t) {
                const e = decodeURIComponent(t.split("=")[1]).split(":");
                if (e.length > 1) return e.shift(), "~" === e[0] ? null : e.join(":")
            }
            return null
        }
        updateConsentCookie(e) {
            const t = document.cookie.match(new RegExp(`(^|;\\s*)${this.ccN}=([^;]*)`)),
                i = t ? t[2] : null;
            let n = "";
            if (i) {
                n = decodeURIComponent(i).split(":")[0]
            }
            const s = encodeURIComponent(`${n}:${e}`);
            document.cookie = `${this.ccN}=${s}; path=/; domain=.${window.VWO.consentMode.domain}; max-age=31536000`
        }
    }
    const N = function() {
            const e = window.VWO.consentMode;
            return !!e && !!e.dT
        },
        A = new C;
    window.VWO.modules.utils.consentModeUtils = A;
    let V = !1;

    function b(e) {
        return e.split(";").reduce(((e, t) => {
            const i = t.indexOf("=");
            if (-1 !== i) {
                const n = t.substring(0, i).trim(),
                    s = t.substring(i + 1).trim();
                e[n] = s
            } else e[t.trim()] = "";
            return e
        }), {})
    }
    class R {
        constructor() {
            this.operations = []
        }
        push(e, t) {
            this.operations.push({
                name: e,
                value: t
            })
        }
        pop_front() {
            this.operations.splice(0, 1)
        }
        fullfil(e, t = !0) {
            const i = b(e);
            t && this.pop_front(), this.operations.forEach((e => {
                i[e.name] = e.value
            }));
            return Object.entries(i).map((e => e.join("="))).join("; ")
        }
    }
    class P {
        static internalUtils() {
            var e;
            return {
                isCookiePayloadObject: e => !(!E(e) || !["value", "fromThread", "origin"].reduce(((t, i) => t && i in e), !0)),
                isCurrentContextMT: !!(null === (e = null === window || void 0 === window ? void 0 : window.mainThread) || void 0 === e ? void 0 : e.webWorker)
            }
        }
        getSetter(e) {
            let t;
            return t = new R, i => {
                if ("string" == typeof i) i = {
                    value: i
                };
                else if (!P.internalUtils().isCookiePayloadObject(i)) return void console.error("Invalid value type!");
                const {
                    value: n,
                    fromThread: s
                } = i;
                let {
                    origin: r
                } = i, o = !0;
                if (P.internalUtils().isCurrentContextMT || "MAIN" === s) document.__cookie = n, o = "MAIN" !== s;
                else {
                    {
                        const e = n.indexOf("="),
                            i = n.substring(0, e).trim(),
                            s = b(n);
                        let a = !1;
                        (s.expires && new Date(s.expires) < new Date(Date.now()) || "0" === s["max-age"]) && (a = !0);
                        const l = b(document.__cookie);
                        a ? i in l ? delete l[i] : o = !1 : (l[i] = s[i], t.push(i, s[i]), r = "WORKER"), document.__cookie = Object.entries(l).map((e => e.join("="))).join(";")
                    }
                }
                return "MAIN" === s && (document.__cookie = t.fullfil(document.__cookie, "WORKER" === r)), o && e({
                    type: "sync",
                    data: {
                        propertyName: "cookie",
                        value: {
                            value: P.internalUtils().isCurrentContextMT ? document.__cookie : n,
                            fromThread: P.internalUtils().isCurrentContextMT ? "MAIN" : "WORKER",
                            origin: V ? "WORKER" : r
                        }
                    },
                    syncType: h.Document
                }), !0
            }
        }
    }

    function x(e) {
        if (!P.internalUtils().isCookiePayloadObject(e)) return void console.error("Invalid value type!");
        const {
            value: t
        } = e;
        if (window.VWO.consentMode) {
            if (N()) return;
            let e = t.split("=");
            if (A.deferOnConsent("handlerForReqFromWT", null, null, null, e[0], e[1], t)) return
        }
        V = !0, document.cookie = t, V = !1
    }
    let L = {},
        D;
    class U {
        static register(e, t) {
            switch (e) {
                case "cookie":
                    0;
                default:
                    this.registerProperty(e, t)
            }
        }
        static registerProperty(e, t) {
            if (document) {
                if (e in window.document) {
                    let i;
                    i = Object.getOwnPropertyDescriptor(window.document, e);
                    const n = {
                        enumerable: i.enumerable,
                        configurable: i.configurable,
                        get: () => document["__" + e],
                        set: this.internalUtils.getSetter(e, t)
                    };
                    Object.defineProperty(window.document, "__" + e, i), Object.defineProperty(window.document, e, n)
                }
            } else console.error("The property doesn't exist on the `DOCUMENT` object.")
        }
        static sync({
            propertyName: e,
            value: t
        }) {
            document[e] = t
        }
    }
    U.internalUtils = {
        getSetter: (e, t) => {
            switch (e) {
                case "cookie":
                    return (new P).getSetter(t);
                default:
                    return i => (JSON.stringify(document["__" + e]) === JSON.stringify(i) || (document["__" + e] = i, t({
                        type: "sync",
                        data: {
                            propertyName: e,
                            value: document["__" + e]
                        },
                        syncType: h.Document
                    })), !0)
            }
        },
        isKeyNonConfigurable: e => {
            var t, i, n;
            const s = [document, null === (t = null === window || void 0 === window ? void 0 : window.Document) || void 0 === t ? void 0 : t.prototype, null === (i = null === window || void 0 === window ? void 0 : window.HTMLDocument) || void 0 === i ? void 0 : i.prototype];
            for (let t = 0; t < s.length; t++)
                if (!1 === (null === (n = Object.getOwnPropertyDescriptor(s[t] || {}, e)) || void 0 === n ? void 0 : n.configurable)) return !0;
            return !1
        }
    };
    class W {
        static register(e, t, i, n) {
            i in e ? console.error("The property must not pre-exist inside the object.") : Object.defineProperty(e, i, {
                enumerable: !0,
                configurable: !1,
                get: () => e[`__${i}`],
                set: s => (e[`__${i}`] = s, n({
                    type: "sync",
                    data: {
                        identifier: t,
                        property: i,
                        value: s
                    },
                    syncType: h.Property
                }), !0)
            })
        }
    }
    const M = {
        get: e => {
            try {
                0;
                return window.localStorage.getItem(e)
            } catch (e) {
                return ""
            }
        },
        set: (e, t) => {
            try {
                return window.localStorage.setItem(e, t)
            } catch (e) {
                return ""
            }
        },
        remove: e => {
            try {
                return window.localStorage.removeItem(e)
            } catch (e) {
                return !1
            }
        },
        getItem: function(e) {
            return this.get(e)
        },
        setItem: function(e, t) {
            this.set(e, t)
        },
        deleteAll: function() {},
        deleteItem: function(e) {
            this.remove(e)
        }
    };

    function k(e) {
        D = e
    }

    function G() {
        class e {
            constructor(e) {
                this.__transferData = !0, this.length = (null == e ? void 0 : e.length) || 0, this.value = e || {}, this.callback = window.fetcher.postMessage.bind(window.fetcher)
            }
            key(e) {
                if (e >= this.length) return null;
                const t = Object.keys(this.value);
                for (let i = 0; i < t.length; i++)
                    if (i === e) return t[i]
            }
            getItem(e) {
                return e in this.value ? this.value[e] : null
            }
            setItem(e, t) {
                if (e in this.value) {
                    if (this.value[e] === t) return
                } else this.length++;
                return this.value[e] = t, this.callback({
                    data: {
                        key: e,
                        value: t
                    },
                    type: "sync",
                    syncType: {
                        type: "custom",
                        method: "localStorage",
                        operation: "setItem"
                    }
                }), null
            }
            removeItem(e) {
                return e in this.value ? (this.length--, delete this.value[e], this.callback({
                    data: {
                        key: e
                    },
                    type: "sync",
                    syncType: {
                        type: "custom",
                        method: "localStorage",
                        operation: "removeItem"
                    }
                }), null) : null
            }
            clear() {
                return 0 === this.length || (this.length = 0, this.value = {}, this.callback({
                    data: {},
                    type: "sync",
                    syncType: {
                        type: "custom",
                        method: "localStorage",
                        operation: "clear"
                    }
                })), null
            }
        }
        window.localStorage = new e(window.localStorage)
    }

    function F(e) {
        if ("number" != typeof e.syncType) {
            switch (window.localStorage.__transferData && (window.localStorage.__transferData = !1), e.syncType.operation) {
                case "setItem":
                    window.localStorage.setItem(e.data.key, e.data.value);
                    break;
                case "removeItem":
                    window.localStorage.removeItem(e.data.key);
                    break;
                case "clear":
                    window.localStorage.clear();
                    break;
                default:
                    return
            }
            window.localStorage.__transferData && (window.localStorage.__transferData = !0)
        }
    }
    window.VWO._.localStorageService = M;
    class j {}
    j.syncLocalStorage = G;
    class H extends j {
        constructor() {
            super(), this.objectSyncer = new p
        }
        register(e, t, i = {}, n = "", s = !1) {
            if ("object" != typeof i || Array.isArray(i)) return;
            const r = window.fetcher.postMessage.bind(window.fetcher);
            switch (e) {
                case "custom":
                    switch (t) {
                        case "localStorage":
                            H.syncLocalStorage();
                            break;
                        default:
                            throw new Error("Unknown property name!")
                    }
                    break;
                case h.Object:
                    {
                        const e = this.objectSyncer.register(i, t, r);
                        return s && r({
                            data: {
                                value: JSON.stringify(i),
                                path: t
                            },
                            type: "sync",
                            syncType: h.OverWrite
                        }),
                        e
                    }
                case h.Property:
                    W.register(i, n, t, r);
                    break;
                case h.Document:
                    U.register(t, r);
                    break;
                default:
                    console.error("Unknown 'syncAblesEnum' type!")
            }
        }
        append(e, t) {
            return this.objectSyncer.append(e, t)
        }
        static sync(e, t) {
            var i;
            const {
                data: n
            } = e;
            if ("object" != typeof e.syncType || "custom" !== e.syncType.type) switch (e.syncType) {
                case h.Object:
                    {
                        n.value = l.parse(n.value);
                        const e = n.path.substring(0, n.path.lastIndexOf(".")).split(".");window[e[0]] = p.sync(window[e[0]], n.value, e[0], e.splice(1), t);
                        break
                    }
                case h.Document:
                    U.sync(n);
                    break;
                case h.Property:
                case h.Variable:
                    t(n);
                    break;
                case h.OverWrite:
                    if (!("__transferData" in (null !== (i = window[n.path]) && void 0 !== i ? i : {}))) return void(window[n.path] = JSON.parse(n.value));
                    window[n.path] = p.getProxy(JSON.parse(n.value), t, n.path);
                    break;
                case h.Delete:
                    {
                        const e = JSON.parse(n),
                            t = e.path.substring(0, e.path.lastIndexOf(".")).split(".").reduce(((e, t) => Object.keys(e).length ? e[t] : window[t]), {}),
                            i = e.key;i in t && (t.__transferData = !1, delete t[i], t.__transferData = !0);
                        break
                    }
                default:
                    console.error("Unknown 'syncAblesEnum' type!")
            } else switch (e.syncType.method) {
                case "localStorage":
                    F(e);
                    break;
                default:
                    return
            }
        }
        declare(e, t) {
            W.register(window, "window", e, t)
        }
    }

    function $(e, t = {
        sendErrorLog: !1
    }, i) {
        try {
            return e()
        } catch (e) {
            return t.sendErrorLog && setTimeout((() => {
                try {
                    s({
                        msg: t.msg || "safelyGetValue failed!",
                        url: t.url || "errorHandler.ts",
                        source: t.source || e
                    })
                } catch (e) {}
            }), 100), i
        }
    }
    const B = (e, t) => {
        if (e && "function" == typeof e && e.bind) try {
            e = e.bind(t)
        } catch (t) {
            if (/(cannot be invoked without 'new')|(Cannot call a class constructor without |new|)/i.test(t.message)) return e;
            console.error(t)
        }
        return e
    };

    function K(e, t, i = {}) {
        if ("window" === e) return window;
        let n = window;
        const {
            captureGroups: s = null,
            filter: r
        } = i, o = e.split("."), a = o.length;
        for (let e = 0; e < a; e++) {
            let t = o[e];
            if (t.endsWith(")")) {
                const e = t.substring(0, t.indexOf("("));
                let i = t.substring(t.indexOf("("));
                i = "[" + i.slice(1, i.length - 1) + "]";
                const r = i.slice(1, i.length - 1).split(",");
                r.forEach(((e, t) => {
                    e.startsWith('"') || (r[t] = '"vwoCurrThreadRef' + e + '"')
                }));
                const o = JSON.parse(i, ((e, t) => {
                    let i;
                    if ("string" == typeof t) {
                        if (i = t.match(/\${{([0-9]*)}}/)) return s[i[1] - 1];
                        if (i = t.match(/vwoCurrThreadRef(.*)/)) return K(i[1])
                    }
                    return t
                }));
                n = n[e](...o)
            } else {
                let e = !1;
                t.endsWith("?") && (t = t.slice(0, -1), e = !0);
                const i = n[t];
                if (n = B(i, n), e && null == n) return n
            }
        }
        if (r) {
            const e = {};
            r.forEach((t => {
                e[t] = n[t]
            })), n = e
        }
        return n
    }
    const z = function(e) {
            return window.functionWrapper.unwrap(e.id)(...l.parse(e.args))
        },
        Y = function(e) {
            var t, n;
            return i(this, void 0, void 0, (function*() {
                switch (e.type) {
                    case "callWrappedFunction":
                        {
                            let t = z(e);
                            return t && "function" == typeof t.then && (t = yield t),
                            l.stringify(t)
                        }
                    case "vwoClassInstanceBridge":
                        {
                            const t = e.path.dest.lastIndexOf(".");
                            let i = window,
                                n = e.path.dest; - 1 !== t && (i = K(e.path.dest.slice(0, t)), n = e.path.dest.substr(t + 1));
                            const s = i[n],
                                [r, o] = new s(...e.args);
                            return o.otherSide = (...t) => {
                                const i = e.path.src + "." + r + "." + t[0];
                                return t[0] = i, window.fetcher.getValue(...t)
                            },
                            "" + r
                        }
                    default:
                        {
                            let i, s;
                            if ("setValue" === (e = l.parse(e)).type) {
                                -1 == e.path.lastIndexOf(".") && (e.path = "window." + e.path);
                                const t = e.path;
                                e.path = t.slice(0, t.lastIndexOf(".")), i = t.slice(t.lastIndexOf(".") + 1)
                            }(null === (t = e.config) || void 0 === t ? void 0 : t.captureGroups) && (e.config.captureGroups = l.parse(e.config.captureGroups));
                            const r = s = K(e.path, e.args, null == e ? void 0 : e.config);
                            return (null === (n = e.config) || void 0 === n ? void 0 : n.constructable) ? s = new r(...e.args) : "function" == typeof r && (s = r(...e.args || [])),
                            i && (s = r[i] = e.val),
                            s = yield s,
                            l.stringify(s)
                        }
                }
            }))
        };
    class J {}
    class X extends J {
        init() {
            this.isMTInstance = !!$((() => window.mainThread.webWorker)), this.thread = this.isMTInstance ? window.vwoChannelFW : null === window || void 0 === window ? void 0 : window.workerThread, this.request = v, this.response = g, this.isMTInstance ? this.thread.port1.onmessage = this.onMessage.bind(this) : (this.thread.onmessage = this.isMessageChannel(this.thread) && this.onMessage.bind(this), this.auxiliaryMessageHandler())
        }
        auxiliaryMessageHandler() {
            const e = this,
                t = function(i) {
                    const {
                        vwoChannelToW: n,
                        vwoChannelFW: s
                    } = i.data;
                    n && s && (window.vwoChannelToW = n, window.vwoChannelFW = s, e.thread = n, e.thread.onmessage = e.onMessage.bind(e), self.removeEventListener("message", t))
                };
            self.addEventListener("message", t)
        }
        isMessageChannel(e) {
            return e && e.port1 instanceof MessagePort && e.port2 instanceof MessagePort
        }
        postMessage(e) {
            try {
                this.isMTInstance ? window.vwoChannelToW.port2.postMessage(e) : window.vwoChannelFW.postMessage(e)
            } catch (e) {
                console.error(e)
            }
        }
        onMessage(e) {
            var t, i, n, s;
            const {
                data: r
            } = e;
            switch (r.type) {
                case "initDone":
                    window.vwo_initDone(r);
                    break;
                case "request":
                    this.request(r).resolve(Y);
                    break;
                case "response":
                    this.response(r).resolve(l.parse.bind(l));
                    break;
                case "sync":
                    {
                        let e = () => null;
                        switch (r.syncType) {
                            case h.OverWrite:
                            case h.Object:
                                e = this.postMessage.bind(this);
                                break;
                            case h.Property:
                            case h.Document:
                            case h.Variable:
                            case h.Delete:
                        }
                        H.sync(r, e);
                        break
                    }
                default:
                    window.VwoUnitTestsRunning && ("unit-test" === r.type ? eval(r.code) : "unit-test-result" === r.type && (null === (i = null === (t = window.PromiseResolver) || void 0 === t ? void 0 : t[r.id]) || void 0 === i || i.resolve(r))), null === (s = (n = this.thread)._onMessage) || void 0 === s || s.call(n, e)
            }
        }
        getValue(e, t, i = {}) {
            let n;
            (null == i ? void 0 : i.captureGroups) && (n = l.stringify(i.captureGroups));
            const s = {
                path: e,
                args: t,
                config: Object.assign(Object.assign({}, i), {
                    captureGroups: n
                })
            };
            return this.request(l.stringify(s)).send().catch((() => {}))
        }
        setValue(e, t) {
            const i = {
                type: "setValue",
                path: e,
                val: t
            };
            return this.request(l.stringify(i)).send().catch((() => {}))
        }
    }
    const q = X;
    window.fetcher = new q;
    class Q {
        constructor() {
            this.id = 0, this.store = {}
        }
        wrap(e, t) {
            const i = this.id++;
            return this.store = this.store || {}, this.store[i] = t ? e.bind(t) : e, i
        }
        unwrap(e) {
            return this.store[e]
        }
    }
    var Z = "undefined" != typeof globalThis ? globalThis : "undefined" != typeof window ? window : "undefined" != typeof global ? global : "undefined" != typeof self ? self : {};

    function ee(e) {
        var t = {
            exports: {}
        };
        return e(t, t.exports), t.exports
    }
    var te = function(e) {
            return e && e.Math == Math && e
        },
        ie = te("object" == typeof globalThis && globalThis) || te("object" == typeof window && window) || te("object" == typeof self && self) || te("object" == typeof Z && Z) || function() {
            return this
        }() || Function("return this")(),
        ne = function(e) {
            try {
                return !!e()
            } catch (e) {
                return !0
            }
        },
        se = !ne((function() {
            return 7 != Object.defineProperty({}, 1, {
                get: function() {
                    return 7
                }
            })[1]
        })),
        re = {}.propertyIsEnumerable,
        oe = Object.getOwnPropertyDescriptor,
        ae = {
            f: oe && !re.call({
                1: 2
            }, 1) ? function(e) {
                var t = oe(this, e);
                return !!t && t.enumerable
            } : re
        },
        le = function(e, t) {
            return {
                enumerable: !(1 & e),
                configurable: !(2 & e),
                writable: !(4 & e),
                value: t
            }
        },
        de = {}.toString,
        ce = "".split,
        ue = ne((function() {
            return !Object("z").propertyIsEnumerable(0)
        })) ? function(e) {
            return "String" == function(e) {
                return de.call(e).slice(8, -1)
            }(e) ? ce.call(e, "") : Object(e)
        } : Object,
        ge = function(e) {
            return ue(function(e) {
                if (null == e) throw TypeError("Can't call method on " + e);
                return e
            }(e))
        },
        ve = function(e) {
            return "object" == typeof e ? null !== e : "function" == typeof e
        },
        he = function(e, t) {
            if (!ve(e)) return e;
            var i, n;
            if (t && "function" == typeof(i = e.toString) && !ve(n = i.call(e))) return n;
            if ("function" == typeof(i = e.valueOf) && !ve(n = i.call(e))) return n;
            if (!t && "function" == typeof(i = e.toString) && !ve(n = i.call(e))) return n;
            throw TypeError("Can't convert object to primitive value")
        },
        pe = {}.hasOwnProperty,
        we = function(e, t) {
            return pe.call(e, t)
        },
        Ee = ie.document,
        fe = ve(Ee) && ve(Ee.createElement),
        _e = !se && !ne((function() {
            return 7 != Object.defineProperty(("div", fe ? Ee.createElement("div") : {}), "a", {
                get: function() {
                    return 7
                }
            }).a
        })),
        me = Object.getOwnPropertyDescriptor,
        Oe = {
            f: se ? me : function(e, t) {
                if (e = ge(e), t = he(t, !0), _e) try {
                    return me(e, t)
                } catch (e) {}
                if (we(e, t)) return le(!ae.f.call(e, t), e[t])
            }
        },
        Se = function(e) {
            if (!ve(e)) throw TypeError(String(e) + " is not an object");
            return e
        },
        ye = Object.defineProperty,
        Te = {
            f: se ? ye : function(e, t, i) {
                if (Se(e), t = he(t, !0), Se(i), _e) try {
                    return ye(e, t, i)
                } catch (e) {}
                if ("get" in i || "set" in i) throw TypeError("Accessors not supported");
                return "value" in i && (e[t] = i.value), e
            }
        },
        Ie = se ? function(e, t, i) {
            return Te.f(e, t, le(1, i))
        } : function(e, t, i) {
            return e[t] = i, e
        },
        Ce = function(e, t) {
            try {
                Ie(ie, e, t)
            } catch (i) {
                ie[e] = t
            }
            return t
        },
        Ne = ie["__core-js_shared__"] || Ce("__core-js_shared__", {}),
        Ae = Function.toString;
    "function" != typeof Ne.inspectSource && (Ne.inspectSource = function(e) {
        return Ae.call(e)
    });
    var Ve, be, Re, Pe, xe = Ne.inspectSource,
        Le = ie.WeakMap,
        De = "function" == typeof Le && /native code/.test(xe(Le)),
        Ue = ee((function(e) {
            (e.exports = function(e, t) {
                return Ne[e] || (Ne[e] = void 0 !== t ? t : {})
            })("versions", []).push({
                version: "3.8.1",
                mode: "global",
                copyright: "© 2020 Denis Pushkarev (zloirock.ru)"
            })
        })),
        We = 0,
        Me = Math.random(),
        ke = Ue("keys"),
        Ge = {},
        Fe = ie.WeakMap;
    if (De) {
        var je = Ne.state || (Ne.state = new Fe),
            He = je.get,
            $e = je.has,
            Be = je.set;
        Ve = function(e, t) {
            return t.facade = e, Be.call(je, e, t), t
        }, be = function(e) {
            return He.call(je, e) || {}
        }, Re = function(e) {
            return $e.call(je, e)
        }
    } else {
        var Ke = ke[Pe = "state"] || (ke[Pe] = function(e) {
            return "Symbol(" + String(void 0 === e ? "" : e) + ")_" + (++We + Me).toString(36)
        }(Pe));
        Ge[Ke] = !0, Ve = function(e, t) {
            return t.facade = e, Ie(e, Ke, t), t
        }, be = function(e) {
            return we(e, Ke) ? e[Ke] : {}
        }, Re = function(e) {
            return we(e, Ke)
        }
    }
    var ze = {
            set: Ve,
            get: be,
            has: Re,
            enforce: function(e) {
                return Re(e) ? be(e) : Ve(e, {})
            },
            getterFor: function(e) {
                return function(t) {
                    var i;
                    if (!ve(t) || (i = be(t)).type !== e) throw TypeError("Incompatible receiver, " + e + " required");
                    return i
                }
            }
        },
        Ye = ee((function(e) {
            var t = ze.get,
                i = ze.enforce,
                n = String(String).split("String");
            (e.exports = function(e, t, s, r) {
                var o, a = !!r && !!r.unsafe,
                    l = !!r && !!r.enumerable,
                    d = !!r && !!r.noTargetGet;
                "function" == typeof s && ("string" != typeof t || we(s, "name") || Ie(s, "name", t), (o = i(s)).source || (o.source = n.join("string" == typeof t ? t : ""))), e !== ie ? (a ? !d && e[t] && (l = !0) : delete e[t], l ? e[t] = s : Ie(e, t, s)) : l ? e[t] = s : Ce(t, s)
            })(Function.prototype, "toString", (function() {
                return "function" == typeof this && t(this).source || xe(this)
            }))
        })),
        Je = ie,
        Xe = function(e) {
            return "function" == typeof e ? e : void 0
        },
        qe = Math.ceil,
        Qe = Math.floor,
        Ze = function(e) {
            return isNaN(e = +e) ? 0 : (e > 0 ? Qe : qe)(e)
        },
        et = Math.min,
        tt = Math.max,
        it = Math.min,
        nt = function(e) {
            return function(t, i, n) {
                var s, r, o = ge(t),
                    a = (s = o.length) > 0 ? et(Ze(s), 9007199254740991) : 0,
                    l = function(e, t) {
                        var i = Ze(e);
                        return i < 0 ? tt(i + t, 0) : it(i, t)
                    }(n, a);
                if (e && i != i) {
                    for (; a > l;)
                        if ((r = o[l++]) != r) return !0
                } else
                    for (; a > l; l++)
                        if ((e || l in o) && o[l] === i) return e || l || 0;
                return !e && -1
            }
        },
        st = (nt(!0), nt(!1)),
        rt = function(e, t) {
            var i, n = ge(e),
                s = 0,
                r = [];
            for (i in n) !we(Ge, i) && we(n, i) && r.push(i);
            for (; t.length > s;) we(n, i = t[s++]) && (~st(r, i) || r.push(i));
            return r
        },
        ot = ["constructor", "hasOwnProperty", "isPrototypeOf", "propertyIsEnumerable", "toLocaleString", "toString", "valueOf"],
        at = ot.concat("length", "prototype"),
        lt = {
            f: Object.getOwnPropertyNames || function(e) {
                return rt(e, at)
            }
        },
        dt = {
            f: Object.getOwnPropertySymbols
        },
        ct = function(e, t) {
            return arguments.length < 2 ? Xe(Je[e]) || Xe(ie[e]) : Je[e] && Je[e][t] || ie[e] && ie[e][t]
        }("Reflect", "ownKeys") || function(e) {
            var t = lt.f(Se(e)),
                i = dt.f;
            return i ? t.concat(i(e)) : t
        },
        ut = function(e, t) {
            for (var i = ct(t), n = Te.f, s = Oe.f, r = 0; r < i.length; r++) {
                var o = i[r];
                we(e, o) || n(e, o, s(t, o))
            }
        },
        gt = /#|\.prototype\./,
        vt = function(e, t) {
            var i = pt[ht(e)];
            return i == Et || i != wt && ("function" == typeof t ? ne(t) : !!t)
        },
        ht = vt.normalize = function(e) {
            return String(e).replace(gt, ".").toLowerCase()
        },
        pt = vt.data = {},
        wt = vt.NATIVE = "N",
        Et = vt.POLYFILL = "P",
        ft = vt,
        _t = Oe.f,
        mt = function(e, t) {
            var i, n, s, r, o, a = e.target,
                l = e.global,
                d = e.stat;
            if (i = l ? ie : d ? ie[a] || Ce(a, {}) : (ie[a] || {}).prototype)
                for (n in t) {
                    if (r = t[n], s = e.noTargetGet ? (o = _t(i, n)) && o.value : i[n], !ft(l ? n : a + (d ? "." : "#") + n, e.forced) && void 0 !== s) {
                        if (typeof r == typeof s) continue;
                        ut(r, s)
                    }(e.sham || s && s.sham) && Ie(r, "sham", !0), Ye(i, n, r, e)
                }
        },
        Ot = Object.keys || function(e) {
            return rt(e, ot)
        },
        St = ae.f,
        yt = function(e) {
            return function(t) {
                for (var i, n = ge(t), s = Ot(n), r = s.length, o = 0, a = []; r > o;) i = s[o++], se && !St.call(n, i) || a.push(e ? [i, n[i]] : n[i]);
                return a
            }
        },
        Tt = (yt(!0), yt(!1));
    mt({
        target: "Object",
        stat: !0
    }, {
        values: function(e) {
            return Tt(e)
        }
    }), Je.Object.values;
    var It = function(e, t, i) {
            var n = he(t);
            n in e ? Te.f(e, n, le(0, i)) : e[n] = i
        },
        Ct, Nt, At, Vt, bt, Rt, Pt;
    mt({
            target: "Object",
            stat: !0,
            sham: !se
        }, {
            getOwnPropertyDescriptors: function(e) {
                for (var t, i, n = ge(e), s = Oe.f, r = ct(n), o = {}, a = 0; r.length > a;) void 0 !== (i = s(n, t = r[a++])) && It(o, t, i);
                return o
            }
        }), Je.Object.getOwnPropertyDescriptors,
        function(e) {
            e.DOM = "vwo_dom"
        }(Ct || (Ct = {})),
        function(e) {
            e.WILD_CARD = "*", e.TRIGGER = "trigger", e.POST_INIT = "post-init", e.TIMER = "vwo_timer"
        }(Nt || (Nt = {})),
        function(e) {
            e.URL_CHANGE = "vwo_urlChange", e.LEAVE_INTENT = "vwo_leaveIntent", e.CLICK_EVENT = "vwo_dom_click", e.SUBMIT_EVENT = "vwo_dom_submit", e.PAGE_LOAD_EVENT = "vwo_page_load"
        }(At || (At = {})),
        function(e) {
            e.PAGE_VIEW = "vwo_pageView", e.PAGE_UNLOAD_EVENT = "vwo_pageUnload"
        }(Vt || (Vt = {})),
        function(e) {
            e.EXIT_CONDITIONS = "__exitConditions"
        }(bt || (bt = {})),
        function(e) {
            e.DOM_CONTENT_LOADED = "DOMContentLoaded", e.SCROLL = "scroll", e.CLICK = "click", e.SUBMIT = "submit"
        }(Rt || (Rt = {})),
        function(e) {
            e[e.DEBUG = 0] = "DEBUG", e[e.INFO = 1] = "INFO", e[e.WARN = 2] = "WARN", e[e.ERROR = 3] = "ERROR"
        }(Pt || (Pt = {}));
    class xt {
        constructor(e) {
            this.setLevel(e)
        }
        setLevel(e = "warn") {
            this.logLevel = Pt[e.toUpperCase()]
        }
        info(e, t = {}) {
            this.customLog(Pt.INFO, e, t)
        }
        debug(e, t = {}) {
            this.customLog(Pt.DEBUG, e, t)
        }
        warn(e, t = {}) {
            var i, n;
            this.customLog(Pt.WARN, e, t, null === (n = null === (i = window.VWO) || void 0 === i ? void 0 : i._) || void 0 === n ? void 0 : n.customError)
        }
        error(e, t = {}) {
            var i, n;
            this.customLog(Pt.ERROR, e, t, null === (n = null === (i = window.VWO) || void 0 === i ? void 0 : i._) || void 0 === n ? void 0 : n.customError)
        }
        customLog(e, t, i, n = null) {
            var s, r, o;
            if (e >= this.logLevel) {
                const a = this.formatMessage(e, t, i);
                null === (o = null === (r = null === (s = window.VWOEvents) || void 0 === s ? void 0 : s.store) || void 0 === r ? void 0 : r.actions) || void 0 === o || o.addLogsForDebugging(a), n ? n(a) : this.consoleLog(e, [a])
            }
        }
        consoleLog(e, t) {
            switch (e) {
                case Pt.INFO:
                    console.info(...t);
                    break;
                case Pt.WARN:
                    console.warn(...t);
                    break;
                case Pt.ERROR:
                    console.error(...t);
                    break;
                default:
                    console.log(...t)
            }
        }
        formatMessage(e, t, i) {
            var n, s;
            const r = Object.keys(i).reduce(((e, t) => e.replace(new RegExp(`{{${t}}}`, "g"), i[t])), t),
                o = Ct.DOM + "_";
            let a = i;
            const l = (null === (n = i.data) || void 0 === n ? void 0 : n.vwoEventName) || i.vwoEventName;
            l !== o + Rt.CLICK && l !== o + Rt.SUBMIT || (a = i.data ? null === (s = i.data) || void 0 === s ? void 0 : s.props : a.props, a = a || {
                name: l
            });
            let d = JSON.stringify;
            try {
                d = window.VWO._.native.JSON.stringify || JSON.stringify
            } catch (e) {}
            return `VWO: [${Pt[e].toUpperCase()}] [${(new Date).toUTCString()}] ${r} ${d(a)}`
        }
    }
    class Lt {
        static parseUrl(e) {
            try {
                e = decodeURIComponent(e)
            } catch (e) {
                console.warn("Not a valid URL.")
            }
            const t = /^((((\w+)(:\/\/))?((\w+):(\w+)@)?(www\.)?)([^?#\/:\s]*)?:?([0-9][^?#\/\s]*)?)\/?([^?#\s]*)\??([^#]*)#?(.*)$/.exec(e.trim());
            if (!t) throw new Error("Not a valid URL.");
            return t && {
                url: t[0],
                origin: t[1].replace(t[6], ""),
                protocol: t[4] || "",
                hasWWW: Boolean(t[9]),
                username: t[7] || "",
                password: t[8] || "",
                host: (t[9] || "") + t[10],
                domain: t[10],
                port: t[11] || "",
                path: t[12],
                query: t[13] || "",
                queryParams: t[13] ? t[13].split("&").reduce(((e, t) => {
                    const [i, n = ""] = t.split("=");
                    return e[i] = n, e
                }), {}) : {},
                fragment: t[14] || "",
                urlWithoutProtocol: t[0].replace(t[3], ""),
                urlWithoutProtocolAndWww: t[0].replace(t[2], "")
            }
        }
    }
    var Dt = new xt(Lt.parseUrl(window.location.href).queryParams.vwoLogLevel || "error");
    const {
        toString: Ut
    } = Object.prototype;

    function Wt(e) {
        return "[object Object]" === Ut.call(e)
    }

    function Mt(e) {
        return "[object Array]" === Ut.call(e)
    }

    function kt(e) {
        return "[object Null]" === Ut.call(e)
    }

    function Gt(e) {
        return "[object Undefined]" === Ut.call(e)
    }

    function Ft(e) {
        return !Gt(e) && !kt(e)
    }

    function jt(e) {
        return !Number.isNaN(e) && "[object Number]" === Ut.call(e)
    }

    function Ht(e) {
        return "[object String]" === Ut.call(e)
    }

    function $t(e) {
        return "[object Boolean]" === Ut.call(e)
    }

    function Bt(e) {
        return "[object Function]" === Ut.call(e)
    }

    function Kt(e) {
        return "[object AsyncFunction]" === Ut.call(e)
    }

    function zt(e) {
        return Wt(e) ? "Object" : Mt(e) ? "Array" : kt(e) ? "Null" : Gt(e) ? "Undefined" : function(e) {
            return e != e
        }(e) ? "NaN" : jt(e) ? "Number" : Ht(e) ? "String" : $t(e) ? "Boolean" : function(e) {
            return "[object Date]" === Ut.call(e)
        }(e) ? "Date" : function(e) {
            return "[object RegExp]" === Ut.call(e)
        }(e) ? "Regex" : Bt(e) ? "Function" : Kt(e) ? "AsyncFunction" : function(e) {
            return "[object Promise]" === Ut.call(e)
        }(e) ? "Promise" : "Unknown Type"
    }
    var Yt, Jt, Xt, qt, Qt = new class {
        mergeNestedObjects(...e) {
            return e.reduce(((e, t) => this.recursivelyMerge(e, t)))
        }
        mergeNestedObjectsV2(e = {
            mergeArrays: !1
        }, ...t) {
            return t.reduce(((t, i) => this.recursivelyMerge(t, i, {}, e)))
        }
        createNestedObjects(e, t) {
            let i = e;
            return t && t.split(".").forEach((e => {
                Object.prototype.hasOwnProperty.call(i, e) || (i[e] = {}), i = i[e]
            })), i
        }
        clearNestedObject(e, t) {
            let i = e;
            const n = t.split("."),
                s = n[n.length - 1];
            for (let e = 0; e < n.length - 1; e++) i = i[n[e]];
            Wt(i[s]) ? i[s] = {} : delete i[s]
        }
        recursivelyMerge(e, t, i = {}, n = {
            mergeArrays: !1
        }) {
            if (Wt(e) && Wt(t)) {
                const s = {};
                Object.keys(e).concat(Object.keys(t)).forEach((e => {
                    s[e] = 1
                }));
                const r = Object.getOwnPropertyDescriptors(e),
                    o = Object.getOwnPropertyDescriptors(t);
                return Object.keys(s).forEach((s => {
                    o[s] ? Object.defineProperty(i, s, o[s]) : Object.defineProperty(i, s, r[s]), this.recursivelyMerge(e[s], t[s], i[s], n)
                })), i
            }
            return n.mergeArrays && Mt(e) && Mt(t) ? (Mt(i) || (i = []), i.splice(0, i.length, ...e.concat(t)), i) : t || e
        }
    };
    ! function(e) {
        e.SETTINGS = "settings", e.CUSTOM = "custom", e.TRIGGERS = "triggers", e.TAGS = "tags", e.EVENTS = "events", e.DEBUG = "debug", e.ROOT = "root"
    }(Yt || (Yt = {})), (Jt || (Jt = {})).HISTORY = "history",
        function(e) {
            e.LOGS = "logs"
        }(Xt || (Xt = {})),
        function(e) {
            e.ALIAS = "$alias"
        }(qt || (qt = {}));
    class Zt {
        constructor() {
            this.originalValues = {
                [Yt.SETTINGS]: {},
                [Yt.CUSTOM]: {},
                [Yt.TRIGGERS]: {},
                [Yt.EVENTS]: {
                    [Jt.HISTORY]: []
                },
                [Yt.DEBUG]: {
                    [Xt.LOGS]: []
                }
            }, this.reset()
        }
        reset() {
            this.values = JSON.parse(JSON.stringify(this.originalValues)), this.setValues(this.values)
        }
        getValue(e = "") {
            try {
                return e.split(".").reduce(((e, t) => e[t]), this.values)
            } catch (e) {
                return null
            }
        }
        setValues(e, t = "") {
            const i = this.getNamespace(t);
            if (Wt(e)) {
                const t = i ? Qt.createNestedObjects(this.values, i) : this.values;
                Object.keys(e).forEach((i => {
                    const n = Object.getOwnPropertyDescriptor(e, i);
                    Object.prototype.hasOwnProperty.call(n, "get") ? Object.defineProperty(t, i, n) : t[i] = e[i]
                }))
            }
        }
        clear(e) {
            const t = this.getNamespace(e);
            Qt.clearNestedObject(this.values, t)
        }
        getNamespace(e) {
            return (null == e ? void 0 : e.startsWith(".")) ? e.substring(1) : e
        }
    }
    Zt.logger = new xt("warn");
    var ei = new Zt,
        ti, ii = new class {
            constructor() {
                this.plugins = {}
            }
            register(e) {
                Dt.debug(`Registering plugin '${e.pluginName}' in Plugins factory`), this.plugins[e.pluginName] = e
            }
            unregister(e) {
                let t;
                t = Ht(e) ? e : e.pluginName, Dt.debug(`Unregistering plugin '${t}' in Plugins factory`), this.plugins[t].removeAll(), delete this.plugins[t]
            }
            unregisterAll() {
                Dt.debug("Unregistering all plugins in Plugins factory"), Object.keys(this.plugins).forEach((e => {
                    this.plugins[e].removeAll(), delete this.plugins[e]
                }))
            }
            clearData() {
                Dt.debug("Clearing the data of all the plugins"), Object.keys(this.plugins).forEach((e => {
                    this.plugins[e].clearData()
                }))
            }
        };
    ! function(e) {
        e.EVENT = "event", e.EVENT_PROPS = "eventProps", e.STORAGE = "storage", e.FORMULA = "formula", e.OPERATOR = "operator", e.TAG = "tag", e.CONDITION_LEVEL_OPERATOR = "clOperator"
    }(ti || (ti = {}));
    var ni, si = new class {
        get(e) {
            return this[e]
        }
        set(e, t) {
            this[e] = t
        }
    };
    ! function(e) {
        e[e.EXCLUDE_PASSED = 1] = "EXCLUDE_PASSED", e[e.INCLUDE_PASSED = 2] = "INCLUDE_PASSED", e[e.INCLUDE_FAILED = 3] = "INCLUDE_FAILED"
    }(ni || (ni = {}));
    var ri, oi = ni;
    ! function(e) {
        e.OR = "o", e.AND = "a"
    }(ri || (ri = {}));
    var ai = ri;
    class li {
        constructor() {
            this.experimentConfig = {}, this.pageConfig = {}, this.experimentConfigCache = {}, this.pageConfigCache = {}, this.previewParamsCleanedUrlCache = {}, li.cleanerRegex = /(^https?:\/\/)?(w{3}\.)?(.*?)?((?:\/)(?:home|default|index)\.\w{3,4})?(\/)?([?#].*)?$/i, li.logicalOperators = [ai.AND, ai.OR]
        }
        static get currentUrl() {
            return window.location.href
        }
        add(e, t) {
            if (Dt.debug("Adding pageGroup config to phoenix"), Ft(e) && (Object.hasOwnProperty.call(e, "ec") && e.ec.forEach((e => {
                    const t = Object.keys(e)[0];
                    this.experimentConfig[t] || (this.experimentConfig[t] = e[t])
                })), Object.hasOwnProperty.call(e, "pc") && e.pc.forEach((e => {
                    const t = Object.keys(e)[0];
                    this.pageConfig[t] || (this.pageConfig[t] = e[t])
                }))), Ft(t)) {
                if (Mt(t.pc)) {
                    const e = this.getCache(li.currentUrl, !0);
                    t.pc.forEach((t => {
                        e[t] = {
                            didMatch: !0,
                            reason: oi.INCLUDE_PASSED,
                            cacheHit: !0
                        }
                    }))
                }
                if (Mt(t.ec)) {
                    const e = this.getCache(li.currentUrl);
                    t.ec.forEach((t => {
                        e[t] = {
                            didMatch: !0,
                            reason: oi.INCLUDE_PASSED,
                            cacheHit: !0
                        }
                    }))
                }
            }
        }
        getCache(e, t) {
            return t ? (this.pageConfigCache = this.pageConfigCache || {}, this.pageConfigCache[e] = this.pageConfigCache[e] || {}, this.pageConfigCache[e]) : (this.experimentConfigCache = this.experimentConfigCache || {}, this.experimentConfigCache[e] = this.experimentConfigCache[e] || {}, this.experimentConfigCache[e])
        }
        getPreviewParamsCleanedUrl(e) {
            return e ? (this.previewParamsCleanedUrlCache = this.previewParamsCleanedUrlCache || {}, this.previewParamsCleanedUrlCache[e] || (this.previewParamsCleanedUrlCache[e] = si.get("jsLibUtils").getCleanedUrl(e, !0)), this.previewParamsCleanedUrlCache[e]) : e
        }
        getIndexFileCleanedUrl(e) {
            return e ? (this.indexFileCleanedUrlCache = this.indexFileCleanedUrlCache || {}, this.indexFileCleanedUrlCache[e] || (this.indexFileCleanedUrlCache[e] = e.replace(li.cleanerRegex, "$1$2$3$5$6")), this.indexFileCleanedUrlCache[e]) : e
        }
        validatePage(e, t, i, n) {
            const s = t ? this.pageConfig[e] : this.experimentConfig[e];
            if (!s) return Dt.info(`ConfigId ${e} is not present inside ${t?"pageConfig":"experimentConfig"}`), {
                didMatch: !1,
                reason: oi.INCLUDE_FAILED,
                cacheHit: !1
            };
            const r = i || li.currentUrl,
                o = this.getCache(r, t);
            if (o && Object.hasOwnProperty.call(o, e)) return Dt.info(`Fetching value from cache for ${t?"pageConfigId":"experimentConfigId"} = ${e}`), o[e].cacheHit = !0, o[e];
            let a;
            const l = s.exc,
                d = s.inc;
            if (Array.isArray(l) && l.length > 0) {
                const t = this.evaluateDSL(l, r, n || !1);
                if (t) return a = {
                    didMatch: !t,
                    reason: oi.EXCLUDE_PASSED,
                    cacheHit: !1
                }, n || (o[e] = a), a
            }
            if (Array.isArray(d))
                if (d.length) {
                    const e = this.evaluateDSL(d, r, n || !1);
                    a = e ? {
                        didMatch: e,
                        reason: oi.INCLUDE_PASSED,
                        cacheHit: !1
                    } : {
                        didMatch: e,
                        reason: oi.INCLUDE_FAILED,
                        cacheHit: !1
                    }
                } else a = {
                    didMatch: !0,
                    reason: oi.INCLUDE_PASSED,
                    cacheHit: !1
                };
            return a = a || {
                didMatch: !1,
                reason: oi.INCLUDE_FAILED,
                cacheHit: !1
            }, n || (o[e] = a), a
        }
        evaluateDSL(e, t, i) {
            if (!Mt(e) || e.length < 2) return Dt.error("Invalid dsl tree", e), !1;
            const n = [];
            return e.forEach((e => {
                var s;
                let r;
                if (e || (r = !1), Ht(e) && (r = e), Mt(e))
                    if (li.logicalOperators.includes(e[0])) r = this.evaluateDSL(e, t, i);
                    else {
                        const [n, o, ...a] = e, l = null === (s = ii.plugins[ti.OPERATOR]) || void 0 === s ? void 0 : s.get(o);
                        let d;
                        if (n.includes("url")) d = this.getIndexFileCleanedUrl(this.getPreviewParamsCleanedUrl(t));
                        else {
                            const e = a[0];
                            d = this.validatePage(e, !0, t, i).didMatch, a[0] = !0
                        }
                        r = null == l ? void 0 : l(d, ...a, {
                            jsLibUtils: si.get("jsLibUtils"),
                            pageUrl: !0
                        })
                    }
                n.push(r || !1)
            })), this.evaluateTree(n)
        }
        evaluateTree(e) {
            let t = !1;
            switch (e[0]) {
                case ai.AND:
                    t = !e.includes(!1);
                    break;
                case ai.OR:
                    t = e.includes(!0)
            }
            return t
        }
    }
    var di = new li,
        ci = new class {
            constructor() {
                const e = function(e, t) {
                    Object.defineProperty(this, e, {
                        get: t,
                        enumerable: !0
                    })
                }.bind(this);
                e("settings", (() => ei.values[Yt.SETTINGS])), e("custom", (() => ei.values[Yt.CUSTOM])), e("url", (() => window._vis_opt_url || window.location.href)), e("refUrl", (() => window.document.referrer))
            }
            get triggers() {
                return ei.values[Yt.TRIGGERS]
            }
            get tags() {
                return ei.values[Yt.TAGS]
            }
            get pageGroupExperimentConfig() {
                return di.experimentConfig
            }
            get pageGroupPageConfig() {
                return di.pageConfig
            }
            get events() {
                return ei.values[Yt.EVENTS]
            }
            get eventsHistory() {
                return this.events[Jt.HISTORY]
            }
            get window() {
                return window
            }
            addNameSpace(e = "") {
                const t = e.split(".")[0];
                t in this || Object.defineProperty(this, t, {
                    get: () => ei.values[t],
                    configurable: !0,
                    enumerable: !0
                })
            }
            getValue(e = "") {
                return ei.getValue(e)
            }
            getHistoryEvents(e) {
                var t, i, n;
                const s = [];
                null === (t = this.events[Jt.HISTORY]) || void 0 === t || t.forEach((({
                    name: t,
                    event: i
                }) => {
                    t === e && s.push(i)
                }));
                const r = null === (n = null === (i = window.VWO) || void 0 === i ? void 0 : i._) || void 0 === n ? void 0 : n.crossStore;
                let o = null == r ? void 0 : r.getLocal({
                    key: Jt.HISTORY
                });
                return o && (o = JSON.parse(o)), o && o.forEach((({
                    name: t,
                    event: i
                }) => {
                    t === e && s.push(i)
                })), s
            }
            async getCrossStoreHistoryEvents(e) {
                return new Promise((t => {
                    const i = [];
                    window.fetcher.getValue("VWO._.crossStore.getLocal", [{
                        key: Jt.HISTORY
                    }]).then((n => {
                        let s;
                        n && (s = JSON.parse(n)), s && s.forEach((({
                            name: t,
                            event: n
                        }) => {
                            t === e && i.push(n)
                        })), t(i)
                    })).catch((() => {
                        t(i)
                    }))
                }))
            }
        },
        ui = {
            MAX_EVENTS_HISTORY: 1e4,
            MAX_LOGS_HISTORY: 1e3,
            EVENTS_TO_PERSIST: ["s.q", "s.r"]
        };
    class gi {
        refreshState(e) {
            ei.setValues(e)
        }
        addValues(e, t = Yt.CUSTOM) {
            const i = t === Yt.ROOT ? "" : t;
            ei.setValues(e, i), i ? ci.addNameSpace(i) : Object.keys(e).forEach((e => {
                ci.addNameSpace(e)
            }))
        }
        set(e, t, i = Yt.CUSTOM) {
            this.addValues({
                [e]: t
            }, i)
        }
        clear(e) {
            ei.clear(e)
        }
        clearAll() {
            ei.reset()
        }
        updateSettings(e, t = "") {
            this.addValues(e, t ? `${Yt.SETTINGS}.${t}` : Yt.SETTINGS)
        }
        addEventInHistory({
            name: e,
            event: t
        }) {
            const i = ei.values[Yt.EVENTS][Jt.HISTORY];
            i.push({
                name: e,
                event: t
            }), i.length > ui.MAX_EVENTS_HISTORY && i.shift(), ui.EVENTS_TO_PERSIST.indexOf(e) > -1 && window.fetcher.getValue("VWO._.crossStore.getLocal", [{
                key: Jt.HISTORY
            }]).then((i => {
                let n = i;
                n ? (n = JSON.parse(n), n.push({
                    name: e,
                    event: t
                })) : n = [{
                    name: e,
                    event: t
                }], window.fetcher.getValue("VWO._.crossStore.set", [Jt.HISTORY, JSON.stringify(n)])
            })).catch((e => {
                console.log(e)
            }))
        }
        addLogsForDebugging(e) {
            const t = ei.values[Yt.DEBUG][Xt.LOGS];
            t.push(e), t.length > ui.MAX_LOGS_HISTORY && t.shift()
        }
        updateTriggerState(e, t) {
            this.addValues({
                state: t
            }, `${Yt.TRIGGERS}.${e}`)
        }
        updateTriggerLastMetTS(e, t) {
            this.addValues({
                lastMetTS: t
            }, `${Yt.TRIGGERS}.${e}`)
        }
        updateTagState(e, t) {
            gi.logger.debug(`Updating tag ${e} in store' with `, t), this.addValues(t, `${Yt.TAGS}.${e}`)
        }
        clearTriggerState(e = "") {
            this.clear(e ? `${Yt.TRIGGERS}.${e}` : Yt.TRIGGERS)
        }
        updateTriggerExecutionCount(e, t) {
            this.addValues({
                executionCount: t
            }, `${Yt.TRIGGERS}.${e}`)
        }
    }
    gi.logger = new xt("warn");
    var vi = new gi,
        hi = new class {
            setStoragePlugin(e) {
                this.storagePlugin = e
            }
        };
    const pi = "{{survey_id}}";

    function wi(e, t) {
        if (e.filters)
            for (let i = 0; i < e.filters.length; i++)
                if ("string" != typeof e.filters[i])
                    for (let n = 0; n < e.filters[i].length; n++) e.filters[i][n] === pi && (e.filters[i][n] = t);
        return e
    }
    var Ei = new class {
        async execute({
            callbacks: e,
            data: t,
            eventName: i
        }) {
            let n = t;
            for (let t = 0; t < e.length; t++)
                if (Bt(e[t]) || Kt(e[t])) {
                    const s = await e[t](n, i);
                    s && (n = s)
                }
        }
    };

    function fi(e) {
        const t = e;
        return t.VWO = t.VWO || {
            firedTime: Date.now()
        }, t.VWO.firedTime = t.VWO.firedTime || Date.now(), t
    }
    var _i, mi = new class {
        constructor() {
            this.events = {}, this.eventIdsMapping = {}, this.globalInterceptors = {}, this.eventId = 0, this.eventsFrequencyManager = {}
        }
        setGlobalInterceptors(e = {}) {
            this.globalInterceptors = e
        }
        async trigger(e, t = {}, i = {}) {
            const n = fi(t);
            e !== Nt.TIMER && Dt.info(`Triggering event '${e}'`, {
                data: n,
                event: this.events[e]
            }), Object.values(Nt).every((t => !e.startsWith(t))) && !(null == n ? void 0 : n.preventWildCardEvent) && (this.addComputedEventProps(e, n), await this.trigger(Nt.WILD_CARD, n, {
                eventName: e
            }), vi.addEventInHistory({
                name: e,
                event: n
            }));
            const s = this.events[e];
            let r;
            r = this.eventsFrequencyManager[e] ? this.eventsFrequencyManager[e].then((() => this.executeListeners(e, n, i, s))) : this.executeListeners(e, n, i, s), this.eventsFrequencyManager[e] = r, await r
        }
        async executeListeners(e, t, i = {}, n) {
            const s = null == n ? void 0 : n.length;
            if (s) {
                const r = [],
                    o = n.push;
                n.push = s => {
                    o.call(n, s), r.push(new Promise((n => {
                        queueMicrotask((async () => {
                            await Ei.execute({
                                callbacks: [this.globalInterceptors.before, s.before, s.fn, this.globalInterceptors.after, s.after],
                                data: t,
                                eventName: i.eventName || e
                            }), this.eventIdsMapping[s.id].executionCount++, n(null)
                        }))
                    })))
                };
                for (let o = 0; o < s; ++o) {
                    const s = n[o];
                    Dt.debug(`Synchronously executing listener for event '${s.id}' with event name '${e}'`), r.push(Ei.execute({
                        callbacks: [this.globalInterceptors.before, s.before, s.fn, this.globalInterceptors.after, s.after],
                        data: t,
                        eventName: i.eventName || e
                    }).then((() => {
                        this.eventIdsMapping[s.id].executionCount++
                    })))
                }
                await Promise.all(r), n.push = o
            } else e !== Nt.TIMER && Dt.debug(`No callbacks for the event ${e}. Events List in eventBus is`, this.events)
        }
        on(e, t, {
            before: i,
            after: n
        } = {}) {
            var s;
            Dt.debug(`Attaching listener for event name '${e}'`);
            const r = ++this.eventId,
                o = Object.assign(Object.assign({
                    id: r,
                    fn: t
                }, i && {
                    before: i
                }), n && {
                    after: n
                });
            return this.events[e] = this.events[e] || [], this.events[e].push(o), this.eventIdsMapping[r] = {
                executionCount: 0,
                event: o
            }, null === (s = ii.plugins[ti.EVENT]) || void 0 === s || s.addDomEvent(e), this.eventId
        }
        off(e, t) {
            var i, n;
            Dt.debug(`Removing a listener for event '${e}'`, {
                reference: t
            });
            const s = null === (i = this.events[e]) || void 0 === i ? void 0 : i.push;
            this.events[e] = null === (n = this.events[e]) || void 0 === n ? void 0 : n.filter((e => Bt(t) ? e.fn !== t : e.id !== Number(t))), s && (this.events[e].push = s)
        }
        remove(e) {
            Dt.debug(`Removing all the listener for event '${e}'`), this.events[e] = []
        }
        removeAll() {
            Dt.debug("Removing all the listener for every event"), this.events = {}, this.eventIdsMapping = {}
        }
        event(e) {
            return {
                register(t, i) {
                    Dt.debug(`Registering hooks for event ID '${e}'`, {
                        hook: t
                    }), this.eventIdsMapping[e][t] = i
                },
                get executionCount() {
                    return this.eventIdsMapping[e].executionCount
                },
                get hasExecuted() {
                    return Boolean(this.eventIdsMapping[e].executionCount)
                }
            }
        }
        addComputedEventProps(e, t) {
            var i, n;
            const s = (null === (i = ii.plugins[ti.EVENT_PROPS]) || void 0 === i ? void 0 : i.get("*")) || {},
                r = (null === (n = ii.plugins[ti.EVENT_PROPS]) || void 0 === n ? void 0 : n.get(e)) || {},
                o = Object.assign(Object.assign({}, s), r);
            Object.keys(o).forEach((i => {
                t[i] = o[i](t, e)
            }))
        }
    };
    ! function(e) {
        e.AND = "a", e.OR = "o"
    }(_i || (_i = {}));
    var Oi, Si = new class {
        evaluateExpression(e) {
            return e && this.recursivelyEvaluateTree(0, e)
        }
        recursivelyEvaluateTree(e, t) {
            const i = t[e];
            if ($t(i)) return Boolean(t[e]);
            if (Mt(i)) return Boolean(i[0]);
            if (!Ft(i)) return !0;
            const n = 2 * (e + 1) - 1,
                s = 2 * (e + 1);
            return this.isValid(Ht(i) ? i : JSON.parse(JSON.stringify(i)), this.recursivelyEvaluateTree(n, t), this.recursivelyEvaluateTree(s, t))
        }
        isValid(e, t, i) {
            switch (e) {
                case _i.AND:
                    return t && i;
                case _i.OR:
                    return t || i;
                default:
                    throw new Error(e)
            }
        }
    };
    class yi {
        clearData() {}
    }! function(e) {
        e.EVENT = "event", e.FORMULA = "formula", e.CUSTOM = "custom", e.SETTINGS = "settings", e.DEFAULT = "default", e.STORAGE = "storage", e.WINDOW = "window", e.TAGS = "tags"
    }(Oi || (Oi = {}));
    var Ti = new class {
            async getValue({
                event: e,
                variableName: t,
                formulaValues: i
            }) {
                return Dt.debug(`Extracting value of variable '${t}'`), Mt(t) ? Promise.all(t.map((async t => await this.extractVariableValue({
                    event: e,
                    variableName: t,
                    formulaValues: i
                })))) : this.extractVariableValue({
                    event: e,
                    variableName: t,
                    formulaValues: i
                })
            }
            async extractVariableValue({
                event: e,
                variableName: t,
                formulaValues: i
            }) {
                var n, s;
                Dt.debug(`Extracting value of variable '${t}'`);
                const {
                    type: r,
                    key: o
                } = this.getVariableTypeAndKey(t);
                let a, l = !1;
                switch (Dt.debug(`Evaluated type and key of variable '${t}' are `, {
                    type: r,
                    key: o
                }), r) {
                    case Oi.EVENT:
                        a = e;
                        break;
                    case Oi.FORMULA:
                        a = i;
                        break;
                    case Oi.CUSTOM:
                        a = ci.custom;
                        break;
                    case Oi.SETTINGS:
                        a = ci.settings;
                        break;
                    case Oi.WINDOW:
                        a = this.getValueFromMtWindow(o);
                        break;
                    case Oi.TAGS:
                        try {
                            a = o.startsWith("js") ? await (null === (n = ii.plugins[ti.TAG]) || void 0 === n ? void 0 : n.get(o).fn()) : null === (s = ii.plugins[ti.TAG]) || void 0 === s ? void 0 : s.get(o).fn
                        } catch (e) {}
                        break;
                    case Oi.STORAGE:
                        {
                            const e = ii.plugins[ti.STORAGE],
                                t = o.split("."),
                                [i, n, s] = t,
                                r = null == e ? void 0 : e.get(i);a = "includes" === n ? await (r && r.includes ? r.includes(s, "cookies" === i) : void 0) : await (r && r.getItem ? r.getItem(n, "cookies" === i) : void 0);
                            break
                        }
                    default:
                        a = ci, l = !0
                }
                let d = r === Oi.STORAGE || r === Oi.WINDOW ? a : this.extractValue(a, o);
                return d = r === Oi.TAGS ? a : d, l && Gt(d) && (d = this.getValueFromMtWindow(o)), Dt.debug(`Evaluated value of variable '${t}' is '${d}'`), d
            }
            async getValueFromMtWindow(e) {
                const t = window.fetcher || {
                    getValue: e => Promise.resolve(window[e])
                };
                let i;
                try {
                    i = await t.getValue(e)
                } catch (e) {}
                return i
            }
            getVariableTypeAndKey(e) {
                var t;
                Dt.debug(`Getting type and key of variable '${e}'`);
                const i = e.split("."),
                    n = null === (t = i.splice(0, 1)[0]) || void 0 === t ? void 0 : t.toLowerCase();
                return Object.values(Oi).includes(n) ? {
                    type: n,
                    key: i.join(".")
                } : {
                    type: Oi.DEFAULT,
                    key: e
                }
            }
            extractValue(e, t) {
                try {
                    return e[t] || t.split(".").reduce(((e, t) => e[t]), e)
                } catch (e) {
                    return
                }
            }
        },
        Ii = new class {
            sum(e, t) {
                return e.reduce(((e, i) => e + t.reduce(((e, t) => e + (Number(i[t]) || 0)), 0)), 0)
            }
            multiply(e, t) {
                return e.reduce(((e, i) => e + t.reduce(((e, t) => e * (Number(i[t]) || 1)), 1)), 1)
            }
        };
    class Ci {
        static ordered(e, t) {
            const i = Ci.getTriggerConditionsOrderState(e.id);
            for (let n = 0; n < t.length; n++) {
                const s = t[n];
                if (!i[s] && !e.currentSatisfiedConditions[s]) return !1;
                i[s] = !0, vi.updateTriggerState(Ci.generateTriggerOrderingNamespace(e.id), i)
            }
            return !0
        }
        static strict(e, t) {
            const i = Object.keys(Ci.getTriggerConditionsOrderState(e.id)).map((e => Number(e))).sort(((e, i) => t.indexOf(e) - t.indexOf(i))),
                n = Object.keys(e.currentSatisfiedConditions).filter((t => Boolean(e.currentSatisfiedConditions[t]))).map((e => Number(e))).sort(((e, i) => t.indexOf(e) - t.indexOf(i))),
                s = [...i, ...n].slice(0, t.length);
            let r = {};
            for (let i = 0; i < s.length; i++) {
                if (t[i] !== s[i]) {
                    r = {};
                    for (let e = 0; e < n.length && t[e] === n[e]; e++) r[n[e]] = !0;
                    return vi.updateTriggerState(Ci.generateTriggerOrderingNamespace(e.id), r), !1
                }
                r[s[i]] = !0
            }
            return vi.updateTriggerState(Ci.generateTriggerOrderingNamespace(e.id), r), s.length === t.length
        }
        static generateTriggerOrderingNamespace(e) {
            return e + ".order"
        }
        static getTriggerConditionsOrderState(e) {
            return ci.getValue(`${Yt.TRIGGERS}.${this.generateTriggerOrderingNamespace(e)}.state`) || {}
        }
    }
    var Ni = {
        sum: Ii.sum,
        multiply: Ii.multiply,
        ordered: Ci.ordered,
        strict: Ci.strict
    };
    const Ai = {
            UNKNOWN_SET_API_TYPE: "Unknown type '{{type}}' found in set API.",
            EVENTS: {
                ALREADY_EXISTS: "Event with name '{{eventName}}' already exists. Please use 'update' API if you want to override it.",
                NOT_REGISTERED: "Event with name '{{eventName}}' has not been registered yet. Please use 'add' API to register it."
            },
            OPERATORS: {
                ALREADY_EXISTS: "Operator with name '{{operatorName}}' already exists. Please use 'update' API if you want to override it.",
                NOT_REGISTERED: "Operator with name '{{operatorName}}' has not been registered yet. Please use 'add' API to register it."
            },
            FORMULAS: {
                ALREADY_EXISTS: "Formula with name '{{formulaName}}' already exists. Please use 'update' API if you want to override it.",
                NOT_REGISTERED: "Formula with name '{{formulaName}}' has not been registered yet. Please use 'add' API to register it."
            },
            STORAGES: {
                ALREADY_EXISTS: "Storage with name '{{storageName}}' already exists. Please use 'update' API if you want to override it.",
                NOT_REGISTERED: "Storage with name '{{storageName}}' has not been registered yet. Please use 'add' API to register it."
            },
            TAGS: {
                ALREADY_EXISTS: "Tag with name '{{tagName}}' already exists. Please use 'update' API if you want to override it.",
                NOT_REGISTERED: "Tag with name '{{tagName}}' has not been registered yet. Please use 'add' API to register it."
            },
            EVENT_PROP: {
                ALREADY_EXISTS: "Event property with name '{{propName}}' already exists for event '{{eventName}}'. Please use 'update' API if you want to override it.",
                NOT_REGISTERED: "Event property with name '{{propName}}' has not been registered yet for event '{{eventName}}'. Please use 'add' API to register it."
            }
        },
        Vi = {
            EVENTS: {
                NO_EVENT_TO_REMOVE: "Unable to remove Event '{{eventName}}' as it's not been registered."
            },
            OPERATORS: {
                NO_OPERATOR_TO_REMOVE: "Unable to remove Operator '{{operatorName}}' as it's not been registered."
            },
            FORMULAS: {
                NO_FORMULA_TO_REMOVE: "Unable to remove Formula '{{formulaName}}' as it's not been registered."
            },
            STORAGES: {
                NO_STORAGE_TO_REMOVE: "Unable to remove Storage '{{storageName}}' as it's not been registered."
            },
            TAGS: {
                NO_TAG_TO_REMOVE: "Unable to remove Tag '{{tagName}}' as it's not been registered."
            },
            EVENT_PROP: {
                NO_EVENT_PROP_TO_REMOVE: "Unable to remove Event property '{{propName}}' for event '{{eventName}}' as it's not been registered."
            }
        };
    class bi extends yi {
        constructor() {
            super(), this.pluginName = ti.FORMULA, this.formulas = {}, this.initialize()
        }
        add(e, t) {
            Dt.debug(`Adding formula '${e}' in FormulasManager`), this.formulas[e] ? Dt.error(Ai.FORMULAS.ALREADY_EXISTS, {
                formulaName: e
            }) : this.formulas[e] = t
        }
        update(e, t) {
            Dt.debug(`Updating formula '${e}' in FormulasManager`), this.formulas[e] = t
        }
        get(e) {
            return Dt.debug(`Getting formula '${e}' in FormulasManager`), this.formulas[e] ? this.formulas[e] : (Dt.error(Ai.FORMULAS.NOT_REGISTERED, {
                formulaName: e
            }), null)
        }
        remove(e) {
            Dt.debug(`Removing formula '${e}' in FormulasManager`), this.formulas[e] ? delete this.formulas[e] : Dt.warn(Vi.FORMULAS.NO_FORMULA_TO_REMOVE, {
                formulaName: e
            })
        }
        removeAll() {
            Dt.debug("Removing all formulas in FormulasManager"), this.formulas = {}
        }
        initialize() {
            Object.keys(Ni).forEach((e => {
                this.add(e, Ni[e])
            }))
        }
    }
    var Ri = new class {
        evaluate(e = [], t) {
            const i = {};
            return e.forEach((e => {
                const n = this.evaluateFormulaExpression(e, t);
                i[e.id] = n
            })), i
        }
        evaluateFilter(e, t) {
            return e.reduce(((e, t) => this.evaluateFormulaExpression(t, e)), t)
        }
        evaluateArgs(e, t) {
            return e.map((e => this.evaluateFormulaExpression(e, t)))
        }
        evaluateFormulaExpression(e, t) {
            var i;
            return (e.fn || (null === (i = ii.plugins[ti.FORMULA]) || void 0 === i ? void 0 : i.get(e.type)))(Ft(e.filters) ? this.evaluateFilter(e.filters, t) : t, Ft(e.args) ? e.args.map((e => Wt(e) ? this.evaluateArgs([e], t)[0] : e)) : e.args)
        }
    };
    class Pi {
        clearData() {}
        get persistedState() {
            return ""
        }
    }

    function xi(e) {
        return e && e instanceof Pi
    }

    function Li(e) {
        return Wt(e) && !xi(e)
    }
    var Di = new class {
            constructor() {
                this.serializeValidationQueue = {}
            }
            async isValid(e, t, i, n, s = {}, r = !1) {
                Dt.debug(`Checking validity of trigger '${e}'`, {
                    trigger: t,
                    event: i,
                    eventName: n
                }), this.saveEventState(e, n, !0);
                const {
                    isValid: o,
                    shouldRemoveEventListener: a
                } = await this.validateTrigger(t, i, {
                    eventName: n,
                    triggerName: e,
                    exitTriggersMet: s,
                    validateFromState: r
                });
                return this.saveTriggerState(e, this.hasTriggerMetBefore(e) || o), {
                    isValid: o,
                    shouldRemoveEventListener: a
                }
            }
            async validateTrigger(e, t, {
                eventName: i,
                triggerName: n,
                skipEventProps: s,
                exitTriggersMet: r,
                skipValidationIfTimer: o,
                validateFromState: a
            } = {}, l) {
                return n ? (this.serializeValidationQueue[n] = this.serializeValidationQueue[n] || Promise.resolve(), this.serializeValidationQueue[n] = this.serializeValidationQueue[n].then((() => a ? this.validateFromState(e, {
                    triggerName: n,
                    exitTriggersMet: r
                }) : this.validateTriggerHandler(e, t, {
                    eventName: i,
                    triggerName: n,
                    skipEventProps: s,
                    exitTriggersMet: r,
                    skipValidationIfTimer: o
                }, l))), this.serializeValidationQueue[n]) : this.validateTriggerHandler(e, t, {
                    eventName: i,
                    triggerName: n,
                    skipEventProps: s,
                    exitTriggersMet: r,
                    skipValidationIfTimer: o
                }, l)
            }
            async validateFromState(e, {
                triggerName: t
            } = {}) {
                const i = {},
                    n = e => {
                        const s = e => () => Mt(e) ? n(e) : e ? !1 !== e.persistState && this.hasConditionMetBefore(t, e.id) ? (i[e.id] = !0, Promise.resolve([!0, this.getTimeStampForConditionMet(t, e.id)])) : Promise.resolve([!1, 0]) : null;
                        return xi(e[0]) ? e[0].fn({}, ...e.slice(1).map(s)) : s(e[0])()
                    };
                let s = e.cnds;
                e.dslv && 1 !== e.dslv || (s = this.convertLevelOrderToTree(e.cnds, 0), s = Mt(s) ? s : [s]);
                const r = (await n(s))[0];
                return {
                    isValid: (!e.formula || e.formula.every((e => {
                        var n, s;
                        return null === (s = null === (n = ii.plugins[ti.FORMULA]) || void 0 === n ? void 0 : n.get(e.type)) || void 0 === s ? void 0 : s({
                            id: t,
                            currentSatisfiedConditions: i
                        }, e.args)
                    }))) && r,
                    shouldRemoveEventListener: !1
                }
            }
            iterateAndValidate(e, t, i, n, s, r, o = {}) {
                const a = e => async (o = {}) => {
                    if (Mt(e)) return this.iterateAndValidate(e, t, i, n, s, r, o);
                    let a = !0,
                        l = !1;
                    if (i) {
                        if ((!o.doNotUseCache || n !== e.event) && !1 !== e.persistState && this.hasConditionMetBefore(i, e.id)) return Dt.debug("Trigger condition already met before. Skipping it's validation.", {
                            triggerName: i,
                            conditionId: e.id
                        }), [!0, this.getTimeStampForConditionMet(i, e.id)];
                        a = this.hasEventOccurred(i, e.event), e.hist && e.event !== n && (l = !0), a || Dt.debug("Event hasn't occurred before. Skipping it's validation.", {
                            triggerName: i,
                            event: e.event
                        })
                    }
                    if (n && e.event !== n) return [!1, 0];
                    const d = (a || l) && await this.validateTriggerOperand(e, e.event === n || (null == t ? void 0 : t.useUnsavedEvents) ? t : {
                        fromLocalStorage: !0
                    }, {
                        eventName: e.event,
                        skipEventProps: s,
                        triggerName: i
                    });
                    return !d && !1 !== e.persistState && this.hasConditionMetBefore(i, e.id) ? [!0, this.getTimeStampForConditionMet(i, e.id)] : (i && this.saveConditionState(i, e.id, d, n === e.event ? t.VWO.firedTime : null), d && !1 !== e.persistState && (r[e.id] = !0), [d, d ? this.getTimeStampForConditionMet(i, e.id) : 0])
                };
                return xi(e[0]) ? e[0].fn(o, ...e.slice(1).map(a)) : a(e[0])()
            }
            convertLevelOrderToTree(e, t) {
                if (!e[t]) return null;
                if (Ht(e[t]) || xi(e[t])) {
                    const i = [e[t]],
                        n = this.convertLevelOrderToTree(e, 2 * (t + 1) - 1);
                    n && i.push(n);
                    const s = this.convertLevelOrderToTree(e, 2 * (t + 1));
                    return s && i.push(s), i
                }
                return e[t]
            }
            async validateTriggerHandler(e, t, {
                eventName: i,
                triggerName: n,
                skipEventProps: s,
                exitTriggersMet: r,
                skipValidationIfTimer: o
            } = {}, a) {
                try {
                    if (!e || !Wt(e)) return {
                        isValid: !0,
                        shouldRemoveEventListener: !1
                    };
                    if (o) {
                        const {
                            cnds: t
                        } = e, i = e => Mt(e) ? e.some(i) : !(!Li(e) || e.event !== Nt.TIMER);
                        if (t.some(i)) return {
                            isValid: !0,
                            shouldRemoveEventListener: !1
                        }
                    }
                    const l = {};
                    let d = !1;
                    if (i) {
                        const {
                            cnds: r
                        } = e, o = r.length, l = async e => {
                            if (Mt(e)) return void await Promise.all(e.map(l));
                            a && Li(e) && wi(e, a);
                            let r = !1;
                            if (Li(e) && e.event === i) {
                                let o = !0;
                                (e.filters || []).forEach((async e => {
                                    Ht(e) || "exec" === e[1] && (o = !1)
                                })), o && (r = await this.validateTriggerOperand(e, t, {
                                    eventName: i,
                                    triggerName: n,
                                    skipEventProps: s
                                }))
                            }
                            r && (d = !0)
                        };
                        for (let e = 0; e < o; ++e) await l(r[e])
                    } else d = !0;
                    let c = null == e ? void 0 : e.cnds;
                    e.dslv && 1 !== e.dslv || (c = this.convertLevelOrderToTree(c, 0), c = Mt(c) ? c : [c]);
                    const u = (await this.iterateAndValidate(c, t, n, i, s, l, {}))[0];
                    Dt.debug("Generated tree conditions for trigger", {
                        triggerName: n,
                        treeConditions: u
                    });
                    const g = !e.formula || e.formula.every((e => {
                            var t, i;
                            return null === (i = null === (t = ii.plugins[ti.FORMULA]) || void 0 === t ? void 0 : t.get(e.type)) || void 0 === i ? void 0 : i({
                                id: n,
                                currentSatisfiedConditions: l
                            }, e.args)
                        })),
                        v = this.shouldRemoveEventListener(i, c, l, r);
                    return {
                        isValid: d && g && u,
                        shouldRemoveEventListener: v
                    }
                } catch (i) {
                    let s;
                    throw (null == e ? void 0 : e.cnds) || (s = "Empty Triggger Present - " + n), 126657 === window._vwo_acc_id && window.VWO._.customError && window.VWO._.customError({
                        msg: i.stack || i.message || "Something wrong in TValidator",
                        url: "Error in triggerValidator",
                        lineno: 252,
                        colno: 5,
                        source: JSON.stringify({
                            triggerName: n,
                            event: t,
                            trigger: JSON.stringify(e),
                            errMsg: i.message,
                            errStk: i.stack
                        })
                    }), new Error(s || i)
                }
            }
            async validateTriggerOperand({
                filters: e,
                formula: t,
                id: i,
                hist: n
            }, s, {
                eventName: r,
                triggerName: o,
                skipEventProps: a = !1
            } = {}) {
                var l;
                const d = Ri.evaluate(t, [s]);
                if (n && Object.keys(n).length && r && s) {
                    const e = window.VWO.modules.eventHistHandler.getEventsByDate(r, n.dr, s);
                    let t;
                    if (null === (l = n.pf) || void 0 === l ? void 0 : l.length) {
                        const i = this.evaluateDSL.bind(this),
                            s = null == e ? void 0 : e.map((async e => i(n.pf, {
                                triggerName: o,
                                eventName: r,
                                event: e,
                                formulaValues: d
                            })));
                        t = s && await Promise.all(s), t = e.filter(((e, i) => t[i]))
                    } else t = e;
                    Object.keys(n.data).length && (s._meta = window.VWO.modules.eventHistHandler.getCumulativeData(t, n.data))
                }
                const c = null == e ? void 0 : e.map((async (e, t) => {
                        var n, l, c, u, g, v;
                        if (Ht(e)) return e;
                        const h = null === (u = null === (c = null === (l = null === (n = s) || void 0 === n ? void 0 : n.preComputedConds) || void 0 === l ? void 0 : l[o]) || void 0 === c ? void 0 : c[i]) || void 0 === u ? void 0 : u[t];
                        if ("boolean" == typeof h) return h;
                        let [p, w, ...E] = e;
                        if ((null === (g = window._vwoCc) || void 0 === g ? void 0 : g.syncServerUrl) && (E = await Promise.all(this.processRhsOperand(E))), a && p.startsWith(Oi.EVENT + ".")) return !0;
                        const f = await Ti.getValue({
                                event: s,
                                variableName: p,
                                formulaValues: d
                            }),
                            _ = null === (v = ii.plugins[ti.OPERATOR]) || void 0 === v ? void 0 : v.get(w);
                        return await (null == _ ? void 0 : _(f, ...E, {
                            eventName: r,
                            triggerName: o,
                            jsLibUtils: si.get("jsLibUtils")
                        })) || !1
                    })),
                    u = c && await Promise.all(c);
                return !u || this.evaluateFilterExpression(u)
            }
            evaluateFilterExpression(e) {
                if (!$t(e[0])) return Si.evaluateExpression(e);
                for (let t = e.length - 1; t >= 0; --t)
                    if (!e[t]) return !1;
                return !0
            }
            hasEventOccurred(e, t) {
                return Boolean(ci.getValue(`${Yt.TRIGGERS}.${this.generateTriggerEventId(e,t)}.state`))
            }
            hasConditionMetBefore(e, t) {
                return Boolean(ci.getValue(`${Yt.TRIGGERS}.${this.generateTriggerConditionId(e,t)}.state`))
            }
            getTimeStampForConditionMet(e, t) {
                return Number(ci.getValue(`${Yt.TRIGGERS}.${this.generateTriggerConditionId(e,t)}.lastMetTS`))
            }
            hasTriggerMetBefore(e) {
                return Boolean(ci.getValue(`${Yt.TRIGGERS}.${e}.state`))
            }
            saveTriggerState(e, t) {
                vi.updateTriggerState(e, t)
            }
            saveEventState(e, t, i) {
                Dt.debug("Saving trigger event's state i.e. it's event occurred or not.", {
                    triggerName: e,
                    eventName: t,
                    state: i
                }), vi.updateTriggerState(this.generateTriggerEventId(e, t), i)
            }
            saveConditionState(e, t, i, n) {
                Dt.debug("Saving trigger condition's state i.e. it's condition satisfied or not.", {
                    triggerName: e,
                    conditionId: t,
                    state: i
                }), vi.updateTriggerState(this.generateTriggerConditionId(e, t), i), i && vi.updateTriggerLastMetTS(this.generateTriggerConditionId(e, t), n)
            }
            generateTriggerConditionId(e, t) {
                return `${e}.trigger.${t}`
            }
            generateTriggerEventId(e, t) {
                return `${e}.event.${t}`
            }
            shouldRemoveEventListener(e, t, i, n) {
                if (e === Nt.TIMER) {
                    const e = t => Mt(t) ? t.every(e) : !(Li(t) && t.event === Nt.TIMER && !n[t.exitTrigger] && !i[t.id]);
                    return t.every(e)
                }
                return !1
            }
            async evaluateDSL(e, {
                triggerName: t,
                eventName: i,
                event: n,
                formulaValues: s
            }) {
                if (Mt(e) && e.length > 1) {
                    const r = e.map((async e => {
                            var r, o;
                            let a;
                            if (e || (a = !1), Ht(e) && (a = e), Mt(e))
                                if ([ai.AND, ai.OR].includes(e[0])) a = this.evaluateDSL(e, {
                                    triggerName: t,
                                    eventName: i,
                                    event: n,
                                    formulaValues: s
                                });
                                else {
                                    let [l, d, ...c] = e;
                                    (null === (r = window._vwoCc) || void 0 === r ? void 0 : r.syncServerUrl) && (c = await Promise.all(this.processRhsOperand(c)));
                                    const u = null === (o = ii.plugins[ti.OPERATOR]) || void 0 === o ? void 0 : o.get(d),
                                        g = await Ti.getValue({
                                            event: n,
                                            variableName: l,
                                            formulaValues: s
                                        });
                                    a = await (null == u ? void 0 : u(g, ...c, {
                                        eventName: i,
                                        triggerName: t,
                                        jsLibUtils: si.get("jsLibUtils")
                                    })) || !1
                                }
                            return a
                        })),
                        o = r && await Promise.all(r);
                    return this.evaluateTree(o)
                }
                return !1
            }
            evaluateTree(e) {
                let t = !1;
                switch (e[0]) {
                    case ai.AND:
                        t = !e.includes(!1);
                        break;
                    case ai.OR:
                        t = e.includes(!0)
                }
                return t
            }
            processRhsOperand(e) {
                return e.map((e => {
                    if ("string" == typeof e && e.startsWith("{{") && e.endsWith("}}")) {
                        const t = e.slice(2, -2);
                        return Ti.getValueFromMtWindow(t)
                    }
                    return e
                }))
            }
        },
        Ui = new class {
            constructor() {
                this.triggerEventListeners = {}
            }
            initialize(e) {
                Object.keys(e).forEach((t => {
                    this.triggerEventListeners[t] ? vi.updateTriggerExecutionCount(t, 0) : this.add(t, e[t])
                }))
            }
            add(e, t, i) {
                var n;
                Dt.debug(`Attaching event listeners for all the events using in '${e}' trigger conditions.`);
                const s = {};
                this.triggerEventListeners[e] = this.triggerEventListeners[e] || {};
                const r = n => {
                    var o, a;
                    if (Mt(n)) n.forEach(r);
                    else if (Li(n)) {
                        if (i && wi(n, i), null === (o = ii.plugins[ti.EVENT]) || void 0 === o || o.updateConditions(n.event, [this.extractDomEventFromCondition(e, n)]), !this.triggerEventListeners[e][n.event]) {
                            n.persistState && this.executeTriggerValidatorOfHistoryEvents(e, t, n.event), n.hist && (window.VWO.modules.eventHistHandler.triggerEvents.push({
                                triggerName: e,
                                trigger: t,
                                eventName: n.event
                            }), (null === (a = window._vwoCc) || void 0 === a ? void 0 : a.syncServerUrl) || this.validateEventFromHistHandler(e, t, n.event));
                            const i = mi.on(n.event, (async (n, s) => {
                                await this.validateAndDispatchTriggerEvent(e, t, n, s, i)
                            }));
                            this.triggerEventListeners[e][n.event] = i
                        }
                        if (n.exitTrigger) {
                            const i = this.getTriggerEventName(n.exitTrigger),
                                r = mi.on(i, (o => {
                                    s[n.exitTrigger] = 1;
                                    const a = this.triggerEventListeners[e][n.event];
                                    this.validateAndDispatchTriggerEvent(e, t, o, n.event, a, s), mi.off(i, r)
                                }));
                            this.triggerEventListeners[e][i] = r
                        }
                    }
                };
                null === (n = null == t ? void 0 : t.cnds) || void 0 === n || n.forEach(r)
            }
            async validateTriggerFromHistory(e, t) {
                let i = t;
                t.cnds && (i = t.cnds);
                const n = null == i ? void 0 : i.map((async t => {
                        var i;
                        if (null === (i = t) || void 0 === i ? void 0 : i.event) {
                            const i = ci.getHistoryEvents(t.event),
                                n = await ci.getCrossStoreHistoryEvents(t.event);
                            let s = !1;
                            const r = [...i, ...n].map((async i => {
                                const {
                                    isValid: n
                                } = await Di.isValid(e, {
                                    cnds: [t]
                                }, i, t.event);
                                n && (s = n)
                            }));
                            return await Promise.all(r), s
                        }
                        return Array.isArray(t) ? this.validateTriggerFromHistory(e, t) : t
                    })),
                    s = await Promise.all(n);
                return Si.evaluateExpression(s)
            }
            executeTriggerValidatorOfHistoryEvents(e, t, i) {
                ci.getHistoryEvents(i).forEach((n => this.validateAndDispatchTriggerEvent(e, t, n, i)))
            }
            async validateEventFromHistHandler(e, t, i) {
                await Di.isValid(e, t, fi({
                    isCustomEvent: !0,
                    fromLocalStorage: !0
                }), i)
            }
            async validateAndDispatchTriggerEvent(e, t, i, n, s, r, o = !1) {
                var a, l, d;
                try {
                    let a = this.getExecutionCount(e) || 0;
                    if (t.occurrence && t.occurrence <= a) return;
                    const {
                        isValid: l,
                        shouldRemoveEventListener: d
                    } = await Di.isValid(e, t, i, n, r, o);
                    if (d && mi.off(n, s), l) {
                        if (a = this.getExecutionCount(e) || 0, t.occurrence && t.occurrence <= a) return;
                        vi.updateTriggerExecutionCount(e, a + 1), await mi.trigger(this.getTriggerEventName(e), i)
                    }
                } catch (t) {
                    if (126657 === window._vwo_acc_id) {
                        const i = (null === (d = null === (l = null === (a = window.VWO) || void 0 === a ? void 0 : a._) || void 0 === l ? void 0 : l.allSettings) || void 0 === d ? void 0 : d.triggers[e]) || "defNotFound";
                        window.VWO._.customError && window.VWO._.customError({
                            msg: t.stack || t.message || "Something wrong in validateAndDispatch",
                            url: "Error in validateAndDispatch",
                            lineno: 252,
                            colno: 5,
                            source: JSON.stringify({
                                triggerName: e,
                                triggerDef: i,
                                errMsg: t.message,
                                errStk: t.stack
                            })
                        })
                    }
                }
            }
            remove(e) {
                Dt.debug(`Removing event listeners of trigger '${e}'`), Object.entries(this.triggerEventListeners[e]).forEach((([e, t]) => {
                    mi.off(e, t)
                })), delete this.triggerEventListeners[e]
            }
            removeAll() {
                Object.keys(this.triggerEventListeners).forEach((e => {
                    this.remove(e)
                }))
            }
            extractDomEvents(e) {
                Dt.debug("Extracting DOM events from triggers config");
                const t = {};
                return Object.keys(e).forEach((i => {
                    var n, s;
                    null === (s = null === (n = e[i]) || void 0 === n ? void 0 : n.cnds) || void 0 === s || s.forEach((e => {
                        Li(e) && this.extractDomEventFromCondition(i, e) && (t[e.event] = t[e.event] || [], t[e.event].push(Object.assign(Object.assign({}, e), {
                            triggerName: i
                        })))
                    }))
                })), t
            }
            extractDomEventFromCondition(e, t) {
                if (t.event === Vt.PAGE_UNLOAD_EVENT) this.setPageUnloadTrigger(e);
                else if (t.event.toLowerCase().includes(Ct.DOM + "_")) return Object.assign(Object.assign({}, t), {
                    triggerName: e
                });
                return null
            }
            setPageUnloadTrigger(e) {
                window.VWO._.pageUnloadTriggers = window.VWO._.pageUnloadTriggers || {}, window.VWO._.pageUnloadTriggers[e] = !0
            }
            getTriggerEventName(e) {
                return `${Nt.TRIGGER}.${e}`
            }
            getExecutionCount(e) {
                const t = ci.getValue(`${Yt.TRIGGERS}.${e}.executionCount`);
                return null != t ? t : 0
            }
        },
        Wi = {
            log: (new class {
                log({
                    event: e,
                    data: t,
                    getters: i,
                    actions: n,
                    vwoEvents: s
                }) {
                    console.log(e, t, i, n, s)
                }
            }).log
        },
        Mi = Object.freeze({
            __proto__: null,
            default: Wi
        });
    class ki extends yi {
        constructor() {
            super(), this.pluginName = ti.TAG, this.tags = {}, this.initialize()
        }
        add(e, t, {
            occurrence: i,
            scope: n,
            sync: s
        } = {}) {
            Dt.debug(`Adding tag '${e}' in TagsManager`), this.tags[e] ? Dt.error(Ai.TAGS.ALREADY_EXISTS, {
                tagName: e
            }) : this.tags[e] = {
                fn: t,
                occurrence: i,
                scope: n,
                sync: s
            }
        }
        update(e, t, {
            occurrence: i,
            scope: n,
            sync: s,
            fireUniquelyForEveryEvent: r
        } = {}) {
            Dt.debug(`Updating tag '${e}' in TagsManager`), this.tags[e] = {
                fn: t,
                occurrence: i,
                scope: n,
                sync: s,
                fireUniquelyForEveryEvent: r
            }
        }
        get(e) {
            return Dt.debug(`Getting tag '${e}' in TagsManager`), this.tags[e] ? this.tags[e] : (Dt.error(Ai.TAGS.NOT_REGISTERED, {
                tagName: e
            }), null)
        }
        remove(e) {
            Dt.debug(`Removing tag '${e}' in TagsManager`), this.tags[e] ? delete this.tags[e] : Dt.warn(Vi.TAGS.NO_TAG_TO_REMOVE, {
                tagName: e
            })
        }
        removeAll() {
            Dt.debug("Removing all tags in TagsManager"), this.tags = {}
        }
        initialize() {
            Object.keys(Mi).forEach((e => {
                this.add(e, Mi[e])
            }))
        }
    }
    var Gi = new class {
        flushTagData(e) {
            delete this._vwoEventsInstance[e]
        }
        getVwoInstanceObject() {
            return this._vwoEventsInstance
        }
        async executeTagUniquelyForEveryEvent(e) {
            if (!e || !e.tags) return;
            const t = [];
            Object.keys(e.tags).forEach((i => {
                const n = e.tags[i];
                if (n) {
                    const s = n.data,
                        r = this.getTagDetails(i).fn({
                            data: s,
                            event: e.event,
                            getters: ci,
                            actions: vi,
                            vwoEvents: this._vwoEventsInstance
                        });
                    t.push(r)
                }
            })), await Promise.all(t)
        }
        async execute(e, t, i, n, s) {
            var r, o, a, l, d, c;
            if (this._vwoEventsInstance = s, "metric" === t.id && (null === (l = null === (a = null === (o = null === (r = window.VWO) || void 0 === r ? void 0 : r.modules) || void 0 === o ? void 0 : o.utils) || void 0 === a ? void 0 : a.libUtils) || void 0 === l ? void 0 : l.isBot2())) return;
            const u = "metric" === t.id || "sampleVisitor" === t.id ? t.data : await Ti.getValue({
                    variableName: `${Yt.SETTINGS}.${t.data}`
                }),
                g = this.generateTagId(t.id),
                v = ci.tags[g].executionCount || 0,
                h = this.getTagDetails(t.id);
            if (!jt(h.occurrence) || v < h.occurrence) {
                Dt.info(`Started executing tag '${t.id}' because of:`, i), vi.updateTagState(g, {
                    hasExecuted: !0,
                    executionCount: (ci.tags[g].executionCount || 0) + 1,
                    stoppedByExitCondition: !1
                });
                const s = i.name;
                if (this._vwoEventsInstance[s] = this._vwoEventsInstance[s] || {}, s && n.fireUniquelyForEveryEvent) return this._vwoEventsInstance[s].event = this._vwoEventsInstance[s].event || i, this._vwoEventsInstance[s].tags = this._vwoEventsInstance[s].tags || {}, this._vwoEventsInstance[s].tags[t.id] = this._vwoEventsInstance[s].tags[t.id] || {}, this._vwoEventsInstance[s].tags[t.id].data = this._vwoEventsInstance[s].tags[t.id].data || [], void this._vwoEventsInstance[s].tags[t.id].data.push(u);
                n.sync || await Promise.resolve();
                const r = null === (c = null === (d = h.fn) || void 0 === d ? void 0 : d.type) || void 0 === c ? void 0 : c.startsWith("vwoWrappedFn");
                i.executingTagTrigger = n.triggerName, await h.fn({
                    data: u,
                    event: i,
                    getters: ci,
                    actions: vi,
                    vwoEvents: r ? null : e
                })
            } else Dt.info(`Maximum occurrence of ${h.occurrence} has reached. Skipping executing tag '${t.id}'`)
        }
        generateTagId(e) {
            return "" + e
        }
        getTagDetails(e) {
            var t;
            return null === (t = ii.plugins[ti.TAG]) || void 0 === t ? void 0 : t.get(e)
        }
    };
    let Fi = 0;
    var ji = new class {
        constructor() {
            this.rulesMap = {}
        }
        initialize(e, t) {
            this.vwoEventsInstance = e, Object.keys(t).forEach((e => {
                this.remove(e), this.add(t[e], e)
            }))
        }
        add(e, t) {
            (Mt(e.triggers) ? e.triggers : [e.triggers]).forEach((i => {
                this.rulesMap[i] ? Dt.debug(`Skipping adding the listeners for trigger ${i} as rulesMap exist`) : this.attachTriggersListeners(this.vwoEventsInstance, i), this.rulesMap[i] = this.rulesMap[i] || [], e.tags.forEach((n => {
                    this.rulesMap[i].push(Object.assign(Object.assign({}, e), {
                        id: t,
                        tags: [n]
                    }))
                })), this.sortPriorityRuleTags(i)
            }))
        }
        remove(e) {
            Dt.debug(`Removing all event listeners of rule '${e}'`), Object.keys(this.rulesMap).forEach((t => {
                this.rulesMap[t] = this.rulesMap[t].filter((t => t.id !== e))
            }))
        }
        removeAll() {
            Object.keys(this.rulesMap).forEach((e => {
                mi.remove(Ui.getTriggerEventName(e)), delete this.rulesMap[e]
            }))
        }
        generateRandomRuleId() {
            return Fi++, "rule_" + Fi
        }
        attachTriggersListeners(e, t) {
            mi.on(Ui.getTriggerEventName(t), (async i => {
                const n = this.rulesMap[t].map((n => n.tags.map((async s => {
                    var r, o, a;
                    const l = Ft(s.shouldConsiderParentExitCondition) ? s.shouldConsiderParentExitCondition : n.shouldConsiderParentExitCondition,
                        d = this.getExitConditions(i, n.exitCondition, s.exitCondition, l),
                        c = await this.shouldExit(i, d),
                        u = Gi.generateTagId(s.id);
                    if (vi.updateTagState(u, {
                            stoppedByExitCondition: c,
                            lastExitConditions: d
                        }), c) Dt.info(`Exit condition(s) met. Not executing tag '${s.id}'`, d);
                    else {
                        const n = null === (o = null === (r = this.vwoEventsInstance.tags) || void 0 === r ? void 0 : r.tags) || void 0 === o ? void 0 : o[s.id];
                        await Gi.execute(Object.assign(Object.assign({}, e), {
                            [bt.EXIT_CONDITIONS]: d,
                            clearData: e.clearData.bind(e)
                        }), s, i, {
                            sync: n.sync || (null === (a = i.props) || void 0 === a ? void 0 : a.fireLinkedTagSync),
                            fireUniquelyForEveryEvent: n.fireUniquelyForEveryEvent,
                            triggerName: t
                        }, e)
                    }
                }))));
                await Promise.all(Array.prototype.concat.apply([], n))
            }))
        }
        sortPriorityRuleTags(e) {
            this.rulesMap[e].sort(((e, t) => {
                const i = e.tags[0],
                    n = t.tags[0];
                return (Ft(i.priority) && -1 !== i.priority ? i.priority : 1 / 0) - (Ft(n.priority) && -1 !== n.priority ? n.priority : 1 / 0)
            }))
        }
        assignExecutionProperty(e) {
            const t = this.rulesMap[e],
                i = {},
                n = ["sync", "fireUniquelyForEveryEvent"];
            for (let e = t.length - 1; e >= 0; e--) {
                const s = t[e].tags[0],
                    r = i[s.id] || Gi.getTagDetails(s.id);
                Object.keys(n).forEach((e => {
                    const t = n[e];
                    s[t] = r[t]
                })), i[s.id] = r
            }
        }
        getExitConditions(e, t, i, n = !0) {
            const s = n ? [...e[bt.EXIT_CONDITIONS] || []] : [];
            return t && s.push(t), i && s.push(i), s
        }
        async shouldExit(e, t = []) {
            const i = t.length;
            for (let n = 0; n < i; ++n) {
                const i = t[n];
                if (await Di.validateTriggerOperand({
                        filters: i
                    }, e)) return !0
            }
            return !1
        }
    };
    class Hi {
        static get(e) {
            const t = document.cookie.split("; ").find((t => t.startsWith(e + "=")));
            return (null == t ? void 0 : t.split("=")[1]) || ""
        }
        static expire(e) {
            document.cookie = e + "=; expires=Thu, 01 Jan 1970 00:00:00 UTC; path=/;"
        }
    }
    class $i {
        static init() {
            $i.actualReferrer = Hi.get("_vwo_referrer"), Hi.expire("_vwo_referrer"), "string" != typeof $i.actualReferrer && ($i.actualReferrer = document.referrer)
        }
        static get() {
            return -1 === window.location.search.search("_vwo_test_ref") ? document.referrer || "" : $i.actualReferrer
        }
    }
    $i.actualReferrer = "";
    class Bi {
        static get() {
            const {
                queryParams: e
            } = Lt.parseUrl(document.URL);
            let t, i;
            const n = $i.get();
            if (/facebook\.com|quora\.com|reddit\.com|imgur\.com|tapiture\.com|disqus\.com|9gag\.com|tumblr\.com|plus\.google|stumbleupon\.com|twitter\.com|linkedin|del\.icio\.us|delicious\.com|technorati|digg\.com| hootsuite|stumbleupon|myspace|bit\.ly|tr\.im|tinyurl|ow\.ly|reddit|m\.facebook\.com|youtube|flickr|pinterest\.com|^https:\/\/t\.co\/|tweetdeck/.test(n)) return "soc";
            Bi.organicReferralSource() && (t = !0);
            const {
                gclid: s,
                utm_medium: r
            } = e;
            if (n && (i = !0), t && s) return "pst";
            if (r) {
                if ("email" === (null == r ? void 0 : r.toString().toLowerCase())) return "eml";
                if (null == r ? void 0 : r.toString().match(new RegExp("^(?:cpc|ppc|cpa|cpm|cpv|cpp)$", "i"))) return "pst"
            } else if (t) return "org";
            return i ? "ref" : "dir"
        }
        static organicReferralSource() {
            for (let e = 0; e < Bi.organicReferrals.length; e++)
                if (-1 !== $i.get().indexOf(Bi.organicReferrals[e].s)) return Bi.organicReferrals[e].i;
            return 0
        }
    }
    Bi.organicReferrals = [{
        s: "search.yahoo.com/",
        p: "p",
        i: 1
    }, {
        s: "www.google.",
        p: "q",
        i: 2
    }, {
        s: "www.bing.com/",
        p: "q",
        i: 3
    }, {
        s: ".ask.com/",
        p: "q",
        i: 4
    }, {
        s: "www.search.com/",
        p: "q",
        i: 5
    }, {
        s: "www.baidu.com/",
        p: "wd",
        i: 6
    }, {
        s: "search.aol.com/",
        p: "q",
        i: 7
    }, {
        s: "duckduckgo.com/",
        p: "q",
        i: 8
    }];
    const Ki = {
        get url() {
            return window._vis_opt_url || window.location.href
        },
        get refUrl() {
            return $i.get()
        },
        get trafficSource() {
            return Bi.get()
        },
        get queryParams() {
            return Lt.parseUrl(window._vis_opt_url || window.location.href).queryParams
        },
        get timeSpent() {
            const e = Date.now();
            return Math.floor((Date.now() - e) / 1e3)
        },
        get scroll() {
            var e, t;
            const {
                scrollY: i,
                innerHeight: n
            } = window, s = 100 * i / ((null === (e = document.body) || void 0 === e ? void 0 : e.offsetHeight) - n);
            return {
                pxTop: i,
                pxBottom: (null === (t = document.body) || void 0 === t ? void 0 : t.offsetHeight) - n - i,
                top: s,
                bottom: 100 - s
            }
        },
        get operatingSystem() {
            var e;
            const {
                visitorDetails: t
            } = Ki;
            if (null === (e = null == t ? void 0 : t.UA) || void 0 === e ? void 0 : e.os) return t.UA.os;
            const {
                appVersion: i
            } = window.navigator;
            return i.includes("Win") ? "windows" : i.includes("Mac") ? "macOS" : i.includes("X11") ? "unix" : i.includes("Linux") ? "linux" : ""
        },
        get browser() {
            var e, t;
            return null === (t = null === (e = Ki.visitorDetails) || void 0 === e ? void 0 : e.UA) || void 0 === t ? void 0 : t.br
        },
        get visitorDetails() {
            var e, t, i, n;
            return (null === (n = null === (i = null === (t = null === (e = window.VWO) || void 0 === e ? void 0 : e._) || void 0 === t ? void 0 : t.allSettings) || void 0 === i ? void 0 : i.dataStore) || void 0 === n ? void 0 : n.plugins) || {}
        },
        get userAgent() {
            return navigator.userAgent
        },
        get deviceType() {
            var e, t;
            return null === (t = null === (e = Ki.visitorDetails) || void 0 === e ? void 0 : e.UA) || void 0 === t ? void 0 : t.dt
        },
        get deviceName() {
            var e, t;
            return null === (t = null === (e = Ki.visitorDetails) || void 0 === e ? void 0 : e.UA) || void 0 === t ? void 0 : t.de
        },
        get day() {
            return (new Date).getDay().toString()
        },
        get hour() {
            return (new Date).getHours()
        },
        get visitorType() {
            var e, t;
            const i = null === (t = null === (e = window.VWO) || void 0 === e ? void 0 : e.data) || void 0 === t ? void 0 : t.vi;
            return (null == i ? void 0 : i.vt) || "new"
        },
        get oldVisitorType() {
            var e, t;
            const i = null === (t = null === (e = window.VWO) || void 0 === e ? void 0 : e.data) || void 0 === t ? void 0 : t.vi;
            return (null == i ? void 0 : i.vt) || "new"
        },
        get ip() {
            var e;
            return null === (e = Ki.visitorDetails) || void 0 === e ? void 0 : e.IP
        },
        get country() {
            var e, t;
            return null === (t = null === (e = Ki.visitorDetails) || void 0 === e ? void 0 : e.GEO) || void 0 === t ? void 0 : t.cn
        },
        get countryCode() {
            var e, t;
            return null === (t = null === (e = Ki.visitorDetails) || void 0 === e ? void 0 : e.GEO) || void 0 === t ? void 0 : t.cc
        },
        get city() {
            var e, t;
            return null === (t = null === (e = Ki.visitorDetails) || void 0 === e ? void 0 : e.GEO) || void 0 === t ? void 0 : t.c
        },
        get region() {
            var e, t;
            return null === (t = null === (e = Ki.visitorDetails) || void 0 === e ? void 0 : e.GEO) || void 0 === t ? void 0 : t.r
        },
        get gpc() {
            var e, t;
            return null === (t = null === (e = Ki.visitorDetails) || void 0 === e ? void 0 : e.GEO) || void 0 === t ? void 0 : t.p
        },
        get loc() {
            var e, t, i;
            const {
                visitorDetails: n
            } = Ki;
            return {
                countryCode: null === (e = null == n ? void 0 : n.GEO) || void 0 === e ? void 0 : e.cc,
                city: null === (t = null == n ? void 0 : n.GEO) || void 0 === t ? void 0 : t.c,
                region: null === (i = null == n ? void 0 : n.GEO) || void 0 === i ? void 0 : i.r
            }
        }
    };
    var zi;
    ! function(e) {
        e.LOCAL_STORAGE = "localStorage", e.LOCAL_STORAGE_SERVICE = "localStorageService"
    }(zi || (zi = {}));
    var Yi = new class extends class {} {
            constructor() {
                super(), this.storageName = zi.LOCAL_STORAGE
            }
            get() {
                return window.localStorage
            }
            getItem(e) {
                return window.localStorage.getItem(e)
            }
            set(e) {
                Object.keys(e).forEach((t => {
                    window.localStorage.setItem(t, e[t])
                }))
            }
            setItem(e, t) {
                window.localStorage.setItem(e, t)
            }
            deleteAll() {
                window.localStorage.clear()
            }
            deleteItem(e) {
                window.localStorage.removeItem(e)
            }
            includes() {
                return 0
            }
        },
        Ji = Object.freeze({
            __proto__: null,
            localStorage: Yi
        });
    class Xi extends yi {
        constructor() {
            super(), this.pluginName = ti.STORAGE, this.storages = {}, this.initialize()
        }
        add(e, t) {
            Dt.debug(`Adding storage plugin '${t.storageName}' in StoragesManager`), this.storages[e] ? Dt.error(Ai.STORAGES.ALREADY_EXISTS, {
                storageName: e
            }) : this.storages[e] = t
        }
        update(e, t) {
            Dt.debug(`Updating storage plugin '${e}' in StoragesManager`), this.storages[e] = t
        }
        get(e) {
            return Dt.debug(`Getting storage plugin '${e}' in StoragesManager`), this.storages[e] ? this.storages[e] : (Dt.error(Ai.STORAGES.NOT_REGISTERED, {
                storageName: e
            }), null)
        }
        remove(e) {
            Dt.debug(`Removing storage plugin '${e}' in StoragesManager`), this.storages[e] ? delete this.storages[e] : Dt.warn(Vi.STORAGES.NO_STORAGE_TO_REMOVE, {
                storageName: e
            })
        }
        removeAll() {
            Dt.debug("Removing all storage plugins in StoragesManager"), this.storages = {}
        }
        initialize() {
            Object.keys(Ji).forEach((e => {
                this.add(e, Ji[e])
            }))
        }
    }
    var qi, Qi, Zi = new class extends yi {
        constructor() {
            super(), this.pluginName = ti.OPERATOR, this.operators = {}
        }
        add(e, t) {
            Dt.debug(`Adding operator '${e}' in OperatorsManager`), this.operators[e] ? Dt.error(Ai.OPERATORS.ALREADY_EXISTS, {
                operatorName: e
            }) : this.operators[e] = t
        }
        update(e, t) {
            Dt.debug(`Updating operator '${e}' in OperatorsManager`), this.operators[e] = t
        }
        get(e) {
            return Dt.debug(`Getting operator '${e}' in OperatorsManager`), this.operators[e] ? this.operators[e] : (Dt.error(Ai.OPERATORS.NOT_REGISTERED, {
                operatorName: e
            }), null)
        }
        remove(e) {
            Dt.debug(`Removing operator '${e}' in OperatorsManager`), this.operators[e] ? delete this.operators[e] : Dt.warn(Vi.OPERATORS.NO_OPERATOR_TO_REMOVE, {
                operatorName: e
            })
        }
        removeAll() {
            Dt.debug("Removing all operators in OperatorsManager"), this.operators = {}
        }
        initialize(e) {
            Object.assign(this.operators, e)
        }
    };
    ! function(e) {
        e.EQUAL = "eq", e.NOT_EQUAL = "neq", e.EQUAL_CASE_SENSITIVE = "eqs", e.NOT_EQUAL_CASE_SENSITIVE = "neqs", e.REGEX = "reg", e.REGEX_CASE_SENSITIVE = "regs", e.CONTAINS = "cn", e.NOT_CONTAINS = "ncn", e.BLANK = "bl", e.NOT_BLANK = "nbl", e.GREATER_THAN = "gt", e.LESS_THAN = "lt", e.GREATER_THAN_EQUAL = "gte", e.LESS_THAN_EQUAL = "lte", e.IN = "in", e.NOT_IN = "nin", e.EXEC = "exec", e.SELECTOR = "sel", e.IN_LOCATION = "inloc", e.NOT_IN_LOCATION = "ninloc", e.URL_REGEX = "urlReg", e.NOT_URL_REGEX = "nUrlReg", e.RANGE_COMPARISON = "rg", e.PAGE_CONFIG_EVALUATION = "pgc"
    }(qi || (qi = {})),
    function(e) {
        e.PAGE = "PAGE", e.EVENT = "EVENT", e.JS_VARIABLE = "JS_VARIABLE"
    }(Qi || (Qi = {}));
    class en extends yi {
        constructor(e, {
            globals: t = {}
        } = {}) {
            super(), this.pluginName = ti.EVENT, this.vwoEventsInstance = e, this.config = {
                domEventsDebounceTime: t.domEventsDebounceTime
            }, this.events = {}, this.initialize()
        }
        add(...e) {
            var t, i;
            let n, s, r, o;
            Wt(e[0]) ? ([{
                eventName: n
            }] = e, s = e[0].on.bind(e[0]), r = null === (t = e[0].off) || void 0 === t ? void 0 : t.bind(e[0]), o = null === (i = e[0].eventConditionsUpdate) || void 0 === i ? void 0 : i.bind(e[0])) : [n, s, r, o] = e, Dt.debug(`Adding event listener '${n}' in EventsManager`), this.events[n] ? Dt.error(Ai.EVENTS.ALREADY_EXISTS, {
                eventName: n
            }) : (this.events[n] = {
                on: s,
                off: r,
                eventConditionsUpdate: o
            }, s((e => this.triggerCustomEvent(n, e)), {
                vwoEvents: this.vwoEventsInstance
            }))
        }
        updateCustomEvents(...e) {
            var t;
            let i, n, s;
            Wt(e[0]) ? ([{
                eventName: i
            }] = e, n = e[0].on.bind(e[0]), s = null === (t = e[0].off) || void 0 === t ? void 0 : t.bind(e[0])) : [i, n, s] = e, Dt.debug(`Updating event listener '${i}' in EventsManager`), this.events[i] = {
                on: n,
                off: s
            }, n((e => this.triggerCustomEvent(i, e)), {
                vwoEvents: this.vwoEventsInstance
            })
        }
        update(e) {
            Object.keys(e).forEach((t => {
                this.updateCustomEvents(t, e[t].fn)
            }))
        }
        get(e) {
            return Dt.debug(`Getting event listener '${e}' definition from EventsManager`), this.events[e] ? this.events[e] : (Dt.error(Ai.EVENTS.NOT_REGISTERED, {
                eventName: e
            }), null)
        }
        remove(e) {
            var t, i;
            Dt.debug(`Removing event listener '${e}' from EventsManager`), this.events[e] ? (null === (i = (t = this.events[e]).off) || void 0 === i || i.call(t), delete this.events[e], mi.remove(e)) : Dt.warn(Vi.EVENTS.NO_EVENT_TO_REMOVE, {
                eventName: e
            })
        }
        removeAll() {
            Dt.debug("Removing all event listeners from EventsManager"), Object.keys(this.events).forEach((e => {
                var t, i;
                null === (i = (t = this.events[e]).off) || void 0 === i || i.call(t)
            })), mi.removeAll(), this.events = {}
        }
        addDomEvent(e, t) {
            var i, n, s, r;
            if (null === (r = null === (s = null === (n = null === (i = window.VWO) || void 0 === i ? void 0 : i.modules) || void 0 === n ? void 0 : n.utils) || void 0 === s ? void 0 : s.libUtils) || void 0 === r ? void 0 : r.isBot2()) return;
            const {
                preDefinedEvents: o
            } = this.vwoEventsInstance;
            !this.events[e] && e.toLowerCase().includes(Ct.DOM + "_") && (e === Ct.DOM + "_click" ? this.add(new o.ClickDomEvent(t)) : e === Ct.DOM + "_submit" ? this.add(new o.SubmitDomEvent(t)) : this.add(new o.GenericDomEvent(e, t, this.config.domEventsDebounceTime)))
        }
        updateConditions(e, t) {
            if (this.events[e]) {
                if (null !== t[0]) {
                    const {
                        eventConditionsUpdate: i
                    } = this.events[e];
                    i && Array.isArray(t) && i(t)
                }
            } else this.addDomEvent(e, t)
        }
        triggerCustomEvent(e, t) {
            mi.trigger(e, t)
        }
        initialize() {
            const {
                preDefinedEvents: e
            } = this.vwoEventsInstance;
            Object.keys(e).forEach((t => {
                t.endsWith("DomEvent") || this.add(new e[t])
            }))
        }
    }
    const tn = {
        [qi.EQUAL]: (e, t) => String(e).toLowerCase() === String(t).toLowerCase(),
        [qi.NOT_EQUAL]: (e, t) => !tn[qi.EQUAL](e, t),
        [qi.EQUAL_CASE_SENSITIVE]: (e, t) => String(e) === String(t),
        [qi.NOT_EQUAL_CASE_SENSITIVE]: (e, t) => !tn[qi.EQUAL_CASE_SENSITIVE](e, t),
        [qi.REGEX](e, t) {
            try {
                return new RegExp(t, "i").test(String(e))
            } catch (e) {
                return !1
            }
        },
        [qi.URL_REGEX](e, t, i) {
            const n = null == i ? void 0 : i.jsLibUtils;
            return n ? n.verifyUrl(e, t, null, null == i ? void 0 : i.pageUrl) : tn[qi.REGEX](e, t)
        },
        [qi.NOT_URL_REGEX]: (e, t, i) => !tn[qi.URL_REGEX](e, t, i),
        [qi.REGEX_CASE_SENSITIVE](e, t) {
            try {
                return new RegExp(t).test(String(e))
            } catch (e) {
                return !1
            }
        },
        [qi.CONTAINS]: (e, t) => String(e).toLowerCase().includes(String(t).toLowerCase()),
        [qi.NOT_CONTAINS]: (e, t) => !tn[qi.CONTAINS](e, t),
        [qi.BLANK]: e => !e,
        [qi.NOT_BLANK]: e => !tn[qi.BLANK](e),
        [qi.GREATER_THAN](e, t) {
            if (!Ft(e) || !Ft(t)) return !1;
            const i = +e,
                n = +t;
            return jt(i) && jt(n) && i > n
        },
        [qi.GREATER_THAN_EQUAL](e, t) {
            if (!Ft(e) || !Ft(t)) return !1;
            const i = +e,
                n = +t;
            return jt(i) && jt(n) && i >= n
        },
        [qi.LESS_THAN](e, t) {
            if (!Ft(e) || !Ft(t)) return !1;
            const i = +e,
                n = +t;
            return jt(i) && jt(n) && i < n
        },
        [qi.LESS_THAN_EQUAL](e, t) {
            if (!Ft(e) || !Ft(t)) return !1;
            const i = +e,
                n = +t;
            return jt(i) && jt(n) && i <= n
        },
        [qi.NOT_IN_LOCATION](e, t) {
            let i = !1;
            if (!t || 0 === t.length) return !1;
            for (let n = 0; n < t.length; n++) {
                const s = t[n];
                if (s === e.countryCode || s === `${e.countryCode}-${e.region}` || s === `${e.countryCode}-${e.region}-${e.city}`) {
                    i = !1;
                    break
                }
                i = !0
            }
            return i
        },
        [qi.IN_LOCATION](e, t) {
            let i = !1;
            if (!t || 0 === t.length) return !1;
            for (let n = 0; n < t.length; n++) {
                const s = t[n];
                if (s === e.countryCode || s === `${e.countryCode}-${e.region}` || s === `${e.countryCode}-${e.region}-${e.city}`) {
                    i = !0;
                    break
                }
            }
            return i
        },
        [qi.IN]: (e, t) => t.map((e => String(e).toLowerCase())).includes(String(e).toLowerCase()),
        [qi.NOT_IN]: (e, t) => !tn[qi.IN](e, t),
        [qi.RANGE_COMPARISON](e, t) {
            try {
                let i = JSON.parse;
                try {
                    i = window.VWO._.native.JSON.parse || JSON.parse
                } catch (e) {}
                const n = i(e),
                    s = t.split("'")[1].split("-"),
                    r = s[0],
                    o = s[1];
                return tn[qi.GREATER_THAN_EQUAL](n[0], parseInt(r, 10)) && tn[qi.LESS_THAN_EQUAL](n[0], parseInt(o, 10))
            } catch (e) {
                return Dt.info("RANGE OPERATOR ERROR: " + (e && e.stack)), !1
            }
        },
        [qi.PAGE_CONFIG_EVALUATION]: (e, t) => di.validatePage(t, !1, e).didMatch
    };
    var nn = Object.assign(tn, {
        sel: () => !1,
        [qi.EXEC](e, t, i = {
            triggerName: ""
        }) {
            if (!e) return !1;
            let n;
            n = t && "object" == typeof t ? t.triggerName : i.triggerName;
            for (let i = 0; i < e.length; i++) {
                const s = e[i];
                if (!s) break;
                window.VWO._.pageNo = window.VWO._.pageNo || 0;
                const {
                    pageNo: r
                } = window.VWO._, o = () => {
                    r === window.VWO._.pageNo && mi.trigger(Ui.getTriggerEventName(n))
                };
                try {
                    s(o, window.vwo_$, "string" == typeof t ? {
                        sel: t,
                        triggerName: n
                    } : void 0)
                } catch (e) {
                    Dt.warn("Issue with custom trigger " + (null == s ? void 0 : s.toString()))
                }
            }
            return !1
        }
    });
    Zi.initialize(nn);
    const sn = {};
    class rn extends yi {
        constructor() {
            super(), this.pluginName = ti.EVENT_PROPS, this.eventsProps = {}, this.initialize()
        }
        add(e, t, i) {
            Dt.debug(`Adding computed prop '${t}' of event '${e}'.`), this.eventsProps[e] = this.eventsProps[e] || {}, this.eventsProps[e][t] ? Dt.error(Ai.EVENT_PROP.ALREADY_EXISTS, {
                propName: t,
                eventName: e
            }) : this.eventsProps[e][t] = i
        }
        update(e, t, i) {
            Dt.debug(`Updating computed prop '${t}' of event '${e}'.`), this.eventsProps[e] = this.eventsProps[e] || {}, this.eventsProps[e][t] = i
        }
        get(e, t) {
            var i;
            return Dt.debug(`Getting '${t||"all"}' computed prop of event '${e}'.`), t ? (null === (i = this.eventsProps[e]) || void 0 === i ? void 0 : i[t]) ? this.eventsProps[e][t] : (Dt.error(Ai.EVENT_PROP.NOT_REGISTERED, {
                eventName: e,
                propName: t
            }), null) : this.eventsProps[e] || {}
        }
        remove(e, t) {
            var i;
            Dt.debug(`Removing '${t||"all"}' computed prop of event '${e}'.`), t ? (null === (i = this.eventsProps[e]) || void 0 === i ? void 0 : i[t]) ? delete this.eventsProps[e][t] : Dt.warn(Vi.EVENT_PROP.NO_EVENT_PROP_TO_REMOVE, {
                eventName: e,
                propName: t
            }) : this.eventsProps[e] = {}
        }
        removeAll() {
            Dt.debug("Removing all computed props of all events"), this.eventsProps = {}
        }
        initialize() {
            Object.keys(sn).forEach((e => {
                Object.keys(sn[e]).forEach((t => {
                    this.add(e, t, sn[e][t])
                }))
            }))
        }
    }
    var on = Object.freeze({
        __proto__: null,
        A: class extends Pi {
            async fn(e, ...t) {
                const i = await Promise.all(t.map((t => t(e))));
                let n = 0;
                for (let e = i.length - 1; e >= 0; --e) {
                    const t = i[e];
                    if (!t[0]) return [!1, 0];
                    n = t[1] > n ? t[1] : n
                }
                return [!0, n]
            }
            toJSON() {
                return "a"
            }
        },
        O: class extends Pi {
            async fn(e, ...t) {
                const i = await Promise.all(t.map((t => t(e))));
                let n = 1 / 0,
                    s = !1;
                for (let e = i.length - 1; e >= 0; --e) {
                    const t = i[e];
                    t[0] && (s = !0, n = t[1] < n ? t[1] : n)
                }
                return [s, s ? n : 0]
            }
            toJSON() {
                return "o"
            }
        },
        TS: class extends Pi {
            constructor({
                data: e
            }, {
                trigger: t,
                triggerName: i
            }) {
                super(), [this.op] = e;
                const n = e[1];
                this.threshold = "INF" === n ? 1 / 0 : 1e3 * n, this.validateFromStateAndDispatchTriggerEvent = () => Ui.validateAndDispatchTriggerEvent(i, t, {}, "", null, null, !0), this.isTimeoutCompleted = !1
            }
            lt(e, t) {
                if (e[0] && t[0]) {
                    this.previousLHS = e[1];
                    const i = t[1] - e[1];
                    if (i >= 0 && i <= this.threshold) return [!0, t[1]]
                }
                return [!1, 0]
            }
            gtn(e, t) {
                if (!e[0]) return [!1, 0];
                if (this.isTimeoutCompleted) return [!0, e[1] + this.threshold];
                if (this.threshold === 1 / 0) return [!1, 0];
                if (t[1] > e[1]) return this.timeout && (clearTimeout(this.timeout), this.timeout = void 0), this.lt(e, t)[0] ? [!1, 0] : [!0, e[1] + this.threshold];
                if (!this.timeout || this.previousLHS !== e[1]) {
                    this.previousLHS = e[1], this.isTimeoutCompleted = !1;
                    const t = e[1] + this.threshold - Date.now();
                    this.timeout && clearTimeout(this.timeout), this.timeout = setTimeout((() => {
                        this.isTimeoutCompleted = !0, this.validateFromStateAndDispatchTriggerEvent()
                    }), t > 0 ? t : 0)
                }
                return [!1, 0]
            }
            async fn(e, t, i) {
                const n = await t({
                        doNotUseCache: e.doNotUseCache || !this._persistedState
                    }),
                    s = i ? await i({
                        doNotUseCache: e.doNotUseCache || !this._persistedState
                    }) : [!1, 0];
                if (n[0] && !n[1]) return [!0, 0];
                if (!e.doNotUseCache && this._persistedState) return this._persistedState;
                const r = this[this.op](n, s);
                return r[0] && (this._persistedState = r), r
            }
            get persistedData() {
                var e;
                return null !== (e = JSON.stringify(this._persistedState)) && void 0 !== e ? e : ""
            }
            toJSON() {
                return {
                    op: "ts",
                    data: [this.op, this.threshold / 1e3]
                }
            }
            clearData() {
                clearTimeout(this.timeout), this.timeout = void 0, this.isTimeoutCompleted = !1, this._persistedState = null
            }
        }
    });
    class an extends yi {
        constructor() {
            super(), this.pluginName = ti.CONDITION_LEVEL_OPERATOR, this.operators = {}, this.initialize()
        }
        add(e, t) {
            Dt.debug(`Adding Condition Level operator '${e}' in CLOperatorsManager`), this.operators[e] ? Dt.error(Ai.OPERATORS.ALREADY_EXISTS, {
                operatorName: e
            }) : this.operators[e] = {
                definition: t,
                instances: []
            }
        }
        update(e, t) {
            Dt.debug(`Updating operator '${e}' in CLOperatorsManager`), this.operators[e].definition = t
        }
        get(e, t, {
            trigger: i,
            triggerName: n
        }) {
            Dt.debug(`Getting operator '${e}' in CLOperatorsManager`);
            const s = this.operators[e.toUpperCase()];
            if (!s) return Dt.error(Ai.OPERATORS.NOT_REGISTERED, {
                operatorName: e
            }), null;
            const r = new s.definition(t, {
                trigger: i,
                triggerName: n
            });
            return s.instances.push(r), r
        }
        remove(e) {
            Dt.debug(`Removing operator '${e}' in CLOperatorsManager`), this.operators[e] ? delete this.operators[e] : Dt.warn(Vi.OPERATORS.NO_OPERATOR_TO_REMOVE, {
                operatorName: e
            })
        }
        removeAll() {
            Dt.debug("Removing all operators in CLOperatorsManager"), this.operators = {}
        }
        initialize() {
            Object.keys(on).forEach((e => {
                this.add(e, on[e])
            }))
        }
        clearData() {
            Object.keys(this.operators).forEach((e => {
                this.operators[e].instances.forEach((e => {
                    e.clearData()
                }))
            }))
        }
    }
    var ln, dn, cn = new class {
        execute(e, t) {
            this.initializeStore(t.dataStore, t.storages, t.props), this.initializeOperators(t.operators), this.initializeFormulas(), this.initializeTags(t.tags), this.initializeEventsProps(t.eventsProps), this.initializeEvents(e, t.events, t.globals), this.initializeTriggers(t.triggers), this.initializePages(t.pages, t.pagesEval), this.initializeRules(e, t.rules)
        }
        clear() {
            ii.unregisterAll(), Ui.removeAll(), ji.removeAll(), mi.removeAll(), vi.clearAll()
        }
        initializeOperators(e) {
            Dt.debug("Initializing Operators");
            const t = ii.plugins[ti.OPERATOR] || Zi;
            Object.keys(e).forEach((i => {
                t.update(i, e[i])
            })), ii.register(t)
        }
        initializeFormulas() {
            Dt.debug("Initializing Formulas"), ii.register(ii.plugins[ti.FORMULA] || new bi)
        }
        initializeStore(e, t, i) {
            Dt.debug("Initializing Store");
            const n = ii.plugins[ti.STORAGE] || new Xi;
            Object.keys(t).forEach((e => {
                n.update(e, t[e]), hi.setStoragePlugin(n.get(e))
            })), vi.addValues(e, Yt.SETTINGS), vi.set("storages", n, Yt.ROOT), class {
                static initialize(e) {
                    vi.addValues(e, Yt.ROOT), vi.addValues(Ki, Yt.ROOT)
                }
            }.initialize(i), ii.register(n)
        }
        initializeEventsProps(e) {
            Dt.debug("Initializing Events computed props");
            const t = ii.plugins[ti.EVENT_PROPS] || new rn;
            Object.keys(e).forEach((i => {
                Object.keys(e[i]).forEach((n => {
                    t.update(i, n, e[i][n])
                }))
            })), ii.register(t)
        }
        initializeEvents(e, t, i) {
            Dt.debug("Initializing Events");
            const n = ii.plugins[ti.EVENT] || new en(e, {
                globals: i
            });
            n.update(t), ii.register(n)
        }
        initializeTriggers(e) {
            Dt.debug("Initializing Triggers"), Ui.initialize(e)
        }
        initializePages(e, t) {
            Dt.debug("Initializing Pages"), di.add(e, t)
        }
        initializeTags(e) {
            Dt.debug("Initializing Tags");
            const t = ii.plugins[ti.TAG] || new ki;
            Object.keys(e).forEach((i => {
                const n = e[i];
                t.update(i, n.fn, {
                    occurrence: n.occurrence,
                    scope: n.scope,
                    sync: n.sync,
                    fireUniquelyForEveryEvent: n.fireUniquelyForEveryEvent
                })
            })), ii.register(t)
        }
        initializeRules(e, t) {
            Dt.debug("Initializing Rules", t), ji.initialize(e, t)
        }
        initializeCLOperators() {
            Dt.debug("Initializing CL_Operators");
            const e = ii.plugins[ti.CONDITION_LEVEL_OPERATOR] || new an;
            ii.register(e)
        }
    };
    ! function(e) {
        e.OR = "OR"
    }(ln || (ln = {})),
    function(e) {
        e.ALL = "*"
    }(dn || (dn = {}));
    class un {
        static or(...e) {
            return e._validationType = ln.OR, e
        }
        static validateJSON(e, t, i = "settings") {
            try {
                if (e) {
                    const n = e._validationType === ln.OR && Mt(e) ? e : [e],
                        s = e._validationType === ln.OR && Mt(e) ? e.find((e => zt(e) === zt(t))) : e;
                    if (!un.isSchemaMatching(n, t)) return un.logger.info(`Settings schema validation failed for '${i}'.`, {
                        expectedSchema: un.getStringifiedSchemaValues(e),
                        actualValue: t
                    }), un.getNewInitializedValue(e);
                    if (Wt(s)) Object.keys(t).forEach((e => {
                        t[e] = un.validateJSON(s[e] || s[dn.ALL], t[e], `${i}.${e}`)
                    }));
                    else if (Mt(s)) return t.map(((e, t) => un.validateJSON(s[0] === un.RECURSION ? n : s[0], e, `${i}.${t}`)))
                }
                return t
            } catch (e) {
                return un.logger.error("Got exception while validating settings schema", {
                    err: e
                }), t
            }
        }
        static isSchemaMatching(e, t) {
            return e.some((e => zt(t) === zt(e) || zt(t) === (null == e ? void 0 : e.name)))
        }
        static getNewInitializedValue(e) {
            if (Bt(e)) return new e;
            let t = e;
            return e._validationType === ln.OR && Mt(e) && (t = e.find((e => un.isArraySchema(e) || un.isObjectSchema(e))) || e[0]), un.isObjectSchema(t) ? {} : un.isArraySchema(t) ? [] : void 0
        }
        static isArraySchema(e) {
            return "Array" === e.name || Mt(e)
        }
        static isObjectSchema(e) {
            return "Object" === e.name || Wt(e)
        }
        static getStringifiedSchemaValues(e) {
            return Bt(e) ? e.name : Mt(e) ? e.map((e => un.getStringifiedSchemaValues(e))) : Wt(e) ? Object.keys(e).reduce(((t, i) => Object.assign(Object.assign({}, t), {
                [i]: un.getStringifiedSchemaValues(e[i])
            })), {}) : e
        }
    }
    un.logger = new xt("warn"), un.RECURSION = "r";
    class gn {
        constructor(e, ...t) {
            var i;
            this.vwoEventsInstance = e, this.currentSettings = {}, this.baseSettings = {
                globals: {},
                dataStore: {},
                props: {},
                operators: {},
                eventsProps: {},
                events: {},
                triggers: {},
                pages: {
                    ec: [],
                    pc: []
                },
                pagesEval: {
                    ec: [],
                    pc: []
                },
                tags: {},
                rules: {},
                storages: {},
                jsLibUtils: {}
            }, this.replace(...t), si.set("jsLibUtils", null === (i = this.currentSettings) || void 0 === i ? void 0 : i.jsLibUtils)
        }
        add(...e) {
            cn.initializeCLOperators();
            const t = this.preProcessSettings(e);
            return this.currentSettings.pagesEval = {}, this.currentSettings = Qt.mergeNestedObjects(this.currentSettings, ...t), this.initialize(), this.currentSettings
        }
        replace(...e) {
            return this.resetSettings(), this.add(...e)
        }
        remove() {
            this.resetSettings(), cn.clear()
        }
        initialize() {
            cn.execute(this.vwoEventsInstance, this.currentSettings)
        }
        resetSettings() {
            this.currentSettings = Object.assign({}, this.baseSettings)
        }
        preProcessSettings(e) {
            let t = this.preProcessRules(e);
            for (let e = this.preProcessSettings.length - 1; e >= 0; --e) {
                const i = t[e];
                if (i.triggers) {
                    const e = {};
                    Object.keys(i.triggers).forEach((t => {
                        e[t] = this.preProcessTriggers(t, i.triggers[t])
                    })), i.triggers = e
                }
            }
            return t = this.preProcessCampaigns(t), this.getValidatedSettings(t)
        }
        getValidatedSettings(e) {
            const t = {
                    id: un.or(String, Number),
                    type: String,
                    filters: [un.or(String, Array)],
                    args: Array,
                    fn: Function
                },
                i = [{
                    dataStore: Object,
                    props: Object,
                    operators: {
                        "*": Function
                    },
                    eventsProps: {
                        "*": {
                            "*": Function
                        }
                    },
                    events: {
                        "*": {
                            fn: Function
                        }
                    },
                    triggers: {
                        "*": {
                            cnds: [un.or(String, Boolean, null, Array, [un.RECURSION], {
                                id: un.or(String, Number),
                                event: String,
                                filters: [un.or(String, Array)],
                                formula: [t],
                                op: String,
                                data: [un.or(String, Number)]
                            })],
                            formula: [t]
                        }
                    },
                    tags: {
                        "*": {
                            fn: un.or(Function, Object.getPrototypeOf((async function() {})).constructor),
                            occurrence: Number,
                            scope: String
                        }
                    },
                    rules: {
                        "*": {
                            triggers: un.or(String, [String]),
                            tags: [{
                                id: String,
                                priority: Number,
                                exitCondition: [un.or(String, Array)],
                                shouldConsiderParentExitCondition: Boolean
                            }],
                            exitCondition: [un.or(String, Array)],
                            shouldConsiderParentExitCondition: Boolean
                        }
                    },
                    storages: {
                        "*": Object
                    }
                }];
            return un.validateJSON(i, e)
        }
        preProcessRules(e) {
            const t = {},
                i = this;
            return Object.keys(this.currentSettings.rules).forEach((e => {
                const n = this.currentSettings.rules[e],
                    s = [];
                n.tags.forEach((e => {
                    s.push(JSON.stringify(i.sortTag(e)))
                })), t[n.triggers[0]] = s
            })), e.map((e => {
                let {
                    rules: n
                } = e;
                if (n = n || [], Mt(n)) {
                    const e = {};
                    n.forEach((n => {
                        let s = !1;
                        const r = [],
                            o = ji.generateRandomRuleId(),
                            a = {
                                triggers: n.triggers,
                                tags: []
                            };
                        n.tags.forEach((e => {
                            const o = n.triggers[0],
                                a = JSON.stringify(i.sortTag(e));
                            t[o] = t[o] || [], t[o].includes(a) || (s = !0, t[o].push(a), r.push(e))
                        })), s && (a.tags = r, e[o] = a)
                    })), n = e
                }
                return Object.assign(Object.assign({}, e), {
                    rules: n
                })
            }))
        }
        sortTag(e) {
            return Object.keys(e).sort().reduce(((t, i) => (t[i] = e[i], t)), {})
        }
        preProcessCampaigns(e) {
            return e.map((e => {
                if (Wt(e.triggers) && Ft(e.triggers) && Wt(e.dataStore) && Ft(e.dataStore)) {
                    const {
                        triggers: t
                    } = e, {
                        campaigns: i
                    } = e.dataStore;
                    Object.keys(i).forEach((e => {
                        const n = i[e].triggers[0];
                        Wt(t[n]) && Ft(t[n]) && (t[n].occurrence = 1)
                    }))
                }
                return e
            }))
        }
        preProcessTriggers(e, t) {
            var i;
            if (t && t.cnds && t.cnds.length >= 0) {
                const n = (i, s, r) => {
                    if (Mt(i)) i.forEach(n);
                    else if (i) {
                        if (Ht(i) || Ft(i.op)) {
                            const n = Ht(i) ? i : i.op;
                            r[s] = ii.plugins[ti.CONDITION_LEVEL_OPERATOR].get(n, i, {
                                trigger: t,
                                triggerName: e
                            })
                        }
                        Ft(i.persistState) || "vwo_postInit" === i.event && (i.persistState = !0)
                    }
                };
                null === (i = t.cnds) || void 0 === i || i.forEach(n)
            }
            return t
        }
    }
    class vn {
        static getTriggersConditionInfo(e, t) {
            const i = t => Mt(t) ? t.map(i) : Li(t) ? Object.assign(Object.assign({}, t), {
                hasEventFired: Di.hasEventOccurred(e, t.event),
                arePartialConditionsMet: Di.hasConditionMetBefore(e, t.id)
            }) : xi(t) ? `${JSON.stringify(t)} -> ${t.persistedState}` : "string" == typeof t ? t : JSON.stringify(t);
            return t.map(i)
        }
        static tag(e, t) {
            var i;
            const n = Gi.generateTagId(t),
                s = ci.tags[n],
                r = null === (i = ii.plugins[ti.TAG]) || void 0 === i ? void 0 : i.get(t),
                o = Object.values(e.currentSettings.rules).filter((e => e.tags.some((e => e.id === t)))).reduce(((e, t) => {
                    const i = Mt(t.triggers) ? t.triggers : [t.triggers];
                    return e.push(...i), e
                }), []).reduce(((t, i) => {
                    const n = ci.triggers[i];
                    return Object.assign(Object.assign({}, t), {
                        [i]: {
                            areConditionsMet: n.state,
                            cndsState: this.getTriggersConditionInfo(i, e.currentSettings.triggers[i].cnds)
                        }
                    })
                }), {});
            return {
                hasExecuted: s.hasExecuted || !1,
                executionCount: s.executionCount || 0,
                fn: r.fn,
                occurrence: r.occurrence || null,
                scope: r.scope || null,
                stoppedByExitCondition: s.stoppedByExitCondition,
                lastExitConditions: s.lastExitConditions,
                dependentTriggers: o
            }
        }
        static trigger(e, t) {
            return {
                areConditionsMet: ci.triggers[t].state,
                cndsState: this.getTriggersConditionInfo(t, e.currentSettings.triggers[t].cnds)
            }
        }
        static page(e, t, i) {
            const n = di.validatePage(e, t, i, !0);
            let s = t ? "pageConfig" : "experimentConfig";
            if (n) {
                switch (n.reason) {
                    case oi.EXCLUDE_PASSED:
                        s = "Url matches the excludes of " + s;
                        break;
                    case oi.INCLUDE_FAILED:
                        s = "Url does not match the includes of " + s;
                        break;
                    case oi.INCLUDE_PASSED:
                        s = "Url matches the includes of " + s
                }
                return delete n.reason, Object.assign(Object.assign({}, n), {
                    brief: s
                })
            }
        }
    }
    const hn = function() {};
    class pn {
        constructor(e, t) {
            this.toJSON = () => {
                const e = Object.assign({}, this);
                return delete e.preDefinedEvents, e
            }, this.store = {
                state: ei,
                getters: ci,
                actions: vi
            }, this.eventBus = mi, this.preDefinedEvents = null == t ? void 0 : t.preDefinedEvents, this.debug = {
                tag: e => vn.tag(this.settings, e),
                trigger: e => vn.trigger(this.settings, e),
                page: (e, t, i) => vn.page(e, t, i)
            }, this.rules = ji, this.triggers = Ui, this.pageGroup = di, Object.assign(this, this.getEventMethods()), $i.init(), this.activate(e)
        }
        get version() {
            return "1.1.22"
        }
        add(...e) {
            this.settings.add(...e), mi.trigger(Nt.POST_INIT)
        }
        replace(...e) {
            this.settings.replace(...e), mi.trigger(Nt.POST_INIT)
        }
        on(e, t, {
            before: i,
            after: n
        } = {}) {
            if (Ht(e)) return mi.on(e, t, {
                before: i,
                after: n
            });
            const s = Date.now() * Math.floor(1e3 * Math.random());
            this.tags.add(s, t), this.rules.add({
                triggers: [s.toString()],
                tags: [{
                    id: s.toString()
                }]
            }, this.rules.generateRandomRuleId());
            const r = this.settings.preProcessTriggers(s.toString(), e);
            return Ui.add(s.toString(), r), s
        }
        async validateTrigger(e, t = {}, i, n) {
            return Dt.info("Validating Trigger conditions through API", {
                trigger: e,
                event: t
            }), (await Di.validateTrigger(e, t, i, n)).isValid
        }
        validateTriggerFromHistory(e, t) {
            return Ui.validateTriggerFromHistory(e, t)
        }
        validateAndDispatchTriggerEvent(e, t, i, n, s, r, o = !1) {
            return Ui.validateAndDispatchTriggerEvent(e, t, fi(i), n, s, r, o)
        }
        destroy() {
            this.settings.remove()
        }
        async isValid(e, t, i) {
            await Di.isValid(e, t, fi({
                isCustomEvent: !0,
                fromLocalStorage: !0
            }), i)
        }
        activate(e) {
            Dt.info("Activating Phoenix!"), this.settings = new gn(this, ...e), this.initializeValues(), mi.trigger(Nt.POST_INIT)
        }
        getEventMethods() {
            return {
                on: this.on,
                off: mi.off.bind(mi),
                removeEvent: mi.remove.bind(mi),
                async trigger(e, t = {}, i) {
                    try {
                        Object.defineProperty(t, bt.EXIT_CONDITIONS, {
                            value: this[bt.EXIT_CONDITIONS],
                            enumerable: !1,
                            writable: !0
                        });
                        try {
                            if (await mi.trigger(e, t), e && Gi.getVwoInstanceObject() && Gi.getVwoInstanceObject()[e]) {
                                const t = Gi.getVwoInstanceObject()[e];
                                Gi.flushTagData(e), await Gi.executeTagUniquelyForEveryEvent(t)
                            }
                        } catch (e) {}(Bt(i) || Kt(i)) && await i(t)
                    } catch (e) {
                        window.VWO._.customError && window.VWO._.customError({
                            msg: e.stack || e.message || "Something wrong",
                            url: "Trigger.ts",
                            lineno: 252,
                            colno: 5,
                            source: JSON.stringify({
                                event: t,
                                opt_url: window._vis_opt_url,
                                errMsg: e.message,
                                errStk: e.stack
                            })
                        })
                    }
                },
                getEvent: mi.event.bind(mi)
            }
        }
        initializeValues() {
            this.operators = ii.plugins[ti.OPERATOR], this.formulas = ii.plugins[ti.FORMULA], this.events = ii.plugins[ti.EVENT], this.storages = ii.plugins[ti.STORAGE], this.tags = ii.plugins[ti.TAG], this.eventsProps = ii.plugins[ti.EVENT_PROPS]
        }
        deactivate() {
            this.trigger = hn, this.add = hn
        }
        clearData() {
            ii.clearData()
        }
    }
    pn.version = "1.1.22";
    class wn {
        constructor() {
            this.storageLookUpKey = "_vwo_store_content"
        }
        otherSide(...e) {
            return e[0] = "VWO.modules.vwoUtils.contentSync." + e[0], e[2] && (e[2] = {
                captureGroups: e[2]
            }), window.fetcher.getValue(...e)
        }
    }
    const En = function(...e) {
        window.fetcher.getValue("VWO._.triggerEvent", e)
    };
    class fn {}

    function _n(e) {
        var t = [];
        for (var i in e) e.hasOwnProperty(i) && t.push(i);
        return t
    }

    function mn(e, t) {
        window.fetcher.getValue("VWO.modules.vwoUtils.urlUtils.customEvent", [e, t])
    }
    const On = Object.keys;

    function Sn(e, t) {
        if (!(e instanceof Array)) return -1;
        for (var i = 0; i < e.length; i++)
            if (t === e[i]) return i;
        return -1
    }

    function yn(e, t, i) {
        var n, s, r, o = !1;
        return -1 === t || i ? (s = requestAnimationFrame, r = cancelAnimationFrame) : (s = setTimeout, r = clearTimeout),
            function(...i) {
                o && (r(n), n = null), n = s((function() {
                    e.apply(this, i)
                }), t), o = !0
            }
    }

    function Tn(e, t) {
        return e.length > t ? e.slice(0, t - 1) + "..." : e
    }

    function In(e) {
        return null !== e && "object" == typeof e && !Array.isArray(e)
    }

    function Cn() {}
    try {
        Cn.prototype = Object.create(Array.prototype), Object.defineProperty(Cn.prototype, "clear", {
            value: void 0,
            writable: !0,
            enumerable: !1
        })
    } catch (Z) {}
    const Nn = In(window._vwoCc) ? window._vwoCc : {},
        An = e => (Nn.SPA_SPLIT = Nn.SPA_SPLIT || {}, !(!Nn.SPA_SPLIT[e] && !Nn.SPA_SPLIT["*"])),
        Vn = (() => {
            const e = Nn.debugConfig || {};
            return {
                CLICK_DEBUG: e.CLICK_DEBUG,
                TIMEOUT_DEBUG: e.TIMEOUT_DEBUG,
                GA_DEBUG: e.GA_DEBUG,
                URL_DEBUG: e.URL_DEBUG,
                VARIATION_SHOWN_DEBUG: e.VARIATION_SHOWN_DEBUG,
                IN_LIST_DEBUG: e.IN_LIST_DEBUG
            }
        })(),
        bn = () => Nn.syncServerUrl,
        Rn = Nn.disableAsp,
        Pn = Nn.CLICK_PERF,
        xn = Nn.tpcBeacon,
        Ln = !Nn.vwoUuidV2Secure,
        Dn = $((() => window.VWO._.useCdn)) || !1,
        Un = Nn.enableRefreshLimit,
        Wn = Nn.expUrlChange,
        Mn = window._vwo_acc_id > 1044e3 || Nn.enableLoader,
        kn = !!Nn.eblCSync,
        Gn = !!Nn.hdPR;
    var Fn, jn = window._vwo_acc_id,
        Hn = [],
        $n = 0,
        Bn, Kn = !1,
        zn = function() {
            for (var e = 0; e < Hn.length; e++) Hn[e].d || (Hn[e].c(), Hn[e].d = !0)
        };

    function Yn() {
        return window._vis_debug
    }
    const Jn = {
        domain: void 0,
        _create: function(e, t, i, n, s, r, o) {
            var a, l;
            Yn() && 0 !== e.indexOf("debug") && (e = "debug" + e);
            const d = i > 0;
            let c = window._vis_opt_cookieDays;
            window.VWO._.cLFE && (r = !1), "_vwo_sn" !== e && "_vwo_ds" !== e && "_vis_opt_test_cookie" !== e && !isNaN(c = parseFloat(c)) && isFinite(c) && d && (i = c);
            var u = "";
            if (s ? u += "; expires=" + new Date(s).toGMTString() : i ? u += "; expires=" + new Date((new Date).getTime() + 864e5 * i).toGMTString() : !1 === i && (u = "; expires=Thu, 01 Jan 1970 00:00:01 GMT"), n || (n = Jn.domain), void 0 !== n) {
                n = (null === (l = null === (a = window.VWO._.allSettings.dataStore.plugins.DACDNCONFIG) || void 0 === a ? void 0 : a.jsConfig) || void 0 === l ? void 0 : l.dNISD) && !window._vis_opt_domain ? "" : "; domain=." + n
            }
            const g = e + "=" + encodeURIComponent(t) + u + (n || "") + "; path=/";
            window.VWO._.ss && !o ? (document.cookie = g + "; secure; samesite=none; Partitioned;", 6 === window._vwo_acc_id && e.indexOf("_vwo_ds") > -1 && !Kn && (this.create(e, "", !1, n, 1, r, !0), Kn = !0)) : document.cookie = g
        },
        create: function(e, t, i, n, s, r, o) {
            this._create(e, t, i, n, s, r, o), En(m.SET_COOKIE, e, t, i, s), mn("meta", {
                ckName: e,
                ckValue: t,
                ckDays: i,
                ckExpiryTs: s
            })
        },
        get: function(e, t, i, n) {
            var s;
            e = e.trim(), !i && Yn() && (e = "debug" + e), window.VWO._.cLFE;
            var r = document.cookie.match(new RegExp("(?:^|;)\\s*" + e.replace(/([.*+?^=!:${}()|[\]\/\\])/g, "\\$1") + "=(.*?)(?:;|$)", "i"));
            return s = r && decodeURIComponent(r[1]), En(m.GET_COOKIE, e, s), s
        },
        erase: function(e, t, i) {
            this.create(e, "", !1, t, 1, i), En(m.ERASE_COOKIE, e)
        },
        createThirdParty: function(e, t, i, n, s, r, o) {
            if (!window.mainThread) return window.fetcher.getValue("VWO._.cookies.createThirdParty", [e, t, i, n, s, r, o]);
            var a, l, d, c;
            let u = !1;
            if (s && (u = o ? o.multiple_domains : window._vwo_exp[s].multiple_domains), "_vwo" !== e && this._create(e, t, i, n), Yn() && 0 !== e.indexOf("debug") && (e = "debug" + e), !((c = window.vwo_$) && s && u || r || "_vwo" === e)) return void En(m.SET_THIRD_PARTY_COOKIE_ERROR, e, t, i, n);
            a = c("<iframe>").attr({
                height: "1px",
                width: "1px",
                border: "0",
                class: "vwo_iframe",
                name: "vwo_" + Math.random(),
                style: "position: absolute; left: -2000px; display: none"
            }).appendTo("head").load((function() {
                -1 !== e.indexOf("split") && this.parentNode.removeChild(this), --$n || zn()
            })), $n++;
            const g = window._vwo_server_url || "https://dev.visualwebsiteoptimizer.com";
            l = g + "/ping_tpc.php?account=" + jn + "&name=" + encodeURIComponent(e) + "&value=" + encodeURIComponent(t) + "&days=" + i + "&random=" + Math.random(), /MSIE (\d+\.\d+);/.test(navigator.userAgent) ? a.attr("src", l) : window.VWO._.lastPageUnloadURL || xn ? window.VWO.modules.tags.dataSync.utils.sendCall(null, {
                url: "/ping_tpc.php?account=" + jn + "&name=" + encodeURIComponent(e) + "&value=" + encodeURIComponent(t) + "&days=" + i + "&random=" + Math.random(),
                serverUrl: g
            }, null, zn, !0) : ((d = c("<form>").attr({
                action: g + "/ping_tpc.php",
                "accept-charset": "UTF-8",
                target: a.attr("name"),
                enctype: "application/x-www-form-urlencoded",
                method: "post",
                id: "vwo_form",
                style: "display:none"
            }).appendTo("head")).attr("action", l).submit(), d.remove()), En(m.SET_COOKIE, e, t, i, s, !0)
        },
        waitForThirdPartySync: function(e) {
            return i(this, void 0, void 0, (function*() {
                window.mainThread ? Hn.push({
                    c: e
                }) : yield window.fetcher.getValue('VWO._.cookies.waitForThirdPartySync("${{1}}")', null, {
                    captureGroups: [e]
                })
            }))
        },
        getAll: function(e = !1) {
            if (e && window.VWO._.isCookieFallbackEnabled) return window.fetcher.getValue("window.VWO._.cookies.getAll");
            const t = document.cookie.split(/; ?/),
                i = {};
            for (let e = 0; e < t.length; e++) {
                const n = t[e].split("="),
                    s = n[0],
                    r = n[1];
                try {
                    i[s] = r
                } catch (e) {}
            }
            return i
        },
        getItem: function(e, t = !1) {
            return t && window.VWO._.isCookieFallbackEnabled ? window.fetcher.getValue("window.VWO._.cookies.getItem", [e]) : e.indexOf("_vis_opt_") > -1 || e.indexOf("_vwo_") > -1 ? this.get(e) || this.get(e, !0) : this.get(e, !0, !0, !0)
        },
        setItem: function(e, t) {
            this.create(e, t)
        },
        includes: function(e, t = !1) {
            if (t && window.VWO._.isCookieFallbackEnabled) return window.fetcher.getValue("window.VWO._.cookies.includes", [e]);
            const i = new RegExp(e),
                n = Object.keys(Jn.getAll());
            for (let e = 0; e < n.length; e++)
                if (i.test(n[e])) return 1;
            return 0
        }
    };
    window.VWO._.cookies = Jn;
    class Xn {
        constructor(e) {
            this.storageConfig = e
        }
        getValWithPref(e) {
            const {
                prefer: t,
                cookieExpDays: i
            } = this.storageConfig, n = M.get(e), s = M.get(e);
            return "cookie" == t ? M.set(e, n) : "ls" == t && Jn.create(e, s, i), {
                cookie: JSON.parse(n),
                ls: JSON.parse(s)
            }
        }
        getVal(e) {
            const {
                strategy: t,
                prefer: i
            } = this.storageConfig;
            for (const n of t) return "all" == n ? this.getValWithPref(e)[i] : "ls" == n ? JSON.parse(M.get(e)) : Jn.get(e)
        }
        setVal(e, t) {
            const {
                strategy: i,
                cookieExpDays: n
            } = this.storageConfig;
            for (const s of i) "all" == s ? (M.set(e, t), Jn.create(e, t, n)) : "cookie" == s ? Jn.create(e, t, n) : M.set(e, t)
        }
    }

    function qn(e) {
        const t = new URL(e);
        return t.searchParams.set("_cu", encodeURI(window.location.href)), t.toString()
    }
    const Qn = e => {
        const {
            url: t,
            data: i,
            success: n = (() => {}),
            error: s = (() => {})
        } = e, r = new XMLHttpRequest;
        r.open("POST", qn(t), !0), r.send(i), r.onload = () => {
            n(r.responseText)
        }, r.onerror = s
    };
    class Zn extends wn {
        constructor(e, t, i) {
            super(), this.contentSyncLocation = "VWO._.contentSyncService", this.collectedData = {}, this.requestsChecker = {}, this.url = t, this.globalLookupContext = i, this.storageObj = new Xn(e)
        }
        parseStorageInfo(e, t, i) {
            if (!e) return;
            const [n, s] = t.split(".");
            return e[n] = e[n] || {}, e[n][s] = e[n][s] || {}, e[n][s][i]
        }
        getInfoFromGlobalObject(e, t) {
            if (!e) return;
            const [i, n] = t.split(".");
            return e[i] = e[i] || {}, e[i][n] = e[i][n] || {}, {
                argVn: e[i][n].args || {},
                vn: e[i][n].vn
            }
        }
        fixResponse(e) {
            const t = {
                fns: {}
            };
            e.fns = e.fns || {};
            for (const i in e.fns)
                for (const n in e.fns[i]) {
                    const s = e.fns[i][n],
                        r = JSON.stringify(JSON.parse(n));
                    t.fns[i] = t.fns[i] || {}, t.fns[i][r] = s
                }
            return t
        }
        syncGet(e, t, i = !0) {
            const n = {
                dataPresent: !1
            };
            if (!this.storageObj) return n;
            const s = this.storageObj.getVal(this.storageLookUpKey),
                r = JSON.stringify(t),
                o = this.getInfoFromGlobalObject(this.globalLookupContext, e),
                a = o && o.vn,
                l = o && o.argVn,
                d = this.parseStorageInfo(s, e, r);
            if (d && i) {
                const i = d;
                let s = !1,
                    o = !1;
                for (const n in l)
                    for (const a in l[n])
                        if (i.argVn[n] && i.argVn[n][a] && parseInt(i.argVn[n][a]) < parseInt(l[n][a])) {
                            s = !0, this.syncFromBackend(e, t, r, this.url), o = !0;
                            break
                        }
                return a && parseInt(a) > parseInt(i.vn) && !s && (this.syncFromBackend(e, t, r, this.url), o = !0), kn && o ? n : {
                    dataPresent: !0,
                    val: i.val
                }
            }
            return this.syncFromBackend(e, t, r, this.url), n
        }
        syncFromBackend(e, t, i, n) {
            if (kn) {
                const [s, r] = e.split(".");
                if (this.collectedData[s] = this.collectedData[s] || {}, this.collectedData[s][r] = this.collectedData[s][r] || [], this.requestsChecker[i]) return;
                this.requestsChecker[i] = 1, this.collectedData[s][r].push(t), this.debouncedCall = this.debouncedCall || yn((() => {
                    Qn({
                        url: n + "sync?a=" + window._vwo_acc_id,
                        data: JSON.stringify(this.collectedData),
                        success: e => {
                            this.updateStorage(JSON.parse(e))
                        }
                    }), this.collectedData = {}
                }), 10), this.debouncedCall()
            } else this.otherSide("syncFromBackend", [e, t, i, n])
        }
        updateStorage(e) {
            const t = this.storageObj.getVal(this.storageLookUpKey);
            let i = {};
            (t || !1) && (i = t);
            const n = (e = this.fixResponse(e)).fns;
            for (const e in n) {
                const t = n[e];
                for (const n in t) {
                    i.fns = i.fns || {}, i.fns[e] = i.fns[e] || {};
                    const s = i.fns[e][n];
                    if (s)
                        if (parseInt(s.vn) == parseInt(t[n].vn)) {
                            const s = i.fns[e][n].argVn;
                            let r = !0;
                            for (const e in s) {
                                if (!t[n].argVn[e] || !r) {
                                    r = !1;
                                    break
                                }
                                for (const i in s[e]) {
                                    const o = t[n].argVn[e][i],
                                        a = s[e][i];
                                    if (!o || parseInt(a) <= parseInt(o)) {
                                        r = !1;
                                        break
                                    }
                                }
                            }!r && (i.fns[e][n] = t[n])
                        } else parseInt(s.vn) < parseInt(t[n].vn) && (i.fns[e][n] = t[n]);
                    else i.fns[e][n] = t[n]
                }
            }
            this.storageObj.setVal(this.storageLookUpKey, JSON.stringify(i))
        }
    }
    window.VWO.modules.tags = {};
    class es {
        otherSide(...e) {
            e[0] = "VWO.modules.tags.checkEnvironment." + e[0], window.fetcher.getValue(...e)
        }
    }
    window.VWO.modules.tags.checkEnvironment = {};
    const ts = "lT",
        is = "sT",
        ns = "ivp",
        ss = "gp",
        rs = "ca",
        os = 10,
        as = 2,
        ls = function() {},
        ds = "w",
        cs = [739074, 714884, 708439, 765649],
        us = {
            VS_DATA: "vwoVsData"
        },
        gs = {
            SPLIT_REDIRECT: "_vwo_split_redirect"
        },
        vs = "vwoStandardTrigger",
        hs = window.VWO.TRACK_SESSION_COOKIE_EXPIRY_CUSTOM || 1 / 48,
        ps = "_vis_opt_",
        ws = "_vwo_",
        Es = {
            TRACK_GLOBAL_COOKIE_NAME: "_vwo_ds",
            TRACK_SESSION_COOKIE_NAME: "_vwo_sn",
            TRACK_SESSION_COOKIE_EXPIRY: hs,
            SESSION_TIMER_EXPIRE: 60 * hs * 60 * 1e3 * 24,
            COOKIE_VERSION: 3,
            COOKIE_TS_INDEX: 1,
            COOKIE_VERSION_INDEX: 0,
            FIRST_SESSION_ID_INDEX: 0,
            PC_TRAFFIC_INDEX: 1,
            RELATIVE_SESSION_ID_INDEX: 0,
            PAGE_ID_INFORMATION_INDEX: 1,
            SESSION_SYNCED_STATE_INDEX: 4,
            PAGE_ID_EXPIRY: 15,
            GLOBAL_OPT_OUT: "_vwo_global_opt_out",
            OPT_OUT: "_vis_opt_out",
            TEST_COOKIE: "_vis_opt_test_cookie",
            COOKIE_JAR: "_vwo",
            SAME_SITE: "_vwo_ssm",
            UUID: "uuid",
            UUID_V2: "uuid_v2",
            VWO_COOKIE_QUERY_PARAM: "vwo_q",
            DEFAULT_EXPIRY: 100,
            UUID_COOKIE_EXPIRY: 365.2425
        };

    function fs() {
        return Math.min(window.VWO.TRACK_GLOBAL_COOKIE_EXPIRY_CUSTOM || window.VWO.data.rp || 90, 90)
    }

    function _s() {
        return {
            [ps + "test_cookie"]: 0,
            [ws + "ds"]: fs(),
            [ws + "sn"]: hs,
            [ws + "referrer"]: 18e-5,
            [ws + "uuid"]: 3650,
            [ws + "uuid_v2"]: 366,
            [Es.SAME_SITE]: 3650
        }
    }
    class ms {
        otherSide(...e) {
            return e[0] = "window.VWO.modules.utils.campaignUtils." + e[0], window.fetcher.getValue(...e)
        }
        updateGoalCookieValueForExperience(e, t) {
            let i = e ? e.split("mE_")[1].split(",") : [];
            return i.includes(t) || i.push(t), `mE_${i.join(",")}`
        }
        isGoalTriggeredForExperience(e, t) {
            return (e ? e.split("mE_")[1].split(",") : []).includes(t)
        }
    }
    let Os;

    function Ss(e) {
        if (!e) return e;
        try {
            e = window.decodeURIComponent(e)
        } catch (e) {}
        return e
    }
    const ys = function() {
            if (void 0 !== Os) return Os;
            const e = [],
                t = window.VWO._.allSettings.dataStore.campaigns;
            let i, n;
            for (let i in t) e.push(i);
            return Os = !!(i = (window.location.search + window.location.hash).match(/.*_vis_test_id=(.*?)&.*_vis_opt_preview_combination=(.*?)(?:&|#|$)/)) && (!(!e.includes(i[1]) || !t[i[1]] || void 0 === t[i[1]].combs[n = Ss(i[2])]) && n), Os
        },
        Ts = e => window._vis_heatmap ? e : e[0],
        Is = /:nth-parent\((\d+)\)$/,
        Cs = /[A-Za-z1-9]*?:tm\(["']([\s\S]*?)["']\)(?:\:nth-parent\(\d\))?/,
        Ns = e => e.indexOf(":tm(") > -1,
        As = e => !!Ns(e),
        Vs = e => {
            const t = e.match(Is) || [];
            if (t.length < 2) return;
            const i = +t[1];
            return isNaN(i) ? void 0 : i
        };

    function bs() {
        const e = {};
        return function(t) {
            if (e[t]) return e[t];
            if (Ns(t)) {
                const {
                    targetElement: i,
                    targetText: n,
                    ancestorLevelCount: s,
                    childSel: r
                } = (e => {
                    const t = e.match(Cs) || [e],
                        i = t[0],
                        [n] = e.split(":tm("),
                        s = t[1],
                        r = Vs(i),
                        o = void 0 !== t.index ? e.slice(t.index + i.length, e.length).trim() : "",
                        a = n.trim().split(" ");
                    return {
                        targetElement: 1 == a.length ? a[0].toUpperCase() : a.map((e => (-1 === e.search(/(\.|#)/) && (e = e.toUpperCase()), e))).join(" "),
                        targetText: s,
                        ancestorLevelCount: r,
                        childSel: o
                    }
                })(t);
                return e[t] = {
                    targetElement: i,
                    targetText: n,
                    ancestorLevelCount: s,
                    childSel: r
                }
            }
            return {
                targetElement: "",
                targetText: ""
            }
        }
    }
    const Rs = bs(),
        Ps = {};

    function xs(e) {
        if (Array.isArray(Ps[e])) return Ps[e];
        const t = e.split("<vwo_sep>");
        return 1 === t.length ? Ps[e] = [{
            sel: e,
            isTxtSel: !0
        }] : Ps[e] = t.map((e => ({
            sel: e.trim(),
            isTxtSel: As(e)
        })))
    }
    const Ls = e => "number" == typeof e,
        Ds = (e, t) => !(!e || e.sel !== t),
        Us = (e, t) => Ls(e) && e === t,
        Ws = ({
            targetElement: e,
            targetText: t,
            ancestorLevelCount: i,
            childSel: n
        }, s, r) => {
            const o = [e, t].join(".");
            if (!r || !Array.isArray(r[o])) return null;
            for (let e = 0; e < r[o].length; e++) {
                const t = s[r[o][e]];
                if (!t) return null;
                const a = !i && !t.d || Us(i, t.d),
                    l = !n && !t.cd || Ds(t.cd, n);
                if (a && l) return t
            }
            return null
        },
        Ms = e => {
            const t = xs(e),
                i = window.VWO._.txtCfg || {},
                n = i.mp = i.mp || {};
            let s = "";
            const r = e => {
                s += e + ","
            };
            for (const e of t)
                if (e.isTxtSel)
                    if (n[e.sel]) r(n[e.sel]);
                    else {
                        const t = Rs(e.sel),
                            s = Ws(t, i.t, i.txtSelMap);
                        if (s && s.s) {
                            const t = "." + s.s;
                            r(t), n[e.sel] = t
                        }
                    }
            else r(e.sel);
            return s
        },
        ks = () => {
            window.VWO._.txtCfg && window.VWO._.txtCfg.mp && window.fetcher.setValue("window.VWO._.txtCfg.mp", window.VWO._.txtCfg.mp)
        };
    class Gs {
        constructor() {
            this.uuid = "", this.preview = ys, this.hideElExpression = "{opacity:0 !important;filter:alpha(opacity=0) !important;background:none !important;}", this.domIndependentCampaigns = ["ANALYSIS", "SURVEY", "ANALYZE_RECORDING", "ANALYZE_HEATMAP", "ANALYZE_FORM", "TRACK", "FUNNEL", "INSIGHTS_FUNNEL", "INSIGHTS_METRIC"], this.sessionBasedCampaigns = {
                ANALYZE_RECORDING: !0,
                ANALYZE_HEATMAP: !0,
                ANALYZE_FORM: !0,
                TRACK: !0,
                FUNNEL: !0,
                INSIGHTS_FUNNEL: !0,
                INSIGHTS_METRIC: !0
            }
        }
        otherSide(...e) {
            return e[0] = "VWO.modules.utils.libUtils." + e[0], e[2] && (e[2] = {
                captureGroups: e[2]
            }), window.fetcher.getValue(...e)
        }
        isDomDependent(e) {
            return "VISUAL_AB" === e || "VISUAL" === e
        }
        isTestingCampaign(e) {
            return this.isDomDependent(e) || "SPLIT_URL" === e
        }
        generateUUID() {
            return "Jxxxxxxxxxxx4xxxyxxxxxx5xxxxxxxx9".replace(/[xy]/g, (function(e) {
                const t = 16 * Math.random() | 0;
                return ("x" == e ? t : 3 & t | 8).toString(16).toUpperCase()
            }))
        }
        isDomIndependentCampaign(e) {
            return -1 !== this.domIndependentCampaigns.indexOf(e)
        }
        isSessionBasedCampaign2(e) {
            const t = e.type;
            return !!this.sessionBasedCampaigns[t]
        }
        hasInsightsMetric(e) {
            return "INSIGHTS_FUNNEL" === e || "INSIGHTS_METRIC" === e
        }
        isBot2() {
            return window.VWO._.isBot || window.navigator.userAgent.toLowerCase().indexOf("bot") >= 0 || window.navigator.userAgent.toLowerCase().indexOf("spider") >= 0 || window.navigator.userAgent.toLowerCase().indexOf("preview") >= 0
        }
        isPageBasedGoal(e) {
            return "SEPARATE_PAGE" === e || "CUSTOM_GOAL" === e || "REVENUE_TRACKING" === e
        }
        isSplitVariation(e) {
            return "SPLIT_URL" === e.type && e[ns]
        }
        shouldTrackUserForCampaign(e) {
            return "number" == typeof e && (e = window._vwo_exp[e]), !e || !window._vwo_code || !window._vwo_code[ts] && !window._vwo_code[is] || (this.isDomIndependentCampaign(e.type) || this.isSplitVariation(e))
        }
        getUUIDString(e) {
            return e ? "&u=" + e : ""
        }
        isAnalyzeCampaign(e) {
            return "ANALYZE_RECORDING" === e || "ANALYZE_HEATMAP" === e || "ANALYZE_FORM" === e
        }
        updateGoalsKind(e, t) {
            const i = {};
            return Object.keys(e).forEach((n => {
                const s = e[n],
                    r = e[n].mt;
                r && Object.keys(s.goals).length && Object.entries(r).forEach((([e, s]) => {
                    const r = this.getGoalKind(s);
                    !r || t && !t[r] || (i[n] = i[n] || {}, i[n][e] = r)
                }))
            })), t || (window.VWO._.goalsToBeConvertedSynchronously = i), i
        }
        getGoalKind(e) {
            let t;
            const i = window.VWO._.allSettings.triggers[e];
            if (i)
                if ("object" == typeof i.cnds[0]) {
                    switch (i.cnds[0].event) {
                        case _.DOM_CLICK:
                            t = "CLICK_ELEMENT";
                            break;
                        case _.DOM_SUBMIT:
                            t = "FORM_SUBMIT";
                            break;
                        case _.PAGE_UNLOAD:
                            t = "PAGE_UNLOAD"
                    }
                } else {
                    switch (i.cnds[1].event) {
                        case _.DOM_SUBMIT:
                        case _.DOM_CLICK:
                            t = "ENGAGEMENT"
                    }
                }
            return t
        }
        isXpathAllHead(e, t, i = !1) {
            if (e.muts = e.muts || {}, "boolean" == typeof e.muts.pvtMut && !i) return e.muts.pvtMut;
            const n = t.split(",");
            let s = !0;
            for (let e = 0; e < n.length; e++)
                if (n[e].trim() && "head" !== n[e].toLowerCase()) {
                    s = !1;
                    break
                }
            return i || (e.muts.pvtMut = s), s
        }
        isEligibleToSendCall(e, t) {
            return !ys() && (t && !t.visDebug || !window._vis_debug) && this.shouldTrackUserForCampaign(e) && (t && t.vwoInternalProperties.shouldExecuteLib || window.VWO._.shouldExecuteLib)
        }
        isPersonalizeCampaign(e) {
            var t;
            return "TARGETING" === (null === (t = e.iType) || void 0 === t ? void 0 : t.type)
        }
        doNotHideElements(e) {
            return e && "boolean" == typeof e
        }
        getMatchedCookies(e) {
            let t = [];
            return document.cookie && (t = document.cookie.match(e) || []), t
        }
        getCombinationCookie() {
            let e = this.getMatchedCookies(/(?:^|;)\s?(_vis_opt_exp_\d+_combi=[^;$]*)/gi);
            e = e.map((function(e) {
                try {
                    const t = decodeURIComponent(e);
                    return /_vis_opt_exp_\d+_combi=(?:\d+,?)+\s*$/.test(t) ? t : ""
                } catch (e) {
                    return ""
                }
            }));
            const t = [];
            return e.forEach((function(e) {
                const i = e.match(/([\d,]+)/g);
                i && t.push(i.join("-"))
            })), t.join("|")
        }
        getSelectorPath(e, t) {
            let i = "",
                n = "",
                s = t.sections[1].variations[e];
            if ("string" == typeof s && (s = vwo_$.parseJSON(s)), s)
                for (let e = 0; e < s.length; e++) {
                    let r = s[e].xpath;
                    r && (s[e].dHE ? t.dHE = !0 : (t.mSP && (r = r.replace(/html\.vwo_p_s_\w+\s*/g, "")), As(r) ? i += Ms(r) : i += r + ",")), s[e].cpath && !s[e].dHE && (n += s[e].cpath + ",")
                }
            return {
                variationXPathSelector: i,
                variationCPathSelector: n
            }
        }
        getCampaignXPath(e) {
            const t = {
                selector: "",
                selectorPerVariation: {},
                cPathSelector: "",
                cPathSelectorPerVariation: {}
            };
            if (e.xPath) return t.selector = e.xPath, t.cPathSelector = e.cPath, t;
            if (!this.isDomDependent(e.type)) return t;
            let i = e.combination_chosen || e.cc;
            const n = e.sections;
            if ("VISUAL_AB" === e.type) {
                if (i) 1 != i && (t.selector = this.getSelectorPath(i, e).variationXPathSelector);
                else
                    for (i in e.combs)
                        if (e.combs.hasOwnProperty(i)) {
                            const {
                                variationXPathSelector: n,
                                variationCPathSelector: s
                            } = this.getSelectorPath(i, e);
                            t.selector += n, t.cPathSelector += s, t.cPathSelectorPerVariation[i] = s, t.selectorPerVariation[i] = n.substring(0, n.length - 1)
                        }
            } else {
                const e = On(n);
                let i = e.length;
                for (; i--;) n[e[i]].path && (t.selector += n[e[i]].path + ",")
            }
            return !e.dHE || t.selector && !this.isXpathAllHead(e, t.selector, !0) || (t.selector = (t.selector || "") + ".vwo_dummy_selector,"), t.cPathSelector && (t.cPathSelector = t.cPathSelector.substring(0, t.cPathSelector.length - 1)), t.selector && (t.selector = t.selector.substring(0, t.selector.length - 1)), ks(), t
        }
    }
    class Fs {
        otherSide(...e) {
            return e[0] = "VWO.modules.tags.dataSync." + e[0], window.fetcher.getValue(...e)
        }
    }
    var js = {
            PARSE_TLD: "pTLD"
        },
        Hs = ["co", "org", "com", "net", "edu", "au", "ac"],
        $s;

    function Bs(e) {
        var t, i = e.split("."),
            n = i.length,
            s = i[n - 2];
        return s && Hs.includes(s) ? (t = i[n - 3] + "." + s + "." + i[n - 1], En(js.PARSE_TLD, e, t), t) : (t = s + "." + i[n - 1], En(js.PARSE_TLD, e, t), t)
    }
    const Ks = {
        init: function() {
            $s = Jn.get("_vwo_referrer"), Jn.erase("_vwo_referrer"), "string" != typeof $s && ($s = document.referrer)
        },
        get: function() {
            return -1 !== location.search.search("_vwo_test_ref") ? document.referrer : $s
        },
        set: function() {
            Jn.create("_vwo_referrer", $s, 18e-5)
        }
    };
    window.VWO.modules.vwoUtils.referrer = Ks;
    const zs = {
        get navigator() {
            return navigator
        },
        get pageTitle() {
            return document.title
        },
        get doNotTrack() {
            return window.doNotTrack
        },
        get windowName() {
            return window.name
        },
        get currentUrl() {
            return window._vis_opt_url || window.location.href
        },
        get location() {
            return window.location
        },
        get document() {
            return window.location
        },
        get history() {
            return window.history
        },
        get accountId() {
            return window._vwo_acc_id
        },
        get smartCodeVersion() {
            return window._vwo_code_version
        },
        get serverUrl() {
            return window._vwo_server_url || "https://dev.visualwebsiteoptimizer.com/"
        },
        get vwoText() {
            return window._vwo_text
        },
        get vwoCode() {
            return window._vwo_code
        },
        get MutationObserver() {
            let e = window.MutationObserver || window.WebKitMutationObserver || window.MozMutationObserver;
            return window.Zone && window.Zone.__symbol__ && (e = window[window.Zone.__symbol__("MutationObserver")]), e
        },
        get vwoInternalProperties() {
            return window.VWO._
        },
        get cookie() {
            return document.cookie
        },
        get visDebug() {
            return window._vis_debug
        },
        get cookieDomain() {
            return window._vis_opt_domain || window._vwo_cookieDomain || Bs(window.location.host || new URL(document.URL).host)
        },
        get vwoStyle() {
            return window._vwo_style
        },
        get screen() {
            return window.screen
        },
        get vwoCss() {
            return window._vwo_css
        },
        get visOptUrl() {
            return window._vis_opt_url
        },
        get allSettings() {
            return window.VWO._.allSettings
        },
        get apiSectionCallback() {
            return window._vwo_api_section_callback
        },
        get encodeURIComponent() {
            return window.encodeURIComponent
        },
        get page() {
            return {
                title: zs.pageTitle,
                url: zs.currentUrl,
                referrerUrl: Ks.get()
            }
        },
        get timeSpentInASession() {
            var e, t, i, n, s, r;
            return +Date.now() - 1e3 * +(null === (i = null === (t = null === (e = window.VWO.phoenix) || void 0 === e ? void 0 : e.store) || void 0 === t ? void 0 : t.getters) || void 0 === i ? void 0 : i.sessionStart) ? (+Date.now() - 1e3 * +(null === (r = null === (s = null === (n = window.VWO.phoenix) || void 0 === n ? void 0 : n.store) || void 0 === s ? void 0 : s.getters) || void 0 === r ? void 0 : r.sessionStart)) / 1e3 : 0
        },
        get vwoUUID() {
            return window._vwo_uuid
        }
    };
    window.VWO.modules.dataStorePlugin = zs;
    const Ys = {
            [_.VARIATION_SHOWN]: {
                ignoreMetricDataCheck: !0
            },
            [_.ERROR_ONPAGE]: {},
            [_.CURSOR_THRASHED]: {},
            [_.PAGE_REFRESHED]: {},
            [_.QUICK_BACK]: {},
            [_.COPY]: {},
            [_.SELECTION]: {},
            [_.TAB_IN]: {},
            [_.TAB_OUT]: {},
            [_.LEAVE_INTENT]: {},
            [_.REPEATED_SCROLLED]: {},
            [_.REPEATED_HOVERED]: {},
            [_.PAGE_VIEW]: {},
            [_.DOM_CLICK]: {},
            [_.DOM_SUBMIT]: {},
            [_.CUSTOM_CONVERSION]: {},
            [_.REVENUE_CONVERSION]: {},
            [_.SYNC_VISITOR_PROP]: {
                ignoreMetricDataCheck: !0
            },
            [_.PAGE_UNLOAD]: {},
            [_.DEBUG_EVENT]: {
                ignoreMetricDataCheck: !0
            }
        },
        Js = e => !!Ys[e],
        Xs = e => !!$((() => window.VWO._.allSettings.dataStore.events[e].nw));
    class qs {
        constructor() {
            this.vwoEventsToBeSynced = Object.assign({}, Ys), this.allowedMetaDataProps = {
                ogName: !0,
                source: !0
            }
        }
        otherSide(...e) {
            return e[0] = "VWO.modules.tags.dataSync.utils." + e[0], window.fetcher.getValue(...e)
        }
        shouldSendEventCall(e, t) {
            var i;
            const n = t.name;
            if (!n) return !1;
            const s = this.vwoEventsToBeSynced[t.name];
            if (void 0 === s && !t.props.isCustomEvent && !t.props.isSurveyEvent) return !1;
            if (!window.VWO._.allSettings.dataStore.events[n]) {
                let e;
                try {
                    e = JSON.parse(M.get(qs.UNREG_EVENT_LOCAL_STORAGE_NAME)) || {}
                } catch (t) {
                    e = {}
                }
                if (e[n]) return !1; {
                    e[n] = !0;
                    const t = JSON.stringify(e);
                    M.set(qs.UNREG_EVENT_LOCAL_STORAGE_NAME, t)
                }
            }
            if (t.props.isCustomEvent || t.props.isSurveyEvent || t.props.forceCall) return !0;
            if (!s || !s.ignoreMetricDataCheck) {
                const e = null === (i = t._vwo) || void 0 === i ? void 0 : i.eventDataConfig;
                if (!e || Object.keys(e).length <= 0) return !1
            }
            if (t.name !== _.VARIATION_SHOWN) return !0;
            let r = "non-analytics";
            location.href.includes("jsMode=Any") && (r = "analytics");
            const o = null == t ? void 0 : t.props,
                a = null == o ? void 0 : o.id;
            if (!o || !a) return !1;
            const l = e.currentSettings.dataStore.campaigns[a] || window._vwo_exp[a],
                d = window.VWO.modules.utils.libUtils.isSessionBasedCampaign2(l),
                c = "SURVEY" === l.type;
            return !(!("analytics" === r || "non-analytics" === r && o.isFirst) || d || c)
        }
        evaluateDataForEventsCall(e, t, i) {
            var n, s, r, o, a;
            let l = !0;
            const d = null === (s = null === (n = i._vwo) || void 0 === n ? void 0 : n.eventDataConfig) || void 0 === s ? void 0 : s.addVwoPageMeta;
            null === (o = null === (r = i._vwo) || void 0 === r ? void 0 : r.eventDataConfig) || void 0 === o || delete o.addVwoPageMeta, this.syncAdditionalDataWithEventsData(null === (a = i._vwo) || void 0 === a ? void 0 : a.eventDataConfig, i);
            const c = i.eventUuid,
                u = {
                    d: {}
                };
            if (u.d.msgId = `${t}-${+new Date}`, u.d.visId = t, c && (u.d.eventUuid = c), u.d.event = {
                    props: this.excludeEventPropsNotToBeSynced(e, i.name, i.props),
                    name: i.name,
                    time: i.time
                }, i.props.$metaData) {
                const e = {},
                    t = i.props.$metaData;
                for (const i in t) Object.prototype.hasOwnProperty.call(this.allowedMetaDataProps, i) && (e[i] = t[i]);
                Object.keys(e).length > 0 && (u.d.event.props.vwoMeta = u.d.event.props.vwoMeta || {}, Object.assign(u.d.event.props.vwoMeta, e)), delete u.d.event.props.$metaData
            }
            return i.props.$visitor && (u.d.visitor = i.props.$visitor, delete i.props.$visitor, Object.keys(u.d.visitor.props).length <= 0 && (l = !1)), u.d.event.props.page = i.page || this.getPageInfo(d), this.resetDataForCurrentEvent(i), {
                payload: u,
                shouldSyncCall: l
            }
        }
        getPageInfo(e) {
            var t;
            const i = zs.page;
            return e && (i.cnnUrl = document.querySelector && ((null === (t = document.querySelector("link[rel='canonical']")) || void 0 === t ? void 0 : t.href) || ""), i.pageViewId = window.VWO._.track.getTrackPageId && window.VWO._.track.getTrackPageId() || window.VWO._.pageId), i
        }
        syncAdditionalDataWithEventsData(e, t) {
            if (e)
                for (const i in e)
                    if (Object.prototype.hasOwnProperty.call(e, i) && "shouldSyncData" !== i) {
                        const n = e[i];
                        if (void 0 === n) continue;
                        t.props ? t.props[i] = n : t[i] = n
                    }
        }
        resetDataForCurrentEvent(e) {
            var t;
            let i = (null === (t = e._vwo) || void 0 === t ? void 0 : t.eventDataConfig) || {};
            (i || e.props) && (i = {}, e.props = {})
        }
        excludeEventPropsNotToBeSynced(e, t, i) {
            var n, s, r, o, a, l, d;
            const c = ["fireLinkedTagSync", "isTrusted", "page", "$visitor", "isCustomEvent", "forceCall", "VWO"];
            if (!i.isCustomEvent) {
                const i = (null === (o = null === (r = null === (s = null === (n = e.currentSettings) || void 0 === n ? void 0 : n.dataStore) || void 0 === s ? void 0 : s.events) || void 0 === r ? void 0 : r[t]) || void 0 === o ? void 0 : o.nS) || (null === (d = null === (l = null === (a = window.VWO._.allSettings.dataStore) || void 0 === a ? void 0 : a.events) || void 0 === l ? void 0 : l[t]) || void 0 === d ? void 0 : d.nS) || [];
                Array.prototype.push.apply(c, i)
            }
            if (!c || !c.length) return i;
            const u = {},
                g = window.VWO._.allSettings.dataStore.events[t];
            for (const e in i)
                if (Object.prototype.hasOwnProperty.call(i, e)) {
                    const t = i[e];
                    c.indexOf(e) > -1 ? delete u[e] : u[e] = !g && t ? Tn(t, 100) : t
                }
            return u
        }
    }
    qs.UNREG_EVENT_LOCAL_STORAGE_NAME = "vwoUnRegEvents";
    class Qs extends qs {
        sendCall(e, t, i, n, s, r, o) {
            return this.otherSide("sendCall", [e, t, i, n, s, r, o])
        }
        addDataFromMTAndSend(e, t, i, n, s, r, o, a) {
            return gr(o.name), this.otherSide("addDataFromMTAndSend", [e, t, i, n, s, r, o, a])
        }
        getDataForEventsCall(e, t, n) {
            return i(this, void 0, void 0, (function*() {
                const i = window.VWO.modules.tags.sessionInfoService,
                    {
                        payload: s,
                        shouldSyncCall: r
                    } = this.evaluateDataForEventsCall(e, t, n);
                return s.d.sessionId = yield i.getSessionId(), {
                    data: s,
                    shouldSyncCall: r
                }
            }))
        }
    }
    var Zs = new Qs;
    class er extends Fs {
        execute({
            event: e,
            getters: t
        }, n, s, r, o) {
            return i(this, void 0, void 0, (function*() {
                if (t.visDebug)(s = s || ls)(r);
                else if (Zs.shouldSendEventCall(t, e)) {
                    o = o || (yield Xr.createUUIDCookie2(t, n));
                    const {
                        data: i,
                        shouldSyncCall: a
                    } = yield Zs.getDataForEventsCall(t, o, e);
                    a && (yield Zs.addDataFromMTAndSend(t, null, i, s, !0, r, e, null == n ? void 0 : n.id))
                } else(s = s || ls)(r)
            }))
        }
    }
    const tr = new er,
        ir = tr.execute.bind(tr);

    function nr(e, t) {
        var i;
        const n = e.conflictingPropsData || {};
        if (!e.props) {
            e.props = {};
            const i = ["name", "props", "_vwo", "_meta", "conflictingPropsData", "eventUuid"];
            for (const t in e) Object.prototype.hasOwnProperty.call(e, t) && (i.includes(t) || (e.props[t] = e[t]));
            Object.assign(e.props, n), Object.keys(t).forEach((i => {
                e.props[i] = t[i]
            }))
        }
        e.aux && (e.props.aux = e.aux), e.time = e.time || (null === (i = e.VWO) || void 0 === i ? void 0 : i.firedTime) || +new Date
    }
    const sr = {};

    function rr({
        vwoEvents: e,
        getters: t,
        data: n,
        actions: s,
        event: r
    }, o, a = {}, l = null) {
        var d, c;
        return i(this, void 0, void 0, (function*() {
            if (r.name = o || r.name, null === (d = sr[r.name]) || void 0 === d ? void 0 : d.shouldWaitForCallbackExecution) return;
            let i;
            nr(r, a), r.name === _.VARIATION_SHOWN && (i = t.settings.campaigns[r.props.id]);
            const u = null === (c = r._vwo) || void 0 === c ? void 0 : c.eventDataConfig;
            if (u) {
                const i = Object.keys(u);
                for (let o = i.length - 1; o >= 0; --o) {
                    const a = i[o];
                    r._vwo.eventDataConfig = u[a], delete u[a], yield ir({
                        getters: t,
                        event: r,
                        data: n,
                        actions: s,
                        vwoEvents: e
                    }, null, l, r.props, a)
                }
            } else yield ir({
                getters: t,
                event: r,
                data: n,
                actions: s,
                vwoEvents: e
            }, i, l, r.props)
        }))
    }

    function or(e, t, n, s = null) {
        const r = window.phoenix;
        return n.name = n.name || t, sr[t] = sr[t] || {}, sr[t].shouldWaitForCallbackExecution = !0, (e = e || r).trigger(t, n, (function(e) {
            return i(this, void 0, void 0, (function*() {
                const i = r.store,
                    n = i.getters;
                sr[t].shouldWaitForCallbackExecution = !1, yield rr({
                    getters: n,
                    actions: i.actions,
                    event: e,
                    vwoEvents: r,
                    data: {}
                }, t, {}, s)
            }))
        }))
    }

    function ar(e, t, i, n = {}, s = null) {
        return or(e, t, Object.assign({
            name: t,
            time: +new Date,
            props: i
        }, n), s)
    }
    const lr = {
            variationShown(e, t, n) {
                return i(this, void 0, void 0, (function*() {
                    e = e || window.phoenix, n && void 0 === n.isFirst && (n.isFirst = t.isFirst), yield ar(e, _.VARIATION_SHOWN, t, {}, Xr.variationShownCallback)
                }))
            },
            dimensionTagPushed(e, t) {
                ar(e = e || window.phoenix, _.DIMENSION_TAG_PUSHED, t)
            },
            unhideElement(e, t, i) {
                i || ar(e, _.UNHIDE_ELEMENT, void 0, t)
            }
        },
        dr = e => !!$((() => window.VWO._.eventsSynced[e])),
        cr = () => {
            window.VWO.phoenix.on(_.END_APPLY_CHANGES, (() => {
                const e = $((() => window.VWO.phoenix.store.getters)),
                    t = e.eventsHistory || [],
                    i = window.VWO._.allSettings.dataStore.events;
                if (t.length > 0) try {
                    const n = (e => e.reduce(((e, t) => (e[t.name] = t.event, e)), {}))(t);
                    for (const t in i)
                        if (Js(t) && Xs(t) && n[t] && !dr(t)) {
                            const i = n[t];
                            i.props = i.props || {}, i.props.forceCall = !0, rr({
                                getters: e,
                                actions: window.VWO.phoenix.store.actions,
                                event: n[t],
                                vwoEvents: window.VWO.phoenix,
                                data: {}
                            }, t)
                        }
                } catch (e) {}
            }))
        },
        ur = () => {
            window.VWO._.eventsSynced = {}
        },
        gr = e => {
            window.VWO._.eventsSynced = window.VWO._.eventsSynced || {}, window.VWO._.eventsSynced[e] = !0
        };
    let vr;

    function hr(e) {
        vr = e
    }

    function pr(e) {
        window.VWO = null != e ? e : vr
    }
    window.VWO.modules.events = {
        syncEventsDataToDataLayer: rr,
        fireEventAndSyncData: or,
        events: lr,
        markEventSyncedWT: gr
    };
    const wr = {
        VISITOR_IS_NOT_OPTED_OUT: "visitorIsNotOptedOut",
        VISITOR_IS_OPTED_OUT_COMPLETELY: "visitorIsOptedOutCompletely",
        VISITOR_IS_OPTED_OUT: "visitorIsOptedOut"
    };
    var Er;
    ! function(e) {
        e[e.OPTED_OUT_WITH_EXPERIENCE = 0] = "OPTED_OUT_WITH_EXPERIENCE", e[e.OPTED_OUT_PARTIALLY = 1] = "OPTED_OUT_PARTIALLY", e[e.OPTED_OUT_COMPLETELY = 2] = "OPTED_OUT_COMPLETELY"
    }(Er || (Er = {}));
    class fr {
        setOptOutStateConfig() {
            let e, t, i, n;
            switch (e = window.VWO._.isWorkerThread ? window.phoenix.storages.storages.cookies.get("_vis_opt_out", !0) : window.VWO._.cookies.get("_vis_opt_out", !0), e && (e = Number(e)), e) {
                case 0:
                    t = wr.VISITOR_IS_OPTED_OUT, i = !0, n = !1;
                    break;
                case 1:
                case 2:
                    t = wr.VISITOR_IS_OPTED_OUT_COMPLETELY, i = !1, n = !1;
                    break;
                default:
                    t = wr.VISITOR_IS_NOT_OPTED_OUT, i = !0, n = !0
            }
            window.VWO.phoenix && window.fetcher.setValue("window.VWO._.optOutStates", {
                state: t,
                executeLib: i,
                shouldWeTrackVisitor: n
            }), window.VWO._.optOutStates = {
                state: t,
                executeLib: i,
                shouldWeTrackVisitor: n
            }
        }
        callStopAnalyzeAndSurvey() {
            window.VWO._.optOutStates.shouldWeTrackVisitor || (window.VWO._.isWorkerThread ? window.fetcher.getValue("window.VWO.modules.otherLibDeps.stopAnalyzeAndSurvey") : window.VWO.modules.otherLibDeps.stopAnalyzeAndSurvey())
        }
        getOptOutStateConfig() {
            return window.VWO._.optOutStates
        }
        shouldExecuteLibOnBasisOfCurrentOptOutState() {
            return !(!ys() && !window._vis_debug) || (this.getOptOutStateConfig().executeLib || window._removeVwoGlobalStyle(), this.getOptOutStateConfig().executeLib)
        }
        shouldWeTrackVisitor() {
            return !(!ys() && !window._vis_debug) || this.getOptOutStateConfig().shouldWeTrackVisitor
        }
        isVisitorOptedOut() {
            return !ys() && !window._vis_debug && this.getOptOutStateConfig().state !== wr.VISITOR_IS_NOT_OPTED_OUT
        }
    }
    const _r = new fr;

    function mr(e) {
        const t = e.storages.storages.cookies.get("_vis_opt_out", !0);
        return !!(t || window.location.href.indexOf("vwo_opt_out=1") > -1) && ("0" !== t && ("2" !== t && window.fetcher.getValue("window.VWO.optOut.process", [{
            accountId: e.accountId,
            domain: e.cookieDomain
        }]), !0))
    }
    var Or, Sr, yr, Tr, Ir, Cr, Nr;
    window.VWO.modules.vwoUtils.optOut = _r,
        function(e) {
            e.DOM = "vwo_dom"
        }(Or || (Or = {})),
        function(e) {
            e.WILD_CARD = "*", e.TRIGGER = "trigger", e.POST_INIT = "post-init", e.TIMER = "vwo_timer"
        }(Sr || (Sr = {})),
        function(e) {
            e.URL_CHANGE = "vwo_urlChange", e.LEAVE_INTENT = "vwo_leaveIntent", e.CLICK_EVENT = "vwo_dom_click", e.SUBMIT_EVENT = "vwo_dom_submit", e.PAGE_LOAD_EVENT = "vwo_page_load"
        }(yr || (yr = {})),
        function(e) {
            e.PAGE_VIEW = "vwo_pageView", e.PAGE_UNLOAD_EVENT = "vwo_pageUnload"
        }(Tr || (Tr = {})),
        function(e) {
            e.EXIT_CONDITIONS = "__exitConditions"
        }(Ir || (Ir = {})),
        function(e) {
            e.DOM_CONTENT_LOADED = "DOMContentLoaded", e.SCROLL = "scroll", e.CLICK = "click", e.SUBMIT = "submit"
        }(Cr || (Cr = {})),
        function(e) {
            e[e.DEBUG = 0] = "DEBUG", e[e.INFO = 1] = "INFO", e[e.WARN = 2] = "WARN", e[e.ERROR = 3] = "ERROR"
        }(Nr || (Nr = {}));
    class Ar {
        constructor(e) {
            this.setLevel(e)
        }
        setLevel(e = "warn") {
            this.logLevel = Nr[e.toUpperCase()]
        }
        info(e, t = {}) {
            this.customLog(Nr.INFO, e, t)
        }
        debug(e, t = {}) {
            this.customLog(Nr.DEBUG, e, t)
        }
        warn(e, t = {}) {
            var i, n;
            this.customLog(Nr.WARN, e, t, null === (n = null === (i = window.VWO) || void 0 === i ? void 0 : i._) || void 0 === n ? void 0 : n.customError)
        }
        error(e, t = {}) {
            var i, n;
            this.customLog(Nr.ERROR, e, t, null === (n = null === (i = window.VWO) || void 0 === i ? void 0 : i._) || void 0 === n ? void 0 : n.customError)
        }
        customLog(e, t, i, n = null) {
            var s, r, o;
            if (e >= this.logLevel) {
                const a = this.formatMessage(e, t, i);
                null === (o = null === (r = null === (s = window.VWOEvents) || void 0 === s ? void 0 : s.store) || void 0 === r ? void 0 : r.actions) || void 0 === o || o.addLogsForDebugging(a), n ? n(a) : this.consoleLog(e, [a])
            }
        }
        consoleLog(e, t) {
            switch (e) {
                case Nr.INFO:
                    console.info(...t);
                    break;
                case Nr.WARN:
                    console.warn(...t);
                    break;
                case Nr.ERROR:
                    console.error(...t);
                    break;
                default:
                    console.log(...t)
            }
        }
        formatMessage(e, t, i) {
            var n, s;
            const r = Object.keys(i).reduce(((e, t) => e.replace(new RegExp(`{{${t}}}`, "g"), i[t])), t),
                o = `${Or.DOM}_`;
            let a = i;
            const l = (null === (n = i.data) || void 0 === n ? void 0 : n.vwoEventName) || i.vwoEventName;
            l !== o + Cr.CLICK && l !== o + Cr.SUBMIT || (a = i.data ? null === (s = i.data) || void 0 === s ? void 0 : s.props : a.props, a = a || {
                name: l
            });
            let d = JSON.stringify;
            try {
                d = window.VWO._.native.JSON.stringify || JSON.stringify
            } catch (e) {}
            return `VWO: [${Nr[e].toUpperCase()}] [${(new Date).toUTCString()}] ${r} ${d(a)}`
        }
    }
    var Vr = new Ar("warn");
    class br {
        toAbsURL(e) {
            return new URL(e, document.baseURI).href
        }
        isHashPresent(e) {
            return -1 !== e.indexOf("#")
        }
        isQueryParamPresent(e, t) {
            var i = e.indexOf("#"),
                n = e.indexOf("?"),
                s = t ? -1 : e.indexOf("=");
            return -1 === i ? -1 !== n || -1 !== s : -1 !== n && i > n || -1 !== s && i > s
        }
        otherSide(...e) {
            return e[0] = "VWO.modules.vwoUtils.urlUtils." + e[0], window.fetcher.getValue(...e)
        }
    }
    var Rr = function(e) {
            return e.replace(/^(https?:\/\/)(?:w{3}\.)?(.*?)(?:\/(?:home|default|index)\..{3,4}|\/$)?(?:\/)?([\?#].*)?$/i, "$1$2$3")
        },
        Pr = function(e) {
            return e.replace(/^(https?:\/\/)(?:w{3}\.)?(.*?)(?:(?:home|default|index)\..{3,4})?([\?#].*)?$/i, "$1$2$3")
        },
        xr = function(e) {
            return Pr(e).replace(/\/\?/gi, "?")
        },
        Lr = window._vis_opt_url,
        Dr;
    class Ur {
        constructor() {
            Dr = this
        }
        regexEscape(e) {
            return e.replace(/[\-\[\]{}()*+?.,\/\\^$|#\s]/g, "\\$&")
        }
        cleanURL(e, t) {
            return Lr && !t ? Lr : e.replace(/^(.*[^\*])(\/(home|default|index)\..{3,4})((\?|#).*)*$/i, "$1$4")
        }
        removeWWW(e, t) {
            return e = e.replace(/^(https?:\/\/)(www\.)?(.*)$/i, "$1$3"), t && (e = e.replace(/(^\*?|\/\/)www\./i, "$1")), e
        }
        stripSlashes(e, t, i) {
            if (e = e.replace(/\/$/, ""), t) {
                var n = e.indexOf("/?");
                e.indexOf("?") - 1 === n && (e = e.replace(/\/\?([^\?]*)(.*)/, "?$1$2"))
            }
            if (i) {
                var s = e.indexOf("/#");
                e.indexOf("#") - 1 === s && (e = e.replace(/\/#([^#]*)(.*)/, "#$1$2"))
            }
            return e
        }
        cleanPattern(e) {
            let t = "";
            return {
                regex: e.replace(/\(\?([a-zA-Z])\)/g, ((...e) => (e[1] && (t += e[1]), ""))),
                flags: t
            }
        }
        matchRegex(e, t, i, n) {
            if ("string" != typeof e || "string" != typeof t) return !1;
            let s = "ig";
            if (n) {
                const {
                    regex: i,
                    flags: n
                } = Dr.cleanPattern(t);
                s = n || "g";
                try {
                    return new RegExp(i, s).exec(e) || Dr.matchRelativeUrl(e, i, s)
                } catch (e) {
                    const n = "Failed to create regex for the pattern: " + t + ", the cleaned regex derived from the pattern is: " + i + " and regexFlag is: " + s;
                    return Vr.error(n), !1
                }
            }
            var r = function(i) {
                return new RegExp(t, s).exec(e) || new RegExp(t, s).exec(i(e)) || Dr.matchRelativeUrl(e, t, s, i)
            };
            let o = Rr,
                a = !1;
            390187 == window._vwo_acc_id && (a = !0), a && (o = xr);
            var l = r(o);
            return l && !a ? (o = Pr, i && r(o) || l) : l
        }
        matchRelativeUrl(e, t, i, n) {
            if (0 === e.indexOf("http")) return !1;
            const s = (new br).toAbsURL(e);
            var r = new RegExp(t, i).exec(s);
            return n && !r && (r = new RegExp(t, i).exec(n(s))), !!r
        }
        matchWildcard(e, t, i) {
            if ("string" != typeof e || "string" != typeof t) return !1;
            const n = new br;
            var s = n.isQueryParamPresent(t),
                r = n.isHashPresent(t),
                o = n.isQueryParamPresent(e),
                a = n.isHashPresent(e);
            s || (o && a ? e = e.replace(/^(.*?)(\?[^#]*)(#?.*)$/, "$1$3") : o && !a && (e = e.replace(/^(.*)(\?.*)$/, "$1"))), r || a && (e = e.replace(/^(.*?)(#.*)$/, "$1")), "/" !== e && (e = Dr.stripSlashes(e, o, a)), "/" !== t && (t = Dr.stripSlashes(t, s, r));
            var l, d, c = new RegExp("^" + Dr.regexEscape(t).replace(/\\\*/g, "(.*)") + "$", "gi");
            return c.test(e) ? (c = new RegExp("^" + Dr.regexEscape(t).replace(/\\\*/g, "(.*)") + "$", "gi"), !i || c.exec(e)) : (e = Dr.removeWWW(e), t = Dr.removeWWW(t, !0), (c = new RegExp("^" + Dr.regexEscape(t).replace(/\\\*/g, "(.*)") + "$", "gi")).test(e) ? (c = new RegExp("^" + Dr.regexEscape(t).replace(/\\\*/g, "(.*)") + "$", "gi"), !i || c.exec(e)) : (l = Dr.cleanURL(t, !0), -1 === t.indexOf("*") && ((d = Dr.removeWWW(n.toAbsURL(e)).replace(/\/$/, "").replace(/\/\?/, "?")) === t || d === l) || (e = Dr.cleanURL(e), t = l, !!(c = new RegExp("^" + Dr.regexEscape(t).replace(/\\\*/g, "(.*)") + "$", "gi")).test(e) && (c = new RegExp("^" + Dr.regexEscape(t).replace(/\\\*/g, "(.*)") + "$", "gi"), !i || c.exec(e)))))
        }
    }
    const Wr = new Ur;
    window.VWO.modules.vwoUtils.url = Wr, window.VWO._.matchRegex = Wr.matchRegex;
    class Mr {
        verifyUrl(e, t, i, n) {
            let s = !1;
            const r = n ? e : this.getCleanedUrl(e);
            if (t)
                if (n) s = !!Wr.matchRegex(r, t, null, n);
                else {
                    const i = this.getCleanedUrl(e, !0);
                    s = !(!Wr.matchRegex(r, t, null, n) && !Wr.matchRegex(i, t, !0, n))
                }
            else s = Wr.matchWildcard(r, i) || Wr.matchWildcard(e, i);
            return s
        }
        getCleanedUrl(e, t) {
            if (!e) return;
            let i;
            return -1 !== e.search(/_vis_(test_id|hash|opt_(preview_combination|random))=[a-z\.\d,]+&?/) ? (i = e.replace(/_vis_(test_id|hash|opt_(preview_combination|random))=[a-z\.\d,]+&?/g, ""), i = t ? i.replace(/(\??&?)$/, "") : i.replace(/(\/?\??&?)$/, "")) : i = t ? e : e.replace(/\/$/, ""), i
        }
        compareUrlWithIncludeExcludeRegex(e, t, i, n) {
            const s = {};
            return i && Wr.matchRegex(e, i) ? (s.didMatch = !1, s.reason = 1, s) : (s.didMatch = this.verifyUrl(e, t, n), s.reason = s.didMatch ? 2 : 3, s)
        }
    }
    const kr = new Mr;
    class Gr extends Mr {
        compareUrlWithIncludeExcludeRegex(e, t, n, s) {
            return i(this, void 0, void 0, (function*() {
                return window.fetcher.getValue('VWO.modules.utils.urlUtils.compareUrlWithIncludeExcludeRegex("${{1}}", "${{2}}", "${{3}}", "${{4}}")', null, {
                    captureGroups: [e, t, n, s]
                })
            }))
        }
    }
    const Fr = new Gr;
    window.VWO.modules.utils.urlUtils = Fr;
    const jr = {
            cb: "combi",
            gl: "goal",
            ex: "exclude",
            ud: "uuid",
            sp: "split"
        },
        Hr = {},
        $r = (e, t = window.VWO._.cookies.setItem) => {
            try {
                const i = e.currentUrl,
                    n = new URL(i).searchParams.get(Es.VWO_COOKIE_QUERY_PARAM);
                if (n) {
                    const e = (e, i) => {
                            Hr[e] || (t(e, i), Hr[e] = !0)
                        },
                        i = decodeURIComponent(n).split("|");
                    for (const t of i) {
                        const [i, n] = t.split("="), [s, r, o] = i.split("_");
                        if (jr[s]) {
                            const t = jr[s];
                            if ("uuid" === t) e(`_vwo_uuid_${r}`, n);
                            else {
                                const i = `_vis_opt_exp_${r}_${t}`;
                                "goal" === t && o ? o.split(",").forEach((t => {
                                    e(`${i}_${t}`, n)
                                })) : e(i, n)
                            }
                        }
                    }
                }
            } catch (e) {}
        };

    function Br(e, t) {
        window.fetcher.getValue('VWO.modules.utils.helperFunctions.onUrlChange("${{1}}", "${{2}}")', null, {
            captureGroups: [e, t]
        })
    }

    function Kr(e, t) {
        let i, n = !1;
        return function(...s) {
            n && (clearTimeout(i), i = null), i = setTimeout((function() {
                e.apply(null, s)
            }), t), n = !0
        }
    }
    const zr = {};
    const Yr = function(e) {
        return window.VWO.phoenix.validateTrigger(e, null, {
            skipEventProps: !0,
            skipValidationIfTimer: !0
        })
    };
    class Jr extends Gs {
        constructor() {
            super(...arguments), this.loadScriptLoadedScripts = {}, this.thirdPartyCookiesSuccess = {}, this.urlCache = {}, this.additionalStyle = window._vwo_acc_id > 742099 || 718480 === window._vwo_acc_id ? "-webkit-transform:none;-ms-transform:none;transform:none;" : "", this.hideElExpression = `{opacity:0 !important;filter:alpha(opacity=0) !important;background:none !important;${this.additionalStyle}}`
        }
        isGloballyOptedOut(e) {
            return !!parseInt(e.storages.storages.cookies.get(Es.GLOBAL_OPT_OUT, !0), 10)
        }
        firePageViewEvent(e = {}) {
            return i(this, void 0, void 0, (function*() {
                const t = window.VWO.phoenix,
                    i = window.VWO.phoenix.store.getters;
                yield window.VWO._.session_init_complete, t.trigger(_.START_APPLY_CHANGES), or(t, _.PAGE_VIEW, {
                    name: _.PAGE_VIEW,
                    time: +new Date,
                    props: {
                        url: i.currentUrl
                    }
                }, (() => {
                    t.trigger(_.END_APPLY_CHANGES), t.trigger(_.AFTER_SAMPLING_TRIGGER), window.VWO._.phoenixMT.trigger(_.END_APPLY_CHANGES), e.shouldFireDomLoad && t.trigger(_.DOM_CONTENTLOADED)
                }))
            }))
        }
        fireAuxiliaryPageView() {
            if (window.VWO._.auxPageViewFired) return;
            window.VWO._.auxPageViewFired = !0;
            const e = window.VWO.phoenix,
                t = window.VWO.phoenix.store.getters;
            or(e, _.PAGE_VIEW, {
                name: _.PAGE_VIEW,
                time: +new Date,
                aux: !0,
                ins: !0,
                props: {
                    url: t.currentUrl
                }
            })
        }
        doNotTrack(e) {
            if (e.settings.vwoData.dntEnabled) return "yes" === e.navigator.doNotTrack || "1" == e.navigator.doNotTrack || "1" == e.navigator.msDoNotTrack || "1" == e.doNotTrack
        }
        _optOut(e, t) {
            return mr(e) ? (t.trigger(_.OPT_OUT, {
                oldArgs: [!0]
            }), !0) : (t.trigger(_.OPT_OUT, {
                oldArgs: [!1]
            }), !1)
        }
        setCampaignIds(e) {
            return i(this, void 0, void 0, (function*() {
                this.otherSide('setCampaignIds("${{1}}", "${{2}}")', null, [e]);
                const {
                    actions: t,
                    getters: i
                } = vr.phoenix.store, n = i.vwoInternalProperties.experimentIds || [];
                e.forEach((e => n.push(+e))), t.addValues({
                    experimentIds: n
                }, "vwoInternalProperties")
            }))
        }
        isCustomTrigger(e) {
            var t;
            return null === (t = e.cnds) || void 0 === t ? void 0 : t.some((function(e) {
                var t;
                return "object" == typeof e && null !== e && (null === (t = e.filters) || void 0 === t ? void 0 : t.some((function(e) {
                    return e instanceof Array && e.indexOf("exec") >= 0
                })))
            }))
        }
        addListener(e, t, i, n) {
            if (!i || !Object.keys(i).length) return;
            let s;
            const r = i.eventName,
                o = i.triggerID,
                a = i.trigger;
            if (o) {
                const t = e.currentSettings.triggers[o];
                s = t && this.isCustomTrigger(t) ? `trigger.${o}` : t
            }
            if (s = s || r || a, s) {
                return [s, t.on(s, n)]
            }
        }
        extraData2(e) {
            return i(this, void 0, void 0, (function*() {
                return this.otherSide('extraData2("${{1}}")', null, [e])
            }))
        }
        isBotScreen() {
            return i(this, void 0, void 0, (function*() {
                return this.otherSide("isBotScreen")
            }))
        }
        shouldTrackUserForCampaign(e) {
            return "number" == typeof e && (e = window._vwo_exp[e]), !e || !window._vwo_code || !window._vwo_code[ts] && !window._vwo_code[is] || (this.isDomIndependentCampaign(e.type) || this.isSplitVariation(e))
        }
        createCookie(e, t, n, s, r) {
            return i(this, void 0, void 0, (function*() {
                const i = (e = e || vr.phoenix.store.getters).storages.storages.cookies;
                this.shouldTrackUserForCampaign(r) && (r && r.multiple_domains ? yield i.createThirdParty(t, n, s, void 0, r.id, void 0, r): i.create(t, n, s))
            }))
        }
        createUUIDCookie2(e, t) {
            return i(this, void 0, void 0, (function*() {
                if (_r.isVisitorOptedOut()) return;
                e = e || window.VWO.phoenix.store.getters;
                const i = yield this.getUUID(e, t), n = t && t.id && t.multiple_domains ? "_" + t.id : "";
                return (yield e.storages.storages.cookies.get("_vwo_uuid" + n)) || (yield this.createCookie(e, "_vwo_uuid" + n, i, Es.UUID_COOKIE_EXPIRY, t)), vr.data = vr.data || {}, vr.data.vin = vr.data.vin || {}, vr.data.vin.uuid = i, yield window.fetcher.setValue("VWO.data.vin", vr.data.vin), i
            }))
        }
        getUUID(e, t) {
            e = e || vr.phoenix.store.getters, t = t || {}, this.uuid = e.vwoUUID;
            const i = t && t.id && t.multiple_domains && e.storages.storages.cookies.get("_vwo_uuid_" + t.id) || e.storages.storages.cookies.get("_vwo_uuid");
            return this.uuid = i || this.uuid || this.generateUUID(), this.uuid
        }
        getSplitDecision(e, t) {
            return window._vis_debug || ys() ? this.otherSide("getSplitDecision", [t]) : e.storages.storages.cookies.get("_vis_opt_exp_" + t + "_split")
        }
        loadScript(e, t, i, n = {
            allowReload: !1
        }) {
            this.loadScriptLoadedScripts[e] ? null == t || t() : (n.allowReload || (this.loadScriptLoadedScripts[e] = 1), window.fetcher.getValue('VWO.modules.utils.loadScript("${{1}}", "${{2}}", "${{3}}", "${{4}}")', null, {
                captureGroups: [e, t, i, n]
            }))
        }
        checkForWrongConsent(e, t) {
            return t && "http:" === e.location.protocol
        }
        setThirdPartyCookiesForApplicableCamps(e) {
            const t = (t, i) => {
                const n = "_vwo_uuid_" === t.substring(0, 10) ? Es.UUID_COOKIE_EXPIRY : 100;
                this.createCookie(e, t, i, n)
            };
            if (E(e.settings.crossDomain) && !e.flags.cookieLessModeEnabled) {
                const i = Object.keys(e.settings.crossDomain);
                for (let n = 0; n < i.length; n++) {
                    const s = i[n];
                    if (this.thirdPartyCookiesSuccess[s]) continue;
                    const r = e.settings.crossDomain[s];
                    let o = Object.keys(r).length - 1;
                    for (; o >= 0;) {
                        const e = r[o];
                        t(e.name, e.value), o--
                    }
                    this.thirdPartyCookiesSuccess[s] = !0
                }
            }
            $r(e, t)
        }
        shouldExecuteLib({
            getters: e,
            actions: t,
            vwoEvents: n
        }) {
            return i(this, void 0, void 0, (function*() {
                const {
                    doCookiesMatter: i,
                    areCookiesDisabled: s,
                    shouldStopExecWhenSsmNotFound: r,
                    isSSApp: o
                } = window.VWO._.envUtils;
                if (i) {
                    const i = this.checkForWrongConsent(e, o);
                    if (i || r) {
                        e.vwoCode && e.vwoCode.finish(), or(n, _.SHOULD_EXECUTE_LIB_ERROR, i ? {
                            message: "SameSite;Secure enabled but visitor landed on HTTP page and thus cookies can't be created",
                            oldArgs: [void 0, void 0, void 0, void 0, void 0, void 0, 1]
                        } : {
                            message: "Visitor has been to HTTPS page when SameSite and Secure cookies were dropped",
                            oldArgs: [void 0, !0, void 0, void 0, void 0, void 0, 2]
                        });
                        const s = [{
                            shouldExecuteLib: !1,
                            destinationMT: "VWO._.shouldExecuteLib"
                        }];
                        return this.setPropertiesToBothThreads(t, s, "vwoInternalProperties")
                    }
                }
                if (this.isTrackingNotPossible) {
                    const e = [{
                        shouldExecuteLib: !this.isTrackingNotPossible,
                        destinationMT: "VWO._.shouldExecuteLib"
                    }];
                    return this.setPropertiesToBothThreads(t, e, "vwoInternalProperties")
                }
                if (!s) {
                    if (this.preview() || e.visDebug) {
                        const e = [{
                            shouldExecuteLib: !0,
                            destinationMT: "VWO._.shouldExecuteLib"
                        }];
                        return this.setPropertiesToBothThreads(t, e, "vwoInternalProperties")
                    }
                    if (this.doNotTrack(e) || this.isGloballyOptedOut(e) || this._optOut(e, n)) {
                        e.vwoCode && e.vwoCode.finish(), or(n, _.SHOULD_EXECUTE_LIB_ERROR, {
                            message: "User has opted out"
                        }), this.isTrackingNotPossible = !0, t.addValues({
                            optOut: !0
                        }, "vwoInternalProperties");
                        const i = [{
                            shouldExecuteLib: !1,
                            destinationMT: "VWO._.shouldExecuteLib"
                        }];
                        return this.setPropertiesToBothThreads(t, i, "vwoInternalProperties")
                    }
                    const i = [{
                        shouldExecuteLib: !0,
                        destinationMT: "VWO._.shouldExecuteLib"
                    }];
                    return this.setPropertiesToBothThreads(t, i, "vwoInternalProperties")
                }
                t.addValues({
                    cookiesDisabled: !0
                }, "vwoInternalProperties"), N() || or(n, _.SHOULD_EXECUTE_LIB_ERROR, {
                    message: "Cookies disabled",
                    oldArgs: [void 0, s]
                }), this.isTrackingNotPossible = !0;
                return this.setPropertiesToBothThreads(t, [{
                    shouldExecuteLib: !1,
                    destinationMT: "VWO._.shouldExecuteLib"
                }], "vwoInternalProperties")
            }))
        }
        insertCSS(e, t) {
            this.otherSide("insertCSS", [e, t])
        }
        unhideCampaignLevelStyle(e) {
            window.fetcher.getValue("VWO.modules.utils.libUtils.removeCampaignLevelStyleTag", [e])
        }
        setXpathAndHideEl(e, t, n, s = !0) {
            const r = this;
            let o, a, l = "";
            const d = [e.id],
                c = this.isPersonalizeCampaign(e),
                u = e.sen;
            let {
                visibilityServiceCache: g
            } = window.VWO._;
            if (e.id)
                for (o = d.length; o--;) {
                    a = d[o], g = g || {};
                    const v = g[a] && g[a].xpath || {},
                        h = g[a] && g[a].cpath || {},
                        p = Object.assign(v, h);
                    let {
                        selector: w,
                        selectorPerVariation: E,
                        cPathSelector: f,
                        cPathSelectorPerVariation: _
                    } = Object.keys(p).length > 0 ? p : this.getCampaignXPath(e);
                    if (w = w || n || "", f = f || "", _ = _ || {}, e.xPath = w, f && (e.cPath = f), window.VWO._.ac && window.VWO._.ac.PRTHD || window.VWO._.preventHidingInSPA || !s) return;
                    if (c) {
                        let n = null;
                        if (Gn) {
                            const t = ys(),
                                i = e.debug || {};
                            n = t || (i.tg ? null : i.v), n || (n = $((() => e.ss.csa)) ? null : Jn.get("_vis_opt_exp_" + a + "_combi"))
                        }
                        Object.keys(E).forEach((function(s) {
                            return i(this, void 0, void 0, (function*() {
                                const i = e.sections[1].triggers[s],
                                    o = t.currentSettings.triggers[i],
                                    l = e.sections[1].sen;
                                if ((!l || !r.doNotHideElements(l[s])) && (n == s || (yield Yr(o)))) {
                                    let e = "";
                                    E[s] && (e = E[s]), _[s] && (e += (e.length > 0 ? "," : "") + _[s]), e.length > 0 && ("," === e[e.length - 1] && (e = e.substring(0, e.length - 1)), e += r.hideElExpression, r.insertCSS("_vis_opt_path_hides_" + a + "_" + s, e))
                                }
                            }))
                        }))
                    }
                    if (this.doNotHideElements(u)) return;
                    w && (l = w), f && (l += (l.length > 0 ? "," : "") + f), l && (l += r.hideElExpression, this.insertCSS("_vis_opt_path_hides_" + a, l))
                } else l = t.vwoText || "", this.insertCSS("_vis_opt_path_hides", l)
        }
        setPropertiesToBothThreads(e, t, i) {
            let n = {};
            t.forEach((e => {
                const t = Object.keys(e)[0],
                    i = e[t];
                n = Object.assign(Object.assign({}, n), {
                    [t]: i
                }), window.fetcher.setValue(e.destinationMT, i)
            })), e.addValues(n, i)
        }
        checkAndInitializeClickClass() {
            if (!window.VWO.isVwoClickClassInitialized) {
                const e = window.VWO._.allSettings.dataStore.campaigns;
                let t = !1;
                const i = ["ANALYZE_HEATMAP", "ANALYZE_RECORDING"];
                for (const n in e)
                    if (e[n].clickmap || i.includes(e[n].type)) {
                        t = !0;
                        break
                    }
                if (t) {
                    const e = window.VWO.modules.phoenixPlugins.events.predefinedEvents;
                    window.VWO.phoenix.events.add(new e.ClickDomEvent)
                }
            }
        }
        addListenerForSessionInitComplete() {
            vr._.session_init_complete = new Promise((e => {
                const t = window.VWO.phoenix.on(_.SESSION_INIT_COMPLETE, (() => {
                    window.VWO.phoenix.off(_.SESSION_INIT_COMPLETE, t), e()
                }))
            }))
        }
        isCurrentURLSplitVariation({
            chosenVariation: e,
            getters: t,
            campaignData: i
        }) {
            if ("SPLIT_URL" !== i.type) return !1;
            let n = "";
            const s = Fr.getCleanedUrl(t.currentUrl),
                r = this.urlCache[i.id] = this.urlCache[i.id] || {},
                o = r[s] || r[t.currentUrl];
            if (void 0 !== o) return o;
            let a = !1,
                l = t.currentUrl;
            const d = i.sections;
            return d[1].variationsRegex ? (n = d[1].variationsRegex[e], a = Fr.verifyUrl(t.currentUrl, n, null)) : (n = d[1].variations[e], a = Wr.matchWildcard(s, n), l = s), r[l] = !!a
        }
        variationShownCallback(e) {
            var t;
            const {
                id: i,
                variation: n
            } = e, s = window.VWO.phoenix;
            switch (null === (t = window._vwo_exp[i]) || void 0 === t ? void 0 : t.type) {
                case "VISUAL_AB":
                case "VISUAL":
                    s.trigger(_.VARIATION_SHOWN_SENT, {
                        oldArgs: ["" + i, n]
                    });
                    break;
                case "SPLIT_URL":
                    1 == n && s.trigger(_.VARIATION_SHOWN_SENT, {
                        oldArgs: ["" + i, n]
                    })
            }
        }
    }
    const Xr = new Jr;
    var qr;
    window.VWO.modules.utils.libUtils = Xr,
        function(e) {
            e.ANALYSIS = "r", e.ANALYZE_FORM = "a", e.ANALYZE_HEATMAP = "a", e.ANALYZE_RECORDING = "a", e.FUNNEL = "t", e.SURVEY = "s", e.TRACK = "t", e.INSIGHTS_FUNNEL = "t", e.INSIGHTS_METRIC = "t"
        }(qr || (qr = {}));
    const Qr = function(e, t) {
            let i = 0;
            for (let t = e.length - 1; t >= 0; t--) i += e.charCodeAt(t);
            let n = i + t;
            for (let e = 0; e < 19; e++) n = (9301 * n + 49297) % 233280;
            return n / 233280
        },
        Zr = function(e, t) {
            return e / (t / 100)
        },
        eo = {
            getRandom: function(e, t) {
                return window.VWO._.allSettings.dataStore.CIF ? Qr(e, t) : Math.random()
            },
            getRandomForVariation: function(e, t) {
                return e && t && window.VWO._.allSettings.dataStore.CIF ? Zr(e, t) : Math.random()
            }
        };
    class to extends ms {
        constructor() {
            super(...arguments), this.campaignsInternalMap = window.VWO._.campaignsInternalMap = {}
        }
        markGoalTriggered(e, t, n) {
            var s, r;
            return i(this, void 0, void 0, (function*() {
                if ("TRACK" === e.type) yield null === (r = (s = window.tracklib).markGoalTriggered) || void 0 === r ? void 0 : r.call(s, e.id, t);
                else if ("INSIGHTS_METRIC" === e.type) e.goals[t].mca || window.VWO._.insightsUtils.includeInsightsMetric(e.id);
                else {
                    let i = n.storages.storages.cookies.get("_vis_opt_exp_" + e.id + "_goal_" + t);
                    if (e.mE) {
                        const t = n.storages.storages.cookies.get("_vis_opt_exp_" + e.id + "_combi");
                        i = this.updateGoalCookieValueForExperience(i, t)
                    } else e.goals[t].mca && i && (i = +i + 1);
                    yield Xr.createCookie(n, "_vis_opt_exp_" + e.id + "_goal_" + t, String(null != i ? i : 1), 100, e)
                }
            }))
        }
        isGoalTriggered(e, t, n) {
            var s, r;
            return i(this, void 0, void 0, (function*() {
                if ("TRACK" === e.type) {
                    return !(yield null === (r = (s = window.tracklib).shouldTriggerGoal) || void 0 === r ? void 0 : r.call(s, e.id, t))
                }
                if (e.goals[t].mca) return null;
                if ("INSIGHTS_METRIC" === e.type) return window.VWO._.insightsUtils.isMetircTriggered(e.id);
                const i = n.storages.storages.cookies.get("_vis_opt_exp_" + e.id + "_goal_" + t);
                if (e.mE) {
                    const t = n.storages.storages.cookies.get("_vis_opt_exp_" + e.id + "_combi");
                    return this.isGoalTriggeredForExperience(i, t)
                }
                return i
            }))
        }
        clearTimeouts(e) {
            return i(this, void 0, void 0, (function*() {
                yield this.otherSide("clearTimeoutsHandler", [e]), delete e.timeout
            }))
        }
        clearTimerAfterTimeout(e, t) {
            return i(this, void 0, void 0, (function*() {
                clearTimeout(e[ds]), e[ds] = setTimeout((() => {
                    this.clearTimeouts(e)
                }), (yield t.vwoInternalProperties.SPA_ELEMENT_WAIT_TIMEOUT) || 2e3)
            }))
        }
        getTrackGoalIdFromExp(e, t) {
            return _n(t.settings.campaigns[e].goals)[0]
        }
        isExcluded(e, t) {
            const i = window.tracklib,
                n = window.VWO._.insightsUtils;
            return "TRACK" === t.type ? i.isGoalExcluded(this.getTrackGoalIdFromExp(t.id, e)) : "FUNNEL" === t.type ? i.isFunnelExcluded(t.id) : "INSIGHTS_FUNNEL" === t.type ? n.isFunnelExcluded(t.id) : Xr.isAnalyzeCampaign(t.type) ? i.isAnalyzeCampaignExcluded(t.id) : !!e.storages.storages.cookies.get("_vis_opt_exp_" + t.id + "_exclude")
        }
        getCombi(e, t, n) {
            return i(this, void 0, void 0, (function*() {
                e = e || vr.phoenix.store.getters;
                const i = window.tracklib,
                    s = window.VWO._.insightsUtils;
                if ("TRACK" === t.type) return (null == i ? void 0 : i.isGoalIncluded) ? yield i.isGoalIncluded(this.getTrackGoalIdFromExp(t.id, e)): void(n || (yield window.fetcher.getValue('VWO.push("${{1}}")', null, {
                    captureGroups: [
                        ["track.delayedGoalConversion", {
                            campaignId: t.id,
                            type: "TRACK",
                            goalId: this.getTrackGoalIdFromExp(t.id, e)
                        }]
                    ]
                })));
                if ("FUNNEL" === t.type) return (null == i ? void 0 : i.isFunnelIncluded) ? yield i.isFunnelIncluded(t.id): void(n || (yield window.fetcher.getValue('VWO.push("${{1}}")', null, {
                    captureGroups: [
                        ["track.delayedGoalConversion", {
                            campaignId: t.id,
                            type: "FUNNEL"
                        }]
                    ]
                })));
                if ("INSIGHTS_FUNNEL" === t.type) return yield s.isFunnelIncluded(t.id);
                if (Xr.hasInsightsMetric(t.type)) {
                    if (t.ready) return window.VWO._.insightsUtils.isVisBucketedForTrack()
                } else if (null == Xr ? void 0 : Xr.isAnalyzeCampaign(t.type)) return i.isAnalyzeCampaignIncluded ? yield i.isAnalyzeCampaignIncluded(t.id): void(n || (yield window.fetcher.getValue('VWO.push("${{1}}")', null, {
                    captureGroups: [
                        ["track.delayedGoalConversion", {
                            campaignId: t.id,
                            type: t.type
                        }]
                    ]
                })));
                return this.getCombiCookie(e, t.id)
            }))
        }
        getCombiCookie(e, t) {
            return i(this, void 0, void 0, (function*() {
                return (e.visDebug || ys()) && (yield window.VWO._.previewDebuggerBooted), e.storages.storages.cookies.get("_vis_opt_exp_" + t + "_combi")
            }))
        }
        getCombiCookieFromMT(e) {
            return this.otherSide("getCombiCookie", [e])
        }
        exclude(e, t) {
            return i(this, void 0, void 0, (function*() {
                if ("INSIGHTS_METRIC" === t.type) return;
                const i = window.tracklib,
                    n = window.VWO._.insightsUtils;
                "TRACK" === t.type ? yield i.excludeGoal(this.getTrackGoalIdFromExp(t.id, e)): "FUNNEL" === t.type ? yield i.excludeFunnel(t.id): "INSIGHTS_FUNNEL" === t.type ? yield n.excludeFunnel(t.id): Xr.isAnalyzeCampaign(t.type) ? yield i.excludeAnalyzeCampaign(t.id): yield Xr.createCookie(e, "_vis_opt_exp_" + t.id + "_exclude", "1", 100, t)
            }))
        }
        include(e, t, n, s) {
            return i(this, void 0, void 0, (function*() {
                if ("INSIGHTS_METRIC" === s.type) return;
                let i = !1;
                const r = window.tracklib,
                    o = window.VWO._.insightsUtils;
                return "TRACK" === s.type ? yield r.includeGoal(this.getTrackGoalIdFromExp(s.id, e)): "FUNNEL" === s.type ? yield r.includeFunnel(t): "INSIGHTS_FUNNEL" === s.type ? yield o.includeFunnel(t): Xr.isAnalyzeCampaign(s.type) ? yield r.includeAnalyzeCampaign(t): (yield Xr.createCookie(e, "_vis_opt_exp_" + t + "_combi", n, 100, s), i = !0), {
                    isCookieCreated: i
                }
            }))
        }
        isLogged(e, t, i) {
            if (i || !window.VWO._.allSettings.dataStore.campaigns[t].mE) return e.storages.storages.cookies.get("_vis_opt_exp_" + t + "_combi_choose")
        }
        isBucketed(e, t, n) {
            return i(this, void 0, void 0, (function*() {
                return !!(yield this.getCombi(e, t, n))
            }))
        }
        shouldBucket(e, t) {
            return i(this, void 0, void 0, (function*() {
                const i = t.type,
                    n = window.tracklib,
                    s = void 0 !== vr.data.pc[qr[i]],
                    r = window.VWO._.insightsUtils;
                let o, a = t.pc_traffic;
                if (a = void 0 === a ? 100 : a, !a) return !1;
                this.campaignsInternalMap[t.id] && this.campaignsInternalMap[t.id].r ? o = this.campaignsInternalMap[t.id].r : (this.campaignsInternalMap[t.id] = {}, o = this.campaignsInternalMap[t.id].r = eo.getRandom(e.vwoUUID, t.id));
                const l = Xr.isSessionBasedCampaign2(t) ? yield vr._.sessionInfoService.getPcTraffic(): 100 * o;
                let d = n.isFeatureBucketed && Number(yield n.isFeatureBucketed(qr[i]));
                return Xr.hasInsightsMetric(i) && (d = Number(yield r.isVisBucketedForTrack())), (!s || d) && l <= a
            }))
        }
        chooseCombination(e, t, n) {
            var s, r, o;
            return i(this, void 0, void 0, (function*() {
                let i;
                if (_r.isVisitorOptedOut()) return;
                const a = t && t.id && (t.combination_chosen || t.cc);
                if (t && Xr.isPersonalizeCampaign(t)) {
                    if (a && "0" !== a && !t.debug) return a
                } else if (a && "0" !== a) return a;
                if (window.chooseCombinationPersonalisation && window.vwoPersonalisationCampaigns && window.vwoPersonalisationCampaigns.indexOf && -1 !== window.vwoPersonalisationCampaigns.indexOf(+t.id) && (i = window.chooseCombinationPersonalisation(t.id), 0 != i)) {
                    return "" + (parseInt(i.split(":")[1], 10) + 1)
                }
                const l = !!window.VWO._.allSettings.dataStore.CIF,
                    d = n && n.scaleInfo || t.combs,
                    c = {},
                    u = {};
                let g, v, h, p = eo.getRandomForVariation(null === (s = this.campaignsInternalMap[null == t ? void 0 : t.id]) || void 0 === s ? void 0 : s.r, null == t ? void 0 : t.pc_traffic),
                    w = l ? _n(d).sort() : _n(d),
                    E = w.length,
                    f = 0,
                    _ = !1,
                    m = !1;
                for (bn() && Xr.isPersonalizeCampaign(t) && (yield window.VWO._.syncEventsCallCompleted), h = 0; h < E; h++) {
                    i = w[h];
                    const s = n ? i : t.id;
                    if (v = e.settings.campaigns[s].type, !isNaN(parseFloat(d[i])) && 0 != d[i])
                        if ("VISUAL_AB" === v || "SPLIT_URL" === v) {
                            g = n ? n.segmentInfo : t.sections[1].triggers;
                            const s = g[i];
                            0 === g.length || !1 === (null === (r = e.currentSettings.triggers[s]) || void 0 === r ? void 0 : r.cnds[0]) ? (m = !0, u[i] = d[i]) : (n ? yield window.VWO.phoenix.validateTrigger(e.currentSettings.triggers[g[i]], null, {
                                skipEventProps: !0,
                                skipValidationIfTimer: !0
                            }): !bn() && e.triggers[s] && "boolean" == typeof e.triggers[s].state ? e.triggers[s].state : yield window.VWO.phoenix.validateTrigger(e.currentSettings.triggers[s], bn() ? {
                                useUnsavedEvents: !0
                            } : null)) && (1 != g[i] && (_ = !0), c[i] = f + d[i], f += d[i])
                        } else c[i] = f + d[i], f += d[i]
                }
                if (!_ && m)
                    for (w = _n(u), E = w.length, h = 0; h < E; h++) i = w[h], c[i] = f + u[i], f += u[i];
                let O = -1;
                const S = null === (o = null == t ? void 0 : t.sections[1]) || void 0 === o ? void 0 : o.priority;
                if (S)
                    for (E = S.length, h = 0; h < E; h++)
                        if (c[S[h]]) {
                            O = h;
                            break
                        }
                if (O >= 0) return S[O] + "";
                for (0 < f && 1 !== f && (p *= f), w = l ? _n(c).sort() : _n(c), E = w.length, h = 0; h < E; h++)
                    if (i = w[h], !isNaN(parseFloat(d[i])) && p <= c[i]) return i
            }))
        }
        getGroupBasedCampaigns({
            getters: e
        }) {
            var t, i;
            let n = [],
                s = null === (i = null === (t = e.settings.vwoData) || void 0 === t ? void 0 : t.gC) || void 0 === i ? void 0 : i.map((e => e.c));
            s = s || [];
            for (const e of s) n = n.concat(e);
            return n.map((e => "" + e))
        }
        firePatternMatchingEvent(e, t, i, n, s, r) {
            1 === i ? e.trigger(_.EXCLUDE_URL, {
                oldArgs: [t]
            }) : e.trigger(_.MATCH_WILDCARD, {
                oldArgs: [t, n, s, r]
            })
        }
        doExperimentHere(e, t, i, n = {}) {
            let s;
            if (t.pg_config) {
                const n = t.pg_config[0];
                s = i.pageGroup.validatePage(n, null, e.currentUrl)
            } else s = kr.compareUrlWithIncludeExcludeRegex(e.currentUrl, n.urlRegex || t.urlRegex, n.excludeUrl || t.exclude_url, n.urlPattern || t.url_pattern);
            return [s.didMatch, s.reason]
        }
        checkForVariationTargeting(e) {
            const t = e.sections[1].triggers;
            return !(!t || 0 === t.length)
        }
        executeCampaignJS(e, t) {
            const i = e.id,
                n = e.combination_chosen || e.cc,
                s = `${t}Executed`;
            if (e.globalCode[t] && !e.globalCode[s]) try {
                window.VWO.phoenix.settings.currentSettings.tags[e.globalCode[t]].fn(i, n), e.globalCode[s] = !0
            } catch (e) {}
        }
    }
    const io = new to;
    window.VWO.modules.utils.campaignUtils = io;
    class no {
        otherSide(...e) {
            e[0] = "VWO.modules.tags.checkEnvironment.utils." + e[0], window.fetcher.getValue(...e)
        }
    }
    class so extends no {
        checkCookieJarMismatch({
            getters: e,
            vwoEvents: t
        }) {
            const i = e.storages.storages,
                n = i.localStorageService,
                s = i.cookies;
            let r;
            !(r = s.get(Es.COOKIE_JAR, !0)) && !(r = n.get(Es.COOKIE_JAR)) || e.vwoInternalProperties.jar || (s.create(Es.COOKIE_JAR, r, !1), n.remove(Es.COOKIE_JAR), t.trigger(_.ERROR, {
                error: `_vwo(value = ${r}) cookie found but Cookie jar is not created. Deleting it.`
            }))
        }
        checkForTimeout({
            getters: e,
            actions: t,
            vwoEvents: i
        }) {
            if (e.vwoCode && window.vwoCodeEndBeforeVA && !e.vwoCode[is]) {
                i.trigger(_.SHOULD_EXECUTE_LIB_ERROR, {
                    message: "SSM cookie found",
                    oldArgs: [null, void 0, void 0, void 0, !0]
                });
                const e = [{
                    [ts]: 1,
                    destinationMT: "window._vwo_code.lT"
                }];
                Xr.setPropertiesToBothThreads(t, e, "vwoCode")
            }
        }
        addDomListenerForTimeout(e) {
            this.otherSide('addDomReadyListener("${{1}}")', null, {
                captureGroups: [function() {
                    window.VWO.phoenix.store.getters.vwoInternalProperties.willRedirectionOccur || window.fetcher.getValue("window._removeVwoGlobalStyle");
                    for (const t in window.VWO.phoenix.store.getters.settings.campaigns) {
                        const i = window.VWO.phoenix.store.getters.settings.campaigns[t];
                        i && "object" == typeof i && (i.dontKillTimer ? io.clearTimerAfterTimeout(i, e) : setTimeout((() => {
                            io.clearTimeouts(i)
                        }), 500))
                    }
                    window.VWO._.coreLib.finished = 1, window.vwo_libExecuted = !0, window.fetcher.setValue("vwo_libExecuted", !0)
                }]
            })
        }
    }
    var ro = new so;
    class oo {
        migrateCookiesToSSM({
            getters: e
        }) {
            const t = _s(),
                i = e.storages.storages.cookies,
                n = i.getAll(),
                s = Es.SAME_SITE;
            if (e.vwoInternalProperties.ss && !n[s]) {
                const e = ws + Es.UUID_V2;
                for (const s in n) {
                    if (s === e) {
                        if (Ln) continue;
                        i.create(s, "", -1, void 0, void 0, !0, !0)
                    }
                    const r = s.indexOf(ps) >= 0 || s.indexOf(ws) >= 0;
                    r && (Object.prototype.hasOwnProperty.call(t, s) ? i.create(s, decodeURIComponent(n[s]), t[s], void 0, void 0, !0) : i.create(s, decodeURIComponent(n[s]), Es.DEFAULT_EXPIRY, void 0, void 0, !0))
                }
                i.create(Es.SAME_SITE, 1, t[Es.SAME_SITE], void 0, void 0, !0)
            }
        }
        unhideIfNoCampaignsRunning({
            getters: e
        }) {
            const t = e.settings.campaigns;
            0 !== Object.keys(t).length || t.constructor
        }
        mergeThirdPartyCookiesInFirstPartyJar({
            getters: e,
            vwoEvents: t
        }) {
            e.settings.vwoData.tpc && e.settings.vwoData.tpc._vwo && (e.vwoInternalProperties.jar ? e.storages.storages.cookies.mergeInFPJar() : t.trigger(_.ERROR, {
                data: `TPC._vwo (value = ${e.settings.vwoData.tpc._vwo}) value found but cookie jar not available. Value of CJ is ${e.settings.vwoData.cj}.`
            }))
        }
        setListenerForCustomAndDomEvents({
            vwoEvents: e,
            getters: t,
            data: i,
            actions: n
        }) {
            Xr.addListener(t, e, {
                eventName: "*"
            }, ((s, r) => {
                s.name = s.name || r, rr({
                    vwoEvents: e,
                    getters: t,
                    data: i,
                    actions: n,
                    event: s
                }, r)
            }))
        }
        updateGlobalOptOutCookie(e, t) {
            const i = e.storages.storages.cookies;
            t ? i._create(Es.GLOBAL_OPT_OUT, 1, Es.DEFAULT_EXPIRY) : i.erase(Es.GLOBAL_OPT_OUT)
        }
    }
    var ao = new oo;

    function lo({
        event: e,
        data: t,
        getters: i,
        actions: n,
        vwoEvents: s
    }) {
        if (Br(!1, (function() {
                s.trigger("hashchange")
            })), _r.shouldWeTrackVisitor()) {
            const e = i.storages.get("cookies");
            if (!e.get("_vis_opt_test_cookie")) {
                const t = e.get("_vis_opt_s");
                t ? e.create("_vis_opt_s", parseInt(t.split("|")[0], 10) + 1 + "|", 100) : e.create("_vis_opt_s", "1|", 100)
            }
            e.create("_vis_opt_test_cookie", 1)
        }
        ao.migrateCookiesToSSM({
            event: e,
            data: t,
            getters: i,
            actions: n,
            vwoEvents: s
        }), ao.unhideIfNoCampaignsRunning({
            event: e,
            data: t,
            getters: i,
            actions: n,
            vwoEvents: s
        }), ao.mergeThirdPartyCookiesInFirstPartyJar({
            event: e,
            data: t,
            getters: i,
            actions: n,
            vwoEvents: s
        }), ao.setListenerForCustomAndDomEvents({
            event: e,
            data: t,
            getters: i,
            actions: n,
            vwoEvents: s
        });
        const r = Object.keys(i.settings.campaigns);
        Xr.setCampaignIds(r);
        Xr.setPropertiesToBothThreads(n, [{
            willRedirectionOccur: !1,
            destinationMT: "VWO._.willRedirectionOccur"
        }], "vwoInternalProperties"), n.addValues({
            waitingForThirdPartySync: !1
        }, "vwoInternalProperties")
    }
    class co {
        otherSide(...e) {
            return e[0] = "VWO.modules.tags.setSession." + e[0], window.fetcher.getValue(...e)
        }
    }
    class uo {
        otherSide(...e) {
            return e[0] = "VWO.modules.tags.sessionInfoService." + e[0], e[2] && (e[2] = {
                captureGroups: e[2]
            }), window.fetcher.getValue(...e)
        }
    }
    let go;
    uo.LOCAL_STORAGE_SESSION_EXPIRY = 30, uo.LOCAL_STORAGE_NAME = window._vis_debug ? "debug_vwoSn" : "vwoSn", uo.ACCOUNT_ID = window._vwo_acc_id;
    class vo extends uo {
        constructor(e, t) {
            super(), go = this, this.visitorInformation = window.VWO.data.vi = window.VWO.data.vi || {}, Xr.addListener(e, t, {
                eventName: _.NEW_SESSION
            }, (function() {
                return i(this, void 0, void 0, (function*() {
                    yield go.otherSide("eraseSessionCookie()", null, [])
                }))
            })), Xr.addListener(e, t, {
                eventName: _.REDIRECT_DECISION
            }, (function(t) {
                return i(this, void 0, void 0, (function*() {
                    const i = e.allSettings.dataStore.campaigns[t.oldArgs[1]];
                    Xr.isSessionBasedCampaign2(i) && (yield go.otherSide("markPageIdSessionExpiry()"))
                }))
            }))
        }
        getSessionId() {
            return this.otherSide("getSessionId()")
        }
        shouldSendSessionInfoInCall() {
            return !0
        }
        setSNCookieValueByIndex2(e, t) {
            return this.otherSide("setSNCookieValueByIndex2", [e, t])
        }
        getPcTraffic() {
            return this.otherSide("getPcTraffic()")
        }
        setPageIdValue(e) {
            window.phoenix.store.actions.addValues({
                pageId: e
            }, "vwoInternalProperties"), window.phoenix.store.actions.addValues({
                pageViewCount: e
            }, "root")
        }
    }
    class ho extends co {
        execute({
            getters: e,
            actions: t,
            vwoEvents: i
        }) {
            let n;
            window.VWO.modules.tags.sessionInfoService ? n = window.VWO.modules.tags.sessionInfoService : (n = new vo(e, i), window.VWO.modules.tags.sessionInfoService = n, t.addValues({
                sessionInfoService: n
            }, "vwoInternalProperties"))
        }
    }
    const po = new ho,
        wo = po.execute.bind(po);
    window.VWO.modules.tags.setSession = wo;
    class Eo extends es {
        constructor() {
            super(...arguments), this.checkEnvironmentInitialized = !1
        }
        execute({
            event: e,
            data: t,
            getters: n,
            actions: s,
            vwoEvents: r
        }) {
            return i(this, void 0, void 0, (function*() {
                if (!this.checkEnvironmentInitialized)
                    if (this.checkEnvironmentInitialized = !0, wo({
                            event: e,
                            data: t,
                            getters: n,
                            actions: s,
                            vwoEvents: r
                        }), lo({
                            event: e,
                            data: t,
                            getters: n,
                            actions: s,
                            vwoEvents: r
                        }), Xr.shouldExecuteLib({
                            event: e,
                            data: t,
                            getters: n,
                            actions: s,
                            vwoEvents: r
                        }), ro.checkCookieJarMismatch({
                            event: e,
                            data: t,
                            getters: n,
                            actions: s,
                            vwoEvents: r
                        }), ro.checkForTimeout({
                            event: e,
                            data: t,
                            getters: n,
                            actions: s,
                            vwoEvents: r
                        }), ro.addDomListenerForTimeout(n), n.vwoInternalProperties.shouldExecuteLib) {
                        if (Xr.setThirdPartyCookiesForApplicableCamps(n), window._vis_debug || window._vis_heatmap) {
                            const e = n.vwoInternalProperties.experimentIds,
                                t = Ts(e);
                            yield r.trigger(_.CAMPAIGN_FLOW_START, {
                                oldArgs: [t]
                            })
                        }
                        r.on(_.RUN_REVERT_TAGS, (() => {
                            window.VWO._.phoenixMT.trigger(_.RUN_REVERT_TAGS)
                        })), yield Xr.firePageViewEvent(), window.VWO._.phoenixMT.trigger(_.LOAD_SETTINGS), r.trigger(_.UPDATE_SETTINGS_CALL)
                    } else window.fetcher.getValue("window._removeVwoGlobalStyle")
            }))
        }
    }
    const fo = new Eo,
        _o = fo.execute.bind(fo);
    window.VWO.modules.tags.checkEnvironment.fn = fo;
    const mo = function(...e) {
        return e[0] = "VWO.modules.tags.visibilityService." + e[0], window.fetcher.getValue(...e)
    };
    let Oo = !1;

    function So({
        getters: e,
        vwoEvents: t
    }) {
        Oo || (Xr.addListener(e, t, {
            eventName: _.UNHIDE_ELEMENT
        }, (function(e) {
            const {
                ruleName: t,
                campaignData: i,
                variation: n,
                rulesArr: s
            } = e;
            let r;
            i && (r = {
                id: i.id,
                variation: n
            }), window.fetcher.getValue("VWO.modules.utils.libUtils.delCSSWrapper", [{
                ruleName: t,
                campaignData: r,
                rulesArr: s
            }])
        })), Oo = !0)
    }

    function yo({
        vwoEvents: e
    }, t) {
        var i, n, s, r, o, a, l, d, c = _n(t.sections),
            u = c.length,
            g = t.type;
        if ("VISUAL_AB" === g)
            for (; u--;)
                if (i = c[u], (n = t.sections[i]).variations)
                    for (r = (s = _n(n.variations)).length, e.trigger(_.UNHIDE_SECTION, {
                            id: t.id,
                            key: i
                        }, i); r--;)
                        if (o = s[r], l = n.variations[o], n.variations[o] = l = "string" == typeof l ? l && vwo_$.parseJSON(l) : l, l)
                            for (a = l.length, e.trigger(_.UNHIDE_VARIATION, {
                                    id: t.id,
                                    key: i
                                }); a--;) {
                                const i = {
                                    ruleName: "",
                                    campaignData: t,
                                    rulesArr: []
                                };
                                d = l[a].xpath, l[a].cpath ? i.rulesArr = [d, l[a].cpath] : i.ruleName = d, lr.unhideElement(e, i)
                            } else e.trigger(_.UNHIDE_VARIATION, {
                                id: t.id,
                                key: i
                            }, i);
                        else e.trigger(_.UNHIDE_SECTION, {
                            id: t.id,
                            key: i
                        });
        else if ("VISUAL" === g)
            for (; u--;) i = c[u], d = t.sections[i].path, lr.unhideElement(e, {
                ruleName: d,
                campaignData: t
            });
        else "SPLIT_URL" === g && lr.unhideElement(e, {
            ruleName: "*",
            campaignData: t
        })
    }

    function To({
        event: e,
        data: t,
        getters: i,
        actions: n,
        vwoEvents: s
    }, r) {
        const o = [],
            a = i.settings.campaigns;
        for (const l in a) {
            const d = a[l],
                c = d.sen,
                u = d.sections[1],
                g = u.sen;
            if (Object.prototype.hasOwnProperty.call(a, l)) {
                if (c && !Xr.doNotHideElements(c)) {
                    const d = Xr.addListener(i, s, {
                        triggerID: c
                    }, (() => {
                        yo({
                            event: e,
                            data: t,
                            getters: i,
                            actions: n,
                            vwoEvents: s
                        }, a[l]), s.trigger(_.CHECK_SEGMENTATION, r)
                    }));
                    d && o.push(d)
                }
                if (g) {
                    const e = u.variations;
                    for (const t in e) {
                        const n = e[t],
                            r = g[t];
                        n && !Xr.doNotHideElements(r) && Xr.addListener(i, s, {
                            triggerID: r
                        }, (() => {
                            for (const e of n) {
                                const i = e.xpath,
                                    n = {
                                        ruleName: "",
                                        campaignData: d,
                                        variation: t
                                    };
                                e.cpath ? n.rulesArr = [i, e.cpath] : n.ruleName = i, s.trigger(_.UNHIDE_ELEMENT, n)
                            }
                        }))
                    }
                }
            }
        }
        return o
    }

    function Io({
        vwoEvents: e
    }, t, i) {
        return t.forEach((t => {
            e.off.apply(t)
        })), mo("unhideElementsAfterTimer", [i])
    }
    class Co {
        modifyTriggerConditions(e, t) {
            const i = [];
            return Array.isArray(e) ? (e.forEach((e => {
                if (Array.isArray(e)) i.push(this.modifyTriggerConditions(e, t));
                else {
                    const n = t(e);
                    i.push(n)
                }
            })), i) : e
        }
        getExitTrigger(e) {
            for (let t = 0; t < e.length; t++) {
                if (Array.isArray(e[t])) {
                    const i = this.getExitTrigger(e[t]);
                    if (i) return i
                }
                if ("object" == typeof e[t] && null !== e[t] && e[t].exitTrigger) return e[t].exitTrigger
            }
        }
    }
    var No = new Co;
    let Ao = [];
    const Vo = {
        state: {}
    };

    function bo({
        getters: e,
        vwoEvents: t,
        campaignData: i
    }) {
        if (Xr.isDomDependent(i.type) && "VISUAL" !== i.type && io.getCombi(e, i)) return t.trigger(_.CAMPAIGN_UNLOADED, {
            expId: i.id
        })
    }

    function Ro({
        getters: e,
        expId: t,
        combination: i
    }) {
        try {
            const n = window._vwo_exp[t];
            if (!Xr.isDomDependent(n.type) || "VISUAL" === n.type) return;
            const s = n.sections[1].variations[i],
                r = t => !(!e.triggers[t] || !e.triggers[t].state);
            for (let e = 0; e < s.length; e++) {
                const t = s[e].t;
                t && (Vo.state[t] = r(t))
            }
            window.fetcher.getValue('window.VWO.modules.utils.tagExecutor.updateTriggerStates("${{1}}")', null, {
                captureGroups: [Vo.state]
            })
        } catch (e) {}
    }

    function Po() {
        Ao.forEach((e => e())), Ao = []
    }

    function xo(e) {
        window.VWO.phoenix.trigger(_.TAG_EVALUATED, {
            tG: {
                [e]: !0
            }
        })
    }

    function Lo(e, t, {
        isWaitForElementEvent: i,
        campId: n,
        preventCallBackRemovalOnSpa: r,
        isCampUnloadEvent: o
    }) {
        const a = window.VWO.phoenix,
            l = window.VWO.phoenix.store.getters,
            d = `trigger.${e}`;
        if (c = e, l.triggers[c] && l.triggers[c].state && !o) return t(d);
        var c;
        a.on(d, (function() {
            try {
                t(d)
            } catch (e) {
                s({
                    msg: `Error occurred while executing "${d}" trigger`,
                    url: "triggerBasedTagExecutorWT.ts",
                    source: e
                })
            }
            a.removeEvent(d)
        })), r || Ao.push((function() {
            a.removeEvent(d)
        })), i && a.trigger(_.HTML_ELEMENT_LOADED, {
            expId: n
        })
    }
    window.VWO.modules.utils.tagExecutor = {
        attachTriggerListenersForTagExecution: Lo,
        fireTagEvaluatedEvent: xo
    };
    const Do = function(e) {
            return window.VWO.phoenix.validateTrigger(e, null, {
                skipEventProps: !0
            })
        },
        Uo = function({
            event: e,
            data: t,
            getters: n,
            actions: s,
            vwoEvents: r
        }) {
            return i(this, void 0, void 0, (function*() {
                if (n.vwoCode && (n.vwoCode[ts] || n.vwoCode[is])) return r.trigger(_.VISIBILITY_TRIGGERED), void r.trigger(_.NOT_REDIRECTING);
                const o = [],
                    {
                        visibilityServiceCache: a
                    } = window.VWO._;
                if (window.VWO._.auxPageViewFired) return;
                const l = n.settings.campaigns;
                let d = !1;
                const c = {
                        hideElementsTriggered: !1,
                        segmentCndsSatisfied: void 0
                    },
                    u = [],
                    g = io.getGroupBasedCampaigns({
                        event: e,
                        data: t,
                        getters: n,
                        actions: s,
                        vwoEvents: r
                    });
                yield Promise.all(Object.keys(l).map((t => i(this, void 0, void 0, (function*() {
                    var i, v, h, p, w, E;
                    const f = l[t],
                        m = l[t].type;
                    if ("FUNNEL" === m || f.ready) return;
                    const O = null === (i = f.triggers) || void 0 === i ? void 0 : i[0],
                        S = O && r.settings.currentSettings.triggers[O];
                    let y;
                    const T = "SPLIT_URL" === m,
                        I = "SURVEY" === m,
                        C = null !== (h = null === (v = null == a ? void 0 : a[t]) || void 0 === v ? void 0 : v.dEH) && void 0 !== h ? h : null !== (E = null === (w = null === (p = e) || void 0 === p ? void 0 : p._vwo) || void 0 === w ? void 0 : w.doExperimentHere) && void 0 !== E ? E : io.doExperimentHere(n, f, r)[0],
                        N = yield $((() => io.getCombiCookie(n, +t)));
                    if (f.exec = !!N, I && C && !window.VWO._.allSettings.dataStore.previewExtraSettings && (window.VWO.phoenix.trigger(_.LOAD_SURVEY_LIB), window.fetcher.setValue("window.VWO._.shouldLoadSurveyLib", !0)), T && C && (d = !0, S && (y = yield Do(S)) && u.push(t)), !C) return (ys() || window._vis_debug) && S && ((null != y ? y : yield Yr(S)) || (c.segmentCndsSatisfied = !1, r.trigger(_.CHECK_SEGMENTATION, c))), bo({
                        getters: n,
                        vwoEvents: r,
                        campaignData: f
                    }), void(f.isApplicable = 0);
                    o.push(t);
                    const A = -1 === g.indexOf(t) || g.indexOf(t) >= 0 && f.shouldHideElement,
                        V = An(t),
                        b = T && !V ? window.VWO._.bodyPath : void 0,
                        R = S && (null != y ? y : yield Yr(S)),
                        P = A && !f.manual;
                    if (R && P && (f.isApplicable = 1), !f.eHIR && f.cA && f.isApplicable) return;
                    if (R) {
                        if (c.segmentCndsSatisfied = !0, P ? ("DEPLOY" === f.type && (f.orgType = f.type, f.type = "VISUAL_AB"), Xr.setXpathAndHideEl(f, n, b), T || (r.trigger(_.HIDE_ELEMENTS, {
                                oldArgs: [f.id, f.cc]
                            }), c.hideElementsTriggered = !0)) : Xr.setXpathAndHideEl(f, n, b, !1), f.ss && f.ss.csa) {
                            const e = "trigger." + vs,
                                t = r.on(e, (() => {
                                    const i = window.VWO.phoenix.store.getters.triggers;
                                    i[O] && !i[O].state && bo({
                                        getters: n,
                                        vwoEvents: r,
                                        campaignData: f
                                    }), r.off(e, t)
                                }))
                        }
                    } else bo({
                        getters: n,
                        vwoEvents: r,
                        campaignData: f
                    }), c.segmentCndsSatisfied = !1, r.trigger(_.CHECK_SEGMENTATION, c);
                    const x = No.getExitTrigger(S.cnds);
                    S && x && !T && Xr.addListener(n, r, {
                        triggerID: x
                    }, (() => {
                        f.ready || ("SPLIT_URL" === m && r.trigger(_.NOT_REDIRECTING), yo({
                            event: e,
                            data: f,
                            getters: n,
                            actions: s,
                            vwoEvents: r
                        }, f), r.trigger(_.CHECK_SEGMENTATION, c))
                    })), f.debug && s.updateSettings({
                        cc: f.debug.v
                    }, `campaigns.${t}`)
                }))))), d && 0 !== u.length || r.trigger(_.NOT_REDIRECTING);
                const v = [{
                    executableCampaignsOnCurrentPage: o,
                    destinationMT: "window.VWO._.executableCampaignsOnCurrentPage"
                }];
                Xr.setPropertiesToBothThreads(s, v, "vwoInternalProperties"), window.fetcher.getValue("VWO.modules.utils.libUtils.delCSS", [{
                    ruleName: "*",
                    campaignData: t
                }]), So({
                    event: e,
                    data: t,
                    getters: n,
                    actions: s,
                    vwoEvents: r
                });
                const h = To({
                    event: e,
                    data: t,
                    getters: n,
                    actions: s,
                    vwoEvents: r
                }, c);
                Io({
                    event: e,
                    data: t,
                    getters: n,
                    actions: s,
                    vwoEvents: r
                }, h, c), r.trigger(_.RUN_REVERT_TAGS), r.trigger(_.VISIBILITY_TRIGGERED)
            }))
        };
    class Wo {}
    class Mo {}
    class ko extends Mo {
        constructor() {
            super(...arguments), this.preview = ys, this.noopFn = function() {}
        }
        isTimedOut(e) {
            return e.vwoCode && (e.vwoCode[ts] || e.vwoCode[is])
        }
        unhideElForCombinationsToNotUse(e, t, i, n) {
            if ("VISUAL_AB" !== i.type) return;
            Xr.isPersonalizeCampaign(i) && Xr.unhideCampaignLevelStyle(i.id);
            const s = i.sections[1].variations,
                r = On(s);
            for (let e = 0; e < r.length; e++)
                if (r[e] !== t) {
                    let t = s[r[e]];
                    if ("object" != typeof t && (t = JSON.parse(t)), !t) continue;
                    for (let s = 0; s < t.length; s++) {
                        const o = t[s].xpath,
                            a = {
                                ruleName: "",
                                campaignData: i,
                                variation: Xr.isPersonalizeCampaign(i) ? r[e] : null
                            };
                        t[s].cpath ? a.rulesArr = [o, t[s].cpath] : a.ruleName = o, lr.unhideElement(n, a, t[s].dHE)
                    }
                }
        }
        unhideVariationIfNotSplit(e, t, i) {
            "SPLIT_URL" !== t.type && this.unhideVariation(e, t, i)
        }
        unhideVariation(e, t, i) {
            const n = On(t.sections);
            let s, r, o, a, l, d, c, u, g = n.length;
            const v = t.type;
            if ("VISUAL_AB" === v)
                for (; g--;)
                    if (s = n[g], r = t.sections[s], r.variations)
                        for (o = On(r.variations), a = o.length, i.trigger(_.UNHIDE_SECTION, {
                                oldArgs: [t.id, s, !a]
                            }); a--;)
                            if (l = o[a], c = r.variations[l], r.variations[l] = c = "string" == typeof c ? c && JSON.parse(c) : c, c)
                                for (d = c.length, i.trigger(_.UNHIDE_VARIATION, {
                                        oldArgs: [t.id, s, l, !d]
                                    }); d--;) {
                                    u = c[d].xpath;
                                    const e = {
                                        ruleName: "",
                                        campaignData: t,
                                        key: s,
                                        oldArgs: [t.id, s, l, u]
                                    };
                                    c[d].cpath ? e.rulesArr = [u, c[d].cpath] : e.ruleName = u, lr.unhideElement(i, e, c[d].dHE)
                                } else i.trigger(_.UNHIDE_VARIATION, {
                                    oldArgs: [t.id, s, l, !c]
                                });
                            else i.trigger(_.UNHIDE_SECTION, {
                                oldArgs: [t.id, s, !0]
                            });
            else if ("VISUAL" === v)
                for (; g--;) s = n[g], u = t.sections[s].path, i.trigger(_.UNHIDE_ELEMENT, {
                    ruleName: u,
                    campaignData: t,
                    key: s,
                    oldArgs: [t.id, s, void 0, u]
                });
            else "SPLIT_URL" === v && (i.trigger(_.UNHIDE_ELEMENT, {
                ruleName: "*",
                campaignData: t,
                oldArgs: [t.id, void 0, void 0, "*"]
            }), i.trigger(_.UNHIDE_ELEMENT, {
                ruleName: "body",
                campaignData: t,
                oldArgs: [t.id, void 0, void 0, "body"]
            }))
        }
        isSplit(e, t, n) {
            return i(this, void 0, void 0, (function*() {
                if ("SPLIT_URL" === t.type && ("RUNNING" === t.status || this.preview() || window._vis_debug)) {
                    let i;
                    const s = yield Xr.getSplitDecision(e, t.id);
                    if (!s) return !1;
                    if (Xr.isCurrentURLSplitVariation({
                            chosenVariation: s,
                            getters: e,
                            campaignData: t
                        })) return i = yield io.getCombi(e, t), t.combination_chosen = s, t[ns] = 1, n.trigger(_.CHOOSE_COMBINATION, {
                        oldArgs: [t.id, s, !0]
                    }), i || this.preview() ? this.preview() || (n.trigger(_.SPLIT_VARIATION_SHOWN, {
                        oldArgs: ["" + t.id, i]
                    }), or(n, _.CAMPAIGN_READY, {
                        id: t.id,
                        combination: i
                    })) : (i = s, t && void 0 === t.isFirst && (t.isFirst = 1), yield io.include(e, t.id, i, t), n.trigger(_.REGISTER_HIT, {
                        oldArgs: ["" + t.id, i]
                    }), or(n, _.CAMPAIGN_READY, {
                        id: t.id,
                        combination: i
                    })), this.currentUrlValidationForSplitFromLs(t.id) && (n.trigger(_.VARIATION_SHOWN_SENT, {
                        oldArgs: ["" + t.id, i]
                    }), window.VWO._.phoenixMT.trigger(_.VARIATION_SHOWN_SENT, t.id)), window.fetcher.getValue("VWO.modules.utils.collectAndSendDataForGA", [t.id, i]), !0
                }
                return !1
            }))
        }
        currentUrlValidationForSplitFromLs(e) {
            var t;
            const i = JSON.parse(window.localStorage.getItem(us.VS_DATA));
            return (null === (t = null == i ? void 0 : i[e]) || void 0 === t ? void 0 : t.u) === window.location.href
        }
        checkSplitVariationAndMarkReady(e, t, n) {
            return i(this, void 0, void 0, (function*() {
                if (yield this.isSplit(e, t, n)) return n.trigger(_.NOT_REDIRECTING), lr.unhideElement(n, {
                    ruleName: "*",
                    campaignData: t,
                    oldArgs: [t.id, void 0, void 0, "*"]
                }), this.unhideVariationIfNotSplit(e, t, n), t.ready = !0, !0
            }))
        }
        getCombination(e, t, n) {
            return i(this, void 0, void 0, (function*() {
                let i = !1,
                    s = this.preview() || (t.mE ? void 0 : yield io.getCombi(e, t));
                return (s || t.combination_chosen) && (i = !0), s = s || io.isLogged(e, t.id) || !n && (yield io.chooseCombination(e, t)), {
                    alreadyChosen: i,
                    combi: s
                }
            }))
        }
        registerHit(e, t, n, s, r, o, a) {
            return i(this, void 0, void 0, (function*() {
                if (o = o || this.noopFn, !n.mE && (yield io.isBucketed(e, n)) || Xr.isBot2() || !Xr.shouldTrackUserForCampaign(n)) return o(a);
                if ("TRACK" === n.type) {
                    var i = io.getTrackGoalIdFromExp(n.id, e);
                    yield window.fetcher.getValue('VWO.push("${{1}}", "${{2}}")', null, {
                        captureGroups: [
                            ["tag", n.id, t, "session", !0],
                            ["tag", i, null, "eg"]
                        ]
                    })
                } else "INSIGHTS_FUNNEL" === n.type && (yield window.fetcher.getValue('VWO.push("${{1}}")', null, {
                    captureGroups: [
                        ["tag", `${n.id}`, null, "fIds"]
                    ]
                }));
                s.trigger(_.SEND_NEW_VISITOR_CALL, {
                    combination: t,
                    campaignData: n,
                    function() {}
                }), yield ir({
                    getters: e,
                    event: {
                        time: +new Date,
                        name: _.REGISTER_HIT,
                        props: {
                            id: "" + n.id,
                            combination: t
                        }
                    },
                    actions: {},
                    vwoEvents: {},
                    data: {}
                }, n, o, a), r || s.trigger(_.REGISTER_HIT, {
                    oldArgs: ["" + n.id, t]
                }), or(s, _.CAMPAIGN_READY, {
                    id: n.id,
                    combination: t
                })
            }))
        }
        createTempCombiCookie(e, t, n) {
            return i(this, void 0, void 0, (function*() {
                yield Xr.createCookie(e, "_vis_opt_exp_" + t.id + "_combi_choose", n, 100, t)
            }))
        }
        recordVisitor(e, t, n, s, r) {
            return i(this, void 0, void 0, (function*() {
                if (!t) return;
                const i = n.type;
                s && !(yield io.isLogged(e, n.id)) ? (yield this.registerHit(e, t, n, r), Xr.isDomDependent(i) && (yield this.createTempCombiCookie(e, n, t))) : or(r, _.CAMPAIGN_READY, {
                    id: n.id,
                    combination: t
                }), window.fetcher.getValue("VWO.modules.utils.collectAndSendDataForGA", [n.id, t])
            }))
        }
        deleteCampaignParams(e) {
            delete e.ready, delete e.timedout, delete e[ns]
        }
        setMutsData(e) {
            e.muts = e.muts || {}, e.muts.pre = e.muts.pre || {}, e.muts.post = e.muts.post || {}, e.muts.pre.enabled && (e.manual = !0)
        }
        handleDeployCampaign(e) {
            "DEPLOY" === e.type && (e.orgType = e.type, e.type = "VISUAL_AB")
        }
        handlePCTraffic(e) {
            e.pc_traffic = void 0 === e.pc_traffic ? 100 : e.pc_traffic
        }
        checkAlreadyChosenForMultiExperience(e, t, n) {
            return i(this, void 0, void 0, (function*() {
                if (t.mE) {
                    const i = (yield io.getCombi(e, t)) || io.isLogged(e, t.id, !0);
                    return n == i
                }
            }))
        }
        preProcessCampaignData({
            data: e
        }) {
            this.setMutsData(e), this.handleDeployCampaign(e), this.handlePCTraffic(e), e.globalCode = e.globalCode || {}, e.goals || (e.goals = {}), Xr.isDomIndependentCampaign(e.type) && (e.clickmap = 0), "FUNNEL" === e.type ? (e.g = e.g || e.goals, e.goals = {}, e.segment_code = void 0 === e.segment_code ? "true" : e.segment_code, e.manual = !0, e.v = e.v || 1) : e.manual = !!e.manual
        }
        processCampaign({
            data: e,
            event: t,
            actions: n,
            getters: s,
            vwoEvents: r,
            executingTagTrigger: o
        }) {
            var a, l;
            return i(this, void 0, void 0, (function*() {
                let i, d;
                const c = "SPLIT_URL" === e.type;
                if (!this.preview() && (yield io.isExcluded(s, e)) && !Xr.isSessionBasedCampaign2(e)) return r.trigger(_.UNHIDE_ALL_VARIATIONS, {
                    oldArgs: [e.id, !0, !0]
                }), c && (r.trigger(_.NOT_REDIRECTING), lr.unhideElement(r, {
                    ruleName: "*",
                    campaignData: e
                })), void this.unhideVariationIfNotSplit(s, e, r);
                if (this.deleteCampaignParams(e), c && (yield this.checkSplitVariationAndMarkReady(s, e, r))) return void r.trigger(_.UNHIDE_ALL_VARIATIONS, {
                    oldArgs: [e.id, void 0, void 0, !0]
                });
                if (!Xr.shouldTrackUserForCampaign(e)) return e.timedout = !0, c && (r.trigger(_.NOT_REDIRECTING), lr.unhideElement(r, {
                    ruleName: "*",
                    campaignData: e
                })), void this.unhideVariation(s, e, r);
                const u = io.doExperimentHere(s, e, r)[0];
                if (u) {
                    const t = (Xr.isPersonalizeCampaign(e) && e.sections[1].triggers[null === (a = e.debug) || void 0 === a ? void 0 : a.v]) === o;
                    if (r.trigger(_.SEGMENTATION_EVALUATED, {
                            oldArgs: [e.id, u, t, !0]
                        }), Xr.isDomDependent(e.type) && (i = yield this.getCombination(s, e, !0), d = i.alreadyChosen, i.combi && (e.cc = i.combi)), !this.preview() && !(yield io.isBucketed(s, e)) && !(yield io.shouldBucket(s, e))) return r.trigger(_.UNHIDE_ALL_VARIATIONS, {
                        oldArgs: [e.id, !0, void 0, void 0, !0]
                    }), yield io.exclude(s, e), "SPLIT_URL" === e.type && (r.trigger(_.NOT_REDIRECTING), lr.unhideElement(r, {
                        ruleName: "*",
                        campaignData: e
                    })), void this.unhideVariationIfNotSplit(s, e, r);
                    if (Xr.shouldTrackUserForCampaign(e) ? (e.ready = !0, e.cA = !0) : e.timedout = !0, c)
                        if (this.isTimedOut(s)) r.trigger(_.NOT_REDIRECTING), lr.unhideElement(r, {
                            ruleName: "*",
                            campaignData: e
                        });
                        else {
                            i = yield this.getCombination(s, e), d = i.alreadyChosen, e.combination_chosen = i.combi;
                            const t = [{
                                willRedirectionOccur: !0,
                                destinationMT: "VWO._.willRedirectionOccur"
                            }];
                            "1" !== e.combination_chosen && Xr.setPropertiesToBothThreads(n, t, "vwoInternalProperties");
                            const o = yield Xr.isBotScreen();
                            io.executeCampaignJS(e, "pre"), yield lr.variationShown(r, Object.assign(Object.assign({
                                id: e.id,
                                variation: e.combination_chosen,
                                isFirst: d ? 0 : 1,
                                isSplitVariation: "1" !== e.combination_chosen
                            }, o && {
                                vwoMeta: {
                                    isBot: o
                                }
                            }), {
                                fireLinkedTagSync: "FUNNEL" === e.type
                            }), e)
                        }
                    else {
                        i = yield this.getCombination(s, e);
                        const t = e.combination_chosen = i.combi;
                        if (d = i.alreadyChosen || (yield this.checkAlreadyChosenForMultiExperience(s, e, t)), delete e.cc, !t) return r.trigger(_.UNHIDE_ALL_VARIATIONS, {
                            oldArgs: [e.id, void 0, void 0, void 0, void 0, void 0, !0, Xr.isPersonalizeCampaign(e)]
                        }), this.unhideVariationIfNotSplit(s, e, r), s.vwoInternalProperties.isSpaEnabled && r.trigger(_.TEST_NOT_RUNNING, {
                            oldArgs: [e.id]
                        }), void(e.ready = !1);
                        r.trigger(_.CHOOSE_COMBINATION, {
                            oldArgs: [e.id, t, d, Xr.isPersonalizeCampaign(e) ? d : void 0]
                        }), this.recordVisitor(s, t, e, !d, r), this.unhideElForCombinationsToNotUse(s, t, e, r), "VISUAL_AB" === e.type && 1 == t && (r.trigger(_.UNHIDE_ALL_VARIATIONS, {
                            oldArgs: [e.id, void 0, void 0, void 0, void 0, void 0, void 0, !0]
                        }), this.unhideVariationIfNotSplit(s, e, r)), io.executeCampaignJS(e, "pre");
                        const n = yield Xr.isBotScreen();
                        yield lr.variationShown(r, Object.assign(Object.assign({
                            id: e.id,
                            variation: i.combi,
                            isFirst: d ? 0 : 1
                        }, n && {
                            vwoMeta: {
                                isBot: n
                            }
                        }), {
                            fireLinkedTagSync: "FUNNEL" === e.type
                        }), e)
                    }
                } else s.vwoInternalProperties.isSpaEnabled && r.trigger(_.TEST_NOT_RUNNING, {
                    oldArgs: [e.id]
                }), Xr.shouldTrackUserForCampaign(e) && (r.trigger(_.GOAL_VISIT, {
                    expId: e.id
                }), (null === (l = null == e ? void 0 : e.globalCode) || void 0 === l ? void 0 : l.pre) && r.trigger(`${_.EVALUATE_GOAL_PAGE_FOR_PREJS}_${e.id}`)), yo({
                    data: e,
                    event: t,
                    getters: s,
                    actions: n,
                    vwoEvents: r
                }, e), this.unhideVariationIfNotSplit(s, e, r)
            }))
        }
    }
    var Go = new ko;

    function Fo(e) {
        var t;
        return (ys() || window._vis_debug) && (null === (t = e.debug) || void 0 === t ? void 0 : t.v)
    }

    function jo(e, t) {
        var i, n, s, r;
        if ((null === (i = e.triggers) || void 0 === i ? void 0 : i[0]) == t) return !1;
        return (null === (r = null === (s = null === (n = e.sections) || void 0 === n ? void 0 : n[1]) || void 0 === s ? void 0 : s.triggers) || void 0 === r ? void 0 : r[e.debug.v]) !== t
    }

    function Ho(e, t) {
        return !e.ready && !(Fo(e) && Xr.isPersonalizeCampaign(e) && jo(e, t))
    }
    class $o extends Wo {
        constructor() {
            super(...arguments), this.listenerAdded = !1, this.runCampaignSync = {}
        }
        executeWrapper(e, t) {
            return i(this, void 0, void 0, (function*() {
                const n = e.data.id;
                this.runCampaignSync[n] ? this.runCampaignSync[n] = this.runCampaignSync[n].then((() => i(this, void 0, void 0, (function*() {
                    try {
                        yield this.execute(e, t)
                    } catch (e) {
                        Vr.error(e)
                    }
                })))) : this.runCampaignSync[n] = this.execute(e, t), yield this.runCampaignSync[n], delete this.runCampaignSync[n]
            }))
        }
        execute({
            event: e,
            data: t,
            actions: n,
            getters: s,
            vwoEvents: r
        }, o) {
            return i(this, void 0, void 0, (function*() {
                if (N()) return;
                "SURVEY" === t.type && (yield vr._.insightsOnConsentPromise);
                const {
                    executingTagTrigger: a
                } = e;
                if (delete e.executingTagTrigger, !Ho(t, a)) return;
                if (s = s || vr.phoenix.store.getters, n = n || vr.phoenix.store.actions, t.isTriggerValidated = !0, !o && "FUNNEL" === t.type) return;
                if (!s.vwoInternalProperties.shouldExecuteLib) return "SPLIT_URL" === t.type && r.trigger(_.NOT_REDIRECTING), void lr.unhideElement(r, {
                    ruleName: "*",
                    campaignData: t
                });
                if (s.vwoInternalProperties.willRedirectionOccur) return;
                "TRACK" === t.type && (yield this.processFunnelCampaign({
                    getters: s,
                    actions: n,
                    vwoEvents: r,
                    data: t,
                    event: e
                })), this.listenerAdded || (r.on(_.EXECUTE_FUNNEL_FOR_GOAL_CAMPAIGN, (t => i(this, void 0, void 0, (function*() {
                    yield this.processFunnelCampaign({
                        getters: s,
                        actions: n,
                        vwoEvents: r,
                        data: t.campaign,
                        event: e
                    })
                })))), this.listenerAdded = !0), Go.preProcessCampaignData({
                    event: e,
                    data: t,
                    actions: n,
                    getters: s,
                    vwoEvents: r
                });
                const l = yield io.getCombiCookie(s, t.id);
                yield window.VWO._.visibilityTriggered, t.isProcessing = 1, yield Go.processCampaign({
                    event: e,
                    data: t,
                    actions: n,
                    getters: s,
                    vwoEvents: r,
                    executingTagTrigger: a
                }), delete t.isProcessing, t.ready && !l && (e._vwo = e._vwo || {
                    campaignsConverted: []
                }, e._vwo.campaignsConverted.push(t.id)), r.trigger(_.CAMPAIGN_FLOW_END)
            }))
        }
        processFunnelCampaign({
            getters: e,
            actions: t,
            vwoEvents: n,
            data: s,
            event: r
        }) {
            return i(this, void 0, void 0, (function*() {
                if (!s.funnel) return;
                const o = yield window.fetcher.getValue("VWO._.commonCookieHandler.getDataStore()"), a = s.funnel.map((a => i(this, void 0, void 0, (function*() {
                    const i = o.split(":")[as].split(",");
                    let l, d, c, u = i.length;
                    const g = e.settings.campaigns[a.id].g[0].id === parseInt(Object.keys(s.goals)[0], os),
                        v = a.triggers[0],
                        h = yield window.VWO.phoenix.validateTrigger(window.VWO.phoenix.settings.currentSettings.triggers[v], null, {
                            triggerName: v
                        });
                    for (; u--;) l = i[u].split("_"), d = l[0], parseInt(d, os) === parseInt(a.id, os) && (c = !0);
                    h && g && !c && (yield this.execute({
                        event: r,
                        data: e.settings.campaigns[a.id],
                        actions: t,
                        getters: e,
                        vwoEvents: n
                    }, !0))
                }))));
                yield Promise.all(a)
            }))
        }
    }
    const Bo = new $o,
        Ko = Bo.executeWrapper.bind(Bo);
    class zo extends ko {
        constructor() {
            super(...arguments), this.preview = ys, this.currentCombinationXPaths = {}
        }
        checkAndApplyChanges(e, t, n, s, r) {
            return i(this, void 0, void 0, (function*() {
                if (!t || !t.ready) return;
                let i, o = !1;
                if (i = "", "SPLIT_URL" === t.type) this.runSplitCampaign(t, e, s, n);
                else {
                    if (i = this.preview() || (yield io.getCombi(e, t)), i) {
                        if (t.mE) {
                            const n = yield io.chooseCombination(e, t);
                            if (i != n && (o = !0, i = n), !i) return
                        }
                    } else if (o = !0, i = yield io.chooseCombination(e, t), !i) return;
                    n.trigger(_.ELEMENT_LOAD_TIMER_STOP, {
                        oldArgs: [t.id, i]
                    }), e.vwoInternalProperties.isSpaEnabled && void 0 === t.dontKillTimer && (t.dontKillTimer = !0), Ro({
                        getters: e,
                        expId: t.id,
                        combination: i
                    }), this.otherSide("tryApplyingChanges", [i, t, null, r]), yield this.record(e, i, t, o, n), Xr.isDomIndependentCampaign(t.type) && Xr.shouldTrackUserForCampaign(t) && n.trigger(_.GOAL_VISIT, {
                        expId: t.id
                    });
                    for (const e in this.currentCombinationXPaths) Object.prototype.hasOwnProperty.call(this.currentCombinationXPaths, e) && n.trigger(_.ELEMENT_NOT_LOADED, {
                        oldArgs: [t.id, this.currentCombinationXPaths[e][0], this.currentCombinationXPaths[e][1], e]
                    })
                }
            }))
        }
        record(e, t, n, s, r) {
            return i(this, void 0, void 0, (function*() {
                if (!s || !t) return;
                (yield io.include(e, n.id, t, n)).isCookieCreated && window.fetcher.getValue("VWO._.phoenixMT.trigger", [_.CAMPAIGN_COMBI_CREATED]), e.storages.storages.cookies.erase("_vis_opt_exp_" + n.id + "_combi_choose"), n.id && n.multiple_domains
            }))
        }
        runSplitCampaign(e, t, n, s) {
            return i(this, void 0, void 0, (function*() {
                let r;
                window._vis_debug || (r = ys() || (yield io.getCombi(t, e)) || (yield Xr.getSplitDecision(t, e.id)));
                const o = e.sections[1].variations,
                    a = t.storages.storages.cookies;
                if (r = parseInt(r, os), r) 1 === r ? (lr.unhideElement(s, {
                    ruleName: "*",
                    campaignData: e,
                    oldArgs: [e.id, void 0, void 0, "*"]
                }), s.trigger(_.NOT_REDIRECTING), s.trigger(_.UNHIDE_ALL_VARIATIONS, {
                    oldArgs: [e.id, void 0, void 0, void 0, !0]
                }), e.combination_chosen = r, s.trigger(_.CHOOSE_COMBINATION, {
                    oldArgs: [e.id, r, !0]
                }), this.recordVisitor(t, r, e, !1, s)) : (Xr.createCookie(t, "_vis_opt_exp_" + e.id + "_split", r, 100, e), Ks.set(), s.trigger(_.SPLIT_URL, {
                    oldArgs: [e.id]
                }), this.redirectToURL(t, n, e, o[r], r, s));
                else {
                    if (isNaN(r = parseInt(yield io.chooseCombination(t, e), os))) return s.trigger(_.CHOOSE_COMBINATION, {
                        oldArgs: [e.id, void 0, !1]
                    }), void s.trigger(_.UNHIDE_ALL_VARIATIONS, {
                        oldArgs: [e.id, void 0, void 0, void 0, !0]
                    });
                    if (e.multiple_domains && 1 !== r) {
                        Xr.createCookie(t, "_vis_opt_exp_" + e.id + "_split", r, 100, e), Ks.set(), s.trigger(_.SPLIT_URL, {
                            oldArgs: [e.id]
                        });
                        const l = [{
                            willRedirectionOccur: !0,
                            destinationMT: "VWO._.willRedirectionOccur"
                        }];
                        Xr.setPropertiesToBothThreads(n, l, "vwoInternalProperties"), n.addValues({
                            waitingForThirdPartySync: !0
                        }, "vwoInternalProperties"), a.waitForThirdPartySync((() => i(this, void 0, void 0, (function*() {
                            n.addValues({
                                waitingForThirdPartySync: !1
                            }, "vwoInternalProperties"), this.redirectToURL(t, n, e, o[r], r, s, !0)
                        }))))
                    } else 1 !== r ? (Xr.createCookie(t, "_vis_opt_exp_" + e.id + "_split", r, 100, e), Ks.set(), s.trigger(_.SPLIT_URL, {
                        oldArgs: [e.id]
                    }), this.redirectToURL(t, n, e, o[r], r, s, !0)) : (s.trigger(_.NOT_REDIRECTING), lr.unhideElement(s, {
                        ruleName: "*",
                        campaignData: e,
                        oldArgs: [e.id, void 0, void 0, "*"]
                    }), e.combination_chosen = r, s.trigger(_.CHOOSE_COMBINATION, {
                        oldArgs: [e.id, r, !1]
                    }), s.trigger(_.UNHIDE_ALL_VARIATIONS, {
                        oldArgs: [e.id, void 0, void 0, void 0, !0]
                    }), yield this.recordVisitor(t, "1", e, !0, s), this.record(t, "1", e, !0, s))
                }
            }))
        }
        redirectToURL(e, t, i, n, s, r, o) {
            if (this.isTimedOut(e)) return;
            Xr.setPropertiesToBothThreads(t, [{
                willRedirectionOccur: !0,
                destinationMT: "VWO._.willRedirectionOccur"
            }], "vwoInternalProperties"), r.trigger(_.REDIRECT_DECISION, {
                oldArgs: [!0, i.id]
            }), o ? this.registerHit(e, s, i, r, !0, this.processRedirect.bind(this), {
                getters: e,
                campaignData: i,
                redirectURL: n,
                isNewVisitor: o,
                context: this
            }) : this.processRedirect({
                getters: e,
                campaignData: i,
                redirectURL: n,
                isNewVisitor: o,
                context: this
            })
        }
        processRedirect({
            getters: e,
            campaignData: t,
            redirectURL: i,
            isNewVisitor: n
        }) {
            this.otherSide("processRedirect", [{
                getters: e,
                campaignData: t,
                redirectURL: i,
                isNewVisitor: n
            }])
        }
        otherSide(...e) {
            e[0] = "VWO.modules.tags.runTestCampaign.utils." + e[0], window.fetcher.getValue(...e)
        }
    }
    var Yo = new zo;
    const Jo = function({
        event: e,
        getters: t,
        actions: n,
        vwoEvents: s
    }) {
        return i(this, void 0, void 0, (function*() {
            const i = t.settings.campaigns[e.props.id];
            yield Yo.checkAndApplyChanges(t, i, s, n), io.executeCampaignJS(i, "post")
        }))
    };
    class Xo {
        serializeNonGoalData(e) {
            Xr.isDomIndependentCampaign(e.type) && (e.clickmap = 0), ("ANALYSIS" === e.type || Xr.isAnalyzeCampaign(e.type)) && (e.goals = {})
        }
        serializeData(e) {
            this.serializeNonGoalData(e);
            const t = e.goals;
            for (const e in t) Xr.isPageBasedGoal(t[e].type) ? (t[e].pUrl = t[e].pUrl || t[e].urlRegex, t[e].pExcludeUrl = t[e].pExcludeUrl || t[e].excludeUrl) : t[e].pUrl = t[e].pUrl || ".*"
        }
        isGoalEligible(e, t, i) {
            i = i || window.VWO.phoenix;
            const n = (t = t || i.store.getters).currentUrl;
            return e.pExcludeUrl && Wr.matchRegex(n, e.pExcludeUrl) ? (i.trigger(_.EXCLUDE_GOAL_URL), !1) : e.pUrl ? Fr.verifyUrl(n, e.pUrl, null) : Fr.verifyUrl(n, null, e.urlRegex)
        }
        registerConversion(e, t, n, s, r, o, a) {
            return i(this, void 0, void 0, (function*() {
                if ("INSIGHTS_FUNNEL" === n.type) return void window.VWO._.insightsUtils.markFunnelValue(n.id, 1, t, n.version);
                if (!_r.shouldWeTrackVisitor()) return;
                t = t || 1;
                const i = window.VWO.phoenix;
                e = e || i.store.getters, yield this._triggerGoalConversion(i, e, t, n, s, r, Object.assign(Object.assign({}, a || {}), {
                    combination: (null == a ? void 0 : a.combination) || (yield io.getCombi(e, n, o))
                }))
            }))
        }
        getImgUrlForConversion(e, t, n, s, r, o) {
            return i(this, void 0, void 0, (function*() {
                var i, a;
                const l = e.id;
                if (a = "c.gif?account_id=" + s.accountId + "&experiment_id=" + l + "&goal_id=" + t + "&ru=" + encodeURIComponent(yield Ks.get()) + (void 0 === r ? "" : "&r=" + r) + Xr.getUUIDString((null == o ? void 0 : o.uuid) || (yield Xr.getUUID(s, e))), "TRACK" === e.type) {
                    i = (null == o ? void 0 : o.sId) || (yield s.vwoInternalProperties.sessionInfoService.getSessionId()), yield window.VWO.phoenix.trigger(_.EXECUTE_FUNNEL_FOR_GOAL_CAMPAIGN, {
                        campaign: e
                    });
                    let n = null == o ? void 0 : o.gtAndF;
                    if (n || (n = yield window.tracklib.getGtAndF(t)), n) {
                        return a + "&s=" + i + "&ifs=" + +(i === ((null == o ? void 0 : o.sId) || (yield s.vwoInternalProperties.sessionInfoService.getSessionId()))) + "&t=1&cu=" + encodeURIComponent((null == o ? void 0 : o.pageUrl) || s.currentUrl) + n
                    }
                    return ""
                }
                return (yield s.vwoInternalProperties.sessionInfoService.shouldSendSessionInfoInCall()) && (i = yield s.vwoInternalProperties.sessionInfoService.getSessionId()), a + "&combination=" + n + (i = i ? "&sId=" + i : "")
            }))
        }
        _triggerGoalConversion(e, t, n, s, r, o, a) {
            return i(this, void 0, void 0, (function*() {
                if ("INSIGHTS_METRIC" === s.type) return void io.markGoalTriggered(s, n, t);
                const i = a.combination;
                if (!o && (!i || (yield io.isGoalTriggered(s, n, t)) || Xr.isBot2())) return void(yield e.trigger(_.REGISTER_CONVERSION, {
                    oldArgs: [s.id, n, r, !1],
                    campaignId: s.id,
                    goalId: n
                }));
                "REVENUE_TRACKING" !== s.goals[n].type && (r = void 0);
                const l = yield this.getImgUrlForConversion(s, n, i, t, r, {
                    uuid: a.uuid,
                    gtAndF: a.gtAndF,
                    sId: a.sId,
                    pageUrl: a.pageUrl
                });
                l && ((yield Xr.isEligibleToSendCall(s.id, t)) && (yield Zs.sendCall(t, {
                    url: l
                }, null, null)), yield io.markGoalTriggered(s, n, t)), A.queueGoalLogs(s.id, n, r, !!l) && (yield e.trigger(_.REGISTER_CONVERSION, {
                    oldArgs: [s.id, n, r, !!l],
                    campaignId: s.id,
                    goalId: n
                }))
            }))
        }
    }
    const qo = new Xo;
    class Qo {}
    class Zo extends Qo {
        shouldWeTriggerMetric(e, t, i, n, s) {
            const r = n.isFirst,
                {
                    excludeUrl: o,
                    pExcludeUrl: a,
                    urlRegex: l,
                    pUrl: d
                } = s;
            let c;
            c = !(o || a || l || d) || qo.isGoalEligible(s, e, t);
            return !(!Xr.isSessionBasedCampaign2(n) && r && i.name === _.PAGE_VIEW && "CUSTOM_GOAL" === s.type) && c
        }
        isItCustomConversionEvent(e) {
            return e.name === _.CUSTOM_CONVERSION
        }
    }
    var ea = new Zo;
    class ta {
        constructor() {
            this.whiteListedEventsForVsKey = [_.PAGE_VIEW, _.CUSTOM_CONVERSION, _.DOM_CLICK, _.DOM_SUBMIT, _.REVENUE_CONVERSION]
        }
        getCurrentEventData(e, t, i) {
            const n = {};
            if (!(Object.keys(t).length <= 0)) return Object.keys(t).forEach((s => {
                var r;
                n[s] = n[s] || {}, n[s] = {
                    vwoMeta: {
                        metric: t[s].metrics
                    }
                }, this.whiteListedEventsForVsKey.includes(e) && t[s].comb && (n[s].vwoMeta.vS = t[s].comb), (null === (r = i[s]) || void 0 === r ? void 0 : r.length) > 0 && (n[s].matchedSelectors = i[s])
            })), n
        }
    }
    class ia extends ta {
        constructor() {
            super(...arguments), this.mutex = {}
        }
        execute({
            data: e,
            vwoEvents: t,
            getters: n,
            event: s
        }) {
            var r;
            return i(this, void 0, void 0, (function*() {
                if ([_.DOM_CLICK, _.DOM_SUBMIT, _.PAGE_UNLOAD].includes(s.name)) return;
                if (n.vwoInternalProperties.willRedirectionOccur) return;
                if (Xr.isBot2()) return;
                const o = {},
                    a = {};
                for (const r of e)
                    for (const e of r.campaigns) {
                        const r = e.c,
                            l = e.g;
                        let d, c, u = r && n.currentSettings.dataStore.campaigns[r],
                            g = !1;
                        const v = r + "-" + l;
                        this.mutex[v] || (this.mutex[v] = Promise.resolve()), this.mutex[v] && (c = this.mutex[v].then((() => i(this, void 0, void 0, (function*() {
                            var i, c, v, h;
                            if (!r || !l || (null === (c = null === (i = s._vwo) || void 0 === i ? void 0 : i.campaignsConverted) || void 0 === c ? void 0 : c.includes(r)) || !u) return;
                            if (d = u.goals[l], !d) return;
                            g = Xr.isSessionBasedCampaign2(u);
                            const p = "SEPARATE_PAGE" === d.type,
                                w = "TRACK" === u.type;
                            if (g && w && !p) return;
                            const E = Xr.hasInsightsMetric(u.type);
                            if ([_.PAGE_VIEW, _.CUSTOM_CONVERSION, _.REVENUE_CONVERSION].includes(s.name)) {
                                if (s.ins && !E) return;
                                if (E && !s.ins) return
                            }
                            const f = u.triggers[0];
                            if (!g && !(null === (v = window._vwoCc) || void 0 === v ? void 0 : v.ignoreCSAForGoals) && (null === (h = null == u ? void 0 : u.ss) || void 0 === h ? void 0 : h.csa) && !(yield t.validateTrigger(n.currentSettings.triggers[f], null, {
                                    triggerName: f
                                }))) return;
                            if ((null == u ? void 0 : u.mE) && io.doExperimentHere(n, u, t)[0] && !u.combination_chosen) return;
                            if (!Xr.shouldTrackUserForCampaign(u) || !(yield io.isBucketed(n, u, !p)) || !ea.shouldWeTriggerMetric(n, t, s, u, d)) return;
                            if (p && delete d.pageVisited, yield io.isGoalTriggered(u, l, n)) return;
                            u || (u = {
                                id: r,
                                goals: {
                                    [l]: {}
                                },
                                type: e.sess
                            });
                            const m = e.uuid || (yield Xr.createUUIDCookie2(n, u));
                            if (!g || E) {
                                if ("CUSTOM_GOAL" === (null == d ? void 0 : d.type) && (null == d ? void 0 : d.url)) {
                                    const e = d.url;
                                    o[m] = o[m] || [], o[m].indexOf(e) < 0 && o[m].push(e)
                                }
                                a[m] = a[m] || {};
                                const e = "id_" + r;
                                a[m].metrics = a[m].metrics || {}, a[m].metrics[e] = a[m].metrics[e] || [], a[m].metrics[e].push("g_" + l), u.isEventMigrated && !s.isCustomEvent && (a[m].comb = a[m].comb || {}, a[m].comb[e] = yield io.getCombi(n, u, !0))
                            }
                            var O = !0;
                            window.VWO._.isBeaconAvailable = s.isBeaconAvailable, window.VWO._.isLinkRedirecting = s.isLinkRedirecting;
                            yield qo.registerConversion(n, l, u, s.revenue, !0, !0, {
                                combination: e.combination,
                                uuid: m,
                                gtAndF: e.gtAndF,
                                sId: e.sId,
                                pageUrl: e.pageUrl
                            }), t.trigger(_.GOAL_CONVERTED, {
                                campaignId: u.id,
                                goalId: l
                            }), window.VWO._.isLinkRedirecting = !1, O = O && window.VWO._.isBeaconAvailable
                        }))))), this.mutex[v] = c, yield c
                    }
                const l = this.getCurrentEventData(s.name, a, o);
                s._vwo = (null == s ? void 0 : s._vwo) || {}, s._vwo.eventDataConfig = (null === (r = s._vwo) || void 0 === r ? void 0 : r.eventDataConfig) || {}, s._vwo.eventDataConfig = l
            }))
        }
    }
    const na = new ia,
        sa = na.execute.bind(na);
    window.VWO.modules.tags.metricWT = sa;
    class ra {
        otherSide(...e) {
            return e[0] = "VWO.modules.tags.prePostMutation.fn." + e[0], window.fetcher.getValue(...e)
        }
    }
    window.VWO.modules.tags.prePostMutation = {};
    class oa {
        otherSide(...e) {
            return e[0] = "VWO.modules.tags.prePostMutation.utils." + e[0], window.fetcher.getValue(...e)
        }
    }
    class aa extends oa {
        isMonitorPageChangesRequired(e) {
            for (const t in e)
                if (Object.prototype.hasOwnProperty.call(e, t)) {
                    const i = e[t];
                    if (i.muts && i.muts.post && i.muts.post.enabled) return !0
                }
            return !1
        }
        getWaitForDOMRenderingConfigAndHideElements(e) {
            let t = !1,
                i = 10,
                n = 1e3;
            for (const s in e)
                if (Object.prototype.hasOwnProperty.call(e, s)) {
                    const r = e[s],
                        o = r.muts && r.muts.pre;
                    o && o.enabled && (t = !0, o.timer && o.timer > i && (i = o.timer), o.timeout && o.timeout > n && (n = o.timeout))
                }
            return {
                enabled: t,
                timer: i,
                timeout: n
            }
        }
        waitForDOMRenderingAndExecuteCampaign(e) {
            return this.otherSide("waitForDOMRenderingAndExecuteCampaign", [e])
        }
        monitorPageForChanges() {
            return this.otherSide("monitorPageForChanges")
        }
    }
    const la = new aa;

    function da() {
        ca("attachEditorChangeObserverEvents")
    }

    function ca(...e) {
        return e[0] = "VWO.modules.tags.prePostMutation.editorChangesObserver." + e[0], window.fetcher.getValue(...e)
    }
    window.VWO.modules.tags.prePostMutation.utils = la;
    class ua extends ra {
        constructor() {
            super(...arguments), this.editorChangeObserverEventsAttached = !1
        }
        execute({
            data: e,
            getters: t,
            vwoEvents: n
        }) {
            return i(this, void 0, void 0, (function*() {
                e = t.settings.campaigns;
                const i = t.MutationObserver;
                let s, r = Promise.resolve();
                if (i) {
                    const t = la.getWaitForDOMRenderingConfigAndHideElements(e);
                    t.enabled ? r = la.waitForDOMRenderingAndExecuteCampaign(t) : n.trigger(_.SSR_COMPLETE)
                }
                la.isMonitorPageChangesRequired(e) && (s = la.monitorPageForChanges(), this.editorChangeObserverEventsAttached || (da(), this.editorChangeObserverEventsAttached = !0)), yield Promise.all([r, s])
            }))
        }
    }
    const ga = new ua,
        va = ga.execute.bind(ga);
    window.VWO.modules.tags.prePostMutation.fn = ga;
    class ha extends n {
        errorTracking({
            getters: e
        }) {
            return i(this, void 0, void 0, (function*() {
                const t = yield window.fetcher.getValue('VWO.modules.tags.errorTrackingCallback("${{1}}")', null, {
                    captureGroups: [e]
                });
                this.setCustomError(t)
            }))
        }
    }
    const pa = new ha,
        wa = pa.errorTracking;

    function Ea(e) {
        return i(this, void 0, void 0, (function*() {
            const t = e.data,
                i = e.event.name;
            yield window.fetcher.getValue('VWO.modules.tags.sampleVisitor("${{1}}","${{2}}")', null, {
                captureGroups: [t, i]
            })
        }))
    }
    class fa {}
    class _a extends fa {
        constructor(e, t) {
            super(), this.flattenedGroup = [], this.groupsConfig = e, this.campaigns = t, this.expPossibleToRunMap = {}
        }
        flattenGroupsData() {
            if (this.groupsConfig)
                for (let e = 0; e < this.groupsConfig.length; e++) {
                    if (this.groupsConfig[e].c instanceof Array)
                        for (let t = 0; t < this.groupsConfig[e].c.length; t++) this.groupsConfig[e].c[t] = this.groupsConfig[e].c[t].toString();
                    this.flattenedGroup = this.flattenedGroup.concat(this.groupsConfig[e].c)
                }
        }
        isExperimentReadyToRun(e, t, n) {
            return i(this, void 0, void 0, (function*() {
                const i = t.triggers[0];
                return io.doExperimentHere(e, t, n)[0] && (yield window.VWO.phoenix.validateTrigger(e.currentSettings.triggers[i], null, {
                    skipEventProps: !0,
                    skipValidationIfTimer: !0
                }))
            }))
        }
        checkForExistingWinner(e, t) {
            return i(this, void 0, void 0, (function*() {
                return e = e || window.phoenix.store.getters, !!((yield io.getCombiCookie(e, t)) || io.isLogged(e, t, !0) || "SPLIT_URL" === this.campaigns[t].type && (yield Xr.getSplitDecision(e, t)) || 1 === this.expPossibleToRunMap[t])
            }))
        }
        checkWinnerAlreadyExists(e) {
            for (let t = 0; t < e.length; t++) {
                const i = parseInt(e[t], 10);
                if (1 === this.expPossibleToRunMap[i]) return i
            }
        }
        checkIfStringArray(e) {
            return !!Array.isArray(e)
        }
        processExperimentsOnBasisOfRandomness(e, t) {
            return i(this, void 0, void 0, (function*() {
                const n = {},
                    s = {},
                    r = t.c;
                let o = 0;
                if (!this.checkIfStringArray(r)) return;
                let a = this.checkWinnerAlreadyExists(r);
                if (!a) {
                    for (let e = 0; e < r.length; e++) 2 === this.expPossibleToRunMap[r[e]] && (o += 1, s[r[e]] = this.campaigns[r[e]].triggers);
                    if (!o) {
                        const t = r.map((t => i(this, void 0, void 0, (function*() {
                            4 === this.expPossibleToRunMap[t] && (yield io.exclude(e, this.campaigns[t]))
                        }))));
                        return void(yield Promise.all(t))
                    }
                    for (const e in s) Object.prototype.hasOwnProperty.call(s, e) && (n[e] = +(100 / o).toFixed(2));
                    a = yield io.chooseCombination(e, null, {
                        scaleInfo: n,
                        segmentInfo: s
                    }), this.expPossibleToRunMap[a] = 1
                }
            }))
        }
        processExperimentsOnBasisOfPriorityAndWeight(e, t) {
            return i(this, void 0, void 0, (function*() {
                const n = t.c,
                    s = t.p,
                    r = t.wt;
                if (!this.checkIfStringArray(n)) return;
                let o = this.checkWinnerAlreadyExists(n);
                if (!o) {
                    if (s && s.length)
                        for (let e = 0; e < s.length; e++)
                            if (2 === this.expPossibleToRunMap[s[e]]) {
                                o = s[e], this.expPossibleToRunMap[o] = 1;
                                break
                            }
                    if (!o && r) {
                        const t = {},
                            i = {};
                        let n = 0,
                            s = 0;
                        const a = Object.keys(r);
                        for (let e = 0; e < a.length; e++) 2 === this.expPossibleToRunMap[a[e]] && (n += 1, i[a[e]] = this.campaigns[a[e]].triggers, s += r[a[e]]);
                        if (n) {
                            for (let e in i) Object.prototype.hasOwnProperty.call(i, e) && (t[e] = +(100 * r[e] / s).toFixed(2));
                            o = yield io.chooseCombination(e, null, {
                                scaleInfo: t,
                                segmentInfo: i
                            }), this.expPossibleToRunMap[o] = 1
                        }
                    }
                    if (!o) {
                        const t = n.map((t => i(this, void 0, void 0, (function*() {
                            4 === this.expPossibleToRunMap[t] && (yield io.exclude(e, this.campaigns[t]))
                        }))));
                        yield Promise.all(t)
                    }
                }
            }))
        }
        updateExperiments(e) {
            return i(this, void 0, void 0, (function*() {
                let t, n = !1,
                    s = !1,
                    r = !1;
                const o = Object.keys(this.campaigns).map((o => i(this, void 0, void 0, (function*() {
                    if (Object.prototype.hasOwnProperty.call(this.campaigns, o)) {
                        const i = e.settings.currentSettings.triggers[window._vwo_exp[o].triggers[0]],
                            a = "SPLIT_URL" === this.campaigns[o].type;
                        let l;
                        0 !== this.expPossibleToRunMap[o] && 1 !== this.expPossibleToRunMap[o] || (this.campaigns[o].shouldHideElement = 1), 1 === this.expPossibleToRunMap[o] && (e.trigger(_.GROUP_WINNER_CHOOSEN, {
                            groupWinner: parseInt(o, os)
                        }), a && (n = !0, (l = yield window.VWO.phoenix.validateTrigger(i, null, {
                            skipEventProps: !0
                        })) && (r = !0))), !a || s && t || (this.flattenedGroup.indexOf(o) < 0 ? l && io.doExperimentHere(e.store.getters, window._vwo_exp[o], e)[0] && (t = !0) : s = !0)
                    }
                }))));
                yield Promise.all(o), n && r || !s || t || e.trigger(_.NOT_REDIRECTING)
            }))
        }
        filterByGroupType(e) {
            const t = this.groupsConfig.map((t => 1 == t.et ? this.processExperimentsOnBasisOfRandomness(e, t) : 2 == t.et ? this.processExperimentsOnBasisOfPriorityAndWeight(e, t) : void 0));
            return Promise.all(t)
        }
        filterExperimentsFromGroups({
            getters: e,
            vwoEvents: t
        }) {
            var n;
            return i(this, void 0, void 0, (function*() {
                if (!(null === (n = this.groupsConfig) || void 0 === n ? void 0 : n.length)) {
                    for (const e in this.campaigns) Object.prototype.hasOwnProperty.call(this.campaigns, e) && t.trigger(_.GROUP_WINNER_CHOOSEN, {
                        groupWinner: parseInt(e, os)
                    });
                    return
                }
                let s = !1;
                const r = Object.keys(this.campaigns).map((n => i(this, void 0, void 0, (function*() {
                    n = n.toString();
                    const i = this.campaigns[n];
                    if (Sn(this.flattenedGroup, n) < 0) this.expPossibleToRunMap[n] = 0;
                    else if (yield io.isExcluded(e, i)) this.expPossibleToRunMap[n] = 3;
                    else {
                        s = !0;
                        const r = this.checkForExistingWinner(e, i.id),
                            o = this.isExperimentReadyToRun(e, i, t);
                        (yield r) ? this.expPossibleToRunMap[n] = 1: 1 !== this.expPossibleToRunMap[n] && (yield o) && ((yield io.shouldBucket(e, i)) ? this.expPossibleToRunMap[n] = 2 : this.expPossibleToRunMap[n] = 4)
                    }
                }))));
                return yield Promise.all(r), s && (yield this.filterByGroupType(e)), this.updateExperiments(t)
            }))
        }
    }
    class ma {}
    class Oa extends ma {
        constructor() {
            super(), this.campaignsData = {}
        }
        processCampaigns({
            event: e,
            getters: t,
            actions: i,
            vwoEvents: n,
            data: s
        }) {
            const r = t.settings.vwoData.gC,
                o = t.settings.campaigns,
                a = new _a(r, o);
            return a.flattenGroupsData(), a.filterExperimentsFromGroups({
                actions: i,
                getters: t,
                event: e,
                vwoEvents: n,
                data: s
            })
        }
        processGroupCampaigns({
            event: e,
            data: t,
            getters: i,
            actions: n,
            vwoEvents: s
        }) {
            if (window.VWO._.pageData = window.VWO._.pageData || {}, window.fetcher.setValue("VWO._.pageData", window.VWO._.pageData), window.VWO._.pageData._grp_campaign_called) return void s.trigger("vwo_groupCampTriggered");
            window.VWO._.pageData._grp_campaign_called = !0, window.fetcher.setValue("VWO._.pageData._grp_campaign_called", !0);
            const r = io.getGroupBasedCampaigns({
                event: e,
                data: t,
                getters: i,
                actions: n,
                vwoEvents: s
            });
            if (r.length) {
                for (const e of r) this.campaignsData[e] = i.settings.campaigns[e];
                return this.processCampaigns({
                    event: e,
                    getters: i,
                    actions: n,
                    vwoEvents: s,
                    data: this.campaignsData
                }).then((() => {
                    s.trigger("vwo_groupCampTriggered")
                }))
            }
            s.trigger("vwo_groupCampTriggered")
        }
    }
    const Sa = new Oa,
        ya = Sa.processGroupCampaigns.bind(Sa);
    window.VWO.modules.tags.groupCampaigns = ya;
    class Ta {}
    class Ia {}
    class Ca extends Ia {
        resetTriggerStates() {
            var e, t, i, n;
            const s = vr.phoenix;
            vr._.pageNo = "number" == typeof vr._.pageNo ? vr._.pageNo + 1 : 0;
            const r = "vwo_timer",
                o = null === (i = null === (t = null === (e = s.store) || void 0 === e ? void 0 : e.state) || void 0 === t ? void 0 : t.values) || void 0 === i ? void 0 : i.triggers,
                a = s.settings.currentSettings.triggers,
                l = s.triggers.triggerEventListeners;
            or(s, _.POST_INIT, {}), s.eventBus.remove(r);
            for (const e in a)
                if (Object.prototype.hasOwnProperty.call(a, e)) {
                    const t = null === (n = a[e]) || void 0 === n ? void 0 : n.cnds;
                    let i, d;
                    o[e] && (i = o[e].event, d = o[e].trigger);
                    let c, u = !1;
                    if (t) {
                        const e = t => {
                            if (t && "string" != typeof t) {
                                if (Array.isArray(t)) return t.forEach(e);
                                if ("[object Object]" === t.toString() && void 0 !== t.id) {
                                    d && d[t.id] && (d[t.id].state = !1);
                                    t.event === r && (u = !0, c = t.exitTrigger)
                                }
                            }
                        };
                        t.forEach(e)
                    }
                    if (i) {
                        o[e].state = !1;
                        for (const e in i) Object.prototype.hasOwnProperty.call(i, e) && delete i[e]
                    }
                    u && (c && (s.eventBus.remove(s.triggers.getTriggerEventName(c)), c = null), delete l[e].vwo_timer, s.triggers.add(e, a[e]), u = !1)
                }
            s.events.events.vwo_timer.eventConditionsUpdate()
        }
        resetExpParams(e, t, i) {
            const n = e.currentSettings.dataStore.campaigns;
            for (const t in n)
                if (Object.prototype.hasOwnProperty.call(n, t) && (!i || "TRACK" === n[t].type)) {
                    const o = n[t];
                    if (o[rs] = 0, i && "TRACK" === o.type) {
                        const t = o.triggers[0];
                        e.triggers[t] && (e.triggers[t].executionCount = 0)
                    }
                    delete o[ns], delete o[ss], delete o.clicks, delete o.combination_chosen, delete o.mutElg, delete o.segment_eligble, delete o.isFirst, o.muts && delete o.muts.pvtMut, 779155 == window._vwo_acc_id ? !o.isProcessing && delete o.ready : delete o.ready, delete o.timedout, delete o.processed, delete o[ns], io.checkForVariationTargeting(o) && delete o.xPath, clearTimeout(o[ds]), delete o[ds], delete o.globalCode.preExecuted, delete o.globalCode.postExecuted;
                    for (var s = On(o.sections), r = 0; r < s.length; r++) delete o.sections[s[r]].loaded
                }
            if (window._vis_debug && !i) {
                e.getValue("tags.segmentEligibilityTest") && t.addValues({
                    executionCount: 0
                }, "tags.segmentEligibilityTest")
            }
        }
        resetTriggerExecutionCount(e) {
            const t = e.triggers;
            Object.keys(t).forEach((e => {
                t[e].executionCount = 0
            }))
        }
    }
    var Na = new Ca;
    class Aa extends Ta {
        constructor() {
            super(...arguments), this.lastExecutedForUrl = window.location.href
        }
        execute({
            event: e,
            data: t,
            getters: n,
            actions: s,
            vwoEvents: r
        }) {
            var o, a;
            return i(this, void 0, void 0, (function*() {
                Object.assign(window.location, (null == e ? void 0 : e.location) || {});
                const i = window.location.href = $((() => e.props.virtualPageUrl)) || $((() => e.location.href)) || (yield window.fetcher.getValue("location.href"));
                if (n.previousUrl = this.lastExecutedForUrl, window._vis_opt_url = $((() => e.location.visOptUrl)), this.lastExecutedForUrl === i || n.vwoInternalProperties.waitingForThirdPartySync) return;
                if (n.vwoInternalProperties.willRedirectionOccur) return;
                (null === (o = null == e ? void 0 : e.props) || void 0 === o ? void 0 : o.virtualPageUrl) || (this.lastExecutedForUrl = i), r.clearData(), Na.resetTriggerStates(), Po(), Na.resetExpParams(r.store.getters, s), Na.resetTriggerExecutionCount(r.store.getters), r.trigger(_.URL_CHANGED, {
                    preventWildCardEvent: !!Wn
                });
                if (Xr.setPropertiesToBothThreads(s, [{
                        willRedirectionOccur: !1,
                        destinationMT: "VWO._.willRedirectionOccur"
                    }], "vwoInternalProperties"), Xr.addListenerForSessionInitComplete(), wo({
                        event: e,
                        data: t,
                        getters: n,
                        actions: s,
                        vwoEvents: r
                    }), n.accountId, ur(), window.VWO._.campaignsInternalMap = {}, window.VWO._.preventHidingInSPA = null === (a = window.VWO.featureInfo) || void 0 === a ? void 0 : a.dNHEL, window.VWO._.pageData = window.VWO._.pageData || {}, delete window.VWO._.pageData._grp_campaign_called, yield Xr.shouldExecuteLib({
                        event: e,
                        data: t,
                        getters: n,
                        actions: s,
                        vwoEvents: r
                    }), window.fetcher.setValue("window.VWO._.urlChangeProcessingPending", !1), n.vwoInternalProperties.shouldExecuteLib) {
                    if (window._vis_debug || window._vis_heatmap) {
                        const e = n.vwoInternalProperties.experimentIds,
                            t = Ts(e);
                        r.trigger(_.CAMPAIGN_FLOW_START, {
                            oldArgs: [t]
                        })
                    }
                    n.vwoCode && (n.vwoCode[ts] = !1, n.vwoCode[is] = !1), window.VWO._.visibilityTriggered = new Promise((e => {
                        const t = r.on(_.VISIBILITY_TRIGGERED, (() => {
                            r.off(t), e()
                        }))
                    })), delete window.VWO._.auxPageViewFired, window.VWO._.insightsUtils.activateFunnels(), (n.visDebug || ys() || window.VWO._.selfHosted) && (yield Xr.firePageViewEvent({
                        shouldFireDomLoad: !0
                    })), window.VWO._.phoenixMT.trigger(_.LOAD_SETTINGS, 2), r.trigger(_.POST_URL_CHANGE, {
                        oldArgs: [Fr.getCleanedUrl(i), n.vwoInternalProperties.willRedirectionOccur]
                    })
                }
            }))
        }
    }
    const Va = new Aa,
        ba = Va.execute.bind(Va);

    function Ra({
        data: e,
        event: t,
        getters: i,
        vwoEvents: n
    }) {
        const s = t.props;
        !1 === s.segmentCndsSatisfied ? n.trigger(_.SEGMENTATION_EVALUATED, {
            oldArgs: [e.id, s.segmentCndsSatisfied, !0]
        }) : s.hideElementsTriggered && n.trigger(_.ELEMENTS_SHOWN_WITHOUT_CHANGES, {
            oldArgs: [e.id]
        })
    }
    const Pa = function({
        getters: e,
        vwoEvents: t
    }) {
        return i(this, void 0, void 0, (function*() {
            const i = Object.keys(e.settings.campaigns || {});
            if (!ys() && !window._vis_debug || !window.VWO._.blockedState && !N())
                for (const n of i) {
                    const i = e.settings.campaigns[n],
                        s = kr.getCleanedUrl(e.currentUrl),
                        r = i.urlRegex,
                        o = i.url_pattern;
                    let a = !1;
                    if ("SPLIT_URL" === i.type) {
                        const t = yield Xr.getSplitDecision(e, i.id);
                        t && Xr.isCurrentURLSplitVariation({
                            chosenVariation: t,
                            getters: e,
                            campaignData: i
                        }) && (a = !0)
                    }
                    const [l, d] = a ? [!0, 2] : io.doExperimentHere(e, i, t, {
                        urlRegex: r,
                        urlPattern: o
                    });
                    io.firePatternMatchingEvent(t, i.id, d, s, r || o, l), t.trigger("executePatternMatching", {
                        _vwo: {
                            doExperimentHere: "SPLIT_URL" === i.type ? null : l
                        }
                    })
                }
        }))
    };
    class xa {
        constructor() {
            this.noOp = function() {}
        }
        getPhoenixConfig() {
            const e = {
                tags: {
                    checkEnvironment: {
                        fn: _o,
                        sync: !0
                    },
                    prePostMutation: {
                        fn: va,
                        sync: !0
                    },
                    visibilityService: {
                        fn: Uo,
                        sync: !0
                    },
                    groupCampaigns: {
                        fn: ya,
                        sync: !0
                    },
                    runCampaign: {
                        fn: Ko,
                        sync: !0
                    },
                    metric: {
                        fn: sa,
                        sync: !0,
                        fireUniquelyForEveryEvent: !0
                    },
                    runTestCampaign: {
                        fn: Jo,
                        sync: !0
                    },
                    errorTracking: {
                        fn: wa,
                        sync: !0
                    },
                    sampleVisitor: {
                        fn: Ea
                    },
                    urlChange: {
                        fn: ba,
                        sync: !0
                    },
                    executePreCampJsForGoalsPage: {
                        fn: ({
                            data: e
                        }) => {
                            io.executeCampaignJS(e, "pre")
                        },
                        sync: !0
                    }
                },
                storages: {
                    localStorageService: M,
                    cookies: Jn
                },
                jsLibUtils: {
                    verifyUrl: function() {
                        return Fr.verifyUrl.apply(Fr, arguments)
                    },
                    getCleanedUrl: function() {
                        return Fr.getCleanedUrl.apply(Fr, arguments)
                    }
                }
            };
            return window._vis_debug && (e.tags.segmentEligibilityTest = {
                fn: Ra,
                sync: !1,
                occurrence: 1
            }, e.tags.compareUrlAndFireResultantEvent = {
                fn: Pa,
                sync: !0
            }), e
        }
        setFunnelExps(e) {
            var t, i;
            const n = null === (t = null == e ? void 0 : e.settings) || void 0 === t ? void 0 : t.campaigns,
                s = [];
            for (const e in window._vwo_exp)
                if (window._vwo_exp[e].funnel)
                    for (const t of window._vwo_exp[e].funnel) {
                        const e = t;
                        (null === (i = window._vwo_exp[e.id]) || void 0 === i ? void 0 : i.g) || (window._vwo_exp[e.id] = e, window._vwo_exp[e.id].g = e.goals, window._vwo_exp[e.id].goals = {}, s.push(e.id + ""), n && (n[e.id] = window._vwo_exp[e.id]))
                    }
            return s
        }
        sendMessageToParentFrame(e) {
            return window.fetcher.getValue("VWO.modules.utils.initUtils.sendMessageToParentFrame", [e])
        }
        getCookieJarValidValue(e) {
            return ["null", null, void 0, "undefined"].indexOf(e) > -1 ? "" : e
        }
    }
    var La = new xa;
    class Da {
        mergeNestedObjects(...e) {
            return e.reduce(((e, t) => this.recursivelyMerge(e, t)))
        }
        mergeNestedObjectsV2(e = {
            mergeArrays: !1
        }, ...t) {
            return t.reduce(((t, i) => this.recursivelyMerge(t, i, {}, e)))
        }
        createNestedObjects(e, t) {
            let i = e;
            return t && t.split(".").forEach((e => {
                Object.prototype.hasOwnProperty.call(i, e) || (i[e] = {}), i = i[e]
            })), i
        }
        clearNestedObject(e, t) {
            let i = e;
            const n = t.split("."),
                s = n[n.length - 1];
            for (let e = 0; e < n.length - 1; e++) i = i[n[e]];
            E(i[s]) ? i[s] = {} : delete i[s]
        }
        recursivelyMerge(e, t, i = {}, n = {
            mergeArrays: !1
        }) {
            if (E(e) && E(t)) {
                const s = {};
                Object.keys(e).concat(Object.keys(t)).forEach((e => {
                    s[e] = 1
                }));
                const r = Object.getOwnPropertyDescriptors(e),
                    o = Object.getOwnPropertyDescriptors(t);
                return Object.keys(s).forEach((s => {
                    o[s] ? Object.defineProperty(i, s, o[s]) : Object.defineProperty(i, s, r[s]), this.recursivelyMerge(e[s], t[s], i[s], n)
                })), i
            }
            return n.mergeArrays && f(e) && f(t) ? (f(i) || (i = []), i.splice(0, i.length, ...e.concat(t)), i) : t || e
        }
    }
    var Ua = new Da,
        Wa, Ma, ka;

    function Ga(...e) {
        return e[0] = "VWO.modules.otherLibDeps." + e[0], window.fetcher.getValue(...e)
    }

    function Fa(e) {
        return Ga('storeSurveyDataInVWOSurveySettings("${{1}}")', null, {
            captureGroups: [e]
        })
    }! function(e) {
        e.SUM = "sum", e.COUNT = "count", e.AVG = "avg", e.MIN = "min", e.MAX = "max", e.FIRST_VALUE = "first", e.LAST_VALUE = "last", e.MEAN = "mean", e.MEDIAN = "median"
    }(Wa || (Wa = {})),
    function(e) {
        e.SINCE = "s", e.WITHIN = "w", e.FROM = "f"
    }(Ma || (Ma = {})),
    function(e) {
        e.DAYS = "d", e.WEEKS = "w", e.MONTHS = "m", e.YEARS = "y"
    }(ka || (ka = {}));
    class ja {
        setItem(e, t) {
            e = this.getKeyBasedOnMode(e), M.set(e, JSON.stringify(t))
        }
        getItem(e) {
            return e = this.getKeyBasedOnMode(e), M.get(e)
        }
        removeItem(e) {
            e = this.getKeyBasedOnMode(e), M.remove(e)
        }
        getKeyBasedOnMode(e) {
            if (!window._vis_debug && !ys()) return e;
            return "debug" + e + "_" + Object.keys(window._vwo_exp || {}).join("_")
        }
    }
    class Ha extends ja {
        constructor() {
            super(...arguments), this.isStorageSyncerForCurrentSessionEnabled = !1, this.areEventHistSessionListenersAttached = !1, this.areUrlChangeListenersAttached = !1, this.areEventHistListenersAttached = !1
        }
        initialize() {
            this.eventHist = JSON.parse(this.getItem("_vwo_eventHist") || "{}"), this.eventHistForCurrentSession = JSON.parse(this.getItem("_vwo_eventHistSession") || "{}"), this.currentSessionEvents = window.VWO._.allSettings.dataStore.cSE || {}, this.syncEvents = window.VWO._.allSettings.dataStore.syE || {}, this.customSyncURL = bn(), this.syncEventsConfig = this.customSyncURL ? _.SEND_SYNC_CALL : window.VWO._.allSettings.dataStore.syncEvent || "sessionCreated", this.triggerEvents = []
        }
        clearHistEventData() {
            window.VWO._.ac.cSHS ? Ba.clearsyE() : (ys() || window._vis_heatmap || !Object.keys(this.syncEvents).length) && Ba.clearHistoricalDataOfType(Ka.Events)
        }
        addEventHistSessionListeners() {
            (window.VWO._.ac.cSHS || Object.keys(this.currentSessionEvents).length || Object.keys(this.eventHistForCurrentSession).length) && (this.isStorageSyncerForCurrentSessionEnabled || !Object.keys(this.currentSessionEvents).length && !window.VWO._.ac.cSHS || (this.isStorageSyncerForCurrentSessionEnabled = !0, window.VWO.modules.utils.storageSyncer.add(this.getKeyBasedOnMode("_vwo_eventHistSession"), (e => {
                try {
                    this.eventHistForCurrentSession = JSON.parse(e) || {}
                } catch (e) {}
            }))), this.areEventHistSessionListenersAttached || (window.VWO.phoenix.on(_.NEW_SESSION_CREATED, (() => {
                this.clearEventHistSessionData()
            })), window.VWO.phoenix.on(_.TRACK_NEW_SESSION_CREATED, (() => {
                this.clearEventHistSessionData()
            })), this.areEventHistSessionListenersAttached = !0))
        }
        clearEventHistSessionData() {
            this.eventHistForCurrentSession = {}, Ba.clearHistoricalDataOfType(Ka.SessionEvents)
        }
        addUrlChangeListener() {
            ys() || window._vis_heatmap || !Object.keys(this.syncEvents).length && !Object.keys(this.currentSessionEvents).length || this.areUrlChangeListenersAttached || (window.VWO.phoenix.on(_.URL_CHANGED, (() => {
                this.triggerEvents.forEach((e => i(this, void 0, void 0, (function*() {
                    (this.eventHist[e.eventName] || this.eventHistForCurrentSession[e.eventName]) && (yield window.VWO.phoenix.isValid(e.triggerName, e.trigger, e.eventName))
                })))), this.shouldSyncEvents = !window.VWO._.ac.cSHS && !!Object.keys(this.syncEvents).length
            })), this.areUrlChangeListenersAttached = !0)
        }
        addEventHistListeners() {
            if (!ys() && !window._vis_heatmap && (window.VWO._.ac.cSHS || Object.keys(this.syncEvents).length || this.customSyncURL) && !this.areEventHistListenersAttached) {
                if (this.areEventHistListenersAttached = !0, window.VWO.modules.utils.storageSyncer.add(this.getKeyBasedOnMode("_vwo_eventHist"), (e => {
                        try {
                            this.eventHist = JSON.parse(e) || {}
                        } catch (e) {}
                    })), window.VWO._.ac.cSHS) return void this.addListenerForSyEUpdation();
                if (this.shouldSyncEvents = !0, window._vis_debug) return void this.addListenerForSyEUpdation();
                "pageView" === this.syncEventsConfig ? window.VWO.phoenix.on(_.PAGE_VIEW, (() => {
                    this.syncEvents = window.VWO._.allSettings.dataStore.syE || {}, this.shouldSyncEvents && (this.syncEventsData(), this.shouldSyncEvents = !1)
                })) : this.syncEventsConfig === _.SEND_SYNC_CALL ? window.VWO.phoenix.on(_.SEND_SYNC_CALL, (() => {
                    this.syncEvents = window.VWO._.allSettings.dataStore.syE || {}, this.shouldSyncEvents && (this.syncEventsData(), this.shouldSyncEvents = !1)
                })) : "sessionCreated" === this.syncEventsConfig && (window.VWO.phoenix.on(_.NEW_SESSION_CREATED, (() => {
                    this.syncEvents = window.VWO._.allSettings.dataStore.syE || {}, this.shouldSyncEvents && (this.syncEventsData(), this.shouldSyncEvents = !1)
                })), window.VWO.phoenix.on(_.TRACK_NEW_SESSION_CREATED, (() => {
                    this.syncEvents = window.VWO._.allSettings.dataStore.syE || {}, this.shouldSyncEvents && (this.syncEventsData(), this.shouldSyncEvents = !1)
                })), this.addListenerForSyEUpdation())
            }
        }
        addListenersForHistoricalEvents() {
            this.addEventHistListeners(), this.addEventHistSessionListeners(), this.addUrlChangeListener()
        }
        addListenerForSyEUpdation() {
            const e = window.VWO.phoenix.on(_.PAGE_VIEW, (() => {
                this.syncEvents = window.VWO._.allSettings.dataStore.syE || {}, window.VWO._.ac.cSHS || (JSON.stringify(this.syncEvents) !== this.getItem("_vwo_syE") && (Object.keys(this.syncEvents).length && this.setItem("_vwo_syE", this.syncEvents), this.shouldSyncEvents && (this.syncEventsData(), this.shouldSyncEvents = !1)), window._vis_debug && window.VWO.phoenix.off(_.PAGE_VIEW, e))
            }))
        }
        setEventHistInLS(e) {
            this.eventHist[e].length ? this.setItem("_vwo_eventHist", this.eventHist) : delete this.eventHist[e]
        }
        setSessionEventHistInLS(e) {
            this.eventHistForCurrentSession[e].length ? this.setItem("_vwo_eventHistSession", this.eventHistForCurrentSession) : delete this.eventHistForCurrentSession[e]
        }
        deleteUncessaryProps(e) {
            const t = Object.assign({}, e);
            return delete t.name, delete t.props, delete t._meta, delete t.time, delete t.page, delete t.isCustomEvent, delete t.executingTagTrigger, delete t._vwo, t
        }
        updateEventHist(e, t) {
            var i, n;
            if (!ys() && !window._vis_heatmap && (window.VWO._.ac.cSHS || (null === (i = window._vwoCc) || void 0 === i ? void 0 : i.syncServerUrl) || Object.keys(this.syncEvents).length || Object.keys(this.currentSessionEvents).length))
                if (window.VWO._.ac.cSHS || (null === (n = window._vwoCc) || void 0 === n ? void 0 : n.syncServerUrl)) {
                    this.eventHist[e] = this.eventHist[e] || [], this.eventHistForCurrentSession[e] = this.eventHistForCurrentSession[e] || [];
                    const i = this.deleteUncessaryProps(t);
                    this.eventHistForCurrentSession[e].push(Object.assign({}, i)), this.setSessionEventHistInLS(e), i.ts = t.time, this.eventHist[e].push(i), this.setEventHistInLS(e)
                } else {
                    if (Object.keys(this.syncEvents).length) {
                        if (this.eventHist[e] = this.eventHist[e] || [], Object.prototype.hasOwnProperty.call(this.syncEvents, "*")) {
                            const i = this.deleteUncessaryProps(t);
                            i.ts = t.time, this.eventHist[e].push(i)
                        } else if (Object.prototype.hasOwnProperty.call(this.syncEvents, e)) {
                            const i = {};
                            if (Array.isArray(this.syncEvents[e]))
                                for (let n of this.syncEvents[e]) void 0 !== t[n] && (i[n] = t[n]);
                            i.ts = t.time, this.eventHist[e].push(i)
                        }
                        this.setEventHistInLS(e)
                    }
                    if (Object.keys(this.currentSessionEvents).length) {
                        if (this.eventHistForCurrentSession[e] = this.eventHistForCurrentSession[e] || [], Object.prototype.hasOwnProperty.call(this.currentSessionEvents, "*")) {
                            const i = this.deleteUncessaryProps(t);
                            this.eventHistForCurrentSession[e].push(i)
                        } else if (Object.prototype.hasOwnProperty.call(this.currentSessionEvents, e)) {
                            const i = {};
                            if (Array.isArray(this.currentSessionEvents[e]))
                                for (let n of this.currentSessionEvents[e]) void 0 !== t[n] && (i[n] = t[n]);
                            this.eventHistForCurrentSession[e].push(i)
                        }
                        this.setSessionEventHistInLS(e)
                    }
                }
        }
        syncEventsData() {
            return i(this, void 0, void 0, (function*() {
                if (ys() || window._vis_heatmap || !Object.keys(this.syncEvents).length && !this.customSyncURL) return;
                let e = yield Xr.getUUID(window.VWO.phoenix.store.getters), t = "";
                if (window._vis_debug) {
                    const i = Jn.get("_vwo_uuid", !1, !0);
                    if (!i) return;
                    window._vwo_exp && Object.keys(window._vwo_exp).length && (e = i, t = "&cId=" + Object.keys(window._vwo_exp).join("|"))
                }
                const i = this.customSyncURL || window._vwo_server_url || "https://dev.visualwebsiteoptimizer.com/",
                    n = "sync/events?a=" + window._vwo_acc_id + "&uuid=" + e + t;
                try {
                    const e = yield fetch(i + n), t = yield e.json();
                    this.eventHist = t, this.eventHist && Object.keys(this.eventHist).length ? this.setItem("_vwo_eventHist", this.eventHist) : this.removeItem("_vwo_eventHist"), window.VWO.phoenix.trigger(_.SYNC_EVENTS_COMPLETED), this.validateHistEventsPostSync()
                } catch (e) {
                    window.VWO.phoenix.trigger(_.SYNC_EVENTS_COMPLETED), this.validateHistEventsPostSync(), Vr.warn("Error in syncing historical Events Data.")
                }
            }))
        }
        validateHistEventsPostSync() {
            this.customSyncURL && this.triggerEvents.forEach((e => i(this, void 0, void 0, (function*() {
                yield window.VWO.phoenix.validateAndDispatchTriggerEvent(e.triggerName, e.trigger, {
                    useUnsavedEvents: !0
                }, e.eventName)
            }))))
        }
        getEventsByDate(e, t, i) {
            var n, s;
            const r = Object.assign({}, i);
            let o;
            if (delete r.page, delete r.isCustomEvent, delete r.name, "cs" === t) return o = [...this.eventHistForCurrentSession[e] || [], r], r.fromLocalStorage && (o = [...this.eventHistForCurrentSession[e] || []]), o; {
                o = [...this.eventHist[e] || [], Object.assign(Object.assign({}, r), {
                    ts: +new Date
                })], r.fromLocalStorage && (o = [...this.eventHist[e] || []]), r.useUnsavedEvents && (o = [...this.eventHist[e] || [], ...(null === (s = null === (n = window._VWO) || void 0 === n ? void 0 : n.unsavedEventsHistory) || void 0 === s ? void 0 : s[e]) || []]);
                const {
                    firstDate: i,
                    lastDate: a
                } = this.getDateRange(t, o), l = this.binSearchFirstIndex(i, o), d = this.binSearchLastIndex(a, o);
                return -1 === l || -1 === d ? [] : o.slice(l, d + 1)
            }
        }
        getDateRange(e, t) {
            let i, n;
            const s = e.split("-"),
                r = s[0],
                o = s[1];
            return r === Ma.SINCE ? (n = new Date, i = new Date, o[o.length - 1] === ka.DAYS ? i.setDate(i.getDate() - Number(o.slice(0, o.length - 1))) : o[o.length - 1] === ka.WEEKS ? i.setDate(i.getDate() - 7 * Number(o.slice(0, o.length - 1))) : o[o.length - 1] === ka.MONTHS ? i.setMonth(i.getMonth() - Number(o.slice(0, o.length - 1))) : o[o.length - 1] === ka.YEARS ? i.setFullYear(i.getFullYear() - Number(o.slice(0, o.length - 1))) : i = new Date(o)) : r === Ma.FROM ? (i = new Date(s[1]), n = new Date(s[2])) : r === Ma.WITHIN && (i = new Date(t[0].ts), n = new Date(i), o[o.length - 1] === ka.DAYS ? n.setDate(i.getDate() + Number(o.slice(0, o.length - 1))) : o[o.length - 1] === ka.WEEKS ? n.setDate(i.getDate() + 7 * Number(o.slice(0, o.length - 1))) : o[o.length - 1] === ka.MONTHS ? n.setMonth(i.getMonth() + Number(o.slice(0, o.length - 1))) : o[o.length - 1] === ka.YEARS ? n.setFullYear(i.getFullYear() + Number(o.slice(0, o.length - 1))) : n = new Date(o)), i.setHours(0, 0, 0, 0), n.setHours(23, 59, 59, 59), i = +i, n = +n, {
                firstDate: i,
                lastDate: n
            }
        }
        binSearchFirstIndex(e, t) {
            let i, n, s = 0,
                r = t.length - 1;
            for (; s <= r;) i = Math.floor((s - r) / 2 + r), n = t[i].ts, n >= e ? r = i - 1 : s = i + 1;
            return s >= t.length ? -1 : s
        }
        binSearchLastIndex(e, t) {
            let i, n, s = 0,
                r = t.length - 1;
            for (; s <= r;) i = Math.floor((s - r) / 2 + r), n = t[i].ts, n > e ? r = i - 1 : s = i + 1;
            return r < 0 ? -1 : r
        }
        getCumulativeData(e, t) {
            if (0 === e.length) return {
                "*": {
                    count: 0
                }
            };
            const i = {};
            for (let n in t) {
                for (let s of e) i[n] = i[n] || {}, "*" === n ? (i[n].count = i[n].count || 0, i["*"].count++) : Object.prototype.hasOwnProperty.call(s, n) && this.handleOps(s[n], t[n], i[n]);
                if (t[n].includes(Wa.AVG) && void 0 !== i[n][Wa.AVG] && (i[n][Wa.AVG] = i[n][Wa.AVG].sum / i[n][Wa.AVG].count), t[n].includes(Wa.MEDIAN) && void 0 !== i[n][Wa.MEDIAN]) {
                    const e = i[n][Wa.MEDIAN],
                        t = Math.floor(e.length / 2),
                        s = [...e].sort(((e, t) => e - t));
                    i[n][Wa.MEDIAN] = e.length % 2 != 0 ? s[t] : (s[t - 1] + s[t]) / 2
                }
            }
            return i
        }
        handleOps(e, t, i) {
            e = +e;
            for (let n of t) switch (n) {
                case Wa.COUNT:
                    i.count = i.count || 0, i.count++;
                    break;
                case Wa.SUM:
                    i.sum = i.sum || 0, i.sum += e;
                    break;
                case Wa.MIN:
                    i.min = Math.min(e, i.min || e);
                    break;
                case Wa.MAX:
                    i.max = Math.max(e, i.max || e);
                    break;
                case Wa.FIRST_VALUE:
                    void 0 === i.first && (i.first = e);
                    break;
                case Wa.LAST_VALUE:
                    i.last = e;
                    break;
                case Wa.AVG:
                    i.avg = i.avg || {}, i.avg.sum = i.avg.sum || 0, i.avg.sum += e, i.avg.count = i.avg.count || 0, i.avg.count++;
                    break;
                case Wa.MEDIAN:
                    i.median = i.median || [], i.median.push(e)
            }
        }
    }
    const $a = new Ha,
        Ba = {
            get syncEvents() {
                return $a.syncEvents
            },
            get syncAttributes() {
                return Ya.syncVisAttrs
            },
            get eventHistory() {
                return $a.eventHist
            },
            get visitorProperties() {
                return Ya.visitorAttrs
            },
            clearHistoricalDataOfType(e) {
                e === Ka.Attributes ? (M.remove(Ya.getKeyBasedOnMode("_vwo_syV")), M.remove(Ya.getKeyBasedOnMode("_vwo_visProps"))) : e === Ka.Events ? (M.remove($a.getKeyBasedOnMode("_vwo_syE")), M.remove($a.getKeyBasedOnMode("_vwo_eventHist"))) : e === Ka.SessionEvents && M.remove($a.getKeyBasedOnMode("_vwo_eventHistSession"))
            },
            clearHistoricalData(e, t) {
                window.VWO._.ac.cSHS || (Object.keys(t).length || (M.remove($a.getKeyBasedOnMode("_vwo_syE")), M.remove($a.getKeyBasedOnMode("_vwo_eventHist"))), Array.isArray(e) && (null == e ? void 0 : e.length) || M.remove(Ya.getKeyBasedOnMode("_vwo_syV")))
            },
            clearsyE() {
                M.remove($a.getKeyBasedOnMode("_vwo_syE"))
            },
            clearsyV() {
                M.remove(Ya.getKeyBasedOnMode("_vwo_syV"))
            }
        };
    var Ka;
    ! function(e) {
        e[e.Attributes = 0] = "Attributes", e[e.Events = 1] = "Events", e[e.SessionEvents = 2] = "SessionEvents"
    }(Ka || (Ka = {}));
    class za extends ja {
        constructor() {
            super(...arguments), this.areListenersAttachedAndStorageSyncerEnabled = !1
        }
        initialize() {
            this.visitorAttrs = JSON.parse(this.getItem("_vwo_visProps") || "{}"), this.syncVisAttrs = window.VWO._.allSettings.dataStore.syV || {}, this.customSyncURL = bn(), this.syncAttrConfig = this.customSyncURL ? _.SEND_SYNC_CALL : window.VWO._.allSettings.dataStore.syncAttr || "sessionCreated", window._vis_heatmap || (this.addListenerAndEnableStorageSyncer(), this.visitorAttrs && Object.keys(this.visitorAttrs).length && this.addAttrToGetters())
        }
        addListenerAndEnableStorageSyncer() {
            this.areListenersAttachedAndStorageSyncerEnabled || ($((() => this.syncVisAttrs.length)) && !window.VWO._.ac.cSHS ? (this.shouldSyncAttr = !0, this.addVisAttrListeners()) : Ba.clearsyV(), this.storageSyncAcrossTabs(), this.areListenersAttachedAndStorageSyncerEnabled = !0)
        }
        storageSyncAcrossTabs() {
            window.VWO.modules.utils.storageSyncer.add(this.getKeyBasedOnMode("_vwo_visProps"), (e => {
                try {
                    this.visitorAttrs = JSON.parse(e) || {}
                } catch (e) {}
            }))
        }
        addVisAttrListeners() {
            ys() || (window._vis_debug ? this.addListenerForSyVUpdation() : ("pageView" === this.syncAttrConfig ? window.VWO.phoenix.on(_.PAGE_VIEW, (() => {
                this.syncVisAttrs = window.VWO._.allSettings.dataStore.syV, this.shouldSyncAttr && (this.syncVisData(), this.shouldSyncAttr = !1)
            })) : this.syncAttrConfig === _.SEND_SYNC_CALL ? window.VWO.phoenix.on(_.SEND_SYNC_CALL, (() => {
                this.syncVisAttrs = window.VWO._.allSettings.dataStore.syV, this.shouldSyncAttr && (this.syncVisData(), this.shouldSyncAttr = !1)
            })) : "sessionCreated" === this.syncAttrConfig && (window.VWO.phoenix.on(_.NEW_SESSION_CREATED, (() => {
                this.syncVisAttrs = window.VWO._.allSettings.dataStore.syV, this.shouldSyncAttr && (this.syncVisData(), this.shouldSyncAttr = !1)
            })), window.VWO.phoenix.on(_.TRACK_NEW_SESSION_CREATED, (() => {
                this.syncVisAttrs = window.VWO._.allSettings.dataStore.syV, this.shouldSyncAttr && (this.syncVisData(), this.shouldSyncAttr = !1)
            })), this.addListenerForSyVUpdation()), window.VWO.phoenix.on(_.URL_CHANGED, (() => {
                this.shouldSyncAttr = !0
            }))))
        }
        addListenerForSyVUpdation() {
            const e = window.VWO.phoenix.on(_.PAGE_VIEW, (() => {
                this.syncVisAttrs = window.VWO._.allSettings.dataStore.syV, JSON.stringify(this.syncVisAttrs) !== this.getItem("_vwo_syV") && (Array.isArray(this.syncVisAttrs) && this.syncVisAttrs.length && this.setItem("_vwo_syV", this.syncVisAttrs), this.shouldSyncAttr && (this.syncVisData(), this.shouldSyncAttr = !1)), window._vis_debug && window.VWO.phoenix.off(_.PAGE_VIEW, e)
            }))
        }
        updateVisAttr(e) {
            if (!window._vis_heatmap) {
                for (const t in e) Object.prototype.hasOwnProperty.call(e, t) && (this.visitorAttrs[t] = e[t]);
                try {
                    Object.keys(this.visitorAttrs).length && this.setItem("_vwo_visProps", this.visitorAttrs)
                } catch (e) {}
            }
        }
        syncVisData() {
            var e;
            return i(this, void 0, void 0, (function*() {
                if (ys() || window._vis_heatmap || window.VWO._.ac.cSHS || (!Array.isArray(this.syncVisAttrs) || !(null === (e = this.syncVisAttrs) || void 0 === e ? void 0 : e.length)) && !this.customSyncURL) return;
                let t = yield Xr.getUUID(window.VWO.phoenix.store.getters), i = "";
                if (window._vis_debug) {
                    const e = Jn.get("_vwo_uuid", !1, !0);
                    if (!e) return;
                    Object.keys(window._vwo_exp || {}).length && (t = e, i = "&cId=" + Object.keys(window._vwo_exp).join("|"))
                }
                const n = this.customSyncURL || window._vwo_server_url || "https://dev.visualwebsiteoptimizer.com/",
                    s = "sync/attributes?a=" + window._vwo_acc_id + "&uuid=" + t + i;
                try {
                    const e = yield fetch(n + s), t = yield e.json();
                    this.visitorAttrs = t, this.visitorAttrs && Object.keys(this.visitorAttrs).length ? (this.setItem("_vwo_visProps", this.visitorAttrs), this.addAttrToGetters()) : this.removeItem("_vwo_visProps")
                } catch (e) {
                    Vr.warn("Error in syncing Historical Attributes Data.")
                }
            }))
        }
        addAttrToGetters() {
            for (const e in this.visitorAttrs) Object.prototype.hasOwnProperty.call(this.visitorAttrs, e) && window.VWO.phoenix.store.actions.set(e, this.visitorAttrs[e], "")
        }
    }
    const Ya = new za;

    function Ja(e, t, i) {
        const n = window.VWO;
        switch (e.toLowerCase()) {
            case "tags":
                n.phoenix.tags.add(t, i.fn);
                break;
            case "operators":
                n.phoenix.operators.add(i.fn);
                break;
            case "storages":
                n.phoenix.storages.add(i);
                break;
            case "store":
                n.phoenix.store.actions.addValues(i)
        }
    }
    class Xa {
        constructor(e) {
            this.toJSON = function() {
                const e = Object.assign({}, this);
                return delete e.modules, e
            }, Object.keys(e).forEach((t => {
                this[t] = e[t]
            }))
        }
        config(e) {
            return e && (this.configSettings = e), this.configSettings
        }
        definePlugin(e, t = {}) {
            const i = e.split(".")[0],
                n = e.split(".")[1],
                s = window.VWO;
            s.phoenix ? Ja(i, n, t) : (s.pluginStorage = s.pluginStorage || {}, s.pluginStorage[i] = s.pluginStorage[i] || {}, n ? (s.pluginStorage[i][n] = s.pluginStorage[i][n] || {}, s.pluginStorage[i][n] = Ua.mergeNestedObjects(s.pluginStorage[i][n], t)) : s.pluginStorage[i] = Ua.mergeNestedObjects(s.pluginStorage[i], t))
        }
        updateSettings(e, t, n) {
            var s, r, o, a, l, d;
            return i(this, void 0, void 0, (function*() {
                const i = window.VWO._.allSettings.dataStore.campaigns,
                    c = this.phoenix.store.getters,
                    u = null === (s = t.dataStore) || void 0 === s ? void 0 : s.campaigns,
                    g = Array.isArray(window._VWO.fSeg) ? window._VWO.fSeg : [];
                if (u)
                    for (const e in u)(i[e] || g.includes(e)) && delete u[e];
                const v = window.VWO._.allSettings.dataStore.changeSets || {},
                    h = (null === (r = t.dataStore) || void 0 === r ? void 0 : r.changeSets) || {};
                Object.assign(v, h);
                const p = JSON.parse(JSON.stringify(u)),
                    w = JSON.parse(JSON.stringify((null === (o = t.dataStore) || void 0 === o ? void 0 : o.syE) || {})),
                    E = JSON.parse(JSON.stringify((null === (a = t.dataStore) || void 0 === a ? void 0 : a.syV) || {})),
                    f = JSON.parse(JSON.stringify((null === (l = t.dataStore) || void 0 === l ? void 0 : l.cSE) || {})),
                    m = Object.keys(p);
                for (var O in u) window.VWO._.allSettings.dataStore.campaigns[O] = u[O], u[O] = window.VWO._.allSettings.dataStore.campaigns[O];
                this.phoenix.add(t), 1 !== n && 4 !== n || window.VWO._.phoenixMT.trigger("vwo_campaignsLoaded"), Xr.checkAndInitializeClickClass(), this.phoenix.store.actions.addValues({
                    currentSettings: this.phoenix.settings.currentSettings
                }, "root"), window.VWO._.allSettings.dataStore.syE = w, window.VWO._.allSettings.dataStore.syV = E, window.VWO._.allSettings.dataStore.cSE = f, window.VWO.modules.eventHistHandler.syncEvents = w, window.VWO.modules.eventHistHandler.currentSessionEvents = f, window.VWO.modules.eventHistHandler.addListenersForHistoricalEvents(), (window.VWO._.ac.cSHS || Array.isArray(E) && (null == E ? void 0 : E.length)) && Ya.addListenerAndEnableStorageSyncer(), Ba.clearHistoricalData(E, w), Fa(p);
                const S = La.setFunnelExps(this.phoenix.store.getters);
                m.push(...S), Xr.setCampaignIds(m), Xr.setThirdPartyCookiesForApplicableCamps(this.phoenix.store.getters);
                for (const e in window.VWO._.allSettings.dataStore.campaigns) Object.prototype.hasOwnProperty.call(window.VWO._.allSettings.dataStore.campaigns, e) && qo.serializeData(window.VWO._.allSettings.dataStore.campaigns[e]);
                if (window.fetcher.getValue("VWO._.loadNonTestingLibraries", [m, n]), (2 == n || 5 == n || 4 === n && c.previousUrl && c.previousUrl !== c.currentUrl) && (this.phoenix.trigger(yr.PAGE_LOAD_EVENT), yield Xr.firePageViewEvent({
                        shouldFireDomLoad: !0
                    })), (4 === n || 2 == n) && e)
                    for (var O in p) Xr.isSessionBasedCampaign2(p[O]) && (null === (d = window.VWO._.track) || void 0 === d ? void 0 : d.isUserBucketed()) && (yield or(this.phoenix, _._ACTIVATED, {
                        id: O
                    }));
                window.VWO.push(["setVariation"])
            }))
        }
        addPhoenix(e) {
            this.event = function(t, i, n) {
                bn() && (window._VWO.unsavedEventsHistory = window._VWO.unsavedEventsHistory || {}, window._VWO.unsavedEventsHistory[t] = window._VWO.unsavedEventsHistory[t] || [], window._VWO.unsavedEventsHistory[t].push(Object.assign(Object.assign({}, i), {
                    ts: Date.now(),
                    VWO: {
                        firedTime: this.ts
                    }
                }))), (i = i || {}).isCustomEvent = !0, i.page = zs.page, or(e, t, i, (() => {
                    var e;
                    window.VWO.modules.eventHistHandler.updateEventHist(t, i), bn() && (null === (e = window._VWO.unsavedEventsHistory) || void 0 === e || delete e[t]), null == n || n()
                }))
            }, this.phoenix = e
        }
        splice(...e) {
            const t = this.queue.splice.apply(this.queue, e);
            return this.length = this.queue.length, t
        }
        push(...e) {
            const t = this.queue.push.apply(this.queue, e);
            return this.length = this.queue.length, this[this.length - 1] = this.queue[this.queue.length - 1], t
        }
        sort(...e) {
            return this.queue.sort.apply(this.queue, e)
        }
        visitor(e) {
            if (!e) return;
            const t = e.$metaData;
            delete e.$metaData;
            for (const t in e) Object.prototype.hasOwnProperty.call(e, t) && this.phoenix.store.actions.set(t, e[t], "");
            or(this.phoenix, _.SYNC_VISITOR_PROP, {
                $visitor: {
                    props: e
                },
                $metaData: t,
                isCustomEvent: !0
            }, (() => {
                Ya.updateVisAttr(e)
            }))
        }
        syncAttributes() {
            Ya.syncVisData()
        }
        syncEvents() {
            window.VWO.modules.eventHistHandler.syncEventsData()
        }
    }
    class qa {
        otherSide(...e) {
            return e[0] = "VWO.modules.tags.backwardCompatibilityUtils." + e[0], e[2] && (e[2] = {
                captureGroups: e[2]
            }), window.fetcher.getValue(...e)
        }
    }
    class Qa {}
    const Za = {
        primary: (e, t, i = !1, n, s) => new Proxy(t, {
            construct(t, r) {
                this.store = this.store || ["1"];
                const o = new t(...r),
                    a = this.store.length;
                this.store.push(o);
                let l = r;
                i && (l = n(o)), Object.defineProperty(o, "otherSideCreated", {
                    value: !1,
                    enumerable: !1,
                    writable: !0
                }), o.otherSide = (...e) => o.otherSideCreated.then((() => o.otherSide(...e).then((e => e))));
                const d = {
                    type: "vwoClassInstanceBridge",
                    id: a,
                    args: l,
                    path: e
                };
                return o.otherSideCreated = new Promise((t => {
                    window.fetcher.request(d).send().then((i => {
                        o.otherSide = (...t) => {
                            const n = e.dest + "." + i + "." + t[0];
                            return t[0] = n, window.fetcher.getValue(...t)
                        }, t(null), s && s(i)
                    }))
                })), o
            },
            get(e, t) {
                return "symbol" == typeof t || isNaN(+t) ? e : this.store[t]
            }
        }),
        secondary: (e, t, i) => new Proxy(t, {
            construct(e, t) {
                this.store = this.store || ["1"];
                const n = new e(...t),
                    s = this.store.length;
                return this.store.push(n), i && i(n), [s, n]
            },
            get(e, t) {
                return "symbol" == typeof t || isNaN(+t) ? e : this.store[t]
            }
        })
    };

    function el(e) {
        const t = [];
        return e.forEach((e => {
            var i;
            null === (i = e.filters) || void 0 === i || i.forEach(((i, n) => {
                const s = i[0].substring(0, i[0].indexOf("."));
                if ("event" === s || "page" === s) {
                    const s = JSON.parse(JSON.stringify(i));
                    t.push({
                        condition: s,
                        triggerName: e.triggerName,
                        condId: e.id,
                        filterId: n
                    })
                }
            }))
        })), t
    }
    let tl, il = {
        promise: new Promise((function(e) {
            tl = e
        })),
        resolve: tl
    };
    class nl extends Qa {
        constructor(e) {
            super(), this.eventName = yr.CLICK_EVENT, this.attachedFilters = e && el(e) || [], window.VWO.isVwoClickClassInitialized = !0
        }
        initializeMT() {
            return [this.attachedFilters]
        }
        eventConditionsUpdate(e) {
            e && (this.attachedFilters = [...this.attachedFilters, ...el(e)], this.otherSide("eventConditionsUpdate", [this.attachedFilters]))
        }
        isGoalEligible(e, t, i) {
            return qo.isGoalEligible(e, window.VWO.phoenix.store.getters, window.VWO.phoenix)
        }
        registerConversion(e, t, n) {
            return i(this, void 0, void 0, (function*() {
                yield qo.registerConversion(window.VWO.phoenix.store.getters, t, n)
            }))
        }
        on(e) {
            il.promise.then((t => {
                this.otherSide("on", [e, {
                    serverUrl: t.serverUrl,
                    vwoUUID: t.vwoUUID,
                    useCapturePhase: t.settings.plugins.DACDNCONFIG.UCP
                }])
            }))
        }
        off() {}
        otherSide(...e) {
            throw new Error("entered into outdated otherSide")
        }
    }
    const sl = Za.primary({
        src: "VWO.modules.phoenixPlugins.events.predefinedEvents.ClickDomEvent",
        dest: "VWO.modules.phoenixPlugins.events.predefinedEvents.ClickDomEvent"
    }, nl, !0, (e => e.initializeMT()));
    let rl;
    window.VWO.modules.phoenixPlugins.events.predefinedEvents.ClickDomEvent = sl;
    let ol = {
        promise: new Promise((e => {
            rl = e
        })),
        resolve: rl
    };
    class al extends Qa {
        constructor(e) {
            super(), this.eventName = _.DOM_SUBMIT, this.attachedFilters = el(e)
        }
        initializeMT() {
            return [this.attachedFilters]
        }
        eventConditionsUpdate(e) {
            e && (this.attachedFilters = [...this.attachedFilters, ...el(e)], this.otherSide("eventConditionsUpdate", [this.attachedFilters]))
        }
        isGoalEligible(e, t, i) {
            return qo.isGoalEligible(e, window.VWO.phoenix.store.getters, window.VWO.phoenix)
        }
        registerConversion(e, t, n) {
            return i(this, void 0, void 0, (function*() {
                yield qo.registerConversion(window.VWO.phoenix.store.getters, t, n)
            }))
        }
        on(e) {
            ol.promise.then((t => {
                this.otherSide("on", [e, {
                    vwoUUID: t.vwoUUID,
                    useCapturePhase: t.settings.plugins.DACDNCONFIG.UCP
                }])
            }))
        }
        off() {}
        otherSide(...e) {
            throw new Error("entered into outdated otherSide")
        }
    }
    const ll = Za.primary({
        src: "VWO.modules.phoenixPlugins.events.predefinedEvents.SubmitDomEvent",
        dest: "VWO.modules.phoenixPlugins.events.predefinedEvents.SubmitDomEvent"
    }, al, !0, (e => e.initializeMT()));
    window.VWO.modules.phoenixPlugins.events.predefinedEvents.SubmitDomEvent = ll;
    const dl = function() {};
    class cl extends qa {
        sendRegisterCall(e, t, n, r, o) {
            return i(this, void 0, void 0, (function*() {
                if (r = r || dl, o.visDebug || ys()) return r(), !0;
                var i, a, l = yield Xr.createUUIDCookie2(o, n), d = yield Xr.extraData2(), c = "", u = null;
                if ((yield o.vwoInternalProperties.sessionInfoService.shouldSendSessionInfoInCall()) && (c = "&sId=" + (u = yield o.vwoInternalProperties.sessionInfoService.getSessionId()), yield o.vwoInternalProperties.sessionInfoService.setSNCookieValueByIndex2(Es.SESSION_SYNCED_STATE_INDEX, 1)), cs.includes(window._vwo_acc_id) && "l.gif" == e) {
                    let e = window.localStorage.getItem("vwo_newSessionCreated");
                    try {
                        e = JSON.parse(e) || {}
                    } catch (t) {
                        e = {}
                    }
                    const t = "new" == e.user;
                    if (null == e || delete e.user, t) {
                        const t = 0 == Object.keys(e).length,
                            i = !t && (null == e ? void 0 : e.sessionId) != u,
                            r = !t && (null == e ? void 0 : e.uuid) != l;
                        if (i || r || t) {
                            let a = "Session didn't create properly due to";
                            t && (a += " session call failure"), i && (a += " session id mismatch"), r && (a += " uuid mismatch"), s({
                                msg: a,
                                url: "utilsWT.ts",
                                source: JSON.stringify({
                                    lastSessionData: e,
                                    current: {
                                        uuid: l,
                                        sessionIdStore: u,
                                        sessionCookie: window.VWO._.cookies.get(Es.TRACK_SESSION_COOKIE_NAME)
                                    },
                                    expId: (null == n ? void 0 : n.id) || "not-found",
                                    cURL: o.currentUrl
                                })
                            })
                        }
                    }
                }
                const g = encodeURIComponent(d),
                    v = d;
                d = n.ps || void 0 === n.ps ? "&ed=" + g : "", i = "&s=" + (n.version >= 4 ? vr.data.vi && "new" === vr.data.vi.vt ? 1 : 2 : parseInt(((yield o.storages.storages.cookies.get("_vis_opt_s")) || "").split("|")[0], 10)), a = e + "?experiment_id=" + n.id + "&account_id=" + o.accountId + "&cu=" + encodeURIComponent(o.currentUrl) + "&combination=" + t + i + c + Xr.getUUIDString(l) + d;
                try {
                    d && JSON.parse(decodeURIComponent(g)).lt
                } catch (t) {
                    s({
                        msg: `extraData(ed) is not a JSON string [while sending call for '${e}']`,
                        url: "utilsWT.ts",
                        source: JSON.stringify({
                            extraData: v,
                            lt: (new Date).getTime(),
                            referrer: Ks.get(),
                            requestURL: a
                        })
                    })
                }
                Xr.isSessionBasedCampaign2(n) || "SURVEY" === n.type || (yield this.otherSide('sendRegisterCall("${{1}}", "${{2}}", "${{3}}")', null, [o, a, r]))
            }))
        }
        markRevenueGoal(e, t, n) {
            return i(this, void 0, void 0, (function*() {
                if (void 0 !== (yield window.fetcher.getValue("_vis_opt_revenue"))) {
                    n.trigger(_.CONVERT_REVENUE_GOALS_FOR_EXPERIMENT, {
                        oldArgs: [e.id, window._vis_opt_revenue]
                    });
                    const i = On(e.goals);
                    let s, r = i.length;
                    for (; r--;) s = i[r], "REVENUE_TRACKING" === e.goals[s].type && qo.isGoalEligible(e.goals[s], t, n) && (yield qo.registerConversion(t, s, e, window._vis_opt_revenue))
                }
            }))
        }
        pollForNewVisitor({
            vwoEvents: e,
            getters: t
        }) {
            const n = this;
            Xr.addListener(t, e, {
                eventName: _.SEND_NEW_VISITOR_CALL
            }, (function(e) {
                return i(this, void 0, void 0, (function*() {
                    const {
                        combination: i,
                        campaignData: s,
                        callbackFn: r
                    } = e;
                    yield n.sendRegisterCall("l.gif", i, s, r, t)
                }))
            }))
        }
        goalRevenuePoll({
            vwoEvents: e,
            getters: t
        }) {
            const n = this;
            Xr.addListener(t, e, {
                eventName: _.VARIATION_SHOWN
            }, (function(s) {
                return i(this, void 0, void 0, (function*() {
                    const i = t.settings.campaigns[s.props.id];
                    yield n.markRevenueGoal(i, t, e)
                }))
            }))
        }
        customGoalConversion(e, t) {
            return i(this, void 0, void 0, (function*() {
                const i = vr.phoenix.store.getters;
                if (isNaN(parseInt(e, 10))) return;
                vr.phoenix.trigger(_.CONVERT_GOAL_FOR_ALL_EXPERIMENTS, {
                    oldArgs: [e]
                }), or(vr.phoenix, _.CUSTOM_CONVERSION, {
                    gId: e,
                    ins: !0
                });
                var n, s = On(i.settings.campaigns);
                let r = s.length;
                for (; r--;) n = s[r], "object" == typeof i.settings.campaigns[n].goals[e] && "TRACK" === i.settings.campaigns[n].type === t && qo.isGoalEligible(i.settings.campaigns[n].goals[e], vr.phoenix.store.getters, vr.phoenix) && (yield qo.registerConversion(vr.phoenix.store.getters, e, i.settings.campaigns[n]))
            }))
        }
        customRevenueConversion(e, t) {
            return i(this, void 0, void 0, (function*() {
                const i = vr.phoenix.store.getters;
                if (isNaN(parseFloat(e))) return;
                var n, s, r;
                vr.phoenix.trigger(_.CONVERT_ALL_VISIT_GOALS_FOR_EXPERIMENT, {
                    oldArgs: [e]
                }), or(null, _.REVENUE_CONVERSION, {
                    revenue: e,
                    ins: !0
                });
                var o, a = On(i.settings.campaigns);
                let l = a.length;
                for (; l--;)
                    if (o = a[l], "TRACK" === i.settings.campaigns[o].type === t)
                        for (r = (s = On(i.settings.campaigns[o].goals)).length; r--;) n = s[r], "REVENUE_TRACKING" === i.settings.campaigns[o].goals[n].type && qo.isGoalEligible(i.settings.campaigns[o].goals[n], window.VWO.phoenix.store.getters, window.VWO.phoenix) && (yield qo.registerConversion(window.VWO.phoenix.store.getters, n, i.settings.campaigns[o], e))
            }))
        }
        delayedGoalConversion(e) {
            return i(this, void 0, void 0, (function*() {
                let t;
                const i = vr.phoenix,
                    n = i.store.getters,
                    s = n.settings.campaigns[e.campaignId];
                e = e || {};
                const r = window.tracklib;
                "TRACK" === e.type ? t = yield r.isGoalIncluded(e.goalId): "FUNNEL" === e.type ? t = yield r.isFunnelIncluded(e.campaignId): Xr.isAnalyzeCampaign(e.type) && (t = yield r.isAnalyzeCampaignIncluded(e.campaignId)), qo._triggerGoalConversion(i, n, e.goalId, s, e.revenue, !1, {
                    combination: t
                })
            }))
        }
        makeSessionAndTagCall(e, t) {
            const n = this;
            Xr.addListener(e, t, {
                eventName: _.DIMENSION_TAG_PUSHED
            }, (function(e) {
                return i(this, void 0, void 0, (function*() {
                    const {
                        props: t
                    } = e;
                    yield n.otherSide('makeCallForTagsAndSession("${{1}}", "${{2}}")', null, [t, "sessionUpdate"])
                }))
            }))
        }
        initialise({
            event: e,
            actions: t,
            vwoEvents: n,
            data: s,
            getters: r
        }) {
            return i(this, void 0, void 0, (function*() {
                for (const e in r.settings.campaigns) Object.prototype.hasOwnProperty.call(r.settings.campaigns, e) && qo.serializeData(r.settings.campaigns[e]);
                this.attachEventListeners(r), this.pollForNewVisitor({
                    event: e,
                    actions: t,
                    vwoEvents: n,
                    data: s,
                    getters: r
                }), this.goalRevenuePoll({
                    event: e,
                    actions: t,
                    vwoEvents: n,
                    data: s,
                    getters: r
                }), yield this.declareVWOAPI(), this.makeSessionAndTagCall(r, n)
            }))
        }
        attachEventListeners(e) {
            il.resolve(e), ol.resolve(e)
        }
        declareVWOAPI() {
            return i(this, void 0, void 0, (function*() {
                yield this.otherSide("declareVWOAPI()", null, [])
            }))
        }
    }
    window.VWO.modules.tags.activate = function() {
        return i(this, void 0, void 0, (function*() {
            const {
                getters: e,
                actions: t
            } = window.VWO.phoenix.store;
            Na.resetTriggerStates(), Na.resetExpParams(e, t), Na.resetTriggerExecutionCount(e), Xr.firePageViewEvent()
        }))
    };
    const ul = new cl;
    window.VWO.modules.tags.backwardCompatibilityUtils = ul;
    class gl {}

    function vl(...e) {
        return e[0] = "VWO.modules.tags." + e[0], e[2] && (e[2] = {
            captureGroups: e[2]
        }), window.fetcher.getValue(...e)
    }
    class hl extends gl {
        execute({
            event: e,
            actions: t,
            vwoEvents: n,
            data: s,
            getters: r
        }) {
            return i(this, void 0, void 0, (function*() {
                Xr.addListener(r, n, {
                    eventName: "*"
                }, (function(e, t) {
                    vl('wildCardCallback("${{1}}", "${{2}}")', null, [e, t])
                }));
                const i = ul.initialise({
                        event: e,
                        actions: t,
                        vwoEvents: n,
                        data: s,
                        getters: r
                    }),
                    o = window.fetcher.getValue("VWO.modules.tags.backwardCompatibility");
                yield Promise.all([i, o])
            }))
        }
    }
    const pl = new hl,
        wl = pl.execute;
    window.VWO.modules.tags.backwardCompatibility = wl;
    class El extends Qa {
        constructor(e, t, i) {
            super();
            const n = `${Or.DOM}_`;
            this.eventName = e, this.domEventsDebounceTime = i, this.domEventName = e.includes(n) ? e.substr(n.length) : e, this.attachedFilters = el(t)
        }
        initializeMT() {
            return [this.eventName, this.domEventName, this.domEventsDebounceTime, this.attachedFilters]
        }
        eventConditionsUpdate(e) {
            e && (this.attachedFilters = [...this.attachedFilters, ...el(e)], this.otherSide("eventConditionsUpdate", [this.attachedFilters]))
        }
        on(e) {
            this.otherSide("on", [e])
        }
        off() {
            this.otherSide("off", [null])
        }
        otherSide(...e) {
            throw new Error("entered into outdated otherSide")
        }
    }
    const fl = Za.primary({
        src: "VWO.modules.phoenixPlugins.events.predefinedEvents.GenericDOMEvent",
        dest: "VWO.modules.phoenixPlugins.events.predefinedEvents.GenericDOMEvent"
    }, El, !0, (e => e.initializeMT()));
    window.VWO.modules.phoenixPlugins.events.predefinedEvents.GenericDOMEvent = fl;
    class _l extends Qa {
        constructor({
            occurrence: e = 1e3
        } = {}) {
            super(), this.eventName = Sr.TIMER, this.occurrence = e, this.start = Date.now()
        }
        eventConditionsUpdate() {
            this.start = Date.now()
        }
        on(e) {
            this.interval = window.setInterval((() => {
                const t = {
                    time: Date.now(),
                    timeSpent: Math.floor((Date.now() - this.start) / 1e3)
                };
                e(t)
            }), this.occurrence)
        }
        off() {
            window.clearInterval(this.interval)
        }
    }
    class ml extends Qa {
        constructor(e) {
            super(), this.eventName = yr.URL_CHANGE, this.events = e || ["pushState", "replaceState", "hashchange", "popstate"]
        }
        on(e) {
            this.otherSide("on", [e])
        }
        off() {
            this.otherSide("off", [null])
        }
        otherSide(...e) {
            throw new Error("entered into outdated otherSide")
        }
    }
    const Ol = Za.primary({
        src: "VWO.modules.phoenixPlugins.events.predefinedEvents.UrlChangeEvent",
        dest: "VWO.modules.phoenixPlugins.events.predefinedEvents.UrlChangeEvent"
    }, ml);
    window.VWO.modules.phoenixPlugins.events.predefinedEvents.UrlChangeEvent = Ol;
    class Sl extends Qa {
        constructor() {
            super(...arguments), this.eventName = yr.LEAVE_INTENT
        }
        on(e) {
            this.otherSide("on", [e])
        }
        off() {
            this.otherSide("off", [null])
        }
        otherSide(...e) {
            throw new Error("entered into outdated otherSide")
        }
    }
    const yl = Za.primary({
        src: "VWO.modules.phoenixPlugins.events.predefinedEvents.LeaveIntentEvent",
        dest: "VWO.modules.phoenixPlugins.events.predefinedEvents.LeaveIntentEvent"
    }, Sl);
    window.VWO.modules.phoenixPlugins.events.predefinedEvents.LeaveIntentEvent = yl;
    class Tl extends Qa {
        constructor() {
            super(...arguments), this.eventName = yr.PAGE_LOAD_EVENT
        }
        on(e) {
            this.otherSide("on", [e])
        }
        off() {
            this.otherSide("off", [null])
        }
        otherSide(...e) {
            throw new Error("entered into outdated otherSide")
        }
    }
    const Il = Za.primary({
        src: "VWO.modules.phoenixPlugins.events.predefinedEvents.PageLoadEvent",
        dest: "VWO.modules.phoenixPlugins.events.predefinedEvents.PageLoadEvent"
    }, Tl);
    window.VWO.modules.phoenixPlugins.events.predefinedEvents.PageLoadEvent = Il;
    var Cl = Object.freeze({
        __proto__: null,
        GenericDomEvent: fl,
        TimeEvent: _l,
        UrlChangeEvent: Ol,
        LeaveIntentEvent: yl,
        ClickDomEvent: sl,
        SubmitDomEvent: ll,
        PageLoadEvent: Il
    });
    const Nl = e => !!e,
        Al = function(e, t, i) {
            if (e && t) {
                const n = e.countryCode,
                    s = e.city,
                    r = e.region;
                return Pl.f_in_list(n, t, i) || Pl.f_in_list(`${n}-${r}`, t, i) || Pl.f_in_list(`${n}-${r}-${s}`, t, i)
            }
        },
        Vl = function(e, t, i) {
            if (e && t) {
                const n = e.countryCode,
                    s = e.city,
                    r = e.region;
                return Pl.f_nin_list(n, t, i) && Pl.f_nin_list(`${n}-${r}`, t, i) && Pl.f_nin_list(`${n}-${r}-${s}`, t, i)
            }
        },
        bl = (e, t, i) => {
            if (!Nl(e)) return !1;
            const n = i.syncGet("fns.list", [e, t]);
            return !!n.dataPresent && n.val
        },
        Rl = (e, t, i) => {
            if ("" === e) return !0;
            if (!Nl(e)) return !1;
            const n = i.syncGet("fns.list", [e, t]);
            return !!n.dataPresent && !n.val
        },
        Pl = {
            f_in_list: bl,
            f_nin_list: Rl,
            f_in_loc: Al,
            f_nin_loc: Vl
        };

    function xl(e) {
        window.VWO._.contentSyncService = new Zn({
            strategy: ["ls"]
        }, e, window.VWO.data.content)
    }

    function Ll() {
        try {
            queueMicrotask((() => {}))
        } catch (e) {
            self.queueMicrotask = function(e) {
                Promise.resolve().then(e)
            }
        }
    }
    class Dl {
        constructor() {
            this.queue = {}
        }
        add(e, t) {
            this.queue[e] = this.queue[e] || [], this.queue[e].push(t), this.otherSide("addListener", [e])
        }
        processQueue(e, t) {
            this.queue[e] && this.queue[e].length && this.queue[e].forEach((e => {
                e(t)
            }))
        }
        otherSide(...e) {
            e[0] = "VWO.modules.utils.storageSyncer." + e[0], window.fetcher.getValue(...e)
        }
    }
    const Ul = new Dl;

    function Wl() {
        window.VWO.phoenix.on(_.CAMPAIGN_FLOW_START, (() => {
            window.VWO._.previewDebuggerBooted = new Promise((e => {
                const t = window.VWO.phoenix.on(_.TIB_DONE, (() => {
                    window.VWO.phoenix.off(_.TIB_DONE, t), e()
                }))
            }))
        }))
    }
    window.VWO.modules.utils.storageSyncer = Ul;
    const Ml = {
        BASE_PATH: "VWO._.phoenixMT",
        trigger(e, t) {
            t ? window.fetcher.getValue(`${this.BASE_PATH}.trigger("\${{1}}", "\${{2}}")`, null, {
                captureGroups: [e, t]
            }) : window.fetcher.getValue(`${this.BASE_PATH}.trigger`, [e, t])
        },
        getEventHistory(e) {
            return window.fetcher.getValue(`${this.BASE_PATH}.getEventHistory`, [e])
        }
    };
    window.functionWrapper = new Q, window.fetcher.init();
    const kl = window.VWO;
    window.VWO._.cookieInitWT = e => {
        $((() => Jn.setJar(e)))
    }, window.VWO._.phoenixMT = Ml, window.setVWO = e => {
        const t = new Xa(e);
        hr(t), pr(t), window.VWO.modules = kl.modules, Object.assign(window.VWO._, kl._)
    }, window.phoenixInstantiate = () => {
        var e;
        try {
            window = Object.assign(window, window.fakeWindow), delete window.fakeWindow, window.window = window, document = window.document;
            const t = window.VWO;
            window.VWO._.isWorkerThread = !0;
            const i = window.VWOSettings;
            if (i[1] = Object.assign(i[1], La.getPhoenixConfig()), window.VWO.consentMode) {
                const e = Jn.get;
                Jn.get = function(...t) {
                    const i = window.VWO.consentMode;
                    if (!i.dT) {
                        if (i.hT) {
                            const e = window.VWO.modules.utils.consentModeUtils.getGoalCookie(t[0]);
                            if (e) return e
                        }
                        return e.apply(Jn, t)
                    }
                }
            }
            i[1].storages.cookies = Jn;
            const n = new H;
            window._vwo_exp = window.VWO._.allSettings.dataStore.campaigns, window._vwo_exp = n.register(h.Object, "_vwo_exp", window._vwo_exp), window.VWO._.allSettings.dataStore.campaigns = window._vwo_exp, n.register(h.Document, "cookie"), window._vwo_surveySettings = n.register(h.Object, "_vwo_surveySettings", window._vwo_surveySettings), window.tracklib = n.register(h.Object, "", {}, "", !1), n.register("custom", "localStorage"), i[0] = window.VWO._.allSettings, t.definePlugin("props", zs), i[2] = t.pluginStorage, window.VWO.modules.eventHistHandler = $a, window.VWO.modules.eventHistHandler.initialize(), window.phoenix = new pn(i, {
                preDefinedEvents: Cl
            }), xl(window.phoenix.store.getters.serverUrl), window.phoenix.store.actions.addValues({
                currentSettings: window.phoenix.settings.currentSettings
            }, "root"), window.VWO.addPhoenix(window.phoenix), Xr.addListenerForSessionInitComplete(), Xr.isBot2() || Xr.checkAndInitializeClickClass(), window.phoenix.store.getters.flags = window.phoenix.store.getters.flags || {}, window.phoenix.store.getters.flags.cookieLessModeEnabled = !!window.VWO._.cLFE, Ya.initialize(), window.VWO.modules.eventHistHandler.clearHistEventData(), window.VWO.modules.eventHistHandler.addListenersForHistoricalEvents(), wl({
                event: void 0,
                data: void 0,
                getters: window.phoenix.store.getters,
                actions: window.phoenix.store.actions,
                vwoEvents: window.phoenix
            }), t.phoenix.operators.add("inlist", (function(e, t) {
                return !!Pl.f_in_list(e, t, window.VWO._.contentSyncService)
            })), t.phoenix.operators.add("ninlist", (function(e, t) {
                return !!Pl.f_nin_list(e, t, window.VWO._.contentSyncService)
            })), t.phoenix.operators.add("inloclist", (function(e, t) {
                return !!Pl.f_in_loc(e, t, window.VWO._.contentSyncService)
            })), t.phoenix.operators.add("ninloclist", (function(e, t) {
                return !Pl.f_nin_loc(e, t, window.VWO._.contentSyncService)
            })), window.isInsightsOnConsentEnabled && (t._.insightsOnConsentPromise = new Promise((e => {
                t.phoenix.on("trigger.InsightsOnConsentTrigger", e)
            }))), window.VWO.phoenix.store.getters.settings.campaigns = window._vwo_exp, i[1].storages.cookies.domain = window.VWO.phoenix.store.getters.cookieDomain;
            const s = window.phoenix.store.getters,
                r = s.settings.plugins,
                o = {
                    vwoData: {
                        accountJSInfo: null === (e = s.settings.vwoData) || void 0 === e ? void 0 : e.accountJSInfo
                    }
                };
            r && (o.plugins = {
                DACDNCONFIG: r.DACDNCONFIG
            }), window.phoenix.on(_.RETRACK_VISITOR, (() => {
                const {
                    getters: e,
                    actions: t
                } = window.VWO.phoenix.store;
                Na.resetExpParams(e, t, !0)
            })), Ll(), cr();
            return queueMicrotask((() => {
                Wl(), window.VWO._.visibilityTriggered = new Promise((e => {
                    const t = window.phoenix.on(_.VISIBILITY_TRIGGERED, (() => {
                        window.phoenix.off(t), e()
                    }))
                })), bn() && (window.VWO._.syncEventsCallCompleted = new Promise((e => {
                    const t = window.phoenix.on(_.SYNC_EVENTS_COMPLETED, (() => {
                        window.phoenix.off(t), e()
                    }))
                }))), window.VWO._.insightsUtils.activateFunnels(), bn() && or(t.phoenix, _.SEND_SYNC_CALL, {}), or(t.phoenix, _.POST_INIT, {}).then((() => {
                    window.fetcher.getValue("VWO.modules.utils.libUtils.initAuxiliaryPageView")
                }))
            })), {
                cookieDomain: s.cookieDomain,
                accountId: s.accountId,
                flags: s.flags,
                vwoInternalProperties: {},
                settings: o
            }
        } catch (e) {
            window.vwo_libExecuted = !0, window.fetcher.setValue("vwo_libExecuted", !0), window.fetcher.getValue("window._removeVwoGlobalStyle"), Vr.error("Error in phoenix initialization at worker:", e.stack)
        }
    }
})();