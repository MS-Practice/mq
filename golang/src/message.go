package src

import "time"

type CreateUserMessage struct {
	UserName string
	Age      uint8
	Sex      bool
	Email    string
	Birthday time.Time
}
