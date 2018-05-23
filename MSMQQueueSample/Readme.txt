This is an example for communication using MSMQ queue

1. Make sure that the MSMQ services are enabled and are running. Goto programs and features-> Turn on windows features-> enable MSMQ Server and MSMQ Trigger.

1. When we send message, the default serializer used is xml serializer, you can specify custom serializer here and assign it to messageQueue.Formatter
2. Use the same logic to deserialize the message at receiver side by specifying the formatter on message as message.Formatter
3. set messageQueue.DefaultPropertiesToSend.Recoverable = true; to make the message durable. It will persist even after machine restart.
