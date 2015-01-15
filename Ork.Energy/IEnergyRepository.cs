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
// Copyright (c) 2014, HTW Berlin

#endregion

using System;
using System.Collections.Generic;
using System.Data.Services.Client;
using Ork.Energy.DomainModelService;

namespace Ork.Energy
{
  public interface IEnergyRepository

  {
    DataServiceCollection<ConsumerGroup> ConsumerGroups { get; }
    DataServiceCollection<Building> Buildings { get; }
    DataServiceCollection<Consumer> Consumers { get; }
    DataServiceCollection<Distributor> Distributors { get; }
    DataServiceCollection<Reading> Readings { get; }
    DataServiceCollection<ConsumerType> ConsumerTypes { get; }
    bool HasConnection { get; }
    void Save();
    event EventHandler ContextChanged;
    event EventHandler SaveCompleted;

    IEnumerable<EntityDescriptor> Entities { get; }
    IEnumerable<LinkDescriptor> Links { get; }
  }
}