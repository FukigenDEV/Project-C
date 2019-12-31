import requests
import json

# address = "http://localhost/login"
# response = requests.post(address, json={"Email": "Administrator", "Password": "W@chtw00rd", "RememberMe": True})
# print(response.cookies.get_dict())
# print(response.content)
# print(response)
# print("---")

Cookies = dict(SessionID="f0MFQxFu+ky32iN5uCUEqQ==")

address = "http://localhost/department?name=Administrators"
JSON = {

}
response = requests.get(address, json=JSON, cookies=Cookies)
print(response.headers)
print(response.content)
print(response)
print("---")