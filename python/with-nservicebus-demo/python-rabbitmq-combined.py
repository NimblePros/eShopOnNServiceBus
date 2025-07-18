#!/usr/bin/env 
# pip3 install pika
# pip install pika
import pika, sys, os

RABBITMQ_HOST = 'localhost'
RABBITMQ_PORT = 5672

RABBITMQ_USERNAME = 'rabbitUser'
RABBITMQ_PASSWORD = 'rabbitPassword'

def main():
    credentials = pika.PlainCredentials(username=RABBITMQ_USERNAME, password=RABBITMQ_PASSWORD)
    connection = pika.BlockingConnection(pika.ConnectionParameters(host=RABBITMQ_HOST, port=RABBITMQ_PORT, credentials=credentials))
    channel = connection.channel()

    channel.queue_declare(queue='orders',durable=True,arguments={"x-queue-type": "quorum"})
    channel.queue_declare(queue='payments',durable=True,arguments={"x-queue-type": "quorum"})

    def callback(ch, method, properties, body):
        print(f" [x] Received {body}")
        channel.basic_publish(exchange='', routing_key='payments', body='Payment Processed')
        print(" [x] Sent 'Payment Processed'")


    channel.basic_consume(queue='orders', on_message_callback=callback, auto_ack=True)

    print(' [*] Waiting for messages. To exit press CTRL+C')
    channel.start_consuming()

if __name__ == '__main__':
    try:
        main()
    except KeyboardInterrupt:
        print('Interrupted')
        try:
            sys.exit(0)
        except SystemExit:
            os._exit(0)