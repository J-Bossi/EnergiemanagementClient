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
using System.Collections.Generic;
using System.Linq;
using Caliburn.Micro;

namespace Ork.Framework
{
  public class ApplicationCloseStrategy : ICloseStrategy<IWorkspace>
  {
    private Action<bool, IEnumerable<IWorkspace>> callback;
    private IEnumerator<IWorkspace> enumerator;
    private bool finalResult;

    public void Execute(IEnumerable<IWorkspace> toClose, Action<bool, IEnumerable<IWorkspace>> cb)
    {
      enumerator = toClose.GetEnumerator();
      callback = cb;
      finalResult = true;

      Evaluate(finalResult);
    }

    private void Evaluate(bool result)
    {
      finalResult = finalResult && result;

      if (!enumerator.MoveNext() ||
          !result)
      {
        callback(finalResult, new List<IWorkspace>());
      }
      else
      {
        var current = enumerator.Current;

        var conductor = current as IConductor;
        if (conductor != null)
        {
          var tasks = conductor.GetChildren()
                               .OfType<IHaveShutdownTask>()
                               .Select(x => x.GetShutdownTask())
                               .Where(x => x != null);

          var sequential = new SequentialResult(tasks.GetEnumerator());
          sequential.Completed += (s, e) =>
                                  {
                                    if (!e.WasCancelled)
                                    {
                                      Evaluate(!e.WasCancelled);
                                    }
                                  };
          sequential.Execute(new ActionExecutionContext());
        }
        else
        {
          var haveShutdownTask = current as IHaveShutdownTask;
          if (haveShutdownTask != null)
          {
            var shutdownTask = haveShutdownTask.GetShutdownTask();
            if (shutdownTask != null)
            {
              shutdownTask.Completed += (s, e) =>
                                        {
                                          if (!e.WasCancelled)
                                          {
                                            Evaluate(!e.WasCancelled);
                                          }
                                        };
              IoC.BuildUp(shutdownTask);
              shutdownTask.Execute(new ActionExecutionContext());
            }
            else
            {
              Evaluate(true);
            }
          }
          else
          {
            Evaluate(true);
          }
        }
      }
    }
  }
}