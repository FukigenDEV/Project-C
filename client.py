import requests
address = "http://localhost/example"

response = requests.get(address, json={"hello": "world"})
print(response.content)
print(response.status_code)