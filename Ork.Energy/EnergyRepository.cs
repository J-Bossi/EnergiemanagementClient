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
using System.ComponentModel.Composition;
using System.Data.Services.Client;
using System.Linq;
using Ork.Energy.Domain.DomainModelService;
using Ork.Setting;

namespace Ork.Energy
{
  [Export(typeof (IEnergyRepository))]
  public class EnergyRepository : IEnergyRepository
  {
    private readonly Func<DomainModelContext> m_CreateMethod;
    private DomainModelContext m_Context;

    [ImportingConstructor]
    public EnergyRepository([Import] ISettingsProvider settingsContainer, [Import] Func<DomainModelContext> createMethod)
    {
      m_CreateMethod = createMethod;
      settingsContainer.ConnectionStringUpdated += (s, e) => Initialize();
      Initialize();
    }

    public DataServiceCollection<ConsumerGroup> ConsumerGroups { get; private set; }
    public DataServiceCollection<Distributor> Distributors { get; private set; }
    public DataServiceCollection<Reading> Readings { get; private set; }
    public DataServiceCollection<Consumer> Consumers { get; private set; }
    public DataServiceCollection<ConsumerType> ConsumerTypes { get; private set; }
    public DataServiceCollection<ResponsibleSubject> ResponsibleSubjects { get; private set; }
    public DataServiceCollection<EnergyMeasure> Measures { get; private set; }
    public DataServiceCollection<SubMeasure> SubMeasures { get; private set; }
    public DataServiceCollection<Room> Rooms { get; private set; } 
    public bool HasConnection { get; private set; }

    public void Save()
    {
      if (m_Context.ApplyingChanges)
      {
        return;
      }

      var result = m_Context.BeginSaveChanges(SaveChangesOptions.Batch, c =>
                                                                        {
                                                                          var dmc = (DomainModelContext) c.AsyncState;
                                                                          dmc.EndSaveChanges(c);
                                                                          RaiseEvent(SaveCompleted);
                                                                        }, m_Context);
    }

    public event EventHandler ContextChanged;
    public event EventHandler SaveCompleted;

    public IEnumerable<EntityDescriptor> Entities
    {
      get { return m_Context.Entities; }
    }

    public IEnumerable<LinkDescriptor> Links
    {
      get { return m_Context.Links; }
    }

    private void Initialize()
    {
      m_Context = m_CreateMethod();

      try
      {
        LoadConsumerGroups();
        LoadConsumers();
        LoadDistributors();
        LoadReadings();
        LoadConsumerTypes();
        LoadMeasures();
        LoadSubMeasures();
        LoadResponsibleSubjects();
        LoadRooms();
        HasConnection = true;
      }
      catch (Exception)
      {
        HasConnection = false;
      }
      RaiseEvent(ContextChanged);
    }

    private void LoadConsumerGroups()
    {
      ConsumerGroups = new DataServiceCollection<ConsumerGroup>(m_Context);

      var query = m_Context.ConsumerGroups.Expand("ConsumerTypes");
      ConsumerGroups.Load(query);
    }

    private void LoadConsumers()
    {
      Consumers = new DataServiceCollection<Consumer>(m_Context);

      var query = m_Context.Consumers.Expand("OpenResKit.DomainModel.Consumer/ConsumerGroup")
                           .Expand("OpenResKit.DomainModel.Consumer/Distributor")
                           .Expand("OpenResKit.DomainModel.Consumer/ConsumerType")
                           .Expand("Readings");
      Consumers.Load(query);
    }

    private void LoadRooms()
    {
      Rooms = new DataServiceCollection<Room>(m_Context);
      var query = m_Context.Rooms.Expand("RoomInformation");
      Rooms.Load(query);
    }

    private void LoadMeasures()
    {
      Measures = new DataServiceCollection<EnergyMeasure>(m_Context);
      var query = m_Context.Measures.Expand("ResponsibleSubject")
                           .Expand("AttachedDocuments/DocumentSource")
                           .Expand("MeasureImageSource")
                           .Expand("Room")
                           .Expand("Consumer")
                           .OfType<EnergyMeasure>();
      Measures.Load(query);
    }

    private void LoadSubMeasures()
    {
      SubMeasures = new DataServiceCollection<SubMeasure>(m_Context);

      var query = m_Context.SubMeasures.Expand("OpenResKit.DomainModel.SubMeasure/ReleatedMeasure")
                           .Expand("OpenResKit.DomainModel.SubMeasure/ResponsibleSubject");
      SubMeasures.Load(query);
    }

    private void LoadResponsibleSubjects()
    {
      ResponsibleSubjects = new DataServiceCollection<ResponsibleSubject>(m_Context);

      var query = m_Context.ResponsibleSubjects.Expand("OpenResKit.DomainModel.Employee/Groups");
      ResponsibleSubjects.Load(query);
    }

    private void LoadConsumerTypes()
    {
      ConsumerTypes = new DataServiceCollection<ConsumerType>(m_Context);

      var query = m_Context.ConsumerTypes;
      ConsumerTypes.Load(query);
    }

    private void LoadReadings()
    {
      Readings = new DataServiceCollection<Reading>(m_Context);

      var query = m_Context.Readings;
      Readings.Load(query);
    }

    private void LoadDistributors()
    {
      Distributors = new DataServiceCollection<Distributor>(m_Context);

      var query = m_Context.Distributors.Expand("Readings");
      Distributors.Load(query);
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