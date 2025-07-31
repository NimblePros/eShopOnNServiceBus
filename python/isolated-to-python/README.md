# Python RabbitMQ Demo - Isolated to Python

For this demo, we have RabbitMQ set up via .NET Aspire:

```csharp

var rabbitUser = builder.AddParameter("rabbitUser", "rabbitUser");
var rabbitPassword = builder.AddParameter("rabbitPassword", "rabbitPassword");
var rabbitmq = builder.AddRabbitMQ("messaging", rabbitUser, rabbitPassword,5672)
.WithManagementPlugin();
```

The **python-rabbitmq-receive.py** can be used to receive test messages from a queue called `hello`.
- This script can be stopped with <kbd>Ctrl+C</kbd>.

The **python-rabbitmq-send.py** can be used to send test messages to a queue called `hello`. 
- Execute this in another Python prompt to show how you can send and the receive script will pick up what's sent as it's sent.
