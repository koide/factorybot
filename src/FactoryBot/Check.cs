﻿using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace FactoryBot
{
    [DebuggerStepThrough]
    internal static class Check
    {
        public static void NotNull(object parameter, string parameterName)
        {
            if (parameter == null)
            {
                throw new ArgumentNullException(parameterName);
            }
        }

        public static void NotNullOrWhiteSpace(string parameter, string parameterName)
        {
            if (string.IsNullOrWhiteSpace(parameter))
            {
                throw new ArgumentException("String should not be null or empty.", parameterName);
            }
        }

        public static void GreaterThanZero(int parameter, string parameterName)
        {
            if (parameter <= 0)
            {
                throw new ArgumentOutOfRangeException(parameterName, parameter, "Should be greater than zero.");
            }
        }

        public static void MinMax<T>(T minParameter, T maxParameter, string minParameterName) where T : IComparable<T>
        {
            if (minParameter.CompareTo(maxParameter) > 0)
            {
                throw new ArgumentOutOfRangeException(minParameterName, "Minimum should not be greater than maximum.");
            }
        }

        public static void CollectionNotEmpty<T>(IReadOnlyCollection<T> collection, string parameterName)
        {
            if (collection.Count == 0)
            {
                throw new ArgumentException("Collection should not be empty.", parameterName);
            }
        }
    }
}