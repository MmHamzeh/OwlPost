namespace OwlPost.IoModels.DtoModels;

public record SendMessageDto(Guid RoomId, string Content, Guid? ParentId);