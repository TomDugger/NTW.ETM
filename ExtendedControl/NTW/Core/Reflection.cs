using NTW.Attrebute;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;

namespace NTW.Core
{
    public static class Reflection
    {

        public static ObservableCollection<ObjectNodeKey> GetTree(string name, Type type) {
            ObservableCollection<ObjectNodeKey> nodes = new ObservableCollection<ObjectNodeKey>();
            nodes.Add(new ObjectNodeKey(name, type));
            return nodes;
        }

        public static ObservableCollection<ObjectNodeKey> GetTreeByNamespase(string space, Type baseType) {
            ObservableCollection<ObjectNodeKey> nodes = new ObservableCollection<ObjectNodeKey>();
            Type[] typelist = Assembly.GetAssembly(baseType).GetTypes().Where(t => t.Namespace == space).ToArray();
            foreach (var item in typelist)
                if(Attribute.GetCustomAttribute(item, typeof(ReportStat)) != null)
                nodes.Add(new ObjectNodeKey(item.Name, item));
            return nodes;
        }
    }

    public class ObjectNodeKey {

        public ObjectNodeKey() { }

        public ObjectNodeKey(string name, Type type) {
            if (type.IsGenericType)
                ParseTreeGeneric(name, type);
            else if (Attribute.GetCustomAttribute(type, typeof(ReportStat)) != null)
                ParseTree(name, type);
            else
            {
                this._name = name;
                this._type = type;
            }
        }

        public ObjectNodeKey(string name, PropertyInfo info)
        {
            if (info.PropertyType.IsInterface && info.PropertyType == typeof(ICollectionView))
                ParseTreeCollectionView(name, info);
            else {
                _name = name;
                _type = info.PropertyType;
            }
        }

        #region members
        private string _name;
        private Type _type;
        #endregion

        #region property
        public string Name {
            get { return _name; }
        }

        public Type Type {
            get { return _type; }
        }

        public ObservableCollection<ObjectNodeKey> Children { get; set; }
        #endregion

        #region members
        protected void ParseTree(string name, Type type) {
            Children = new ObservableCollection<ObjectNodeKey>();
            _type = type;
            _name = name;

            PropertyInfo[] props = type.GetProperties();

            foreach (var p in props) {
                if (p.PropertyType.IsPublic) {
                    ReportStat rs = (ReportStat)Attribute.GetCustomAttribute(p, typeof(ReportStat));
                    if (rs != null ) {
                        if (p.PropertyType.IsInterface && p.PropertyType == typeof(ICollectionView))
                            Children.Add(new ObjectNodeKey(p.Name, p));
                        else
                            Children.Add(new ObjectNodeKey(p.Name, p.PropertyType));
                    }
                }
            }
        }

        protected void ParseTreeGeneric(string name, Type type) {
            Children = new ObservableCollection<ObjectNodeKey>();
            _type = type;
            _name = name;

            var gtype = type.GetGenericArguments()[0];

            var obj = new ObjectNodeKey(gtype.Name, gtype);
            Children.Add(obj);
        }

        protected void ParseTreeCollectionView(string name, PropertyInfo property) {
            Children = new ObservableCollection<ObjectNodeKey>();

            _name = name;
            _type = property.PropertyType;

            ReportType rt = (ReportType)Attribute.GetCustomAttribute(property, typeof(ReportType));

            if (rt != null) {
                var obj = new ObjectNodeKey(rt.Nametype.Name, rt.Nametype);
                Children.Add(obj);
            }
        }
        #endregion
    }
}
