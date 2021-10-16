using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;

using yapsi.Default;

namespace yapsi.Test
{
    [TestClass]
    public class UnitTest
    {
        [TestMethod]
        public void TestSimplePubSub()
        {
            using var pipeline = new Pipeline<int>();

            Assert.AreEqual(0, pipeline.Subscriptions.Count);
            Assert.AreEqual(0, pipeline.Contracts.Count);

            var contract = pipeline.Bind();

            Assert.AreEqual(0, pipeline.Subscriptions.Count);
            Assert.AreEqual(1, pipeline.Contracts.Count);
            Assert.IsFalse(contract.IsCancelled);

            var subscription = pipeline.Subscribe();

            Assert.AreEqual(1, pipeline.Subscriptions.Count);
            Assert.AreEqual(1, pipeline.Contracts.Count);
            Assert.IsFalse(subscription.IsCancelled);
            Assert.IsFalse(subscription.IsPaused);

            int? data = null;
            subscription.Published += (sender, d) => data = d;

            contract.Publish(5);

            Assert.AreEqual(5, data);
        }

        [TestMethod]
        public void TestMultiplePubSub()
        {
            using var pipeline = new Pipeline<int>();

            Assert.AreEqual(0, pipeline.Subscriptions.Count);
            Assert.AreEqual(0, pipeline.Contracts.Count);

            var contractA = pipeline.Bind();
            var contractB = pipeline.Bind();

            Assert.AreEqual(0, pipeline.Subscriptions.Count);
            Assert.AreEqual(2, pipeline.Contracts.Count);

            var subscriptionA = pipeline.Subscribe();
            var subscriptionB = pipeline.Subscribe();

            Assert.AreEqual(2, pipeline.Subscriptions.Count);
            Assert.AreEqual(2, pipeline.Contracts.Count);

            int? dataA = null;
            int? dataB = null;
            subscriptionA.Published += (sender, d) => dataA = d;
            subscriptionB.Published += (sender, d) => dataB = d;

            contractA.Publish(5);
            Assert.AreEqual(5, dataA);
            Assert.AreEqual(5, dataB);

            contractB.Publish(3);
            Assert.AreEqual(3, dataA);
            Assert.AreEqual(3, dataB);
        }

        [TestMethod]
        public void TestBinding()
        {
            using var pipeline = new Pipeline<int>();

            Assert.AreEqual(0, pipeline.Subscriptions.Count);
            Assert.AreEqual(0, pipeline.Contracts.Count);

            _ = pipeline.Bind();

            Assert.AreEqual(0, pipeline.Subscriptions.Count);
            Assert.AreEqual(1, pipeline.Contracts.Count);
        }

        [TestMethod]
        public void TestCancelContract()
        {
            using var pipeline = new Pipeline<int>();
            var contract = pipeline.Bind();

            Assert.IsFalse(contract.IsCancelled);

            int? data = null;
            var subscription = pipeline.Subscribe();
            subscription.Published += (sender, d) => data = d;

            contract.Publish(5);
            Assert.AreEqual(5, data);

            contract.Cancel();
            Assert.IsTrue(contract.IsCancelled);
            Assert.ThrowsException<OperationCanceledException>(() => contract.Publish(1));
        }

        [TestMethod]
        public void TestCancelSubscription()
        {
            using var pipeline = new Pipeline<int>();
            var contract = pipeline.Bind();

            Assert.AreEqual(0, pipeline.Subscriptions.Count);

            int? data = null;
            var subscription = pipeline.Subscribe();
            subscription.Published += (sender, d) => data = d;

            Assert.AreEqual(1, pipeline.Subscriptions.Count);
            Assert.IsFalse(subscription.IsCancelled);

            contract.Publish(5);
            Assert.AreEqual(5, data);

            subscription.Cancel();
            Assert.IsTrue(subscription.IsCancelled);
            Assert.AreEqual(0, pipeline.Subscriptions.Count);

            contract.Publish(1);
            Assert.AreEqual(5, data);
        }

        [TestMethod]
        public void TestPauseResumeSubscription()
        {
            using var pipeline = new Pipeline<int>();
            var contract = pipeline.Bind();

            Assert.AreEqual(0, pipeline.Subscriptions.Count);

            int? data = null;
            var pausedTriggered = false;
            var resumedTriggered = false;
            var subscription = pipeline.Subscribe();
            subscription.Published += (sender, d) => data = d;
            subscription.Paused += (sender) => pausedTriggered = true;
            subscription.Resumed += (sender) => resumedTriggered = true;

            Assert.AreEqual(1, pipeline.Subscriptions.Count);
            Assert.IsFalse(subscription.IsPaused);
            Assert.IsFalse(pausedTriggered);
            Assert.IsFalse(resumedTriggered);

            contract.Publish(5);
            Assert.AreEqual(5, data);

            subscription.Pause();
            Assert.IsTrue(subscription.IsPaused);
            Assert.AreEqual(1, pipeline.Subscriptions.Count);
            Assert.IsTrue(pausedTriggered);

            contract.Publish(1);
            Assert.AreEqual(5, data);

            subscription.Resume();
            Assert.IsFalse(subscription.IsPaused);
            Assert.IsTrue(resumedTriggered);

            contract.Publish(8);
            Assert.AreEqual(8, data);
        }

        [TestMethod]
        public void TestDisposed()
        {
            var pipeline = new Pipeline<int>();
            var subscription = pipeline.Subscribe();
            int? data = null;
            subscription.Published += (sender, d) => data = d;

            using (var contract = pipeline.Bind())
            {
                contract.Publish(8);
            }

            Assert.AreEqual(0, pipeline.Contracts.Count);
            Assert.AreEqual(8, data);

            using (var contract = pipeline.Bind())
            {
                contract.Publish(5);
            }

            Assert.AreEqual(0, pipeline.Contracts.Count);
            Assert.AreEqual(5, data);
        }

        [TestMethod]
        public void TestSingleBind()
        {
            using var pipeline = new SingleBindPipeline<int>();

            _ = pipeline.Bind();

            Assert.ThrowsException<InvalidOperationException>(() => pipeline.Bind());
        }

        [TestMethod]
        public void TestSingleSubscription()
        {
            using var pipeline = new SingleSubscribePipeline<int>();

            _ = pipeline.Subscribe();

            Assert.ThrowsException<InvalidOperationException>(() => pipeline.Subscribe());
        }
    }
}
