namespace OwlPost.IoModels.DtoModels;

public record JoinRoomDto(Guid RoomId);

public record CreateRoomDto(string Name, string Description);

public record EditRoomDto(Guid RoomId, string Name, string Description);

public record DeleteRoomDto(Guid RoomId);

