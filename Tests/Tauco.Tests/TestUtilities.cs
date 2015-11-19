using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using NUnit.Framework;


namespace Tauco.Tests
{
    public class TestUtilities
    {
        public static void TestConstructorArgumentsNullCombinations(Type testedType, Type[] typeArguments, IList<Func<object>> arguments)
        {
            if (testedType.IsGenericType)
            {
                testedType = testedType.MakeGenericType(typeArguments);
            }

            var argumentTypes = arguments.Select(argument => argument.Method.ReturnType).ToArray();
            var constructor = testedType.GetConstructor(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, argumentTypes, null);

            if (constructor == null)
            {
                throw new ArgumentException("Constructor could not be found");
            }

            for (int i = 0; i < arguments.Count(); i++)
            {
                object[] args = arguments.Select(a => a()).ToArray();
                args[i] = null;

                Assert.That(() =>
                {
                    try
                    {
                        constructor.Invoke(args);
                    }
                    catch (TargetInvocationException exception)
                    {
                        throw exception.InnerException;
                    }
                }, Throws.TypeOf<ArgumentNullException>());
            }
        }
    }
}