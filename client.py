import requests

address = "http://localhost/login"
response = requests.get(address, json={"Email": "Administrator", "Password": "W@chtw00rd", "RememberMe": True})
print(response.cookies.get_dict())
print(response.content)
print(response.status_code)
print("---")

#Cookies = dict(SessionID="ZbvIdpiQcUWaFSggxnmdpA==")
address = "http://localhost/account"
response = requests.post(address, json={"Email": "Test2", "Password": "Yeet", "Firstname": "1", "Lastname": "2"}, cookies=response.cookies)
print(response.cookies.get_dict())
print(response.content)
print(response.status_code)
print("---")