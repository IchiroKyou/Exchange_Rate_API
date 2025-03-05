using AutoMapper;
using ExchangeRateApi.Models.Dtos;

namespace ExchangeRateApi.Models.MappingProfiles
{
    /// <summary>
    /// Profile so the Automapper knows how to map between ExchangeRate and ExchangeRateDto
    /// </summary>
    public class ExchangeRateProfile : Profile
    {
        public ExchangeRateProfile()
        {
            CreateMap<ExchangeRate, ExchangeRateDto>()
                .ForMember(dest => dest.FromCurrency, opt => opt.MapFrom(src => src.FromCurrency))
                .ForMember(dest => dest.ToCurrency, opt => opt.MapFrom(src => src.ToCurrency))
                .ForMember(dest => dest.Rate, opt => opt.MapFrom(src => src.Rate))
                .ForMember(dest => dest.Bid, opt => opt.MapFrom(src => src.Bid))
                .ForMember(dest => dest.Ask, opt => opt.MapFrom(src => src.Ask));
            CreateMap<ExchangeRateDto, ExchangeRate>()
                .ForMember(dest => dest.FromCurrency, opt => opt.MapFrom(src => src.FromCurrency))
                .ForMember(dest => dest.ToCurrency, opt => opt.MapFrom(src => src.ToCurrency))
                .ForMember(dest => dest.Rate, opt => opt.MapFrom(src => src.Rate))
                .ForMember(dest => dest.Bid, opt => opt.MapFrom(src => src.Bid))
                .ForMember(dest => dest.Ask, opt => opt.MapFrom(src => src.Ask))
                .ForMember(dest => dest.ExchangeRateId, opt => opt.Ignore())
                .ForMember(dest => dest.LastUpdate, opt => opt.Ignore());
        }
    }
}
