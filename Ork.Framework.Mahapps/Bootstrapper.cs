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
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.ReflectionModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows;
using Caliburn.Micro;
using Ork.Framework.Mahapps.ViewModels;

namespace Ork.Framework.Mahapps
{
  public class Bootstrapper : BootstrapperBase
  {
    private const string ConsolePath = @"./";
    private IEnumerable<Assembly> m_Assemblies;

    private CompositionContainer m_Container;
    private DirectoryCatalog m_DirectoryCatalog;
    private Mutex m_Mutex; //this have to stay here!

    public Bootstrapper()
    {
      Initialize();
    }

    public string ApplicationName
    {
      set
      {
        var batch = new CompositionBatch();
        batch.AddExportedValue("ApplicationName", value);
        m_Container.Compose(batch);
      }
    }

    protected override void OnStartup(object sender, StartupEventArgs e)
    {
      var assembly = Assembly.GetEntryAssembly();
      var mutexName = String.Format(CultureInfo.InvariantCulture, "Local\\{{{0}}}{{{1}}}", assembly.GetType()
                                                                                                   .GUID, assembly.GetName()
                                                                                                                  .Name);
      bool createdNew;
      m_Mutex = new Mutex(false, mutexName, out createdNew);
      if (!createdNew)
      {
        Application.Shutdown();
        return;
      }

      DisplayRootViewFor<ShellViewModel>();
    }

    protected override void BuildUp(object instance)
    {
      m_Container.SatisfyImportsOnce(instance);
    }

    protected override void Configure()
    {
      var assemblyCatalogs = m_Assemblies.Select(x => new AssemblyCatalog(x));
      var catalog = new AggregateCatalog(assemblyCatalogs);

      m_Container = new CompositionContainer(catalog);

      var batch = new CompositionBatch();
      batch.AddExportedValue<Func<IMessageBox>>(() => m_Container.GetExportedValue<IMessageBox>());
      batch.AddExportedValue<IWindowManager>(new WindowManager());
      batch.AddExportedValue<IEventAggregator>(new EventAggregator());
      batch.AddExportedValue(m_Container);

      m_Container.Compose(batch);
    }


    protected override IEnumerable<object> GetAllInstances(Type serviceType)
    {
      return m_Container.GetExportedValues<object>(AttributedModelServices.GetContractName(serviceType));
    }

    protected override object GetInstance(Type serviceType, string key)
    {
      var contract = string.IsNullOrEmpty(key)
        ? AttributedModelServices.GetContractName(serviceType)
        : key;
      var exports = m_Container.GetExportedValues<object>(contract);

      if (exports.Count() > 0)
      {
        return exports.First();
      }

      throw new Exception(string.Format("Could not locate any instances of contract {0}.", contract));
    }

    protected override IEnumerable<Assembly> SelectAssemblies()
    {
      m_DirectoryCatalog = new DirectoryCatalog(ConsolePath);
      m_Assemblies = m_DirectoryCatalog.Parts.Select(part => ReflectionModelServices.GetPartType(part)
                                                                                    .Value.Assembly)
                                       .Distinct();

      var assemblyNames = m_Assemblies.Select(a => a.ManifestModule.Name)
                                      .ToList();
      var assemblyList = new List<Assembly>
                         {
                           Assembly.GetEntryAssembly()
                         };

      foreach (var assembly in assemblyNames)
      {
        assemblyList.Add(Assembly.LoadFrom(assembly));
      }

      return assemblyList;
    }
  }
}