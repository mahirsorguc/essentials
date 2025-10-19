using HMS.Essentials.MediatR;

namespace HMS.MainApp.Samples;

public class GetSamplesQueryHandler : QueryHandler<GetSamplesQuery, List<SampleDto>>
{
    private readonly ISampleRepository _sampleRepository;

    public GetSamplesQueryHandler(ISampleRepository sampleRepository)
    {
        _sampleRepository = sampleRepository;
    }

    public override async Task<List<SampleDto>> Handle(GetSamplesQuery request, CancellationToken cancellationToken)
    {
        var samples = await _sampleRepository.GetAllAsync(cancellationToken);
        return ObjectMapper.Map<List<Sample>, List<SampleDto>>(samples);
    }
}