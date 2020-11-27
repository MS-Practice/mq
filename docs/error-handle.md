# EasyNetQ 对消息的错误处理

## 消费者宕机

生产者成功发送消息到 rabbitmq 的 broken，如果消费端在接收这个消息并处理时，消费端服务发送故障宕机，则 rabbitmq 不会删除这个消息，直到消费端发送一个 ack 才会删除。并且在生产端成功推送消息之后以及消费端消费消息之前就挂掉时，这时 EasyNetQ 会将这些消息存到**内存订阅队列**中去，这样在消费端重新启动时就能迅速恢复消息并继续处理。

# 消费者处理消息的速度比生产者慢

EasyNetQ 默认设置 rabbitmq 的参数 `prefectch-count` 为 50。这也就说明在订阅者队列中最多也就 50 个消息。这样就预防了订阅消息程序发生内存超出的异常。一旦未经确认的消息数量达到了 `prefetch-count` 设定的值，rabbitmq 就会停止发送消息。

## 订阅者与 RabbitMQ 的 broker 之间出现了网络故障

这是在连接 rabbitmq 阶段的问题，EasyNetQ 实现了“懒连接”策略。他假定了 broker 并不总是可用的。当你第一次使用 `RabbitHutch.CreateBus` 连接 broker 时，EasyNetQ 会进入循环连接阶段，如果 broker 在你连接的地址下不可用时，你将会看到 `Tring to Connect` 提示信息。甚至订阅者可以在 broker 不可用的时候使用 `bus.Subscribe` 订阅。订阅器细节被 EasyNetQ 缓存起来了。当 broker 可用时，就会因循环连接而成功，与 broker 的连接就建立起来了，以及所有前面缓存的订阅都会被创建。

这与 EasyNetQ 失去与 broker 的连接一样，它会返回一个循环连接处理并显示提示消息 `Trying to Connect`。一旦重新建立连接，就会再次创建缓存的订阅者。这样做的结果是，您可以让您的订阅者在网络连接不可靠或需要在弹性 RabbitMQ 代理的环境中运行。

## 在订阅者回调处理消息的时候抛错

当你订阅者回调报错时，EasyNetQ 会将这个正在消费的消息包装到一个指定错误的消息内。这个错误消息将被 EasyNetQ 推送到指定的错误队列中（名为 `EasyNetQ_Default_Error_Queue`）。你应该监控这个队列里的错误的消息。错误消息包括重新发布原始消息所需的所有信息以及异常的类型，信息和堆栈信息。你可以使用 `EasyNetQ.Hosepipe` 工具来重新发布错误消息。