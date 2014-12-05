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
// Copyright (c) 2013, HTW Berlin

#endregion

using System;
using System.ComponentModel.Composition;
using System.Data.Services.Client;
using Ork.Energy.DomainModelService;
using Ork.Setting;

namespace Ork.Energy
{
  [Export(typeof (IMeasureRepository))]
  internal class MeasureRepository : IMeasureRepository
  {
    private readonly Func<DomainModelContext> m_CreateMethod;
    private DomainModelContext m_Context;

    [ImportingConstructor]
    public MeasureRepository([Import] ISettingsProvider settingsContainer, [Import] Func<DomainModelContext> createMethod)
    {
      m_CreateMethod = createMethod;
      settingsContainer.ConnectionStringUpdated += (s, e) => Initialize();
      Initialize();
    }

    public bool HasConnection { get; private set; }
    public DataServiceCollection<ResponsibleSubject> ResponsibleSubjects { get; private set; }
    public DataServiceCollection<Catalog> Catalogs { get; private set; }
    //public DataServiceCollection<MeasureImageSource> MeasureImageSource { get; private set; } 


    //public DataServiceCollection<DomainModelService.Measure> Measures { get; private set; }
    public event EventHandler ContextChanged;
    public event EventHandler SaveCompleted;


    public void Save()
    {
      if (m_Context.ApplyingChanges)
      {
        return;
      }

      var result = m_Context.BeginSaveChanges(SaveChangesOptions.Batch, r =>
                                                                        {
                                                                          var dm = (DomainModelContext) r.AsyncState;
                                                                          dm.EndSaveChanges(r);
                                                                          RaiseEvent(SaveCompleted);
                                                                        }, m_Context);
    }


    private void Initialize()
    {
      m_Context = m_CreateMethod();

      try
      {
        LoadResponsibleSubjects();
        //LoadMeasures();
        LoadCatalogs();
        LoadSubMeasures();
        //LoadMeasureImageSources();
        HasConnection = true;
      }
      catch (Exception e)
      {
        HasConnection = false;
      }

      RaiseEvent(ContextChanged);
    }

    private void LoadResponsibleSubjects()
    {
      ResponsibleSubjects = new DataServiceCollection<ResponsibleSubject>(m_Context);

      var query = m_Context.ResponsibleSubjects.Expand("OpenResKit.DomainModel.Employee/Groups");
      ResponsibleSubjects.Load(query);
    }

    private void LoadCatalogs()
    {
      Catalogs = new DataServiceCollection<Catalog>(m_Context);

      var query = m_Context.Catalogs.Expand("Measures/OpenResKit.DomainModel.Measure/ResponsibleSubject")
                           .Expand("Measures/OpenResKit.DomainModel.Measure/MeasureImageSource")
                           .Expand("Measures/OpenResKit.DomainModel.Measure/AttachedDocuments/DocumentSource");
      Catalogs.Load(query);
    }

    private void LoadSubMeasures()
    {
        SubMeasures = new DataServiceCollection<SubMeasure>(m_Context);

        var query = m_Context.SubMeasures.Expand("OpenResKit.DomainModel.SubMeasure/ReleatedMeasure").Expand("OpenResKit.DomainModel.SubMeasure/ResponsibleSubject");
        SubMeasures.Load(query);
    }


    private void RaiseEvent(EventHandler eventHandler)
    {
      if (eventHandler != null)
      {
        eventHandler(this, new EventArgs());
      }
    }
    public DataServiceCollection<SubMeasure> SubMeasures { get; private set; }
  }
}