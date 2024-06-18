using ukazkaPlc;
using ukazkaPlcConnector;
using NUnit.Framework;
using System.Linq;
using TcoCore;

namespace ukazkaTests
{

    public class TechnologyTests
    {
        private const int timeOut = 7000;
        [SetUp]
        public void Setup()
        {
            System.Threading.Thread.Sleep(500);


        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            ukazkaApp.Get.KillApp();
        }

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            var a = ukazkaApp.Get;
            Entry.LoadAppSettings("testing1", true);
            Entry.Plc.Connector.BuildAndStart();
         

        }

        [Test]
        [Timeout(timeOut)]
        public void run_ground_mode()
        {
            var technology = Entry.Plc.MAIN._technology;
            technology._cu00x._manualTask.Execute();    // This is just to reset all other tasks                                       

            technology._groundAllTask.Execute();

            System.Threading.Thread.Sleep(250);

            Assert.AreEqual(eTaskState.Busy, (eTaskState)technology._cu00x._groundTask._task._taskState.Synchron);
            System.Threading.Thread.Sleep(2500); // Wait for ground to finish.
            Assert.AreEqual(eTaskState.Done, (eTaskState)technology._cu00x._groundTask._task._taskState.Synchron);
            Assert.IsTrue(technology._cu00x._groundTask._groundDone.Synchron);
        }

        [Test]
        [Timeout(timeOut)]
        public void run_automat_mode_ground_not_done()
        {
            var technology = Entry.Plc.MAIN._technology;
            var technologyTask = technology._automatAllTask;
            var cuTask = technology._cu00x._automatTask;
            technology._cu00x._manualTask.Execute();    // This is just to reset all other tasks
            System.Threading.Thread.Sleep(2500);
            Assert.IsFalse(technology._cu00x._groundTask._groundDone.Synchron);

            technologyTask.Execute();

            Assert.AreEqual(eTaskState.Ready, (eTaskState)cuTask._task._taskState.Synchron);
            System.Threading.Thread.Sleep(2500); // Wait for ground to finish.
            Assert.AreEqual(eTaskState.Ready, (eTaskState)cuTask._task._taskState.Synchron);

        }

        [Test]
        [Timeout(timeOut)]
        public void run_automat_mode_ground_done()
        {
            var technology = Entry.Plc.MAIN._technology;
            var technologyTask = technology._automatAllTask;
            var cuTask = technology._cu00x._automatTask;
            technology._cu00x._manualTask._restoreRequest.Synchron = true; ;
            technology._cu00x._manualTask.Execute();    // This is just to reset all other tasks
            System.Threading.Thread.Sleep(250);
            while ((eTaskState)technology._cu00x._manualTask._taskState.Synchron != eTaskState.Busy) ;
            technology._groundAllTask.Execute();    // This is just to reset all other tasks                                                        

            while ((eTaskState)technology._cu00x._groundTask._task._taskState.Synchron != eTaskState.Done) ;

            Assert.IsTrue(technology._cu00x._groundTask._groundDone.Synchron);

            technologyTask.Execute();

            while ((eTaskState)cuTask._task._taskState.Synchron != eTaskState.Busy) ;


        }
    }
}
