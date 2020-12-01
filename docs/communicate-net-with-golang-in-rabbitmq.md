# RabbitMQ 连接 .Net 与 Go

## 摘要

因为 .Net 与 Golang 用的消息中间件都是 RabbitMQ，所以自然存在跨语言消息调度消费的问题。本身消息中间件是与语言无关的，只要.Net 与 Golang 都实现了 RabbitMQ 的消息协议（比如 AMQP 协议）就能完成一方消息的推送，另一方消费的目的。

## 问题清单

### Go 端发送消息至 RabbitMQ，.NET 无法正常接收

这多半（以我目前碰到的情况）是因为语言两者之间序列化消息（Message）的格式不同导致的。在这里我将一个对象序列化为字节数组推送给 RabbitMQ，用到了 gob 库：

```go
// 前面代码省略
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
			Body:        buffer,
		}
)
...
```

.NET 接收端：

```
// 省略声明交换机与队列和后续的绑定操作
bus.Advanced.Consume(queue,(body,properties,info)=>{
    string json = UTF8Encoding.UTF8.GetString(body);
    var obj = System.Text.Json.JsonSerializer.Deserialize<CreateUserMessage>(json);
});
```

运行 .NET 端消费失败，在反序列化时报错，调试发现取出来的 `json` 值是乱码的，遂以为是 golang 发送消息时序列化时的格式没有设置正确，随后查看发现 golang 默认也是用的 utf-8 的编码格式。

后来查阅一番资料，发现 `encoding/gob` 这个库是不支持跨语言的。只使用于 go 特定平台，所以它的性能才非常高。原话是这么说的

> But for a Go-specific environment, such as communicating between two servers written in Go, there's an opportunity to build something much easier to use and possibly more efficient.
>
> Gobs work with the language in a way that an externally-defined, language-independent encoding cannot. At the same time, there are lessons to be learned from the existing systems.

这里就说明了为了更高的性能，选择了自描述的新协议 gob 用于 go 与 go 程序内部的交互。而后的一段话直接点名了 gob 是专属于 go 平台的

> First, and most obvious, it had to be very easy to use. First, because Go has reflection, there is no need for a separate interface definition language or "protocol compiler". The data structure itself is all the package should need to figure out how to encode and decode it. On the other hand, this approach means that gobs will never work as well with other languages, but that's OK: gobs are unashamedly Go-centric.

最后一句话就说明了其 gob 的使用的平台限制。

所以要支持不同语言的消息调度，那就直接用 json 字符串来传递消息：

```go
golang:
...
buffer, err := json.Marshal(body)
err = ch.Publish(
		"",
		queue.Name,
		false,
		false,
		amqp.Publishing{
			ContentType: "text/plain",
			Body:        buffer,
		}
)
...
```

这样消息就能正常在多语言之间交互了。