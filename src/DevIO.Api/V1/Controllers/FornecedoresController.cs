using AutoMapper;
using DevIO.Api.Controllers;
using DevIO.Api.Extensions;
using DevIO.Api.ViewModels;
using DevIO.Business.Intefaces;
using DevIO.Business.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DevIO.Api.V1.Controllers
{
    [Authorize]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/fornecedores")]
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
            INotificador notificador,
            IUser user) : base(notificador, user)
        {
            _fornecedorRepository = fornecedorRepository;
            _fornecedorService = fornecedorService;
            _enderecoRepository = enderecoRepository;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IEnumerable<FornecedorViewModel>> ObterTodos()
        {
            var fornecedores = _mapper
                .Map<IEnumerable<FornecedorViewModel>>(await _fornecedorRepository
                    .ObterTodos()
                    .ConfigureAwait(false));

            return fornecedores;
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<FornecedorViewModel>> ObterPorId(Guid id)
        {
            var fornecedor = await ObterFornecedorProdutosEndereco(id)
                .ConfigureAwait(false);

            if (fornecedor is null)
            {
                return NotFound();
            }

            return fornecedor;
        }

        [ClaimsAuthorize("Fornecedor", "Adicionar")]
        [HttpPost]
        public async Task<ActionResult<FornecedorViewModel>> Adicionar(FornecedorViewModel fornecedorViewModel)
        {
            if (!ModelState.IsValid)
            {
                return CustomResponse(ModelState);
            }

            await _fornecedorService
                .Adicionar(_mapper.Map<Fornecedor>(fornecedorViewModel))
                .ConfigureAwait(false);

            return CustomResponse(fornecedorViewModel);
        }

        [ClaimsAuthorize("Fornecedor", "Atualizar")]
        [HttpPut("{id:guid}")]
        public async Task<ActionResult<FornecedorViewModel>> Atualizar(
            Guid id,
            FornecedorViewModel fornecedorViewModel)
        {
            if (id != fornecedorViewModel.Id)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return CustomResponse(ModelState);
            }

            await _fornecedorService
                .Atualizar(_mapper.Map<Fornecedor>(fornecedorViewModel))
                .ConfigureAwait(false);

            return CustomResponse(fornecedorViewModel);
        }

        [ClaimsAuthorize("Fornecedor", "Excluir")]
        [HttpDelete("{id:guid}")]
        public async Task<ActionResult<FornecedorViewModel>> Excluir(Guid id)
        {
            var fornecedorViewModel = await ObterFornecedorEndereco(id)
                .ConfigureAwait(false);

            if (fornecedorViewModel is null)
            {
                return NotFound();
            }

            await _fornecedorService
                .Remover(id)
                .ConfigureAwait(false);

            return CustomResponse(fornecedorViewModel);
        }

        [HttpGet("endereco/{id:guid}")]
        public async Task<EnderecoViewModel> ObterEnderecoPorId(Guid id)
        {
            return _mapper
                .Map<EnderecoViewModel>(await _enderecoRepository
                    .ObterPorId(id)
                    .ConfigureAwait(false));
        }

        [ClaimsAuthorize("Fornecedor", "Atualizar")]
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

            if (!ModelState.IsValid) return CustomResponse(ModelState);

            await _fornecedorService
                .AtualizarEndereco(_mapper
                    .Map<Endereco>(enderecoViewModel))
                    .ConfigureAwait(false);

            return CustomResponse(enderecoViewModel);
        }

        private async Task<FornecedorViewModel> ObterFornecedorProdutosEndereco(Guid id)
        {
            return _mapper
                .Map<FornecedorViewModel>(await _fornecedorRepository
                    .ObterFornecedorProdutosEndereco(id)
                    .ConfigureAwait(false));
        }

        private async Task<FornecedorViewModel> ObterFornecedorEndereco(Guid id)
        {
            return _mapper
                .Map<FornecedorViewModel>(await _fornecedorRepository
                    .ObterFornecedorEndereco(id)
                    .ConfigureAwait(false));
        }
    }
}
