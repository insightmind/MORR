using System;
using System.IO;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MORR.Shared.Utility;

namespace SharedTest.Utility
{
    [TestClass]
    public class FilePathTest
    {
        [TestMethod]
        public void TestFilePath_ValidPath()
        {
            /* GIVEN */
            var validPath = Assembly.GetExecutingAssembly().Location;

            /* WHEN */
            var filePath = new FilePath(validPath);

            /* THEN */
            Assert.AreEqual(validPath, filePath.ToString());
        }

        [TestMethod]
        public void TestFilePath_InvalidPath()
        {
            /* GIVEN */
            var assemblyPath = Assembly.GetExecutingAssembly().Location;
            var invalidPath = Path.GetDirectoryName(assemblyPath) + "\\";


            /* WHEN */
            Assert.ThrowsException<ArgumentException>(() => new FilePath(invalidPath));
        }

        [TestMethod]
        public void TestFilePath_InvalidPath_NoValidation()
        {
            /* GIVEN */
            var assemblyPath = Assembly.GetExecutingAssembly().Location;
            var invalidPath = Path.GetDirectoryName(assemblyPath) + "\\";

            /* WHEN */
            var filePath = new FilePath(invalidPath, true);

            /* THEN */
            Assert.AreEqual(invalidPath, filePath.ToString());
        }
    }
}
