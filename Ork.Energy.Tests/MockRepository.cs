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

using System;
using System.Collections.Generic;
using System.Data.Services.Client;
using Ork.Energy.Domain.DomainModelService;

namespace Ork.Energy.Tests
{
  public class MockRepository : IEnergyRepository
  {
    public DataServiceCollection<ConsumerGroup> ConsumerGroups
    {
      get { throw new NotImplementedException(); }
    }

    public DataServiceCollection<Consumer> Consumers
    {
      get { throw new NotImplementedException(); }
    }

    public DataServiceCollection<Distributor> Distributors
    {
      get { throw new NotImplementedException(); }
    }

    public DataServiceCollection<Reading> Readings
    {
      get { throw new NotImplementedException(); }
    }

    public DataServiceCollection<ConsumerType> ConsumerTypes
    {
      get { throw new NotImplementedException(); }
    }

    public DataServiceCollection<ResponsibleSubject> ResponsibleSubjects
    {
      get { throw new NotImplementedException(); }
    }

    public DataServiceCollection<EnergyMeasure> Measures
    {
      get { throw new NotImplementedException(); }
    }

    public DataServiceCollection<SubMeasure> SubMeasures
    {
      get { throw new NotImplementedException(); }
    }

    public bool HasConnection
    {
      get { throw new NotImplementedException(); }
    }

    public IEnumerable<EntityDescriptor> Entities
    {
      get { throw new NotImplementedException(); }
    }

    public IEnumerable<LinkDescriptor> Links
    {
      get { throw new NotImplementedException(); }
    }

    public void Save()
    {
      throw new NotImplementedException();
    }

    public event EventHandler ContextChanged;
    public event EventHandler SaveCompleted;
  }
}