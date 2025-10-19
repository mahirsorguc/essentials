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

    public async Task<SampleDto> Handle(CreateSampleCommand createSampleCommand, CancellationToken cancellationToken)
    {
        var insertedSample =
            await _sampleRepository.InsertAsync(
                new Sample(Guid.NewGuid(), createSampleCommand.Name, createSampleCommand.Description,
                    createSampleCommand.IsActive), cancellationToken: cancellationToken);

        return _objectMapper.Map<Sample, SampleDto>(insertedSample);
    }
}