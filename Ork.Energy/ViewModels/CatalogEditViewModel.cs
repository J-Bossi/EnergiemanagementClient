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
using Ork.Framework;

namespace Ork.Energy.ViewModels
{
  public class CatalogEditViewModel : CatalogAddViewModel
  {
    private readonly Action m_RemoveCatalog;

    public CatalogEditViewModel(CatalogViewModel model, Action removeCatalogAction)
      : base(model.Model)
    {
      DisplayName = TranslationProvider.Translate("EditCatalog");
      CatalogViewModel = model;
      m_RemoveCatalog = removeCatalogAction;
    }


    public CatalogViewModel CatalogViewModel { get; set; }

    public void RemoveCatalog()
    {
      m_RemoveCatalog();
      
    }
  }
}