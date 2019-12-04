import requests
import json

# address = "http://localhost/login"
# response = requests.post(address, json={"Email": "Administrator", "Password": "W@chtw00rd", "RememberMe": True})
# print(response.cookies.get_dict())
# print(response.content)
# print(response)
# print("---")

Cookies = dict(SessionID="7xZ5YxF3dE61Eg9p51wcxA==")

address = "http://localhost/account?department=Administrators"
JSON = {
	"Email": "memes@memes.com",
	"Password": "whateverlol",
	"AccountType": "User",
	"MemberOf": "All Users"
}
response = requests.get(address, json=JSON, cookies=Cookies)
print(response.headers)
print(json.loads(response.content))
print(response)
print("---")