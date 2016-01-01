using System;
using System.Collections.Generic;
using System.Linq;

using Dicom;
using Dicom.Network;

using NSubstitute;

using NUnit.Framework;

using Tauco.Dicom.Shared;
using Tauco.Tests;
using Tauco.Tests.Fakes;

namespace Tauco.Dicom.Abstraction.FellowOak.Tests
{
    [TestFixture]
    public class FellowOakDicomFindRequestTests
    {
        [Test]
        public void Constructor_ArgumentsCombinations()
        {
            var mockProvider = new MockProvider();
            var fellowOakMockProvider = new FellowOakMockProvider();
            TestUtilities.TestConstructorArgumentsNullCombinations(typeof(FellowOakDicomFindRequest<>), new[] { typeof(TestInfo) }, new List<Func<object>>
            {
                mockProvider.GetDicomMappingFake,
                fellowOakMockProvider.GetDicomTagAdapterFake,
                mockProvider.GetDicomInfoBuilderFake,
                mockProvider.GetGeneralizedInfoProviderFake,
                fellowOakMockProvider.GetDicomSopClassUidProviderFake,
                GetActionFake,
                mockProvider.GetDicomWhereCollectionFake
            });
        }


        [Test]
        public void Constructor_CreatesInnerRequestWithWhereConditions()
        {
            // Arrange
            var mockProvider = new MockProvider();
            var fellowOakMockProvider = new FellowOakMockProvider();
            var whereCollection = mockProvider.GetWhereCollection();
            whereCollection.WhereEquals(DicomTags.PatientID, "PatientIDValue");
            whereCollection.WhereLike(DicomTags.PatientName, "PatientNameValue");

            // Act
            var dicomFindRequest = new FellowOakDicomFindRequest<TestInfo>(mockProvider.GetDicomMappingFake(), fellowOakMockProvider.GetDicomTagAdapterFake(), mockProvider.GetDicomInfoBuilderFake(), mockProvider.GetGeneralizedInfoProviderFake(), fellowOakMockProvider.GetDicomSopClassUidProviderFake(), c => {}, whereCollection.GetDicomWhereCollections().First());
            var innerRequestDataset = dicomFindRequest.InnerRequest.Dataset;

            // Assert
            Assert.That(innerRequestDataset.Get<string>(DicomTag.PatientID), Is.EqualTo("PatientIDValue"));
            Assert.That(innerRequestDataset.Get<string>(DicomTag.PatientName), Is.EqualTo("*PatientNameValue*"));
        }


        [Test]
        public void Constructor_ResponseCallbackIsCalled()
        {
            // Arrange
            var mockProvider = new MockProvider();
            var fellowOakMockProvider = new FellowOakMockProvider();
            var whereCollection = mockProvider.GetWhereCollection();
            var infoBuilder = mockProvider.GetDicomInfoBuilderFake();
            infoBuilder.BuildInfo<TestInfo>(Arg.Any<object>()).Returns(new TestInfo
            {
                PatientID = 666,
                PatientName = "patientName"
            });

            var response = new DicomCFindResponse(new DicomDataset())
            {
                Dataset = new DicomDataset()
            };
            TestInfo responseItem = null;
            var responseAction = new Action<TestInfo>(item =>
            {
                responseItem = item;
            });

            // Act
            var dicomFindRequest = new FellowOakDicomFindRequest<TestInfo>(mockProvider.GetDicomMappingFake(), fellowOakMockProvider.GetDicomTagAdapterFake(), infoBuilder, mockProvider.GetGeneralizedInfoProviderFake(), fellowOakMockProvider.GetDicomSopClassUidProviderFake(), responseAction, whereCollection.GetDicomWhereCollections().First());
            var innerRequest = (DicomCFindRequest)dicomFindRequest.InnerRequest;
            
            innerRequest.OnResponseReceived(null, response);

            // Assert
            Assert.That(responseItem.PatientID, Is.EqualTo(666));
            Assert.That(responseItem.PatientName, Is.EqualTo("patientName"));
        }



        private Action<TestInfo> GetActionFake()
        {
            return c => { };
        }
    }
}
