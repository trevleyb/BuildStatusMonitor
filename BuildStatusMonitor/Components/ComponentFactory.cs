using System;
using System.Reflection;
using BuildStatusMonitor.Utilities;

namespace BuildStatusMonitor.Components
{
    /// <summary>
    /// This static class is used to instantiate a object from a class name from a specified assembly. The class being
    /// instatitaed must support the <c>IComponent</c> interface and it will then initialise the object by passing through 
    /// settings component of the component definition. 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class ComponentFactory<T> where T : class 
    {
        public static T CreateComponent(IComponentDef componentDef) {
            try {
                var assembly = string.IsNullOrEmpty(componentDef.Assembly) ? Assembly.GetExecutingAssembly().FullName : componentDef.Assembly;
                var instance = Activator.CreateInstance(assembly, componentDef.Class).Unwrap() as IComponent;
                if (instance != null) {
                    instance.Initialise(componentDef.Name, componentDef.Settings);
                    return instance as T;
                }
                FileLogger.Logger.LogError("Could not instantiate instance '{0}' in '{1}'", componentDef.Class, componentDef.Assembly);
            } catch (Exception ex) {
                throw new LogApplicationException("Could not instantiate " + componentDef.Name, ex);
            }
            return null;
        }

    }
}
