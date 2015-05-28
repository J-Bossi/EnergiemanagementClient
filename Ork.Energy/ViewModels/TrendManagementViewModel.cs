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
    private PlotModel m_ConsumerPlot;
    private PlotModel m_DistributorPlot;
    private bool m_IsEnabled;
    private Distributor m_SelectedDistributor;
    private readonly IEnergyRepository m_Repository;

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
        NotifyOfPropertyChange(() => TrendConsumerPlot);
        NotifyOfPropertyChange(() => TrendDistributorPlot);
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

    public PlotModel TrendConsumerPlot
    {
      get
      {
        InitializeTrendConsumerPlot();
        LoadActualValueSeries();

        m_ConsumerPlot.InvalidatePlot(true);
        return m_ConsumerPlot;
      }
    }

    public PlotModel TrendDistributorPlot
    {
      get
      {
        InitializeTrendDistributorPlot();
        LoadDistributorValueSeries();
        m_DistributorPlot.InvalidatePlot(true);
        return m_DistributorPlot;
      }
    }

    public int Index
    {
      get { return 402; }
    }

    public bool IsEnabled
    {
      get { return m_IsEnabled; }
      set
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
      m_SelectedDistributor = null;
      NotifyOfPropertyChange(() => SelectedDistributor);
    }

    private void LoadActualValueSeries()
    {
      var allReadingDatesFromSelectedDistributor = RelevantConsumers.SelectMany(rc => rc.Readings)
                                                                    .ToList();
      allReadingDatesFromSelectedDistributor.Sort((a, b) => a.ReadingDate.CompareTo(b.ReadingDate));
      var allRelevantMeasuresFromSelectedDistributor = m_Repository.Measures.Where(m => RelevantConsumers.Contains(m.Consumer));
      var valueSeries = new LineSeries
      {
        Title = "Gemessene Verbrauchswerte",
        Color = OxyColors.DarkGray,
        StrokeThickness = 3,
        TrackerFormatString = "{0} " + Environment.NewLine + "{1}: {2:dd.MM.yy} " + Environment.NewLine + "{3}: {4} "
      };

      var calculatedSeries = new LineSeries
      {
        Title = "Prognostizierte Verbrauchswerte",
        Color = OxyColors.DarkViolet,
        StrokeThickness = 3,
        TrackerFormatString = "{0} " + Environment.NewLine + "{1}: {2:dd.MM.yy} " + Environment.NewLine + "{3}: {4} "
      };

      var startValue = 0.0;
      var startDate = new DateTime();
      var lastPoint = new Reading();
      var lastMeasure = new Measure();
      foreach (var pointt in  allReadingDatesFromSelectedDistributor.OrderBy(r => r.ReadingDate))
      {
        valueSeries.Points.Add(new DataPoint(DateTimeAxis.ToDouble(pointt.ReadingDate),
          AccumulatedValuesAtDate(pointt.ReadingDate)));
        if (!calculatedSeries.Points.Any())
        {
          startValue = AccumulatedValuesAtDate(pointt.ReadingDate);
          startDate = pointt.ReadingDate;
          calculatedSeries.Points.Add(new DataPoint(DateTimeAxis.ToDouble(pointt.ReadingDate), startValue));
        }
        lastPoint = pointt;
      }
      var newValue = startValue;
      foreach (var measure in allRelevantMeasuresFromSelectedDistributor.OrderBy(m => m.DueDate))
      {
        if (measure.DueDate > startDate) { 
        newValue -= measure.SavedWattShould;
        calculatedSeries.Points.Add(new DataPoint(DateTimeAxis.ToDouble(measure.DueDate), newValue));
          lastMeasure = measure;
        }
      }
      if (lastPoint.ReadingDate > lastMeasure.DueDate )
      {
        calculatedSeries.Points.Add(new DataPoint(DateTimeAxis.ToDouble(lastPoint.ReadingDate), newValue));
      }
      m_ConsumerPlot.Series.Add(valueSeries);
      m_ConsumerPlot.Series.Add((calculatedSeries));
    }

    private void LoadDistributorValueSeries()
    {
      var distributorSeries = new LineSeries
      {
        Title = "Verteiler",
        Color = OxyColors.DarkOrange,
        StrokeThickness = 3,
        TrackerFormatString = "{0} " + Environment.NewLine + "{1}: {2:dd.MM.yy} " + Environment.NewLine + "{3}: {4} "
      };

      foreach (var reading in SelectedDistributor.Readings.OrderBy(r => r.ReadingDate))
      {
        distributorSeries.Points.Add(new DataPoint(DateTimeAxis.ToDouble(reading.ReadingDate), reading.CounterReading));
      }
      m_DistributorPlot.Series.Add((distributorSeries));
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

    private void InitializeTrendConsumerPlot()
    {
      m_ConsumerPlot = new PlotModel
      {
        LegendTitle = "Legende",
      };

      m_ConsumerPlot.Axes.Clear();
      m_ConsumerPlot.Series.Clear();

      var textForegroundColor = (Color) Application.Current.Resources["BlackColor"];
      var lightControlColor = (Color) Application.Current.Resources["WhiteColor"];

      m_ConsumerPlot.Title = SelectedDistributor.Name;
      m_ConsumerPlot.LegendOrientation = LegendOrientation.Horizontal;
      m_ConsumerPlot.LegendPlacement = LegendPlacement.Outside;
      m_ConsumerPlot.LegendPosition = LegendPosition.BottomLeft;
      m_ConsumerPlot.TextColor = OxyColor.Parse(textForegroundColor.ToString());
      m_ConsumerPlot.PlotAreaBorderColor = OxyColor.Parse(textForegroundColor.ToString());
      m_ConsumerPlot.PlotAreaBorderThickness = new OxyThickness(1);

      var dateAxis = new DateTimeAxis()
      {
        IsPanEnabled = false,
        IsZoomEnabled = false,
        Title = "Datum",
        StringFormat = "dd-MM-yyyy",
      };
      m_ConsumerPlot.Axes.Add(dateAxis);

      var valueAxis = new LinearAxis(AxisPosition.Left, 0)
      {
        ShowMinorTicks = true,
        MinorGridlineStyle = LineStyle.Dot,
        MajorGridlineStyle = LineStyle.Dot,
        MajorGridlineColor = OxyColor.Parse(lightControlColor.ToString()),
        MinorGridlineColor = OxyColor.Parse(lightControlColor.ToString()),
        TicklineColor = OxyColor.Parse(textForegroundColor.ToString()),
        Title = "kWh pro Jahr",
        IsZoomEnabled = false,
        IsPanEnabled = false
      };

      m_ConsumerPlot.Axes.Add(valueAxis);
    }

    private void InitializeTrendDistributorPlot()
    {
      m_DistributorPlot = new PlotModel
      {
        LegendTitle = "Legende",
      };

      m_DistributorPlot.Axes.Clear();
      m_DistributorPlot.Series.Clear();

      var textForegroundColor = (Color) Application.Current.Resources["BlackColor"];
      var lightControlColor = (Color) Application.Current.Resources["WhiteColor"];

      m_DistributorPlot.Title = SelectedDistributor.Name;
      m_DistributorPlot.LegendOrientation = LegendOrientation.Horizontal;
      m_DistributorPlot.LegendPlacement = LegendPlacement.Outside;
      m_DistributorPlot.LegendPosition = LegendPosition.BottomLeft;
      m_DistributorPlot.TextColor = OxyColor.Parse(textForegroundColor.ToString());
      m_DistributorPlot.PlotAreaBorderColor = OxyColor.Parse(textForegroundColor.ToString());
      m_DistributorPlot.PlotAreaBorderThickness = new OxyThickness(1);

      var dateAxis = new DateTimeAxis()
      {
        IsPanEnabled = false,
        IsZoomEnabled = false,
        Title = "Datum",
        StringFormat = "dd-MM-yyyy",
      };
      m_DistributorPlot.Axes.Add(dateAxis);

      var valueAxis = new LinearAxis(AxisPosition.Left, 0)
      {
        ShowMinorTicks = true,
        MinorGridlineStyle = LineStyle.Dot,
        MajorGridlineStyle = LineStyle.Dot,
        MajorGridlineColor = OxyColor.Parse(lightControlColor.ToString()),
        MinorGridlineColor = OxyColor.Parse(lightControlColor.ToString()),
        TicklineColor = OxyColor.Parse(textForegroundColor.ToString()),
        Title = "kWh pro Jahr",
        IsZoomEnabled = false,
        IsPanEnabled = false
      };

      m_DistributorPlot.Axes.Add(valueAxis);
    }
  }
}