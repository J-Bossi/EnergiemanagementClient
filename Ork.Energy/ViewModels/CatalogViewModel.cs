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

using System.Data.Services.Client;
using System.Linq;
using Ork.Energy.DomainModelService;

namespace Ork.Energy.ViewModels
{
  public class CatalogViewModel
  {
    private readonly Catalog m_Model;

    public CatalogViewModel(Catalog catalog)
    {
      m_Model = catalog;
    }

    public Catalog Model
    {
      get { return m_Model; }
    }

    public string Name
    {
      get { return m_Model.Name; }
    }

    public string Description
    {
      get { return m_Model.Description; }
    }

    public DataServiceCollection<DomainModelService.Measure> Measures
    {
      get { return m_Model.Measures; }
    }

    public string FullDate
    {
      get
      {
        var dateList = m_Model.Measures.Select(measure => measure.DueDate)
                              .ToList();
        dateList.Sort();

        if (dateList.Count == 0)
        {
          return "";
        }
        else
        {
          return dateList.First()
                         .ToShortDateString() + " - " + dateList.Last()
                                                                .ToShortDateString();
        }
      }
    }
  }
}