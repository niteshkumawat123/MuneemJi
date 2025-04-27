! function() {
    var e, t, n;
    VWO.v_t = "7.0.434", e = function() {
        return VWO && VWO._ && VWO._.libLoaded
    }, t = function() {
        ! function() {
            var w = {
                TRACK_PAGE_COOKIE_NAME: "_vwo_p",
                FUNNEL_EXPIRY: 100,
                INITIAL_PRICING_VERSION: 0,
                FEATURE_BUCKET_INDEX: 1,
                SAMPLING_VERSION_INDEX: 2,
                TRACK_GLOBAL_COOKIE_EXPIRY_STATE_INDEX: 3,
                FUNNEL_INFORMATION_INDEX: 2,
                GOAL_INFORMATION_INDEX: 3,
                ANALYZE_INFORMATION_INDEX: 4,
                CRO_START_TIMESTAMP_INDEX: 5,
                PAGE_ID_INFORMATION_INDEX: 1,
                ANALYSE_SERVER_NAME_INDEX: 2,
                TRACK_PAGE_ID_INFORMATION_INDEX: 3
            };

            function t() {
                var e = d.getDataStore();
                e && ((e = e.split(":"))[w.ANALYZE_INFORMATION_INDEX] = function(e) {
                    for (var t = a.getKeys(e), n = t.length, i = ""; n--;) i += t[n] + "_" + e[t[n]] + (0 === n ? "" : ",");
                    return i
                }(n.analyze), d.setDataStore(e.join(":")))
            }
            VWO._.commonUtil.extend(w, VWO._.CookieEnum);
            var e, a = VWO._.commonUtil,
                n = {
                    analyze: {}
                },
                r = 0,
                d = {
                    init: function() {
                        var e, t;
                        n.analyze = (t = {}, (e = d.getDataStore()) && (t = function(e) {
                            var t, n, i, a = {};
                            if (!e) return a;
                            for (e = e.split(","), r = 0; r < e.length; r++) n = (t = e[r].split("_"))[0], i = t[1], a[n] = i;
                            return a
                        }(e.split(":")[w.ANALYZE_INFORMATION_INDEX])), t)
                    },
                    includeAnalyzeCampaign: function(e) {
                        n.analyze[e] = "1", t()
                    },
                    excludeAnalyzeCampaign: function(e) {
                        n.analyze[e] = "0", t()
                    },
                    isAnalyzeCampaignIncluded: function(e) {
                        if ("1" === n.analyze[e] || 1 === n.analyze[e]) return "1"
                    },
                    isAnalyzeCampaignExcluded: function(e) {
                        return "0" === n.analyze[e] || 0 === n.analyze[e]
                    }
                };
            VWO._.commonUtil.extend(d, VWO._.commonCookieHandler);
            var o, s, _, S, V, A, p, u, l, i, c, T, m, g, R, E, I, O, N, f, v, C, L, W, D, F, G, k, h, y, U, x, M, P, X = VWO.TRACK_SESSION_COOKIE_EXPIRY_CUSTOM || 1 / 48,
                K = VWO.TRACK_GLOBAL_COOKIE_EXPIRY_CUSTOM || window.VWO.data.rp || 90,
                B = Math.min(K, 90);
            (e = {})._vis_opt_test_cookie = 0, e._vwo_ds = K, e._vwo_sn = X, e._vwo_referrer = 18e-5, e._vwo_uuid = 3650, e._vwo_uuid_v2 = 366, e._vwo_ssm = 3650, window.___vwo = 1, VWO._.track.loaded || (o = VWO._.utils, s = VWO._.CampaignEnum, _ = VWO._.GoalsEnum, S = VWO._.libUtils, V = VWO._.cookies, A = VWO._.EventsEnum, p = VWO._.triggerEvent, u = VWO._.commonUtil, l = VWO._.coreLib, i = VWO._.vwoLib, VWO._.campaign, c = VWO._.listener, T = VWO._.sessionInfoService, m = VWO._.tags, g = VWO._.localStorageService, R = window._vwo_acc_id, E = window._vwo_exp, I = window._vwo_exp_ids, w.FUNNEL_EXPIRY = VWO.FUNNEL_EXPIRY_CUSTOM || 100, f = N = O = !1, L = 1 / 0, W = void 0, (P = W = {})[P.DEFAULT_SAMPLING_RATE = -2] = "DEFAULT_SAMPLING_RATE", P[P.PLAN_EXIPRED = -1] = "PLAN_EXIPRED", D = {
                setUp: function() {
                    D.preProcessData()
                },
                init: function() {
                    return D.initiated || S.doesUuidCookiesExist() || (V.erase(w.TRACK_SESSION_COOKIE_NAME), V.erase(w.TRACK_GLOBAL_COOKIE_NAME)), D.expireGlobalCookie(), D.expireGoals(), D.createGlobalCookieReturnEligibility() ? (D.isUserBucketed() && !N ? (D.startSession(), N = !0) : p(A.RECORDING_NOT_ELIGIBLE, "USER_NOT_BUCKETED"), D.expireFunnels(u.getServerStartTimestamp(!0)), d.init(), D.initiated = !0, !(D.visitorRetracked = !1)) : (p(A.RECORDING_NOT_ELIGIBLE, "URL_NOT_MATCHING"), !1)
                },
                preProcessData: function() {
                    VWO.data.url = VWO.data.url || {}, VWO.data.url.i = VWO.data.url.i || ".*"
                },
                shouldExcludeUser: function() {
                    return 0 < navigator.userAgent.indexOf("MSIE") || 0 < navigator.userAgent.indexOf("Trident")
                },
                isUserEligible: function() {
                    return l.compareUrlWithIncludeExcludeRegex(l.getCurrentUrl(), VWO.data.url.i, VWO.data.url.e).didMatch
                },
                expireGlobalCookie: function() {
                    D.shouldExpireGlobalCookie() && (V.erase(w.TRACK_GLOBAL_COOKIE_NAME), V.erase(w.TRACK_SESSION_COOKIE_NAME))
                },
                getLatestSamplingVersion: function() {
                    return window.VWO.data.pvn || w.INITIAL_PRICING_VERSION
                },
                getCpt: function() {
                    return window.VWO.data.cpt || 0
                },
                updateTrackPageId: function() {
                    var e = D.getTrackPageId() + 1;
                    return D.markTrackPageId(e), e
                },
                getTrackPageId: function() {
                    var e = T.getSNCookieValueByIndex(w.TRACK_PAGE_ID_INFORMATION_INDEX);
                    return e ? parseInt(e, 10) : 0
                },
                markTrackPageId: function(e) {
                    T.setSNCookieValueByIndex(w.TRACK_PAGE_ID_INFORMATION_INDEX, e)
                },
                getCroStartTimestamp: function() {
                    return d.getDataInfoByIndex(w.CRO_START_TIMESTAMP_INDEX)
                },
                setCroStartTimestamp: function() {
                    var e = u.getCurrentTimestamp(!0) - T.getFirstSessionId();
                    d.setDataInfoByIndex(w.CRO_START_TIMESTAMP_INDEX, e)
                },
                shouldExpireGlobalCookie: function() {
                    var e = d.getDataStore(),
                        t = d.getMetaInfoByIndex(w.TRACK_GLOBAL_COOKIE_EXPIRY_STATE_INDEX);
                    if (e) {
                        var n = u.getServerStartTimestamp(!0),
                            i = T.getFirstSessionId(),
                            a = D.getCroStartTimestamp();
                        if (24 * w.TRACK_GLOBAL_COOKIE_EXPIRY * 60 * 60 + a + i < n) return !T.getSessionStore() || (t || d.setMetaInfoByIndex(w.TRACK_GLOBAL_COOKIE_EXPIRY_STATE_INDEX, 1), !1);
                        if (i < D.getCpt()) return !0;
                        var r = D.getLatestSamplingVersion(),
                            o = d.getMetaInfoByIndex(w.SAMPLING_VERSION_INDEX) || w.INITIAL_PRICING_VERSION;
                        if (!(o < r) && Math.abs(o) < Math.abs(r) && D.isUserBucketed()) return !0
                    }
                    return !1
                },
                _markFunnelValue: function(e, t, n) {
                    this._markFeatureValue(w.FUNNEL_INFORMATION_INDEX, e, [t, n, T.getRelativeSessionId(), E[e].v])
                },
                _isFunnelValue: function(e, t, n) {
                    return this._isFeatureValue(w.FUNNEL_INFORMATION_INDEX, e, [t, n])
                },
                expireFunnels: function(e) {
                    var t, n, i, a, r, o, s = d.getDataStore(),
                        _ = T.getFirstSessionId();
                    if (s) {
                        for (n = (t = (s = s.split(":"))[w.FUNNEL_INFORMATION_INDEX].split(",")).length; n--;) a = (i = t[n].split("_"))[0], r = +i[3] + 24 * w.FUNNEL_EXPIRY * 60 * 60 + _, o = +i[4], (r < e || E[a] && E[a].v > o) && t.splice(n, 1);
                        s[w.FUNNEL_INFORMATION_INDEX] = t.join(","), s = s.join(":"), d.setDataStore(s)
                    }
                },
                expireGoals: function() {
                    T.getSessionStore() && D.getTrackPageId() && !D.visitorRetracked || d.deleteDataStoreInfoByIndex(w.GOAL_INFORMATION_INDEX)
                },
                getSessionIdOfFunnel: function(e) {
                    var t = d.getDataStore().match(new RegExp("[:,]" + e + "_[^_]*_._([^_]*)_[^,:]*"));
                    return t && t[1] ? +t[1] + T.getFirstSessionId() : 0
                },
                _markFeatureValue: function(e, t, n, i) {
                    var a = i ? d.getMetaStore() : d.getDataStore(),
                        r = a.split(":"),
                        o = t,
                        s = r[e],
                        _ = r.length;
                    if (!s)
                        for (; _ <= e;) r[_] = "", _++;
                    var u = (s = r[e]).match(new RegExp("(?:^|,)(" + t + "_[^,]+)"));
                    void 0 === n && (n = []), n instanceof Array || (n = [n]);
                    for (var l = 0; l < n.length; l++) o += "_" + n[l];
                    r[e] = u ? r[e].replace(new RegExp("(^|,)(" + t + "_[^,]+)"), "$1" + o) : r[e] + (0 === r[e].length ? "" : ",") + o, a = r.join(":"), i ? d.setMetaStore(a) : d.setDataStore(a)
                },
                _isFeatureValue: function(e, t, n, i) {
                    var a, r, o = (i ? d.getMetaStore() : d.getDataStore()).split(":")[e];
                    return void 0 === n && (n = []), n instanceof Array || (n = [n]), !!(e === w.FUNNEL_INFORMATION_INDEX ? (r = n[1], a = (a = n[0]) || "[^_]*", r = null == r ? "." : r, new RegExp("(,|^)" + t + "_" + a + "_" + r)) : (r = null == (r = n[0]) ? "." : r, new RegExp("(,|^)" + t + "_" + r))).test(o) && "1"
                },
                _markGoalValue: function(e, t) {
                    this._markFeatureValue(w.GOAL_INFORMATION_INDEX, e, t)
                },
                _isGoalValue: function(e, t) {
                    return this._isFeatureValue(w.GOAL_INFORMATION_INDEX, e, t)
                },
                isCroEnabled: function() {
                    if (!V.get(w.TRACK_GLOBAL_COOKIE_NAME)) return !1;
                    var e = d.getMetaStore().split(":") || [];
                    return !(!e[w.FEATURE_BUCKET_INDEX] && !e[w.SAMPLING_VERSION_INDEX]) || void 0
                },
                createGlobalCookieReturnEligibility: function() {
                    if (D.shouldExcludeUser()) return !1;
                    if (!D.isCroEnabled()) {
                        if (!D.isUserEligible()) return !1;
                        V.get(w.TRACK_GLOBAL_COOKIE_NAME) || (S.createUUIDCookie(), T.createGlobalCookie()), D.setCroStartTimestamp()
                    }
                    return D.markFeatureLevelBucketing(), D.setSamplingVersion(), !0
                },
                markFeatureLevelBucketing: function() {
                    var e = window.VWO.track.sampleData,
                        t = x(),
                        n = T.getPcTraffic(),
                        i = window._vwo_pc_custom || window.VWO.data.pc,
                        a = u.getKeys(i),
                        r = a.length,
                        o = !1;
                    for (!window._vwo_pc_custom && window.VWO.data.pc.a !== W.PLAN_EXIPRED && t && (t.samplingRate !== W.DEFAULT_SAMPLING_RATE && (i.a = t.samplingRate, i.t = t.samplingRate), o = !D.isUserBucketed(), o = f ? o : o && 0 < e.length); r--;) D._isFeatureValue(w.FEATURE_BUCKET_INDEX, a[r], null, 1) && !VWO.data.eFSFI && !o || D._markFeatureValue(w.FEATURE_BUCKET_INDEX, a[r], +(n < i[a[r]]), !0);
                    D.fixConflictingSampling(a)
                },
                fixConflictingSampling: function(e) {
                    if (D._isFeatureValue(w.FEATURE_BUCKET_INDEX, "a", 1, 1) ^ D._isFeatureValue(w.FEATURE_BUCKET_INDEX, "t", 1, 1))
                        for (var t = e.length; t--;) D._markFeatureValue(w.FEATURE_BUCKET_INDEX, e[t], 1, !0)
                },
                setSamplingVersion: function() {
                    d.setMetaInfoByIndex(w.SAMPLING_VERSION_INDEX, D.getLatestSamplingVersion())
                },
                isUserBucketed: function() {
                    for (var e = window.VWO.data.pc, t = u.getKeys(e), n = t.length; n--;)
                        if (D.isFeatureBucketed(t[n])) return !0
                },
                isFeatureBucketed: function(e) {
                    return !e || D._isFeatureValue(w.FEATURE_BUCKET_INDEX, e, 1, !0)
                },
                excludeFunnel: function(e) {
                    D._markFunnelValue(e, 0, 0)
                },
                includeFunnel: function(e) {
                    D._markFunnelValue(e, 0, 1)
                },
                includeAnalyzeCampaign: function(e) {
                    d.includeAnalyzeCampaign(e)
                },
                excludeAnalyzeCampaign: function(e) {
                    d.excludeAnalyzeCampaign(e)
                },
                excludeGoal: function(e) {
                    D._markGoalValue(e, 0)
                },
                includeGoal: function(e) {
                    D._markGoalValue(e, 1)
                },
                shouldAddGoalInFunnel: function(e, t) {
                    t = parseInt(t, 10);
                    var n, i = D.getGoalIndexInFunnel(e, t);
                    if (i < 0) return !1;
                    for (var a, r, o, s = E[e].g[0].id === t, _ = d.getDataStore().split(":")[w.FUNNEL_INFORMATION_INDEX].split(","), u = _.length; u--;)
                        if ((a = _[u].split("_"))[0] === e) {
                            if (o = !0, r = +a[1], !+a[2]) return !1;
                            n = D.getGoalIndexInFunnel(e, r) + 1 === i
                        }
                    return s && !o && (l.runCampaigns({
                        keepElementLoadedRunning: !1,
                        expIds: [e],
                        isManual: !0
                    }, null, null, !0), D.isFunnelIncluded(e) && (n = !0)), n
                },
                getGoalIndexInFunnel: function(e, t) {
                    for (var n = 0; n < E[e].g.length; n++)
                        if (E[e].g[n].id === t) return n;
                    return -1
                },
                getGoalsString: function(e) {
                    for (var t = "", n = 0; n < e.length; n++) t = t + e[n].id + ("REVENUE_TRACKING" === e[n].type ? "_1" : "") + (n === e.length - 1 ? "" : ",");
                    return t
                },
                getGtAndF: function(e) {
                    for (var t, n, i = I.length, a = {}; i--;) t = I[i], E[t].type === s.FUNNEL_CAMPAIGN && this.shouldAddGoalInFunnel(t, e) && (D._markFunnelValue(t, e, 1), a[t] = this.getGoalsString(E[t].g) + ":" + D.getSessionIdOfFunnel(t));
                    return n = u.getKeys(a), "&gt=" + +!D.isGoalTriggered(e) + "_" + n.join(",") + "&f=" + o.jsonStringify(a)
                },
                startSession: function() {
                    var e, t, n, i, a, r, o, s, _, u, l, d, c = document.URL,
                        g = "s.gif?account_id=" + R + S.getUUIDString(S.createUUIDCookie()),
                        E = 1;
                    if (T.isSessionInfoSynced()) {
                        T.updateAndSyncPageId(), o = D.updateTrackPageId(), e = m.getTags(), t = m.getEgTags(), n = m.getFunnelTags && m.getFunnelTags();
                        try {
                            s = 6 == window._vwo_acc_id && e ? JSON.parse(e).si && Object.keys(JSON.parse(e).si).length : void 0
                        } catch (t) {
                            window.VWO._.customError && window.VWO._.customError({
                                msg: "Issue with tagValue",
                                url: "track-lib.ts",
                                lineno: 667,
                                colno: 5,
                                source: JSON.stringify({
                                    tagValue: e,
                                    lt: (new Date).getTime(),
                                    referrer: document.referrer
                                })
                            })
                        }
                        if (r = T.getSessionId(), E = VWO._.pageId, a = r > T.getFirstSessionId(), g = g + "&s=" + r + "&p=" + E, !window._vis_debug && (S.sendCall(g + (e ? "&tags=" + e : "") + (t ? "&eg=" + t : "") + (n ? "&fIds=" + n : "") + (s ? "&tagsLength=" + s : "") + (6 == window._vwo_acc_id && window.VWO.isSafari ? "&isSafari=" + window.VWO.isSafari : "") + "&update=1&cq=1" + (VWO.phoenix ? "&ttl=" + B : "")), 6 == window._vwo_acc_id)) {
                            _ = m.getTags(!0);
                            try {
                                u = 6 == window._vwo_acc_id && _ ? JSON.parse(_).si && Object.keys(JSON.parse(_).si).length : void 0
                            } catch (t) {
                                window.VWO._.customError && window.VWO._.customError({
                                    msg: "Issue with tagValue",
                                    url: "track-lib.ts",
                                    lineno: 667,
                                    colno: 5,
                                    source: JSON.stringify({
                                        tagValue: e,
                                        lt: (new Date).getTime(),
                                        referrer: document.referrer
                                    })
                                })
                            }
                            _ && S.sendCall(g + (e ? "&tags=" + _ : "") + (s ? "&tagsLength=" + u : "") + (6 == window._vwo_acc_id && window.VWO.isSafari ? "&isSafari=" + window.VWO.isSafari : "") + "&pageExitListener=" + (window.VWO && window.VWO.pageExitListener) + "&batch=true&fromTrack=true&update=1&cq=1" + (VWO.phoenix ? "&ttl=" + B : ""))
                        }
                    } else {
                        E = 1, m.refresh(), e = m.getTags(), t = m.getEgTags(), n = m.getFunnelTags && m.getFunnelTags();
                        try {
                            s = 6 == window._vwo_acc_id && e ? JSON.parse(e).si && Object.keys(JSON.parse(e).si).length : void 0
                        } catch (t) {
                            window.VWO._.customError && window.VWO._.customError({
                                msg: "Issue with tagValue",
                                url: "track-lib.ts",
                                lineno: 667,
                                colno: 5,
                                source: JSON.stringify({
                                    tagValue: e,
                                    lt: (new Date).getTime(),
                                    referrer: document.referrer
                                })
                            })
                        }
                        if (T.getSessionStore() ? (T.updateAndSyncPageId(), E = VWO._.pageId, T.setSNCookieValueByIndex(w.SESSION_SYNCED_STATE_INDEX, 1)) : (i = T.getRelativeSessionTimestamp(this), T.initializeSession(i + ":" + E + ":::1"), VWO._.pageId = E), o = D.updateTrackPageId(), g = g + "&s=" + (r = T.getSessionId()) + "&p=" + E, a = r > T.getFirstSessionId(), !window._vis_debug) {
                            var I, O = S.extraData(!0),
                                N = encodeURIComponent(O),
                                f = g + "&ed=" + N + "&cu=" + encodeURIComponent(T.getInfo().cu || c) + (e ? "&tags=" + e : "") + (t ? "&eg=" + t : "") + (n ? "&fIds=" + n : "") + (s ? "&tagsLength=" + s : "") + "&r=" + +a + (6 == window._vwo_acc_id && window.VWO.isSafari ? "&isSafari=" + window.VWO.isSafari : "") + "&cq=1" + (VWO.phoenix ? "&ttl=" + B : "");
                            if (S.sendCall(f), 6 == window._vwo_acc_id) {
                                _ = m.getTags(!0);
                                try {
                                    u = 6 == window._vwo_acc_id && _ ? JSON.parse(_).si && Object.keys(JSON.parse(_).si).length : void 0
                                } catch (t) {
                                    window.VWO._.customError && window.VWO._.customError({
                                        msg: "Issue with tagValue",
                                        url: "track-lib.ts",
                                        lineno: 667,
                                        colno: 5,
                                        source: JSON.stringify({
                                            tagValue: e,
                                            lt: (new Date).getTime(),
                                            referrer: document.referrer
                                        })
                                    })
                                }
                                _ && (I = g + "&ed=" + N + "&cu=" + encodeURIComponent(T.getInfo().cu || c) + (e ? "&tags=" + _ : "") + (s ? "&tagsLength=" + u : "") + "&r=" + +a + (6 == window._vwo_acc_id && window.VWO.isSafari ? "&isSafari=" + window.VWO.isSafari : "") + "&pageExitListener=" + (window.VWO && window.VWO.pageExitListener) + "&batch=true&fromTrack=true&cq=1" + (VWO.phoenix ? "&ttl=" + B : ""), S.sendCall(I))
                            }
                            try {
                                JSON.parse(decodeURIComponent(N)).lt
                            } catch (t) {
                                window.VWO._.customError && window.VWO._.customError({
                                    msg: "extraData(ed) is not a JSON string [while sending call for 's.gif']",
                                    url: "track-lib.ts",
                                    lineno: 688,
                                    colno: 5,
                                    source: JSON.stringify({
                                        extraData: O,
                                        lt: (new Date).getTime(),
                                        referrer: document.referrer,
                                        requestURL: f
                                    })
                                })
                            }
                        }
                    }(isNaN(r) || isNaN(E)) && window.VWO._.customError && (l = {
                        _vwo_sn: V.get("_vwo_sn"),
                        _vwo_ds: V.get("_vwo_ds")
                    }, d = "Error while sending s.gif: ", isNaN(r) && (d += "Session Id is NaN"), isNaN(E) && (d = d ? d + ", " : d, d += "Page Id is NaN", l.pageId = E), window.VWO._.customError({
                        msg: d,
                        url: "track-lib.ts",
                        source: JSON.stringify(l),
                        lineno: 640,
                        colno: 641
                    })), T.setVisitorInformation(), D.setAnalyzeServerName(), p(A.TRACK_SESSION_CREATED, r, E, a, 1 === o, o), T.updateSession()
                },
                setAnalyzeServerName: function() {
                    var e = T.getSNCookieValueByIndex(w.ANALYSE_SERVER_NAME_INDEX);
                    e ? window.VWO.data.asn = e : (window.VWO.data.as && T.setSNCookieValueByIndex(w.ANALYSE_SERVER_NAME_INDEX, window.VWO.data.as), window.VWO.data.asn = window.VWO.data.as || "dev.visualwebsiteoptimizer.com")
                },
                isGoalIncluded: function(e) {
                    return this._isGoalValue(e, 1) || this._isGoalValue(e, 2)
                },
                isGoalExcluded: function(e) {
                    return this._isGoalValue(e, 0)
                },
                isAnalyzeCampaignExcluded: function(e) {
                    return d.isAnalyzeCampaignExcluded(e)
                },
                isAnalyzeCampaignIncluded: function(e) {
                    return d.isAnalyzeCampaignIncluded(e)
                },
                isFunnelIncluded: function(e) {
                    return D._isFunnelValue(e, void 0, 1)
                },
                isFunnelExcluded: function(e) {
                    return D._isFunnelValue(e, void 0, 0)
                },
                markGoalTriggered: function(e, t) {
                    var n = E[e].goals[t];
                    D._markGoalValue(t, 2), n.type === _.SEPARATE_PAGE && (n.pageVisited = 1)
                },
                isGoalTriggered: function(e) {
                    return D._isGoalValue(e, 2)
                },
                shouldTriggerGoal: function(e, t) {
                    var n = E[e].goals[t],
                        i = !1;
                    if (D._isGoalValue(t, 0)) return !1;
                    if (D._isGoalValue(t, 1) && (i = !n.pageVisited), D._isGoalValue(t, 2) && (i = !1), !n.pageVisited && !i)
                        for (var a, r = I.length; r--;) a = I[r], E[a].type === s.FUNNEL_CAMPAIGN && this.shouldAddGoalInFunnel(a, t) && (i = !0);
                    return n.type === _.SEPARATE_PAGE && (n.pageVisited = !0), i
                },
                loaded: !0,
                initiated: !(P[P.EXCLUDED_SAMPLING = 0] = "EXCLUDED_SAMPLING")
            }, u.extend(VWO._.track, D), D = VWO._.track, F = function() {
                3 === VWO.data.tcVersion && !y || (D.setUp(), D.init() && l.runCampaigns({
                    keepElementLoadedRunning: !1,
                    expIds: u.filter(I, function(e) {
                        return S.isSessionBasedCampaign(e)
                    }),
                    isManual: !1
                }, null, null, !0))
            }, G = function() {
                VWO._.ac && VWO._.ac.cInstJS ? VWO._.addConsentTrigger(F) : F()
            }, k = function() {
                try {
                    var e = "_vwo_track_data_" + window._vwo_acc_id,
                        t = g.get(e);
                    if (t)
                        for (var n = o.jsonParse(t), i = Object.keys(n), a = i.length; a--;) window.VWO.push([n[i[a]][0], n[i[a]][1]]);
                    g.remove(e)
                } catch (e) {
                    var r = "[JSLIB_TRACK] Error - Check for persisited track data.";
                    window.VWO._.customError && window.VWO._.customError({
                        msg: r,
                        url: "track-lib.ts",
                        source: encodeURIComponent(r)
                    })
                }
            }, h = function() {
                (3 !== VWO.data.tcVersion || y) && U && (D.initiated || (k(), G(), i.init("track") && (D.vwoLibInitiated = !0)))
            }, x = function() {
                var e, t = {};
                if (!f && ((t = (e = window.VWO.track.sampleData) && e[0]) && t.samplingRate == W.EXCLUDED_SAMPLING && (L = t.priority), e))
                    for (var n = 1; n < e.length; n++) e[n].samplingRate == W.EXCLUDED_SAMPLING && e[n].priority < L && (L = e[n].priority), e[n].priority < t.priority && (t = e[n]);
                return f && (t.samplingRate = v, t.priority = C), t
            }, M = function() {
                N = !1, D.visitorRetracked = !0, D.init(), l.runCampaigns(!1, u.filter(I, function(t) {
                    try {
                        window._vwo_exp[t].combination_chosen
                    } catch (e) {
                        window.VWO._.customError && window.VWO._.customError({
                            msg: "LOGGING: experiment id " + t + "not found in " + window._vwo_exp_ids,
                            url: "track-lib.ts",
                            lineno: 879,
                            colno: 9
                        })
                    }
                    return window._vwo_exp && window._vwo_exp[t] ? S.isSessionBasedCampaign(t) && (window._vwo_exp[t].combination_chosen = void 0, 1) : S.isSessionBasedCampaign(t)
                }), null, !0), D.visitorRetracked = !1
            }, c.onEventReceive(A.RETRACK_VISITOR, function() {
                VWO._.ac && VWO._.ac.cInstJS ? VWO._.addConsentTrigger(M) : M()
            }), c.onEventReceive(A.NEW_SESSION_CREATED, function() {
                D.visitorRetracked = !0
            }), c.onEventReceive(A.POST_URL_CHANGE, function() {
                N = !(O = !0), VWO.phoenix || (G(), D.vwoLibInitiated || (k(), i.init("track")))
            }), c.onEventReceive(A.AFTER_SAMPLING_TRIGGER, function(e) {
                e && e[1] && e[1].samplingRate && (f = !0, v = e[1].samplingRate, (C = e[1].priority) < L && (0 != v || (L = C, 0))) ? G() : O && (G(), D.vwoLibInitiated || (k(), i.init("track")), O = !1)
            }), c.onEventReceive(A.NOT_REDIRECTING, function() {
                U || (U = !0, h())
            }), c.onEventReceive(A.UPDATE_SETTINGS_CALL, function() {
                y || (y = !0, h())
            }))
        }()
    }, (n = function() {
        e() ? t() : setTimeout(n, 100)
    })()
}();