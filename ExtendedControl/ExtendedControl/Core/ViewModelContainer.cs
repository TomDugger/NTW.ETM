using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection.Emit;
using System.Text;

namespace NTW.Core
{
    public class ViewModelContainer
    {
        private static ViewModelContainer _instance;

        public static ViewModelContainer Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ViewModelContainer();
                    _instance.viewModel = new Dictionary<Type, object>();
                }

                return _instance;
            }
        }

        private ViewModelContainer() { }

        public object Register(Type type)
        {

            if (!viewModel.ContainsKey(type))
                viewModel.Add(type, CreateInstance(type)); //DynamicModuleLambdaCompiler.GenerateFactory<ContextViewModel>().Invoke());

            return viewModel[type];
        }

        public T Register<T>()
        {
            if (!viewModel.ContainsKey(typeof(T)))
                viewModel.Add(typeof(T), CreateInstance(typeof(T)));

            return (T)viewModel[typeof(T)];
        }

        public object Attached(Type type)
        {
            return CreateInstance(type); //DynamicModuleLambdaCompiler.GenerateFactory<ContextViewModel>();
        }

        private static object CreateInstance(Type onType)
        {
            try
            {
                return Activator.CreateInstance(onType);
            }
            catch { return null; }
        }

        private Dictionary<Type, object> viewModel;

        //пока это оставим здесь
        private static class DynamicModuleLambdaCompiler
        {
            public static Func<T> GenerateFactory<T>() where T : new()
            {
                Expression<Func<T>> expr = () => new T();
                NewExpression newExpr = (NewExpression)expr.Body;

                var method = new DynamicMethod(
                    name: "lambda",
                    returnType: newExpr.Type,
                    parameterTypes: new Type[0],
                    m: typeof(DynamicModuleLambdaCompiler).Module,
                    skipVisibility: true);

                ILGenerator ilGen = method.GetILGenerator();

                if (newExpr.Constructor != null)
                {
                    ilGen.Emit(OpCodes.Newobj, newExpr.Constructor);
                }
                else
                {
                    LocalBuilder temp = ilGen.DeclareLocal(newExpr.Type);
                    ilGen.Emit(OpCodes.Ldloca, temp);
                    ilGen.Emit(OpCodes.Initobj, newExpr.Type);
                    ilGen.Emit(OpCodes.Ldloc, temp);
                }

                ilGen.Emit(OpCodes.Ret);

                return (Func<T>)method.CreateDelegate(typeof(Func<T>));
            }
        }
    }
}
