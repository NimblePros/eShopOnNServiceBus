#!/usr/bin/env python
import pika

RABBITMQ_HOST = 'localhost'
RABBITMQ_PORT = 5672

RABBITMQ_USERNAME = 'rabbitUser'
RABBITMQ_PASSWORD = 'rabbitPassword'

credentials = pika.PlainCredentials(username=RABBITMQ_USERNAME, password=RABBITMQ_PASSWORD)


connection = pika.BlockingConnection(
    pika.ConnectionParameters(host=RABBITMQ_HOST,port=RABBITMQ_PORT, credentials=credentials))
channel = connection.channel()

channel.queue_declare(queue='hello')

channel.basic_publish(exchange='', routing_key='hello', body='Hello World!')
print(" [x] Sent 'Hello World!'")
connection.close()