using HMS.Essentials.MediatR;
using HMS.Essentials.ObjectMapping;
using HMS.Essentials.SequentialGuid;

namespace HMS.MainApp.Samples;

public class CreateSampleCommandHandler : CommandHandler<CreateSampleCommand, SampleDto>
// public class CreateSampleCommandHandler : ICommandHandler<CreateSampleCommand, SampleDto>
{
    private readonly IObjectMapper _objectMapper;
    private readonly ISampleRepository _sampleRepository;

    public CreateSampleCommandHandler(ISampleRepository sampleRepository, IObjectMapper objectMapper,
        ISequentialGuidGenerator guidGenerator) : base(guidGenerator)
    {
        _sampleRepository = sampleRepository;
        _objectMapper = objectMapper;
    }

    public override async Task<SampleDto> Handle(CreateSampleCommand createSampleCommand, CancellationToken cancellationToken)
    {
        var insertedSample =
            await _sampleRepository.InsertAsync(
                new Sample(SequentialGuidGenerator.Create(), createSampleCommand.Name, createSampleCommand.Description,
                    createSampleCommand.IsActive), cancellationToken: cancellationToken);

        return _objectMapper.Map<Sample, SampleDto>(insertedSample);
    }
}