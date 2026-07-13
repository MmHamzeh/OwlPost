namespace OwlPost.Sql.Repositories;

public class ChatRoomRepository(OwlPostDbContext context) : IChatRoomRepository
{
    public async Task<long?> DoesUserHaveAccessToRoom(Guid roomId, Guid userId, CancellationToken ct)
    {
        return await context.ChatRooms.AsNoTracking()
            .Where(e => e.PublicId == roomId &&
                        e.IsArchived == false &&
                                      e.ChatRoomUsers.Any(cru => cru.UserId == userId))
            .Select(e => e.Id)
            .FirstOrDefaultAsync(ct);
    }

}