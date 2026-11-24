namespace attendance.Services;

public sealed class AttendanceUpdates(ILogger<AttendanceUpdates> logger)
{
    public delegate Task AttendanceSaved(
        string className,
        string date,
        IReadOnlyDictionary<string, (bool Present, bool Exempt)> marks);

    public event AttendanceSaved? Saved;

    public Task PublishAsync(
        string className,
        string date,
        IReadOnlyDictionary<string, (bool Present, bool Exempt)> marks)
    {
        var subscribers = Saved;
        if (subscribers == null) return Task.CompletedTask;

        return Task.WhenAll(
            subscribers.GetInvocationList().Cast<AttendanceSaved>().Select(NotifyAsync));

        async Task NotifyAsync(AttendanceSaved subscriber)
        {
            try
            {
                await subscriber(className, date, marks);
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Unable to update an attendance view.");
            }
        }
    }
}