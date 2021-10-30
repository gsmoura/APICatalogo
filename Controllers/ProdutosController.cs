using APICatalogo.Context;
using APICatalogo.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APICatalogo.Controllers
{
    [Route("api/[Controller]")]
    [ApiController]
    public class ProdutosController : ControllerBase
    {
        private readonly AppDbContext _context;
        public ProdutosController(AppDbContext contexto)
        {
            _context = contexto;
        }

        // api/produtos
        [HttpGet]
        public ActionResult<IEnumerable<Produto>> Get()
        {
            //AsNoTracking desabilita o gerenciamento do estado das entidades
            //so deve ser usado em consultas sem alteração
            //return _context.Produtos.AsNoTracking().ToList();
            try
            {
                return _context.Produtos.ToList();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                     "Erro ao tentar obter os produtos do banco de dados");
            }
        }

        // api/produtos/1
        [HttpGet("{id}", Name = "ObterProduto")]
        public ActionResult<Produto> Get(int id)
        {
            //AsNoTracking desabilita o gerenciamento do estado das entidades
            //so deve ser usado em consultas sem alteração
            //var produto = _context.Produtos.AsNoTracking()
            //    .FirstOrDefault(p => p.ProdutoId == id);
            try
            {
                var produto = _context.Produtos.FirstOrDefault(p => p.ProdutoId == id);

                if (produto == null)
                {
                    return NotFound($"O produto com id={id} não foi encontrada");
                }
                return produto;
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                     $"Erro ao tentar obter dados do produto com id = {id}");
            }
        }

        //  api/produtos
        [HttpPost]
        public ActionResult Post([FromBody] Produto produto)
        {
            //a validação do ModelState é feito automaticamente
            //quando aplicamos o atributo [ApiController]
            //if (!ModelState.IsValid)
            //{
            //    return BadRequest(ModelState);
            //}
            try
            {
                _context.Produtos.Add(produto);
                _context.SaveChanges();

                return new CreatedAtRouteResult("ObterProduto",
                    new { id = produto.ProdutoId }, produto);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                   "Erro ao criar um novo produto");
            }

        }

        // api/produtos/1
        [HttpPut("{id}")]
        public ActionResult Put(int id, [FromBody] Produto produto)
        {
            //a validação do ModelState é feito automaticamente
            //quando aplicamos o atributo [ApiController]
            //if(!ModelState.IsValid)
            //{
            //    return BadRequest(ModelState);
            //}
            try
            {
                if (id != produto.ProdutoId)
                {
                    return BadRequest($"Não foi possível atualizar o produto com id={id}");
                }

                _context.Entry(produto).State = EntityState.Modified;
                _context.SaveChanges();
                return Ok($"Produto com id={id} atualizado com sucesso");
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                       $"Erro ao atualizar dados do produto com id =  {id}");
            }
        }

        //  api/produtos/1
        [HttpDelete("{id}")]
        public ActionResult<Produto> Delete(int id)
        {
            // Usar o método Find é uma opção para localizar 
            // o produto pelo id (não suporta AsNoTracking)
            //var produto = _context.Produtos.Find(id);

            try
            {
                var produto = _context.Produtos.FirstOrDefault(p => p.ProdutoId == id);

                if (produto == null)
                {
                    return NotFound("O produto com id={id} não foi encontrado");
                }

                _context.Produtos.Remove(produto);
                _context.SaveChanges();
                return produto;
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                                       $"Erro ao excluir o produto de id = {id}");
            }
        }
    }
}
