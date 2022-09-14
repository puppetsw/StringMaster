using System;
using System.Collections.Generic;
using System.Reflection;

namespace StringMaster;

public static class Ioc
{
    private static readonly IDictionary<Type, Type> s_types = new Dictionary<Type, Type>();

    public static void Register<TContract, TImplementation>()
    {
        s_types[typeof(TContract)] = typeof(TImplementation);
    }

    public static void Register<TImplementation>()
    {
        s_types[typeof(TImplementation)] = typeof(TImplementation);
    }

    public static T Resolve<T>()
    {
        return (T)Resolve(typeof(T));
    }

    public static object Resolve(Type contract)
    {
        Type implementation = s_types[contract];
        ConstructorInfo constructor = implementation.GetConstructors()[0];
        ParameterInfo[] constructorParameters = constructor.GetParameters();
        if (constructorParameters.Length == 0)
        {
            return Activator.CreateInstance(implementation);
        }

        List<object> parameters = new List<object>(constructorParameters.Length);
        foreach (ParameterInfo parameterInfo in constructorParameters)
        {
            parameters.Add(Resolve(parameterInfo.ParameterType));
        }

        return constructor.Invoke(parameters.ToArray());
    }
}
