package main

import (
	"encoding/json"
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
	err = ch.QueueBind(queue.Name, "go.create.user", "platform.exchange.user", false, nil)
	failOnError(err, "绑定队列失败")
	// 发送消息
	err = publish(ch, queue, &src.CreateUserMessage{"marsonshine", 27, true, "marson@163.com", time.Now()})
	failOnError(err, "发送消息失败")
}

func publish(ch *amqp.Channel, queue amqp.Queue, body interface{}) error {
	buffer, err := json.Marshal(body)
	// var network bytes.Buffer

	// gob.Register(src.CreateUserMessage{})
	// enc := gob.NewEncoder(&network)
	// err := enc.Encode(body)
	if err != nil {
		return err
	}

	err = ch.Publish(
		"",
		queue.Name,
		false,
		false,
		amqp.Publishing{
			ContentType: "applicaton/json",
			Body:        buffer,
		})
	log.Printf("[x] 发送消息 %s", body)
	return err
}

func declareQueue(ch *amqp.Channel) (amqp.Queue, error) {
	queue, err := ch.QueueDeclare(
		"platform.queue.user",
		true,
		false,
		false,
		false,
		nil)

	return queue, err
}
func declareExchange(ch *amqp.Channel) {
	err := ch.ExchangeDeclare("platform.exchange.user", "direct", true, false, false, false, nil)
	failOnError(err, "Failed to declare an exchange")
}
func failOnError(err error, msg string) {
	if err != nil {
		log.Fatalf("%s: %s", msg, err)
	}
}
