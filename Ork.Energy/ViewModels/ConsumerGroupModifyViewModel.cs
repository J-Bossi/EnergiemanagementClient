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

using System.ComponentModel.Composition;
using System.Data.Services.Client;
using System.Linq;
using Caliburn.Micro;
using Ork.Energy.DomainModelService;

namespace Ork.Energy.ViewModels
{
  public sealed class ConsumerGroupModifyViewModel : Screen
  {
    private readonly ConsumerGroup m_Model;
    private readonly IEnergyRepository m_Repository;

    public ConsumerGroupModifyViewModel(ConsumerGroup model, [Import] IEnergyRepository consumerRepository)
    {
      m_Repository = consumerRepository;
      DisplayName = "Verbrauchergruppe bearbeiten...";
      m_Model = model;
    }

    public ConsumerGroup Model
    {
      get { return m_Model; }
    }

    public string GroupName
    {
      get { return m_Model.GroupName; }
      set
      {
        m_Model.GroupName = value;
        NotifyOfPropertyChange(() => GroupName);
      }
    }

    public DataServiceCollection<ConsumerType> ConsumerTypes
    {
      get { return m_Model.ConsumerTypes; }
    }

    public string ConsumerType { get; set; }

    public void AddConsumerType()
    {
      m_Model.ConsumerTypes.Add(new ConsumerType
      {
        TypeName = ConsumerType
      });
      ConsumerType = null;
      NotifyOfPropertyChange(() => ConsumerType);
    }

    public void DeleteConsumerType(object dataContext)
    {
      if (m_Repository.Links.Any(c => c.Target == ((ConsumerType) dataContext)))
      {
        foreach (
          Consumer relatedConsumers in m_Repository.Consumers.Where(c => c.ConsumerType == (((ConsumerType) dataContext))))
        {
          relatedConsumers.ConsumerType = null;
        }
      }
      m_Model.ConsumerTypes.Remove((ConsumerType) dataContext);
      m_Repository.Save();

      NotifyOfPropertyChange(() => ConsumerTypes);
    }
  }
}