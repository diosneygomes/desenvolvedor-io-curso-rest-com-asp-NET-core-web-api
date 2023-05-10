using AutoMapper;
using DevIO.Api.ViewModels;
using DevIO.Business.Intefaces;
using DevIO.Business.Models;
using DevIO.Data.Repository;
using Microsoft.AspNetCore.Mvc;

namespace DevIO.Api.Controllers
{
    [Route("api/fornecedores")]
    public class FornecedoresController : MainController
    {
        private readonly IFornecedorRepository _fornecedorRepository;
        private readonly IFornecedorService _fornecedorService;
        private readonly IEnderecoRepository _enderecoRepository;
        private readonly IMapper _mapper;

        public FornecedoresController(
            IFornecedorRepository fornecedorRepository,
            IFornecedorService fornecedorService,
            IEnderecoRepository enderecoRepository,
            IMapper mapper,
            INotificador notificador) : base(notificador)
        {
            this._fornecedorRepository = fornecedorRepository;
            this._fornecedorService = fornecedorService;
            this._enderecoRepository = enderecoRepository;
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
                return this.CustomResponse(this.ModelState);
            }

            await this._fornecedorService
                .Adicionar(this._mapper.Map<Fornecedor>(fornecedorViewModel));

            return this.CustomResponse(fornecedorViewModel);
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
                return this.CustomResponse(this.ModelState);
            }

            await this._fornecedorService
                .Atualizar(this._mapper.Map<Fornecedor>(fornecedorViewModel));

            return this.CustomResponse(fornecedorViewModel);
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

            return this.CustomResponse(fornecedorViewModel);
        }

        [HttpGet("endereco/{id:guid}")]
        public async Task<EnderecoViewModel> ObterEnderecoPorId(Guid id)
        {
            return this._mapper.Map<EnderecoViewModel>(await this._enderecoRepository.ObterPorId(id));
        }

        [HttpPut("endereco/{id:guid}")]
        public async Task<IActionResult> AtualizarEndereco(
            Guid id,
            EnderecoViewModel enderecoViewModel)
        {
            if (id != enderecoViewModel.Id)
            {
                NotificarErro("O id informado não é o mesmo que foi passado na query");
                return CustomResponse(enderecoViewModel);
            }

            if (!this.ModelState.IsValid) return CustomResponse(this.ModelState);

            await this._fornecedorService.AtualizarEndereco(this._mapper.Map<Endereco>(enderecoViewModel));

            return CustomResponse(enderecoViewModel);
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
