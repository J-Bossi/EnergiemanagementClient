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

namespace Ork.Framework.Converters
{
  public class BoolToVisibilityReversableConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if ((string) parameter == "true")
      {
        if ((bool) value)
        {
          return Visibility.Visible;
        }
        else
        {
          return Visibility.Collapsed;
        }
      }
      else
      {
        if ((bool) value)
        {
          return Visibility.Collapsed;
        }
        else
        {
          return Visibility.Visible;
        }
      }
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}