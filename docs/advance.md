# EasyNetQ 高级应用

一般情况下，交换机（Exchange）、绑定（bindings）以及队列（queue）是根据发出的消息类型默认生成的。但是如有必要，EasyNetQ 还是提供了一些高级 API 支持自定义这些信息的。

EasyNetQ 通过接口 `IAdvancedBus` 暴露这些操作。

```c#
var advancedBus = RabbitHutch.CreateBus("host=localhost").Advanced;
```

## 申明交换机 Exchange

```c#
advancedBus.ExchangeDeclare(string name, string type, bool durable = true, bool autoDelete = false);

name: exchange 名称
type: exchange 类型，必须是 AMQP 协议的有效类型。可以使用静态属性类 ExchangeType 安全的申明
durable：服务重启时，持久化交换机可以保持活跃
autoDelete：如果设置了 true，当所有队列都完成时，交换机会自动删除
// 举个例子
// 直接交换机（direct）
var exchange = advancedBus.ExchangeDeclare("my_exchange", ExchangeType.Direct);

// 主题交换机（topic）
var exchange = advancedBus.ExchangeDeclare("my_exchange", ExchangeType.Topic);

// 申明一个分裂交换机
var exchange = advancedBus.ExchangeDeclare("my_exchange", ExchangeType.Fanout);

// 获取默认的交换机
var defaultExchange = Exchange.GetDefault();
```

## 申明队列 Queue

```c#
advancedBus.QueueDeclare(string name, bool durable, bool exclusive, bool autoDelete);

name: 队列名称
durable：服务重启时，持久化队列保持活跃
exclusive：排他队列只有在当前连接建立时访问，连接关闭时会自动删除
autoDelete：队列所有消息被消费完了会自动删除

// 申明持久化队列
advancedBus.QueueDeclare("my_queue");
// 申明一个未命名的排他队列
advancedBus.QueueDeclare();
```

## 绑定

绑定队列到交换机上：

```
var queue = advancedBus.QueueDeclare("my.queue");
var exchange = advancedBus.ExchangeDeclare("my.exchange", ExchangeType.Topic);
var binding = advancedBus.Bind(exchange, queue, "A.*");
```

相同的队列和交换机可以指定多个绑定

```
var queue = advancedBus.QueueDeclare("my.queue");
var exchange = advancedBus.ExchangeDeclare("my.exchange", ExchangeType.Topic);
advancedBus.Bind(exchange, queue, "A.B");
advancedBus.Bind(exchange, queue, "A.C");
```

也可以在交换机之间指定绑定

```
var sourceExchange = advancedBus.ExchangeDeclare("my.exchange.1", ExchangeType.Topic);
var destinationExchange = advancedBus.ExchangeDeclare("my.exchange.2", ExchangeType.Topic);
var queue = advancedBus.QueueDeclare("my.queue");

advancedBus.Bind(sourceExchange, destinationExchange, "A.*");
advancedBus.Bind(destinationExchange, queue, "A.C");
```

## 推送

高级 api 推送方法 `Publish` 允许你指定消息想要发送的交换机。它也支持你访问消息的 AMQP 基本属性。

利用 `Message` 将你的消息包装进去

```
var myMessage = new MyMessage { Text = "Hello World" };
var message = new Message<MyMessage>(myMessage);
```

然后 message 允许你访问 AMQP 的基本属性：

```
message.Properties.AppId = "my_app_id";
message.Properties.ReplyTo = "my_reply_queue";
```

最后发布消息

```
bus.Publish(Exchange.GetDefault(), queueName, false, false, message);
```

`Publish` 重载方法支持你传递 EasyNetQ 消息序列化以及创建你自己的字节数组消息：

```
var properties = new MessageProperties();
var body = Encoding.UTF8.GetBytes("Hello World!");
bus.Publish(Exchange.GetDefault(), queueName, false, false, properties, body);
```

