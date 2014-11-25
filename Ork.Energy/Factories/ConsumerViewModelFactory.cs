﻿using System;
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
            return new ConsumerGroupViewModel(consumerGroup);
        }

        public ConsumerViewModel CreateFromExisting(Consumer consumer)
        {
            throw new NotImplementedException();
        }

        public DistributorViewModel CreateFromExisting(Distributor distributor)
        {
            throw new NotImplementedException();
        }

        public ConsumerGroupModifyViewModel CreateConsumerGroupModifyVM(ConsumerGroup consumerGroup)
        {
            return new ConsumerGroupModifyViewModel(consumerGroup);
        }

        public ConsumerModifyViewModel CreateConsumerModifyVM(Consumer consumer)
        {
            throw new NotImplementedException();
        }

        public DistributorModifyViewModel CreateDistributorModifyVM(Distributor distributor)
        {
            throw new NotImplementedException();
        }
    }
}