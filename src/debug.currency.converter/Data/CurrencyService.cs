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

    public CurrencyViewModel? GetCurrencyRate(string? bCurrency, string? tCurrency)
    {
        decimal? rate = 0;
        var currency = _currencies
            .FirstOrDefault(x => x.BCurrency == bCurrency && x.TCurrency == tCurrency);
        
        rate = currency?.Rate;

        if (currency == null)
        {
            currency = _currencies
                .FirstOrDefault(x => x.BCurrency == tCurrency && x.TCurrency == bCurrency);

            if (currency != null)
            {
                rate = 1 / currency?.Rate;
            }
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

    public CurrencyViewModel GetCrossCurrencies(string cCurrency, string fCurrency,
        string sCurrency)
    {
        var firstCurrency = _currencies
            .FirstOrDefault(x => x.BCurrency == cCurrency && x.TCurrency == fCurrency);
        
        var secondCurrency = _currencies
            .FirstOrDefault(x => x.BCurrency == cCurrency && x.TCurrency == sCurrency);

        if (firstCurrency != null && secondCurrency != null)
        {
            var firstViewModel = new CurrencyViewModel
            {
                BCurrency = firstCurrency.TCurrency,
                TCurrency = secondCurrency.TCurrency,
                Rate = secondCurrency.Rate / firstCurrency.Rate,
                Fee = firstCurrency.Fee + secondCurrency.Fee
            };
            
            return firstViewModel;
        }
        
        return null!;
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