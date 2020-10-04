from flask import request, Response
from flask_restful import Resource
from calculator import find_next_prime
from Generated import science_pb2

class FindNextPrime(Resource):

    def post(self):
        data = request.files["protomessage"].read()

        calculationRequest = science_pb2.PrimeCalculationRequest()
        calculationRequest.ParseFromString(data)

        response = science_pb2.NextPrimeResponse()

        response.nextPrime = find_next_prime(calculationRequest.number)

        return Response(response.SerializeToString(), mimetype='application/octet-stream')