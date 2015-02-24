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
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using Caliburn.Micro;
using Ork.Energy.Domain.DomainModelService;
using Ork.Energy.Factories;
using Ork.Framework;
using Ork.Framework.Utilities;

namespace Ork.Energy.ViewModels
{
  public class MeasureAddViewModel : DocumentBase
  {
    private readonly EnergyMeasure m_Model;
    private readonly IEnumerable m_Priorities;


    private readonly IEnergyRepository m_Repository;
    private readonly IEnumerable<ResponsibleSubjectViewModel> m_ResponsibleSubjects;

    private readonly BindableCollection<SubMeasureViewModel> m_SubMeasureViewModels =
      new BindableCollection<SubMeasureViewModel>();

    private readonly IViewModelFactory m_ViewModelFactory;
    private string _newSubMeasureName;
    private ResponsibleSubjectViewModel _newSubMeasureResponsibleSubject;
    private double m_CalculatedConsumption;
    private IEnumerable<Catalog> m_Catalogs;
    private string m_ResponsibleSubjectSearchText = string.Empty;
    private Catalog m_SelectedCatalog;
    private ResponsibleSubjectViewModel m_SelectedResponsibleSubject;

    [ImportingConstructor]
    public MeasureAddViewModel(EnergyMeasure model, IEnumerable<ResponsibleSubjectViewModel> responsibleSubjectViewModels,
                               [Import] IEnergyRepository energyRepository, [Import] IViewModelFactory viewModelFactory)
    {
      m_Model = model;
      if (m_Model.CreationDate == new DateTime())
      {
        m_Model.CreationDate = DateTime.Now;
        m_Model.DueDate = DateTime.Now;
      }
      m_Priorities = Enum.GetValues(typeof (Priority));
      m_ResponsibleSubjects = responsibleSubjectViewModels;
      DisplayName = TranslationProvider.Translate("TitleMeasureAddViewModel");

      m_Repository = energyRepository;
      m_ViewModelFactory = viewModelFactory;

      m_CalculatedConsumption = CurrentConsumption - m_Model.SavedWattShould;
      LoadSubMeasures();
      m_SubMeasureViewModels.CollectionChanged += SubMeasuresOnCollectionChanged;
    }

    public IEnumerable<ResponsibleSubjectViewModel> AllResponsibleSubjects
    {
      get { return m_ResponsibleSubjects; }
    }

    public bool SubMeasureAddAllowed
    {
      get { return (!String.IsNullOrEmpty(NewSubMeasureName) && NewSubMeasureResponsibleSubject != null); }
    }

    public string NewSubMeasureName
    {
      get { return _newSubMeasureName; }
      set
      {
        _newSubMeasureName = value;
        NotifyOfPropertyChange(() => NewSubMeasureName);
        NotifyOfPropertyChange(() => SubMeasureAddAllowed);
      }
    }

    public ResponsibleSubjectViewModel NewSubMeasureResponsibleSubject
    {
      get { return _newSubMeasureResponsibleSubject; }
      set
      {
        _newSubMeasureResponsibleSubject = value;
        NotifyOfPropertyChange(() => NewSubMeasureResponsibleSubject);
        NotifyOfPropertyChange(() => SubMeasureAddAllowed);
      }
    }

    public IObservableCollection<SubMeasureViewModel> SubMeasures
    {
      get { return m_SubMeasureViewModels; }
    }

    public string Name
    {
      get { return m_Model.Name; }
      set { m_Model.Name = value; }
    }

    public MeasureImageSource MeasureImageSource
    {
      get { return m_Model.MeasureImageSource; }
      set
      {
        m_Model.MeasureImageSource = value;
        NotifyOfPropertyChange(() => MeasureImageSource);
      }
    }

    public byte[] Image
    {
      get
      {
        if (m_Model.MeasureImageSource != null &&
            m_Model.MeasureImageSource.BinarySource != null)
        {
          return m_Model.MeasureImageSource.BinarySource;
        }

        Stream imageStream = File.OpenRead(@".\Resources\Images\Camera.png");

        byte[] byteArray;
        using (var br = new BinaryReader(imageStream))
        {
          byteArray = br.ReadBytes((int) imageStream.Length);
          return byteArray;
        }
      }
    }

    public Document Document
    {
      get
      {
        if (m_Model.AttachedDocuments != null &&
            m_Model.AttachedDocuments.All(ad => ad.DocumentSource != null))
        {
          return m_Model.AttachedDocuments.First();
        }
        return null;
      }
    }

    public IEnumerable<Document> AttachedDocuments
    {
      get
      {
        if (m_Model.AttachedDocuments != null)
        {
          return m_Model.AttachedDocuments;
        }
        return null;
      }
    }

    public string Description
    {
      get { return m_Model.Description; }
      set { m_Model.Description = value; }
    }

    public DateTime DueDate
    {
      get { return m_Model.DueDate; }
      set { m_Model.DueDate = value; }
    }

    public string DueDateString
    {
      get { return m_Model.DueDate.ToShortDateString(); }
    }

    public string Room
    {
      get
      {
        return m_Model.Consumer != null && m_Model.Consumer.Room != null
          ? m_Model.Consumer.Room.RoomNumber
          : "Kein Raum verfügbar";
      }
    }

    public string Building
    {
      get
      {
        return m_Model.Consumer != null && m_Model.Consumer.Room != null
          ? m_Model.Consumer.Room.Building
          : "Kein Gebäude verfügbar";
      }
    }

    public string ConsumerUnit // Property für Verbraucher, z.B: M25
    {
      get { return m_Model.ConsumerUnit; }
      set { m_Model.ConsumerUnit = value; }
    }

    public Consumer Consumer
    {
      get { return m_Model.Consumer; }
      set
      {
        m_Model.Consumer = value;

        NotifyOfPropertyChange(() => Room);
        NotifyOfPropertyChange(() => Building);
        NotifyOfPropertyChange(() => ConsumerGroupName);
        ////NotifyOfPropertyChange(() => AllRelatedReadings);
        //NotifyOfPropertyChange(() => ActualConsumptionReading);
        //NotifyOfPropertyChange(() => CurrentConsumptionReading);
        //NotifyOfPropertyChange(() => ActualConsumptionSaving);
        //NotifyOfPropertyChange(() => CalculatedConsumptionSaving);
        ////NotifyOfPropertyChange(() => AllRelatedReadings);
      }
    }

    public IEnumerable<Consumer> Consumers
    {
      get { return m_Repository.Consumers; }
    }

    public string ConsumerGroupName
    {
      get
      {
        if (m_Model.Consumer != null)
        {
          return m_Model.Consumer.ConsumerGroup.GroupName;
        }
        return "";
      }
    }

    public string Kenn // Property für Kenngröße (Einheit EnPI), z.B: kWh
    {
      get { return m_Model.Parameter; }
      set { m_Model.Parameter = value; }
    }

    public string MeterDevice // Property für verwendetes Messgerät
    {
      get { return m_Model.Meter; }
      set { m_Model.Meter = value; }
    }

    public double CostsNeeded // Property für nötige Investition btw GeplanteKosten
    {
      get { return m_Model.Investment; }
      set { m_Model.Investment = value; }
    }

    public double CalculatedMoneySaving
    {
      get { return m_Model.SavedMoneyShould; }
    }

    public double CalculatedConsumptionSaving
    {
      get { return m_Model.SavedWattShould; }
    }

    public double CalculatedSpending
    {
      get { return CurrentSpending - CalculatedMoneySaving; }
      set { m_Model.SavedMoneyShould = CurrentSpending - value; }
    }

    public double ActualMoneySaving
    {
      get { return CurrentSpending - ActualSpending; }
    }

    public double CalculatedConsumption
    {
      get { return m_CalculatedConsumption; }
      set
      {
        m_CalculatedConsumption = value;
        m_Model.SavedWattShould = CurrentConsumption - value;
        NotifyOfPropertyChange(() => CalculatedConsumptionSaving);
      }
    }

    public double ActualConsumptionSaving
    {
      get
      {
        if (CurrentConsumption == 0)
        {
          return 0;
        }
        return CurrentConsumption - ActualConsumption;
      }
    }

    public double ActualSpending // Tatsächlicher Verbauchswert nach Abschluss der Maßnahme
    {
      get { return m_Model.SavedMoneyIs; }
      set { m_Model.SavedMoneyIs = value; }
    }

    public double CurrentConsumption
    {
      get
      {
        if (m_Model.ConsumptionActual != null)
        {
          return m_Model.ConsumptionActual.CounterReading;
        }
        return 0;
      }
    }

    public double ActualConsumption
    {
      get
      {
        if (m_Model.ConsumptionNormative != null)
        {
          return m_Model.ConsumptionNormative.CounterReading;
        }
        return 0;
      }
    }

    public Reading ActualConsumptionReading
    {
      get { return AllRelatedReadings.Single(r => r == m_Model.ConsumptionNormative); }

      set
      {
        m_Model.ConsumptionNormative = value;
        NotifyOfPropertyChange(() => ActualConsumptionSaving);
        NotifyOfPropertyChange(() => CalculatedConsumptionSaving);
        NotifyOfPropertyChange(() => SavedCO2);
      }
    }

    public Reading CurrentConsumptionReading
    {
      get { return AllRelatedReadings.Single(r => r == m_Model.ConsumptionActual); }
      set
      {
        m_Model.ConsumptionActual = value;
        NotifyOfPropertyChange(() => ActualConsumptionSaving);
        m_Model.SavedWattShould = value.CounterReading - m_CalculatedConsumption;
        NotifyOfPropertyChange(() => CalculatedConsumptionSaving);
        NotifyOfPropertyChange(() => CurrentConsumptionReading);
      }
    }


    public double CurrentSpending // Property für aktuellen Ist-Verbrauchswert in €
    {
      get { return m_Model.SavedMoneyAtm; }
      set { m_Model.SavedMoneyAtm = value; }
    }

    public double SavedCO2
    {
      get { return ActualConsumption * 0.61; }
    }

    //Amortisationszeit in Tagen (sic)
    public double AmortisationTime
    {
      get { return Math.Round((CostsNeeded + FailureCosts) / CalculatedMoneySaving * 365, 0); }
    }


    public double FailureCosts // Property für Ausfallkosten
    {
      get { return m_Model.FailureMoney; }
      set { m_Model.FailureMoney = value; }
    }


    public string ReferenceTo // Property für Verweis
    {
      get { return m_Model.Reference; }
      set { m_Model.Reference = value; }
    }

    public int Priority
    {
      get { return m_Model.Priority; }
      set { m_Model.Priority = value; }
    }

    public string PriorityName
    {
      get { return TranslationProvider.Translate(((Priority) Priority).ToString()); }
    }

    public IEnumerable Priorities
    {
      get { return m_Priorities; }
    }

    public EnergyMeasure Model
    {
      get { return m_Model; }
    }

    public string ResponsibleSubjectSearchText
    {
      get { return m_ResponsibleSubjectSearchText; }
      set
      {
        m_ResponsibleSubjectSearchText = value;
        NotifyOfPropertyChange(() => FilteredResponsibleSubjects);
      }
    }

    public IEnumerable<ResponsibleSubjectViewModel> FilteredResponsibleSubjects
    {
      get { return SearchInResponsibleObjectList(); }
    }

    public ResponsibleSubjectViewModel SelectedResponsibleSubject
    {
      get { return m_SelectedResponsibleSubject; }
      set
      {
        m_SelectedResponsibleSubject = value;
        m_Model.ResponsibleSubject = value.Model;
        NotifyOfPropertyChange(() => m_Model.ResponsibleSubject);
      }
    }

    public string ResponsibleSubjectName
    {
      get
      {
        if (SelectedResponsibleSubject.Model is Employee)
        {
          var employee = (Employee) SelectedResponsibleSubject.Model;
          return employee.FirstName + " " + employee.LastName;
        }
        else
        {
          var group = (EmployeeGroup) SelectedResponsibleSubject.Model;
          return group.Name;
        }
      }
    }

    public IEnumerable<Reading> AllRelatedReadings
    {
      get
      {
        if (m_Model.Consumer != null &&
            m_Model.Consumer.Readings != null)
        {
          return m_Model.Consumer.Readings;
        }
        return new List<Reading>();
      }
    }

    private void LoadSubMeasures()
    {
      foreach (var subMeasure in m_Repository.SubMeasures.Where(smvm => smvm.ReleatedMeasure == m_Model))
      {
        m_SubMeasureViewModels.Add(m_ViewModelFactory.CreateFromExisting(subMeasure));
      }
    }

    public void RemoveSubMeasure(object dataContext)
    {
      var subMeasure = dataContext as SubMeasureViewModel;
      if (subMeasure != null)
      {
        m_SubMeasureViewModels.Remove(subMeasure);
      }
      NotifyOfPropertyChange(() => SubMeasures);
    }

    public void AddSubMeasure()
    {
      var subMeasure = new SubMeasure
      {
        ReleatedMeasure = m_Model,
        Name = NewSubMeasureName,
        ResponsibleSubject = NewSubMeasureResponsibleSubject.Model
      };
      m_SubMeasureViewModels.Add(m_ViewModelFactory.CreateFromExisting(subMeasure));

      NotifyOfPropertyChange(() => SubMeasures);
    }

    private void SubMeasuresOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs eventArgs)
    {
      switch (eventArgs.Action)
      {
        case NotifyCollectionChangedAction.Add:
          foreach (var sm in eventArgs.NewItems.OfType<SubMeasureViewModel>())
          {
            if (m_Repository.SubMeasures.Contains(sm.Model)) {}
            else
            {
              m_Repository.SubMeasures.Add(sm.Model);
            }
          }
          break;
        case NotifyCollectionChangedAction.Remove:
          foreach (var oldsm in
            eventArgs.OldItems.OfType<SubMeasureViewModel>())
          {
            m_Repository.SubMeasures.Remove(oldsm.Model);
          }
          break;
      }
    }


    public void SetImage()
    {
      var buffer = FileHelpers.GetByeArrayFromUserSelectedFile("Image Files |*.jpeg;*.png;*.jpg;*.gif");

      if (buffer == null)
      {
        return;
      }

      MeasureImageSource = new MeasureImageSource
      {
        BinarySource = ImageHelpers.ResizeImage(buffer, 1134, 756, ImageFormat.Jpeg)
      };
      NotifyOfPropertyChange(() => Image);
    }

    public void DeleteImage()
    {
      if (m_Model.MeasureImageSource == null ||
          m_Model.MeasureImageSource.BinarySource == null)
      {
        return;
      }

      m_Model.MeasureImageSource = null;
      NotifyOfPropertyChange(() => Image);
    }

    public void AddDocument()
    {
      var filename = string.Empty;
      var binarySourceDocument = FileHelpers.GetByeArrayFromUserSelectedFile(string.Empty, out filename);

      if (binarySourceDocument != null)
      {
        m_Model.AttachedDocuments.Add(new Document
        {
          DocumentSource = new DocumentSource
          {
            BinarySource = binarySourceDocument
          },
          Name = filename
        });

        NotifyOfPropertyChange(() => AttachedDocuments);
      }
    }

    public void OpenDocument(Document context)
    {
      File.WriteAllBytes(Path.GetTempPath() + context.Name, context.DocumentSource.BinarySource);

      Process.Start(Path.GetTempPath() + context.Name);
    }

    public void DeleteDocument(Document context)
    {
      m_Model.AttachedDocuments.Remove(context);
      NotifyOfPropertyChange(() => AttachedDocuments);
    }

    protected void RelatedElementViewModelIsSelected(object sender, PropertyChangedEventArgs e)
    {
      if (e.PropertyName == "IsSelected") {}
    }

    private IEnumerable<ResponsibleSubjectViewModel> SearchInResponsibleObjectList()
    {
      if (string.IsNullOrEmpty(ResponsibleSubjectSearchText))
      {
        return m_ResponsibleSubjects;
      }
      var searchText = ResponsibleSubjectSearchText.ToLower();

      var searchResultResponsibleSubjects = m_ResponsibleSubjects.Where(e => (e.Infotext != null) && (e.Infotext.ToLower()
                                                                                                       .Contains(
                                                                                                         searchText.ToLower())));

      return searchResultResponsibleSubjects;
    }
  }
}