using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Castle.Windsor;

using Tauco.Dicom;
using Tauco.Dicom.Abstraction;
using Tauco.Dicom.IO;
using Tauco.Dicom.Models;

namespace Tester
{
    class Program
    {
        static void Main(string[] args)
        {

            var installer = new Installer();
            var container = new WindsorContainer().Install(new CommonInstaller());
            installer.Install(container);

            var p = container.Resolve<IDicomdirFileCacheStoreProvider>();
            p.StoreItemsInCache(@"C:\Downloads\DICOMDIR").Wait();
            //var patients = p.ParseDicomDir(@"C:\Users\Tomas\OneDrive\Projekty\Skola\diplomka\data\2012-10-12-T2W-TSE-clean\DICOMDIR").Result;

            var pip = container.Resolve<IPatientInfoProvider>();

            var patients = pip.GetPatients().LoadFromCache();
        }
    }
}
