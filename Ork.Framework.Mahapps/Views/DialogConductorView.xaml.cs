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
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Ork.Framework.Mahapps.Views
{
  public partial class DialogConductorView : UserControl
  {
    private bool disabled;

    public DialogConductorView()
    {
      InitializeComponent();
      ActiveItem.ContentChanged += OnTransitionCompleted;
      Loaded += OnLoad;
    }

    public void DisableBackground()
    {
      //disabled = true;
      //ChangeEnabledState(GetBackground(),
      //                   false);
    }

    public void EnableBackground()
    {
      //disabled = false;
      //ChangeEnabledState(GetBackground(),
      //                   true);
    }

    private void ChangeEnabledState(IEnumerable<UIElement> background, bool state)
    {
      foreach (var uiElement in background)
      {
        var control = uiElement as Control;
        if (control != null)
        {
          control.IsEnabled = state;
        }
        else
        {
          var panel = uiElement as Panel;
          if (panel != null)
          {
            foreach (UIElement child in panel.Children)
            {
              ChangeEnabledState(new[]
                                 {
                                   child
                                 }, state);
            }
          }
        }
      }
    }

    private IEnumerable<UIElement> GetBackground()
    {
      var contentControl = (ContentControl) Parent;
      var container = (Panel) contentControl.Parent;
      return container.Children.Cast<UIElement>()
                      .Where(child => child != contentControl);
    }

    private void OnLoad(object sender, RoutedEventArgs e)
    {
      if (disabled)
      {
        DisableBackground();
      }
    }

    private void OnTransitionCompleted(object sender, EventArgs e)
    {
      if (ActiveItem.Content == null)
      {
        EnableBackground();
      }
      else
      {
        DisableBackground();

        var control = ActiveItem.Content as Control;
        if (control != null)
        {
          control.Focus();
        }
      }
    }
  }
}