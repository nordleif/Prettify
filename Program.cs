using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Newtonsoft.Json;

namespace Prettify
{
    class Program
    {
        static Program()
        {
            AppDomain.CurrentDomain.AssemblyResolve += (s, e) =>
            {
                try
                {
                    var assembly = Assembly.GetExecutingAssembly();
                    var assemblyName = new AssemblyName(e.Name);
                    var fileName = assemblyName.Name + ".dll";
                    var resources = assembly.GetManifestResourceNames().Where(mrn => mrn.EndsWith(fileName));
                    if (resources.Any())
                    {
                        var resourceName = resources.First();
                        using (var stream = assembly.GetManifestResourceStream(resourceName))
                        {
                            if (stream == null)
                                return null;

                            var buffer = new byte[stream.Length];
                            try
                            {
                                stream.Read(buffer, 0, buffer.Length);
                                return Assembly.Load(buffer);
                            }
                            catch (IOException)
                            {
                                return null;
                            }
                            catch (BadImageFormatException)
                            {
                                return null;
                            }
                        }
                    }
                }
                catch
                {

                }
                return null;
            };
        }

        [STAThread]
        static void Main(string[] args)
        {
            var useClipboard = true;
            var text = Clipboard.GetText();
            if (args.Any())
            {
                text = string.Join(" ", args);
                useClipboard = false;
            }

            if (string.IsNullOrWhiteSpace(text))
                return;

            try
            {
                text = JsonConvert.SerializeObject(JsonConvert.DeserializeObject(text), Formatting.Indented);
                Clipboard.SetText(text);
                Console.WriteLine(useClipboard ? "Clipboard JSON prettified." : text);
                return;
            }
            catch { }
            
           

        }
    }
}
