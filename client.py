import requests
address = "http://localhost/example"

response = requests.get(address, json={"hello": "world"})
print(response.cookies.get_dict())
print(response.content)
print(response.status_code)