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
using System.ComponentModel.Composition;
using Caliburn.Micro;
using Ork.Energy.Domain.DomainModelService;
using Ork.Framework;

namespace Ork.RoomBook.ViewModels
{
  public class RoomViewModel : DocumentBase
  {
    private static readonly BindableCollection<RoomUsage> m_RoomUsages = new BindableCollection<RoomUsage>();
    private readonly IRoomRepository m_Repository;

    public RoomViewModel(Room room, [Import] IRoomRepository repository)
    {
      Model = room;
      m_Repository = repository;

      LoadRoomUsages();
    }

    public RoomViewModel()
    {
      Model = new Room
      {
        Building = "",
      };
    }

    public Room Model { get; private set; }

    public string RoomNumber
    {
      get { return Model.RoomNumber; }
      set { Model.RoomNumber = value; }
    }

    public int Floor
    {
      get { return Model.Floor; }
      set { Model.Floor = value; }
    }

    public float? Space
    {
      get
      {
        if (Model.Space.Equals(null))
        {
          return 0;
        }
        return Model.Space;
      }
      set
      {
        Model.Space = value;
        NotifyOfPropertyChange(() => RoomVolume);
      }
    }

    public float? Height
    {
      get
      {
        return Model.Height.Equals(null)
          ? 0
          : Model.Height;
      }
      set
      {
        Model.Height = value;
        NotifyOfPropertyChange(() => RoomVolume);
      }
    }

    public RoomUsage RoomInformation
    {
      get { return Model.RoomInformation; }
      set
      {
        Model.RoomInformation = value;
        NotifyOfPropertyChange(() => RoomInformation);
      }
    }

    public static IEnumerable<RoomUsage> RoomUsages
    {
      get { return m_RoomUsages; }
    }

    public float? RoomVolume
    {
      get
      {
        if (Model.Space.Equals(null) ||
            Model.Height.Equals(null))
        {
          return 0;
        }
        return (Model.Space * Model.Height);
      }
    }

    private void LoadRoomUsages()
    {
      m_RoomUsages.Clear();

      if (m_Repository.RoomUsages != null)
      {
        foreach (var ru in m_Repository.RoomUsages)
        {
          m_RoomUsages.Add(ru);
        }
      }
      else
      {
        m_RoomUsages.Add(new RoomUsage());
      }
    }
  }
}