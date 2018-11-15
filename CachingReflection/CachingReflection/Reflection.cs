using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CachingReflection
{
    /// <summary>
    /// Represents a model of the Reflection class for equality comparison two objects.
    /// </summary>
    public static class Reflection
    {
        /// <summary>
        /// Equality comparison two objects.
        /// </summary>
        /// <param name="first">first instance</param>
        /// <param name="second">second instance</param>
        /// <returns>if instances are equal return true or false</returns>
        public static bool EquatableComparer(object first, object second)
        {
            if (first == null && second == null) return true;

            if (first == null | second == null) return false;

            var firstType = first.GetType();

            var secondType = second.GetType();

            if (firstType != secondType) return false;

            if (firstType.IsClass)
            {
                if (!EqualityReferenceType(first, second))
                    return false;
            }

            if (firstType.IsValueType)
            {
                if (firstType.IsPrimitive)
                {
                    if ((dynamic)first != (dynamic)second)
                        return false;
                }
                else
                {
                    if (!EqualityValueType(first, second))
                        return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Equality comparison value type instances
        /// </summary>
        /// <param name="first">first instance</param>
        /// <param name="second">second instance</param>
        /// <returns>if instances are equal return true or false</returns>
        private static bool EqualityValueType(object first, object second)
        {
            var firstFields = first.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            var secondFields = second.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            for (int i = 0; i < firstFields.Length; i++)
            {
                if (firstFields[i].FieldType.IsPrimitive)
                {
                    if ((dynamic)firstFields[i].GetValue(first) != (dynamic)secondFields[i].GetValue(second))
                        return false;
                }

                if (firstFields[i].FieldType.IsClass)
                {
                    if (!EqualityReferenceType(firstFields[i].GetValue(first), secondFields[i].GetValue(second)))
                        return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Equality comparison reference type instances
        /// </summary>
        /// <param name="first">first instance</param>
        /// <param name="second">second instance</param>
        /// <returns>if instances are equal return true or false</returns>
        private static bool EqualityReferenceType(object first, object second)
        {
            if (first == null && second == null) return true;

            if (first == null | second == null) return false;

            var firstType = first.GetType();

            var secondType = second.GetType();

            if (firstType != secondType) return false;

            if (ReferenceEquals(first, second)) return true;
                
            if(first.GetType().GetInterfaces().Any(x =>
            x.IsGenericType &&
            x.GetGenericTypeDefinition() == typeof(IEquatable<>)))
            {
                if (!first.Equals(second))
                    return false;
            }

            if (firstType.GetInterfaces().Any(x =>
                x.IsGenericType &&
                x.GetGenericTypeDefinition() == typeof(IEnumerable<>)))
            {
                if (!EqualityIEnumerableType(first, second))
                    return false;
                return true;
            }

            var firstFields = first.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            var secondFields = second.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            if (firstFields.Length != 0)
            {
                for (int i = 0; i < firstFields.Length; i++)
                {
                    if (firstFields[i].FieldType.IsPrimitive)
                    {
                        if ((dynamic)firstFields[i].GetValue(first) != (dynamic)secondFields[i].GetValue(second))
                            return false;
                    }
                    else
                    {
                        if (firstFields[i].FieldType.IsClass)
                        {
                            if (!EqualityReferenceType(firstFields[i].GetValue(first),
                                secondFields[i].GetValue(second)))
                                return false;
                        }

                        if (firstFields[i].FieldType.IsValueType)
                        {
                            if (!EqualityValueType(firstFields[i].GetValue(first), secondFields[i].GetValue(second)))
                                return false;
                        }
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Equality comparison instances implements IEnumerable<T></T>
        /// </summary>
        /// <param name="first">first instance</param>
        /// <param name="second">second instance</param>
        /// <returns>if instances are equal return true or false</returns>
        private static bool EqualityIEnumerableType(object first, object second)
        {
            int firstCount = 0;

            var argumentsType = (dynamic)first.GetType().GetGenericArguments()[0];

            foreach (var itemFirst in (dynamic)first)
            {
                int secondCount = 0;

                foreach (var itemSecond in (dynamic)second)
                {
                    if (secondCount == firstCount)
                    {
                        if (argumentsType.IsClass)
                        {
                            if (!EqualityReferenceType(itemFirst, itemSecond))
                                return false;
                        }

                        if (argumentsType.IsValueType)
                        {
                            if (!EqualityValueType(itemFirst, itemSecond))
                                return false;
                        }
                        break;
                    }
                    ++secondCount;
                }
                ++firstCount;
            }

            return true;
        }
    }
}
