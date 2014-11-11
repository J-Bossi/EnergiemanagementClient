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
using System.Data.Services.Client;
using Ork.Employee.DomainModelService;

namespace Ork.Employee
{
  public interface IEmployeeRepository
  {
    DomainModelContext Context { get; }
    bool HasConnection { get; }
    DataServiceCollection<ResponsibleSubject> ResponsibleSubjects { get; }
    event EventHandler ContextChanged;
    event EventHandler SaveCompleted;
    void Save();
  }
}