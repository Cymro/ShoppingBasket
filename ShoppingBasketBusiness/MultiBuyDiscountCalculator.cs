using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime.ExceptionServices;
using System.Text.RegularExpressions;

namespace ShoppingBasketBusiness
{
    public class MultiBuyDiscountCalculator : IMultiBuyDiscountCalculator
    {
        private ILogger _logger;
        private IMultiBuyOfferRepository _offerRepository;
        private int _minOfferCount;
        private int _maxOfferCount;

        public MultiBuyDiscountCalculator(ILogger logger, IMultiBuyOfferRepository offerRepository)
        {
            _logger = logger;
            _offerRepository = offerRepository;
            _minOfferCount = _offerRepository.DiscountLookup.Keys.Min();
            _maxOfferCount = _offerRepository.DiscountLookup.Keys.Max();
        }

        public decimal GetDiscount(IList<IProductItem> items)
        {
            var eligableItems = new List<IProductItem>();
            foreach (var item in items)
            {
                if (_offerRepository.EligableProducts.Contains(item.ProductId))
                    eligableItems.Add(item);
            }

            var combinationSets = GetCombinationSets(eligableItems, eligableItems.Select(i => i.ProductId).Distinct().Count());

            decimal maxDiscount = 0;
            foreach (var combinationSet in combinationSets)
            {
                var combinationDiscount = CalculateCombinationSetDiscount(combinationSet);
                if (combinationDiscount > maxDiscount)
                    maxDiscount = combinationDiscount;
            }
            return maxDiscount;
        }

        internal decimal GetDiscountMultiplier(IList<IProductItem> items)
        {
            if (items == null || !items.Any()) return 0;

            //the number of unique qualifying items
            var qulifyingItemsCount = items.Count();

            //short circuit - return early with no discount
            if (qulifyingItemsCount < _minOfferCount) return 0m;

            decimal discountMultiplier = 0;
            //check for max multibuy
            if (qulifyingItemsCount > _maxOfferCount)
            {
                qulifyingItemsCount = _maxOfferCount;
                return _offerRepository.DiscountLookup[_maxOfferCount];
            }
            //get the discount from the discount lookup
            else if (_offerRepository.DiscountLookup.TryGetValue(qulifyingItemsCount, out discountMultiplier))
            {
                return discountMultiplier;
            }

            //else get the discount from the highest qualifying band
            var lookupQulifyingItemsCount = _offerRepository.DiscountLookup.Keys
                .Where(itemCount => itemCount <= qulifyingItemsCount)
                .OrderByDescending(itemCount => itemCount)
                .First();

            return _offerRepository.DiscountLookup[lookupQulifyingItemsCount];
        }

        /// <summary>
        /// Get the total discount for a complete combination set
        /// </summary>
        /// <param name="combinationSet"></param>
        /// <returns></returns>
        private decimal CalculateCombinationSetDiscount(List<List<IProductItem>> combinationSet)
        {
            decimal discount = 0;
            foreach (var combination in combinationSet)
            {
                var discountMultiplier = GetDiscountMultiplier(combination);
                foreach (var item in combination)
                {
                    discount += item.Price * discountMultiplier;
                }
            }
            return discount;
        }

        /// <summary>
        /// Sorts the product items into sets of every possible combination
        /// </summary>
        /// <param name="items"></param>
        /// <param name="maxDiscountCombo">Limits the combination size and should be the number of distinct items or total available products for the offer</param>
        /// <returns></returns>
        public List<List<List<IProductItem>>> GetCombinationSets(List<IProductItem> items, int maxDiscountCombo)
        {
            var combinationSetList = new List<List<List<IProductItem>>>();

            for (int combinationSetSize = 1; combinationSetSize <= maxDiscountCombo; combinationSetSize++)
            {
                //start a new combination set based for the number of allowable items
                var combinationSet = new List<List<IProductItem>>();
                foreach (var item in items)
                {
                    var combinations = combinationSet.Where(c => !c.Any(i => item.ProductId == i.ProductId));
                    //get the first combination in the set with room for more items
                    var combination = combinations.FirstOrDefault(c => c.Count() < combinationSetSize);
                    if (combination != null)
                        //add to the existing combination
                        combination.Add(item);
                    else
                        //there are none available, so start a new unique list within the current set
                        combinationSet.Add(new List<IProductItem>() { item });
                }
                combinationSetList.Add(combinationSet);
            }
            return combinationSetList;
        }

    }
}
