using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using NUnit;
using NUnit.Framework;
using CachingReflection;

namespace CashingAndReflections
{
    [TestFixture]
    public class EquatableComparerTests
    {
        public IEnumerable<TestCaseData> EqualitySource
        {
            get
            {
                yield return new TestCaseData(3, 5, false);
                yield return new TestCaseData(5, 5, true);
                yield return new TestCaseData(new Box { X = 2, Y = 5, Size = 56, Location = "Saransk" }, new Box { X = 2, Y = 5, Size = 56, Location = "Saransk" }, true);
                yield return new TestCaseData(new Box { X = 2, Y = 5, Size = 56, Location = "Saransk" }, new Box { X = 2, Y = 6, Size = 56, Location = "Saransk" }, false);
            }
        }

        [TestCaseSource(nameof(EqualitySource))]
        public void TestMethod1(object first, object second, bool expectedResult)
        {
            
        }

        private class Box
        {
            public int X { get; set; }

            public int Y { get; set; }

            public int Size { get; set; }

            public string Location { get; set; }    
        }

    }
}
