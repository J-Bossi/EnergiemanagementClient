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
using System.ComponentModel.Composition;
using Caliburn.Micro;

namespace Ork.Framework.ViewModels
{
  [Export(typeof (IDialogManager))]
  [PartCreationPolicy(CreationPolicy.NonShared)]
  public class DialogConductorViewModel : PropertyChangedBase, IDialogManager, IConductActiveItem
  {
    private readonly Func<IMessageBox> createMessageBox;

    [ImportingConstructor]
    public DialogConductorViewModel(Func<IMessageBox> messageBoxFactory)
    {
      createMessageBox = messageBoxFactory;
    }

    public IScreen ActiveItem { get; private set; }

    object IHaveActiveItem.ActiveItem
    {
      get { return ActiveItem; }
      set { ActivateItem(value); }
    }

    public event EventHandler<ActivationProcessedEventArgs> ActivationProcessed = delegate
                                                                                  {
                                                                                  };

    public void ActivateItem(object item)
    {
      ActiveItem = item as IScreen;

      var child = ActiveItem as IChild;
      if (child != null)
      {
        child.Parent = this;
      }

      if (ActiveItem != null)
      {
        ActiveItem.Activate();
      }

      NotifyOfPropertyChange(() => ActiveItem);
      ActivationProcessed(this, new ActivationProcessedEventArgs
                                {
                                  Item = ActiveItem,
                                  Success = true
                                });
    }

    public void DeactivateItem(object item, bool close)
    {
      var guard = item as IGuardClose;
      if (guard != null)
      {
        guard.CanClose(result =>
                       {
                         if (result)
                         {
                           CloseActiveItemCore();
                         }
                       });
      }
      else
      {
        CloseActiveItemCore();
      }
    }

    public IEnumerable GetChildren()
    {
      return ActiveItem != null
        ? new[]
          {
            ActiveItem
          }
        : new object[0];
    }

    public void ShowDialog(IScreen dialogModel)
    {
      ActivateItem(dialogModel);
    }

    public void ShowMessageBox(string message, string title = "Nicht gespeichert", MessageBoxOptions options = MessageBoxOptions.Ok, Action<IMessageBox> callback = null)
    {
      var box = createMessageBox();

      box.DisplayName = title;
      box.Options = options;
      box.Message = message;

      if (callback != null)
      {
        box.Deactivated += delegate
                           {
                             callback(box);
                           };
      }

      ActivateItem(box);
    }

    private void CloseActiveItemCore()
    {
      var oldItem = ActiveItem;
      ActivateItem(null);
      oldItem.Deactivate(true);
    }
  }
}