using HMS.Essentials.MediatR;
using HMS.Essentials.ObjectMapping;

namespace HMS.MainApp.Samples;

public class CreateSampleCommandHandler : ICommandHandler<CreateSampleCommand, SampleDto>
{
    private readonly IObjectMapper _objectMapper;
    private readonly ISampleRepository _sampleRepository;

    public CreateSampleCommandHandler(ISampleRepository sampleRepository, IObjectMapper objectMapper)
    {
        _sampleRepository = sampleRepository;
        _objectMapper = objectMapper;
    }

    public async Task<SampleDto> Handle(CreateSampleCommand request, CancellationToken cancellationToken)
    {
        var dto = request.CreateSampleDto;
        var insertedSample =
            await _sampleRepository.InsertAsync(new Sample(Guid.NewGuid(), dto.Name, dto.Description, dto.IsActive));
        return _objectMapper.Map<Sample, SampleDto>(insertedSample);
    }
}