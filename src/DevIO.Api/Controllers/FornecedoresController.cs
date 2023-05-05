using AutoMapper;
using DevIO.Api.ViewModels;
using DevIO.Business.Intefaces;
using DevIO.Business.Models;
using DevIO.Business.Services;
using Microsoft.AspNetCore.Mvc;

namespace DevIO.Api.Controllers
{
    [Route("api/fornecedores")]
    public class FornecedoresController : MainController
    {
        private readonly IFornecedorRepository _fornecedorRepository;
        private readonly IFornecedorService _fornecedorService;
        private readonly IMapper _mapper;

        public FornecedoresController(
            IFornecedorRepository fornecedorRepository,
            IFornecedorService fornecedorService,
            IMapper mapper)
        {
            this._fornecedorRepository = fornecedorRepository;
            this._fornecedorService = fornecedorService;
            this._mapper = mapper;
        }

        [HttpGet]
        public async Task<IEnumerable<FornecedorViewModel>> ObterTodos()
        {
            var fornecedores = this._mapper
                .Map<IEnumerable<FornecedorViewModel>>(await this._fornecedorRepository
                    .ObterTodos());

            return fornecedores;
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<FornecedorViewModel>> ObterPorId(Guid id)
        {
            var fornecedor = await this.ObterFornecedorProdutosEndereco(id);

            if (fornecedor is null)
            {
                return this.NotFound();
            }

            return fornecedor;
        }

        [HttpPost]
        public async Task<ActionResult<FornecedorViewModel>> Adicionar(FornecedorViewModel fornecedorViewModel)
        {
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest();
            }

            var fornecedorDao = this._mapper.Map<Fornecedor>(fornecedorViewModel);

            await this._fornecedorService
                .Adicionar(fornecedorDao);

            return this.Ok(fornecedorDao);
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult<FornecedorViewModel>> Atualizar(
            Guid id,
            FornecedorViewModel fornecedorViewModel)
        {
            if (id != fornecedorViewModel.Id)
            {
                return this.BadRequest();
            }

            if (!this.ModelState.IsValid)
            {
                return this.BadRequest();
            }

            var fornecedorDao = this._mapper.Map<Fornecedor>(fornecedorViewModel);

            await this._fornecedorService
                .Atualizar(fornecedorDao);

            return this.Ok(fornecedorDao);
        }

        [HttpDelete("{id:guid}")]
        public async Task<ActionResult<FornecedorViewModel>> Excluir(Guid id)
        {
            var fornecedorViewModel = await this.ObterFornecedorEndereco(id);

            if (fornecedorViewModel is null)
            {
                return this.NotFound();
            }

            await this._fornecedorService
                .Remover(id);

            return this.Ok(fornecedorViewModel);
        }

        public async Task<FornecedorViewModel> ObterFornecedorProdutosEndereco(Guid id)
        {
            return this._mapper
                .Map<FornecedorViewModel>(await this._fornecedorRepository
                    .ObterFornecedorProdutosEndereco(id));
        }

        public async Task<FornecedorViewModel> ObterFornecedorEndereco(Guid id)
        {
            return this._mapper
                .Map<FornecedorViewModel>(await this._fornecedorRepository
                    .ObterFornecedorEndereco(id));
        }
    }
}
