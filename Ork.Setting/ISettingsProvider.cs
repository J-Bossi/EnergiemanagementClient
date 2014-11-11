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

namespace Ork.Setting
{
  public interface ISettingsProvider
  {
    string ConnectionString { get; }
    string User { get; }
    string Password { get; }
    string Url { get; }
    int Port { get; }
    string Language { get; }
    string Theme { get; }

    void Refresh();

    event EventHandler<EventArgs> ConnectionStringUpdated;
    event EventHandler<EventArgs> LanguageChanged;
    event EventHandler<EventArgs> ThemeChanged;

    [Export]
    void NewConnectionSettings(string serverUrl, int serverPort, string user, string password);

    [Export]
    void NewLanguageSettings(CultureInfo language);

    [Export]
    void NewThemeSettings(Assembly theme);
  }
}