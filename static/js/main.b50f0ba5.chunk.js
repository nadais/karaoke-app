(this["webpackJsonpkaraoke-catalog-app-pwa"]=this["webpackJsonpkaraoke-catalog-app-pwa"]||[]).push([[0],{50:function(e,t,n){"use strict";n.r(t);var a=n(0),c=n.n(a),i=n(7),r=n.n(i),s=n(5),l=n(8),o=(n(47),n(48),n(1));var j=function(){var e=Object(a.useState)([]),t=Object(s.a)(e,2),n=t[0],c=t[1],i=Object(a.useState)(null),r=Object(s.a)(i,2),j=r[0],d=r[1],u=Object(a.useState)(!1),b=Object(s.a)(u,2),h=b[0],O=b[1],p=Object(a.useState)({}),f=Object(s.a)(p,2),g=f[0],x=f[1],k=Object(a.useState)(),m=Object(s.a)(k,2),v=m[0],S=m[1],y=Object(a.useState)(null),C=Object(s.a)(y,2),w=(C[0],C[1]);function G(){O(!0),fetch("https://karaoke-juliane.herokuapp.com/songs").then((function(e){return e.json()})).then((function(e){O(!1),x(e.songGroups),A(Object.keys(g)[1])}))}function A(e){S(e),c(g[e])}return Object(a.useEffect)((function(){G()}),[]),Object(o.jsxs)("div",{children:[Object(o.jsx)("h1",{align:"center",children:"Karaoke night"}),Object(o.jsx)("label",{for:"catalog",children:"Select catalog"}),Object(o.jsx)("select",{name:"catalog",onChange:function(e){A(e.target.value)},value:v,children:Object.keys(g).map((function(e){return Object(o.jsx)("option",{value:e,children:e})}))}),Object(o.jsx)("button",{onClick:G,children:"Reload Songs"}),function(){if(h)return Object(o.jsx)("div",{class:"spinner-border",role:"status",children:Object(o.jsx)("span",{class:"visually-hidden",children:"Loading..."})})}(),Object(o.jsx)("div",{style:{backgroundColor:"#dedede",padding:10},children:Object(o.jsx)("input",{type:"search",style:{width:"100%",padding:"10px 20px",borderRadius:20,outline:0,border:"2px #68bf40 solid",fontSize:"100%"},onChange:function(e){j.setQuickFilter(e.target.value)},placeholder:"search songs..."})}),Object(o.jsx)("div",{className:"ag-theme-alpine",style:{height:600},children:Object(o.jsxs)(l.AgGridReact,{rowData:n,defaultColDef:{flex:1,sortable:!0,resizable:!0,filter:!0},onGridReady:function(e){d(e.api),w(e.columnApi)},children:[Object(o.jsx)(l.AgGridColumn,{field:"number",sortable:!0,filter:!0}),Object(o.jsx)(l.AgGridColumn,{field:"name",sortable:!0,filter:!0}),Object(o.jsx)(l.AgGridColumn,{field:"artist",sortable:!0,filter:!0})]})})]})};r.a.render(Object(o.jsx)(c.a.StrictMode,{children:Object(o.jsx)(j,{})}),document.getElementById("root"))}},[[50,1,2]]]);
//# sourceMappingURL=main.b50f0ba5.chunk.js.map