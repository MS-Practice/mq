package src

import (
	"errors"

	"github.com/streadway/amqp"
)

type FangZhiRabbitMQ struct {
	conn   *amqp.Connection
	config *Config
}

func NewRabbitMQ(cfg *Config) *FangZhiRabbitMQ {
	return &FangZhiRabbitMQ{
		config: cfg,
	}
}

func (mq *FangZhiRabbitMQ) Connect() (*amqp.Connection, error) {
	if mq.config == nil {
		return nil, errors.New("配置文件为空")
	}
	conn, err := amqp.Dial(mq.config.ConnectionString())
	mq.conn = conn
	return conn, err
}
