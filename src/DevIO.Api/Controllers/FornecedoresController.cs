using AutoMapper;
using DevIO.Api.ViewModels;
using DevIO.Business.Intefaces;
using Microsoft.AspNetCore.Mvc;

namespace DevIO.Api.Controllers
{
    [Route("api/fornecedores")]
    public class FornecedoresController : MainController
    {
        private readonly IFornecedorRepository _fornecedorRepository;
        private readonly IMapper _mapper;

        public FornecedoresController(
            IFornecedorRepository fornecedorRepository,
            IMapper mapper)
        {
            this._fornecedorRepository = fornecedorRepository;
            this._mapper = mapper;
        }

        public async Task<IEnumerable<FornecedorViewModel>> ObterTodos()
        {
            var fornecedores = this._mapper
                .Map<IEnumerable<FornecedorViewModel>>(await this._fornecedorRepository
                    .ObterTodos());

            return fornecedores;
        }
    }
}
