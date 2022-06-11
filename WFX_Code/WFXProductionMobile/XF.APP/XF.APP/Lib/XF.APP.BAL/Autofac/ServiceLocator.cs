
using Autofac;

namespace XF.APP.BAL
{
    public static class ServiceLocator
    {
        private static IContainer container;

        public static IContainer Container
        {
            get { return container; }
            set { container = value; }
        }

        public static T Resolve<T>()
        {
            return Container.Resolve<T>();
        }
    }
}
