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

namespace Ork.Energy
{
  public enum Status
  {
    Created = 0,
    Progress = 1,
    Completed = 2
  }

  public enum Priority
  {
    Low = 0,
    Medium = 1,
    High = 2
  }

  public enum EvaluationRating
  {
      X = 0,
      XX = 1,
      XXX = 2,
      XXXX = 4,
      XXXXX = 5
  }
}