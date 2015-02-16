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
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using Ork.Energy.Domain.DomainModelService;
using Ork.Energy.Factories;
using Ork.Framework;
using Ork.Framework.Utilities;

namespace Ork.Energy.ViewModels
{
  public class MeasureAddViewModel : DocumentBase
  {
    private string _newSubMeasureName;
    private ResponsibleSubjectViewModel _newSubMeasureResponsibleSubject;
    private IEnumerable<Catalog> m_Catalogs;
    private string m_ResponsibleSubjectSearchText = string.Empty;
    private Catalog m_SelectedCatalog;
    private ResponsibleSubjectViewModel m_SelectedResponsibleSubject;
    private readonly EnergyMeasure m_Model;
    private readonly IEnumerable m_Priorities;
    private readonly IEnergyRepository m_Repository;
    private readonly IEnumerable<ResponsibleSubjectViewModel> m_ResponsibleSubjects;
    private readonly ISubMeasureViewModelFactory m_SubMeasureViewModelFactory;
    private readonly IEnumerable<SubMeasureViewModel> m_SubMeasureViewModels;

    [ImportingConstructor]
    public MeasureAddViewModel(EnergyMeasure model, IEnumerable<ResponsibleSubjectViewModel> responsibleSubjectViewModels,
                               [Import] IEnergyRepository energyRepository,
                               [Import] ISubMeasureViewModelFactory subMeasureViewModelFactory)
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
      m_SubMeasureViewModelFactory = subMeasureViewModelFactory;
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

    //public IEnumerable<Catalog> Catalogs
    //{
    //    get { return m_Catalogs; }
    //}

    //public Catalog SelectedCatalog
    //{
    //    get { return m_SelectedCatalog; }
    //    set
    //    {
    //        m_SelectedCatalog = value;
    //        SetNewCatalog();
    //        NotifyOfPropertyChange(() => SelectedCatalog);
    //    }
    //}

    //private void SetNewCatalog()
    //{
    //    if (!m_Repository.Catalogs.Any(c => c.Measures.Contains(m_Model)))
    //    {
    //        return;
    //    }

    //    var oldCatalog = m_Repository.Catalogs.Single(c => c.Measures.Contains(m_Model));

    //    m_Repository.Catalogs.Single(c => c == SelectedCatalog).Measures.Add(m_Model);
    //    NotifyOfPropertyChange(() => SelectedCatalog);
    //}


    public IEnumerable<SubMeasureViewModel> SubMeasures
    {
      get
      {
        return m_Repository.SubMeasures.Where(smvm => smvm.ReleatedMeasure == m_Model)
                           .Select(m_SubMeasureViewModelFactory.CreateFromExisting);
      }
    }

    public string Name
    {
      get { return m_Model.Name; }
      set
      {
        m_Model.Name = value;
      }
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
      set
      {
        m_Model.Description = value;
      }
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
      set
      {
        m_Model.ConsumerUnit = value;
      }
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
      }
    }

    public ICollection<Consumer> Consumers
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
      set
      {
        m_Model.Parameter = value;
      }
    }

    public string MeterDevice // Property für verwendetes Messgerät
    {
      get { return m_Model.Meter; }
      set
      {
        m_Model.Meter = value;
      }
    }

    public double CostsNeeded // Property für nötige Investition btw GeplanteKosten
    {
      get { return m_Model.Investment; }
      set
      {
        m_Model.Investment = value;
      }
    }

    public double SavedMoneySoll // Property für Wert der Einsparung bzw GeldEinsparung
    {
      get { return m_Model.SavedMoneyShould; }
      set
      {
        m_Model.SavedMoneyShould = value;
      }
    }

    public double SavedMoneyIst // Property für Wert der Einsparung bzw. GeldEinsparung
    {
      get { return m_Model.SavedMoneyIs; }
      set
      {
        m_Model.SavedMoneyIs = value;
      }
    }

    public double SavedWattIst // Property für Wert der Einsparung in kWh (Ist)
    {
      get { return m_Model.SavedWattIs; }
      set
      {
        m_Model.SavedWattIs = value;
      }
    }

    public double SavedWattSoll // Property für Wert der Einsparung in kWh (Soll)
    {
      get { return m_Model.SavedWattShould; }
      set
      {
        m_Model.SavedWattShould = value;
      }
    }

    public double SavedMoneyAktuell // Property für aktuellen Ist-Verbrauchswert in €
    {
      get { return m_Model.SavedMoneyAtm; }
      set
      {
        m_Model.SavedMoneyAtm = value;
      }
    }

    public double SavedWattAktuell // Property für aktuellen Ist-Verbrauchswert in kWh
    {
      get { return m_Model.SavedWattAtm; }
      set
      {
        m_Model.SavedWattAtm = value;
      }
    }

    // Rechnung nun korrekt -> Automatische Werterscheinung, nachdem "nötige Investition" und "Wert der Einsparung" eingegeben sind
    // Dennoch Korrekturbedarf!
    //public double Amortisationtime // Property für Amortisationszeit
    //{
    //    get { return CostsNeeded / SavedMoneyIst; } // Einsparung_Soll oder Einsparung_Ist ???
    //    set
    //    {
    //        m_Model.PaybackTime = value;
    //    }
    //}

    public double FailureCosts // Property für Ausfallkosten
    {
      get { return m_Model.FailureMoney; }
      set
      {
        m_Model.FailureMoney = value;
      }
    }

    public string ReferenceTo // Property für Verweis
    {
      get { return m_Model.Reference; }
      set
      {
        m_Model.Reference = value;
      }
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

    public void RemoveSubMeasure(object dataContext)
    {
      var subMeasure = dataContext as SubMeasureViewModel;
      if (subMeasure != null)
      {
        m_Repository.SubMeasures.Remove(subMeasure.Model);
      }
      NotifyOfPropertyChange(() => SubMeasures);
    }

    private SubMeasureViewModel[] CreateSubMeasures()
    {
      return
        Enumerable.ToArray<SubMeasureViewModel>(m_Repository.SubMeasures.Select(m_SubMeasureViewModelFactory.CreateFromExisting));
    }

    public void AddSubMeasure()
    {
      var subMeasure = new SubMeasure
      {
        ReleatedMeasure = m_Model,
        Name = NewSubMeasureName,
        ResponsibleSubject = NewSubMeasureResponsibleSubject.Model
      };
      m_Repository.SubMeasures.Add(subMeasure);
      m_Repository.Save();
      NotifyOfPropertyChange(() => SubMeasures);
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
      if (e.PropertyName == "IsSelected")
      {
      }
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