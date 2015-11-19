using System;
using System.Linq;

using Castle.Windsor;
using Castle.Windsor.Installer;

using Nito.AsyncEx;

using Tauco.Cache;
using Tauco.Dicom.Abstraction;
using Tauco.Dicom.Models;
using Tauco.Dicom.Models.InfoObjects;

namespace Tauco.Dicom
{
    class Program
    {
        private IPatientInfoProvider mPatientInfoProvider;
        private static ICacheProvider cacheProvider;

        private static AsyncCollection<string> coll = new AsyncCollection<string>();


        //static void TakeNext()
        //{
            //var client = new DicomClient();
            //client.NegotiateAsyncOps();

            //var dimse = new DicomNCreateRequest(DicomUID.PatientRootQueryRetrieveInformationModelFIND, DicomUID.PatientRootQueryRetrieveInformationModelFIND, 1);
            //dimse.Dataset = new DicomDataset();
            //dimse.Dataset.Add(DicomTag.PatientID, "test");
            //dimse.Dataset.Add(DicomTag.PatientName, "testName");
            
            //client.AddRequest(dimse);
            //client.Send("147.251.20.72", 2350, false, "SeqViewer", "PACS2_MGR");


            //var generator = new DicomUIDGenerator();
            //var uid = generator.Generate();

            //var request = new DicomNCreateRequest(DicomUID.ModalityPerformedProcedureStepSOPClass, uid, 0);
            //request.Dataset = new DicomDataset();
            //request.Dataset.Add(DicomTag.PatientID, "test");
            //request.Dataset.Add(DicomTag.PatientName, "testName");

            //var client = new DicomClient();
            //client.NegotiateAsyncOps();
            //client.AddRequest(request);
            //client.Send("147.251.20.72", 2350, false, "SeqViewer", "PACS2_MGR");

            //List<DicomCStoreRequest> requests = new List<DicomCStoreRequest>();

            //DirectoryInfo di = new DirectoryInfo(@"\\ad.fi.muni.cz\DFS\home\xtauer\_profile\Downloads\NovakII\NovakII\DICOM");
            //var files = di.GetFiles();

            //foreach(var file in files)
            //{
            //    requests.Add(new DicomCStoreRequest(file.FullName));
            //}

            //var client = new DicomClient();

            //foreach (var request in requests)
            //{
            //    client.AddRequest(request);
            //}

            //client.Send("147.251.20.72", 2350, false, "SeqViewer", "PACS2_MGR");

            //var server = new DicomServer();


            ////PatientInfoProvier

            //var patients = PatientInfoProvider.GetPatients();
            //foreach (var patient in patients)
            //    {
            //    Console.WriteLine(patient.PatientID + " " + patient.PatientName);
            //    }
                
        static void Main(string[] args)
        {


            AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;


            //var reg = new Registers.AutoMapperRegister();
            //reg.Register();


            var container = new WindsorContainer().Install(new CommonInstaller());


            //         var image = new DicomImage("1.3.6.1.4.1.5962.99.1.2475717327.938511825.1398340121295.11.0.dcm");

            //var clientFactory = container.Resolve<IDicomClientFactory<PatientInfo>>();
            //var client = clientFactory.CreateDicomClient();
            //client.AddPingRequest();
            //client.SendAsync().Wait();

            var mPatientInfoProvider = container.Resolve<IPatientInfoProvider>();
            var a = mPatientInfoProvider.GetPatients().ToListAsync().Result;
         //   a.UseCache = true;
            
            mPatientInfoProvider.DownloadImagesAsync(a.First());

            ////var w = mPatientInfoProvider.GetPatientByBirthNumber("001");
            var q = "asd";
            ////q.Contains("asd", CultureInfo.CurrentCulture);



            //a.GetItemAsync().ContinueWith(c =>
            //{
            //    Console.WriteLine(c.Result.PatientID);
            //});
            //var b = a.ToList();
            //var b = a.ToList();
            //var c = a.WhereEquals(DicomTag.PatientID, "001").ToList();
            //var somePatient = b[5];
       //     mPatientInfoProvider.DownloadImagesAsync(list.First());
            //mPatientInfoProvider.DownloadImagesAsync(somePatient);

            ////a.GetItemAsync().ContinueWith(c =>
            //{
            //   Console.WriteLine(c.Result.PatientID);
            //});

            //a.GetItemAsync().ContinueWith(c =>
            //{
            //    Console.WriteLine(c.Result.PatientID);
            //});
            


            ////AsyncCollection<string> coll = new AsyncCollection<string>();

            //System.Timers.Timer aTimer = new System.Timers.Timer();
            //aTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            //aTimer.Interval = 1000;
            //aTimer.Enabled = true;



            //coll.AddAsync("test");

            //TakeNext();
            //var a  = coll.TakeAsync();
            //a.ContinueWith(c =>
            //{
            //    Console.WriteLine(c.Result);
            //    a = coll.TakeAsync();
            //    coll.AddAsync("test");

            //}
            //);


            //var container = new WindsorContainer().Install(FromAssembly.This());
            //using (var cacheProvider = container.Resolve<ICacheProvider>())
            //{
            //    //var patient = mPatientInfoProvider.GetPatients().ToList().First();

            //    var patient = new PatientInfo()
            //    {
            //        PatientID = "91011240989",
            //        PatientName = "Tauer^Tomas"
            //    };

            //    //cacheProvider.Store(patient.PatientID.StringRepresentation, patient);

            //    var val = cacheProvider.Retrieve<PatientInfo>("asdas");
            //}
            ////cacheProvider.Store("ts", false);

            
           // //var value = provider.Retrieve<string>("test");


           // //var server = new DicomServer();

           // //var container = new WindsorContainer();
           // //container.Register(Component.For<IPatientInfoProvider>().ImplementedBy<PatientInfoProvider>());
            
           // ////var client = new DicomClient();
           // ////client.NegotiateAsyncOps();
           // ////client.AddRequest(new DicomCMoveRequest("123test", "1.2.826.0.1.3680043.8.1103.3.28.2011120711085016"));
           // //////client.Send("localhost", 104, false, "test", "test");
           // ////var generator = new DicomUIDGenerator();
           // ////var uid = generator.Generate();

           // ////var request = new DicomNCreateRequest(DicomUID.ModalityPerformedProcedureStepSOPClass, uid, 0);
           // ////request.Dataset = new DicomDataset();
           // ////request.Dataset.Add(DicomTag.PatientID, "test");
           // ////request.Dataset.Add(DicomTag.PatientName, "testName");

           // ////var client = new DicomClient();
           // ////client.NegotiateAsyncOps();
           // ////client.AddRequest(new DicomCEchoRequest());
           // ////client.Send("10.0.0.3", 104, false, "JVSDICOM", "JVSDICOM");

           // //var patientInfoProvider = new PatientInfoProvider();

           // //var a = patientInfoProvider.GetPatients().ToList();

            


           // //            var list = PatientInfoProvider.GetPatientByBirthNumber("12345").Studies.ToList();
           // //var study2 = PatientInfoProvider.GetPatientByBirthNumber("12345").Studies.First();
           // //var study = StudyInfoProvider.GetStudyByID("1.2.826.0.1.3680043.2.44.6.20061103012526.400.2");

           // //foreach (
           // //    var serie in
           // //        SeriesInfoProvider.GetSeries()
           // //            .WhereEquals(DicomTag.StudyInstanceUID, "1.2.826.0.1.3680043.2.44.6.20061103012526.400.2"))
           // //{
           // //   Console.WriteLine(serie.StudyInstanceUID);
           // //    Console.WriteLine(serie.SeriesInstanceUID);
           // //}

           // //var list =
           // //    ImageInfoProvider.GetImages().WhereEquals(DicomTag.SeriesInstanceUID, "1.2.826.0.1.3680043.2.44.6.20061103012526.400.2.1").First();
            
           // //var client = new DicomClient();
           // //client.NegotiateAsyncOps();
           // //client.AddRequest(new DicomCMoveRequest("123test", "1.2.826.0.1.3680043.8.1103.3.28.2011120711085016"));
           // ////client.Send("localhost", 104, false, "test", "test");
            
            //var firstStudy = firstPatient.Studies.First();
            //foreach(var series in firstStudy.Series)
            //{
            //    Console.WriteLine(series.Modality + " " + series.SeriesNumber);
            //}
           // //client.Send("213.165.94.158", 11112, false, "123test", "123test");
           // //foreach (
           // //       var image in
           // //           ImageInfoProvider.GetImages())
           // //{
           // //    Console.WriteLine(image.ImageType);
           // //}
            
            Console.ReadLine();

            //var anotherPatient = PatientInfoProvider.GetPatientByBirthNumber("2879");
            //if(anotherPatient != null){
            //    anotherPatient.Studies.First().DownloadImagesAsync();
            //}
        }


        private static void UnhandledExceptionTrapper(object sender, UnhandledExceptionEventArgs e)
        {
            throw new NotImplementedException();
        }


        static void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            //cacheProvider.Dispose();
        }
    }
}
