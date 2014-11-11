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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using System.Windows.Markup;
using Ork.Framework;
using WPFLocalizeExtension.Engine;

namespace Ork.Setting.ViewModels
{
  [Export(typeof (IWorkspace))]
  public class SettingViewModel : LocalizableScreen, IWorkspace
  {
    private readonly List<CultureInfo> m_AvailableLanguages;
    private readonly List<Assembly> m_AvailableThemes;
    private readonly ExampleDataProvider m_ExampleDataProvider;
    private readonly Action<string, int, string, string> m_NewConnectionSettings;
    private readonly Action<CultureInfo> m_NewLanguageSettings;
    private readonly Action<Assembly> m_NewThemeSettings;
    private readonly ISettingsProvider m_SettingsProvider;
    private readonly CultureInfo m_oldLanguage;
    private CultureInfo m_SelectedLanguage;
    private Assembly m_SelectedTheme;

    [ImportingConstructor]
    public SettingViewModel([Import] ExampleDataProvider exampleDateProvider, [Import] Action<string, int, string, string> newConnectionSettings, [Import] Action<CultureInfo> newLanguageSettings,
      [Import] Action<Assembly> newThemeSettings, [Import] IDialogManager dialogs, [Import] ISettingsProvider settingsProvider)
    {
      Dialogs = dialogs;
      m_NewConnectionSettings = newConnectionSettings;
      m_NewLanguageSettings = newLanguageSettings;
      m_NewThemeSettings = newThemeSettings;
      m_ExampleDataProvider = exampleDateProvider;
      m_ExampleDataProvider.SeedCompleted += (s, e) =>
                                             {
                                               Save();
                                               Mouse.OverrideCursor = null;
                                             };

      m_SettingsProvider = settingsProvider;

      ServerUrl = m_SettingsProvider.Url;
      ServerPort = m_SettingsProvider.Port;
      User = m_SettingsProvider.User;
      Password = m_SettingsProvider.Password;
      m_AvailableLanguages = new List<CultureInfo>();
      m_AvailableLanguages.Add(CultureInfo.GetCultureInfo("de-DE"));
      m_AvailableLanguages.Add(CultureInfo.GetCultureInfo("en-US"));
      SelectedLanguage = m_AvailableLanguages.Single(kvp => kvp.Name == m_SettingsProvider.Language);

      var directoryCatalog = new DirectoryCatalog("./", "Ork.Theme*.dll");
      m_AvailableThemes = directoryCatalog.LoadedFiles.Select(Assembly.LoadFrom)
                                          .ToList();
      SelectedTheme = m_AvailableThemes.Find(at => at.GetName()
                                                     .Name == m_SettingsProvider.Theme);
      FrameworkElement.LanguageProperty.OverrideMetadata(typeof (FrameworkElement), new FrameworkPropertyMetadata(XmlLanguage.GetLanguage(SelectedLanguage.IetfLanguageTag)));
      m_oldLanguage = SelectedLanguage;
    }

    public IEnumerable AvailableThemes
    {
      get { return m_AvailableThemes; }
    }

    public Assembly SelectedTheme
    {
      get { return m_SelectedTheme; }
      set
      {
        if (m_SelectedTheme == value)
        {
          return;
        }
        m_SelectedTheme = value;
        Application.Current.Resources.MergedDictionaries.Clear();

        var dict = new ResourceDictionary
                   {
                     Source = new Uri("/" + m_SelectedTheme.GetName()
                                                           .Name + ";component/Resources/ResourceLib.xaml", UriKind.Relative)
                   };

        Application.Current.Resources.MergedDictionaries.Add(dict);

        m_NewThemeSettings(SelectedTheme);
      }
    }

    public IEnumerable AvailableLanguages
    {
      get { return m_AvailableLanguages; }
    }

    public IDialogManager Dialogs { get; private set; }

    public int ServerPort { get; set; }
    public string ServerUrl { get; set; }
    public string User { get; set; }
    public string Password { get; set; }

    public CultureInfo SelectedLanguage
    {
      get { return m_SelectedLanguage; }
      set
      {
        if (Equals(m_SelectedLanguage, value))
        {
          return;
        }
        m_SelectedLanguage = value;
        LocalizeDictionary.Instance.Culture = SelectedLanguage;
        m_NewLanguageSettings(SelectedLanguage);
        NotifyOfPropertyChange(() => LanguageChangedContainerVisibility);
      }
    }

    public bool IsExampleDataButtonShown
    {
      get
      {
#if (DEBUG)

        return true;
#endif
        return false;
      }
    }

    public Visibility LanguageChangedContainerVisibility
    {
      get
      {
        if (SelectedLanguage != m_oldLanguage)
        {
          return Visibility.Visible;
        }
        return Visibility.Collapsed;
      }
    }

    public int Index
    {
      get { return 8000; }
    }

    public bool IsEnabled
    {
      get { return true; }
    }

    public string Title
    {
      get { return TranslationProvider.Translate("TitleSettingsViewModel"); }
    }

    public void RestartApplication()
    {
      Process.Start(Application.ResourceAssembly.Location);
      Application.Current.Shutdown();
    }

    public void Save()
    {
      m_NewConnectionSettings(ServerUrl, ServerPort, User, Password);
    }

    public void CreateExampleData()
    {
      m_ExampleDataProvider.CreateExampleData();
      Mouse.OverrideCursor = Cursors.Wait;
    }

    public void TestConnection()
    {
      var uri = new Uri("http://" + ServerUrl + ":" + ServerPort + "/OpenResKitHub");
      var request = WebRequest.Create(uri);
      var cc = new CredentialCache
               {
                 {
                   uri, "Basic", new NetworkCredential(User, Password)
                 }
               };
      request.Credentials = cc;

      request.BeginGetResponse(ar =>
                               {
                                 var req = (HttpWebRequest) ar.AsyncState;
                                 try
                                 {
                                   req.EndGetResponse(ar);
                                   Dialogs.ShowMessageBox(TranslationProvider.Translate("ConnectionSuccessfull"), TranslationProvider.Translate("Success"));
                                 }
                                 catch (WebException ex)
                                 {
                                   var message = ex.Message;
                                   if (ex.InnerException != null)
                                   {
                                     message += message + Environment.NewLine + ex.InnerException.Message;
                                   }
                                   Dialogs.ShowMessageBox(message, TranslationProvider.Translate("ConnectionError"));
                                 }
                               }, request);
    }
  }
}