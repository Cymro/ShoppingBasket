using System;
using System.Collections.Generic;
using NUnit.Framework;
using NUnit.Framework.Internal;
using Moq;
using ShoppingBasketBusiness;
using Microsoft.Extensions.Logging;

namespace ShoppingBasketTest
{
    public class DiscountCalculatorTests
    {
        private MultiBuyDiscountCalculator _discountCalculator;

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
        public void OneEligibleTwoIllegibleBooks_NoDiscount()
        {
            var productList = new List<IProductItem>
            {
                new ProductItem() { ProductId = 100, Price = 8m},
                new ProductItem() { ProductId =  101, Price = 8m},
                new ProductItem() { ProductId =  1, Price = 8m} //this is the only multibuy item
            };

            var discount = _discountCalculator.GetDiscount(productList);
            Assert.AreEqual(0, discount);
        }

        [Test]
        public void TwoEligibleBooks_5PercentDiscount()
        {
            var productList = new List<IProductItem>
            {
                new ProductItem() { ProductId = 1, Price = 8m}, //this is a multibuy item
                new ProductItem() { ProductId = 2, Price = 8m} //this is a multibuy item
            };

            var discount = _discountCalculator.GetDiscount(productList);
            Assert.AreEqual(0.8m, discount);
        }

        [Test]
        public void TwoEligibleOneIllegible_5PercentDiscountOnEligible()
        {
            var productList = new List<IProductItem>
            {
                new ProductItem() { ProductId = 100, Price = 8m},
                new ProductItem() { ProductId = 1, Price = 8m}, //this is a multibuy item
                new ProductItem() { ProductId = 2, Price = 8m} //this is a multibuy item
            };

            var discount = _discountCalculator.GetDiscount(productList);
            Assert.AreEqual(0.8m, discount);
        }

        [Test]
        public void ThreeElligbleBooks_10PercentDiscount()
        {
            var productList = new List<IProductItem>
            {
                new ProductItem() { ProductId = 1, Price = 8m}, //this is a multibuy item
                new ProductItem() { ProductId = 2, Price = 8m}, //this is a multibuy item
                new ProductItem() { ProductId = 3, Price = 8m} //this is a multibuy item
            };

            var discount = _discountCalculator.GetDiscount(productList);
            Assert.AreEqual(2.4m, discount);
        }

        [Test]
        public void ThreeElligbleOneIllegibleBook_10PercentDiscountOnElligable()
        {
            var productList = new List<IProductItem>
            {
                new ProductItem() { ProductId = 1, Price = 8m}, //this is a multibuy item
                new ProductItem() { ProductId = 2, Price = 8m}, //this is a multibuy item
                new ProductItem() { ProductId = 3, Price = 8m}, //this is a multibuy item
                new ProductItem() { ProductId = 101, Price = 8m}
            };

            var discount = _discountCalculator.GetDiscount(productList);
            Assert.AreEqual(2.4m, discount);
        }

        [Test]
        public void ThreeElligbleBooksWithOneAdditional_10PercentDiscountOnElligable()
        {
            var productList = new List<IProductItem>
            {
                new ProductItem() { ProductId = 1, Price = 8m}, //this is a multibuy item
                new ProductItem() { ProductId = 2, Price = 8m}, //this is a multibuy item
                new ProductItem() { ProductId = 3, Price = 8m}, //this is a multibuy item
                new ProductItem() { ProductId = 3, Price = 8m} //this is a duplicate multibuy item
            };

            var discount = _discountCalculator.GetDiscount(productList);
            Assert.AreEqual(2.4m, discount);
        }

        [Test]
        public void MultipleCombosGetBestOffer_20PercentOnFourBooksAnd25PercentOnFive()
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

            var discount = _discountCalculator.GetDiscount(productList);
            Assert.AreEqual(16.4m, discount);
        }
    }
}
