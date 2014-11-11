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
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using Ork.Framework.Controls;

namespace Ork.Framework.Converters
{
  [ValueConversion(typeof (bool), typeof (Thickness))]
  public class PinnedToThicknessConverter : IMultiValueConverter
  {
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
      var flyout = (Flyout) values[0];
      var control = (FrameworkElement) values[1];
      var oldMargin = control.Margin;
      if (flyout.Pinned)
      {
        if (flyout.HorizontalAlignment == HorizontalAlignment.Left)
        {
          return new Thickness(flyout.FlyoutWidth, oldMargin.Top, oldMargin.Right, oldMargin.Bottom);
        }
        else if (flyout.HorizontalAlignment == HorizontalAlignment.Right)
        {
          return new Thickness(oldMargin.Left, oldMargin.Top, flyout.FlyoutWidth, oldMargin.Bottom);
        }
      }
      else
      {
        if (flyout.HorizontalAlignment == HorizontalAlignment.Left)
        {
          return new Thickness(0, oldMargin.Top, oldMargin.Right, oldMargin.Bottom);
        }
        else if (flyout.HorizontalAlignment == HorizontalAlignment.Right)
        {
          return new Thickness(oldMargin.Left, oldMargin.Top, 0, oldMargin.Bottom);
        }
      }

      return new Thickness(0, 0, 0, 0);
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}