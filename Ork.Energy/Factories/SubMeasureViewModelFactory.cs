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

using System.ComponentModel.Composition;
using Ork.Energy.DomainModelService;
using Ork.Energy.ViewModels;

namespace Ork.Energy.Factories
{
  [Export(typeof (ISubMeasureViewModelFactory))]
  internal class SubMeasureViewModelFactory : ISubMeasureViewModelFactory
  {
    private readonly IResponsibleSubjectViewModelFactory m_ResponsibleSubjectViewModelFactory;

    [ImportingConstructor]
    public SubMeasureViewModelFactory([Import] IResponsibleSubjectViewModelFactory responsibleSubjectViewModelFactory)
    {
      m_ResponsibleSubjectViewModelFactory = responsibleSubjectViewModelFactory;
    }

    public SubMeasureViewModel CreateFromExisting(SubMeasure subMeasure)
    {
      return new SubMeasureViewModel(subMeasure, m_ResponsibleSubjectViewModelFactory);
    }
  }
}