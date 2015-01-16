
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
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Ork.Energy.DomainModelService;
using Ork.Energy.ViewModels;

namespace Ork.Energy.Factories
{
  [Export(typeof (IMeasureViewModelFactory))]
  internal class MeasureViewModelFactory : IMeasureViewModelFactory
  {
    private readonly IEnergyRepository m_MeasureRepository;
  
    private readonly IResponsibleSubjectViewModelFactory m_ResponsibleSubjectViewModelFactory;
    private readonly ISubMeasureViewModelFactory m_SubMeasureViewModelFactory;

    [ImportingConstructor]
    public MeasureViewModelFactory([Import] IEnergyRepository energyRepository, [Import] IResponsibleSubjectViewModelFactory responsibleSubjectViewModelFactory,
       [Import] ISubMeasureViewModelFactory subMeasureViewModelFactory)
    {
        m_MeasureRepository = energyRepository;
      m_ResponsibleSubjectViewModelFactory = responsibleSubjectViewModelFactory;

      m_SubMeasureViewModelFactory = subMeasureViewModelFactory;
    }

    public MeasureViewModel CreateFromExisting(DomainModelService.EnergyMeasure measure)
    {
      return new MeasureViewModel(measure);
    }

    public MeasurePrintPreviewViewModel CreatePrintPreviewModel(EnergyMeasure measure, Action removeMeasureAction)
      {
          return new MeasurePrintPreviewViewModel(measure, removeMeasureAction, CreateResponsibleSubjects(), m_MeasureRepository, m_SubMeasureViewModelFactory);
      }

      public MeasureAddViewModel CreateAddViewModel()
    {
      return new MeasureAddViewModel(new DomainModelService.EnergyMeasure(), CreateResponsibleSubjects(), m_MeasureRepository, m_SubMeasureViewModelFactory);
    }

      public MeasureEditViewModel CreateEditViewModel(DomainModelService.EnergyMeasure measure, Action removeMeasureAction)
    {

      return new MeasureEditViewModel(measure, removeMeasureAction, CreateResponsibleSubjects(),  m_MeasureRepository, m_SubMeasureViewModelFactory);
    }

    public CatalogAddViewModel CreateCatalogAddViewModel()
    {
      return new CatalogAddViewModel(new Catalog());
    }

    public CatalogEditViewModel CreateCatalogEditViewModel(CatalogViewModel catalogViewModel, Action removeCatalogAction)
    {
      return new CatalogEditViewModel(catalogViewModel, removeCatalogAction);
    }

    public CatalogViewModel CreateFromExisting(Catalog catalog)
    {
      return new CatalogViewModel(catalog);
    }

    private ResponsibleSubjectViewModel[] CreateResponsibleSubjects()
    {
      return m_MeasureRepository.ResponsibleSubjects.Select(m_ResponsibleSubjectViewModelFactory.CreateFromExisting)
                                .ToArray();
    }
  
  }
}