﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csla;

namespace AsyncLookupRule.Test
{
  [Serializable]
  public class RootFake : BusinessBase<RootFake>
  {
    public static readonly PropertyInfo<int> CustomerIdProperty = RegisterProperty<int>(c => c.CustomerId);
    public int CustomerId
    {
      get { return GetProperty(CustomerIdProperty); }
      set { SetProperty(CustomerIdProperty, value); }
    }

    public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(c => c.Name);
    public string Name
    {
      get { return GetProperty(NameProperty); }
      set { SetProperty(NameProperty, value); }
    }
  }
}
