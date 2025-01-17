﻿namespace Rabbitmq.App.Factories;

public interface IRabbitMqConnectionFactory
{
    void PostMessage(MessageInputModel message);
    void ConsumeMessages();
}