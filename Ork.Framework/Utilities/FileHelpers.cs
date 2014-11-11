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

using System.IO;
using Microsoft.Win32;

namespace Ork.Framework.Utilities
{
  public class FileHelpers
  {
    public static byte[] GetByeArrayFromUserSelectedFile(string dialogFilter)
    {
      var fileName = string.Empty;

      return GetByeArrayFromUserSelectedFile(dialogFilter, out fileName);
    }

    public static byte[] GetByeArrayFromUserSelectedFile(string dialogFilter, out string filename)
    {
      var dlg = new OpenFileDialog();

      filename = string.Empty;

      dlg.DefaultExt = "*.*";

      if (dialogFilter.Length > 0)
      {
        dlg.Filter = dialogFilter;
      }

      var result = dlg.ShowDialog();

      if (result != true)
      {
        return null;
      }

      filename = dlg.SafeFileName;
      var fileStream = new FileStream(dlg.FileName, FileMode.Open, FileAccess.Read);

      try
      {
        var length = (int) fileStream.Length;
        var buffer = new byte[length];
        var count = 0;
        var sum = 0;

        while ((count = fileStream.Read(buffer, sum, length - sum)) > 0)
        {
          sum += count;
        }

        return buffer;
      }
      finally
      {
        fileStream.Close();
      }
    }
  }
}