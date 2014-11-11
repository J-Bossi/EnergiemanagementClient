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

using System.ComponentModel.Composition;
using System.Windows.Input;
using Ork.Framework;

namespace Ork.About.ViewModels
{
  [Export(typeof (IWorkspace))]
  public class AboutViewModel : LocalizableScreen, IWorkspace
  {
    public AboutViewModel()
    {
      OpenInBrowser = new OpenUrlInBrowserCommand();
    }

    public ICommand OpenInBrowser { get; private set; }

    public int Index
    {
      get { return 9999; }
    }

    public bool IsEnabled
    {
      get { return true; }
    }

    public string Title
    {
      get { return "Info"; }
    }
  }
}