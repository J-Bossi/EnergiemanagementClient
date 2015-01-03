﻿#region License

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
// Copyright (c) 2014, HTW Berlin

#endregion

using System.Collections.Generic;
using Ork.Energy.DomainModelService;

namespace Ork.Energy.ViewModels
{
  public class ConsumerViewModel

  {
    private readonly Consumer m_Model;

    public ConsumerViewModel(Consumer consumer)
    {
      m_Model = consumer;
    }

    public Consumer Model
    {
      get { return m_Model; }
    }

    public virtual Room Room
    {
      get { return m_Model.Room; }
    }

    public virtual Distributor Distributor
    {
      get { return m_Model.Distributor; }
    }

    public string Name
    {
      get { return m_Model.Name; }
    }

    public virtual long? PowerOutput
    {
      get { return m_Model.PowerOutput; }
    }

    public virtual long? PowerCurrent
    {
      get { return m_Model.PowerCurrent; }
    }

    public virtual ICollection<Reading> Readings
    {
      get { return m_Model.Readings; }
    }

    public virtual string Comment
    {
      get { return m_Model.Comment; }
    }

    public int? Year
    {
      get { return m_Model.Year; }
    }

    public virtual string Manufacturer
    {
      get { return m_Model.Manufacturer; }
    }
  }
}