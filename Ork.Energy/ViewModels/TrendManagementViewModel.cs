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
using System.Windows.Media;
using Ork.Energy.Domain.DomainModelService;
using Ork.Framework;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;

namespace Ork.Energy.ViewModels
{
  [Export(typeof (IWorkspace))]
  public class TrendManagementViewModel : DocumentBase, IWorkspace
  {
    private readonly IEnergyRepository m_Repository;
    private bool m_IsEnabled;
    private PlotModel m_Plot;

    [ImportingConstructor]
    public TrendManagementViewModel([Import] IEnergyRepository contextRepository)
    {
      m_Repository = contextRepository;
      m_Repository.ContextChanged += (s, e) => Application.Current.Dispatcher.Invoke(Reload);

      Reload();
    }

    private Distributor SelectedDistributor
    {
      get { return m_Repository.Distributors.First(); }
      //set NotifyOfPropertyChange(() => RelevantConsumer);
    }

    private IEnumerable<Consumer> RelevantConsumers
    {
      get { return m_Repository.Consumers.Where(c => c.Distributor == SelectedDistributor); }
    }

    public PlotModel TrendPlot
    {
      get
      {
        InitializeTrendPlot();
        LoadActualValueSeries();

        m_Plot.InvalidatePlot(true);
        return m_Plot;
      }
    }

    public int Index
    {
      get { return 402; }
    }

    public bool IsEnabled
    {
      get { return true; }
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

    private void Reload() {}

    private void LoadActualValueSeries()
    {
      var allReadingDatesFromSelectedDistributor = RelevantConsumers.SelectMany(rc => rc.Readings).ToList();
      var valueSeries = new LineSeries();
      foreach (var pointt in  allReadingDatesFromSelectedDistributor.OrderBy(r => r.ReadingDate))
      {
        valueSeries.Points.Add(new DataPoint(DateTimeAxis.ToDouble(pointt.ReadingDate),
          AccumulatedValuesAtDate(pointt.ReadingDate)));
      }
      m_Plot.Series.Add(valueSeries);
    }

    private double AccumulatedValuesAtDate(DateTime measurePoint)
    {
      var returnvalue = 0.0;

      foreach (var consumer in RelevantConsumers)
      {
        var readingsBeforeMeasurePoint = consumer.Readings.Where(r => r.ReadingDate <= measurePoint).ToList();

        if (readingsBeforeMeasurePoint.Any())
        {
          var closestReading = readingsBeforeMeasurePoint.OrderBy(r => (r.ReadingDate - measurePoint).Duration()).First();
          returnvalue += closestReading.CounterReading;
        }
        else
        {
          returnvalue += consumer.Readings.First().CounterReading;
        }
      }
      return returnvalue;
    }


    private void InitializeTrendPlot()
    {
      m_Plot = new PlotModel
      {
        LegendTitle = "Legende",
      };

      m_Plot.Axes.Clear();
      m_Plot.Series.Clear();

      var textForegroundColor = (Color) Application.Current.Resources["BlackColor"];
      var lightControlColor = (Color) Application.Current.Resources["WhiteColor"];

      // m_Plot.Title = ; //ausgewählter Verteiler
      m_Plot.LegendOrientation = LegendOrientation.Horizontal;
      m_Plot.LegendPlacement = LegendPlacement.Outside;
      m_Plot.TextColor = OxyColor.Parse(textForegroundColor.ToString());
      m_Plot.PlotAreaBorderColor = OxyColor.Parse(textForegroundColor.ToString());
      m_Plot.PlotAreaBorderThickness = new OxyThickness(1);

      var dateAxis = new DateTimeAxis()
      {
        IsPanEnabled = false,
        IsZoomEnabled = false
      };
      m_Plot.Axes.Add(dateAxis);

      var valueAxis = new LinearAxis(AxisPosition.Left, 0)
      {
        ShowMinorTicks = true,
        MinorGridlineStyle = LineStyle.Dot,
        MajorGridlineStyle = LineStyle.Dot,
        MajorGridlineColor = OxyColor.Parse(lightControlColor.ToString()),
        MinorGridlineColor = OxyColor.Parse(lightControlColor.ToString()),
        TicklineColor = OxyColor.Parse(textForegroundColor.ToString()),
        Title = "kwh",
        IsZoomEnabled = false,
        IsPanEnabled = false
      };

      m_Plot.Axes.Add(valueAxis);
    }
  }
}