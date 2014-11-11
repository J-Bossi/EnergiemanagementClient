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
  public class FileExtensionStyleConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      var fileExtension = value as string;

      if (fileExtension == null)
      {
        return null;
      }

      if (fileExtension.ToLower()
                       .Contains("doc") ||
          fileExtension.ToLower()
                       .Contains("text") ||
          fileExtension.ToLower()
                       .Contains("pdf"))
      {
        return (Style) Application.Current.TryFindResource("IconDocuments");
      }
      else if (fileExtension.ToLower()
                            .Contains("jpg") ||
               fileExtension.ToLower()
                            .Contains("bmp") ||
               fileExtension.ToLower()
                            .Contains("png") ||
               fileExtension.ToLower()
                            .Contains("gif"))
      {
        return (Style) Application.Current.TryFindResource("IconPicture");
      }
      else
      {
        return DependencyProperty.UnsetValue;
      }
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}