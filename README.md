# Installation
1. get a encryption token
	curl -v 'http://radioyamaha.vtuner.com/setupapp/Yamaha/asp/BrowseXML/loginXML.asp?token=0'

	response-body looks like: "<EncryptedToken>1234567890123456</EncryptedToken>"
	insert this into your appsettings.json

# mvc core specific
- mvc core 2.0 does not support precompiled Razor Views for self contained deployments

# Vtuner <-> reciever specific
- ListOfItems XML response MUST NOT have any faulty Urls - crashes receiver
- my receiver does not support chunked encoding: therfore add ResponseBuffering
