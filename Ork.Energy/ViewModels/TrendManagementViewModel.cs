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
using System.Linq;
using System.Windows;
using Ork.Energy.Domain.DomainModelService;
using Ork.Framework;

namespace Ork.Energy.ViewModels
{
  [Export(typeof (IWorkspace))]
  public class TrendManagementViewModel : DocumentBase, IWorkspace
  {
    private readonly IEnergyRepository m_Repository;
    private bool m_IsEnabled;

    [ImportingConstructor]
    public TrendManagementViewModel([Import] IEnergyRepository contextRepository)
    {
      m_Repository = contextRepository;
      m_Repository.ContextChanged += (s, e) => Application.Current.Dispatcher.Invoke(Reload);

      Reload();
    }

    public int Index
    {
      get { return 402; }
    }

    public bool IsEnabled
    {
      get { return m_IsEnabled; }
      private set
      {
        m_IsEnabled = value;
        NotifyOfPropertyChange(() => IsEnabled);
      }
    }

    public string Title
    {
      get { return "Auswertung"; }
    }

    private void Reload()
    {
      IsEnabled = m_Repository.HasConnection;
      if (IsEnabled)
      {
        LoadData();
      }
    }

    private void LoadData()
    {
      var result = new List<PlotPoint>();
      var distributor = m_Repository.Distributors.First();
      //LoadMeasures();
      var relevantMeasures = m_Repository.Measures.Where(m => m.Consumer.Distributor == distributor);
      var relevantDays = relevantMeasures.Select(r => r.ConsumptionActual.ReadingDate).ToList();
      relevantDays.AddRange(relevantMeasures.Select(r => r.ConsumptionNormative.ReadingDate));
      relevantDays.Sort();
      foreach (var measurePoint in relevantDays)
      {
        result.Add(new PlotPoint(measurePoint, Summe(measurePoint)));
      }
    }

    private double Summe(DateTime measurePoint)
    {
      var distributor = m_Repository.Distributors.First();
      var returnvalue = 0.0;
      var relevantConsumers = m_Repository.Consumers.Where(c => c.Distributor == distributor);



      foreach (var consumer in relevantConsumers)
      {
        var closestReading = consumer.Readings.OrderBy(r => (r.ReadingDate - measurePoint).Duration())
                                     .First();

        returnvalue += closestReading.CounterReading;
      }

      return returnvalue;

    }


 
   

  }
}

class PlotPoint
{
  private DateTime plotDate;
  private double measureValue;

  public PlotPoint(DateTime dateTime, double value)
  {
    plotDate = dateTime;
    measureValue = value;
  }
}