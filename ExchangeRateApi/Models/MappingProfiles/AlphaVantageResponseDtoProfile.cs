using AutoMapper;
using ExchangeRateApi.Models.Dtos;

namespace ExchangeRateApi.Models.MappingProfiles
{
    public class AlphaVantageResponseDtoProfile : Profile
    {
        public AlphaVantageResponseDtoProfile()
        {
            CreateMap<AlphaVantageResponseDto, ExchangeRateDto>()
                .ForMember(dest => dest.FromCurrency, opt => opt.MapFrom(src => src.RealtimeCurrencyExchangeRate.FromCurrencyCode))
                .ForMember(dest => dest.ToCurrency, opt => opt.MapFrom(src => src.RealtimeCurrencyExchangeRate.ToCurrencyCode))
                .ForMember(dest => dest.Rate, opt => opt.MapFrom(src => src.RealtimeCurrencyExchangeRate.ExchangeRate))
                .ForMember(dest => dest.Bid, opt => opt.MapFrom(src => src.RealtimeCurrencyExchangeRate.BidPrice))
                .ForMember(dest => dest.Ask, opt => opt.MapFrom(src => src.RealtimeCurrencyExchangeRate.AskPrice));
        }
    }
}
