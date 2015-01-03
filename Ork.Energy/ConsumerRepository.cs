﻿#region License

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
using System.ComponentModel.Composition;
using System.Data.Services.Client;
using Ork.Energy.DomainModelService;
using Ork.Setting;

namespace Ork.Energy
{
  [Export(typeof (IConsumerRepository))]
  internal class ConsumerRepository : IConsumerRepository
  {
    private DomainModelContext m_Context;
    private readonly Func<DomainModelContext> m_CreateMethod;

    [ImportingConstructor]
    public ConsumerRepository([Import] ISettingsProvider settingsContainer, [Import] Func<DomainModelContext> createMethod)
    {
      m_CreateMethod = createMethod;
      settingsContainer.ConnectionStringUpdated += (s, e) => Initialize();
      Initialize();
    }

    public DataServiceCollection<ConsumerGroup> ConsumerGroups { get; private set; }
    public DataServiceCollection<Building> Buildings { get; private set; }
    public DataServiceCollection<Distributor> Distributors { get; private set; }
    public DataServiceCollection<Reading> Readings { get; private set; }
    public DataServiceCollection<Consumer> Consumers { get; private set; }
    public DataServiceCollection<ConsumerType> ConsumerTypes { get; private set; }
    public bool HasConnection { get; private set; }

    public void Save()
    {
      if (m_Context.ApplyingChanges)
      {
        return;
      }

      IAsyncResult result = m_Context.BeginSaveChanges(SaveChangesOptions.Batch, c =>
                                                                                 {
                                                                                   var dmc = (DomainModelContext) c.AsyncState;
                                                                                   dmc.EndSaveChanges(c);
                                                                                   RaiseEvent(SaveCompleted);
                                                                                 }, m_Context);
    }

    public event EventHandler ContextChanged;
    public event EventHandler SaveCompleted;

    private void Initialize()
    {
      m_Context = m_CreateMethod();

      try
      {
        LoadConsumerGroups();
        LoadBuildings();
        LoadConsumers();
        LoadDistributors();
        LoadReadings();
        LoadConsumerTypes();
        HasConnection = true;
      }
      catch (Exception ex)
      {
        HasConnection = false;
        string message = ex.Message;
        message += Environment.NewLine + ex.InnerException.Message;
      }
      RaiseEvent(ContextChanged);
    }

    private void LoadConsumerGroups()
    {
      ConsumerGroups = new DataServiceCollection<ConsumerGroup>(m_Context);

      DataServiceQuery<ConsumerGroup> query = m_Context.ConsumerGroups.Expand("ConsumerTypes");
      ConsumerGroups.Load(query);
    }

    private void LoadConsumers()
    {
      Consumers = new DataServiceCollection<Consumer>(m_Context);

      DataServiceQuery<Consumer> query = m_Context.Consumers.Expand("OpenResKit.DomainModel.Consumer/ConsumerGroup")
                                                  .Expand("OpenResKit.DomainModel.Consumer/Distributor")
                                                  .Expand("OpenResKit.DomainModel.Consumer/ConsumerType");
      Consumers.Load(query);
    }

    private void LoadConsumerTypes()
    {
      ConsumerTypes = new DataServiceCollection<ConsumerType>(m_Context);

      DataServiceQuery<ConsumerType> query = m_Context.ConsumerTypes;
      ConsumerTypes.Load(query);
    }

    private void LoadReadings()
    {
      Readings = new DataServiceCollection<Reading>(m_Context);

      DataServiceQuery<Reading> query = m_Context.Readings;
      Readings.Load(query);
    }

    private void LoadDistributors()
    {
      Distributors = new DataServiceCollection<Distributor>(m_Context);

      DataServiceQuery<Distributor> query = m_Context.Distributors;
      Distributors.Load(query);
    }

    private void LoadBuildings()
    {
      Buildings = new DataServiceCollection<Building>(m_Context);

      DataServiceQuery<Building> query = m_Context.Buildings.Expand("OpenResKit.DomainModel.Rooms");
    }

    private void RaiseEvent(EventHandler eventHandler)
    {
      if (eventHandler != null)
      {
        eventHandler(this, new EventArgs());
      }
    }
  }
}