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
using Ork.Energy.DomainModelService;

namespace Ork.Energy.ViewModels
{
  public class ReadingViewModel
  {
    private readonly Reading m_Model;

    public ReadingViewModel(Reading reading)
    {
      m_Model = reading;
    }

    public Reading Model
    {
      get { return m_Model; }
    }

    public long CounterReading
    {
      get { return m_Model.CounterReading; }
    }

    public DateTime ReadingDate
    {
      get { return m_Model.ReadingDate; }
    }

    public string ShortReadingDate
    {
      get { return m_Model.ReadingDate.ToShortDateString(); }
    }
  }
}