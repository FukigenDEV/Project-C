import requests
import json

# address = "http://localhost/login"
# response = requests.post(address, json={"Email": "Administrator", "Password": "W@chtw00rd", "RememberMe": True})
# print(response.cookies.get_dict())
# print(response.content)
# print(response)
# print("---")

Cookies = dict(SessionID="vDWRTROEh0+YBJJoCdwdEg==")

address = "http://localhost/account"
JSON = {
	"Email": "test@example.com",
	"Password": "yeetskeet420",
	"MemberDepartments": {
		"All Users": "Manager"
	}
}
response = requests.post(address, json=JSON, cookies=Cookies)
print(response.headers)
print(response.content)
print(response)
print("---")