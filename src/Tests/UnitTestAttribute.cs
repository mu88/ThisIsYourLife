using System;
using NUnit.Framework;

namespace Tests
{
    [AttributeUsage(AttributeTargets.Method)]
    public class UnitTestAttribute : CategoryAttribute
    {
    }
}