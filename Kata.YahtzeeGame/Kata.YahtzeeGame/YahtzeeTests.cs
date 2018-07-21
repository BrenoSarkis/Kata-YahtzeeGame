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
        ThreeOfAKind
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
                    {
                        var number = FindNumbersThatRepeat(rolls, numberOfTimes: 2).Take(1);
                        return number.Count() == 1 ? number.Sum(p => p * 2) : 0;
                    }
                case Category.TwoPairs:
                    {
                        var twoNumbers = FindNumbersThatRepeat(rolls, numberOfTimes: 2).Take(2);
                        return twoNumbers.Count() == 2 ? twoNumbers.Sum(p => p * 2) : 0;
                    }
                case Category.ThreeOfAKind:
                    var threeOfAKind = FindNumbersThatRepeat(rolls, numberOfTimes: 3).Take(1);
                    return threeOfAKind.Count() == 1 ? threeOfAKind.First() * 3 : 0;

            }

            return rolls.Sum();
        }

        private static IEnumerable<int> FindNumbersThatRepeat(int[] rolls, int numberOfTimes)
        {
            return rolls.GroupBy(r => r).Where(r => r.Count() == numberOfTimes).Select(r => r.Key).Distinct().OrderByDescending(p => p);
        }
    }
}
