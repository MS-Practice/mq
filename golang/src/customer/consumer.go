package main

import (
	"bytes"
	"encoding/gob"
	"gomq/src"
	"log"

	"github.com/streadway/amqp"
)

func failOnError(err error, msg string) {
	if err != nil {
		log.Fatalf("%s: %s", msg, err)
	}
}

func main() {
	conn, err := amqp.Dial("amqp://guest:guest@localhost:5672")
	failOnError(err, "RabbitMQ 连接失败！")
	defer conn.Close()

	ch, err := conn.Channel()
	failOnError(err, "打开通信通道失败！")
	defer ch.Close()

	// 申明队列
	queue, err := declareQueue(ch)
	failOnError(err, "队列申明失败")

	// 消费队列
	err = consumer(ch, queue)
	failOnError(err, "接受消息失败")
}

func consumer(ch *amqp.Channel, queue amqp.Queue) error {
	msgs, err := ch.Consume(queue.Name, "", true, false, false, false, nil)
	failOnError(err, "消费者注册失败")

	forever := make(chan bool)
	go func() {
		for d := range msgs {
			buf := bytes.NewBuffer(d.Body)
			var user = src.CreateUserMessage{}
			dec := gob.NewDecoder(buf)
			err := dec.Decode(&user)

			if err != nil {
				log.Printf("接受消息失败: %s", err.Error())
			} else {
				log.Printf("Received a message: %v", user)
			}

		}
	}()
	log.Printf(" [*] Waiting for messages. To exit press CTRL+C")
	<-forever

	return err
}

func declareQueue(ch *amqp.Channel) (amqp.Queue, error) {
	queue, err := ch.QueueDeclare(
		string(src.UserQueueString),
		true,
		true,
		false,
		false,
		nil)

	return queue, err
}
