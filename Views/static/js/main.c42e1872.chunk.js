(window["webpackJsonpcounter-app"]=window["webpackJsonpcounter-app"]||[]).push([[0],{18:function(e,t,n){e.exports=n(33)},23:function(e,t,n){},24:function(e,t,n){},26:function(e,t,n){},33:function(e,t,n){"use strict";n.r(t);var a=n(0),r=n.n(a),o=n(11),c=n.n(o),l=(n(23),n(1)),i=n(2),s=n(4),u=n(3),m=n(5),p=n(12),b=n(6),d=n(13);n(24),n(25);function g(e,t){var n=Object.keys(e);if(Object.getOwnPropertySymbols){var a=Object.getOwnPropertySymbols(e);t&&(a=a.filter((function(t){return Object.getOwnPropertyDescriptor(e,t).enumerable}))),n.push.apply(n,a)}return n}function f(e){for(var t=1;t<arguments.length;t++){var n=null!=arguments[t]?arguments[t]:{};t%2?g(n,!0).forEach((function(t){Object(d.a)(e,t,n[t])})):Object.getOwnPropertyDescriptors?Object.defineProperties(e,Object.getOwnPropertyDescriptors(n)):g(n).forEach((function(t){Object.defineProperty(e,t,Object.getOwnPropertyDescriptor(n,t))}))}return e}var h=function(e){function t(e){var n;return Object(l.a)(this,t),(n=Object(s.a)(this,Object(u.a)(t).call(this,e))).handleSubmit=function(e){e.preventDefault();var t=n.state.login,a=JSON.stringify(t),r=new XMLHttpRequest;r.open("POST","/login",!0),r.onreadystatechange=function(){if(4===r.readyState){var e=f({},n.state.alert);e.type=r.status,e.value=r.responseText,n.setState({alert:e})}},r.setRequestHeader("Content-Type","application/json"),r.send(a)},n.handleChange=function(e){e.preventDefault();var t=f({},n.state.login);t[e.target.name]=e.target.value,n.setState({login:t})},n.getBadgeClasses=function(){var e="alert m3 ";return e+=200===n.state.alert.type?"alert-success":"alert-danger",e+=0===n.state.alert.type?" d-none":" d-block"},n.state={alert:{type:0,value:""},login:{Email:"",Password:"",RememberMe:!0}},n}return Object(m.a)(t,e),Object(i.a)(t,[{key:"render",value:function(){return r.a.createElement("div",{className:"App mx-auto vertical-center"},r.a.createElement("form",{className:"login-form",onSubmit:this.handleSubmit.bind(this)},r.a.createElement("div",{className:this.getBadgeClasses()},r.a.createElement("b",null,this.state.alert.type,": "),this.state.alert.value),r.a.createElement("input",{onChange:this.handleChange,type:"text",className:"form-control m-3",placeholder:"E-mail",name:"Email",autoComplete:"username"}),r.a.createElement("input",{onChange:this.handleChange,type:"password",className:"form-control m-3",placeholder:"Password",name:"Password",autoComplete:"current-password"}),r.a.createElement("button",{className:"btn btn-primary btn-lg mr-3 ml-3"},"Login")))}}]),t}(a.Component);n(26);function v(){var e=Object(p.a)(["\n  body {\n    background: rgb(2,0,36) !important;\n    background: linear-gradient(90deg, rgba(2,0,36,1) 0%, rgba(35,35,102,1) 30%, rgba(0,142,255,1) 100%) !important;\n  }\n"]);return v=function(){return e},e}var O=Object(b.a)(v()),y=function(e){function t(){return Object(l.a)(this,t),Object(s.a)(this,Object(u.a)(t).apply(this,arguments))}return Object(m.a)(t,e),Object(i.a)(t,[{key:"render",value:function(){return r.a.createElement(r.a.Fragment,null,r.a.createElement(O,null),r.a.createElement("div",{className:"container mx-auto vertical-center"},r.a.createElement(h,null)))}}]),t}(a.Component);Boolean("localhost"===window.location.hostname||"[::1]"===window.location.hostname||window.location.hostname.match(/^127(?:\.(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)){3}$/));n(32);c.a.render(r.a.createElement(y,null),document.getElementById("root")),"serviceWorker"in navigator&&navigator.serviceWorker.ready.then((function(e){e.unregister()}))}},[[18,1,2]]]);
//# sourceMappingURL=main.c42e1872.chunk.js.map