namespace debug.currency.converter.Data;

public class CurrencyDto
{
    public string? BCurrencyID { get; set; }
    public string? TCurrencyID { get; set; }
    public bool IsVisible {get; set;}
    public bool IsManual {get; set;}
    public decimal? Rate {get; set;}
    public decimal? Fee {get; set;}
    public DateTime Updated {get; set;}
}