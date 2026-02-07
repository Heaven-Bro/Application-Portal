namespace Application.Features.ChairmanAvailability.Commands;

using MediatR;
using Application.Common;
using Application.Common.Interfaces;
using Domain.Repositories;
using Domain.Settings;

public record UpdateCurrentWeekChairmanAvailabilityCommand(
    List<ChairmanAvailabilityInput> Slots
) : IRequest<Result>;

public record ChairmanAvailabilityInput(DateTime StartTime, DateTime EndTime);

public class UpdateCurrentWeekChairmanAvailabilityCommandHandler : IRequestHandler<UpdateCurrentWeekChairmanAvailabilityCommand, Result>
{
    private readonly IChairmanAvailabilityRepository _repository;
    private readonly ICurrentUserService _currentUserService;

    public UpdateCurrentWeekChairmanAvailabilityCommandHandler(
        IChairmanAvailabilityRepository repository,
        ICurrentUserService currentUserService)
    {
        _repository = repository;
        _currentUserService = currentUserService;
    }

    public async Task<Result> Handle(UpdateCurrentWeekChairmanAvailabilityCommand request, CancellationToken cancellationToken)
    {
        var (weekStart, weekEnd) = WeekRange(DateTime.UtcNow);

        var ordered = request.Slots
            .OrderBy(s => s.StartTime)
            .ToList();

        if (ordered.Any(s => s.EndTime <= s.StartTime))
            return Result.Failure("End time must be after start time");

        if (ordered.Any(s => s.StartTime < weekStart || s.EndTime > weekEnd))
            return Result.Failure("All slots must be within the current week");

        for (var i = 1; i < ordered.Count; i++)
        {
            if (ordered[i].StartTime < ordered[i - 1].EndTime)
                return Result.Failure("Slots cannot overlap");
        }

        var existing = await _repository.GetBetweenAsync(weekStart, weekEnd, cancellationToken);
        if (existing.Count > 0)
        {
            _repository.RemoveRange(existing);
        }

        if (ordered.Count > 0)
        {
            var currentUserId = _currentUserService.UserId;
            var entities = ordered
                .Select(s => ChairmanAvailabilitySlot.Create(s.StartTime, s.EndTime, currentUserId))
                .ToList();

            await _repository.AddRangeAsync(entities, cancellationToken);
        }

        await _repository.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    private static (DateTime Start, DateTime End) WeekRange(DateTime dateUtc)
    {
        var start = dateUtc.Date;
        while (start.DayOfWeek != DayOfWeek.Monday)
        {
            start = start.AddDays(-1);
        }

        var end = start.AddDays(7);
        return (start, end);
    }
}
