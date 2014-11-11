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

using System.ComponentModel;
using System.ComponentModel.Composition;
using Caliburn.Micro;

namespace Ork.Framework
{
  public abstract class DocumentWorkspace<TDocument> : Conductor<TDocument>.Collection.OneActive, IDocumentWorkspace
    where TDocument : class, INotifyPropertyChanged, IDeactivate, IHaveDisplayName
  {
    private bool m_IsEnabled;
    private DocumentWorkspaceState m_State = DocumentWorkspaceState.Master;

    protected DocumentWorkspace()
    {
      Items.CollectionChanged += delegate
                                 {
                                   NotifyOfPropertyChange(() => Status);
                                 };
      DisplayName = Title;
    }

    [Import]
    public IDialogManager Dialogs { get; set; }

    public DocumentWorkspaceState State
    {
      get { return m_State; }
      set
      {
        if (m_State == value)
        {
          return;
        }

        m_State = value;
        NotifyOfPropertyChange(() => State);
      }
    }

    protected IConductor Conductor
    {
      get { return (IConductor) Parent; }
    }

    public string Status
    {
      get
      {
        return Items.Count > 0
          ? Items.Count.ToString()
          : string.Empty;
      }
    }

    public abstract int Index { get; }

    public virtual bool IsEnabled
    {
      get { return m_IsEnabled; }
      protected set
      {
        if (value.Equals(m_IsEnabled))
        {
          return;
        }
        m_IsEnabled = value;
        NotifyOfPropertyChange(() => IsEnabled);
      }
    }

    public abstract string Title { get; }

    void IDocumentWorkspace.Edit(object document)
    {
      Edit((TDocument) document);
    }

    public override void ActivateItem(TDocument item)
    {
      item.Deactivated += OnItemOnDeactivated;
      item.PropertyChanged += OnItemPropertyChanged;

      base.ActivateItem(item);
    }

    public void Edit(TDocument child)
    {
      Conductor.ActivateItem(this);
      State = DocumentWorkspaceState.Detail;
      DisplayName = child.DisplayName;
      ActivateItem(child);
    }

    public void Show()
    {
      var haveActive = Parent as IHaveActiveItem;
      if (haveActive != null &&
          haveActive.ActiveItem == this)
      {
        DisplayName = Title;
        State = DocumentWorkspaceState.Master;
      }
      else
      {
        Conductor.ActivateItem(this);
      }
    }

    private void OnItemOnDeactivated(object sender, DeactivationEventArgs e)
    {
      var doc = (TDocument) sender;
      if (e.WasClosed)
      {
        DisplayName = Title;
        State = DocumentWorkspaceState.Master;
        doc.Deactivated -= OnItemOnDeactivated;
        doc.PropertyChanged -= OnItemPropertyChanged;
      }
    }

    private void OnItemPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (e.PropertyName == "DisplayName")
      {
        DisplayName = ((TDocument) sender).DisplayName;
      }
    }
  }
}