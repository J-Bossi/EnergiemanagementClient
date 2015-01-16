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
// Copyright (c) 2014, HTW Berlin

#endregion

using System.ComponentModel.Composition;
using System.Data.Services.Client;
using System.Linq;
using Ork.Energy.DomainModelService;

namespace Ork.Energy.ViewModels
{
  public class ConsumerGroupViewModel
  {
    private readonly ConsumerGroup m_Model;
    private readonly IEnergyRepository m_Repository;

    public ConsumerGroupViewModel(ConsumerGroup consumerGroup, [Import] IEnergyRepository consumerRepository)
    {
      m_Model = consumerGroup;
      m_Repository = consumerRepository;
    }

    public ConsumerGroup Model
    {
      get { return m_Model; }
    }

    public string GroupName
    {
      get { return m_Model.GroupName; }
      set { m_Model.GroupName = value; }
    }


    public int RelatedConsumers
    {
      get { return m_Repository.Consumers.Count(c => c.ConsumerGroup == Model); }
    }

    public string FullDate
    {
      get
      {
        //var dateList = m_Repository.Measures.Select(measure => measure.DueDate)
        //                      .ToList();
        //dateList.Sort();

        //if (dateList.Count == 0)
        //{
        //  return "";
        //}
        //return dateList.First()
        //               .ToShortDateString() + " - " + dateList.Last()
        //                                                      .ToShortDateString();
        return "000";
      }
    }
  }
}