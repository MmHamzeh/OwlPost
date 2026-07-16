// ReSharper disable InconsistentNaming
namespace OwlPost.RabbitMq.Topology;

internal static class ConstData
{

    internal const string ChatExchangeName = "chat.exchange";
    internal const string ChatQueueName = "chat.queue";
    internal const string ChatRoutingKey = "chat.queue";

    internal const string NotificationExchangeName = "notification.exchange";
    internal const string NotificationQueueName = "notification.queue";
    internal const string NotificationRoutingKey = "notification.queue";

    internal const string RoomExchangeName = "room.exchange";
    internal const string RoomQueueName = "room.queue";
    internal const string RoomRoutingKey = "room.queue";

    internal const string BasicPropertiesHeaders_MessageType = "message-type";
}