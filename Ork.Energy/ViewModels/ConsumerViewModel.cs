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
  public class ConsumerViewModel : PropertyChangedBase

  {
    private readonly Consumer m_Model;
    private readonly IEnergyRepository m_Repository;

    public ConsumerViewModel(Consumer consumer, [Import] IEnergyRepository consumerRepository)
    {
      m_Model = consumer;
      m_Repository = consumerRepository;
    }

    public Consumer Model
    {
      get { return m_Model; }
    }

    public virtual Room Room
    {
      get { return m_Model.Room; }
    }

    public string RoomName
    {
      get { return m_Model.Room.RoomNumber; }
    }

    public virtual Distributor Distributor
    {
      get { return m_Model.Distributor; }
    }

    public string Name
    {
      get { return m_Model.Name; }
    }

    public virtual long? PowerOutput
    {
      get { return m_Model.PowerOutput; }
    }

    public virtual long? PowerCurrent
    {
      get { return m_Model.PowerCurrent; }
    }

    public virtual ICollection<Reading> Readings
    {
      get { return m_Model.Readings; }
    }

    public int? Year
    {
      get { return m_Model.Year; }
    }

    public virtual string Manufacturer
    {
      get { return m_Model.Manufacturer; }
    }

    public bool IsMachine
    {
      get { return m_Model.IsMachine; }
    }

    public string Identifier
    {
      get { return m_Model.Identifier; }
    }

    public ConsumerType ConsumerType
    {
      get { return m_Model.ConsumerType; }
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