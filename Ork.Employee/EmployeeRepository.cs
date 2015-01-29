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
using System.Linq;
using Caliburn.Micro;
using Ork.Employee.DomainModelService;
using Ork.Setting;

namespace Ork.Employee
{
  [Export(typeof (IEmployeeRepository))]
  public class EmployeeRepository : PropertyChangedBase, IEmployeeRepository
  {
    private readonly Func<DomainModelContext> m_CreateMethod;

    [ImportingConstructor]
    public EmployeeRepository([Import] ISettingsProvider settingsContainer, [Import] Func<DomainModelContext> createMethod)
    {
      m_CreateMethod = createMethod;
      settingsContainer.ConnectionStringUpdated += (s, e) => Initialize();

      Initialize();
    }

    public DomainModelContext Context { get; private set; }
    public bool HasConnection { get; private set; }
    public DataServiceCollection<ResponsibleSubject> ResponsibleSubjects { get; private set; }

    public event EventHandler ContextChanged;
    public event EventHandler SaveCompleted;

    public void Save()
    {
      if (Context.Entities.All(e => e.State == EntityStates.Unchanged) &&
          Context.Links.All(e => e.State == EntityStates.Unchanged))
      {
        return;
      }
      if (Context.ApplyingChanges)
      {
        return;
      }

      Context.BeginSaveChanges(SaveChangesOptions.Batch, r =>
                                                         {
                                                           var dm = (DomainModelContext) r.AsyncState;
                                                           dm.EndSaveChanges(r);
                                                           RaiseEvent(SaveCompleted, new EventArgs());
                                                           Initialize();
                                                         }, Context);
    }

    private void Initialize()
    {
      Context = m_CreateMethod();

      try
      {
        LoadResponsibleSubjects();
        HasConnection = true;
      }
      catch (Exception)
      {
        HasConnection = false;
      }

      RaiseEvent(ContextChanged, new EventArgs());
    }

    private void LoadResponsibleSubjects()
    {
      ResponsibleSubjects = new DataServiceCollection<ResponsibleSubject>(Context);
      var query = Context.ResponsibleSubjects.Expand("OpenResKit.DomainModel.Employee/Groups");
      ResponsibleSubjects.Load(query);
    }

    private void RaiseEvent(EventHandler eventHandler, EventArgs eventArgs)
    {
      if (eventHandler != null &&
          eventArgs != null)
      {
        eventHandler(this, eventArgs);
      }
    }
  }
}