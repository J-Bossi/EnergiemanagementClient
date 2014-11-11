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
using Caliburn.Micro;

namespace Ork.Framework.ViewModels
{
  [Export(typeof (IMessageBox))]
  [PartCreationPolicy(CreationPolicy.NonShared)]
  public class MessageBoxViewModel : Screen, IMessageBox
  {
    private MessageBoxOptions selection;

    public bool CancelVisible
    {
      get { return IsVisible(MessageBoxOptions.Cancel); }
    }

    public bool NoVisible
    {
      get { return IsVisible(MessageBoxOptions.No); }
    }

    public bool OkVisible
    {
      get { return IsVisible(MessageBoxOptions.Ok); }
    }

    public bool YesVisible
    {
      get { return IsVisible(MessageBoxOptions.Yes); }
    }

    public string Message { get; set; }
    public MessageBoxOptions Options { get; set; }

    public void Cancel()
    {
      Select(MessageBoxOptions.Cancel);
    }

    public void No()
    {
      Select(MessageBoxOptions.No);
    }

    public void Ok()
    {
      Select(MessageBoxOptions.Ok);
    }

    public bool WasSelected(MessageBoxOptions option)
    {
      return (selection & option) == option;
    }

    public void Yes()
    {
      Select(MessageBoxOptions.Yes);
    }

    private bool IsVisible(MessageBoxOptions option)
    {
      return (Options & option) == option;
    }

    private void Select(MessageBoxOptions option)
    {
      selection = option;
      TryClose();
    }
  }
}