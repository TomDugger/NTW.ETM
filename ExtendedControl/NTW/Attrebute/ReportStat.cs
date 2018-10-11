using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NTW.Attrebute
{
    [System.AttributeUsage(AttributeTargets.Property | AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    public sealed class ReportStat : Attribute {
    }
}
