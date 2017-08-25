1. get a encryption token
curl -v 'http://radioyamaha.vtuner.com/setupapp/Yamaha/asp/BrowseXML/loginXML.asp?token=0'

response-body looks like: "<EncryptedToken>1234567890123456</EncryptedToken>"
insert this into your appsettings.json

- ListOfItems XML response MUST NOT have any faulty Urls - crashes receiver