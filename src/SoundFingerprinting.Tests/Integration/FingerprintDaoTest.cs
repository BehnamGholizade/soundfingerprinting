﻿namespace SoundFingerprinting.Tests.Integration
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using SoundFingerprinting.DAO;
    using SoundFingerprinting.DAO.Data;
    using SoundFingerprinting.InMemory;

    [TestClass]
    public class FingerprintDaoTest : AbstractIntegrationTest
    {
        private IFingerprintDao fingerprintDao;
        private ITrackDao trackDao;

        [TestInitialize]
        public void SetUp()
        {
            var ramStorage = new RAMStorage(NumberOfHashTables);
            this.fingerprintDao = new FingerprintDao(ramStorage);
            this.trackDao = new TrackDao(ramStorage);
        }

        [TestMethod]
        public void InsertTest()
        {
            TrackData track = new TrackData("isrc", "artist", "title", "album", 1986, 200);
            var trackReference = this.trackDao.InsertTrack(track);

            var fingerprintReference = this.fingerprintDao.InsertFingerprint(new FingerprintData(this.GenericFingerprint, trackReference));

            this.AssertModelReferenceIsInitialized(fingerprintReference);
        }

        [TestMethod]
        public void MultipleFingerprintsInsertTest()
        {
            const int NumberOfFingerprints = 100;
            for (int i = 0; i < NumberOfFingerprints; i++)
            {
                var trackData = new TrackData("isrc" + i, "artist", "title", "album", 2012, 200);
                var trackReference = this.trackDao.InsertTrack(trackData);
                var fingerprintReference = this.fingerprintDao.InsertFingerprint(new FingerprintData(this.GenericFingerprint, trackReference));

                this.AssertModelReferenceIsInitialized(fingerprintReference);
            }
        }

        [TestMethod]
        public void ReadTest()
        {
            const int NumberOfFingerprints = 100;
            TrackData track = new TrackData("isrc", "artist", "title", "album", 1986, 200);
            var trackReference = this.trackDao.InsertTrack(track);

            for (int i = 0; i < NumberOfFingerprints; i++)
            {
                this.fingerprintDao.InsertFingerprint(new FingerprintData(this.GenericFingerprint, trackReference));
            }

            var fingerprints = this.fingerprintDao.ReadFingerprintsByTrackReference(trackReference);

            Assert.IsTrue(fingerprints.Count == NumberOfFingerprints);

            foreach (var fingerprint in fingerprints)
            {
                Assert.IsTrue(this.GenericFingerprint.Length == fingerprint.Signature.Length);
                for (var i = 0; i < this.GenericFingerprint.Length; i++)
                {
                    Assert.AreEqual(this.GenericFingerprint[i], fingerprint.Signature[i]);
                }
            }
        }
    }
}
