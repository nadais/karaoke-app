(this["webpackJsonpkaraoke-catalog-app-pwa"]=this["webpackJsonpkaraoke-catalog-app-pwa"]||[]).push([[0],{72:function(e,t,a){"use strict";a.r(t);var n=a(0),c=a.n(n),r=a(8),l=a.n(r),s=a(18),i=a.n(s),o=a(26),u=a(2),d=a(27),j=a(73),b=a(20),h=a(1);var g=function(e){function t(){var t=!e.node.data.selected,a=e.column.colId,n=JSON.parse(localStorage.getItem("mylist"));null==n&&(n=[]),t?n.push(e.node.data.key):n=n.filter((function(t){return t!==e.node.data.key})),localStorage.setItem("mylist",JSON.stringify(n)),e.node.setDataValue(a,t),e.callbackSelectedList(n)}return!1===e.value?Object(h.jsx)("button",{type:"button",className:"btn btn-success",onClick:t,children:Object(h.jsx)("svg",{xmlns:"http://www.w3.org/2000/svg",width:"16",height:"16",fill:"currentColor",class:"bi bi-plus",viewBox:"0 0 16 16",children:Object(h.jsx)("path",{d:"M8 4a.5.5 0 0 1 .5.5v3h3a.5.5 0 0 1 0 1h-3v3a.5.5 0 0 1-1 0v-3h-3a.5.5 0 0 1 0-1h3v-3A.5.5 0 0 1 8 4z"})})}):Object(h.jsx)("button",{type:"button",class:"btn btn-danger",onClick:t,children:Object(h.jsxs)("svg",{xmlns:"http://www.w3.org/2000/svg",width:"16",height:"16",fill:"currentColor",class:"bi bi-trash",viewBox:"0 0 16 16",children:[Object(h.jsx)("path",{d:"M5.5 5.5A.5.5 0 0 1 6 6v6a.5.5 0 0 1-1 0V6a.5.5 0 0 1 .5-.5zm2.5 0a.5.5 0 0 1 .5.5v6a.5.5 0 0 1-1 0V6a.5.5 0 0 1 .5-.5zm3 .5a.5.5 0 0 0-1 0v6a.5.5 0 0 0 1 0V6z"}),Object(h.jsx)("path",{"fill-rule":"evenodd",d:"M14.5 3a1 1 0 0 1-1 1H13v9a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2V4h-.5a1 1 0 0 1-1-1V2a1 1 0 0 1 1-1H6a1 1 0 0 1 1-1h2a1 1 0 0 1 1 1h3.5a1 1 0 0 1 1 1v1zM4.118 4 4 4.059V13a1 1 0 0 0 1 1h6a1 1 0 0 0 1-1V4.059L11.882 4H4.118zM2.5 3V2h11v1h-11z"})]})})};a(65),a(66);function f(){var e=Object(n.useState)([]),t=Object(u.a)(e,2),a=t[0],c=t[1],r=Object(n.useState)([]),l=Object(u.a)(r,2),s=l[0],f=l[1],v=Object(n.useState)(!1),O=Object(u.a)(v,2),p=O[0],m=O[1],x=Object(b.b)(),w=Object(u.a)(x,2),k=w[0],y=w[1],S=Object(j.a)(),N=S.t,C=S.i18n,z=Object(n.useState)([{headerName:we("mylist"),field:"selected",cellRenderer:"checkboxRenderer",cellRendererParams:{callbackSelectedList:c},flex:1},{field:"number",headerName:we("number"),sortable:!0,filter:!0,flex:1,minWidth:80},{field:"name",headerName:we("name"),sortable:!0,filter:!0,flex:4,minWidth:250},{field:"artist",headerName:we("artist"),sortable:!0,filter:!0,flex:2,sort:"asc",minWidth:170}]),L=Object(u.a)(z,1)[0],M=Object(n.useState)([]),V=Object(u.a)(M,2),R=V[0],A=V[1],I=Object(n.useState)(null),H=Object(u.a)(I,2),B=H[0],G=H[1],J=Object(n.useState)(!1),D=Object(u.a)(J,2),P=D[0],E=D[1],F=Object(n.useState)([]),W=Object(u.a)(F,2),K=W[0],Q=W[1],T=Object(n.useState)([]),U=Object(u.a)(T,2),q=U[0],X=U[1],Y=Object(n.useState)([]),Z=Object(u.a)(Y,2),$=Z[0],_=Z[1],ee=Object(n.useState)(void 0),te=Object(u.a)(ee,2),ae=te[0],ne=te[1],ce=Object(n.useState)(void 0),re=Object(u.a)(ce,2),le=re[0],se=re[1],ie=Object(n.useState)(void 0),oe=Object(u.a)(ie,2),ue=oe[0],de=oe[1],je={padding:10},be={width:"100%",padding:"10px 20px",borderRadius:20,outline:0,border:"2px #68bf40 solid",fontSize:"100%"},he=function(e){B.setQuickFilter(e.target.value)};function ge(){var e=!p;m(!p);var t=ve();t.myList=e,A(Oe(t))}function fe(){return fe=Object(o.a)(i.a.mark((function e(t){var a,n,r,l,s,o,u,d,j;return i.a.wrap((function(e){for(;;)switch(e.prev=e.next){case 0:return E(!0),e.next=3,fetch("https://karaoke-juliane.herokuapp.com/songs/genres?language=".concat(null!==t&&void 0!==t?t:""));case 3:return a=e.sent,e.next=6,fetch("https://karaoke-juliane.herokuapp.com/songs");case 6:return n=e.sent,e.next=9,n.json();case 9:return r=e.sent,e.next=12,a.json();case 12:l=e.sent,_(l),E(!1),null==(s=JSON.parse(localStorage.getItem("mylist")))&&(s=[]),c(s),o=(o=r.songGroups).map((function(e){return e.selected=s.includes(e.key),e})),u=o.flatMap((function(e){return e.catalogs})).filter((function(e,t,a){return a.indexOf(e)===t&&null!=e})),d=o.flatMap((function(e){return e.categories})).filter((function(e,t,a){return a.indexOf(e)===t&&null!=e})),j=o.flatMap((function(e){return e.genres})).filter((function(e,t,a){return a.indexOf(e)===t&&null!=e})).map((function(e){return l.find((function(t){return t.id===e}))})).filter((function(e){return null!=e})),_(j),Q(u),X(d),f(r.songGroups),A(o);case 28:case"end":return e.stop()}}),e)}))),fe.apply(this,arguments)}Object(n.useEffect)((function(){var e=k.get("lang");null!=e&&C.changeLanguage(e),function(e){fe.apply(this,arguments)}(e)}),[C,k]);var ve=function(){return{myList:p,catalog:ue,category:le,genre:ae}};function Oe(e){if(e.myList)return s.filter((function(e){return a.includes(e.key)}));var t=null==e.catalog||""===e.catalog?s:s.filter((function(t){return t.catalogs.includes(e.catalog)}));return null!=e.category&&""!==e.category&&(t=t.filter((function(t){return t.categories.includes(e.category)}))),null!=e.genre&&""!==e.genre&&(t=t.filter((function(t){return t.genres.includes(e.genre)}))),t}function pe(e){var t=""===e.target.value?void 0:e.target.value;se(t);var a=ve();a.category=t,A(Oe(a))}function me(e){var t=""===e.target.value?void 0:parseInt(e.target.value);ne(t);var a=ve();a.genre=t,A(Oe(a))}function xe(e){var t=e.target.value,a=new URLSearchParams(k.toString());a.set("lang",t),y(a.toString()),C.changeLanguage(t)}function we(e){return N(e)}return Object(h.jsxs)("div",{children:[Object(h.jsx)("div",{class:"row",children:Object(h.jsx)("div",{class:"col-6 offset-md-4 offset-lg-4",children:Object(h.jsx)("img",{class:"header-image",src:"header.png",alt:"Karaoke night"})})}),function(){var e={en:"English",pt:"Portugu\xeas",fr:"Fran\xe7ais",it:"Italiano"};return Object(h.jsx)("span",{children:Object(h.jsxs)("select",{name:"language",onChange:xe,children:[Object(h.jsxs)("option",{selected:!0,disabled:!0,children:[" ",N("selectLanguage")]}),Object.keys(e).map((function(t){return Object(h.jsx)("option",{value:t,children:e[t]},t)}))]})})}(),Object(h.jsx)("button",{className:"btn btn-primary",onClick:ge,children:!0===p?Object(h.jsx)("svg",{xmlns:"http://www.w3.org/2000/svg",width:"16",height:"16",fill:"currentColor",class:"bi bi-arrow-left",viewBox:"0 0 16 16",children:Object(h.jsx)("path",{"fill-rule":"evenodd",d:"M15 8a.5.5 0 0 0-.5-.5H2.707l3.147-3.146a.5.5 0 1 0-.708-.708l-4 4a.5.5 0 0 0 0 .708l4 4a.5.5 0 0 0 .708-.708L2.707 8.5H14.5A.5.5 0 0 0 15 8z"})}):Object(h.jsxs)("svg",{xmlns:"http://www.w3.org/2000/svg",width:"16",height:"16",fill:"currentColor",class:"bi bi-card-checklist",viewBox:"0 0 16 16",children:[Object(h.jsx)("path",{d:"M14.5 3a.5.5 0 0 1 .5.5v9a.5.5 0 0 1-.5.5h-13a.5.5 0 0 1-.5-.5v-9a.5.5 0 0 1 .5-.5h13zm-13-1A1.5 1.5 0 0 0 0 3.5v9A1.5 1.5 0 0 0 1.5 14h13a1.5 1.5 0 0 0 1.5-1.5v-9A1.5 1.5 0 0 0 14.5 2h-13z"}),Object(h.jsx)("path",{d:"M7 5.5a.5.5 0 0 1 .5-.5h5a.5.5 0 0 1 0 1h-5a.5.5 0 0 1-.5-.5zm-1.496-.854a.5.5 0 0 1 0 .708l-1.5 1.5a.5.5 0 0 1-.708 0l-.5-.5a.5.5 0 1 1 .708-.708l.146.147 1.146-1.147a.5.5 0 0 1 .708 0zM7 9.5a.5.5 0 0 1 .5-.5h5a.5.5 0 0 1 0 1h-5a.5.5 0 0 1-.5-.5zm-1.496-.854a.5.5 0 0 1 0 .708l-1.5 1.5a.5.5 0 0 1-.708 0l-.5-.5a.5.5 0 0 1 .708-.708l.146.147 1.146-1.147a.5.5 0 0 1 .708 0z"})]})}),Object(h.jsxs)("div",{className:"row",children:[Object(h.jsx)("div",{className:"col",children:Object(h.jsxs)("select",{className:"form-select",name:"catalog",onChange:function(e){var t=""===e.target.value?void 0:e.target.value;de(t);var a=ve();a.catalog=t,A(Oe(a))},children:[Object(h.jsx)("option",{value:"",children:N("selectCatalog")}),K.map((function(e){return Object(h.jsx)("option",{value:e,children:N("catalogs.".concat(e.toLowerCase()))},e)}))]})}),Object(h.jsx)("div",{className:"col",children:Object(h.jsxs)("select",{className:"form-select",name:"catalog",onChange:pe,children:[Object(h.jsxs)("option",{value:"",children:[" ",N("selectCategory")]}),q.map((function(e){return Object(h.jsx)("option",{value:e,children:N("categories.".concat(e.toLowerCase()))},e)}))]})}),Object(h.jsx)("div",{className:"col",children:Object(h.jsxs)("select",{className:"form-select",name:"catalog",onChange:me,children:[Object(h.jsxs)("option",{value:"",children:[" ",N("selectGenre")]}),$.map((function(e){return Object(h.jsx)("option",{value:e.id,children:e.name},e.id)}))]})}),Object(h.jsx)("div",{className:"col",children:function(){if(P)return Object(h.jsx)("div",{className:"spinner-border",role:"status",children:Object(h.jsxs)("span",{className:"visually-hidden",children:[" ",N("loading")]})})}()})]}),function(){if(R.length>0)return Object(h.jsx)("div",{style:je,children:Object(h.jsx)("input",{type:"search",style:be,onChange:he,placeholder:N("searchSongs")})})}(),Object(h.jsx)("div",{className:"ag-theme-alpine",style:{height:1e3},children:Object(h.jsx)(d.AgGridReact,{rowData:R,defaultColDef:{sortable:!0,resizable:!0,filter:!0,suppressMovable:!0},columnDefs:L,localeTextFunc:we,frameworkComponents:{checkboxRenderer:g},onGridReady:function(e){G(e.api)}})})]})}function v(){return Object(h.jsx)(b.a,{children:Object(h.jsx)(n.Suspense,{fallback:"loading",children:Object(h.jsx)(f,{})})})}var O=a(19),p=a(9),m=a(31),x=a(33);O.a.use(p.e).use(m.a).use(x.a).init({fallbackLng:"en",debug:!0,interpolation:{escapeValue:!1},backend:{loadPath:"/locales/{{lng}}/{{ns}}.json"}});O.a;l.a.render(Object(h.jsx)(c.a.StrictMode,{children:Object(h.jsx)(v,{})}),document.getElementById("root"))}},[[72,1,2]]]);
//# sourceMappingURL=main.8a07330d.chunk.js.map