import requests
address = "http://localhost/login"

Cookies = dict(SessionID="eXtPvwFWBkalwIkD0buJNA==")
response = requests.get(address, json={}, cookies=Cookies)
print(response.cookies.get_dict())
print(response.content)
print(response.status_code)