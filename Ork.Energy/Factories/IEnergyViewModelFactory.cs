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

using Ork.Energy.DomainModelService;
using Ork.Energy.ViewModels;

namespace Ork.Energy.Factories
{
  public interface IEnergyViewModelFactory
  {
    ConsumerGroupViewModel CreateFromExisting(ConsumerGroup consumerGroup);
    ConsumerViewModel CreateFromExisting(Consumer consumer);
    DistributorViewModel CreateFromExisting(Distributor distributor);
    ReadingViewModel CreateFromExisting(Reading reading);
    ConsumerTypeViewModel CreateFromExisting(ConsumerType consumerType);
    ConsumerGroupModifyViewModel CreateConsumerGroupModifyVM(ConsumerGroup consumerGroup);
    ConsumerModifyViewModel CreateConsumerModifyVM(Consumer consumer);
    DistributorModifyViewModel CreateDistributorModifyVM(Distributor distributor);
    ReadingAddViewModel CreateReadingModifyViewModel();
  }
}