using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Vets.Data;
using Vets.Models;

namespace Vets.Controllers
{
    public class VeterinariosController : Controller
    {
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// esta variavel vai conter os dados do servidor
        /// </summary>
        private readonly IWebHostEnvironment _webHostEnvironment;

        public VeterinariosController(ApplicationDbContext context, IWebHostEnvironment _webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = _webHostEnvironment;
        }




        // GET: Veterinarios
        public async Task<IActionResult> Index()
        {
            return View(await _context.Veterinarios.ToListAsync());
        }
        /*acesso á base de dados 
         * 
         * 
         * 
         * 
         * edepois vamos enviar os dados para a view
         */




        // GET: Veterinarios/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var veterinarios = await _context.Veterinarios
                .FirstOrDefaultAsync(m => m.Id == id);
            if (veterinarios == null)
            {
                return NotFound();
            }

            return View(veterinarios);
        }

        // GET: Veterinarios/Create
        /// <summary>
        /// usado ara o primeiro acesso à View 'Create000000000000000000000000000000000000000000000000000000000000000', em modo HTTP GET
        /// </summary>
        /// <returns></returns>
        public IActionResult Create()
        {
            return View();
        }




        // POST: Veterinarios/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        /// <summary>
        /// Este métudo é usado para recuperar os dados enviados pelos utilizadores
        /// </summary>
        /// <param name="veterinario"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("Id,Nome,NumCedulaProf,Fotografia")] Veterinarios veterinario,
            IFormFile fotoVet)
        {
            /*
             * Algoritmo para processar ao ficheiro com a imagem
             * 
             * se ficheiro imagem nulo
             *    aribuir uma imagem genérica ao veterinário  
             * else
             *    será que o ficheiro é uma imagem?
             *    se não for
             *       criar mensagem erro
             *       devolver o controlo da app á view
             *    else
             *       - definir o nome a atribuir á imagem
             *       - atribuir aos dados do novo vet, o nome do ficheiro da imagem
             *       - guardar a imagem no disco rigido do servisor
             */

            if (fotoVet == null)
            {
                veterinario.Fotografia = "noVet.png";
            }
            else {
                if (!(fotoVet.ContentType =="image/png" || fotoVet.ContentType == "image/jpeg"))
                {
                    //criar mensagem de erro
                    ModelState.AddModelError("", "Por favor, adicione um ficheiro .png ou .jpg");
                    //devolver o controlo da app á view
                    //fornecendo-lhes os dados que o utilizador já tinha preenchido no formulário
                    return View(veterinario);
                }
                else
                {
                    //temos ficheiro e é uma imagem
                    //+++++++++++++++++++++++++++++
                    //definir o nome da foto
                    Guid g = Guid.NewGuid();
                    string nomeFoto = veterinario.NumCedulaProf +"_"+ g.ToString();
                    string extensaoFoto = Path.GetExtension(fotoVet.FileName).ToLower();
                    nomeFoto += extensaoFoto;
                    //atribuir ao vet o nome da sua foto
                    veterinario.Fotografia = nomeFoto;
                }
            }        

            if (ModelState.IsValid)
            {
                try { 
                    //adicionar os dados á BD
                    _context.Add(veterinario);
                    //consolidar esses dados (commit)
                    await _context.SaveChangesAsync();
                }
                catch (Exception)
                {
                    //é da nossa responsabilidade tratarmos da exceção

                    //registar no disco rígido do servidor todos os dados da operação
                    //   - data + hora
                    //   - nome do utilizador
                    //   - nome do controller + método
                    //   - dados do erro (ex)
                    //   - outros dados considerados úteis

                    //eventualmente, tentar guardar na (numa) base de dados os dados do erro

                    //eventualmente notificar o Administrador da app do erro

                    //no nosso caso,
                    //criar uma msg de erro
                    ModelState.AddModelError("", "Ocorreu um erro com a operação de guardar os dados do veterinario" + veterinario.Nome);
                    //devolver controlo á view
                    return View(veterinario);
                }
                //++++++++++++++++++++++++++++++++
                //concretizar a ação de guardar o ficheiro da foto
                //++++++++++++++++++++++++++++++++
                if(fotoVet != null) { 
                    //onde vai ser guardado?
                    string nomeLocalizacaoFicheiro = _webHostEnvironment.WebRootPath;
                    nomeLocalizacaoFicheiro = Path.Combine(nomeLocalizacaoFicheiro, "Fotos");
                    //avaliar se a pasta 'Fotos' existe
                    if (!Directory.Exists(nomeLocalizacaoFicheiro))
                    {
                        Directory.CreateDirectory(nomeLocalizacaoFicheiro);
                    }
                    //nome do docomento a guardar
                    string nomeDaFoto = Path.Combine(nomeLocalizacaoFicheiro,veterinario.Fotografia);
                    //criar o objeto que vai manipular o ficheiro
                    using var stream = new FileStream(nomeDaFoto, FileMode.Create);
                    //guardar no disco rigido
                    await fotoVet.CopyToAsync(stream);
                }
                //devolver o controlo da app á view  
                return RedirectToAction(nameof(Index));
                
            }
            return View(veterinario);
        }

        // GET: Veterinarios/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var veterinarios = await _context.Veterinarios.FindAsync(id);
            if (veterinarios == null)
            {
                return NotFound();
            }
            return View(veterinarios);
        }

        // POST: Veterinarios/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nome,NumCedulaProf,Fotografia")] Veterinarios veterinarios)
        {
            if (id != veterinarios.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(veterinarios);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VeterinariosExists(veterinarios.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(veterinarios);
        }

        // GET: Veterinarios/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var veterinarios = await _context.Veterinarios
                .FirstOrDefaultAsync(m => m.Id == id);
            if (veterinarios == null)
            {
                return NotFound();
            }

            return View(veterinarios);
        }

        // POST: Veterinarios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try { 
                var veterinarios = await _context.Veterinarios.FindAsync(id);
                _context.Veterinarios.Remove(veterinarios);
                await _context.SaveChangesAsync();
                // remover o ficheiro com a foto do veterinário
                //se a foto NÂO for a 'noVet.png
            }
            catch
            {
                //throw;
                //não esquecer, tratar da exceção
            }
            return RedirectToAction(nameof(Index));
        }

        private bool VeterinariosExists(int id)
        {
            return _context.Veterinarios.Any(e => e.Id == id);
        }
    }
}
