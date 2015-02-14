#region License

// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at
//  
// http://www.apache.org/licenses/LICENSE-2.0.html
//  
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  
// Copyright (c) 2015, HTW Berlin

#endregion

using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Data.Services.Client;
using System.Linq;
using Ork.Energy.Domain.DomainModelService;
using Ork.Energy.ViewModels;

namespace Ork.Energy.Factories
{
  [Export(typeof (IEnergyViewModelFactory))]
  internal class EnergyViewModelFactory : IEnergyViewModelFactory
  {
    private readonly IEnergyRepository m_ConsumerRepository;

    [ImportingConstructor]
    public EnergyViewModelFactory([Import] IEnergyRepository mConsumerRepository)
    {
      m_ConsumerRepository = mConsumerRepository;
    }

    public ConsumerGroupViewModel CreateFromExisting(ConsumerGroup consumerGroup)
    {
      return new ConsumerGroupViewModel(consumerGroup, m_ConsumerRepository);
    }

    public ConsumerViewModel CreateFromExisting(Consumer consumer)
    {
      return new ConsumerViewModel(consumer, m_ConsumerRepository);
    }

    public DistributorViewModel CreateFromExisting(Distributor distributor)
    {
      return new DistributorViewModel(distributor, m_ConsumerRepository);
    }

    public ReadingViewModel CreateFromExisting(Reading reading)
    {
      return new ReadingViewModel(reading);
    }

    public ConsumerTypeViewModel CreateFromExisting(ConsumerType consumerType)
    {
      return new ConsumerTypeViewModel(consumerType);
    }

    public ConsumerGroupModifyViewModel CreateConsumerGroupModifyVM(ConsumerGroup consumerGroup)
    {
      return new ConsumerGroupModifyViewModel(consumerGroup, m_ConsumerRepository, this);
    }

    public ConsumerModifyViewModel CreateConsumerModifyVM(Consumer consumer)
    {
      return new ConsumerModifyViewModel(consumer, m_ConsumerRepository, this);
    }

    public DistributorModifyViewModel CreateDistributorModifyVM(Distributor distributor)
    {
      return new DistributorModifyViewModel(distributor, m_ConsumerRepository, this);
    }

    public ReadingAddViewModel CreateReadingModifyViewModel()
    {
      return new ReadingAddViewModel();
    }

    public static IEnumerable<ReadingViewModel> CreateReadingsViewModels(DataServiceCollection<Reading> readings)
    {
      return readings.Select(reading => new ReadingViewModel(reading))
                     .ToList();
    }
  }
}