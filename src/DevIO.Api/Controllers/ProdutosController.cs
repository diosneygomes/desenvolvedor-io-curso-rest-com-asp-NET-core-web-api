using AutoMapper;
using DevIO.Api.ViewModels;
using DevIO.Business.Intefaces;
using DevIO.Business.Models;
using Microsoft.AspNetCore.Mvc;

namespace DevIO.Api.Controllers
{
    [Route("api/produtos")]
    public class ProdutosController : MainController
    {
        private readonly IProdutoRepository _produtoRepository;
        private readonly IProdutoService _produtoService;
        private readonly IMapper _mapper;

        public ProdutosController(
            INotificador _notificador,
            IProdutoRepository _produtoRepository,
            IProdutoService produtoService,
            IMapper _mapper) : base(_notificador)
        {
            this._produtoService = produtoService;
            this._produtoRepository = _produtoRepository;
            this._mapper = _mapper;
        }

        [HttpGet]
        public async Task<IEnumerable<ProdutoViewModel>> ObterTodos()
        {
            return this._mapper
                .Map<IEnumerable<ProdutoViewModel>>(await this._produtoRepository
                    .ObterProdutosFornecedores()
                    .ConfigureAwait(false));
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<ProdutoViewModel>> ObterPorId(Guid id)
        {
            var produtoViewModel = await this.ObterProduto(id)
                .ConfigureAwait(false);

            if (produtoViewModel is null)
            {
                return this.NotFound();
            }

            return produtoViewModel;
        }

        [HttpPost]
        public async Task<ActionResult<ProdutoViewModel>> Adicionar(ProdutoViewModel produtoViewModel)
        {
            if (!ModelState.IsValid)
            {
                return this.CustomResponse(ModelState);
            }

            var imagemNome = string.Format("{0}_{1}",
                Guid.NewGuid().ToString(),
                produtoViewModel.Imagem);

            if (!this.UploadArquivo(produtoViewModel.ImagemUpload, imagemNome))
            {
                return this.CustomResponse(produtoViewModel);
            }

            produtoViewModel.Imagem = imagemNome;

            await this._produtoService
                .Adicionar(this._mapper.Map<Produto>(produtoViewModel))
                .ConfigureAwait(false);

            return this.CustomResponse(produtoViewModel);

        }

        [HttpDelete("{id:guid}")]
        public async Task<ActionResult<ProdutoViewModel>> Excluir(Guid id)
        {
            var produtoViewModel = await this.ObterProduto(id)
                .ConfigureAwait(false);

            if (produtoViewModel is null)
            {
                return this.NotFound();
            }

            await this._produtoRepository.Remover(id)
                .ConfigureAwait(false);

            return CustomResponse(produtoViewModel);
        }

        public async Task<ProdutoViewModel> ObterProduto(Guid id)
        {
            return this._mapper
                .Map<ProdutoViewModel>(await this._produtoRepository
                    .ObterProdutoFornecedor(id)
                    .ConfigureAwait(false));
        }

        private bool UploadArquivo(string arquivo, string imgNome)
        {
            if (string.IsNullOrEmpty(arquivo))
            {
                NotificarErro("Forneça uma imagem para este produto!");
                return false;
            }

            var imageDataByteArray = Convert.FromBase64String(arquivo);

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/imagens", imgNome);

            if (System.IO.File.Exists(filePath))
            {
                NotificarErro("Já existe um arquivo com este nome!");
                return false;
            }

            System.IO.File.WriteAllBytes(filePath, imageDataByteArray);

            return true;
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
