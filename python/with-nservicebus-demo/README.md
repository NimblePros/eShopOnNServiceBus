# Python RabbitMQ working with NServiceBus

In this demo, we are using NServiceBus with the [RabbitMQ Transport](https://docs.particular.net/transports/rabbitmq/).

For this demo, we have RabbitMQ set up via .NET Aspire:

```csharp

var rabbitUser = builder.AddParameter("rabbitUser", "rabbitUser");
var rabbitPassword = builder.AddParameter("rabbitPassword", "rabbitPassword");
var rabbitmq = builder.AddRabbitMQ("messaging", rabbitUser, rabbitPassword,5672)
.WithManagementPlugin();
```