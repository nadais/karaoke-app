(this["webpackJsonpkaraoke-catalog-app-pwa"]=this["webpackJsonpkaraoke-catalog-app-pwa"]||[]).push([[0],{50:function(e,t,n){"use strict";n.r(t);var o=n(0),a=n.n(o),r=n(6),i=n.n(r),c=n(17),s=n(16),l=(n(1),n(47),n(48),n(4));var u=function(){var e="karaoke-storage",t=Object(o.useState)(u()),n=Object(c.a)(t,2),a=n[0],r=n[1],i={defaultColDef:{resizable:!0},columnDefs:[{field:"name",sortable:!0,filter:!0},{field:"artist",sortable:!0,filter:!0}],rowData:a,onColumnResized:function(e){console.log(e)}};function u(){var t=window.localStorage.getItem(e);return null==t?[]:JSON.parse(t)}return Object(o.useEffect)((function(){var t=u();0!=t.Length?r(t):fetch("https://localhost:7232/songs").then((function(e){return e.json()})).then((function(t){window.localStorage.setItem(e,JSON.stringify(t)),r(t)}))}),[]),Object(l.jsx)("div",{className:"container ag-theme-balham",style:{height:1e3,width:500},children:Object(l.jsx)(s.AgGridReact,{gridOptions:i})})};Boolean("localhost"===window.location.hostname||"[::1]"===window.location.hostname||window.location.hostname.match(/^127(?:\.(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)){3}$/));var h=function(e){e&&e instanceof Function&&n.e(3).then(n.bind(null,51)).then((function(t){var n=t.getCLS,o=t.getFID,a=t.getFCP,r=t.getLCP,i=t.getTTFB;n(e),o(e),a(e),r(e),i(e)}))};i.a.render(Object(l.jsx)(a.a.StrictMode,{children:Object(l.jsx)(u,{})}),document.getElementById("root")),"serviceWorker"in navigator&&navigator.serviceWorker.ready.then((function(e){e.unregister()})).catch((function(e){console.error(e.message)})),h()}},[[50,1,2]]]);
//# sourceMappingURL=main.85345770.chunk.js.map