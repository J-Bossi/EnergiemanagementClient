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
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using Caliburn.Micro;
using Ork.Energy.Domain.DomainModelService;

namespace Ork.Energy.ViewModels
{
  public class DistributorViewModel : PropertyChangedBase
  {
    private readonly Distributor m_Model;
    private readonly IEnergyRepository m_Repository;

    public DistributorViewModel(Distributor distributor, [Import] IEnergyRepository consumerRepository)
    {
      m_Model = distributor;
      m_Repository = consumerRepository;
    }

    public Distributor Model
    {
      get { return m_Model; }
    }

    public string Name
    {
      get { return m_Model.Name; }
    }

    public string Comment
    {
      get { return m_Model.Comment; }
    }

    public ICollection<Reading> Readings
    {
      get { return m_Model.Readings; }
    }

    public ICollection<Room> Rooms
    {
      get { return m_Repository.Rooms; }
    }

    public bool IsMainDistributor
    {
      get { return m_Model.IsMainDistributor; }
    }

    public Room Room
    {
      get { return m_Model.Room; }
    }

    public string RoomName
    {
      get { return m_Model.Room.RoomNumber; }
    }

    public int RelatedConsumers
    {
      get { return m_Repository.Consumers.Count(c => c.Distributor == Model); }
    }

    public String LastReadingDate
    {
      get
      {
        if (m_Model.Readings.Count == 0)
        {
          return "Keine Ablesung verfügbar";
        }
        else
        {
          return m_Model.Readings.OrderByDescending(r => r.ReadingDate)
                        .First()
                        .ReadingDate.ToShortDateString();
        }
      }
    }

    private void ModelPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      NotifyOfPropertyChange(e.PropertyName);
    }
  }
}