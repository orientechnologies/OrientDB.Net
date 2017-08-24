using NUnit.Framework;
using OrientDB.Net.ConnectionProtocols.Binary.Core;
using System;

namespace OrientDB.Net.ConnectionProtocols.Binary.Tests.CoreTests
{
    [TestFixture]
    public class WhenUsingTheOrientDBBinaryConnection
    {
        [Test]
        public void ItShouldThrowAnExceptionWhenPassedNullOptions()
        {
            Assert.Throws(typeof(ArgumentNullException), () => new OrientDBBinaryConnection(null, null, null));
        }

        [Test]
        public void ItShouldThrowAnExceptionWhenPassedANUllSerializer()
        {
            var options = new DatabaseConnectionOptions
            {

            };

            Assert.Throws(typeof(ArgumentNullException), () => new OrientDBBinaryConnection(options, null, null));
        }

        [Test]
        public void ItShouldThrowAnExceptionWithAZeroLengthOrNullHostname()
        {
            Assert.Throws(typeof(ArgumentException), () => new OrientDBBinaryConnection(null, null, null, null, null));
        }

        [Test]
        public void ItShouldThrowAnExceptionWithAZeroLengthOrNullUsername()
        {
            Assert.Throws(typeof(ArgumentException), () => new OrientDBBinaryConnection("localhost", null, null, null, null));
        }

        [Test]
        public void ItShouldThrowAnExceptionWithAZeroLengthOrNullPassword()
        {
            Assert.Throws(typeof(ArgumentException), () => new OrientDBBinaryConnection("localhost", "root", null, null, null));
        }
    }
}
