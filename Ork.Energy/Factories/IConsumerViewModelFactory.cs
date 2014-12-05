using Ork.Energy.DomainModelService;
using Ork.Energy.ViewModels;

namespace Ork.Energy.Factories
{
    public interface IConsumerViewModelFactory
    {
        ConsumerGroupViewModel CreateFromExisting(ConsumerGroup consumerGroup);
        ConsumerViewModel CreateFromExisting(Consumer consumer);
        DistributorViewModel CreateFromExisting(Distributor distributor);

        ConsumerGroupModifyViewModel CreateConsumerGroupModifyVM(ConsumerGroup consumerGroup);
        ConsumerModifyViewModel CreateConsumerModifyVM(Consumer consumer);
        DistributorModifyViewModel CreateDistributorModifyVM(Distributor distributor);


    }
}
