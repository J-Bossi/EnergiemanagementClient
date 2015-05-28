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
// Copyright (c) 2015, HTW Berlin

#endregion

using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;
using Caliburn.Micro;
using Ork.Energy.Domain.DomainModelService;
using Ork.Framework;
using Ork.Setting;

namespace Ork.RoomBook.ViewModels
{
  [Export(typeof (IWorkspace))]
  public class RoomManagementViewModel : DocumentBase, IWorkspace
  {
    private readonly IRoomRepository m_Repository;
    private readonly BindableCollection<RoomViewModel> m_Rooms = new BindableCollection<RoomViewModel>();
    private bool m_IsEnabled;
    private Visibility m_HaveEditsBeenMade = Visibility.Collapsed;
    private readonly ISettingsProvider m_SettingsProvider;
    private readonly BindableCollection<RoomUsage> m_RoomUsages = new BindableCollection<RoomUsage>();
      
    [ImportingConstructor]
    public RoomManagementViewModel([Import] IRoomRepository mRepository, [Import] IDialogManager dialogs, [Import] ISettingsProvider settingsProvider)
    {
      Dialogs = dialogs;
      m_Repository = mRepository;
      m_SettingsProvider = settingsProvider;
      m_Repository.ContextChanged += (s, e) => Application.Current.Dispatcher.Invoke(Reload);
      m_Repository.SaveCompleted += (s, e) => ShowInfoBox();
      Reload();
      m_Rooms.CollectionChanged += RoomsOnCollectionChanged;
    }

    public BindableCollection<RoomViewModel> Rooms
    {
      get { return m_Rooms; }
    }

    public void RefreshDataSource()
    {
      m_SettingsProvider.Refresh();
      HaveEditsBeenMade = Visibility.Collapsed;
    }
    private void ShowInfoBox()
    {
      HaveEditsBeenMade = Visibility.Visible;
    }

    public Visibility HaveEditsBeenMade
    {
      get { return m_HaveEditsBeenMade; }
      set
      {
        m_HaveEditsBeenMade = value;
        NotifyOfPropertyChange(() => HaveEditsBeenMade);
      }
    }

    public int Index
    {
      get { return 401; }
    }

    public bool IsEnabled
    {
      get { return m_IsEnabled; }
      private set
      {
        m_IsEnabled = value;
        NotifyOfPropertyChange(() => IsEnabled);
      }
    }

    public string Title
    {
      get { return "Raumbuch"; }
    }

    private void RoomsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs eventArgs)
    {
      switch (eventArgs.Action)
      {
        case NotifyCollectionChangedAction.Add:
          foreach (var room in eventArgs.NewItems.OfType<RoomViewModel>())
          {
            if (m_Repository.Rooms.Contains(room.Model)) {}
            else
            {
              m_Repository.Rooms.Add(room.Model);
            }
          }
          break;
        case NotifyCollectionChangedAction.Remove:
          foreach (var oldRoom in
            eventArgs.OldItems.OfType<RoomViewModel>())

          {
            m_Repository.Rooms.Remove(oldRoom.Model);
          }
          break;
      }
    }

    private void Reload()
    {
      IsEnabled = m_Repository.HasConnection;
      if (IsEnabled)
      {
        LoadRooms();
      
      }
    }


    private void LoadRooms()
    {
      m_Rooms.Clear();
      //m_Repository.Rooms.CollectionChanged += AlterRoomCollection;
      foreach (var room in m_Repository.Rooms)
      {
        m_Rooms.Add(new RoomViewModel(room, m_Repository));
      }
    }


    //private void AlterRoomCollection(object sender, NotifyCollectionChangedEventArgs eventArgs)
    //{
    //  switch (eventArgs.Action)
    //  {
    //    case NotifyCollectionChangedAction.Add:
    //      foreach (var room in eventArgs.NewItems.OfType<Room>())
    //      {
    //        m_Rooms.Add(new RoomViewModel(room));
    //      }
    //      break;
    //    case NotifyCollectionChangedAction.Remove:
    //      foreach (var oldRoom in
    //        eventArgs.OldItems.OfType<Room>()
    //                 .Select(room => m_Rooms.Single(c => c.Model == room)))
    //      {
    //        m_Rooms.Remove(oldRoom);
    //      }
    //      break;
    //  }
    //}

    public void DeleteRoom(object dataContext)
    {
      if (dataContext.GetType() == typeof (RoomViewModel))
      {
        m_Rooms.Remove((RoomViewModel) dataContext);
        NotifyOfPropertyChange(() => Rooms);
      }
    }

    public void Save(object dataContext)
    {
      m_Repository.Save();
    }

    public void Cancel(object dataContext)
    {
      m_Repository.ClearPendingChanges();
      NotifyOfPropertyChange(() => Rooms);
    }
  }
}