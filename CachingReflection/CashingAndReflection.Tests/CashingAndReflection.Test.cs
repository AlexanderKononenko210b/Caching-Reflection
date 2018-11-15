using System;
using System.Collections.Generic;
using NUnit;
using NUnit.Framework;
using CachingReflection;

namespace CashingAndReflection.Tests
{
    [TestFixture]
    public class CashingAndReflectionTest
    {
        public static IEnumerable<TestCaseData> EqualitySource
        {
            get
            {
                yield return new TestCaseData(null, null, true);
                yield return new TestCaseData(5, null, false);
                yield return new TestCaseData(null, 5, false);
                yield return new TestCaseData(3, 5, false);
                yield return new TestCaseData(5, 5, true);
                yield return new TestCaseData(3.5, 5.01, false);
                yield return new TestCaseData(5.1, 5.1, true);
                yield return new TestCaseData(true, false, false);
                yield return new TestCaseData(true, true, true);
                yield return new TestCaseData(false, false, true);
                yield return new TestCaseData(new Box { X = 2, Y = 5, Size = 56, Location = "Saransk" }, new Box { X = 2, Y = 5, Size = 56, Location = "Saransk" }, true);
                yield return new TestCaseData(new Box { X = 2, Y = 5, Size = 56, Location = "Saransk" }, new Box { X = 2, Y = 6, Size = 56, Location = "Saransk" }, false);
                yield return new TestCaseData(new Stock
                {
                    Number = 5,
                    Boxes = new List<Box>
                    {
                        new Box { X = 2, Y = 5, Size = 56, Location = "Saransk" },
                        new Box { X = 2, Y = 5, Size = 56, Location = "Saransk" },
                        new Box { X = 2, Y = 5, Size = 56, Location = "Saransk" }
                    }
                }, 
                new Stock
                {
                    Number = 5,
                    Boxes = new List<Box>
                    {
                        new Box { X = 2, Y = 5, Size = 56, Location = "Saransk" },
                        new Box { X = 2, Y = 5, Size = 56, Location = "Saransk" },
                        new Box { X = 2, Y = 5, Size = 56, Location = "Saransk" }
                    }
                }
                , true);
                yield return new TestCaseData(new Stock
                    {
                        Number = 5,
                        Boxes = new List<Box>
                        {
                            new Box { X = 2, Y = 6, Size = 56, Location = "Saransk" },
                            new Box { X = 2, Y = 5, Size = 56, Location = "Saransk" },
                            new Box { X = 2, Y = 5, Size = 56, Location = "Saransk" }
                        }
                    },
                    new Stock
                    {
                        Number = 5,
                        Boxes = new List<Box>
                        {
                            new Box { X = 2, Y = 5, Size = 56, Location = "Saransk" },
                            new Box { X = 2, Y = 5, Size = 56, Location = "Saransk" },
                            new Box { X = 2, Y = 5, Size = 56, Location = "Saransk" }
                        }
                    }
                    , false);
            }
        }

        [TestCaseSource(nameof(EqualitySource))]
        public void ReflectionTest(object first, object second, bool expectedResult)
        {
            var equalityResult = Reflection.EquatableComparer(first, second);

            Assert.AreEqual(expectedResult, equalityResult);
        }

        private class Box : IEquatable<Box>
        {
            public int X { get; set; }

            public int Y { get; set; }

            public int Size { get; set; }

            public string Location { get; set; }

            public bool Equals(Box other)
            {
                if (ReferenceEquals(other, null)) return false;

                if (ReferenceEquals(this, other)) return true;

                if (X != other.X ||
                    Y != other.Y ||
                    Size != other.Size ||
                    !Location.Equals(other.Location, StringComparison.CurrentCulture))
                    return false;

                return true;
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(obj, null)) return false;

                if (ReferenceEquals(this, obj)) return true;

                if (this.GetType() != obj.GetType()) return false;

                return Equals((Box)obj);
            }

            public override int GetHashCode()
            {
                return X.GetHashCode() + Y.GetHashCode() + Size.GetHashCode() + Location.GetHashCode();
            }
        }

        private class Stock
        {
            public int Number { get; set; }

            public List<Box> Boxes { get; set; }
        }
    }
}
