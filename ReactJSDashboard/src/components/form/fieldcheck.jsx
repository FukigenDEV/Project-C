function standard(field) {
  if(field === undefined || field === "") {
    return "Dit veld moet worden ingevuld!";
  } else {
    return false;
  }
}

function email(field) {
  var re = /^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
  if(re.test(field)) {
    return false;
  } else if(field === "") {
    return "Dit veld moet worden ingevuld!";
  } else {
    return "E-mailadres is incorrect!"
  }
}

export {
  standard,
  email,
}