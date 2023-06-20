
## Dependencies

**Redis**
https://redis.io/download/
*Configured Port:* 6379

**RabbitMQ**
https://www.rabbitmq.com/download.html
*Configured Port: 5672*


## Additional Libraries

- AspNetCoreRateLimit
- Newtonsoft.Json
- AutoMapper
- MicrosoftExtensions.Caching.Redis
- RabbitMQ.Client
- Swagger
- Configuration management libraries


## API Authentication
Basic authentication

See username and password in  
*EagleRockGateway project > appSettings.json*
in the following section.

"ApiAuth": {
    "Username": "xxxxx",
    "Password": "xxxxx"
},