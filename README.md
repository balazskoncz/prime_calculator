# prime_calculator

Assumptions:

 - There is no distributed cache available around in the ecosystem.
 - There is no fronted (i.e: a javascrip fronted) that would use the api. Therefore I set CORS settings accordingly.
 - There is no central logging implemented around the ecosystem where the logs should be forwarded. Therefore I implementede a local mini logging.
 - I assume that other featore requests could come in the feature. Therefore I implemented a mini service in python for mathematical calculations.

The swagger is available via:
http://localhost:5000/swagger/index.html

For a typical start use docker-compose:
"docker-compose -f .\compose-service.yaml up"
