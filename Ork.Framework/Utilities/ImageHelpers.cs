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
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace Ork.Framework.Utilities
{
  public class ImageHelpers
  {
    public static byte[] ResizeImage(byte[] binarySource, int width, int height, ImageFormat imageFormat)
    {
      if (binarySource == null)
      {
        return null;
      }

      var memoryStream = new MemoryStream(binarySource);
      var fullsizeImage = Image.FromStream(memoryStream);
      var newImage = fullsizeImage.GetThumbnailImage(width, height, null, IntPtr.Zero);
      var myResult = new MemoryStream();
      newImage.Save(myResult, imageFormat);
      return myResult.ToArray();
    }
  }
}