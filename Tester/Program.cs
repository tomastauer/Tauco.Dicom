using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

using Castle.MicroKernel.Registration;
using Castle.Windsor;

using Tauco.Dicom;
using Tauco.Dicom.Abstraction;
using Tauco.Dicom.IO;

namespace Tester
{
    class Program
    {
        static void Main(string[] args)
        {

            var installer = new Installer();
            var container = new WindsorContainer().Install(new CommonInstaller());
            installer.Install(container);

            var parser = container.Resolve<IDicomdirFileParser>();
            var result = parser.ParseDicomdirAsync(@"C:\Users\Tomas\OneDrive\Projekty\Skola\diplomka\data\2012-07-13-T2W-TSE-clean\DICOMDIR").Result;

            var s = container.Resolve<IDicomdirImageStorageProvider>();
            s.StoreImagesFromDicomdir(result);
        }
    }
}
