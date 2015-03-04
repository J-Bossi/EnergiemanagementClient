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
    private Distributor m_SelectedDistributor;

    [ImportingConstructor]
    public TrendManagementViewModel([Import] IEnergyRepository contextRepository)
    {
      m_Repository = contextRepository;
      m_Repository.ContextChanged += (s, e) => Application.Current.Dispatcher.Invoke(Reload);

      Reload();
    }

    public Distributor SelectedDistributor
    {
      get
      {
        if (m_SelectedDistributor == null)
        {
          m_SelectedDistributor = m_Repository.Distributors.First();
        }
        return m_SelectedDistributor;
      }
      set
      {
        m_SelectedDistributor = value;
        NotifyOfPropertyChange(() => RelevantConsumers);
        NotifyOfPropertyChange(() => TrendPlot);
      }
    }

    public IEnumerable<Distributor> AllDistributors
    {
      get { return m_Repository.Distributors; }
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
        LoadDistributorValueSeries();

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
      var allReadingDatesFromSelectedDistributor = RelevantConsumers.SelectMany(rc => rc.Readings)
                                                                    .ToList();
      var allRelevantMeasuresFromSelectedDistributor = m_Repository.Measures.Where(m => RelevantConsumers.Contains(m.Consumer));
      var valueSeries = new LineSeries();
      var calculatedSeries = new LineSeries();
      var startValue = 0.0;
      foreach (var pointt in  allReadingDatesFromSelectedDistributor.OrderBy(r => r.ReadingDate))
      {
        valueSeries.Points.Add(new DataPoint(DateTimeAxis.ToDouble(pointt.ReadingDate),
          AccumulatedValuesAtDate(pointt.ReadingDate)));
        if (!calculatedSeries.Points.Any())
        {
          startValue = AccumulatedValuesAtDate(pointt.ReadingDate);
          calculatedSeries.Points.Add(new DataPoint(DateTimeAxis.ToDouble(pointt.ReadingDate), startValue));
        }
      }
      var newValue = startValue;
      foreach (var measure in allRelevantMeasuresFromSelectedDistributor)
      {
        newValue -= measure.SavedWattShould;
        calculatedSeries.Points.Add(new DataPoint(DateTimeAxis.ToDouble(measure.DueDate), newValue));
      }
      m_Plot.Series.Add(valueSeries);
      m_Plot.Series.Add((calculatedSeries));
    }

    private void LoadDistributorValueSeries()
    {
      var distributorSeries = new LineSeries();
      foreach (var reading in SelectedDistributor.Readings)
      {
        distributorSeries.Points.Add(new DataPoint(DateTimeAxis.ToDouble(reading.ReadingDate), reading.CounterReading));
      }
      m_Plot.Series.Add((distributorSeries));
    }


    private double AccumulatedValuesAtDate(DateTime measurePoint)
    {
      var returnvalue = 0.0;

      foreach (var consumer in RelevantConsumers)
      {
        var readingsBeforeMeasurePoint = consumer.Readings.Where(r => r.ReadingDate <= measurePoint)
                                                 .ToList();

        if (readingsBeforeMeasurePoint.Any())
        {
          var closestReading = readingsBeforeMeasurePoint.OrderBy(r => (r.ReadingDate - measurePoint).Duration())
                                                         .First();
          returnvalue += closestReading.CounterReading;
        }
        else
        {
          var sortedReadings = consumer.Readings.OrderBy(r => r.ReadingDate);
          if (sortedReadings.Any())
          {
            returnvalue += sortedReadings.First()
                                         .CounterReading;
          }
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

      m_Plot.Title = SelectedDistributor.Name;
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