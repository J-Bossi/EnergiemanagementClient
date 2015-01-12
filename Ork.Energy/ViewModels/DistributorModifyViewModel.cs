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
using System.Collections.Generic;
using System.Linq;
using Caliburn.Micro;
using Ork.Energy.DomainModelService;
using Ork.Energy.Factories;

namespace Ork.Energy.ViewModels
{
  public class DistributorModifyViewModel : Screen
  {
    private Reading m_newReading;
    private readonly IEnergyViewModelFactory m_ConsumerViewModelFactory;
    private readonly Distributor m_Model;

    public DistributorModifyViewModel(Distributor model)
    {
      DisplayName = "Verteiler bearbeiten...";
      m_Model = model;
      NewReadingDate = DateTime.Now;
      m_ConsumerViewModelFactory = IoC.Get<IEnergyViewModelFactory>();
    }

    public string Name
    {
      get { return m_Model.Name; }
      set { m_Model.Name = value; }
    }

    public bool IsMainDistributor
    {
      get { return m_Model.IsMainDistributor; }
      set { m_Model.IsMainDistributor = value; }
    }

    public IEnumerable<ReadingViewModel> Readings
    {
      get { return m_Model.Readings.Select(rvm => m_ConsumerViewModelFactory.CreateFromExisting(rvm)); }
    }

    public Room Room
    {
      get { return m_Model.Room; }
      set { m_Model.Room = value; }
    }

    public long NewCounterReading { get; set; }
    public DateTime NewReadingDate { get; set; }

    public void AddNewReading(object dataContext)
    {
      m_Model.Readings.Add(ModelFactory.CreateReading(NewReadingDate, NewCounterReading));
      ClearReadingFields();
      NotifyOfPropertyChange(() => Readings);
    }

    private void ClearReadingFields()
    {
      NewCounterReading = 0;
      NewReadingDate = DateTime.Now;
      NotifyOfPropertyChange(() => NewCounterReading);
      NotifyOfPropertyChange(() => NewReadingDate);
    }
  }
}