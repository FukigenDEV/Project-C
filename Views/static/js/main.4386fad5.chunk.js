(this.webpackJsonpdashboard=this.webpackJsonpdashboard||[]).push([[0],{37:function(e,t,n){e.exports=n(52)},42:function(e,t,n){},50:function(e,t,n){},52:function(e,t,n){"use strict";n.r(t);var a=n(0),r=n.n(a),c=n(27),o=n.n(c),i=(n(42),n(15)),l=n(24),s=n(2),u=n(3),p=n(5),m=n(4),d=n(6),h=n(28),b=n(32),g=n(14);n(43);function v(e,t){var n=Object.keys(e);if(Object.getOwnPropertySymbols){var a=Object.getOwnPropertySymbols(e);t&&(a=a.filter((function(t){return Object.getOwnPropertyDescriptor(e,t).enumerable}))),n.push.apply(n,a)}return n}function f(e){for(var t=1;t<arguments.length;t++){var n=null!=arguments[t]?arguments[t]:{};t%2?v(n,!0).forEach((function(t){Object(i.a)(e,t,n[t])})):Object.getOwnPropertyDescriptors?Object.defineProperties(e,Object.getOwnPropertyDescriptors(n)):v(n).forEach((function(t){Object.defineProperty(e,t,Object.getOwnPropertyDescriptor(n,t))}))}return e}var O=function(e){function t(e){var n;return Object(s.a)(this,t),(n=Object(p.a)(this,Object(m.a)(t).call(this,e))).handleSubmit=function(e){e.preventDefault();var t=n.state.login,a=JSON.stringify(t),r=new XMLHttpRequest;r.open("POST","/login",!0),r.onreadystatechange=function(){if(4===r.readyState){var e=f({},n.state.alert);e.type=r.status,e.value=r.responseText,n.props.onLogin(r.status),n.setState({alert:e})}},r.setRequestHeader("Content-Type","application/json"),r.send(a)},n.handleChange=function(e){e.preventDefault();var t=f({},n.state.login);t[e.target.name]=e.target.value,n.setState({login:t})},n.getBadgeClasses=function(){var e="alert mr-3 ml-3 ";return e+=204===n.state.alert.type||200===n.state.alert.type?"alert-success":"alert-danger",e+=0===n.state.alert.type?" d-none":" d-block"},n.state={alert:{type:0,value:""},login:{Email:"",Password:"",RememberMe:!0}},n}return Object(d.a)(t,e),Object(u.a)(t,[{key:"render",value:function(){return r.a.createElement("div",{className:"container mx-auto vertical-center"},r.a.createElement("div",{className:"mx-auto vertical-center"},r.a.createElement("form",{className:"login-form",onSubmit:this.handleSubmit.bind(this)},r.a.createElement("div",{className:this.getBadgeClasses()},401===this.state.alert.type?"Wrong e-mail or paswword!":this.state.alert.value),r.a.createElement("input",{onChange:this.handleChange,type:"text",className:"form-control m-3",placeholder:"E-mail",name:"Email",autoComplete:"username"}),r.a.createElement("input",{onChange:this.handleChange,type:"password",className:"form-control m-3",placeholder:"Password",name:"Password",autoComplete:"current-password"}),r.a.createElement("button",{className:"btn btn-primary btn-lg mr-3 ml-3"},"Login"))))}}]),t}(a.Component),j=n(29),y=n(11);n(17).b.add(y.h,y.f,y.d,y.c,y.a,y.e,y.b,y.g);var E=function(e){function t(){return Object(s.a)(this,t),Object(p.a)(this,Object(m.a)(t).apply(this,arguments))}return Object(d.a)(t,e),Object(u.a)(t,[{key:"render",value:function(){var e=this,t=0===this.props.nav.id?r.a.createElement("h1",null,this.props.nav.heading):this.props.nav.heading,n=r.a.createElement("div",{className:"nav-icon"},r.a.createElement(j.a,{icon:["fas",this.props.nav.icon]}));return r.a.createElement(g.b,{onClick:function(){return e.props.onSelect(e.props.nav)},className:this.getClassesNav(),to:this.props.nav.link},n," ",t)}},{key:"getClassesNav",value:function(){var e="App-link";return e+=0===this.props.nav.id?" header":"",e+=this.props.nav.active?" selected":""}}]),t}(a.Component),w=function(e){function t(){return Object(s.a)(this,t),Object(p.a)(this,Object(m.a)(t).apply(this,arguments))}return Object(d.a)(t,e),Object(u.a)(t,[{key:"render",value:function(){var e=this.props,t=e.onSelect,n=e.navs;return r.a.createElement("div",{className:"col-lg-3 col-md-3 col-sm-12"},r.a.createElement("div",{className:"row"},r.a.createElement("div",{className:"col-md-3 shadow-sm Nav-Bar bg-white navvv"},r.a.createElement("nav",{className:"flex-column"},n.map((function(e){return r.a.createElement(E,{key:e.id,onSelect:t,nav:e})}))))))}}]),t}(a.Component),k=n(12),N=function(e){function t(){var e,n;Object(s.a)(this,t);for(var a=arguments.length,r=new Array(a),c=0;c<a;c++)r[c]=arguments[c];return(n=Object(p.a)(this,(e=Object(m.a)(t)).call.apply(e,[this].concat(r)))).getPointer=function(){var e=n.props.navs,t={path:"",component:null};for(var a in e)if(!0===e[a].active)return t.path=e[a].path,t.component=e[a].component,t},n}return Object(d.a)(t,e),Object(u.a)(t,[{key:"render",value:function(){var e=this.props.navs;return console.log(this.props.router),r.a.createElement("div",{className:"col-9 col-s-12 main-window"},r.a.createElement(k.c,null,e.map((function(e){return r.a.createElement(k.a,{exact:!0,path:e.path,component:e.component})})),r.a.createElement(k.a,{component:q})))}}]),t}(a.Component);n(50),n(51);function C(e,t){var n=Object.keys(e);if(Object.getOwnPropertySymbols){var a=Object.getOwnPropertySymbols(e);t&&(a=a.filter((function(t){return Object.getOwnPropertyDescriptor(e,t).enumerable}))),n.push.apply(n,a)}return n}function P(e){for(var t=1;t<arguments.length;t++){var n=null!=arguments[t]?arguments[t]:{};t%2?C(n,!0).forEach((function(t){Object(i.a)(e,t,n[t])})):Object.getOwnPropertyDescriptors?Object.defineProperties(e,Object.getOwnPropertyDescriptors(n)):C(n).forEach((function(t){Object.defineProperty(e,t,Object.getOwnPropertyDescriptor(n,t))}))}return e}function S(){var e=Object(h.a)(["body { background: rgb(2,0,36) !important; background: linear-gradient(90deg, rgba(2,0,36,1) 0%, rgba(35,35,102,1) 30%, rgba(0,142,255,1) 100%) !important;}"]);return S=function(){return e},e}var D=Object(b.a)(S()),B=function(e){function t(){var e,n;Object(s.a)(this,t);for(var a=arguments.length,r=new Array(a),c=0;c<a;c++)r[c]=arguments[c];return(n=Object(p.a)(this,(e=Object(m.a)(t)).call.apply(e,[this].concat(r)))).state={loggedin:{value:!1},navs:[{id:0,heading:"Project C",link:"",path:"/",component:R,active:!0,icon:"home"},{id:1,heading:"Gegevens bekijken",link:"GegevensBekijken",path:"/GegevensBekijken",component:A,active:!1,icon:"file-signature"},{id:2,heading:"Gegevens Registreren",link:"GegevensRegistreren",path:"/GegevensRegistreren",component:L,active:!1,icon:"file"},{id:3,heading:"Notities",link:"Notities",path:"/Notities",component:T,active:!1,icon:"clipboard"},{id:4,heading:"Activiteiten geschiedenis",link:"Activiteitengeschiedenis",path:"/Activiteitengeschiedenis",component:x,active:!1,icon:"history"},{id:5,heading:"Back-up maken",link:"Back-up",path:"/Back-up",component:G,active:!1,icon:"download"},{id:6,heading:"Uitloggen",link:"Uitloggen",path:"/Uitloggen",component:U,active:!1,icon:"sign-out-alt"}]},n.handleSelect=function(e){n.setFalse();var t=Object(l.a)(n.state.navs),a=t.indexOf(e);t[a]=P({},e),t[a].active=!0,n.setState({navs:t})},n.handleLogin=function(e){if(200===e||204===e){var t=P({},n.state.loggedin);t.value=!0,n.setState({loggedin:t})}},n.setFalse=function(){var e=Object(l.a)(n.state.navs);for(var t in e)e[t].active=!1;n.setState({navs:e})},n}return Object(d.a)(t,e),Object(u.a)(t,[{key:"componentWillMount",value:function(){var e=this,t=new XMLHttpRequest;t.open("POST","/login",!0),t.onreadystatechange=function(){if(200===t.status||204===t.status){var n=P({},e.state.loggedin);n.value=!0,e.setState({loggedin:n})}},t.setRequestHeader("Content-Type","application/json")}},{key:"componentWillUnmount",value:function(){console.log("UNMOUNTED")}},{key:"render",value:function(){return console.log("loggedin: ",this.state.loggedin.value),this.state.loggedin.value?r.a.createElement(r.a.Fragment,null,r.a.createElement(D,null),r.a.createElement("div",{className:"container-fluid"},r.a.createElement("div",{className:"row"},r.a.createElement(g.a,null,r.a.createElement(w,{navs:this.state.navs,onSelect:this.handleSelect}),r.a.createElement(N,{navs:this.state.navs}))))):r.a.createElement(r.a.Fragment,null,r.a.createElement(D,null),r.a.createElement(O,{onLogin:this.handleLogin}))}}]),t}(a.Component),R=function(e){function t(){return Object(s.a)(this,t),Object(p.a)(this,Object(m.a)(t).apply(this,arguments))}return Object(d.a)(t,e),Object(u.a)(t,[{key:"render",value:function(){return r.a.createElement("div",{className:"shadow-sm p-3 mb-5 bg-white rounded"},"Dashboard")}}]),t}(a.Component),L=function(e){function t(){return Object(s.a)(this,t),Object(p.a)(this,Object(m.a)(t).apply(this,arguments))}return Object(d.a)(t,e),Object(u.a)(t,[{key:"render",value:function(){return r.a.createElement("div",{className:"shadow-sm p-3 mb-5 bg-white rounded"},"GegevensRegistreren")}}]),t}(a.Component),A=function(e){function t(){return Object(s.a)(this,t),Object(p.a)(this,Object(m.a)(t).apply(this,arguments))}return Object(d.a)(t,e),Object(u.a)(t,[{key:"render",value:function(){return r.a.createElement("div",{className:"shadow-sm p-3 mb-5 bg-white rounded"},r.a.createElement("h1",null,"Gegevens bekijken"),r.a.createElement("br",null),r.a.createElement("div",{className:"tabel"},r.a.createElement("table",null,r.a.createElement("tr",null,r.a.createElement("th",null,"Naam"),r.a.createElement("th",null,"Afdeling"),r.a.createElement("th",null,"Aangemaakt op")),r.a.createElement("tr",null,r.a.createElement("td",null,"Very interesting collection of data #1"),r.a.createElement("td",null,"HR"),r.a.createElement("td",null,"13/10/19 14:03")),r.a.createElement("tr",null,r.a.createElement("td",null,"Very interesting collection of data #2"),r.a.createElement("td",null,"HR"),r.a.createElement("td",null,"13/10/19 15:34")),r.a.createElement("tr",null,r.a.createElement("td",null,"Very interesting collection of data #3"),r.a.createElement("td",null,"IT"),r.a.createElement("td",null,"13/10/19 15:47")),r.a.createElement("tr",null,r.a.createElement("td",null,"Very interesting collection of data #4"),r.a.createElement("td",null,"IT"),r.a.createElement("td",null,"13/10/19 15:52")))))}}]),t}(a.Component),T=function(e){function t(){return Object(s.a)(this,t),Object(p.a)(this,Object(m.a)(t).apply(this,arguments))}return Object(d.a)(t,e),Object(u.a)(t,[{key:"render",value:function(){return r.a.createElement("div",{className:"shadow-sm p-3 mb-5 bg-white rounded"},"Notities")}}]),t}(a.Component),x=function(e){function t(){return Object(s.a)(this,t),Object(p.a)(this,Object(m.a)(t).apply(this,arguments))}return Object(d.a)(t,e),Object(u.a)(t,[{key:"render",value:function(){return r.a.createElement("div",{className:"shadow-sm p-3 mb-5 bg-white rounded"},"Activiteitengeschiedenis")}}]),t}(a.Component),G=function(e){function t(){return Object(s.a)(this,t),Object(p.a)(this,Object(m.a)(t).apply(this,arguments))}return Object(d.a)(t,e),Object(u.a)(t,[{key:"render",value:function(){return r.a.createElement("div",{className:"shadow-sm p-3 mb-5 bg-white rounded"},"Backup")}}]),t}(a.Component);function H(e,t){var n=Object.keys(e);if(Object.getOwnPropertySymbols){var a=Object.getOwnPropertySymbols(e);t&&(a=a.filter((function(t){return Object.getOwnPropertyDescriptor(e,t).enumerable}))),n.push.apply(n,a)}return n}var U=function(e){function t(e){var n;return Object(s.a)(this,t),(n=Object(p.a)(this,Object(m.a)(t).call(this,e))).handleLogout=function(e){e.preventDefault();var t=n.state.login,a=JSON.stringify(t),r=new XMLHttpRequest;r.open("DELETE","/login",!0),r.onreadystatechange=function(){if(4===r.readyState){var e=function(e){for(var t=1;t<arguments.length;t++){var n=null!=arguments[t]?arguments[t]:{};t%2?H(n,!0).forEach((function(t){Object(i.a)(e,t,n[t])})):Object.getOwnPropertyDescriptors?Object.defineProperties(e,Object.getOwnPropertyDescriptors(n)):H(n).forEach((function(t){Object.defineProperty(e,t,Object.getOwnPropertyDescriptor(n,t))}))}return e}({},n.state.alert);e.type=r.status,e.value=r.responseText,n.setState({alert:e}),n.props.history.push("/index.html")}},r.setRequestHeader("Content-Type","application/json"),r.send(a)},n.getBadgeClasses=function(){var e="alert mr-3 ml-3 ";return e+=200===n.state.alert.type?"alert-success":"alert-danger",e+=0===n.state.alert.type?" d-none":" d-block"},n.state={alert:{type:0,value:""}},n}return Object(d.a)(t,e),Object(u.a)(t,[{key:"render",value:function(){return r.a.createElement("div",{className:"col-sm-12 shadow-sm p-3 mb-5 bg-white rounded"},"Weet je zeker dat je wilt uitloggen?",r.a.createElement("br",null),r.a.createElement("br",null),r.a.createElement("br",null),r.a.createElement("div",{className:this.getBadgeClasses()},401===this.state.alert.type?"An error occured, couldn't log out":this.state.alert.value),r.a.createElement("button",{onClick:this.handleLogout,className:"logout-button"},"Log uit"))}}]),t}(a.Component),q=function(e){function t(){return Object(s.a)(this,t),Object(p.a)(this,Object(m.a)(t).apply(this,arguments))}return Object(d.a)(t,e),Object(u.a)(t,[{key:"render",value:function(){return r.a.createElement("div",{className:"shadow-sm p-3 mb-5 bg-white rounded"},"404")}}]),t}(a.Component);Boolean("localhost"===window.location.hostname||"[::1]"===window.location.hostname||window.location.hostname.match(/^127(?:\.(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)){3}$/));n.d(t,"Dashboard",(function(){return R})),n.d(t,"GegevensRegistreren",(function(){return L})),n.d(t,"GegevensBekijken",(function(){return A})),n.d(t,"Notities",(function(){return T})),n.d(t,"Activiteitengeschiedenis",(function(){return x})),n.d(t,"Backup",(function(){return G})),n.d(t,"Uitloggen",(function(){return U})),n.d(t,"Error",(function(){return q})),o.a.render(r.a.createElement(B,null),document.getElementById("root")),"serviceWorker"in navigator&&navigator.serviceWorker.ready.then((function(e){e.unregister()}))}},[[37,1,2]]]);
//# sourceMappingURL=main.4386fad5.chunk.js.map