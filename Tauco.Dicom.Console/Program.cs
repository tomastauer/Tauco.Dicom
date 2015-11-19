using System;
using System.IO;
using System.Linq;

using Castle.MicroKernel.Registration;
using Castle.Windsor;

using Tauco.Dicom.Shared;

namespace Tauco.Dicom.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            if (!File.Exists("NLog.config"))
            {
                System.Console.Error.WriteLine("Could not found NLog.config, terminating");
                return;
            }

            var originalOut = System.Console.Out;
            System.Console.SetOut(TextWriter.Null);
            
            var installer = new Installer();
            var container = new WindsorContainer().Install(new CommonInstaller());
            installer.Install(container);

            var settingsProvider = container.Resolve<ISettingsProvider>();
            var parser = container.Resolve<IBirthNumberParser>();
            
            var inputArguments = new InputArguments(settingsProvider, parser);
            if (CommandLine.Parser.Default.ParseArguments(args, inputArguments))
            {
                var errors = inputArguments.Validate().ToList();
                if (errors.Any())
                {
                    System.Console.Error.WriteLine("Program could not create any request due to the following input errors:");
                    System.Console.Error.WriteLine();
                    System.Console.Error.WriteLine(string.Join(Environment.NewLine, errors));

                    return;
                }
            }
            else
            {
                return;
            }

            var inputSettingsProvider = new InputSettingsProvider(inputArguments);
            container.Register(Component.For<ISettingsProvider>().Instance(inputSettingsProvider).IsDefault().Named("InputSettingsProvider"));

            var typeHandlerSelector = container.Resolve<ITypeHandlerSelector>();
            var handler = typeHandlerSelector.SelectTypeHandler(inputArguments.Type);
            handler.HandleTypeAsync(inputArguments, originalOut).Wait();
        }
    }
}