using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using AutoMapper;

using Castle.Core.Logging;

using NSubstitute;

using Tauco.Cache;
using Tauco.Dicom.Abstraction;
using Tauco.Dicom.Models;
using Tauco.Dicom.Network;
using Tauco.Dicom.Shared;

namespace Tauco.Tests.Fakes
{
    internal class MockProvider
    {
        public ICacheProvider GetCacheProviderFake()
        {
            var cacheProvider = Substitute.For<ICacheProvider>();
            cacheProvider.Retrieve<TestInfo>().Returns(c =>
            {
                var result = new List<TestInfo>
                {
                    new TestInfo
                    {
                        PatientID = 0
                    },
                    new TestInfo
                    {
                        PatientID = 1
                    },
                    new TestInfo
                    {
                        PatientID = 10
                    }
                };
                return result;
            });
            return cacheProvider;
        }


        public IDicomMapping GetDicomMappingFake()
        {
            var mappingFake = new Dictionary<PropertyInfo, DicomTags>
            {
                {
                    typeof (TestInfo).GetProperty("PatientID"), DicomTags.PatientID
                },
                {
                    typeof (TestInfo).GetProperty("PatientName"), DicomTags.PatientName
                }
            };

            var dicomMapping = Substitute.For<IDicomMapping>();
            dicomMapping.GetEnumerator().Returns(_ =>
            {
                return mappingFake.ToDictionary(c => c.Key, c => c.Value).GetEnumerator();
            });

            return dicomMapping;
        }


        public IDicomClient<TestInfo> GetDicomClientFake()
        {
            var dicomClient = Substitute.For<IDicomClient<TestInfo>>();

            dicomClient.When(c => c.AddFindRequest(Arg.Any<IDicomFindRequest<TestInfo>>())).Do(c =>
            {
                var findRequest = c.Arg<IDicomFindRequest<TestInfo>>() ?? Substitute.For<IDicomFindRequest<TestInfo>>();

                findRequest.ResponseCallback(new TestInfo
                {
                    PatientID = 1
                });
            });

            return dicomClient;
        }


        public IDicomRequestFactory GetDicomRequestFactoryFake()
        {
            var dicomRequestFactory = Substitute.For<IDicomRequestFactory>();
            dicomRequestFactory.CreateDicomFindRequest(Arg.Any<IDicomWhereCollection>(), Arg.Any<Action<TestInfo>>()).Returns(c => new DicomFindRequest<TestInfo>
            {
                DicomWhereCollection = c.Arg<IDicomWhereCollection>(),
                ResponseCallback = c.Arg<Action<TestInfo>>()
            });

            return dicomRequestFactory;
        }


        public ICacheIndexProvider GetCacheIndexProviderFake()
        {
            var cacheIndexProvider = Substitute.For<ICacheIndexProvider>();

            return cacheIndexProvider;
        }


        public IDicomQuery<TestInfo> GetDicomQuery()
        {
            return new DicomQuery<TestInfo>(GetGeneralizedInfoProviderFake(), GetDicomDataLoaderFake(), GetWhereCollection());
        }


        public ISettingsProvider GetSettingsProviderFake()
        {
            var settingsProvider = Substitute.For<ISettingsProvider>();

            return settingsProvider;
        }


        public IDicomRequestAdapter<TestInfo> GetRequestAdapterFake()
        {
            var requestAdapter = Substitute.For<IDicomRequestAdapter<TestInfo>>();

            return requestAdapter;
        }


        public IDicomClientFactory<TestInfo> GetDicomClientFactoryFake()
        {
            var dicomClientFactory = Substitute.For<IDicomClientFactory<TestInfo>>();
            var dicomClient = GetDicomClientFake();
            dicomClientFactory.CreateDicomClient().ReturnsForAnyArgs(dicomClient);

            return dicomClientFactory;
        }


        public IDicomClientFactory<TestInfo> GetDicomClientFactoryFake(IDicomClient<TestInfo> dicomClient)
        {
            var dicomClientFactory = Substitute.For<IDicomClientFactory<TestInfo>>();
            dicomClientFactory.CreateDicomClient().ReturnsForAnyArgs(dicomClient);
            return dicomClientFactory;
        }


        public IWhereCollection<TestInfo> GetWhereCollectionFake()
        {
            var whereCollection = Substitute.For<IWhereCollection<TestInfo>>();

            return whereCollection;
        }


        public IGeneralizedInfoProvider GetGeneralizedInfoProviderFake()
        {
            var generalizedInfoProvider = Substitute.For<IGeneralizedInfoProvider>();
            generalizedInfoProvider.GetGeneralizedInfo<TestInfo>().Returns(new TestInfo());

            return generalizedInfoProvider;
        }


        public IDicomDataLoader<TestInfo> GetDicomDataLoaderFake()
        {
            var dicomDataLoader = Substitute.For<IDicomDataLoader<TestInfo>>();

            return dicomDataLoader;
        }


        public IDicomDataLoader<TestInfo> GetDicomDataLoader()
        {
            return new DicomDataLoader<TestInfo>(GetCacheProviderFake(), GetCacheIndexProviderFake(), GetDicomClientFactoryFake(), GetDicomRequestFactoryFake());
        }


        public IWhereCollection<TestInfo> GetWhereCollection()
        {
            return new WhereCollection<TestInfo>(new DicomMapping(typeof (TestInfo)));
        }


        public IDicomWhereCollection GetDicomWhereCollectionFake()
        {
            var dicomWhereCollection = Substitute.For<IDicomWhereCollection>();

            return dicomWhereCollection;
        }


        public IDicomServerFactory GetDicomServerFactoryFake()
        {
            var dicomServerFactory = Substitute.For<IDicomServerFactory>();

            return dicomServerFactory;
        }


        public ILogger GetLoggerFake()
        {
            var loggerFake = Substitute.For<ILogger>();

            return loggerFake;
        }


        public IMappingEngine GetMappingEngine()
        {
            var mappingEngine = Substitute.For<IMappingEngine>();
            mappingEngine.DynamicMap(Arg.Any<object>(), typeof (string), Arg.Any<Type>()).Returns(c =>
            {
                var value = c.Arg<object>();
                var destType = c.ArgAt<Type>(2);

                return Convert.ChangeType(value, destType);
            });

            return mappingEngine;
        }


        public IDicomQueryProvider<PatientInfo> GetDicomQueryProviderForPatientsFake()
        {
            var dicomQueryProvider = Substitute.For<IDicomQueryProvider<PatientInfo>>();

            return dicomQueryProvider;
        }


        public IDicomQueryProvider<StudyInfo> GetDicomQueryProviderForStudiesFake()
        {
            var dicomQueryProvider = Substitute.For<IDicomQueryProvider<StudyInfo>>();

            return dicomQueryProvider;
        }


        public IDicomQueryProvider<SeriesInfo> GetDicomQueryProviderForSeriesFake()
        {
            var dicomQueryProvider = Substitute.For<IDicomQueryProvider<SeriesInfo>>();

            return dicomQueryProvider;
        }


        public IDicomDownloader<StudyInfo> GetDicomDownloaderForStudiesFake()
        {
            var dicomDownloader = Substitute.For<IDicomDownloader<StudyInfo>>();

            return dicomDownloader;
        }


        public IStudyInfoProvider GetStudyInfoProviderFake()
        {
            var studyInfoProvider = Substitute.For<IStudyInfoProvider>();

            return studyInfoProvider;
        }


        public IBirthNumberParser GetBirthNumberParserFake()
        {
            var birthNumberParser = Substitute.For<IBirthNumberParser>();
            birthNumberParser.GetBirthNumber(Arg.Any<string>()).Returns(c => new BirthNumber(c.Arg<string>().Replace("/", "")));

            return birthNumberParser;
        }


        public IPatientInfoProvider GetPatientInfoProviderFake()
        {
            var patientInfoProvider = Substitute.For<IPatientInfoProvider>();

            return patientInfoProvider;
        }


        public ISeriesInfoProvider GetSeriesInfoProviderFake()
        {
            var seriesInfoProvider = Substitute.For<ISeriesInfoProvider>();

            return seriesInfoProvider;
        }
    }
}