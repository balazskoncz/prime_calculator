import math

def find_next_prime(number:int) -> int:
    next_number = number + 1

    while not is_prime(next_number):
        next_number = next_number + 1

    return next_number

def is_prime(number:int) -> bool:
    """
    Implemenets simple primality test pseudo code specifid here:
    https://en.wikipedia.org/wiki/Primality_test
    """

    if number <= 3:
        return number > 1
    elif number % 2 == 0 or number % 3 == 0:
        return False
    else:

        iterator = 5

        while math.pow(iterator, 2) <= number:
            if number % iterator == 0 or number % (iterator + 2) == 0:
                return False

            iterator = iterator + 6

        return True