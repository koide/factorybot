﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FactoryBot.Generators;

namespace FactoryBot.DSL.Attributes
{
    public sealed class GeneratorAttribute : GeneratorAttributeBase
    {
        public GeneratorAttribute(Type generatorType)
        {
            if (generatorType.GetInterfaces().All(x => x != typeof(IGenerator)))
            {
                throw new ArgumentException($"Type {generatorType} should implement IGenerator.");
            }

            GeneratorType = generatorType;
        }

        public Type GeneratorType { get; }

        public override IGenerator CreateGenerator(MethodInfo method, IDictionary<string, object> parameters)
        {
            if (!GeneratorType.IsGenericType || !GeneratorType.IsGenericTypeDefinition)
            {
                return CreateGenerator(GeneratorType, parameters);
            }

            if (!method.IsGenericMethod)
            {
                throw new ArgumentException("Only generic methods are allowed for generic generators.", nameof(method));
            }

            // TODO: check if generic arguments of both generator and method match 

            return CreateGenerator(GeneratorType.MakeGenericType(method.GetGenericArguments()), parameters); 
        }
        
        private static IGenerator CreateGenerator(Type generatorType, IDictionary<string, object> parameters)
        {
            var constructor = generatorType.GetConstructors().FirstOrDefault(x => IsSuitableConstructor(x, parameters.Keys));
            if (constructor == null)
            {
                var parameterNames = string.Join(", ", parameters.Keys.Select(x => $"'{x}'"));
                throw new MissingMethodException($"No constructor of class {generatorType} with parameters: {parameterNames} has been found.");
            }

            var constructorParameters = constructor.GetParameters().Select(x => ChooseParameter(x, parameters)).ToArray();

            try
            {
                return (IGenerator)constructor.Invoke(constructorParameters);
            }
            catch (TargetInvocationException ex) when (ex.InnerException != null)
            {
                throw new GeneratorInitializationException(generatorType, ex.InnerException);
            }            
        }

        private static object? ChooseParameter(ParameterInfo parameterInfo, IDictionary<string, object> parameters)
        {
            return parameters.TryGetValue(parameterInfo.Name ?? string.Empty, out object? result) ? result : parameterInfo.DefaultValue;
        }

        private static bool IsSuitableConstructor(ConstructorInfo constructor, ICollection<string> parameterNames)
        {
            var parameters = constructor.GetParameters();
            return parameters.Length >= parameterNames.Count 
                && parameters.All(parameter => parameterNames.Contains(parameter?.Name ?? string.Empty) || (parameter?.HasDefaultValue ?? false));
        }
    }
}