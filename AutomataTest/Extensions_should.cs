using Infrastructure;
using NUnit.Framework;

namespace AutomataTest
{
    public class Extensions_should
    {
        [Test]
        public void CountCompoundSets_ReturnsCorrectValue()
        {
            var states = new HashSet<string> { "0", "1", "Ø", "{0, 2}", "{0, 1}" };
            var actual = states.CountCompoundSets();
            Assert.AreEqual(2, actual);
        }

        [Test]
        public void HashSetToString_ReturnsMultiElementString_OnMultiElementSet()
        {
            var source = new HashSet<string> { "1", "2", "3" };
            var actual = source.SetToString();
            Assert.AreEqual("{1, 2, 3}", actual);
        }

        [Test]
        public void HashSetToString_ReturnsSingleElementString_OnSingleElementHashSet()
        {
            var source = new HashSet<string> { "1" };
            var actual = source.SetToString();
            Assert.AreEqual("1", actual);
        }

        [Test]
        public void StringToSet_ReturnsHashSet_OnMultiElementSet()
        {
            var source = "{1, 2, 3}";
            var actual = source.StringToSet();
            CollectionAssert.AreEquivalent(new HashSet<string> { "1", "2", "3" }, actual);
        }
    }
}
