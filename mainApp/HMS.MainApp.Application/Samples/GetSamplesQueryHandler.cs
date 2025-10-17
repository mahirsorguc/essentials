using HMS.Essentials.MediatR;
using HMS.Essentials.ObjectMapping;

namespace HMS.MainApp.Samples;

public class GetSamplesQueryHandler : IQueryHandler<GetSamplesQuery, List<SampleDto>>
{
    private readonly IObjectMapper _objectMapper;
    private readonly ISampleRepository _sampleRepository;

    public GetSamplesQueryHandler(ISampleRepository sampleRepository, IObjectMapper objectMapper)
    {
        _sampleRepository = sampleRepository;
        _objectMapper = objectMapper;
    }

    public async Task<List<SampleDto>> Handle(GetSamplesQuery request, CancellationToken cancellationToken)
    {
        var samples = await _sampleRepository.GetAllAsync(cancellationToken);
        return _objectMapper.Map<List<Sample>, List<SampleDto>>(samples);
    }
}