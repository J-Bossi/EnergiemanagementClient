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

namespace Ork.Framework.ViewModels
{
  public class SelectableItemViewModel : PropertyChangedBase
  {
    private bool m_IsSelected;

    public SelectableItemViewModel(bool isSelected)
    {
      m_IsSelected = isSelected;
    }

    public bool IsSelected
    {
      get { return m_IsSelected; }
      set
      {
        m_IsSelected = value;
        NotifyOfPropertyChange(() => IsSelected);
      }
    }
  }
}