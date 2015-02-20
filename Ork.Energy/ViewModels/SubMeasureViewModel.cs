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

using Caliburn.Micro;
using Ork.Energy.Domain.DomainModelService;
using Ork.Energy.Factories;

namespace Ork.Energy.ViewModels
{
  public class SubMeasureViewModel : PropertyChangedBase
  {
    private readonly SubMeasure m_Model;
    private readonly IResponsibleSubjectViewModelFactory m_ResponsibleSubjectViewModelFactory;


    public SubMeasureViewModel(SubMeasure submeasure, IResponsibleSubjectViewModelFactory responsibleSubjectViewModelFactory)
    {
      m_Model = submeasure;
      m_ResponsibleSubjectViewModelFactory = responsibleSubjectViewModelFactory;
    }


    public int Id
    {
      get { return m_Model.Id; }
    }

    public string Name
    {
      get { return m_Model.Name; }
      set { m_Model.Name = value; }
    }

    public ResponsibleSubjectViewModel ResponsibleSubject
    {
      get { return m_ResponsibleSubjectViewModelFactory.CreateFromExisting(m_Model.ResponsibleSubject); }
      set { m_Model.ResponsibleSubject = (value).Model; }
    }

    public SubMeasure Model
    {
      get { return m_Model; }
    }

    public bool IsCompleted
    {
      get { return m_Model.IsCompleted; }
      set
      {
        m_Model.IsCompleted = value;
        NotifyOfPropertyChange(() => IsCompleted);
      }
    }

    public Measure RelatedMeasure
    {
      get { return m_Model.ReleatedMeasure; }
    }
  }
}