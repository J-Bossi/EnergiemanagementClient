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
using System.Linq;
using Caliburn.Micro;
using Ork.Energy.Domain.DomainModelService;
using Ork.Energy.Factories;

namespace Ork.Energy.ViewModels
{
  public class ConsumerModifyViewModel : Screen
  {
    private readonly IEnergyViewModelFactory m_EnergyViewModelFactory;
    private readonly Consumer m_Model;
    private readonly IEnergyRepository m_Repository;

    public ConsumerModifyViewModel(Consumer consumer, [Import] IEnergyRepository consumerRepository,
                                   [Import] IEnergyViewModelFactory energyViewModelFactory)
    {
      DisplayName = "Verbraucher bearbeiten...";
      m_Model = consumer;
      m_Repository = consumerRepository;
      m_EnergyViewModelFactory = energyViewModelFactory;
      ReadingAddVm = new ReadingAddViewModel();
    }

    public Consumer Model
    {
      get { return m_Model; }
    }

    public Room Room
    {
      get { return m_Model.Room; }
      set { m_Model.Room = value; }
    }

    public IEnumerable<Room> Rooms
    {
      get { return m_Repository.Rooms; }
    }

    public ReadingAddViewModel ReadingAddVm { get; set; }

    public virtual Distributor Distributor
    {
      get { return m_Model.Distributor; }
      set { m_Model.Distributor = value; }
    }

    public virtual ConsumerGroup ConsumerGroup
    {
      get { return m_Model.ConsumerGroup; }
      set
      {
        m_Model.ConsumerGroup = value;
        NotifyOfPropertyChange(() => AllConsumerTypes);
      }
    }

    public string Name
    {
      get { return m_Model.Name; }
      set
      {
        m_Model.Name = value;
        NotifyOfPropertyChange(() => Name);
      }
    }

    public virtual long? PowerOutput
    {
      get { return m_Model.PowerOutput; }
      set { m_Model.PowerOutput = value; }
    }

    public virtual long? PowerCurrent
    {
      get { return m_Model.PowerCurrent; }
      set { m_Model.PowerCurrent = value; }
    }

    public virtual IList<ReadingViewModel> Readings
    {
      get
      {
        return m_Model.Readings.Select(rvm => m_EnergyViewModelFactory.CreateFromExisting(rvm))
                      .ToList();
      }
    }

    public int? Year
    {
      get { return m_Model.Year; }
      set { m_Model.Year = value; }
    }

    public virtual string Manufacturer
    {
      get { return m_Model.Manufacturer; }
      set { m_Model.Manufacturer = value; }
    }

    public IEnumerable<Distributor> AllDistributors
    {
      get { return m_Repository.Distributors; }
    }

    public IEnumerable<ConsumerGroup> AllConsumerGroups
    {
      get { return m_Repository.ConsumerGroups; }
    }

    public IEnumerable<ConsumerType> AllConsumerTypes
    {
      get { return m_Model.ConsumerGroup.ConsumerTypes; }
    }

    public ConsumerType ConsumerType
    {
      get { return m_Model.ConsumerType; }
      set { m_Model.ConsumerType = value; }
    }

    public bool IsMachine
    {
      get { return m_Model.IsMachine; }
      set { m_Model.IsMachine = value; }
    }

    public string Identifier
    {
      get { return m_Model.Identifier; }
      set { m_Model.Identifier = value; }
    }

    public void AddNewReading(object dataContext)
    {
      m_Model.Readings.Add(ModelFactory.CreateReading(ReadingAddVm.NewReadingDate, ReadingAddVm.NewCounterReading,
        ReadingAddVm.NewMeasuringDevice));
      ReadingAddVm.ClearReadingFields();
      NotifyOfPropertyChange(() => Readings);
    }

    public void DeleteReading(object dataContext)
    {
      m_Model.Readings.Remove(((ReadingViewModel) dataContext).Model);

      NotifyOfPropertyChange(() => Readings);
    }
  }
}