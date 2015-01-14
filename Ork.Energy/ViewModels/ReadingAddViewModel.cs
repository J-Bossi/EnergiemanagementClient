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
using Caliburn.Micro;

namespace Ork.Energy.ViewModels
{
  public class ReadingAddViewModel : Screen
  {
    public ReadingAddViewModel()
    {
      NewReadingDate = DateTime.Now;
    }

    public long NewCounterReading { get; set; }
    public DateTime NewReadingDate { get; set; }

    public void ClearReadingFields()
    {
      NewCounterReading = 0;
      NewReadingDate = DateTime.Now;
      NotifyOfPropertyChange(() => NewCounterReading);
      NotifyOfPropertyChange(() => NewReadingDate);
    }
  }
}