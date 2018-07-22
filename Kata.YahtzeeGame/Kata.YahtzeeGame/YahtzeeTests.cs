using System;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Threading;
using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace Kata.YahtzeeGame
{
    [TestFixture]
    public class YahtzeeTests
    {
        private Player player;
        private YahtzeeGame yahtzee;

        [SetUp]
        public void SetUp()
        {
            player = new Player();
            yahtzee = new YahtzeeGame();
        }

        [Test]
        public void WhenGameStarted_PlayerScoreIsZero()
        {
            var yahtzee = this.yahtzee;

            Assert.That(yahtzee.CalculateScore(Category.Chance, 0), Is.EqualTo(0));
        }

        [TestCase(Category.Chance, new[] { 1, 2, 3, 4, 5 }, 15, TestName = "Chance")]
        [TestCase(Category.Ones, new[] { 1, 1, 1, 1, 2 }, 4, TestName = "Ones")]
        [TestCase(Category.Twos, new[] { 2, 2, 2, 2, 3 }, 8, TestName = "Twos")]
        [TestCase(Category.Threes, new[] { 3, 3, 3, 3, 4 }, 12, TestName = "Threes")]
        [TestCase(Category.Fours, new[] { 4, 4, 4, 4, 5 }, 16, TestName = "Fours")]
        [TestCase(Category.Fives, new[] { 5, 5, 5, 5, 6 }, 20, TestName = "Fives")]
        [TestCase(Category.Sixes, new[] { 6, 6, 6, 6, 5 }, 24, TestName = "Sixes")]
        [TestCase(Category.Pairs, new[] { 3, 3, 3, 4, 4 }, 8, TestName = "Pairs")]
        [TestCase(Category.Pairs, new[] { 1, 2, 3, 4, 5 }, 0, TestName = "Pairs_NoPairs_ScoreIsZero")]
        [TestCase(Category.TwoPairs, new[] { 1, 1, 2, 3, 3 }, 8, TestName = "TwoPairs")]
        [TestCase(Category.TwoPairs, new[] { 1, 1, 2, 3, 4 }, 0, TestName = "TwoPairs_NoTwoPairs_ScoreIsZero")]
        [TestCase(Category.ThreeOfAKind, new[] { 3, 3, 3, 4, 4 }, 9, TestName = "ThreeOfAKind")]
        [TestCase(Category.ThreeOfAKind, new[] { 3, 3, 2, 4, 4 }, 0, TestName = "ThreeOfAKind_NoTriples_ScoreIsZero")]
        [TestCase(Category.FourOfAKind, new[] { 2, 2, 2, 2, 5 }, 8, TestName = "FourOfAKind")]
        [TestCase(Category.FourOfAKind, new[] { 2, 2, 2, 3, 5 }, 0, TestName = "FourOfAKind_NoFourOfAKind_ScoreIsZero")]
        [TestCase(Category.SmallStraight, new[] { 1, 2, 3, 4, 5 }, 15, TestName = "SmallStraight")]
        public void Yahtzee(Category category, int[] dices, int expectedScore)
        {
            var rolls = CollectRolls(player, dices).ToArray();

            Assert.That(yahtzee.CalculateScore(category, rolls), Is.EqualTo(expectedScore));
        }

        private IEnumerable<int> CollectRolls(Player player, params int[] dices)
        {
            return dices.Select(dice => player.Roll(new FakeDice(willReturnOnRoll: dice)));
        }
    }

    public enum Category
    {
        Chance,
        Ones,
        Twos,
        Threes,
        Fours,
        Fives,
        Sixes,
        Pairs,
        TwoPairs,
        ThreeOfAKind,
        FourOfAKind,
        SmallStraight
    }

    public class FakeDice
    {
        public FakeDice(int willReturnOnRoll)
        {
            Value = willReturnOnRoll;
        }

        public int Value { get; set; }
    }

    public class Player
    {
        public int Roll(FakeDice fakeDice)
        {
            return fakeDice.Value;
        }
    }

    public class YahtzeeGame
    {
        public int CalculateScore(Category category, params int[] rolls)
        {
            switch (category)
            {
                case Category.Ones:
                    return rolls.Where(r => r == (int)Category.Ones).Sum();
                case Category.Twos:
                    return rolls.Where(r => r == (int)Category.Twos).Sum();
                case Category.Threes:
                    return rolls.Where(r => r == (int)Category.Threes).Sum();
                case Category.Fours:
                    return rolls.Where(r => r == (int)Category.Fours).Sum();
                case Category.Fives:
                    return rolls.Where(r => r == (int)Category.Fives).Sum();
                case Category.Sixes:
                    return rolls.Where(r => r == (int)Category.Sixes).Sum();
                case Category.Pairs:
                    return ScoreRepetitions(rolls, timesTheyRepeat: 2, numberOfReps: 1);
                case Category.TwoPairs:
                    return ScoreRepetitions(rolls, timesTheyRepeat: 2, numberOfReps: 2);
                case Category.ThreeOfAKind:
                    return ScoreRepetitions(rolls, timesTheyRepeat: 3, numberOfReps: 1);
                case Category.FourOfAKind:
                    return ScoreRepetitions(rolls, timesTheyRepeat: 4, numberOfReps: 1);
                case Category.SmallStraight:
                    return 15;
            }

            return rolls.Sum();
        }

        private int ScoreRepetitions(int[] rolls, int timesTheyRepeat, int numberOfReps)
        {
            var repeated = FindNumbersThatRepeat(rolls, numberOfTimes: timesTheyRepeat).Take(numberOfReps).ToArray();
            return repeated.Length == numberOfReps ? repeated.Sum(f => f * timesTheyRepeat) : 0;
        }

        private static IEnumerable<int> FindNumbersThatRepeat(int[] rolls, int numberOfTimes)
        {
            return rolls.GroupBy(r => r).Where(r => r.Count() == numberOfTimes).Select(r => r.Key).Distinct().OrderByDescending(p => p);
        }
    }
}
