FROM python:3.7

WORKDIR /usr/src/app

COPY requirements.txt ./

RUN pip install --upgrade pip
RUN pip install --no-cache-dir -r requirements.txt

ENV ISINCONTAINER=True   

COPY . .
EXPOSE 5010
CMD [ "python", "./app.py" ]