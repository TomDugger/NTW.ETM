using NTW.Communication.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Text;

namespace NTW.Communication.Beginers
{
    public class ServerBeginer
    {
        #region Fields
        private ServiceHost _host = null;
        private string _ip = "localhost";
        private int _port = 8810;
        private string _serviceName = "commands";

        private string _connectionString;

        private Type _serviceType;
        private Type _implementedContract;
        #endregion

        public ServerBeginer(Type serviceType, Type implementedContract, string ip = "localhost", int port = 8810, string serviceName = "commands") {
            this._serviceType = serviceType;
            this._implementedContract = implementedContract;

            this._ip = ip;
            this._port = port;
            this._serviceName = serviceName;

            this._connectionString = string.Format("http://{0}:{1}/{2}", ip, port, serviceName);
        }

        #region Helps
        public void Start(Action Successfully = null, Action Failed = null) {

            //резервация url (вопрос с правами администратора)
            //System.Diagnostics.Process.Start("CMD.exe", string.Format("netsh http add urlacl url=http://+:{0}/{1} user={2}", _port, _serviceName, Environment.UserName));

            _host = new ServiceHost(_serviceType, new Uri(_connectionString));
            _host.AddServiceEndpoint(_implementedContract, new WebHttpBinding(), new Uri(_connectionString)).Behaviors.Add(new CommHttpBehavior());

            _host.BeginOpen((res) =>
            {
                if (res.IsCompleted && _host.State == CommunicationState.Opened) Successfully?.Invoke();
                else Failed?.Invoke();
            }, null);
        }

        public void Stop(Action Successfully = null, Action Failed = null) {
            _host?.BeginClose((res) => {
                if (res.IsCompleted && _host.State == CommunicationState.Closed) Successfully?.Invoke();
                else Failed?.Invoke();
            }, null);
        }
        #endregion
    }
}
