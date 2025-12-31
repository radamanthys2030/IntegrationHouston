using AutoMapper;
using Integration.Houston.Application.Contract.Models.Dto;
using Integration.Houston.Application.Infrastructure.Models.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Integration.Houston.Application.Infrastructure.Mapper
{
    public class MapperProfile : Profile
    {
        public MapperProfile() {

            CreateMap<Transactions, CreditcardTransaction>();
        }
    }
}
