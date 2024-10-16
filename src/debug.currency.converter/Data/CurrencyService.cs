using Microsoft.EntityFrameworkCore;

namespace debug.currency.converter.Data;

public class CurrencyService
{
    private readonly CurrencyDbContext _context;
    private readonly List<CurrencyViewModel> _currencies;

    public CurrencyService(CurrencyDbContext context)
    {
        _context = context;
        _currencies = GetAllCurrencies();
    }

    public List<CurrencyViewModel> GetAllCurrencies()
    {
        var currenciesDto = _context.BCurrencyRates.ToList();
        
        var currenciesViewModels = new List<CurrencyViewModel>();
        
        foreach (var currency in currenciesDto)
        {
            if (currency.BCurrencyID == currency.TCurrencyID)
            {
                continue;
            }
            
            var model = new CurrencyViewModel
            {
                BCurrency = currency.BCurrencyID,
                TCurrency = currency.TCurrencyID,
                Rate = currency.Rate,
                Fee = currency.Fee,
            };
            
            currenciesViewModels.Add(model);
        }
        
        return currenciesViewModels!;
    }

    public CurrencyViewModel? GetCurrencyRate(string baseCurrency, string bCurrency, string tCurrency)
    {
        var baseCurrencyRates = _currencies
            .Where(c => c.BCurrency == baseCurrency)
            .ToList();
        
        decimal? rate = 0;
        var currency = baseCurrencyRates
            .FirstOrDefault(x => x.BCurrency == bCurrency && x.TCurrency == tCurrency);
        
        rate = currency?.Rate;

        if (currency == null)
        {
            currency = baseCurrencyRates
                .FirstOrDefault(x => x.BCurrency == tCurrency && x.TCurrency == bCurrency);

            if (currency != null)
            {
                rate = 1 / currency?.Rate;
            }
        }

        if (currency == null)
        {
            return GetCrossCurrencies(baseCurrency, bCurrency, tCurrency);
        }
        
        if (currency == null)
        {
            return null;
        }

        var model = new CurrencyViewModel
        {
            BCurrency = currency.BCurrency,
            TCurrency = currency.TCurrency,
            Rate = rate,
            Fee = currency.Fee
        };

        return model;
    }

    private CurrencyViewModel GetCrossCurrencies(string baseCurrency, string fCurrency,
        string sCurrency)
    {
        var baseCurrencyRates = _currencies
            .Where(c => c.BCurrency == baseCurrency)
            .ToList();

        decimal rate = 0;
        
        var firstCurrency = baseCurrencyRates
            .FirstOrDefault(x => x.BCurrency == baseCurrency && x.TCurrency == fCurrency);
        
        var secondCurrency = baseCurrencyRates
            .FirstOrDefault(x => x.BCurrency == baseCurrency && x.TCurrency == sCurrency);

        if (firstCurrency == null || secondCurrency == null)
        {
            return null!;
        }
        
        rate = (decimal)(secondCurrency.Rate / firstCurrency.Rate)!;
        
        var firstViewModel = new CurrencyViewModel
        {
            BCurrency = firstCurrency.TCurrency,
            TCurrency = secondCurrency.TCurrency,
            Rate = rate,
            Fee = firstCurrency.Fee + secondCurrency.Fee
        };
            
        return firstViewModel;
    }

    public List<string> GetAllCurrencyNames()
    {
        var bCurrencies = _currencies
            .Select(x => x.BCurrency)
            .ToList();

        var tCurrencies = _currencies
            .Select(x => x.TCurrency)
            .ToList();
        
        var allCurrencies = bCurrencies
            .Concat(tCurrencies)
            .Distinct()
            .ToList();

        return allCurrencies!;
    }
}