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

using System.Collections;
using System.Windows;
using System.Windows.Controls;

namespace Ork.Theme.Nova.Resources.CustomControls
{
  public static class ListBoxSelectionBehavior
  {
    public static bool GetClickSelection(DependencyObject obj)
    {
      return (bool) obj.GetValue(ClickSelectionProperty);
    }

    public static void SetClickSelection(DependencyObject obj, bool value)
    {
      obj.SetValue(ClickSelectionProperty, value);
    }

    private static void OnClickSelectionChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs e)
    {
      ListBox listBox = dpo as ListBox;
      if (listBox != null)
      {
        if ((bool) e.NewValue == true)
        {
          listBox.SelectionMode = SelectionMode.Multiple;
          listBox.SelectionChanged += OnSelectionChanged;
        }
        else
        {
          listBox.SelectionChanged -= OnSelectionChanged;
        }
      }
    }

    private static void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      if (e.AddedItems.Count > 0)
      {
        ListBox listBox = sender as ListBox;
        var valid = e.AddedItems[0];
        foreach (var item in new ArrayList(listBox.SelectedItems))
        {
          if (item != valid)
          {
            listBox.SelectedItems.Remove(item);
          }
        }
      }
    }

    public static readonly DependencyProperty ClickSelectionProperty = DependencyProperty.RegisterAttached("ClickSelection",
      typeof (bool), typeof (ListBoxSelectionBehavior), new UIPropertyMetadata(false, OnClickSelectionChanged));
  }
}