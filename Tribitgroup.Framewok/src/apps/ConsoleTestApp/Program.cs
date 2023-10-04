using System;
using System.Threading.Tasks;
using Numpy;
using Python.Included;
using Python.Runtime;

namespace ConsoleTestApp
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            await Installer.SetupPython();
            await Installer.TryInstallPip();
            await Installer.PipInstallModule("spacy");
            PythonEngine.Initialize();
            dynamic spacy = Py.Import("spacy");
            Console.WriteLine("Spacy version: " + spacy.__version__);

            Method1();
        }

        private static void Method1()
        {
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