using NUnit.Framework;
using ShieldWall.Audio;

namespace ShieldWall.Tests.Editor
{
    [TestFixture]
    public class AudioUtilityTests
    {
        [Test]
        public void NormalizedToDecibels_ZeroInput_ReturnsMinDecibels()
        {
            float result = AudioUtility.NormalizedToDecibels(0f);
            Assert.AreEqual(AudioUtility.MIN_DECIBELS, result);
        }

        [Test]
        public void NormalizedToDecibels_NegativeInput_ReturnsMinDecibels()
        {
            float result = AudioUtility.NormalizedToDecibels(-0.5f);
            Assert.AreEqual(AudioUtility.MIN_DECIBELS, result);
        }

        [Test]
        public void NormalizedToDecibels_OneInput_ReturnsZero()
        {
            float result = AudioUtility.NormalizedToDecibels(1f);
            Assert.AreEqual(0f, result, 0.001f);
        }

        [Test]
        public void NormalizedToDecibels_HalfInput_ReturnsApproximatelyMinusSix()
        {
            float result = AudioUtility.NormalizedToDecibels(0.5f);
            Assert.AreEqual(-6.02f, result, 0.1f);
        }

        [Test]
        public void NormalizedToDecibels_TenPercentInput_ReturnsMinusTwenty()
        {
            float result = AudioUtility.NormalizedToDecibels(0.1f);
            Assert.AreEqual(-20f, result, 0.001f);
        }

        [Test]
        public void DecibelsToNormalized_MinDecibels_ReturnsZero()
        {
            float result = AudioUtility.DecibelsToNormalized(AudioUtility.MIN_DECIBELS);
            Assert.AreEqual(0f, result);
        }

        [Test]
        public void DecibelsToNormalized_BelowMinDecibels_ReturnsZero()
        {
            float result = AudioUtility.DecibelsToNormalized(-100f);
            Assert.AreEqual(0f, result);
        }

        [Test]
        public void DecibelsToNormalized_ZeroDecibels_ReturnsOne()
        {
            float result = AudioUtility.DecibelsToNormalized(0f);
            Assert.AreEqual(1f, result, 0.001f);
        }

        [Test]
        public void DecibelsToNormalized_MinusSixDecibels_ReturnsApproximatelyHalf()
        {
            float result = AudioUtility.DecibelsToNormalized(-6.02f);
            Assert.AreEqual(0.5f, result, 0.01f);
        }

        [Test]
        public void RoundTrip_NormalizedToDecibelsAndBack_ReturnsOriginal()
        {
            float original = 0.75f;
            float decibels = AudioUtility.NormalizedToDecibels(original);
            float result = AudioUtility.DecibelsToNormalized(decibels);
            Assert.AreEqual(original, result, 0.001f);
        }

        [Test]
        public void RoundTrip_DecibelsToNormalizedAndBack_ReturnsOriginal()
        {
            float original = -12f;
            float normalized = AudioUtility.DecibelsToNormalized(original);
            float result = AudioUtility.NormalizedToDecibels(normalized);
            Assert.AreEqual(original, result, 0.001f);
        }
    }
}

