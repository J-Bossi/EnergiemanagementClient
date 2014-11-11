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

using System.Reflection;
using WPFLocalizeExtension.Extensions;

namespace Ork.Framework
{
  public static class TranslationProvider
  {
    private static LocExtension m_loc = new LocExtension();

    public static string Translate(Assembly assembly, string key)
    {
      string returnValue;
      m_loc.Key = assembly + ":Translate:" + key;
      m_loc.ResolveLocalizedValue(out returnValue);
      if (returnValue == null)
      {
        return "Key:" + key;
      }
      return returnValue;
    }

    public static string Translate(string key)
    {
      var callingAssembly = Assembly.GetCallingAssembly();
      return Translate(callingAssembly, key);
    }
  }
}