(() => {
    "use strict";
    var e = {
            1101: (e, t, n) => {
                n.d(t, {
                    A: () => o
                });
                var r = n(1609);
                const o = e => (0, r.createElement)("svg", {
                    width: "25",
                    height: "24",
                    viewBox: "0 0 25 24",
                    fill: "none",
                    xmlns: "http://www.w3.org/2000/svg",
                    ...e
                }, (0, r.createElement)("mask", {
                    id: "mask0_8026_2609",
                    style: {
                        maskType: "alpha"
                    },
                    maskUnits: "userSpaceOnUse",
                    x: "0",
                    y: "0",
                    width: "25",
                    height: "24"
                }, (0, r.createElement)("rect", {
                    x: "0.5",
                    width: "24",
                    height: "24",
                    fill: "#D9D9D9"
                })), (0, r.createElement)("g", {
                    mask: "url(#mask0_8026_2609)"
                }, (0, r.createElement)("path", {
                    d: "M6.9 19L5.5 17.6L11.1 12L5.5 6.4L6.9 5L12.5 10.6L18.1 5L19.5 6.4L13.9 12L19.5 17.6L18.1 19L12.5 13.4L6.9 19Z",
                    fill: "#667085"
                })))
            },
            79: (e, t, n) => {
                n.d(t, {
                    A: () => o
                });
                var r = n(1609);
                const o = () => (0, r.createElement)("svg", {
                    xmlns: "http://www.w3.org/2000/svg",
                    width: "21",
                    height: "20",
                    viewBox: "0 0 21 20",
                    fill: "none"
                }, (0, r.createElement)("g", {
                    clipPath: "url(#clip0_7196_26074)"
                }, (0, r.createElement)("mask", {
                    id: "mask0_7196_26074",
                    style: {
                        maskType: "alpha"
                    },
                    maskUnits: "userSpaceOnUse",
                    x: "0",
                    y: "0",
                    width: "21",
                    height: "20"
                }, (0, r.createElement)("rect", {
                    x: "0.5",
                    width: "20",
                    height: "20",
                    fill: "#C4C4C4"
                })), (0, r.createElement)("g", {
                    mask: "url(#mask0_7196_26074)"
                }, (0, r.createElement)("path", {
                    d: "M7.99967 15.0013H12.9997C13.2358 15.0013 13.4337 14.9214 13.5934 14.7617C13.7531 14.602 13.833 14.4041 13.833 14.168C13.833 13.9319 13.7531 13.7339 13.5934 13.5742C13.4337 13.4145 13.2358 13.3346 12.9997 13.3346H7.99967C7.76356 13.3346 7.56565 13.4145 7.40592 13.5742C7.2462 13.7339 7.16634 13.9319 7.16634 14.168C7.16634 14.4041 7.2462 14.602 7.40592 14.7617C7.56565 14.9214 7.76356 15.0013 7.99967 15.0013ZM7.99967 11.668H12.9997C13.2358 11.668 13.4337 11.5881 13.5934 11.4284C13.7531 11.2687 13.833 11.0707 13.833 10.8346C13.833 10.5985 13.7531 10.4006 13.5934 10.2409C13.4337 10.0812 13.2358 10.0013 12.9997 10.0013H7.99967C7.76356 10.0013 7.56565 10.0812 7.40592 10.2409C7.2462 10.4006 7.16634 10.5985 7.16634 10.8346C7.16634 11.0707 7.2462 11.2687 7.40592 11.4284C7.56565 11.5881 7.76356 11.668 7.99967 11.668ZM5.49967 18.3346C5.04134 18.3346 4.64898 18.1714 4.32259 17.8451C3.9962 17.5187 3.83301 17.1263 3.83301 16.668V3.33464C3.83301 2.8763 3.9962 2.48394 4.32259 2.15755C4.64898 1.83116 5.04134 1.66797 5.49967 1.66797H11.4788C11.7011 1.66797 11.9129 1.70964 12.1143 1.79297C12.3156 1.8763 12.4927 1.99436 12.6455 2.14714L16.6872 6.1888C16.84 6.34158 16.958 6.51866 17.0413 6.72005C17.1247 6.92144 17.1663 7.13325 17.1663 7.35547V16.668C17.1663 17.1263 17.0031 17.5187 16.6768 17.8451C16.3504 18.1714 15.958 18.3346 15.4997 18.3346H5.49967ZM11.333 6.66797C11.333 6.90408 11.4129 7.102 11.5726 7.26172C11.7323 7.42144 11.9302 7.5013 12.1663 7.5013H15.4997L11.333 3.33464V6.66797Z",
                    fill: "#D0D5DD"
                }))), (0, r.createElement)("defs", null, (0, r.createElement)("clipPath", {
                    id: "clip0_7196_26074"
                }, (0, r.createElement)("rect", {
                    x: "0.5",
                    width: "20",
                    height: "20",
                    rx: "6.4",
                    fill: "white"
                }))))
            },
            7488: (e, t, n) => {
                n.d(t, {
                    A: () => o
                });
                var r = n(1609);
                const o = ({
                    content: e = "",
                    wordLimit: t
                }) => {
                    const [n, o] = (0, r.useState)(!1), s = (i = n ? e : ((e, t) => {
                        if (!e) return "";
                        const n = e.split(/\s+/);
                        return n.slice(0, t).join(" ") + (n.length > t ? "..." : "")
                    })(e, t), i ? i.replace(/<\/?p[^>]*>/g, "") : ""), a = ((c = e) ? c.trim().split(/\s+/).length : 0) > t;
                    var i, c;
                    return (0, r.createElement)("div", {
                        className: "content-sub"
                    }, (0, r.createElement)("p", null, (0, r.createElement)("span", {
                        dangerouslySetInnerHTML: {
                            __html: s
                        }
                    }), (0, r.createElement)("span", {
                        onClick: () => {
                            o(!n)
                        },
                        className: "show-more-btn",
                        style: {
                            cursor: "pointer"
                        }
                    }, a && !n && "Show More", n && "Show Less")))
                }
            },
            580: (e, t, n) => {
                n.d(t, {
                    A: () => o
                });
                var r = n(1609);
                const o = () => (0, r.createElement)("span", null, (0, r.createElement)("svg", {
                    xmlns: "http://www.w3.org/2000/svg",
                    width: "21",
                    height: "20",
                    viewBox: "0 0 21 20",
                    fill: "none"
                }, (0, r.createElement)("mask", {
                    id: "mask0_7196_26080",
                    style: {
                        maskType: "alpha"
                    },
                    maskUnits: "userSpaceOnUse",
                    x: "0",
                    y: "0",
                    width: "21",
                    height: "20"
                }, (0, r.createElement)("rect", {
                    x: "0.5",
                    width: "20",
                    height: "20",
                    fill: "#D9D9D9"
                })), (0, r.createElement)("g", {
                    mask: "url(#mask0_7196_26080)"
                }, (0, r.createElement)("path", {
                    d: "M12.1667 12.4993C12.4029 12.4993 12.6077 12.4125 12.7813 12.2389C12.9549 12.0653 13.0417 11.8605 13.0417 11.6243C13.0417 11.3882 12.9549 11.1834 12.7813 11.0098C12.6077 10.8362 12.4029 10.7493 12.1667 10.7493C11.9306 10.7493 11.7258 10.8362 11.5522 11.0098C11.3786 11.1834 11.2917 11.3882 11.2917 11.6243C11.2917 11.8605 11.3786 12.0653 11.5522 12.2389C11.7258 12.4125 11.9306 12.4993 12.1667 12.4993ZM12.1667 9.83268C12.3195 9.83268 12.4619 9.77713 12.5938 9.66602C12.7258 9.5549 12.8056 9.40907 12.8334 9.22852C12.8612 9.06185 12.9202 8.90907 13.0105 8.77018C13.1008 8.63129 13.264 8.44379 13.5001 8.20768C13.9167 7.79102 14.1945 7.45421 14.3334 7.19727C14.4723 6.94032 14.5417 6.63824 14.5417 6.29102C14.5417 5.66602 14.323 5.1556 13.8855 4.75977C13.448 4.36393 12.8751 4.16602 12.1667 4.16602C11.7084 4.16602 11.2917 4.27018 10.9167 4.47852C10.5417 4.68685 10.2431 4.98546 10.0209 5.37435C9.93758 5.51324 9.93064 5.65907 10.0001 5.81185C10.0695 5.96463 10.1876 6.07574 10.3542 6.14518C10.507 6.21463 10.6563 6.22157 10.8022 6.16602C10.948 6.11046 11.0695 6.01324 11.1667 5.87435C11.2917 5.69379 11.4376 5.55838 11.6042 5.4681C11.7709 5.37782 11.9584 5.33268 12.1667 5.33268C12.5001 5.33268 12.7709 5.42643 12.9792 5.61393C13.1876 5.80143 13.2917 6.0549 13.2917 6.37435C13.2917 6.56879 13.2362 6.75282 13.1251 6.92643C13.014 7.10004 12.8195 7.31879 12.5417 7.58268C12.139 7.9299 11.882 8.19727 11.7709 8.38477C11.6598 8.57227 11.5904 8.84657 11.5626 9.20768C11.5487 9.37435 11.6008 9.52018 11.7188 9.64518C11.8369 9.77018 11.9862 9.83268 12.1667 9.83268ZM7.16675 14.9993C6.70841 14.9993 6.31605 14.8362 5.98966 14.5098C5.66328 14.1834 5.50008 13.791 5.50008 13.3327V3.33268C5.50008 2.87435 5.66328 2.48199 5.98966 2.1556C6.31605 1.82921 6.70841 1.66602 7.16675 1.66602H17.1667C17.6251 1.66602 18.0174 1.82921 18.3438 2.1556C18.6702 2.48199 18.8334 2.87435 18.8334 3.33268V13.3327C18.8334 13.791 18.6702 14.1834 18.3438 14.5098C18.0174 14.8362 17.6251 14.9993 17.1667 14.9993H7.16675ZM3.83341 18.3327C3.37508 18.3327 2.98272 18.1695 2.65633 17.8431C2.32994 17.5167 2.16675 17.1243 2.16675 16.666V5.83268C2.16675 5.59657 2.24661 5.39865 2.40633 5.23893C2.56605 5.07921 2.76397 4.99935 3.00008 4.99935C3.23619 4.99935 3.43411 5.07921 3.59383 5.23893C3.75355 5.39865 3.83341 5.59657 3.83341 5.83268V16.666H14.6667C14.9029 16.666 15.1008 16.7459 15.2605 16.9056C15.4202 17.0653 15.5001 17.2632 15.5001 17.4993C15.5001 17.7355 15.4202 17.9334 15.2605 18.0931C15.1008 18.2528 14.9029 18.3327 14.6667 18.3327H3.83341Z",
                    fill: "#D0D5DD"
                }))))
            },
            4084: (e, t, n) => {
                n.d(t, {
                    A: () => o
                });
                var r = n(1609);
                const o = () => (0, r.createElement)("svg", {
                    xmlns: "http://www.w3.org/2000/svg",
                    width: "17",
                    height: "16",
                    viewBox: "0 0 17 16",
                    fill: "none"
                }, (0, r.createElement)("mask", {
                    id: "mask0_7196_26007",
                    style: {
                        maskType: "alpha"
                    },
                    maskUnits: "userSpaceOnUse",
                    x: "0",
                    y: "0",
                    width: "17",
                    height: "16"
                }, (0, r.createElement)("rect", {
                    x: "0.5",
                    width: "16",
                    height: "16",
                    fill: "#D0D5DD"
                })), (0, r.createElement)("g", {
                    mask: "url(#mask0_7196_26007)"
                }, (0, r.createElement)("path", {
                    d: "M3.1661 13.3327C2.79943 13.3327 2.48554 13.2021 2.22443 12.941C1.96332 12.6799 1.83276 12.366 1.83276 11.9993V3.99935C1.83276 3.63268 1.96332 3.31879 2.22443 3.05768C2.48554 2.79657 2.79943 2.66602 3.1661 2.66602H6.6161C6.79387 2.66602 6.96332 2.69935 7.12443 2.76602C7.28554 2.83268 7.42721 2.92713 7.54943 3.04935L8.49943 3.99935H13.8328C14.1994 3.99935 14.5133 4.1299 14.7744 4.39102C15.0355 4.65213 15.1661 4.96602 15.1661 5.33268V11.9993C15.1661 12.366 15.0355 12.6799 14.7744 12.941C14.5133 13.2021 14.1994 13.3327 13.8328 13.3327H3.1661Z",
                    fill: "#D0D5DD"
                })))
            },
            6679: (e, t, n) => {
                n.d(t, {
                    A: () => o
                });
                var r = n(1609);
                const o = e => {
                    const {
                        placeholder: t,
                        heading: n,
                        subheading: o,
                        headingTag: s = "h2",
                        subheadingTag: a = "h3",
                        buttonText: i,
                        searchButton: c
                    } = e;
                    return (0, r.createElement)("div", {
                        className: "betterdocs-search-layout-1"
                    }, (n || o) && (0, r.createElement)("div", {
                        className: "search-header"
                    }, n && (0, r.createElement)(s, {
                        className: "search-heading"
                    }, n), o && (0, r.createElement)(a, {
                        className: "search-subheading"
                    }, o)), (0, r.createElement)("div", {
                        className: "search-bar",
                        onClick: e.handleSearchFieldClick
                    }, (0, r.createElement)("div", {
                        className: "search-input-wrapper"
                    }, (0, r.createElement)("svg", {
                        className: "search-icon",
                        width: "20",
                        height: "20",
                        viewBox: "0 0 20 20",
                        fill: "none",
                        xmlns: "http://www.w3.org/2000/svg"
                    }, (0, r.createElement)("g", {
                        clipPath: "url(#clip0_7075_27802)"
                    }, (0, r.createElement)("path", {
                        d: "M14.4631 13.6407L17.8394 17.0162L16.724 18.1317L13.3485 14.7554C12.0925 15.7622 10.5303 16.3098 8.92061 16.3075C5.00435 16.3075 1.82593 13.1291 1.82593 9.21285C1.82593 5.29658 5.00435 2.11816 8.92061 2.11816C12.8369 2.11816 16.0153 5.29658 16.0153 9.21285C16.0176 10.8226 15.47 12.3848 14.4631 13.6407ZM12.8818 13.0558C13.8823 12.027 14.441 10.6479 14.4387 9.21285C14.4387 6.16371 11.969 3.69476 8.92061 3.69476C5.87147 3.69476 3.40252 6.16371 3.40252 9.21285C3.40252 12.2612 5.87147 14.7309 8.92061 14.7309C10.3557 14.7332 11.7347 14.1745 12.7636 13.174L12.8818 13.0558Z",
                        fill: "#98A2B3"
                    })), (0, r.createElement)("defs", null, (0, r.createElement)("clipPath", {
                        id: "clip0_7075_27802"
                    }, (0, r.createElement)("rect", {
                        width: "18.9192",
                        height: "18.9192",
                        fill: "white",
                        transform: "translate(0.248535 0.540039)"
                    })))), t && (0, r.createElement)("span", {
                        className: "search-input"
                    }, t)), i && c && (0, r.createElement)("span", {
                        className: "search-button"
                    }, i)))
                }
            },
            9895: (e, t, n) => {
                n.d(t, {
                    A: () => o
                });
                var r = n(1609);
                const o = ({
                    placeholder: e
                }) => (0, r.createElement)("div", {
                    className: "betterdocs-live-search betterdocs-sidebar-search betterdocs-search-popup"
                }, (0, r.createElement)("form", {
                    className: "betterdocs-searchform betterdocs-advance-searchform",
                    onSubmit: e => e.preventDefault()
                }, (0, r.createElement)("div", {
                    className: "betterdocs-searchform-input-wrap"
                }, (0, r.createElement)("svg", {
                    className: "doc-search-icon",
                    width: "28",
                    height: "28",
                    viewBox: "0 0 28 28",
                    fill: "none",
                    xmlns: "http://www.w3.org/2000/svg"
                }, (0, r.createElement)("g", {
                    clipPath: "url(#clip0_7196_25183)"
                }, (0, r.createElement)("path", {
                    d: "M19.0266 17.8469L22.5958 21.4152L21.4166 22.5943L17.8483 19.0252C16.5206 20.0895 14.8691 20.6684 13.1675 20.666C9.02748 20.666 5.66748 17.306 5.66748 13.166C5.66748 9.02602 9.02748 5.66602 13.1675 5.66602C17.3075 5.66602 20.6675 9.02602 20.6675 13.166C20.6699 14.8677 20.091 16.5191 19.0266 17.8469ZM17.355 17.2285C18.4126 16.1409 19.0032 14.683 19.0008 13.166C19.0008 9.94268 16.39 7.33268 13.1675 7.33268C9.94415 7.33268 7.33415 9.94268 7.33415 13.166C7.33415 16.3885 9.94415 18.9993 13.1675 18.9993C14.6845 19.0017 16.1424 18.4111 17.23 17.3535L17.355 17.2285Z",
                    fill: "#98A2B3"
                })), (0, r.createElement)("defs", null, (0, r.createElement)("clipPath", {
                    id: "clip0_7196_25183"
                }, (0, r.createElement)("rect", {
                    width: "20",
                    height: "20",
                    fill: "white",
                    transform: "translate(4 4)"
                })))), e && (0, r.createElement)("span", {
                    className: "betterdocs-search-command"
                }, e)), (0, r.createElement)("span", {
                    className: "command-key"
                }, "⌘ K")))
            },
            2055: (e, t, n) => {
                n.d(t, {
                    A: () => o
                });
                var r = n(1609);
                const o = () => (0, r.createElement)("svg", {
                    className: "doc-search-icon",
                    width: "28",
                    height: "28",
                    viewBox: "0 0 28 28",
                    fill: "none",
                    xmlns: "http://www.w3.org/2000/svg"
                }, (0, r.createElement)("g", {
                    clipPath: "url(#clip0_7196_25183)"
                }, (0, r.createElement)("path", {
                    d: "M19.0266 17.8469L22.5958 21.4152L21.4166 22.5943L17.8483 19.0252C16.5206 20.0895 14.8691 20.6684 13.1675 20.666C9.02748 20.666 5.66748 17.306 5.66748 13.166C5.66748 9.02602 9.02748 5.66602 13.1675 5.66602C17.3075 5.66602 20.6675 9.02602 20.6675 13.166C20.6699 14.8677 20.091 16.5191 19.0266 17.8469ZM17.355 17.2285C18.4126 16.1409 19.0032 14.683 19.0008 13.166C19.0008 9.94268 16.39 7.33268 13.1675 7.33268C9.94415 7.33268 7.33415 9.94268 7.33415 13.166C7.33415 16.3885 9.94415 18.9993 13.1675 18.9993C14.6845 19.0017 16.1424 18.4111 17.23 17.3535L17.355 17.2285Z",
                    fill: "#98A2B3"
                })), (0, r.createElement)("defs", null, (0, r.createElement)("clipPath", {
                    id: "clip0_7196_25183"
                }, (0, r.createElement)("rect", {
                    width: "20",
                    height: "20",
                    fill: "white",
                    transform: "translate(4 4)"
                }))))
            },
            8066: (e, t, n) => {
                n.d(t, {
                    A: () => o
                });
                var r = n(1609);
                const o = () => (0, r.createElement)("svg", {
                    width: "217",
                    height: "216",
                    viewBox: "0 0 217 216",
                    fill: "none",
                    xmlns: "http://www.w3.org/2000/svg"
                }, (0, r.createElement)("mask", {
                    id: "mask0_7201_2527",
                    style: {
                        maskType: "luminance"
                    },
                    maskUnits: "userSpaceOnUse",
                    x: "0",
                    y: "0",
                    width: "217",
                    height: "216"
                }, (0, r.createElement)("path", {
                    d: "M216.333 0H0.687012V215.646H216.333V0Z",
                    fill: "white"
                })), (0, r.createElement)("g", {
                    mask: "url(#mask0_7201_2527)"
                }, (0, r.createElement)("path", {
                    d: "M108.51 193.362C156.149 193.362 194.768 154.743 194.768 107.104C194.768 59.4649 156.149 20.8457 108.51 20.8457C60.8709 20.8457 22.2517 59.4649 22.2517 107.104C22.2517 154.743 60.8709 193.362 108.51 193.362Z",
                    fill: "#F2F4F7"
                }), (0, r.createElement)("path", {
                    d: "M76.1632 45.6855H142.68C149.033 45.6855 154.182 50.8344 154.182 57.1866V145.622C154.182 151.974 149.033 157.123 142.68 157.123H76.1632C69.811 157.123 64.6621 151.974 64.6621 145.622V57.1866C64.6621 50.8344 69.811 45.6855 76.1632 45.6855Z",
                    fill: "url(#paint0_linear_7201_2527)"
                }), (0, r.createElement)("path", {
                    d: "M76.0913 60.0625H104.988C106.933 60.0625 108.51 61.6396 108.51 63.5847C108.51 65.5298 106.933 67.1069 104.988 67.1069H76.0913C74.1462 67.1069 72.5691 65.5298 72.5691 63.5847C72.5691 61.6396 74.1462 60.0625 76.0913 60.0625Z",
                    fill: "#344054"
                }), (0, r.createElement)("path", {
                    d: "M76.0913 80.0449H140.929C142.874 80.0449 144.451 81.622 144.451 83.5671C144.451 85.5122 142.874 87.0893 140.929 87.0893H76.0913C74.1462 87.0893 72.5691 85.5122 72.5691 83.5671C72.5691 81.622 74.1462 80.0449 76.0913 80.0449Z",
                    fill: "#DCEAE9"
                }), (0, r.createElement)("path", {
                    d: "M76.0913 100.027H140.929C142.874 100.027 144.451 101.604 144.451 103.55C144.451 105.495 142.874 107.072 140.929 107.072H76.0913C74.1462 107.072 72.5691 105.495 72.5691 103.55C72.5691 101.604 74.1462 100.027 76.0913 100.027Z",
                    fill: "#DCEAE9"
                }), (0, r.createElement)("path", {
                    d: "M76.0913 120.01H140.929C142.874 120.01 144.451 121.587 144.451 123.532C144.451 125.477 142.874 127.054 140.929 127.054H76.0913C74.1462 127.054 72.5691 125.477 72.5691 123.532C72.5691 121.587 74.1462 120.01 76.0913 120.01Z",
                    fill: "#DCEAE9"
                }), (0, r.createElement)("path", {
                    d: "M76.0913 139.994H140.929C142.874 139.994 144.451 141.571 144.451 143.516C144.451 145.461 142.874 147.039 140.929 147.039H76.0913C74.1462 147.039 72.5691 145.461 72.5691 143.516C72.5691 141.571 74.1462 139.994 76.0913 139.994Z",
                    fill: "#DCEAE9"
                }), (0, r.createElement)("path", {
                    d: "M207.815 33.3711H164.628C162.616 33.3711 160.984 35.0876 160.984 37.206V58.5685C160.984 60.6862 162.616 62.4034 164.628 62.4034H207.815C209.828 62.4034 211.459 60.6862 211.459 58.5685V37.206C211.459 35.0876 209.828 33.3711 207.815 33.3711Z",
                    fill: "white"
                }), (0, r.createElement)("path", {
                    d: "M171.766 52.4735C174.148 52.4735 176.079 50.5427 176.079 48.1606C176.079 45.7784 174.148 43.8477 171.766 43.8477C169.384 43.8477 167.453 45.7784 167.453 48.1606C167.453 50.5427 169.384 52.4735 171.766 52.4735Z",
                    fill: "#C5D8D3"
                }), (0, r.createElement)("path", {
                    d: "M186.142 43.8477H200.519C202.901 43.8477 204.832 45.7784 204.832 48.1606C204.832 50.5427 202.901 52.4735 200.519 52.4735H186.142C183.76 52.4735 181.83 50.5427 181.83 48.1606C181.83 45.7784 183.76 43.8477 186.142 43.8477Z",
                    fill: "#DCEAE9"
                }), (0, r.createElement)("path", {
                    fillRule: "evenodd",
                    clipRule: "evenodd",
                    d: "M133.747 162.139C143.005 162.139 151.609 159.339 158.759 154.539L188.014 182.466L198.624 170.156L170.394 143.208C175.608 135.87 178.674 126.9 178.674 117.213C178.674 92.4018 158.559 72.2871 133.747 72.2871C108.935 72.2871 88.8213 92.4018 88.8213 117.213C88.8213 142.025 108.935 162.139 133.747 162.139ZM172.067 117.213C172.067 138.22 155.037 155.249 134.031 155.249C113.025 155.249 95.9965 138.22 95.9965 117.213C95.9965 96.2072 113.025 79.1784 134.031 79.1784C155.037 79.1784 172.067 96.2072 172.067 117.213Z",
                    fill: "#90B8B1"
                }), (0, r.createElement)("path", {
                    d: "M134.028 155.982C155.664 155.982 173.203 138.603 173.203 117.166C173.203 95.7285 155.664 78.3496 134.028 78.3496C112.391 78.3496 94.8523 95.7285 94.8523 117.166C94.8523 138.603 112.391 155.982 134.028 155.982Z",
                    fill: "white",
                    fillOpacity: "0.3"
                }), (0, r.createElement)("path", {
                    d: "M140.801 117.167L150.282 107.724C151.17 106.789 151.657 105.538 151.657 104.227C151.657 102.042 150.311 100.118 148.433 99.3831C146.556 98.6487 144.403 99.2656 143.166 100.842L134.034 112.34L124.902 100.842C123.665 99.2656 121.512 98.6487 119.635 99.3831C117.758 100.118 116.412 102.042 116.412 104.227C116.412 105.538 116.899 106.789 117.787 107.724L127.268 117.167L117.787 126.609C116.278 128.228 116.278 130.856 117.787 132.474C119.295 134.093 121.767 134.093 123.276 132.474L134.034 120.68L144.793 132.474C145.547 133.312 146.562 133.73 147.578 133.73C148.593 133.73 149.608 133.312 150.362 132.474C151.871 130.856 151.871 128.228 150.362 126.609L140.801 117.167Z",
                    fill: "#344054"
                }), (0, r.createElement)("defs", null, (0, r.createElement)("linearGradient", {
                    id: "paint0_linear_7201_2527",
                    x1: "64.6621",
                    y1: "57.6866",
                    x2: "155.982",
                    y2: "57.6866",
                    gradientUnits: "userSpaceOnUse"
                }, (0, r.createElement)("stop", {
                    stopColor: "white"
                }), (0, r.createElement)("stop", {
                    offset: "1",
                    stopColor: "#DCEAE9"
                })))))
            },
            4593: (e, t, n) => {
                n.d(t, {
                    A: () => c
                });
                var r = n(1609),
                    o = n(5728),
                    s = n(6679),
                    a = n(9895),
                    i = n(1296);
                const c = e => {
                    const [t, n] = (0, r.useState)(""), [c, l] = (0, r.useState)(!1), [u, d] = (0, r.useState)([]), [f, h] = (0, r.useState)([]), [p, m] = (0, r.useState)([]), {
                        layout: g,
                        placeholder: b,
                        heading: E,
                        subheading: y,
                        headingTag: w,
                        subheadingTag: C,
                        buttonText: v,
                        numberofdocs: S,
                        numberoffaqs: _,
                        doc_ids: A,
                        doc_categories_ids: O,
                        faq_categories_ids: R,
                        searchButton: T,
                        topWrapperRef: x = null
                    } = e, N = (0, r.useRef)(null);
                    (0, r.useEffect)((() => {
                        const e = e => {
                            "Escape" === e.key && l(!1), "k" === e.key && (e.metaKey || e.ctrlKey) && l(!0)
                        };
                        return document.addEventListener("keydown", e), () => {
                            document.removeEventListener("keydown", e)
                        }
                    }), []), (0, r.useEffect)((() => {
                        (async () => {
                            try {
                                let e = (await o.A.get("/wp-json/betterdocs/v1/get-terms?taxonomy=doc_category")).data;
                                1 == betterdocsSearchModalConfig.child_category_exclude && (e = e.filter((e => 0 === e.parent))), d(e)
                            } catch (e) {
                                console.error("Error fetching doc categories:", e)
                            }
                        })()
                    }), []), (0, r.useEffect)((() => {
                        (async () => {
                            try {
                                const e = await o.A.get(`/wp-json/betterdocs/v1/search?initial=true&per_page=${S}${A?`&doc_ids=${A}`:""}${O?`&doc_categories_ids=${O}`:""}`);
                                h(e.data);
                                const t = await o.A.get(`/wp-json/betterdocs/v1/search?initial=true&post_type=betterdocs_faq&per_page=${_}${R?`&faq_categories_ids=${R}`:""}`);
                                m(t.data)
                            } catch (e) {
                                console.error("Error fetching initial results:", e)
                            }
                        })()
                    }), []);
                    const L = () => {
                            l(!0)
                        },
                        k = () => {
                            l(!1)
                        },
                        D = e => {
                            N.current && !N.current.contains(e.target) && k()
                        };
                    return (0, r.useEffect)((() => (c ? document.addEventListener("click", D) : document.removeEventListener("click", D), () => {
                        document.removeEventListener("click", D)
                    })), [c]), (0, r.createElement)(r.Fragment, null, "betterdocs-search-modal-sidebar" === g && (0, r.createElement)("div", {
                        onClick: L
                    }, (0, r.createElement)(a.A, {
                        placeholder: b
                    })), "betterdocs-search-modal-layout-1" === g && (0, r.createElement)(s.A, {
                        handleSearchFieldClick: L,
                        placeholder: b,
                        heading: E,
                        subheading: y,
                        headingTag: w,
                        subheadingTag: C,
                        buttonText: v,
                        searchButton: T
                    }), c && (0, r.createElement)("div", {
                        ref: N
                    }, (0, r.createElement)(i.A, {
                        topWrapperRef: x,
                        initialSearchQuery: t,
                        closeModal: k,
                        docCategories: u,
                        popularDocs: f,
                        recentFaqs: p,
                        placeholder: b
                    })))
                }
            },
            1296: (e, t, n) => {
                n.d(t, {
                    A: () => m
                });
                var r = n(1609),
                    o = n(2619),
                    s = n(5728),
                    a = n(7723),
                    i = n(4084),
                    c = n(79),
                    l = n(2055),
                    u = n(580),
                    d = n(8066),
                    f = n(1101),
                    h = n(7488),
                    p = n(2068);
                const m = ({
                    initialSearchQuery: e,
                    closeModal: t,
                    docCategories: n,
                    popularDocs: m,
                    recentFaqs: g,
                    placeholder: b,
                    topWrapperRef: E = null
                }) => {
                    const [y, w] = (0, r.useState)(e || ""), [C, v] = (0, r.useState)(""), [S, _] = (0, r.useState)([]), [A, O] = (0, r.useState)("docs"), [R, T] = (0, r.useState)(!1), x = (0, r.useRef)(null), N = (0, r.useRef)({}), L = e => {
                        const t = document.createElement("textarea");
                        return t.innerHTML = e, t.value
                    }, k = (0, r.useCallback)((() => {
                        let e;
                        return (...t) => {
                            e && clearTimeout(e), e = setTimeout((() => (async (e, t = !1) => {
                                const n = `/wp-json/betterdocs/v1/search-insert?s=${e}${t?"&no_result=1":""}`;
                                try {
                                    await s.A.get(n)
                                } catch (e) {
                                    console.error("Error storing search keyword:", e)
                                }
                            })(...t)), 300)
                        }
                    })(), []), D = (0, r.useCallback)((async (e, t, n) => {
                        try {
                            let r = `/wp-json/betterdocs/v1/search?s=${e}`;
                            if ("docs" === n && t && (r += `&doc_category=${t}`), N.current[r]) _(N.current[r]);
                            else {
                                const t = (await s.A.get(r, {
                                    headers: {
                                        "X-WP-Nonce": searchModalConfig ? .nonce
                                    }
                                })).data;
                                N.current[r] = t, _(t), k(e, 0 === t.length)
                            }
                        } catch (e) {
                            console.error("Error fetching search results:", e)
                        }
                    }), [k]);
                    (0, r.useEffect)((() => {
                        y.length >= betterdocsSearchModalConfig.search_letter_limit || C ? (T(!0), D(y, C, A)) : (T(!1), _("docs" === A ? m : g))
                    }), [y, C, A, D, m, g]);
                    const P = e => S.filter((t => t.post_type === e)),
                        j = P("betterdocs_faq").length > 0,
                        B = [{
                            label: (0, a.__)("Docs", "betterdocs"),
                            type: "docs",
                            icon: (0, r.createElement)(c.A, null)
                        }, ...j ? [{
                            label: (0, a.__)("FAQ", "betterdocs"),
                            type: "betterdocs_faq",
                            icon: (0, r.createElement)(u.A, null)
                        }] : []];
                    return (0, r.useEffect)((() => {
                        x.current && x.current.focus()
                    }), []), (0, r.createElement)("div", {
                        className: "betterdocs-modal-overlay",
                        onClick: () => {
                            t()
                        }
                    }, (0, r.createElement)("div", {
                        className: "betterdocs-search-wrapper",
                        style: {
                            display: "block"
                        }
                    }, (0, r.createElement)("div", {
                        className: "betterdocs-search-details",
                        onClick: e => {
                            e.stopPropagation()
                        }
                    }, (0, r.createElement)("div", {
                        className: "xmark"
                    }, (0, r.createElement)(p.A, {
                        onClick: t,
                        width: "20",
                        height: "20",
                        fill: "#475467"
                    })), (0, r.createElement)("div", {
                        className: "betterdocs-search-header"
                    }, (0, r.createElement)(l.A, null), (0, r.createElement)("div", {
                        className: "betterdocs-searchform-input-wrap"
                    }, (0, r.createElement)("input", {
                        type: "text",
                        className: "betterdocs-search-field",
                        "aria-label": (0, a.__)("Search Input", "betterdocs"),
                        placeholder: b,
                        value: y,
                        onChange: e => w(e.target.value),
                        ref: x
                    }), y ? (0, r.createElement)(f.A, {
                        className: "clear-icon",
                        onClick: e => {
                            e.stopPropagation(), w(""), T(!1), x.current.focus()
                        }
                    }) : (0, r.createElement)("span", {
                        className: "esc-button"
                    }, (0, a.__)("ESC", "betterdocs"))), betterdocsSearchModalConfig.advance_search && n.length > 0 && "docs" === A && (0, o.applyFilters)("betterdocs_search_modal_category_select", [], [C, e => {
                        x.current && x.current.value.trim() && v(e ? e.value : "")
                    }, n, E])), betterdocsSearchModalConfig.advance_search && (0, o.applyFilters)("betterdocs_search_modal_popular_keyword", [], [w, T, C, A, D, E]), (0, r.createElement)("div", {
                        className: "betterdocs-search-content"
                    }, (0, r.createElement)("div", {
                        className: "betterdocs-search-info-tab betterdocs-search-tabs"
                    }, B.map((e => (0, r.createElement)("div", {
                        key: e.type,
                        className: "betterdocs-tab-items " + (A === e.type ? "active" : ""),
                        onClick: () => O(e.type)
                    }, e.icon && (0, r.createElement)("span", {
                        className: "tab-icon"
                    }, e.icon), (0, r.createElement)("span", null, e.label))))), (0, r.createElement)("div", {
                        className: "betterdocs-search-items-wrapper betterdocs-search-tab-content-wrapper"
                    }, P(A).length > 0 ? P(A).map(((e, t) => (0, r.createElement)("div", {
                        className: "betterdocs-search-item-content",
                        key: t
                    }, (0, r.createElement)("div", {
                        className: "betterdocs-search-item-list"
                    }, (0, r.createElement)("div", {
                        className: "content-main"
                    }, (0, r.createElement)(c.A, null), e.permalink && "docs" === e.post_type ? (0, r.createElement)("a", {
                        className: "hasperma",
                        href: e.permalink
                    }, (0, r.createElement)("h4", null, L(e.title))) : (0, r.createElement)("h4", null, L(e.title))), "betterdocs_faq" === e.post_type && (0, r.createElement)(h.A, {
                        content: e.content,
                        wordLimit: 30
                    }), "docs" === e.post_type && (0, r.createElement)("div", {
                        className: "content-sub"
                    }, (0, r.createElement)(i.A, null), (0, r.createElement)("h5", null, e.taxonomies && e.taxonomies.length > 0 ? L(e.taxonomies) : "No taxonomy")))))) : R && (0, r.createElement)("div", {
                        className: "betterdocs-search-items-not-found"
                    }, (0, r.createElement)(d.A, null), (0, r.createElement)("h4", null, "docs" === A ? (0, a.__)("No Docs found for", "betterdocs") : (0, a.__)("No FAQ found for", "betterdocs"), ' "', y, '"')))))))
                }
            },
            2068: (e, t, n) => {
                n.d(t, {
                    A: () => o
                });
                var r = n(1609);
                const o = e => (0, r.createElement)("svg", {
                    xmlns: "http://www.w3.org/2000/svg",
                    viewBox: "0 0 384 512",
                    width: e.width || "24",
                    height: e.height || "24",
                    fill: e.fill || "currentColor",
                    ...e
                }, (0, r.createElement)("path", {
                    d: "M342.6 150.6c12.5-12.5 12.5-32.8 0-45.3s-32.8-12.5-45.3 0L192 210.7 86.6 105.4c-12.5-12.5-32.8-12.5-45.3 0s-12.5 32.8 0 45.3L146.7 256 41.4 361.4c-12.5 12.5-12.5 32.8 0 45.3s32.8 12.5 45.3 0L192 301.3 297.4 406.6c12.5 12.5 32.8 12.5 45.3 0s12.5-32.8 0-45.3L237.3 256 342.6 150.6z"
                }))
            },
            1609: e => {
                e.exports = window.React
            },
            2619: e => {
                e.exports = window.wp.hooks
            },
            7723: e => {
                e.exports = window.wp.i18n
            },
            5728: (e, t, n) => {
                n.d(t, {
                    A: () => yt
                });
                var r = {};

                function o(e, t) {
                    return function() {
                        return e.apply(t, arguments)
                    }
                }
                n.r(r), n.d(r, {
                    hasBrowserEnv: () => he,
                    hasStandardBrowserEnv: () => me,
                    hasStandardBrowserWebWorkerEnv: () => ge,
                    navigator: () => pe,
                    origin: () => be
                });
                const {
                    toString: s
                } = Object.prototype, {
                    getPrototypeOf: a
                } = Object, i = (c = Object.create(null), e => {
                    const t = s.call(e);
                    return c[t] || (c[t] = t.slice(8, -1).toLowerCase())
                });
                var c;
                const l = e => (e = e.toLowerCase(), t => i(t) === e),
                    u = e => t => typeof t === e,
                    {
                        isArray: d
                    } = Array,
                    f = u("undefined"),
                    h = l("ArrayBuffer"),
                    p = u("string"),
                    m = u("function"),
                    g = u("number"),
                    b = e => null !== e && "object" == typeof e,
                    E = e => {
                        if ("object" !== i(e)) return !1;
                        const t = a(e);
                        return !(null !== t && t !== Object.prototype && null !== Object.getPrototypeOf(t) || Symbol.toStringTag in e || Symbol.iterator in e)
                    },
                    y = l("Date"),
                    w = l("File"),
                    C = l("Blob"),
                    v = l("FileList"),
                    S = l("URLSearchParams"),
                    [_, A, O, R] = ["ReadableStream", "Request", "Response", "Headers"].map(l);

                function T(e, t, {
                    allOwnKeys: n = !1
                } = {}) {
                    if (null == e) return;
                    let r, o;
                    if ("object" != typeof e && (e = [e]), d(e))
                        for (r = 0, o = e.length; r < o; r++) t.call(null, e[r], r, e);
                    else {
                        const o = n ? Object.getOwnPropertyNames(e) : Object.keys(e),
                            s = o.length;
                        let a;
                        for (r = 0; r < s; r++) a = o[r], t.call(null, e[a], a, e)
                    }
                }

                function x(e, t) {
                    t = t.toLowerCase();
                    const n = Object.keys(e);
                    let r, o = n.length;
                    for (; o-- > 0;)
                        if (r = n[o], t === r.toLowerCase()) return r;
                    return null
                }
                const N = "undefined" != typeof globalThis ? globalThis : "undefined" != typeof self ? self : "undefined" != typeof window ? window : global,
                    L = e => !f(e) && e !== N,
                    k = (D = "undefined" != typeof Uint8Array && a(Uint8Array), e => D && e instanceof D);
                var D;
                const P = l("HTMLFormElement"),
                    j = (({
                        hasOwnProperty: e
                    }) => (t, n) => e.call(t, n))(Object.prototype),
                    B = l("RegExp"),
                    U = (e, t) => {
                        const n = Object.getOwnPropertyDescriptors(e),
                            r = {};
                        T(n, ((n, o) => {
                            let s;
                            !1 !== (s = t(n, o, e)) && (r[o] = s || n)
                        })), Object.defineProperties(e, r)
                    },
                    F = "abcdefghijklmnopqrstuvwxyz",
                    M = "0123456789",
                    q = {
                        DIGIT: M,
                        ALPHA: F,
                        ALPHA_DIGIT: F + F.toUpperCase() + M
                    },
                    H = l("AsyncFunction"),
                    I = (Z = "function" == typeof setImmediate, z = m(N.postMessage), Z ? setImmediate : z ? (V = `axios@${Math.random()}`, $ = [], N.addEventListener("message", (({
                        source: e,
                        data: t
                    }) => {
                        e === N && t === V && $.length && $.shift()()
                    }), !1), e => {
                        $.push(e), N.postMessage(V, "*")
                    }) : e => setTimeout(e));
                var Z, z, V, $;
                const W = "undefined" != typeof queueMicrotask ? queueMicrotask.bind(N) : "undefined" != typeof process && process.nextTick || I,
                    J = {
                        isArray: d,
                        isArrayBuffer: h,
                        isBuffer: function(e) {
                            return null !== e && !f(e) && null !== e.constructor && !f(e.constructor) && m(e.constructor.isBuffer) && e.constructor.isBuffer(e)
                        },
                        isFormData: e => {
                            let t;
                            return e && ("function" == typeof FormData && e instanceof FormData || m(e.append) && ("formdata" === (t = i(e)) || "object" === t && m(e.toString) && "[object FormData]" === e.toString()))
                        },
                        isArrayBufferView: function(e) {
                            let t;
                            return t = "undefined" != typeof ArrayBuffer && ArrayBuffer.isView ? ArrayBuffer.isView(e) : e && e.buffer && h(e.buffer), t
                        },
                        isString: p,
                        isNumber: g,
                        isBoolean: e => !0 === e || !1 === e,
                        isObject: b,
                        isPlainObject: E,
                        isReadableStream: _,
                        isRequest: A,
                        isResponse: O,
                        isHeaders: R,
                        isUndefined: f,
                        isDate: y,
                        isFile: w,
                        isBlob: C,
                        isRegExp: B,
                        isFunction: m,
                        isStream: e => b(e) && m(e.pipe),
                        isURLSearchParams: S,
                        isTypedArray: k,
                        isFileList: v,
                        forEach: T,
                        merge: function e() {
                            const {
                                caseless: t
                            } = L(this) && this || {}, n = {}, r = (r, o) => {
                                const s = t && x(n, o) || o;
                                E(n[s]) && E(r) ? n[s] = e(n[s], r) : E(r) ? n[s] = e({}, r) : d(r) ? n[s] = r.slice() : n[s] = r
                            };
                            for (let e = 0, t = arguments.length; e < t; e++) arguments[e] && T(arguments[e], r);
                            return n
                        },
                        extend: (e, t, n, {
                            allOwnKeys: r
                        } = {}) => (T(t, ((t, r) => {
                            n && m(t) ? e[r] = o(t, n) : e[r] = t
                        }), {
                            allOwnKeys: r
                        }), e),
                        trim: e => e.trim ? e.trim() : e.replace(/^[\s\uFEFF\xA0]+|[\s\uFEFF\xA0]+$/g, ""),
                        stripBOM: e => (65279 === e.charCodeAt(0) && (e = e.slice(1)), e),
                        inherits: (e, t, n, r) => {
                            e.prototype = Object.create(t.prototype, r), e.prototype.constructor = e, Object.defineProperty(e, "super", {
                                value: t.prototype
                            }), n && Object.assign(e.prototype, n)
                        },
                        toFlatObject: (e, t, n, r) => {
                            let o, s, i;
                            const c = {};
                            if (t = t || {}, null == e) return t;
                            do {
                                for (o = Object.getOwnPropertyNames(e), s = o.length; s-- > 0;) i = o[s], r && !r(i, e, t) || c[i] || (t[i] = e[i], c[i] = !0);
                                e = !1 !== n && a(e)
                            } while (e && (!n || n(e, t)) && e !== Object.prototype);
                            return t
                        },
                        kindOf: i,
                        kindOfTest: l,
                        endsWith: (e, t, n) => {
                            e = String(e), (void 0 === n || n > e.length) && (n = e.length), n -= t.length;
                            const r = e.indexOf(t, n);
                            return -1 !== r && r === n
                        },
                        toArray: e => {
                            if (!e) return null;
                            if (d(e)) return e;
                            let t = e.length;
                            if (!g(t)) return null;
                            const n = new Array(t);
                            for (; t-- > 0;) n[t] = e[t];
                            return n
                        },
                        forEachEntry: (e, t) => {
                            const n = (e && e[Symbol.iterator]).call(e);
                            let r;
                            for (;
                                (r = n.next()) && !r.done;) {
                                const n = r.value;
                                t.call(e, n[0], n[1])
                            }
                        },
                        matchAll: (e, t) => {
                            let n;
                            const r = [];
                            for (; null !== (n = e.exec(t));) r.push(n);
                            return r
                        },
                        isHTMLForm: P,
                        hasOwnProperty: j,
                        hasOwnProp: j,
                        reduceDescriptors: U,
                        freezeMethods: e => {
                            U(e, ((t, n) => {
                                if (m(e) && -1 !== ["arguments", "caller", "callee"].indexOf(n)) return !1;
                                const r = e[n];
                                m(r) && (t.enumerable = !1, "writable" in t ? t.writable = !1 : t.set || (t.set = () => {
                                    throw Error("Can not rewrite read-only method '" + n + "'")
                                }))
                            }))
                        },
                        toObjectSet: (e, t) => {
                            const n = {},
                                r = e => {
                                    e.forEach((e => {
                                        n[e] = !0
                                    }))
                                };
                            return d(e) ? r(e) : r(String(e).split(t)), n
                        },
                        toCamelCase: e => e.toLowerCase().replace(/[-_\s]([a-z\d])(\w*)/g, (function(e, t, n) {
                            return t.toUpperCase() + n
                        })),
                        noop: () => {},
                        toFiniteNumber: (e, t) => null != e && Number.isFinite(e = +e) ? e : t,
                        findKey: x,
                        global: N,
                        isContextDefined: L,
                        ALPHABET: q,
                        generateString: (e = 16, t = q.ALPHA_DIGIT) => {
                            let n = "";
                            const {
                                length: r
                            } = t;
                            for (; e--;) n += t[Math.random() * r | 0];
                            return n
                        },
                        isSpecCompliantForm: function(e) {
                            return !!(e && m(e.append) && "FormData" === e[Symbol.toStringTag] && e[Symbol.iterator])
                        },
                        toJSONObject: e => {
                            const t = new Array(10),
                                n = (e, r) => {
                                    if (b(e)) {
                                        if (t.indexOf(e) >= 0) return;
                                        if (!("toJSON" in e)) {
                                            t[r] = e;
                                            const o = d(e) ? [] : {};
                                            return T(e, ((e, t) => {
                                                const s = n(e, r + 1);
                                                !f(s) && (o[t] = s)
                                            })), t[r] = void 0, o
                                        }
                                    }
                                    return e
                                };
                            return n(e, 0)
                        },
                        isAsyncFn: H,
                        isThenable: e => e && (b(e) || m(e)) && m(e.then) && m(e.catch),
                        setImmediate: I,
                        asap: W
                    };

                function K(e, t, n, r, o) {
                    Error.call(this), Error.captureStackTrace ? Error.captureStackTrace(this, this.constructor) : this.stack = (new Error).stack, this.message = e, this.name = "AxiosError", t && (this.code = t), n && (this.config = n), r && (this.request = r), o && (this.response = o, this.status = o.status ? o.status : null)
                }
                J.inherits(K, Error, {
                    toJSON: function() {
                        return {
                            message: this.message,
                            name: this.name,
                            description: this.description,
                            number: this.number,
                            fileName: this.fileName,
                            lineNumber: this.lineNumber,
                            columnNumber: this.columnNumber,
                            stack: this.stack,
                            config: J.toJSONObject(this.config),
                            code: this.code,
                            status: this.status
                        }
                    }
                });
                const G = K.prototype,
                    X = {};
                ["ERR_BAD_OPTION_VALUE", "ERR_BAD_OPTION", "ECONNABORTED", "ETIMEDOUT", "ERR_NETWORK", "ERR_FR_TOO_MANY_REDIRECTS", "ERR_DEPRECATED", "ERR_BAD_RESPONSE", "ERR_BAD_REQUEST", "ERR_CANCELED", "ERR_NOT_SUPPORT", "ERR_INVALID_URL"].forEach((e => {
                    X[e] = {
                        value: e
                    }
                })), Object.defineProperties(K, X), Object.defineProperty(G, "isAxiosError", {
                    value: !0
                }), K.from = (e, t, n, r, o, s) => {
                    const a = Object.create(G);
                    return J.toFlatObject(e, a, (function(e) {
                        return e !== Error.prototype
                    }), (e => "isAxiosError" !== e)), K.call(a, e.message, t, n, r, o), a.cause = e, a.name = e.name, s && Object.assign(a, s), a
                };
                const Q = K;

                function Y(e) {
                    return J.isPlainObject(e) || J.isArray(e)
                }

                function ee(e) {
                    return J.endsWith(e, "[]") ? e.slice(0, -2) : e
                }

                function te(e, t, n) {
                    return e ? e.concat(t).map((function(e, t) {
                        return e = ee(e), !n && t ? "[" + e + "]" : e
                    })).join(n ? "." : "") : t
                }
                const ne = J.toFlatObject(J, {}, null, (function(e) {
                        return /^is[A-Z]/.test(e)
                    })),
                    re = function(e, t, n) {
                        if (!J.isObject(e)) throw new TypeError("target must be an object");
                        t = t || new FormData;
                        const r = (n = J.toFlatObject(n, {
                                metaTokens: !0,
                                dots: !1,
                                indexes: !1
                            }, !1, (function(e, t) {
                                return !J.isUndefined(t[e])
                            }))).metaTokens,
                            o = n.visitor || l,
                            s = n.dots,
                            a = n.indexes,
                            i = (n.Blob || "undefined" != typeof Blob && Blob) && J.isSpecCompliantForm(t);
                        if (!J.isFunction(o)) throw new TypeError("visitor must be a function");

                        function c(e) {
                            if (null === e) return "";
                            if (J.isDate(e)) return e.toISOString();
                            if (!i && J.isBlob(e)) throw new Q("Blob is not supported. Use a Buffer instead.");
                            return J.isArrayBuffer(e) || J.isTypedArray(e) ? i && "function" == typeof Blob ? new Blob([e]) : Buffer.from(e) : e
                        }

                        function l(e, n, o) {
                            let i = e;
                            if (e && !o && "object" == typeof e)
                                if (J.endsWith(n, "{}")) n = r ? n : n.slice(0, -2), e = JSON.stringify(e);
                                else if (J.isArray(e) && function(e) {
                                    return J.isArray(e) && !e.some(Y)
                                }(e) || (J.isFileList(e) || J.endsWith(n, "[]")) && (i = J.toArray(e))) return n = ee(n), i.forEach((function(e, r) {
                                !J.isUndefined(e) && null !== e && t.append(!0 === a ? te([n], r, s) : null === a ? n : n + "[]", c(e))
                            })), !1;
                            return !!Y(e) || (t.append(te(o, n, s), c(e)), !1)
                        }
                        const u = [],
                            d = Object.assign(ne, {
                                defaultVisitor: l,
                                convertValue: c,
                                isVisitable: Y
                            });
                        if (!J.isObject(e)) throw new TypeError("data must be an object");
                        return function e(n, r) {
                            if (!J.isUndefined(n)) {
                                if (-1 !== u.indexOf(n)) throw Error("Circular reference detected in " + r.join("."));
                                u.push(n), J.forEach(n, (function(n, s) {
                                    !0 === (!(J.isUndefined(n) || null === n) && o.call(t, n, J.isString(s) ? s.trim() : s, r, d)) && e(n, r ? r.concat(s) : [s])
                                })), u.pop()
                            }
                        }(e), t
                    };

                function oe(e) {
                    const t = {
                        "!": "%21",
                        "'": "%27",
                        "(": "%28",
                        ")": "%29",
                        "~": "%7E",
                        "%20": "+",
                        "%00": "\0"
                    };
                    return encodeURIComponent(e).replace(/[!'()~]|%20|%00/g, (function(e) {
                        return t[e]
                    }))
                }

                function se(e, t) {
                    this._pairs = [], e && re(e, this, t)
                }
                const ae = se.prototype;
                ae.append = function(e, t) {
                    this._pairs.push([e, t])
                }, ae.toString = function(e) {
                    const t = e ? function(t) {
                        return e.call(this, t, oe)
                    } : oe;
                    return this._pairs.map((function(e) {
                        return t(e[0]) + "=" + t(e[1])
                    }), "").join("&")
                };
                const ie = se;

                function ce(e) {
                    return encodeURIComponent(e).replace(/%3A/gi, ":").replace(/%24/g, "$").replace(/%2C/gi, ",").replace(/%20/g, "+").replace(/%5B/gi, "[").replace(/%5D/gi, "]")
                }

                function le(e, t, n) {
                    if (!t) return e;
                    const r = n && n.encode || ce;
                    J.isFunction(n) && (n = {
                        serialize: n
                    });
                    const o = n && n.serialize;
                    let s;
                    if (s = o ? o(t, n) : J.isURLSearchParams(t) ? t.toString() : new ie(t, n).toString(r), s) {
                        const t = e.indexOf("#"); - 1 !== t && (e = e.slice(0, t)), e += (-1 === e.indexOf("?") ? "?" : "&") + s
                    }
                    return e
                }
                const ue = class {
                        constructor() {
                            this.handlers = []
                        }
                        use(e, t, n) {
                            return this.handlers.push({
                                fulfilled: e,
                                rejected: t,
                                synchronous: !!n && n.synchronous,
                                runWhen: n ? n.runWhen : null
                            }), this.handlers.length - 1
                        }
                        eject(e) {
                            this.handlers[e] && (this.handlers[e] = null)
                        }
                        clear() {
                            this.handlers && (this.handlers = [])
                        }
                        forEach(e) {
                            J.forEach(this.handlers, (function(t) {
                                null !== t && e(t)
                            }))
                        }
                    },
                    de = {
                        silentJSONParsing: !0,
                        forcedJSONParsing: !0,
                        clarifyTimeoutError: !1
                    },
                    fe = {
                        isBrowser: !0,
                        classes: {
                            URLSearchParams: "undefined" != typeof URLSearchParams ? URLSearchParams : ie,
                            FormData: "undefined" != typeof FormData ? FormData : null,
                            Blob: "undefined" != typeof Blob ? Blob : null
                        },
                        protocols: ["http", "https", "file", "blob", "url", "data"]
                    },
                    he = "undefined" != typeof window && "undefined" != typeof document,
                    pe = "object" == typeof navigator && navigator || void 0,
                    me = he && (!pe || ["ReactNative", "NativeScript", "NS"].indexOf(pe.product) < 0),
                    ge = "undefined" != typeof WorkerGlobalScope && self instanceof WorkerGlobalScope && "function" == typeof self.importScripts,
                    be = he && window.location.href || "http://localhost",
                    Ee = { ...r,
                        ...fe
                    },
                    ye = function(e) {
                        function t(e, n, r, o) {
                            let s = e[o++];
                            if ("__proto__" === s) return !0;
                            const a = Number.isFinite(+s),
                                i = o >= e.length;
                            return s = !s && J.isArray(r) ? r.length : s, i ? (J.hasOwnProp(r, s) ? r[s] = [r[s], n] : r[s] = n, !a) : (r[s] && J.isObject(r[s]) || (r[s] = []), t(e, n, r[s], o) && J.isArray(r[s]) && (r[s] = function(e) {
                                const t = {},
                                    n = Object.keys(e);
                                let r;
                                const o = n.length;
                                let s;
                                for (r = 0; r < o; r++) s = n[r], t[s] = e[s];
                                return t
                            }(r[s])), !a)
                        }
                        if (J.isFormData(e) && J.isFunction(e.entries)) {
                            const n = {};
                            return J.forEachEntry(e, ((e, r) => {
                                t(function(e) {
                                    return J.matchAll(/\w+|\[(\w*)]/g, e).map((e => "[]" === e[0] ? "" : e[1] || e[0]))
                                }(e), r, n, 0)
                            })), n
                        }
                        return null
                    },
                    we = {
                        transitional: de,
                        adapter: ["xhr", "http", "fetch"],
                        transformRequest: [function(e, t) {
                            const n = t.getContentType() || "",
                                r = n.indexOf("application/json") > -1,
                                o = J.isObject(e);
                            if (o && J.isHTMLForm(e) && (e = new FormData(e)), J.isFormData(e)) return r ? JSON.stringify(ye(e)) : e;
                            if (J.isArrayBuffer(e) || J.isBuffer(e) || J.isStream(e) || J.isFile(e) || J.isBlob(e) || J.isReadableStream(e)) return e;
                            if (J.isArrayBufferView(e)) return e.buffer;
                            if (J.isURLSearchParams(e)) return t.setContentType("application/x-www-form-urlencoded;charset=utf-8", !1), e.toString();
                            let s;
                            if (o) {
                                if (n.indexOf("application/x-www-form-urlencoded") > -1) return function(e, t) {
                                    return re(e, new Ee.classes.URLSearchParams, Object.assign({
                                        visitor: function(e, t, n, r) {
                                            return Ee.isNode && J.isBuffer(e) ? (this.append(t, e.toString("base64")), !1) : r.defaultVisitor.apply(this, arguments)
                                        }
                                    }, t))
                                }(e, this.formSerializer).toString();
                                if ((s = J.isFileList(e)) || n.indexOf("multipart/form-data") > -1) {
                                    const t = this.env && this.env.FormData;
                                    return re(s ? {
                                        "files[]": e
                                    } : e, t && new t, this.formSerializer)
                                }
                            }
                            return o || r ? (t.setContentType("application/json", !1), function(e) {
                                if (J.isString(e)) try {
                                    return (0, JSON.parse)(e), J.trim(e)
                                } catch (e) {
                                    if ("SyntaxError" !== e.name) throw e
                                }
                                return (0, JSON.stringify)(e)
                            }(e)) : e
                        }],
                        transformResponse: [function(e) {
                            const t = this.transitional || we.transitional,
                                n = t && t.forcedJSONParsing,
                                r = "json" === this.responseType;
                            if (J.isResponse(e) || J.isReadableStream(e)) return e;
                            if (e && J.isString(e) && (n && !this.responseType || r)) {
                                const n = !(t && t.silentJSONParsing) && r;
                                try {
                                    return JSON.parse(e)
                                } catch (e) {
                                    if (n) {
                                        if ("SyntaxError" === e.name) throw Q.from(e, Q.ERR_BAD_RESPONSE, this, null, this.response);
                                        throw e
                                    }
                                }
                            }
                            return e
                        }],
                        timeout: 0,
                        xsrfCookieName: "XSRF-TOKEN",
                        xsrfHeaderName: "X-XSRF-TOKEN",
                        maxContentLength: -1,
                        maxBodyLength: -1,
                        env: {
                            FormData: Ee.classes.FormData,
                            Blob: Ee.classes.Blob
                        },
                        validateStatus: function(e) {
                            return e >= 200 && e < 300
                        },
                        headers: {
                            common: {
                                Accept: "application/json, text/plain, */*",
                                "Content-Type": void 0
                            }
                        }
                    };
                J.forEach(["delete", "get", "head", "post", "put", "patch"], (e => {
                    we.headers[e] = {}
                }));
                const Ce = we,
                    ve = J.toObjectSet(["age", "authorization", "content-length", "content-type", "etag", "expires", "from", "host", "if-modified-since", "if-unmodified-since", "last-modified", "location", "max-forwards", "proxy-authorization", "referer", "retry-after", "user-agent"]),
                    Se = Symbol("internals");

                function _e(e) {
                    return e && String(e).trim().toLowerCase()
                }

                function Ae(e) {
                    return !1 === e || null == e ? e : J.isArray(e) ? e.map(Ae) : String(e)
                }

                function Oe(e, t, n, r, o) {
                    return J.isFunction(r) ? r.call(this, t, n) : (o && (t = n), J.isString(t) ? J.isString(r) ? -1 !== t.indexOf(r) : J.isRegExp(r) ? r.test(t) : void 0 : void 0)
                }
                class Re {
                    constructor(e) {
                        e && this.set(e)
                    }
                    set(e, t, n) {
                        const r = this;

                        function o(e, t, n) {
                            const o = _e(t);
                            if (!o) throw new Error("header name must be a non-empty string");
                            const s = J.findKey(r, o);
                            (!s || void 0 === r[s] || !0 === n || void 0 === n && !1 !== r[s]) && (r[s || t] = Ae(e))
                        }
                        const s = (e, t) => J.forEach(e, ((e, n) => o(e, n, t)));
                        if (J.isPlainObject(e) || e instanceof this.constructor) s(e, t);
                        else if (J.isString(e) && (e = e.trim()) && !/^[-_a-zA-Z0-9^`|~,!#$%&'*+.]+$/.test(e.trim())) s((e => {
                            const t = {};
                            let n, r, o;
                            return e && e.split("\n").forEach((function(e) {
                                o = e.indexOf(":"), n = e.substring(0, o).trim().toLowerCase(), r = e.substring(o + 1).trim(), !n || t[n] && ve[n] || ("set-cookie" === n ? t[n] ? t[n].push(r) : t[n] = [r] : t[n] = t[n] ? t[n] + ", " + r : r)
                            })), t
                        })(e), t);
                        else if (J.isHeaders(e))
                            for (const [t, r] of e.entries()) o(r, t, n);
                        else null != e && o(t, e, n);
                        return this
                    }
                    get(e, t) {
                        if (e = _e(e)) {
                            const n = J.findKey(this, e);
                            if (n) {
                                const e = this[n];
                                if (!t) return e;
                                if (!0 === t) return function(e) {
                                    const t = Object.create(null),
                                        n = /([^\s,;=]+)\s*(?:=\s*([^,;]+))?/g;
                                    let r;
                                    for (; r = n.exec(e);) t[r[1]] = r[2];
                                    return t
                                }(e);
                                if (J.isFunction(t)) return t.call(this, e, n);
                                if (J.isRegExp(t)) return t.exec(e);
                                throw new TypeError("parser must be boolean|regexp|function")
                            }
                        }
                    }
                    has(e, t) {
                        if (e = _e(e)) {
                            const n = J.findKey(this, e);
                            return !(!n || void 0 === this[n] || t && !Oe(0, this[n], n, t))
                        }
                        return !1
                    }
                    delete(e, t) {
                        const n = this;
                        let r = !1;

                        function o(e) {
                            if (e = _e(e)) {
                                const o = J.findKey(n, e);
                                !o || t && !Oe(0, n[o], o, t) || (delete n[o], r = !0)
                            }
                        }
                        return J.isArray(e) ? e.forEach(o) : o(e), r
                    }
                    clear(e) {
                        const t = Object.keys(this);
                        let n = t.length,
                            r = !1;
                        for (; n--;) {
                            const o = t[n];
                            e && !Oe(0, this[o], o, e, !0) || (delete this[o], r = !0)
                        }
                        return r
                    }
                    normalize(e) {
                        const t = this,
                            n = {};
                        return J.forEach(this, ((r, o) => {
                            const s = J.findKey(n, o);
                            if (s) return t[s] = Ae(r), void delete t[o];
                            const a = e ? function(e) {
                                return e.trim().toLowerCase().replace(/([a-z\d])(\w*)/g, ((e, t, n) => t.toUpperCase() + n))
                            }(o) : String(o).trim();
                            a !== o && delete t[o], t[a] = Ae(r), n[a] = !0
                        })), this
                    }
                    concat(...e) {
                        return this.constructor.concat(this, ...e)
                    }
                    toJSON(e) {
                        const t = Object.create(null);
                        return J.forEach(this, ((n, r) => {
                            null != n && !1 !== n && (t[r] = e && J.isArray(n) ? n.join(", ") : n)
                        })), t
                    }[Symbol.iterator]() {
                        return Object.entries(this.toJSON())[Symbol.iterator]()
                    }
                    toString() {
                        return Object.entries(this.toJSON()).map((([e, t]) => e + ": " + t)).join("\n")
                    }
                    get[Symbol.toStringTag]() {
                        return "AxiosHeaders"
                    }
                    static from(e) {
                        return e instanceof this ? e : new this(e)
                    }
                    static concat(e, ...t) {
                        const n = new this(e);
                        return t.forEach((e => n.set(e))), n
                    }
                    static accessor(e) {
                        const t = (this[Se] = this[Se] = {
                                accessors: {}
                            }).accessors,
                            n = this.prototype;

                        function r(e) {
                            const r = _e(e);
                            t[r] || (function(e, t) {
                                const n = J.toCamelCase(" " + t);
                                ["get", "set", "has"].forEach((r => {
                                    Object.defineProperty(e, r + n, {
                                        value: function(e, n, o) {
                                            return this[r].call(this, t, e, n, o)
                                        },
                                        configurable: !0
                                    })
                                }))
                            }(n, e), t[r] = !0)
                        }
                        return J.isArray(e) ? e.forEach(r) : r(e), this
                    }
                }
                Re.accessor(["Content-Type", "Content-Length", "Accept", "Accept-Encoding", "User-Agent", "Authorization"]), J.reduceDescriptors(Re.prototype, (({
                    value: e
                }, t) => {
                    let n = t[0].toUpperCase() + t.slice(1);
                    return {
                        get: () => e,
                        set(e) {
                            this[n] = e
                        }
                    }
                })), J.freezeMethods(Re);
                const Te = Re;

                function xe(e, t) {
                    const n = this || Ce,
                        r = t || n,
                        o = Te.from(r.headers);
                    let s = r.data;
                    return J.forEach(e, (function(e) {
                        s = e.call(n, s, o.normalize(), t ? t.status : void 0)
                    })), o.normalize(), s
                }

                function Ne(e) {
                    return !(!e || !e.__CANCEL__)
                }

                function Le(e, t, n) {
                    Q.call(this, null == e ? "canceled" : e, Q.ERR_CANCELED, t, n), this.name = "CanceledError"
                }
                J.inherits(Le, Q, {
                    __CANCEL__: !0
                });
                const ke = Le;

                function De(e, t, n) {
                    const r = n.config.validateStatus;
                    n.status && r && !r(n.status) ? t(new Q("Request failed with status code " + n.status, [Q.ERR_BAD_REQUEST, Q.ERR_BAD_RESPONSE][Math.floor(n.status / 100) - 4], n.config, n.request, n)) : e(n)
                }
                const Pe = (e, t, n = 3) => {
                        let r = 0;
                        const o = function(e, t) {
                            e = e || 10;
                            const n = new Array(e),
                                r = new Array(e);
                            let o, s = 0,
                                a = 0;
                            return t = void 0 !== t ? t : 1e3,
                                function(i) {
                                    const c = Date.now(),
                                        l = r[a];
                                    o || (o = c), n[s] = i, r[s] = c;
                                    let u = a,
                                        d = 0;
                                    for (; u !== s;) d += n[u++], u %= e;
                                    if (s = (s + 1) % e, s === a && (a = (a + 1) % e), c - o < t) return;
                                    const f = l && c - l;
                                    return f ? Math.round(1e3 * d / f) : void 0
                                }
                        }(50, 250);
                        return function(e, t) {
                            let n, r, o = 0,
                                s = 1e3 / t;
                            const a = (t, s = Date.now()) => {
                                o = s, n = null, r && (clearTimeout(r), r = null), e.apply(null, t)
                            };
                            return [(...e) => {
                                const t = Date.now(),
                                    i = t - o;
                                i >= s ? a(e, t) : (n = e, r || (r = setTimeout((() => {
                                    r = null, a(n)
                                }), s - i)))
                            }, () => n && a(n)]
                        }((n => {
                            const s = n.loaded,
                                a = n.lengthComputable ? n.total : void 0,
                                i = s - r,
                                c = o(i);
                            r = s, e({
                                loaded: s,
                                total: a,
                                progress: a ? s / a : void 0,
                                bytes: i,
                                rate: c || void 0,
                                estimated: c && a && s <= a ? (a - s) / c : void 0,
                                event: n,
                                lengthComputable: null != a,
                                [t ? "download" : "upload"]: !0
                            })
                        }), n)
                    },
                    je = (e, t) => {
                        const n = null != e;
                        return [r => t[0]({
                            lengthComputable: n,
                            total: e,
                            loaded: r
                        }), t[1]]
                    },
                    Be = e => (...t) => J.asap((() => e(...t))),
                    Ue = Ee.hasStandardBrowserEnv ? ((e, t) => n => (n = new URL(n, Ee.origin), e.protocol === n.protocol && e.host === n.host && (t || e.port === n.port)))(new URL(Ee.origin), Ee.navigator && /(msie|trident)/i.test(Ee.navigator.userAgent)) : () => !0,
                    Fe = Ee.hasStandardBrowserEnv ? {
                        write(e, t, n, r, o, s) {
                            const a = [e + "=" + encodeURIComponent(t)];
                            J.isNumber(n) && a.push("expires=" + new Date(n).toGMTString()), J.isString(r) && a.push("path=" + r), J.isString(o) && a.push("domain=" + o), !0 === s && a.push("secure"), document.cookie = a.join("; ")
                        },
                        read(e) {
                            const t = document.cookie.match(new RegExp("(^|;\\s*)(" + e + ")=([^;]*)"));
                            return t ? decodeURIComponent(t[3]) : null
                        },
                        remove(e) {
                            this.write(e, "", Date.now() - 864e5)
                        }
                    } : {
                        write() {},
                        read: () => null,
                        remove() {}
                    };

                function Me(e, t) {
                    return e && !/^([a-z][a-z\d+\-.]*:)?\/\//i.test(t) ? function(e, t) {
                        return t ? e.replace(/\/?\/$/, "") + "/" + t.replace(/^\/+/, "") : e
                    }(e, t) : t
                }
                const qe = e => e instanceof Te ? { ...e
                } : e;

                function He(e, t) {
                    t = t || {};
                    const n = {};

                    function r(e, t, n, r) {
                        return J.isPlainObject(e) && J.isPlainObject(t) ? J.merge.call({
                            caseless: r
                        }, e, t) : J.isPlainObject(t) ? J.merge({}, t) : J.isArray(t) ? t.slice() : t
                    }

                    function o(e, t, n, o) {
                        return J.isUndefined(t) ? J.isUndefined(e) ? void 0 : r(void 0, e, 0, o) : r(e, t, 0, o)
                    }

                    function s(e, t) {
                        if (!J.isUndefined(t)) return r(void 0, t)
                    }

                    function a(e, t) {
                        return J.isUndefined(t) ? J.isUndefined(e) ? void 0 : r(void 0, e) : r(void 0, t)
                    }

                    function i(n, o, s) {
                        return s in t ? r(n, o) : s in e ? r(void 0, n) : void 0
                    }
                    const c = {
                        url: s,
                        method: s,
                        data: s,
                        baseURL: a,
                        transformRequest: a,
                        transformResponse: a,
                        paramsSerializer: a,
                        timeout: a,
                        timeoutMessage: a,
                        withCredentials: a,
                        withXSRFToken: a,
                        adapter: a,
                        responseType: a,
                        xsrfCookieName: a,
                        xsrfHeaderName: a,
                        onUploadProgress: a,
                        onDownloadProgress: a,
                        decompress: a,
                        maxContentLength: a,
                        maxBodyLength: a,
                        beforeRedirect: a,
                        transport: a,
                        httpAgent: a,
                        httpsAgent: a,
                        cancelToken: a,
                        socketPath: a,
                        responseEncoding: a,
                        validateStatus: i,
                        headers: (e, t, n) => o(qe(e), qe(t), 0, !0)
                    };
                    return J.forEach(Object.keys(Object.assign({}, e, t)), (function(r) {
                        const s = c[r] || o,
                            a = s(e[r], t[r], r);
                        J.isUndefined(a) && s !== i || (n[r] = a)
                    })), n
                }
                const Ie = e => {
                        const t = He({}, e);
                        let n, {
                            data: r,
                            withXSRFToken: o,
                            xsrfHeaderName: s,
                            xsrfCookieName: a,
                            headers: i,
                            auth: c
                        } = t;
                        if (t.headers = i = Te.from(i), t.url = le(Me(t.baseURL, t.url), e.params, e.paramsSerializer), c && i.set("Authorization", "Basic " + btoa((c.username || "") + ":" + (c.password ? unescape(encodeURIComponent(c.password)) : ""))), J.isFormData(r))
                            if (Ee.hasStandardBrowserEnv || Ee.hasStandardBrowserWebWorkerEnv) i.setContentType(void 0);
                            else if (!1 !== (n = i.getContentType())) {
                            const [e, ...t] = n ? n.split(";").map((e => e.trim())).filter(Boolean) : [];
                            i.setContentType([e || "multipart/form-data", ...t].join("; "))
                        }
                        if (Ee.hasStandardBrowserEnv && (o && J.isFunction(o) && (o = o(t)), o || !1 !== o && Ue(t.url))) {
                            const e = s && a && Fe.read(a);
                            e && i.set(s, e)
                        }
                        return t
                    },
                    Ze = "undefined" != typeof XMLHttpRequest && function(e) {
                        return new Promise((function(t, n) {
                            const r = Ie(e);
                            let o = r.data;
                            const s = Te.from(r.headers).normalize();
                            let a, i, c, l, u, {
                                responseType: d,
                                onUploadProgress: f,
                                onDownloadProgress: h
                            } = r;

                            function p() {
                                l && l(), u && u(), r.cancelToken && r.cancelToken.unsubscribe(a), r.signal && r.signal.removeEventListener("abort", a)
                            }
                            let m = new XMLHttpRequest;

                            function g() {
                                if (!m) return;
                                const r = Te.from("getAllResponseHeaders" in m && m.getAllResponseHeaders());
                                De((function(e) {
                                    t(e), p()
                                }), (function(e) {
                                    n(e), p()
                                }), {
                                    data: d && "text" !== d && "json" !== d ? m.response : m.responseText,
                                    status: m.status,
                                    statusText: m.statusText,
                                    headers: r,
                                    config: e,
                                    request: m
                                }), m = null
                            }
                            m.open(r.method.toUpperCase(), r.url, !0), m.timeout = r.timeout, "onloadend" in m ? m.onloadend = g : m.onreadystatechange = function() {
                                m && 4 === m.readyState && (0 !== m.status || m.responseURL && 0 === m.responseURL.indexOf("file:")) && setTimeout(g)
                            }, m.onabort = function() {
                                m && (n(new Q("Request aborted", Q.ECONNABORTED, e, m)), m = null)
                            }, m.onerror = function() {
                                n(new Q("Network Error", Q.ERR_NETWORK, e, m)), m = null
                            }, m.ontimeout = function() {
                                let t = r.timeout ? "timeout of " + r.timeout + "ms exceeded" : "timeout exceeded";
                                const o = r.transitional || de;
                                r.timeoutErrorMessage && (t = r.timeoutErrorMessage), n(new Q(t, o.clarifyTimeoutError ? Q.ETIMEDOUT : Q.ECONNABORTED, e, m)), m = null
                            }, void 0 === o && s.setContentType(null), "setRequestHeader" in m && J.forEach(s.toJSON(), (function(e, t) {
                                m.setRequestHeader(t, e)
                            })), J.isUndefined(r.withCredentials) || (m.withCredentials = !!r.withCredentials), d && "json" !== d && (m.responseType = r.responseType), h && ([c, u] = Pe(h, !0), m.addEventListener("progress", c)), f && m.upload && ([i, l] = Pe(f), m.upload.addEventListener("progress", i), m.upload.addEventListener("loadend", l)), (r.cancelToken || r.signal) && (a = t => {
                                m && (n(!t || t.type ? new ke(null, e, m) : t), m.abort(), m = null)
                            }, r.cancelToken && r.cancelToken.subscribe(a), r.signal && (r.signal.aborted ? a() : r.signal.addEventListener("abort", a)));
                            const b = function(e) {
                                const t = /^([-+\w]{1,25})(:?\/\/|:)/.exec(e);
                                return t && t[1] || ""
                            }(r.url);
                            b && -1 === Ee.protocols.indexOf(b) ? n(new Q("Unsupported protocol " + b + ":", Q.ERR_BAD_REQUEST, e)) : m.send(o || null)
                        }))
                    },
                    ze = (e, t) => {
                        const {
                            length: n
                        } = e = e ? e.filter(Boolean) : [];
                        if (t || n) {
                            let n, r = new AbortController;
                            const o = function(e) {
                                if (!n) {
                                    n = !0, a();
                                    const t = e instanceof Error ? e : this.reason;
                                    r.abort(t instanceof Q ? t : new ke(t instanceof Error ? t.message : t))
                                }
                            };
                            let s = t && setTimeout((() => {
                                s = null, o(new Q(`timeout ${t} of ms exceeded`, Q.ETIMEDOUT))
                            }), t);
                            const a = () => {
                                e && (s && clearTimeout(s), s = null, e.forEach((e => {
                                    e.unsubscribe ? e.unsubscribe(o) : e.removeEventListener("abort", o)
                                })), e = null)
                            };
                            e.forEach((e => e.addEventListener("abort", o)));
                            const {
                                signal: i
                            } = r;
                            return i.unsubscribe = () => J.asap(a), i
                        }
                    },
                    Ve = function*(e, t) {
                        let n = e.byteLength;
                        if (!t || n < t) return void(yield e);
                        let r, o = 0;
                        for (; o < n;) r = o + t, yield e.slice(o, r), o = r
                    },
                    $e = (e, t, n, r) => {
                        const o = async function*(e, t) {
                            for await (const n of async function*(e) {
                                if (e[Symbol.asyncIterator]) return void(yield* e);
                                const t = e.getReader();
                                try {
                                    for (;;) {
                                        const {
                                            done: e,
                                            value: n
                                        } = await t.read();
                                        if (e) break;
                                        yield n
                                    }
                                } finally {
                                    await t.cancel()
                                }
                            }(e)) yield* Ve(n, t)
                        }(e, t);
                        let s, a = 0,
                            i = e => {
                                s || (s = !0, r && r(e))
                            };
                        return new ReadableStream({
                            async pull(e) {
                                try {
                                    const {
                                        done: t,
                                        value: r
                                    } = await o.next();
                                    if (t) return i(), void e.close();
                                    let s = r.byteLength;
                                    if (n) {
                                        let e = a += s;
                                        n(e)
                                    }
                                    e.enqueue(new Uint8Array(r))
                                } catch (e) {
                                    throw i(e), e
                                }
                            },
                            cancel: e => (i(e), o.return())
                        }, {
                            highWaterMark: 2
                        })
                    },
                    We = "function" == typeof fetch && "function" == typeof Request && "function" == typeof Response,
                    Je = We && "function" == typeof ReadableStream,
                    Ke = We && ("function" == typeof TextEncoder ? (Ge = new TextEncoder, e => Ge.encode(e)) : async e => new Uint8Array(await new Response(e).arrayBuffer()));
                var Ge;
                const Xe = (e, ...t) => {
                        try {
                            return !!e(...t)
                        } catch (e) {
                            return !1
                        }
                    },
                    Qe = Je && Xe((() => {
                        let e = !1;
                        const t = new Request(Ee.origin, {
                            body: new ReadableStream,
                            method: "POST",
                            get duplex() {
                                return e = !0, "half"
                            }
                        }).headers.has("Content-Type");
                        return e && !t
                    })),
                    Ye = Je && Xe((() => J.isReadableStream(new Response("").body))),
                    et = {
                        stream: Ye && (e => e.body)
                    };
                var tt;
                We && (tt = new Response, ["text", "arrayBuffer", "blob", "formData", "stream"].forEach((e => {
                    !et[e] && (et[e] = J.isFunction(tt[e]) ? t => t[e]() : (t, n) => {
                        throw new Q(`Response type '${e}' is not supported`, Q.ERR_NOT_SUPPORT, n)
                    })
                })));
                const nt = {
                    http: null,
                    xhr: Ze,
                    fetch: We && (async e => {
                        let {
                            url: t,
                            method: n,
                            data: r,
                            signal: o,
                            cancelToken: s,
                            timeout: a,
                            onDownloadProgress: i,
                            onUploadProgress: c,
                            responseType: l,
                            headers: u,
                            withCredentials: d = "same-origin",
                            fetchOptions: f
                        } = Ie(e);
                        l = l ? (l + "").toLowerCase() : "text";
                        let h, p = ze([o, s && s.toAbortSignal()], a);
                        const m = p && p.unsubscribe && (() => {
                            p.unsubscribe()
                        });
                        let g;
                        try {
                            if (c && Qe && "get" !== n && "head" !== n && 0 !== (g = await (async (e, t) => {
                                    const n = J.toFiniteNumber(e.getContentLength());
                                    return null == n ? (async e => {
                                        if (null == e) return 0;
                                        if (J.isBlob(e)) return e.size;
                                        if (J.isSpecCompliantForm(e)) {
                                            const t = new Request(Ee.origin, {
                                                method: "POST",
                                                body: e
                                            });
                                            return (await t.arrayBuffer()).byteLength
                                        }
                                        return J.isArrayBufferView(e) || J.isArrayBuffer(e) ? e.byteLength : (J.isURLSearchParams(e) && (e += ""), J.isString(e) ? (await Ke(e)).byteLength : void 0)
                                    })(t) : n
                                })(u, r))) {
                                let e, n = new Request(t, {
                                    method: "POST",
                                    body: r,
                                    duplex: "half"
                                });
                                if (J.isFormData(r) && (e = n.headers.get("content-type")) && u.setContentType(e), n.body) {
                                    const [e, t] = je(g, Pe(Be(c)));
                                    r = $e(n.body, 65536, e, t)
                                }
                            }
                            J.isString(d) || (d = d ? "include" : "omit");
                            const o = "credentials" in Request.prototype;
                            h = new Request(t, { ...f,
                                signal: p,
                                method: n.toUpperCase(),
                                headers: u.normalize().toJSON(),
                                body: r,
                                duplex: "half",
                                credentials: o ? d : void 0
                            });
                            let s = await fetch(h);
                            const a = Ye && ("stream" === l || "response" === l);
                            if (Ye && (i || a && m)) {
                                const e = {};
                                ["status", "statusText", "headers"].forEach((t => {
                                    e[t] = s[t]
                                }));
                                const t = J.toFiniteNumber(s.headers.get("content-length")),
                                    [n, r] = i && je(t, Pe(Be(i), !0)) || [];
                                s = new Response($e(s.body, 65536, n, (() => {
                                    r && r(), m && m()
                                })), e)
                            }
                            l = l || "text";
                            let b = await et[J.findKey(et, l) || "text"](s, e);
                            return !a && m && m(), await new Promise(((t, n) => {
                                De(t, n, {
                                    data: b,
                                    headers: Te.from(s.headers),
                                    status: s.status,
                                    statusText: s.statusText,
                                    config: e,
                                    request: h
                                })
                            }))
                        } catch (t) {
                            if (m && m(), t && "TypeError" === t.name && /fetch/i.test(t.message)) throw Object.assign(new Q("Network Error", Q.ERR_NETWORK, e, h), {
                                cause: t.cause || t
                            });
                            throw Q.from(t, t && t.code, e, h)
                        }
                    })
                };
                J.forEach(nt, ((e, t) => {
                    if (e) {
                        try {
                            Object.defineProperty(e, "name", {
                                value: t
                            })
                        } catch (e) {}
                        Object.defineProperty(e, "adapterName", {
                            value: t
                        })
                    }
                }));
                const rt = e => `- ${e}`,
                    ot = e => J.isFunction(e) || null === e || !1 === e,
                    st = e => {
                        e = J.isArray(e) ? e : [e];
                        const {
                            length: t
                        } = e;
                        let n, r;
                        const o = {};
                        for (let s = 0; s < t; s++) {
                            let t;
                            if (n = e[s], r = n, !ot(n) && (r = nt[(t = String(n)).toLowerCase()], void 0 === r)) throw new Q(`Unknown adapter '${t}'`);
                            if (r) break;
                            o[t || "#" + s] = r
                        }
                        if (!r) {
                            const e = Object.entries(o).map((([e, t]) => `adapter ${e} ` + (!1 === t ? "is not supported by the environment" : "is not available in the build")));
                            let n = t ? e.length > 1 ? "since :\n" + e.map(rt).join("\n") : " " + rt(e[0]) : "as no adapter specified";
                            throw new Q("There is no suitable adapter to dispatch the request " + n, "ERR_NOT_SUPPORT")
                        }
                        return r
                    };

                function at(e) {
                    if (e.cancelToken && e.cancelToken.throwIfRequested(), e.signal && e.signal.aborted) throw new ke(null, e)
                }

                function it(e) {
                    return at(e), e.headers = Te.from(e.headers), e.data = xe.call(e, e.transformRequest), -1 !== ["post", "put", "patch"].indexOf(e.method) && e.headers.setContentType("application/x-www-form-urlencoded", !1), st(e.adapter || Ce.adapter)(e).then((function(t) {
                        return at(e), t.data = xe.call(e, e.transformResponse, t), t.headers = Te.from(t.headers), t
                    }), (function(t) {
                        return Ne(t) || (at(e), t && t.response && (t.response.data = xe.call(e, e.transformResponse, t.response), t.response.headers = Te.from(t.response.headers))), Promise.reject(t)
                    }))
                }
                const ct = {};
                ["object", "boolean", "number", "function", "string", "symbol"].forEach(((e, t) => {
                    ct[e] = function(n) {
                        return typeof n === e || "a" + (t < 1 ? "n " : " ") + e
                    }
                }));
                const lt = {};
                ct.transitional = function(e, t, n) {
                    function r(e, t) {
                        return "[Axios v1.7.9] Transitional option '" + e + "'" + t + (n ? ". " + n : "")
                    }
                    return (n, o, s) => {
                        if (!1 === e) throw new Q(r(o, " has been removed" + (t ? " in " + t : "")), Q.ERR_DEPRECATED);
                        return t && !lt[o] && (lt[o] = !0, console.warn(r(o, " has been deprecated since v" + t + " and will be removed in the near future"))), !e || e(n, o, s)
                    }
                }, ct.spelling = function(e) {
                    return (t, n) => (console.warn(`${n} is likely a misspelling of ${e}`), !0)
                };
                const ut = {
                        assertOptions: function(e, t, n) {
                            if ("object" != typeof e) throw new Q("options must be an object", Q.ERR_BAD_OPTION_VALUE);
                            const r = Object.keys(e);
                            let o = r.length;
                            for (; o-- > 0;) {
                                const s = r[o],
                                    a = t[s];
                                if (a) {
                                    const t = e[s],
                                        n = void 0 === t || a(t, s, e);
                                    if (!0 !== n) throw new Q("option " + s + " must be " + n, Q.ERR_BAD_OPTION_VALUE)
                                } else if (!0 !== n) throw new Q("Unknown option " + s, Q.ERR_BAD_OPTION)
                            }
                        },
                        validators: ct
                    },
                    dt = ut.validators;
                class ft {
                    constructor(e) {
                        this.defaults = e, this.interceptors = {
                            request: new ue,
                            response: new ue
                        }
                    }
                    async request(e, t) {
                        try {
                            return await this._request(e, t)
                        } catch (e) {
                            if (e instanceof Error) {
                                let t = {};
                                Error.captureStackTrace ? Error.captureStackTrace(t) : t = new Error;
                                const n = t.stack ? t.stack.replace(/^.+\n/, "") : "";
                                try {
                                    e.stack ? n && !String(e.stack).endsWith(n.replace(/^.+\n.+\n/, "")) && (e.stack += "\n" + n) : e.stack = n
                                } catch (e) {}
                            }
                            throw e
                        }
                    }
                    _request(e, t) {
                        "string" == typeof e ? (t = t || {}).url = e : t = e || {}, t = He(this.defaults, t);
                        const {
                            transitional: n,
                            paramsSerializer: r,
                            headers: o
                        } = t;
                        void 0 !== n && ut.assertOptions(n, {
                            silentJSONParsing: dt.transitional(dt.boolean),
                            forcedJSONParsing: dt.transitional(dt.boolean),
                            clarifyTimeoutError: dt.transitional(dt.boolean)
                        }, !1), null != r && (J.isFunction(r) ? t.paramsSerializer = {
                            serialize: r
                        } : ut.assertOptions(r, {
                            encode: dt.function,
                            serialize: dt.function
                        }, !0)), ut.assertOptions(t, {
                            baseUrl: dt.spelling("baseURL"),
                            withXsrfToken: dt.spelling("withXSRFToken")
                        }, !0), t.method = (t.method || this.defaults.method || "get").toLowerCase();
                        let s = o && J.merge(o.common, o[t.method]);
                        o && J.forEach(["delete", "get", "head", "post", "put", "patch", "common"], (e => {
                            delete o[e]
                        })), t.headers = Te.concat(s, o);
                        const a = [];
                        let i = !0;
                        this.interceptors.request.forEach((function(e) {
                            "function" == typeof e.runWhen && !1 === e.runWhen(t) || (i = i && e.synchronous, a.unshift(e.fulfilled, e.rejected))
                        }));
                        const c = [];
                        let l;
                        this.interceptors.response.forEach((function(e) {
                            c.push(e.fulfilled, e.rejected)
                        }));
                        let u, d = 0;
                        if (!i) {
                            const e = [it.bind(this), void 0];
                            for (e.unshift.apply(e, a), e.push.apply(e, c), u = e.length, l = Promise.resolve(t); d < u;) l = l.then(e[d++], e[d++]);
                            return l
                        }
                        u = a.length;
                        let f = t;
                        for (d = 0; d < u;) {
                            const e = a[d++],
                                t = a[d++];
                            try {
                                f = e(f)
                            } catch (e) {
                                t.call(this, e);
                                break
                            }
                        }
                        try {
                            l = it.call(this, f)
                        } catch (e) {
                            return Promise.reject(e)
                        }
                        for (d = 0, u = c.length; d < u;) l = l.then(c[d++], c[d++]);
                        return l
                    }
                    getUri(e) {
                        return le(Me((e = He(this.defaults, e)).baseURL, e.url), e.params, e.paramsSerializer)
                    }
                }
                J.forEach(["delete", "get", "head", "options"], (function(e) {
                    ft.prototype[e] = function(t, n) {
                        return this.request(He(n || {}, {
                            method: e,
                            url: t,
                            data: (n || {}).data
                        }))
                    }
                })), J.forEach(["post", "put", "patch"], (function(e) {
                    function t(t) {
                        return function(n, r, o) {
                            return this.request(He(o || {}, {
                                method: e,
                                headers: t ? {
                                    "Content-Type": "multipart/form-data"
                                } : {},
                                url: n,
                                data: r
                            }))
                        }
                    }
                    ft.prototype[e] = t(), ft.prototype[e + "Form"] = t(!0)
                }));
                const ht = ft;
                class pt {
                    constructor(e) {
                        if ("function" != typeof e) throw new TypeError("executor must be a function.");
                        let t;
                        this.promise = new Promise((function(e) {
                            t = e
                        }));
                        const n = this;
                        this.promise.then((e => {
                            if (!n._listeners) return;
                            let t = n._listeners.length;
                            for (; t-- > 0;) n._listeners[t](e);
                            n._listeners = null
                        })), this.promise.then = e => {
                            let t;
                            const r = new Promise((e => {
                                n.subscribe(e), t = e
                            })).then(e);
                            return r.cancel = function() {
                                n.unsubscribe(t)
                            }, r
                        }, e((function(e, r, o) {
                            n.reason || (n.reason = new ke(e, r, o), t(n.reason))
                        }))
                    }
                    throwIfRequested() {
                        if (this.reason) throw this.reason
                    }
                    subscribe(e) {
                        this.reason ? e(this.reason) : this._listeners ? this._listeners.push(e) : this._listeners = [e]
                    }
                    unsubscribe(e) {
                        if (!this._listeners) return;
                        const t = this._listeners.indexOf(e); - 1 !== t && this._listeners.splice(t, 1)
                    }
                    toAbortSignal() {
                        const e = new AbortController,
                            t = t => {
                                e.abort(t)
                            };
                        return this.subscribe(t), e.signal.unsubscribe = () => this.unsubscribe(t), e.signal
                    }
                    static source() {
                        let e;
                        return {
                            token: new pt((function(t) {
                                e = t
                            })),
                            cancel: e
                        }
                    }
                }
                const mt = pt,
                    gt = {
                        Continue: 100,
                        SwitchingProtocols: 101,
                        Processing: 102,
                        EarlyHints: 103,
                        Ok: 200,
                        Created: 201,
                        Accepted: 202,
                        NonAuthoritativeInformation: 203,
                        NoContent: 204,
                        ResetContent: 205,
                        PartialContent: 206,
                        MultiStatus: 207,
                        AlreadyReported: 208,
                        ImUsed: 226,
                        MultipleChoices: 300,
                        MovedPermanently: 301,
                        Found: 302,
                        SeeOther: 303,
                        NotModified: 304,
                        UseProxy: 305,
                        Unused: 306,
                        TemporaryRedirect: 307,
                        PermanentRedirect: 308,
                        BadRequest: 400,
                        Unauthorized: 401,
                        PaymentRequired: 402,
                        Forbidden: 403,
                        NotFound: 404,
                        MethodNotAllowed: 405,
                        NotAcceptable: 406,
                        ProxyAuthenticationRequired: 407,
                        RequestTimeout: 408,
                        Conflict: 409,
                        Gone: 410,
                        LengthRequired: 411,
                        PreconditionFailed: 412,
                        PayloadTooLarge: 413,
                        UriTooLong: 414,
                        UnsupportedMediaType: 415,
                        RangeNotSatisfiable: 416,
                        ExpectationFailed: 417,
                        ImATeapot: 418,
                        MisdirectedRequest: 421,
                        UnprocessableEntity: 422,
                        Locked: 423,
                        FailedDependency: 424,
                        TooEarly: 425,
                        UpgradeRequired: 426,
                        PreconditionRequired: 428,
                        TooManyRequests: 429,
                        RequestHeaderFieldsTooLarge: 431,
                        UnavailableForLegalReasons: 451,
                        InternalServerError: 500,
                        NotImplemented: 501,
                        BadGateway: 502,
                        ServiceUnavailable: 503,
                        GatewayTimeout: 504,
                        HttpVersionNotSupported: 505,
                        VariantAlsoNegotiates: 506,
                        InsufficientStorage: 507,
                        LoopDetected: 508,
                        NotExtended: 510,
                        NetworkAuthenticationRequired: 511
                    };
                Object.entries(gt).forEach((([e, t]) => {
                    gt[t] = e
                }));
                const bt = gt,
                    Et = function e(t) {
                        const n = new ht(t),
                            r = o(ht.prototype.request, n);
                        return J.extend(r, ht.prototype, n, {
                            allOwnKeys: !0
                        }), J.extend(r, n, null, {
                            allOwnKeys: !0
                        }), r.create = function(n) {
                            return e(He(t, n))
                        }, r
                    }(Ce);
                Et.Axios = ht, Et.CanceledError = ke, Et.CancelToken = mt, Et.isCancel = Ne, Et.VERSION = "1.7.9", Et.toFormData = re, Et.AxiosError = Q, Et.Cancel = Et.CanceledError, Et.all = function(e) {
                    return Promise.all(e)
                }, Et.spread = function(e) {
                    return function(t) {
                        return e.apply(null, t)
                    }
                }, Et.isAxiosError = function(e) {
                    return J.isObject(e) && !0 === e.isAxiosError
                }, Et.mergeConfig = He, Et.AxiosHeaders = Te, Et.formToJSON = e => ye(J.isHTMLForm(e) ? new FormData(e) : e), Et.getAdapter = st, Et.HttpStatusCode = bt, Et.default = Et;
                const yt = Et
            }
        },
        t = {};

    function n(r) {
        var o = t[r];
        if (void 0 !== o) return o.exports;
        var s = t[r] = {
            exports: {}
        };
        return e[r](s, s.exports, n), s.exports
    }
    n.n = e => {
        var t = e && e.__esModule ? () => e.default : () => e;
        return n.d(t, {
            a: t
        }), t
    }, n.d = (e, t) => {
        for (var r in t) n.o(t, r) && !n.o(e, r) && Object.defineProperty(e, r, {
            enumerable: !0,
            get: t[r]
        })
    }, n.o = (e, t) => Object.prototype.hasOwnProperty.call(e, t), n.r = e => {
        "undefined" != typeof Symbol && Symbol.toStringTag && Object.defineProperty(e, Symbol.toStringTag, {
            value: "Module"
        }), Object.defineProperty(e, "__esModule", {
            value: !0
        })
    };
    var r = n(1609);
    const o = window.ReactDOM;
    var s = n.n(o),
        a = n(4593);
    document.addEventListener("DOMContentLoaded", (() => {
        const e = document.querySelectorAll("[id=betterdocs-search-modal]");
        if (e)
            for (let t of e) {
                const e = t ? .className,
                    n = {
                        placeholder: t.getAttribute("data-placeholder"),
                        heading: t.getAttribute("data-heading"),
                        subheading: t.getAttribute("data-subheading"),
                        headingTag: t.getAttribute("data-headingtag"),
                        subheadingTag: t.getAttribute("data-subheadingtag"),
                        buttonText: t.getAttribute("data-buttontext"),
                        numberofdocs: t.getAttribute("data-numberofdocs"),
                        numberoffaqs: t.getAttribute("data-numberoffaqs"),
                        docterms: t.getAttribute("data-docterms"),
                        faqterms: t.getAttribute("data-faqterms"),
                        searchButton: t.getAttribute("data-searchbutton"),
                        doc_ids: t.getAttribute("data-doc_ids"),
                        doc_categories_ids: t.getAttribute("data-doc_categories_ids"),
                        faq_categories_ids: t.getAttribute("data-faq_categories_ids")
                    };
                s().render((0, r.createElement)(a.A, {
                    layout: e,
                    ...n
                }), t)
            }
    }))
})();