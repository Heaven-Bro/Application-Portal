namespace Application.Features.ChairmanAvailability.Queries;

using MediatR;
using Domain.Repositories;

public record GetCurrentWeekChairmanAvailabilityQuery() : IRequest<List<ChairmanAvailabilityDto>>;

public class GetCurrentWeekChairmanAvailabilityQueryHandler : IRequestHandler<GetCurrentWeekChairmanAvailabilityQuery, List<ChairmanAvailabilityDto>>
{
    private readonly IChairmanAvailabilityRepository _repository;

    public GetCurrentWeekChairmanAvailabilityQueryHandler(IChairmanAvailabilityRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<ChairmanAvailabilityDto>> Handle(GetCurrentWeekChairmanAvailabilityQuery request, CancellationToken cancellationToken)
    {
        var (weekStart, weekEnd) = WeekRange(DateTime.UtcNow);
        var slots = await _repository.GetBetweenAsync(weekStart, weekEnd, cancellationToken);

        return slots
            .OrderBy(s => s.StartTime)
            .Select(s => new ChairmanAvailabilityDto(s.Id, s.StartTime, s.EndTime))
            .ToList();
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

public record ChairmanAvailabilityDto(long Id, DateTime StartTime, DateTime EndTime);
