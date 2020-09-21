using System;
using System.Collections.Generic;
using NUnit.Framework;
using NUnit.Framework.Internal;
using Moq;
using ShoppingBasketBusiness;
using Microsoft.Extensions.Logging;

namespace ShoppingBasketTest
{
    public class DiscountCalculatorCombinationTests
    {
        private MultiBuyDiscountCalculator _discountCalculator;

        private List<string> _infoLogs = new List<string>();

        [SetUp]
        public void Setup()
        {
            Mock<Microsoft.Extensions.Logging.ILogger> mockLogger = 
                new Mock<Microsoft.Extensions.Logging.ILogger>();

            var mockMultiBuyOfferRepository = new Mock<IMultiBuyOfferRepository>();
            mockMultiBuyOfferRepository
                .SetupGet<IList<int>>(m => m.EligableProducts)
                .Returns(new List<int> {1, 2, 3, 4, 5, 6});

            mockMultiBuyOfferRepository
                .SetupGet<Dictionary<int, decimal>>(m => m.DiscountLookup)
                .Returns(new Dictionary<int, decimal>
                {
                    { 2, 0.05m },
                    { 3, 0.1m },
                    { 4, 0.2m },
                    { 5, 0.25m }
                });

            _discountCalculator = new MultiBuyDiscountCalculator(mockLogger.Object, mockMultiBuyOfferRepository.Object);
        }

        [Test]
        public void NoEligibleBooks_0Combo()
        {
            var productList = new List<IProductItem>
            {
                new ProductItem() { ProductId = 100, Price = 8m},
                new ProductItem() { ProductId =  101, Price = 8m},
                new ProductItem() { ProductId =  102, Price = 8m} 
            };

            var combinationSets = _discountCalculator.GetCombinationSets(productList, 1);
            Assert.AreEqual(1, combinationSets.Count);
        }

        [Test]
        public void OneEligibleTwoIllegibleBooks_1Combo()
        {
            var productList = new List<IProductItem>
            {
                new ProductItem() { ProductId = 100, Price = 8m},
                new ProductItem() { ProductId =  101, Price = 8m},
                new ProductItem() { ProductId =  1, Price = 8m} //this is the only multibuy item
            };

            var combinationSets = _discountCalculator.GetCombinationSets(productList, 1);
            Assert.AreEqual(1, combinationSets.Count);
        }

        [Test]
        public void TwoEligibleBooks_2Combos()
        {
            var productList = new List<IProductItem>
            {
                new ProductItem() { ProductId = 1, Price = 8m}, //this is a multibuy item
                new ProductItem() { ProductId = 2, Price = 8m}, //this is a multibuy item
                new ProductItem() { ProductId = 2, Price = 8m} //this is a multibuy item
            };

            var combinationSets = _discountCalculator.GetCombinationSets(productList, 2);
            Assert.AreEqual(2, combinationSets.Count);
        }

        [Test]
        public void TwoEligibleOneIllegible_2Combos()
        {
            var productList = new List<IProductItem>
            {
                new ProductItem() { ProductId = 100, Price = 8m},
                new ProductItem() { ProductId = 1, Price = 8m}, //this is a multibuy item
                new ProductItem() { ProductId = 2, Price = 8m} //this is a multibuy item
            };

            var combinationSets = _discountCalculator.GetCombinationSets(productList, 2);
            Assert.AreEqual(2, combinationSets.Count);
        }

        [Test]
        public void ThreeElligbleBooks_3Combos()
        {
            var productList = new List<IProductItem>
            {
                new ProductItem() { ProductId = 1, Price = 8m}, //this is a multibuy item
                new ProductItem() { ProductId = 2, Price = 8m}, //this is a multibuy item
                new ProductItem() { ProductId = 3, Price = 8m} //this is a multibuy item
            };

            var combinationSets = _discountCalculator.GetCombinationSets(productList, 3);
            Assert.AreEqual(3, combinationSets.Count);
        }

        [Test]
        public void ThreeElligbleOneIllegibleBook_3Combos()
        {
            var productList = new List<IProductItem>
            {
                new ProductItem() { ProductId = 1, Price = 8m}, //this is a multibuy item
                new ProductItem() { ProductId = 2, Price = 8m}, //this is a multibuy item
                new ProductItem() { ProductId = 3, Price = 8m}, //this is a multibuy item
                new ProductItem() { ProductId = 101, Price = 8m}
            };

            var combinationSets = _discountCalculator.GetCombinationSets(productList, 3);
            Assert.AreEqual(3, combinationSets.Count);
        }

        [Test]
        public void ThreeElligbleBooksWithOneAdditional_3Combos()
        {
            var productList = new List<IProductItem>
            {
                new ProductItem() { ProductId = 3, Price = 8m}, //this is a multibuy item
                new ProductItem() { ProductId = 3, Price = 8m}, //this is a duplicate multibuy item
                new ProductItem() { ProductId = 1, Price = 8m}, //this is a multibuy item
                new ProductItem() { ProductId = 2, Price = 8m} //this is a multibuy item
            };
            var combinationSets = _discountCalculator.GetCombinationSets(productList, 3);
            Assert.AreEqual(3, combinationSets.Count);
        }

        [Test]
        public void MultipleCombos_5Combos()
        {
            var productList = new List<IProductItem>
            {
                new ProductItem() { ProductId = 1, Price = 8m}, //this is a multibuy item
                new ProductItem() { ProductId = 1, Price = 8m}, //this is a duplicate multibuy item
                new ProductItem() { ProductId = 2, Price = 8m}, //this is a multibuy item
                new ProductItem() { ProductId = 2, Price = 8m}, //this is a duplicate multibuy item
                new ProductItem() { ProductId = 3, Price = 8m}, //this is a multibuy item
                new ProductItem() { ProductId = 3, Price = 8m}, //this is a duplicate multibuy item
                new ProductItem() { ProductId = 4, Price = 8m}, //this is a multibuy item
                new ProductItem() { ProductId = 5, Price = 8m}, //this is a multibuy item
                new ProductItem() { ProductId = 6, Price = 8m}, //this is a multibuy item
            };

            var combinationSets = _discountCalculator.GetCombinationSets(productList, 5);
            Assert.AreEqual(5, combinationSets.Count);
        }
    }
}
