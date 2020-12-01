package src

import "fmt"

type Config struct {
	Host     string
	Port     int
	UserName string
	Password string
	Vhost    string
}

func NewConfigSimple(host string, port int) Config {
	return NewConfig(host, port, "guest", "guest", "/")
}

func NewConfig(host string, port int, username string, password string, vhost string) Config {
	if port == 0 {
		port = 5672
	}
	if vhost == "" {
		vhost = "/"
	}
	return Config{host, port, username, password, vhost}
}

func (cfg Config) WithLogin(username string, password string) Config {
	cfg.UserName = username
	cfg.Password = password
	return cfg
}

func (cfg Config) ConnectionString() string {
	connectString := fmt.Sprintf("amqp://{0}:{1}@{2}:{3}/", cfg.UserName, cfg.Password, cfg.Host, cfg.Port)
	return connectString
}
