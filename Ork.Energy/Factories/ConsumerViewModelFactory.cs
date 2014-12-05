using System;
using System.ComponentModel.Composition;
using Ork.Energy.DomainModelService;
using Ork.Energy.ViewModels;

namespace Ork.Energy.Factories
{
    [Export(typeof (IConsumerViewModelFactory))]
    internal class ConsumerViewModelFactory : IConsumerViewModelFactory
    {
        private readonly IConsumerRepository m_ConsumerRepository;

        [ImportingConstructor]
        public ConsumerViewModelFactory([Import] IConsumerRepository mConsumerRepository)
        {
            m_ConsumerRepository = mConsumerRepository;
        }

        public ConsumerGroupViewModel CreateFromExisting(ConsumerGroup consumerGroup)
        {
            return new ConsumerGroupViewModel(consumerGroup,m_ConsumerRepository);
        }

        public ConsumerViewModel CreateFromExisting(Consumer consumer)
        {
            return new ConsumerViewModel(consumer);
        }

        public DistributorViewModel CreateFromExisting(Distributor distributor)
        {
            return new DistributorViewModel(distributor, m_ConsumerRepository);
        }

        public ConsumerGroupModifyViewModel CreateConsumerGroupModifyVM(ConsumerGroup consumerGroup)
        {
            return new ConsumerGroupModifyViewModel(consumerGroup);
        }

        public ConsumerModifyViewModel CreateConsumerModifyVM(Consumer consumer)
        {
            return new ConsumerModifyViewModel(consumer, m_ConsumerRepository);
        }

        public DistributorModifyViewModel CreateDistributorModifyVM(Distributor distributor)
        {
            return new DistributorModifyViewModel(distributor);
        }
    }
}