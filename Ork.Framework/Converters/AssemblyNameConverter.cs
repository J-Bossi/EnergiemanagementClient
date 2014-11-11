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
  public class AssemblyNameConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (value == null)
      {
        return DependencyProperty.UnsetValue;
      }

      var returnValue = value as string;

      return returnValue != null
        ? returnValue.Remove(returnValue.IndexOf(",", StringComparison.Ordinal))
                     .Replace("Ork.Theme.", string.Empty)
        : null;

      //var assembly = value as Assembly;

      //if (assembly != null)
      //{
      //  return assembly.GetName()
      //                 .Name;
      //}
      //else
      //{
      //  return DependencyProperty.UnsetValue;
      //}
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return DependencyProperty.UnsetValue;
    }
  }
}