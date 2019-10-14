import requests

address = "http://localhost/example"
response = requests.get(address, json={})
print(response.cookies.get_dict())
print(response.content)
print(response.status_code)
print("---")

address = "http://localhost/login"
response = requests.get(address, json={"Email": "Administrator", "Password": "W@chtw00rd", "RememberMe": True})
print(response.cookies.get_dict())
print(response.content)
print(response.status_code)
print("---")

#Cookies = dict(SessionID="HeDpg+0A2025dcQnLnz2AA==")
address = "http://localhost/example"
response = requests.get(address, json={}, cookies=response.cookies)
print(response.cookies.get_dict())
print(response.content)
print(response.status_code)
print("---")