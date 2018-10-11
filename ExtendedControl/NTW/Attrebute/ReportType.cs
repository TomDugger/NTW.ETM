using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NTW.Attrebute
{
    [System.AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = true)]
    public sealed class ReportType : Attribute
    {
        public ReportType(Type type) {
            Nametype = type;
        }

        public Type Nametype { get; set; }
    }
}
