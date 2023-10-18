using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Numpy;
using Python.Included;
using Python.Runtime;
using Tribitgroup.Framework.Shared.Services;
using Tribitgroup.Framework.Shared.Types;

namespace ConsoleTestApp
{
    public class TestEntity : Entity
        {
            public string FullName { get; set; } = string.Empty;
        }
    internal class Program
    {
        
        static async Task Main(string[] args)
        {
            var res = CSharpCodeRunner.RunLibraryCodeFromFile(await File.ReadAllTextAsync("D:/1.cs"),"LibClass", "LibMethod");
        }

        private static async Task InMemoryCacheTestAsync()
        {
            var cache = new InMemoryEntityCache<TestEntity>(1024);

            var test1 = new TestEntity { FullName = "a" };
            var test2 = new TestEntity { FullName = "b" };

            await cache.AddOrUpdateAsync(test1, test2);

            test1.FullName = "Yashar";

            await cache.AddOrUpdateAsync(test1, TimeSpan.FromSeconds(3));
            var x = await cache.GetAsync(test1.Id);
            await Console.Out.WriteLineAsync(x.FullName);
            await Task.Delay(TimeSpan.FromSeconds(5));

            x = await cache.GetAsync(test1.Id);
            await Console.Out.WriteLineAsync(x?.FullName ?? "NULL");
        }

        private async static Task Method1()
        {
            await Installer.SetupPython();
            await Installer.TryInstallPip();
            await Installer.PipInstallModule("spacy");
            PythonEngine.Initialize();
            dynamic spacy = Py.Import("spacy");
            Console.WriteLine("Spacy version: " + spacy.__version__);
            var a = np.arange(1000);
            var b = np.arange(1000);

            // https://github.com/pythonnet/pythonnet/issues/109
            PythonEngine.BeginAllowThreads();

            Task.Run(() => {
                // when running on different threads you must lock!
                using (Py.GIL())
                {
                    np.matmul(a, b);
                }
            }).Wait();
        }
    }
}