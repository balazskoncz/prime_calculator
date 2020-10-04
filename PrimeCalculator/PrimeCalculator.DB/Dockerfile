FROM postgres:11.5
EXPOSE 5432
# Set the locale
RUN apt-get update
RUN apt-get install -y locales locales-all
ENV LC_ALL hu_HU.UTF-8
ENV LANG hu_HU.UTF-8
ENV LANGUAGE hu_HU.UTF-8
ENV POSTGRES_DB primes
ENV POSTGRES_USER primes
ENV POSTGRES_PASSWORD primes