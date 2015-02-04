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
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Caliburn.Micro;
using Ork.Energy.Domain.DomainModelService;
using Ork.Energy.Factories;
using Ork.Framework;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;

namespace Ork.Energy.ViewModels
{
  [Export(typeof (IWorkspace))]
  public class MeasureManagementViewModel : DocumentBase, IWorkspace
  {
    private readonly BindableCollection<ConsumerGroupViewModel> m_ConsumerGroups =
      new BindableCollection<ConsumerGroupViewModel>();

    private readonly IEnergyViewModelFactory m_EnergyViewModelFactory;
    private readonly IMeasureViewModelFactory m_MeasureViewModelFactory;
    private readonly BindableCollection<MeasureViewModel> m_Measures = new BindableCollection<MeasureViewModel>();
    private readonly IEnergyRepository m_Repository;
    private IScreen m_EditItem;
    private bool m_FlyoutActivated;
    private bool m_IsEnabled;
    private PlotModel m_Plot;
    //private bool m_PlotIsVisible;
    //private bool m_EcoPlotIsVisible;
    //private bool m_EcoPlotIsVisible2;
    private string m_SearchText;
    private string m_SearchTextMeasures;
    private ConsumerGroupViewModel m_SelectedConsumerGroup;
    private MeasureViewModel m_SelectedMeasure;
    //private bool m_DgvVisible;
    //private bool m_DgvVisibleCat;
    //private bool m_DgvVisibleEco;

    [ImportingConstructor]
    public MeasureManagementViewModel([Import] IEnergyRepository contextRepository,
                                      [Import] IMeasureViewModelFactory measureViewModelFactory,
                                      [Import] IEnergyViewModelFactory energyViewModelFactory)
    {
      m_Repository = contextRepository;
      m_MeasureViewModelFactory = measureViewModelFactory;
      m_EnergyViewModelFactory = energyViewModelFactory;

      m_Repository.ContextChanged += (s, e) => Application.Current.Dispatcher.Invoke(Reload);

      Reload();

      //VisibleCat = false;       // Sichtbarkeit ausgewählter Katalog
      //VisibleEco = false;       // Siehtbarkeit ökologische Ansicht
      //VisibleNormal = true;     // Sichtbarkeit normale Ansicht

      //VisibleEcoPlot = false;   // Sichtbarkeit ökologische Grafik (Investition & Ersparnis)
      //VisibleEcoPlot2 = false;  // Sichtbarkeit ökologische Grafik (CO2)
      //VisiblePlot = true;       // Sichtbarkeit normale Grafik

      FlyoutActivated = true;
    }

    public IEnumerable<MeasureViewModel> Measures
    {
      get
      {
        IEnumerable<MeasureViewModel> measure;
        if (SelectedConsumerGroup != null)
        {
          measure =
            m_Measures.Where(
              mvm => mvm.Model.Consumer != null && mvm.Model.Consumer.ConsumerGroup == SelectedConsumerGroup.Model)
                      .ToArray();
        }
        else
        {
          measure = m_Measures;
        }

        //VisiblePlot = measure.Any();
        return SearchInMeasureList(measure);
      }
    }

    public IEnumerable<ConsumerGroupViewModel> ConsumerGroups
    {
      get { return FilteredConsumerGroups; }
    }

    public IEnumerable<ConsumerGroupViewModel> FilteredConsumerGroups
    {
      get
      {
        var filteredConsumerGroups = SearchInConsumerGroupList()
          .OrderBy(cat => cat.GroupName)
          .ToArray();
        return filteredConsumerGroups;
      }
    }

    public string SearchText
    {
      get { return m_SearchText; }
      set
      {
        m_SearchText = value;
        NotifyOfPropertyChange(() => ConsumerGroups);
      }
    }

    public string SearchTextMeasures
    {
      get { return m_SearchTextMeasures; }
      set
      {
        m_SearchTextMeasures = value;
        NotifyOfPropertyChange(() => Measures);
      }
    }

    public bool FlyoutActivated
    {
      get { return m_FlyoutActivated; }
      set
      {
        if (m_FlyoutActivated == value)
        {
          return;
        }
        m_FlyoutActivated = value;
        NotifyOfPropertyChange(() => FlyoutActivated);
      }
    }

    public ConsumerGroupViewModel SelectedConsumerGroup // Zuständig für Sichtbarkeit in Maßnahmenübersicht
    {
      get { return m_SelectedConsumerGroup; }
      set
      {
        //if (value == null)
        //{
        //  VisibleCat = false;
        //  VisibleNormal = true;
        //  VisibleEco = false;

        //  VisiblePlot = true;
        //  VisibleEcoPlot = false;
        //  VisibleEcoPlot2 = false;
        //}
        //else
        //{
        //    VisibleCat = true;
        //    VisibleNormal = false;
        //    VisibleEco = false;

        //    VisiblePlot = true;
        //    VisibleEcoPlot = false;
        //    VisibleEcoPlot2 = false;
        //}

        m_SelectedConsumerGroup = value;
        NotifyOfPropertyChange(() => SelectedConsumerGroup);
        NotifyOfPropertyChange(() => CanAdd);
        NotifyOfPropertyChange(() => PlotModel);
        NotifyOfPropertyChange(() => EcoPlotModel);
        NotifyOfPropertyChange(() => EcoPlotModel2);
        NotifyOfPropertyChange(() => Measures);
      }
    }

    public string AllMeasures
    {
      get { return CalculateDateInterval(); }
    }

    //public bool VisibleNormal           // Sichtbarkeit Alle Maßnahmen
    //{
    //  get { return m_DgvVisible; }
    //  set
    //  {
    //    m_DgvVisible = value;
    //    NotifyOfPropertyChange(() => VisibleNormal);
    //  }
    //}

    //public bool VisibleCat              // Sichtbarkeit ausgewählter Katalog
    //{
    //    get { return m_DgvVisibleCat; }
    //    set
    //    {
    //        m_DgvVisibleCat = value;
    //        NotifyOfPropertyChange(() => VisibleCat);
    //    }
    //}

    //public bool VisibleEco          // Sichtbarkeit Einsparung
    //{
    //    get { return m_DgvVisibleEco; }
    //    set
    //    {
    //        m_DgvVisibleEco = value;
    //        NotifyOfPropertyChange(() => VisibleEco);
    //    }
    //}

    //public bool VisiblePlot     // Sichtbarkeit Grafik-Allg.
    //{
    //    get { return m_PlotIsVisible; }
    //    set
    //    {
    //        m_PlotIsVisible = value;
    //        NotifyOfPropertyChange(() => VisiblePlot);
    //    }
    //}

    //public bool VisibleEcoPlot     // Sichtbarkeit Grafik-Eco
    //{
    //    get { return m_EcoPlotIsVisible; }
    //    set
    //    {
    //        m_EcoPlotIsVisible = value;
    //        NotifyOfPropertyChange(() => VisibleEcoPlot);
    //    }
    //}

    //public bool VisibleEcoPlot2     // Sichtbarkeit Grafik-Eco2
    //{
    //    get { return m_EcoPlotIsVisible2; }
    //    set
    //    {
    //        m_EcoPlotIsVisible2 = value;
    //        NotifyOfPropertyChange(() => VisibleEcoPlot2);
    //    }
    //}


    public bool CanAdd
    {
      // TODO: Zuordnung einer Maßnahme zu einem Verbraucher muss sichergestellt werden... oder nicht?
      get { return true; }
    }

    public MeasureViewModel SelectedMeasure { get; set; }

    public PlotModel PlotModel
    {
      get
      {
        //todo: optimize initialization
        InitializePlot();

        var measures = Measures.ToArray();
        if (measures.Any())
        {
          GenerateGraphData(measures);
        }
        m_Plot.InvalidatePlot(true);
        return m_Plot;
      }
    }

    public PlotModel EcoPlotModel
    {
      get
      {
        //todo: optimize initialization
        InitializeEcoPlot();

        var measures = Measures.ToArray();
        if (measures.Any())
        {
          GenerateEcoGraphData(measures);
        }
        m_Plot.InvalidatePlot(true);
        return m_Plot;
      }
    }

    public PlotModel EcoPlotModel2
    {
      get
      {
        //todo: optimize initialization
        InitializeEcoPlot2();

        var measures = Measures.ToArray();
        if (measures.Any())
        {
          GenerateEcoGraphData2(measures);
        }
        m_Plot.InvalidatePlot(true);
        return m_Plot;
      }
    }

    public int Index
    {
      get { return 300; }
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
      get { return TranslationProvider.Translate("TitleMeasureManagementViewModel"); }
    }

    private string CalculateDateInterval()
    {
      var measures = m_Repository.Measures;
      //Measure[] measures = m_ConsumerGroups.SelectMany(cat => cat.Measures)
      //                                     .ToArray();
      if (!measures.Any())
      {
        return TranslationProvider.Translate("NoneAvailable");
      }
      var dateList = measures.Select(measure => measure.DueDate)
                             .OrderBy(m => m)
                             .ToArray();
      return dateList.First()
                     .ToShortDateString() + " - " + dateList.Last()
                                                            .ToShortDateString();
    }

    private IEnumerable<MeasureViewModel> SearchInMeasureList(IEnumerable<MeasureViewModel> measureList)
    {
      var searchText = m_SearchTextMeasures.ToLower();
      return measureList.Where(mvm => mvm.ResponsibleSubjectName.ToLower()
                                         .Contains(searchText));
    }

    private void AlterConsumerGroupCollection(object sender, NotifyCollectionChangedEventArgs e)
    {
      switch (e.Action)
      {
        case NotifyCollectionChangedAction.Add:
          foreach (var newItem in e.NewItems.OfType<ConsumerGroup>())
          {
            m_ConsumerGroups.Add(CreateConsumerGroupViewModel(newItem));
          }
          break;
        case NotifyCollectionChangedAction.Remove:
          foreach (var consumerGroupViewModel in e.OldItems.OfType<ConsumerGroup>()
                                                  .Select(oldItem => m_ConsumerGroups.Single(r => r.Model == oldItem)))
          {
            m_ConsumerGroups.Remove(consumerGroupViewModel);
          }
          break;
      }
      NotifyOfPropertyChange(() => ConsumerGroups);
    }

    public void RemoveMeasure()
    {
      var measureViewModel = SelectedMeasure;
      var allSubMeasures = m_Repository.SubMeasures.Where(sm => sm.ReleatedMeasure == measureViewModel.Model)
                                       .ToList();
      foreach (var subMeasure in allSubMeasures)
      {
        m_Repository.SubMeasures.Remove(subMeasure);
      }

      m_Measures.Remove(measureViewModel);
      m_Repository.Measures.Remove(measureViewModel.Model);
      Save();

      NotifyOfPropertyChange(() => Measures);
      NotifyOfPropertyChange(() => AllMeasures);
      NotifyOfPropertyChange(() => PlotModel);
    }

    public void AddMeasure(object dataContext)
    {
      var measureAddViewModel = ((MeasureAddViewModel) dataContext);

      CreateMeasureViewModel(measureAddViewModel.Model);
      Save();

      NotifyOfPropertyChange(() => AllMeasures);
      NotifyOfPropertyChange(() => Measures);
      NotifyOfPropertyChange(() => PlotModel);
      NotifyOfPropertyChange(() => EcoPlotModel);
      NotifyOfPropertyChange(() => EcoPlotModel2);
    }

    private void Save()
    {
      CloseEditor();
      m_Repository.Save();
    }

    public void Accept(object dataContext)
    {
      Save();

      NotifyOfPropertyChange(() => Measures);
      NotifyOfPropertyChange(() => ConsumerGroups);
      NotifyOfPropertyChange(() => SelectedConsumerGroup);
      NotifyOfPropertyChange(() => AllMeasures);
      NotifyOfPropertyChange(() => PlotModel);
    }

    public void Back(object dataContext)
    {
      var temp = SelectedMeasure;
      CloseEditor();

      OpenMeasureEditDialog(temp);
    }

    public IEnumerable<ConsumerGroupViewModel> SearchInConsumerGroupList()
    {
      if (string.IsNullOrEmpty(SearchText))
      {
        return m_ConsumerGroups;
      }
      var searchText = SearchText.ToLower();

      return m_ConsumerGroups.Where(c => (((c.GroupName != null) && (c.GroupName.ToLower()
                                                                      .Contains(searchText)))));
    }

    private void OpenEditor(object dataContext)
    {
      m_EditItem = (IScreen) dataContext;
      Dialogs.ShowDialog(m_EditItem);
    }

    public void OpenPrintPreviewDialog(MeasureViewModel measureViewModel)
    {
      if (measureViewModel != null)
      {
        OpenEditor(m_MeasureViewModelFactory.CreatePrintPreviewModel(measureViewModel.Model, RemoveMeasure));
      }
    }

    public void OpenMeasureAddDialog()
    {
      OpenEditor(m_MeasureViewModelFactory.CreateAddViewModel());
    }

    public void OpenCatalogAddDialog()
    {
      OpenEditor(m_MeasureViewModelFactory.CreateCatalogAddViewModel());
    }

    //private void OpenCatalogEditDialog(object dataContext)
    //{
    //  SelectedCatalog = (CatalogViewModel) dataContext;
    //  OpenEditor(m_MeasureViewModelFactory.CreateCatalogEditViewModel((CatalogViewModel) dataContext, RemoveCatalog));
    //}

    private void OpenMeasureEditDialog(MeasureViewModel measureViewModel)
    {
      if (measureViewModel != null)
      {
        OpenEditor(m_MeasureViewModelFactory.CreateEditViewModel(measureViewModel.Model, RemoveMeasure));
      }
    }

    public void OpenEditor(object dataContext, MouseButtonEventArgs e)
    {
      if (e.ClickCount < 2)
      {
        return;
      }
      if (dataContext is ConsumerGroupViewModel) {}
      else if (dataContext is MeasureManagementViewModel)
      {
        OpenMeasureEditDialog(SelectedMeasure);
      }
    }

    public void Cancel()
    {
      CloseEditor();
    }

    //public void Print()
    //{
    //    PrintDialog printDialog = new PrintDialog();
    //    if (printDialog.ShowDialog() == true)
    //    {
    //        printDialog.PrintDocument(((IDocumentPaginatorSource)flowDocument).DocumentPaginator, "Flow Document Print Job");
    //    }
    //}

    private void CloseEditor()
    {
      m_EditItem.TryClose();
      m_EditItem = null;
    }

    private void Reload()
    {
      IsEnabled = m_Repository.HasConnection;
      if (IsEnabled)
      {
        LoadData();
      }

      NotifyOfPropertyChange(() => AllMeasures);
      ShowAllMeasures();
    }

    public void ShowAllMeasures()
    {
      SelectedConsumerGroup = null;
    }

    private void LoadData()
    {
      m_Measures.Clear();
      m_ConsumerGroups.Clear();
      ((INotifyPropertyChanged) m_ConsumerGroups).PropertyChanged += CGPropertyshavechanged;
      LoadConsumerGroups();
      LoadMeasures();
    }


    private void CGPropertyshavechanged(object sender, PropertyChangedEventArgs e)
    {
      NotifyOfPropertyChange(() => ConsumerGroups);
    }

    private void LoadConsumerGroups()
    {
      m_Repository.ConsumerGroups.CollectionChanged += AlterConsumerGroupCollection;

      foreach (var cg in m_Repository.ConsumerGroups)
      {
        m_ConsumerGroups.Add(CreateConsumerGroupViewModel(cg));
      }

      NotifyOfPropertyChange(() => ConsumerGroups);
    }

    private void LoadMeasures()
    {
      foreach (var measure in m_Repository.Measures)
      {
        CreateMeasureViewModel(measure);
      }
      NotifyOfPropertyChange(() => Measures);
    }

    private ConsumerGroupViewModel CreateConsumerGroupViewModel(ConsumerGroup consumerGroup)
    {
      return m_EnergyViewModelFactory.CreateFromExisting(consumerGroup);
    }

    private void CreateMeasureViewModel(EnergyMeasure measure)
    {
      var mvm = m_MeasureViewModelFactory.CreateFromExisting(measure);
      m_Measures.Add(mvm);
    }

    public void printPreview(object dataContext)
    {
      OpenPrintPreviewDialog(SelectedMeasure);
    }

    #region Plot

    private void InitializePlot() // Organisatorische Grafik
    {
      m_Plot = new PlotModel
      {
        LegendTitle = TranslationProvider.Translate(Assembly.GetExecutingAssembly(), "TitleMeasureManagementViewModel"),
      };

      m_Plot.Axes.Clear();
      m_Plot.Series.Clear();
      var cgName = SelectedConsumerGroup == null
        ? TranslationProvider.Translate("AllMeasures")
        : SelectedConsumerGroup.GroupName;

      var textForegroundColor = (Color) Application.Current.Resources["BlackColor"];
      var lightControlColor = (Color) Application.Current.Resources["WhiteColor"];

      m_Plot.Title = cgName;
      m_Plot.LegendOrientation = LegendOrientation.Horizontal;
      m_Plot.LegendPlacement = LegendPlacement.Outside;
      m_Plot.TextColor = OxyColor.Parse(textForegroundColor.ToString());
      m_Plot.PlotAreaBorderColor = OxyColor.Parse(textForegroundColor.ToString());
      m_Plot.PlotAreaBorderThickness = new OxyThickness(1);

      var monthArray = new string[6];
      var k = 0;
      for (var i = -5; i < 1; i++)
      {
        monthArray[k++] = DateTime.Now.AddMonths(i)
                                  .ToString("MMM");
      }

      var categoryAxis = new CategoryAxis(TranslationProvider.Translate("Month"), monthArray.ToArray())
      {
        TicklineColor = OxyColor.Parse(textForegroundColor.ToString()),
        IsZoomEnabled = false,
        IsPanEnabled = false
      };

      m_Plot.Axes.Add(categoryAxis);

      var valueAxis = new LinearAxis(AxisPosition.Left, 0)
      {
        ShowMinorTicks = true,
        MinorGridlineStyle = LineStyle.Dot,
        MajorGridlineStyle = LineStyle.Dot,
        MajorGridlineColor = OxyColor.Parse(lightControlColor.ToString()),
        MinorGridlineColor = OxyColor.Parse(lightControlColor.ToString()),
        Title = TranslationProvider.Translate("Count"),
        TicklineColor = OxyColor.Parse(textForegroundColor.ToString()),
        IsZoomEnabled = false,
        IsPanEnabled = false
      };

      m_Plot.Axes.Add(valueAxis);
    }

    private void InitializeEcoPlot() // Erste ökologische Grafik -> finanzrelevantes Balkendiagramm
    {
      m_Plot = new PlotModel
      {
        LegendTitle = TranslationProvider.Translate(Assembly.GetExecutingAssembly(), "TitleMeasureManagementViewModel"),
      };

      m_Plot.Axes.Clear();
      m_Plot.Series.Clear();
      var cgName = SelectedConsumerGroup == null
        ? TranslationProvider.Translate("AllMeasures")
        : SelectedConsumerGroup.GroupName;

      var textForegroundColor = (Color) Application.Current.Resources["BlackColor"];
      var lightControlColor = (Color) Application.Current.Resources["WhiteColor"];

      m_Plot.Title = cgName;
      m_Plot.LegendOrientation = LegendOrientation.Horizontal;
      m_Plot.LegendPlacement = LegendPlacement.Outside;
      m_Plot.TextColor = OxyColor.Parse(textForegroundColor.ToString());
      m_Plot.PlotAreaBorderColor = OxyColor.Parse(textForegroundColor.ToString());
      m_Plot.PlotAreaBorderThickness = new OxyThickness(1);

      var measureIdArray = Measures.Select(measure => measure.Id.ToString())
                                   .ToArray();

      var categoryAxis = new CategoryAxis(TranslationProvider.Translate("Measure IDs"), new[]
      {
        "1", "2"
      })
      {
        TicklineColor = OxyColor.Parse(textForegroundColor.ToString()),
        IsZoomEnabled = false,
        IsPanEnabled = false,
      };

      m_Plot.Axes.Add(categoryAxis);

      var valueAxis = new LinearAxis(AxisPosition.Left, 0)
      {
        ShowMinorTicks = true,
        MinorGridlineStyle = LineStyle.Dot,
        MajorGridlineStyle = LineStyle.Dot,
        MajorGridlineColor = OxyColor.Parse(lightControlColor.ToString()),
        MinorGridlineColor = OxyColor.Parse(lightControlColor.ToString()),
        TicklineColor = OxyColor.Parse(textForegroundColor.ToString()),
        Title = "Euro",
        IsZoomEnabled = false,
        IsPanEnabled = false
      };

      m_Plot.Axes.Add(valueAxis);
    }

    private void InitializeEcoPlot2() // Zweite ökologische Grafik -> CO2 Balkendiagramm
    {
      m_Plot = new PlotModel
      {
        LegendTitle = TranslationProvider.Translate(Assembly.GetExecutingAssembly(), "TitleMeasureManagementViewModel"),
      };

      m_Plot.Axes.Clear();
      m_Plot.Series.Clear();
      var cgName = SelectedConsumerGroup == null
        ? TranslationProvider.Translate("AllMeasures")
        : SelectedConsumerGroup.GroupName;

      var textForegroundColor = (Color) Application.Current.Resources["BlackColor"];
      var lightControlColor = (Color) Application.Current.Resources["WhiteColor"];

      m_Plot.Title = cgName;
      m_Plot.LegendOrientation = LegendOrientation.Horizontal;
      m_Plot.LegendPlacement = LegendPlacement.Outside;
      m_Plot.TextColor = OxyColor.Parse(textForegroundColor.ToString());
      m_Plot.PlotAreaBorderColor = OxyColor.Parse(textForegroundColor.ToString());
      m_Plot.PlotAreaBorderThickness = new OxyThickness(1);

      var measureNamesArray = Measures.Select(measure => measure.Name)
                                      .ToArray();
      var measureIdArray = Measures.Select(measure => measure.Id.ToString())
                                   .ToArray();

      var categoryAxis = new CategoryAxis(TranslationProvider.Translate("Measure IDs"), new[]
      {
        "1"
      })
      {
        TicklineColor = OxyColor.Parse(textForegroundColor.ToString()),
        IsZoomEnabled = false,
        IsPanEnabled = false,
      };

      m_Plot.Axes.Add(categoryAxis);

      var valueAxis = new LinearAxis(AxisPosition.Left, 0)
      {
        ShowMinorTicks = true,
        MinorGridlineStyle = LineStyle.Dot,
        MajorGridlineStyle = LineStyle.Dot,
        MajorGridlineColor = OxyColor.Parse(lightControlColor.ToString()),
        MinorGridlineColor = OxyColor.Parse(lightControlColor.ToString()),
        TicklineColor = OxyColor.Parse(textForegroundColor.ToString()),
        Title = "kg",
        IsZoomEnabled = false,
        IsPanEnabled = false
      };

      m_Plot.Axes.Add(valueAxis);
    }

    private void GenerateGraphData(IEnumerable<MeasureViewModel> measures) // Erzeugt Daten für organisatorische Grafik
    {
      var columnDelayed = new ColumnSeries
      {
        StrokeThickness = 0,
        Title = "Verspätet abgeschlossen",
        FillColor = OxyColors.IndianRed,
        IsStacked = true,
        StrokeColor = OxyColors.Red
      };

      var columnCompleted = new ColumnSeries
      {
        StrokeThickness = 0,
        Title = "Taggenau abgeschlossen",
        FillColor = OxyColors.GreenYellow,
        IsStacked = true,
        StrokeColor = OxyColors.GreenYellow
      };

      var columnCompletedPrior = new ColumnSeries
      {
        StrokeThickness = 0,
        Title = "Fristgerecht abgeschlossen",
        FillColor = OxyColors.Green,
        IsStacked = true,
        StrokeColor = OxyColors.Green
      };

      var columnPlanned = new ColumnSeries
      {
        StrokeThickness = 0,
        Title = "In Planung",
        FillColor = OxyColors.Yellow,
        IsStacked = false,
        StrokeColor = OxyColors.Yellow,
      };

      var counter = 0;
      var total = 0;

      var lastDayofMonth = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddMonths(1)
                                                                                     .Subtract(new TimeSpan(0, 0, 0, 1));

      var sortedList = measures.Where(
        m => (m.DueDate <= (lastDayofMonth)) && (m.DueDate >= (lastDayofMonth.Subtract(new TimeSpan(200, 0, 0, 0)))))
                               .OrderBy(m => m.DueDate);

      var monthArray = new int[6];
      var k = 0;

      for (var i = -5; i < 1; i++)
      {
        monthArray[k++] = DateTime.Now.AddMonths(i)
                                  .Month;
      }

      foreach (var month in monthArray)
      {
        var completed = measures.Count(m => m.Status == 2 && m.DueDate.Month == m.EntryDate.GetValueOrDefault()
                                                                                 .Month && m.EntryDate <= m.DueDate &&
                                            m.EntryDate.GetValueOrDefault()
                                             .Month == month);
        var delayed = measures.Count(m => m.Status == 2 && m.EntryDate > m.DueDate && m.EntryDate.GetValueOrDefault()
                                                                                       .Month == month);
        var completedPrior = measures.Count(m => m.Status == 2 && m.DueDate.Month > m.EntryDate.GetValueOrDefault()
                                                                                     .Month && m.EntryDate <= m.DueDate &&
                                                 m.EntryDate.GetValueOrDefault()
                                                  .Month == month);

        var planned = measures.Count(m => m.DueDate.Month == month);

        columnCompleted.Items.Add(new ColumnItem(completed, counter));
        columnDelayed.Items.Add(new ColumnItem(delayed, counter));

        columnPlanned.Items.Add(new ColumnItem(planned, counter));
        columnCompletedPrior.Items.Add(new ColumnItem(completedPrior, counter));

        total = total + completed + completedPrior + planned + delayed;

        counter++;
      }

      //if (total == 0)
      //{
      //  VisiblePlot = true;
      //}

      m_Plot.Series.Add(columnCompleted);
      m_Plot.Series.Add(columnCompletedPrior);
      m_Plot.Series.Add(columnDelayed);
      m_Plot.Series.Add(columnPlanned);
    }

    private void GenerateEcoGraphData(IEnumerable<MeasureViewModel> measures)
      // Erzeugt Daten für erste ökologische Grafik -> finanzrelevantes Balkendiagramm
    {
      var columnSavingEuro = new ColumnSeries
      {
        StrokeThickness = 0,
        Title = "Euro Einsparung",
        FillColor = OxyColors.IndianRed,
        IsStacked = false,
        StrokeColor = OxyColors.Red
      };

      var columnInvestment = new ColumnSeries
      {
        StrokeThickness = 0,
        Title = TranslationProvider.Translate("Investment"),
        FillColor = OxyColors.Green,
        IsStacked = false,
        StrokeColor = OxyColors.Green
      };

      double sumSavedMoney = 0;
      double investment = 0;

      foreach (var measure in measures)
      {
        sumSavedMoney += measure.SavedMoneySoll;
        investment += measure.Investment;
      }

      columnSavingEuro.Items.Add(new ColumnItem((int) sumSavedMoney, 0));
      columnInvestment.Items.Add(new ColumnItem((int) investment, 1));

      m_Plot.Series.Add(columnSavingEuro);
      m_Plot.Series.Add(columnInvestment);
    }

    private void GenerateEcoGraphData2(IEnumerable<MeasureViewModel> measures)
      // Erzeugt Daten für erste ökologische Grafik -> CO2 Balkendiagramm
    {
      var columnSavingCO2 = new ColumnSeries
      {
        StrokeThickness = 0,
        Title = "CO2 Einsparung",
        FillColor = OxyColors.GreenYellow,
        IsStacked = false,
        StrokeColor = OxyColors.GreenYellow
      };

      var counter = 0;
      double savedC02 = 0;
      foreach (var measure in measures)
      {
        savedC02 += measure.SavedCO2;

        counter++;
      }
      columnSavingCO2.Items.Add(new ColumnItem((int) savedC02, 0));

      m_Plot.Series.Add(columnSavingCO2);
    }

    #endregion
  }
}