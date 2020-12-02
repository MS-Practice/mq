# 基于 RabbitMQ 实现跨语言的消息调度

微服务的盛行，使我们由原来的单机”巨服务“的项目拆分成了不同的业务相对独立的模块，以及与业务不相关的中间件模块。这样我们免不了在公司不同的模块项目使用不同的团队，而各自的团队所擅长的开发语言也会不一致（当然，我想大多数都是统一了语言体系）。但是在微服务体系下，使用各自语言的优势开发对应的模块是最合适也是合理的诉求。

现在以消息中间件为例子，我们用 rabbitmq 将 .NET 和 Golang 连接起来。

## 前提

RabbitMQ 的准备工作这里省略，用 docker 可以很快的搭建出来，详情请移步谷歌。这里我也给一个我查资料的记录：[Docker 安装运行 Rabbitmq](https://github.com/MS-Practice/MyDocker#docker-%E5%AE%89%E8%A3%85%E8%BF%90%E8%A1%8C-rabbitmq)

## .NET 

关于 .NET 的 RabbitMQ 的消息中间件组件我们使用 [EasyNetQ](https://github.com/EasyNetQ/EasyNetQ) 对消息进行管理调度。我们以新建一个 `MQ.EasyNetQ.Producer` api 项目。我们根据 [EasyNetQ 官方文档的 Quick-Start](https://github.com/EasyNetQ/EasyNetQ/wiki/Quick-Start) 的例子在 `Program.cs` 新建一个 RabbitMQ 连接并推送消息：

```c#
using (var bus = RabbitHutch.CreateBus("host=localhost:5672;username=guest;password=guest"))
{
    var input = "";
    Console.WriteLine("Enter a message. 'Quit' to quit.");
    while ((input = Console.ReadLine()) != "Quit")
    {
        bus.Publish(new TextMessage
            {
                Text = input
            });
    }
}
```

然后新建一个消费端项目 `MQ.EasyNETQ.Customer`，继续在 `Program.cs` 建立与 RabbitMQ 的连接并开启订阅：

```c#
using (var bus = RabbitHutch.CreateBus("host=localhost:5672;username=guest;password=guest"))
{
    bus.PubSub.Subscribe<TextMessage>("test", HandleTextMessage);
}

static void HandleTextMessage(TextMessage textMessage)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine("Got message: {0}", textMessage.Text);
    Console.ResetColor();
}
```

运行发现没有问题。

需要注意一下，安装成功之后 RabbitMQ 自带消息重试，以及持久化的错误消息队列，以便后续的消息恢复。具体详见 RabbitMQ 的[官方文档](https://www.rabbitmq.com/admin-guide.html)。

ok，.NET 这块对 RabbitMQ 消息的调度管理初步成功。接下来我们尝试用 Go

## Go

Go 下的 RabbitMQ 组件我们用官方推荐的 [amqp](https://github.com/streadway/amqp) 库。同样我们新建一个生产者在 `src/producer` 文件夹下的 `producer.go` 下。

> 由于本身 go 的一些限制还有为了方便起见，我把两个项目放在同一个目录下以不同的文件夹命名来区分。

同样我们根据[资料](https://pkg.go.dev/github.com/streadway/amqp#hdr-Use_Case)以及官方示例 [demo](https://github.com/rabbitmq/rabbitmq-tutorials/tree/master/go) 很容易入门在 main 函数写下如下代码片段：

```go
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
```

如何申明交换机和队列以及绑定操作我这里就省略了，然后是发送消息函数

```go
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
			ContentType: "application/json",
			Body:        network.Bytes(),
		})
	log.Printf("[x] 发送消息 %s", body)
	return err
}
```

这里我用的高性能的序列化插件 [encoding/gob](https://golang.org/pkg/encoding/gob/)，这里就是我后面与 .NET 交互时候遇到的问题，后续在说明。

借来是消费端，代码路径在 `src/customer/customer.go`

```go
conn, err := amqp.Dial("amqp://guest:guest@localhost:5672")
... 省略
ch, err := conn.Channel()
declareQueue(ch)
// 消费队列信息
err = consumer(ch, queue)
failOnError(err, "接受消息失败")
```

消费消息代码如下：

```go
func consumer(ch *amqp.Channel, queue amqp.Queue) error {
	msgs, err := ch.Consume(queue.Name, "", true, false, false, false, nil)
	failOnError(err, "消费者注册失败")
	forever := make(chan bool)
	go func() {
		for d := range msgs {
			buf := bytes.NewBuffer(d.Body)
			dec := gob.NewDecoder(buf)
             var user = src.CreateUserMessage{}
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
```

运行项目发现也没有问题。

在使用两边各自的 RabbitMQ 客户端组件没有问题之后，我们开始考虑处理下一个核心问题：如何实现 Go 段服务发消息，应用端 .NET 如何消费。这理论上是很好解决的，因为 .Net 与 Golang 用的消息中间件都是 RabbitMQ，只要.Net 与 Golang 都实现了 RabbitMQ 的消息协议（比如 AMQP 协议）就能完成一方消息的推送，另一方消费的目的。

考虑这个问题并不是空穴来风，因为 Go 是用作处理底层平台 rpc 模块，除了底层平台级不同模块之间的通信外，各大应用端也要订阅平台的基础数据。

# Go 推送消息，Net 消费及其出现的问题

到这一步的时候，出现问题了，登录 [RabbitMQ 管理 UI](http://localhost:15672/#/) 发现 Go 有正常发出消息，queue 以及 exchange 都是对应上的，在 .NET 的订阅方式就如上面写的代码一样。在 queue 中的消息在重试一段时间之后如果还是失败，EasyNetQ 会将无法正常消费的消息转到错误队列中去。并且可以查看发生具体的错误消息，结果发现都是报 `ArgumentNullException:typeName is null` 类型错误。奇怪的是我断点调试也进不来断点，说明 EasyNetQ 在消费消息的时候压根没有运行这段订阅代码：

```c#
using (var bus = RabbitHutch.CreateBus("host=localhost:5672;username=guest;password=guest"))
{
    bus.PubSub.Subscribe<CreateUserMessage>("test", HandleCreateUserMessage);
}
static void HandleCreateUserMessage(CreateUserMessage message) {
	Logger.LogInformation($"接收消息：{JsonSerializer.Serialize(message)} 时间:{DateTimeOffset.Now}");
}
```

后来也去翻 EasyNetQ 源码，得知是因为还没到我写的这个订阅阶段的代码，而是在这段订阅代码 `IDisposable Consume(IQueue queue, MessageHandler onMessage, Action<IConsumerConfiguration> configure)`。这里面有个核心的参数就是 `onMessage`，从建立连接到消费具体队列的消息，这个参数是一直传递下去的。EasyNetQ 会根据初始化与 RabbitMQ 连接的参数来创建消费，比如建立队列时传递 `isExclusive = true` 就会创建一个瞬时消费者，只有当前连接能访问，并且关闭时会自动删除。EasyNetQ 默认会初始化一个持久化消费者 [PersistentConsumer](https://github.com/EasyNetQ/EasyNetQ/blob/9d50eabfad16f662a0d361411eaac13b2cdb9bcd/Source/EasyNetQ/Consumer/PersistentConsumer.cs)，然后触发内部消费者构造一个 [BasicConsumer](https://github.com/EasyNetQ/EasyNetQ/blob/9d50eabfad16f662a0d361411eaac13b2cdb9bcd/Source/EasyNetQ/Consumer/InternalConsumer.cs) 共给 [RabbitMQ.Client](https://github.com/rabbitmq/rabbitmq-dotnet-client) 调用触发方法 `HandleBasicDeliver`，由 RabbitMQ.Client 调用传递所需要的参数，而报的错误也是在这里，因为从 Go 发出的消息，.NET 接收无法解析到对应的元数据信息，所以获取的 [IBasicProperties](https://github.com/rabbitmq/rabbitmq-dotnet-client/blob/2a709aa76f25e8b6754cef4e4f482547d6c1d870/projects/RabbitMQ.Client/client/api/IBasicConsumer.cs) 对象是空的，由此触发了参数检查造成报错。

我们把消费端改成这样就能发现 `content` 能正常接收

```c#
bus.Advanced.Consume(queue, (body, properties, info) =>
{
    string content = Encoding.UTF8.GetString(body);
    var userMessage = System.Text.Json.JsonSerializer.Deserialize<CreateUserMessage>(body);
    Logger.LogInformation($"接收消息：{System.Text.Json.JsonSerializer.Serialize(userMessage)} 时间:{DateTimeOffset.Now}");
});
```

断点能进来了，就能继续往下进行了，随后就会又碰到序列化失败的问题，因为 content 接收的内容是乱码的，跨语言之间经常出现的问题就是编码，所以我把目光又瞄向了 Go，现在我们再来看下 Go 的发消息的那段代码：

```go
var network bytes.Buffer
gob.Register(src.CreateUserMessage{})
enc := gob.NewEncoder(&network)
err := enc.Encode(body)
...
err = ch.Publish(
    "",
    queue.Name,
    false,
    false,
    amqp.Publishing{
        ContentType: "application/json",
        Body:        network.Bytes(),
    })
...
```

## Go 编码库 `encoding/gob`

我首先在网上查资料发现 gob 这个库编码是用的 gbk 编码，实则不然，翻看源码就知道是用的 utf-8，并且也查明 gob 这个库是不能指定编码格式的。无论我是改 ContentType 的类型，在 .Net 消费端依旧无法正常接收。难道只能用 json 序列化传递消息？为了弄明白这个，我开始查阅这个 gob 库是否支持跨语言，也就是说 gob 这个库是否实现了外界公共协议。最后在官网博客下查到了，encoding/gob 只适用于 Go 语言环境，所以在性能方面非常突出。在这里我贴出博客中的一小段原话，引自 https://blog.golang.org/gob

> First, and most obvious, it had to be very easy to use. First, because Go has reflection, there is no need for a separate interface definition language or "protocol compiler". The data structure itself is all the package should need to figure out how to encode and decode it. On the other hand, this approach means that gobs will never work as well with other languages, but that's OK: gobs are unashamedly Go-centric.

既然不支持跨语言，那就心安理得的用 json 了，如果用不了 gob，想追求高性能的化，那么其实还可以用 protobuf 协议或是其它二进制协议来序列化，核心就是双方语言协议格式统一即可。现在的 publish 函数如下

```go
func publish(ch *amqp.Channel, queue amqp.Queue, body interface{}) error {
	buffer, err := json.Marshal(body)
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
```

这样 .NET 消费端就能成功接收消息了。

# 封装 EasyNetQ 与最佳实践

从前面的使用来看，我们把业务处理都放在 Program 明显是不合适的，这里应该只关心模块，与业务无关的。

幸好 EasyNetQ 考虑到了这点，提供了[自动订阅机制](https://github.com/EasyNetQ/EasyNetQ/wiki/Auto-Subscriber)。虽然官网只给出了 Windsor 的例子，但是也很容易就能做到类似下面的封装代码

```c#
// EasyNetRabbitMQICollectionExtensions.cs
public static RabbitMQEasyNetBuilder EasyNetRabbitMQBuilder(this IServiceCollection services, IConfiguration configuration)
{
    string username = configuration["RabbitMQ:UserName"];
    string password = configuration["RabbitMQ:Password"];
    var connectionString = (ConnectionString)$"host={configuration["RabbitMQ:Server"]},{configuration["RabbitMQ:Server"]}:5673;username={username};password={password}";
    // publisherConfirms = true 为开启推送消息确认,建议开启,性能刚高
    // 因为不加上则当 rabbitmq 不可用时,发送消息会系统错误,而开启发送确认则不会,更具有伸缩性
    connectionString.Append("publisherConfirms=true");

    var bus = RabbitHutch.CreateBus(connectionString);
    services.AddSingleton(bus);
    return new RabbitMQEasyNetBuilder(services);
}
```

然后开启自动订阅：

```c#
// RabbitMQEasyNetBuilder.cs
public void UseAutoSubscriber(string subscriptionIdPrefix)
{
    _services.AddSingleton<MessageDispatcher>();
    _services.AddSingleton<AutoSubscriber>(provider =>
    {
        var subscriber = new AutoSubscriber(provider.GetRequiredService<IBus>(), subscriptionIdPrefix)
        {
            AutoSubscriberMessageDispatcher = provider.GetRequiredService<MessageDispatcher>()
        };
        return subscriber;
    });
}
```

这里注入的 `MessageDispatcher` 类跟 `WindsorMessageDispatcher` 差不多，依葫芦画瓢。

最后在提供 Configure 触发自动订阅：

```c#
// IApplicationBuilderExtensions.cs
public static void UseAutoSubscriber(this IApplicationBuilder app,Assembly[] assemblies)
{
    var subscriber = app.ApplicationServices.GetService<AutoSubscriber>();
    subscriber.Subscribe(assemblies);
	...
}
```

这样我们就可以直接定义 `IConsumer<Message>` 的处理程序类即可，完全解耦了业务：

```c#
public class UserMessageHandler : IConsumeAsync<CreateUserMessage>
{
    private readonly ILoggerFactory _loggerFactory;
    public UserMessageHandler(ILoggerFactory loggerFactory)
    {
        _loggerFactory = loggerFactory;
    }
    public ILogger Logger => _loggerFactory.CreateLogger<UserMessageHandler>();
    [ForTopic(Consts.Topic.User)]
    public async Task ConsumeAsync(CreateUserMessage message, CancellationToken cancellationToken = default)
    {
        Logger.LogInformation($"接收消息：{JsonSerializer.Serialize(message)} 时间:{DateTimeOffset.Now}");
        //throw new NotSupportedException();
        await Task.Yield();
    }
}
```

还没结束，除了这种推送订阅方式，EasyNetQ 还提供了 Request/Response，RPC 模式。本质上还是通过 exchange 对 queue 进行消息调度。只是 EasyNetQ 内部做了很多工作，以至于让我们使用非常方便。那么针对这种模式也是可以做到完全解耦的，重点来了，这个是官网没有的姿势啊，且看下面代码

```c#
public interface IResponder
{
    void Subscribe();
}
public abstract class ResponderBase : IResponder
{
    private readonly IBus _bus;
    private ILogger _logger;

    public IBus Bus => _bus;

    public ILogger Logger
    {
        get { return _logger ??= NullLogger.Instance; }
        set { _logger = value; }
    }

    protected ResponderBase(IBus bus)
    {
        _bus = bus;
    }

    public abstract void Subscribe();
}
```

先建立一个规约 `IResponder`，并给一个基类实现。然后在拓展方法 `IApplicationBuilderExtensions.UseAutoSubscriber` 中加入如 AutoSubscriber 机制的代码即可，完整的方法如下：

```c#
public static void UseAutoSubscriber(this IApplicationBuilder app,Assembly[] assemblies)
{
    var subscriber = app.ApplicationServices.GetService<AutoSubscriber>();
    subscriber.Subscribe(assemblies);

    var requests = app.ApplicationServices.GetServices<IResponder>();
    foreach (var request in requests)
    {
        request.Subscribe();
    }

    var advancedSubscribers = app.ApplicationServices.GetServices<IAdvancedSubscriber>();
    foreach (var advanced in advancedSubscribers)
    {
        advanced.Subscribe();
    }
}
```

这样 Request/Response 与 [EasyNetQ 高级 API](https://github.com/EasyNetQ/EasyNetQ/wiki/The-Advanced-API) 都能与业务很好的解耦了。只需要定义各自的 MessageHandler 即可。

# 最后

总体来说虽然踩坑了（明确来说不是库的坑，而是对其不熟导致的），但是也如愿解决了问题点。在实施多语言交互时，一定要注意彼此之间的差异，要定义好规范协议，在解决基本的交互问题之后，就开始继续深入进行重构。虽然目前只是项目演示阶段，等项目真正执行下去肯定还会碰到更多问题，特别是 Go，才接触一星期，公司决定用 Go 作为底层核心 rpc 模块，我个人还是很担心的。

整个 mq 示例源码地址托管在 https://github.com/MS-Practice/mq

# 参考资料

- https://github.com/EasyNetQ/EasyNetQ/wiki
- https://github.com/rabbitmq/rabbitmq-tutorials/tree/master/go
- https://blog.golang.org/gob