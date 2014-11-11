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
using Caliburn.Micro;

namespace Ork.Framework
{
  public class ApplicationCloseCheck : IResult
  {
    private readonly Action<IDialogManager, Action<bool>> m_CloseCheck;
    private readonly IChild m_Screen;

    public ApplicationCloseCheck(IChild screen, Action<IDialogManager, Action<bool>> closeCheck)
    {
      m_Screen = screen;
      m_CloseCheck = closeCheck;
    }

    [Import]
    public IShell Shell { get; set; }

    public event EventHandler<ResultCompletionEventArgs> Completed = delegate
                                                                     {
                                                                     };

    public void Execute(ActionExecutionContext context)
    {
      var documentWorkspace = m_Screen.Parent as IDocumentWorkspace;
      if (documentWorkspace != null)
      {
        documentWorkspace.Edit(m_Screen);
      }
      else
      {
        Shell.ActivateItem(m_Screen);
      }

      m_CloseCheck(Shell.Dialogs, result => Completed(this, new ResultCompletionEventArgs
                                                            {
                                                              WasCancelled = !result
                                                            }));
    }
  }
}