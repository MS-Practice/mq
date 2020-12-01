package main

import (
	"bytes"
	"encoding/gob"
	"gomq/src"
	"log"
	"time"

	"github.com/streadway/amqp"
)

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

	// 申明交换机
	declareExchange(ch)
	// 绑定交换机
	err = ch.QueueBind(queue.Name, queue.Name, "MQ.Shared.Messages.CreateUserMessage, MQ.Shared", false, nil)
	failOnError(err, "绑定队列失败")
	// 发送消息
	err = publish(ch, queue, &src.CreateUserMessage{"marsonshine", 27, true, "marson@163.com", time.Now()})
	failOnError(err, "发送消息失败")
}

func publish(ch *amqp.Channel, queue amqp.Queue, body interface{}) error {

	var network bytes.Buffer
	gob.Register(src.CreateUserMessage{})
	enc := gob.NewEncoder(&network)
	err := enc.Encode(body)
	if err != nil {
		return err
	}

	err = ch.Publish(
		"",
		queue.Name,
		false,
		false,
		amqp.Publishing{
			ContentType: "text/plain",
			Body:        network.Bytes(),
		})
	log.Printf("[x] 发送消息 %s", body)
	return err
}

func declareQueue(ch *amqp.Channel) (amqp.Queue, error) {
	queue, err := ch.QueueDeclare(
		string(src.UserQueueString),
		true,
		false,
		false,
		false,
		nil)

	return queue, err
}
func declareExchange(ch *amqp.Channel) {
	err := ch.ExchangeDeclare("MQ.Shared.Messages.CreateUserMessage, MQ.Shared", "topic", true, false, false, false, nil)
	failOnError(err, "Failed to declare an exchange")
}
func failOnError(err error, msg string) {
	if err != nil {
		log.Fatalf("%s: %s", msg, err)
	}
}
