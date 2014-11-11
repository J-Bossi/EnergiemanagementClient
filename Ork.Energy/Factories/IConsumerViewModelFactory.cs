using Ork.Energy.DomainModelService;
using Ork.Energy.ViewModels;

namespace Ork.Energy.Factories
{
    public interface IConsumerViewModelFactory
    {
        ConsumerGroupViewModel CreateFromExisting(ConsumerGroup consumerGroup);
    }
}
