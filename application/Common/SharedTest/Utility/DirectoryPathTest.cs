using System;
using System.IO;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MORR.Shared.Utility;

namespace SharedTest.Utility
{
    [TestClass]
    public class DirectoryPathTest
    {
        [TestMethod]
        public void TestDirectoryPath_ValidPath()
        {
            /* GIVEN */
            var assemblyPath = Assembly.GetExecutingAssembly().Location;
            var validPath = Path.GetDirectoryName(assemblyPath) + "\\";

            /* WHEN */
            var directoryPath = new DirectoryPath(validPath);

            /* THEN */
            Assert.AreEqual(validPath, directoryPath.ToString());
        }

        [TestMethod]
        public void TestDirectoryPath_InvalidPath()
        {
            /* GIVEN */
            var invalidPath = Assembly.GetExecutingAssembly().Location;

            /* WHEN */
            Assert.ThrowsException<ArgumentException>(() => new DirectoryPath(invalidPath));
        }

        [TestMethod]
        public void TestDirectoryPath_InvalidPath_NoValidation()
        {
            /* GIVEN */
            var invalidPath = Assembly.GetExecutingAssembly().Location;

            /* WHEN */
            var directoryPath = new DirectoryPath(invalidPath, true);

            /* THEN */
            Assert.AreEqual(invalidPath, directoryPath.ToString());
        }
    }
}
