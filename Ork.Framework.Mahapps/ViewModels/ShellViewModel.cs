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

using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Caliburn.Micro;

namespace Ork.Framework.ViewModels
{
  [Export(typeof (Ork.Framework.Mahapps.IShell))]
  public class ShellViewModel : Conductor<IWorkspace>.Collection.OneActive, IShell
  {
      int m_TabIndex = -1;

    [ImportingConstructor]
    public ShellViewModel([Import] IDialogManager dialogManager, [Import("ApplicationName")] string appName)
    {
      DisplayName = appName + " aslfj";
      Dialogs = dialogManager;
      
      //ActivateItem(Items.First(i => i.IsEnabled));
      CloseStrategy = new ApplicationCloseStrategy();
    }

    [ImportMany]
    private IEnumerable<IWorkspace> Workspaces
    {
      set
      {
        Items.AddRange(value.OrderBy(index => index.Index));
        foreach (Screen item in Items)
        {
          item.Parent = this;
        }
      }
    }

      public int InitialTabSelection
      {
          get { return m_TabIndex; }
          set { m_TabIndex = value; }
      }

    public IDialogManager Dialogs { get; private set; }
  }
}