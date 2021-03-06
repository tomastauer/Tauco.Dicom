﻿using Dicom;

using NSubstitute;

using Tauco.Dicom.Shared;

namespace Tauco.Dicom.Abstraction.FellowOak.Tests
{
    internal class FellowOakMockProvider
    {
        public IDicomSOPClassUIDProvider GetDicomSopClassUidProviderFake()
        {
            var dicomSopClassUidProvider = Substitute.For<IDicomSOPClassUIDProvider>();
            dicomSopClassUidProvider.GetDicomSOPClassUid(Arg.Any<IDicomInfo>()).Returns(DicomUID.PatientRootQueryRetrieveInformationModelFIND);

            return dicomSopClassUidProvider;
        }


        public IDicomTagAdapter GetDicomTagAdapterFake()
        {
            var dicomTagAdapter = Substitute.For<IDicomTagAdapter>();
            dicomTagAdapter.GetDicomTag(Arg.Any<DicomTags>()).Returns(c =>
            {
                var tag = c.Arg<DicomTags>();

                if (tag == DicomTags.PatientID)
                {
                    return DicomTag.PatientID;
                }

                return DicomTag.PatientName;
            });

            return dicomTagAdapter;
        }
    }
}