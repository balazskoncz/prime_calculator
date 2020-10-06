from flask import Flask, request, Response
from flask_restful import Api
from resources.find_next_prime import FindNextPrime
from resources.number_is_prime import NumberIsPrime


app = Flask(__name__)

@app.route("/info", methods=['GET'])
def info():
    return "science service is up and running"

api = Api(app)
api.add_resource(NumberIsPrime, "/NumberIsPrime")
api.add_resource(FindNextPrime, "/FindNextPrime")

app.run(host='0.0.0.0', debug=True, port=5010)