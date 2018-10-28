﻿using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UnitTestConstants;

namespace Common.Data.Tests
{
  [TestClass]
  public class UtPatternFinder
  {
    [TestMethod, TestCategory(Constants.METHOD)]
    public void FindPattern()
    {
      var data = new List<string> { "234", "2301", "501", "01" };
      var finder = new PatternFinder(data);
      var result = finder.FindPattern();

      Assert.AreEqual("234|(0|230|50)1", result);
    }
  }
}
