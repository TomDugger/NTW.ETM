using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace NTW.Data.Connections
{
    public interface IConnection: INotifyPropertyChanged, IDataErrorInfo
    {
        string Provider { get; set; }

        string PathToResourceDB { get; set; }

        string GetConnectionString();

        bool HasError { get; }

        IEnumerable<string> NamesServers { get; }

        IEnumerable<string> NamesDB { get; }

        IConnection Copy();
    }
}
