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

using Caliburn.Micro;
using Ork.Energy.DomainModelService;
using Ork.Framework;

namespace Ork.Energy.ViewModels
{
    public class CatalogAddViewModel : Screen
    {
        private readonly CatalogModifyViewModel m_Model;

        public CatalogAddViewModel(Catalog model)
        {
            DisplayName = TranslationProvider.Translate("AddCatalog");
            m_Model = new CatalogModifyViewModel(model);
        }

        public Catalog Model
        {
            get { return m_Model.Model; }
        }

        public string Name
        {
            get { return m_Model.Name; }
            set
            {
                m_Model.Name = value;
                NotifyOfPropertyChange(() => ValidateCatalog);
            }
        }

        public bool ValidateCatalog
        {
            get
            {
                if (!string.IsNullOrEmpty(Name))
                {
                    return true;
                }

                return false;
            }
        }

        public string Description
        {
            get { return m_Model.Description; }
            set { m_Model.Description = value; }
        }
    }
}