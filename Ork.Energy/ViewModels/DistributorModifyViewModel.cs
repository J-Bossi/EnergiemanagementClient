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
  public class DistributorModifyViewModel : Screen, IDistributorModifyViewModel
  {
    private readonly IEnergyViewModelFactory m_EnergyViewModelFactory;
    private readonly Distributor m_Model;
    private readonly IEnergyRepository m_Repository;
    private Reading m_newReading;

    public DistributorModifyViewModel(Distributor model, [Import] IEnergyRepository consumerRepository,
                                      [Import] IEnergyViewModelFactory energyViewModelFactory)
    {
      DisplayName = "Verteiler bearbeiten...";
      m_Model = model;
      m_EnergyViewModelFactory = energyViewModelFactory;
      m_Repository = consumerRepository;
      ReadingAddVm = new ReadingAddViewModel();
    }

    public Distributor Model
    {
      get { return m_Model; }
    }

    public ReadingAddViewModel ReadingAddVm { get; set; }

    public virtual string Name
    {
      get { return m_Model.Name; }
      set
      {
        m_Model.Name = value;
        NotifyOfPropertyChange(() => Name);
      }
    }

    public string Comment
    {
      get { return m_Model.Comment; }
      set { m_Model.Comment = value; }
    }

    public bool IsMainDistributor
    {
      get { return m_Model.IsMainDistributor; }
      set { m_Model.IsMainDistributor = value; }
    }

    public IList<ReadingViewModel> Readings
    {
      get
      {
        return m_Model.Readings.Select(rvm => m_EnergyViewModelFactory.CreateFromExisting(rvm))
                      .ToList();
      }
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