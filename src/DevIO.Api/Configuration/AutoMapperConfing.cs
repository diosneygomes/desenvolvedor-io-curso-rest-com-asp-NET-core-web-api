using AutoMapper;
using DevIO.Api.ViewModels;
using DevIO.Business.Models;

namespace DevIO.Api.Configuration
{
    public class AutoMapperConfing : Profile
    {
        public AutoMapperConfing()
        {
            CreateMap<Fornecedor, FornecedorViewModel>().ReverseMap();
            CreateMap<Endereco, EnderecoViewModel>().ReverseMap();
            CreateMap<Produto, ProdutoViewModel>().ReverseMap();
        }
    }
}
