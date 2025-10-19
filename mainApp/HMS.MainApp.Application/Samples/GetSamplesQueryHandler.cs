using HMS.Essentials.MediatR;
using HMS.Essentials.ObjectMapping;
using HMS.Essentials.SequentialGuid;

namespace HMS.MainApp.Samples;

public class GetSamplesQueryHandler : QueryHandler<GetSamplesQuery, List<SampleDto>>
{
    private readonly IObjectMapper _objectMapper;
    private readonly ISampleRepository _sampleRepository;

    public GetSamplesQueryHandler(ISampleRepository sampleRepository, IObjectMapper objectMapper,
        ISequentialGuidGenerator sequentialGuidGenerator) : base(sequentialGuidGenerator)
    {
        _sampleRepository = sampleRepository;
        _objectMapper = objectMapper;
    }

    public override async Task<List<SampleDto>> Handle(GetSamplesQuery request, CancellationToken cancellationToken)
    {
        var samples = await _sampleRepository.GetAllAsync(cancellationToken);
        return _objectMapper.Map<List<Sample>, List<SampleDto>>(samples);
    }
}