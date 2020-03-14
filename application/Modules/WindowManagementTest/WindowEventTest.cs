using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MORR.Modules.WindowManagement.Events;
using Point = System.Windows.Point;
using System.Windows;
using Size = System.Drawing.Size;

namespace WindowManagementTest
{
    [TestClass]
    public class WindowEventTest
    {
        public class WindowEventImp : WindowEvent { }

        [TestMethod]
        public void TestWindowEvent_Title()
        {
            /* GIVEN */
            var @event = new WindowEventImp();
            var title = "sample title";

            /* WHEN */
            @event.Title = title;

            /* THEN */
            Assert.AreEqual(title, @event.Title);
        }

        [TestMethod]
        public void TestWindowEvent_ProcessName()
        {
            /* GIVEN */
            var @event = new WindowEventImp();
            var processName = "sample process name";

            /* WHEN */
            @event.ProcessName = processName;

            /* THEN */
            Assert.AreEqual(processName, @event.ProcessName);
        }

        [TestMethod]
        public void TestWindowMovementEvent_OldLocation()
        {
            /* GIVEN */
            var @event = new WindowMovementEvent();
            var oldLocation = new Point(1,1);

            /* WHEN */
            @event.OldLocation = oldLocation;

            /* THEN */
            Assert.AreEqual<Point>(oldLocation, @event.OldLocation);
        }
        [TestMethod]
        public void TestWindowMovementEvent_NewLocation()
        {
            /* GIVEN */
            var @event = new WindowMovementEvent();
            var newLocation = new Point(1, 1);

            /* WHEN */
            @event.NewLocation = newLocation;

            /* THEN */
            Assert.AreEqual<Point>(newLocation, @event.NewLocation);
        }

        [TestMethod]
        public void TestWindowResizingEvent_OldSize()
        {
            /* GIVEN */
            var @event = new WindowResizingEvent();
            var oldSize = new Size(1,1);

            /* WHEN */
            @event.OldSize = oldSize;

            /* THEN */
            Assert.AreEqual<Size>(oldSize, @event.OldSize);
        }
        [TestMethod]
        public void TestWindowResizingEvent_NewSize()
        {
            /* GIVEN */
            var @event = new WindowResizingEvent();
            var newSize = new Size(1, 1);

            /* WHEN */
            @event.NewSize = newSize;

            /* THEN */
            Assert.AreEqual<Size>(newSize, @event.NewSize);
        }

        [TestMethod]
        public void TestWindowStateChangedEvent_WindowState()
        {
            /* GIVEN */
            var @event = new WindowStateChangedEvent();
            var windowState = new WindowState();

            /* WHEN */
            @event.WindowState = windowState;

            /* THEN */
            Assert.AreEqual<WindowState>(windowState, @event.WindowState);
        }

    }
}
