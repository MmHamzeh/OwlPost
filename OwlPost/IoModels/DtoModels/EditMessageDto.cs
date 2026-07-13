namespace OwlPost.IoModels.DtoModels;

public record EditMessageDto(Guid RoomId, Guid MessageId, string Content);