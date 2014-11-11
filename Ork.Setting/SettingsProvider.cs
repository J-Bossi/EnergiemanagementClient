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
using System.ComponentModel.Composition;
using System.Globalization;
using System.Reflection;
using Ork.Setting.Properties;

namespace Ork.Setting
{
  [Export(typeof (ISettingsProvider))]
  public class SettingsProvider : ISettingsProvider
  {
    public string ConnectionString
    {
      get { return "http://" + Settings.Default.url + ":" + Settings.Default.port; }
    }

    public string User
    {
      get { return Settings.Default.user; }
    }

    public string Password
    {
      get { return Settings.Default.password; }
    }

    public string Url
    {
      get { return Settings.Default.url; }
    }

    public int Port
    {
      get { return Settings.Default.port; }
    }

    public string Language
    {
      get { return Settings.Default.language; }
    }

    public string Theme
    {
      get { return Settings.Default.theme; }
    }

    public void Refresh()
    {
      //hack: fast and works
      if (ConnectionStringUpdated != null)
      {
        ConnectionStringUpdated(this, new EventArgs());
      }
    }

    public event EventHandler<EventArgs> ConnectionStringUpdated;
    public event EventHandler<EventArgs> LanguageChanged;
    public event EventHandler<EventArgs> ThemeChanged;

    [Export]
    public void NewConnectionSettings(string serverUrl, int serverPort, string user, string password)
    {
      if (string.IsNullOrEmpty(serverUrl))
      {
        throw new ArgumentNullException("serverUrl");
      }
      if (string.IsNullOrEmpty(password))
      {
        throw new ArgumentNullException("password");
      }
      if (string.IsNullOrEmpty(user))
      {
        throw new ArgumentNullException("user");
      }
      if (serverPort <= 0)
      {
        throw new ArgumentOutOfRangeException("serverPort");
      }

      Settings.Default.url = serverUrl;
      Settings.Default.user = user;
      Settings.Default.password = password;
      Settings.Default.port = serverPort;
      Settings.Default.Save();
      if (ConnectionStringUpdated != null)
      {
        ConnectionStringUpdated(this, new EventArgs());
      }
    }

    [Export]
    public void NewLanguageSettings(CultureInfo language)
    {
      Settings.Default.language = language.Name;
      Settings.Default.Save();

      if (LanguageChanged != null)
      {
        LanguageChanged(this, new EventArgs());
      }
    }

    [Export]
    public void NewThemeSettings(Assembly theme)
    {
      Settings.Default.theme = theme.GetName()
                                    .Name;
      Settings.Default.Save();

      if (ThemeChanged != null)
      {
        ThemeChanged(this, new EventArgs());
      }
    }
  }
}