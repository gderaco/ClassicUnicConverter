using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace UConverter.Util
{
    public sealed class AppDomain
    {
        public static AppDomain CurrentDomain { get; private set; }

        static AppDomain()
        {
            CurrentDomain = new AppDomain();
        }

        public Assembly[] GetAssemblies()
        {
            return GetAssemblyListAsync().Result.ToArray();
        }

        private async System.Threading.Tasks.Task<IEnumerable<Assembly>> GetAssemblyListAsync()
        {
            var folder = Windows.ApplicationModel.Package.Current.InstalledLocation;

            List<Assembly> assemblies = new List<Assembly>();
            foreach (Windows.Storage.StorageFile file in await folder.GetFilesAsync())
            {
                if (file.FileType == ".dll" || file.FileType == ".exe")
                {
                    AssemblyName name = new AssemblyName() { Name = file.Name };
                    Assembly asm = Assembly.Load(name);
                    assemblies.Add(asm);
                }
            }

            return assemblies;
        }
    }
}
