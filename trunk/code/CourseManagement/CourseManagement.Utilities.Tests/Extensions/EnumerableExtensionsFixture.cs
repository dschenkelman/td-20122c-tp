namespace CourseManagement.Utilities.Tests.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    
    using Utilities.Extensions;

    [TestClass]
    public class EnumerableExtensionsFixture
    {
        [TestMethod]
        public void ShouldApplyFunctionToAllElementsInEnumerableWhenInvokingForEach()
        {
            // arrange
            Item i1 = new Item { Invoked = false };
            Item i2 = new Item { Invoked = false };
            Item i3 = new Item { Invoked = false };

            Action<Item> markInvoked = i => i.Invoked = true;

            IEnumerable<Item> items = new List<Item> { i1, i2, i3 };

            // act
            items.ForEach(markInvoked);

            // assert
            Assert.IsTrue(items.All(i => i.Invoked));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ShouldThrowIfEnumerableIsNullWhenInvokingForEach()
        {
            // arrange
            Action<Item> markInvoked = i => i.Invoked = true;

            IEnumerable<Item> items = null;

            // act
            items.ForEach(markInvoked);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ShouldThrowIfActionIsNullWhenInvokingForEach()
        {
            // arrange
            Action<Item> markInvoked = null;

            IEnumerable<Item> items = new List<Item>();

            // act
            items.ForEach(markInvoked);
        }

        private class Item
        {
            public bool Invoked { get; set; }
        }
    }
}
