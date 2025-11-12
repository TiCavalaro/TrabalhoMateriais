using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjetoXico.Data;

public class TbLogProjetoController : Controller
{
    private readonly BancoContext _context;

    public TbLogProjetoController(BancoContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index(string? usuario, string? operacao, DateTime? data)
    {
        var logs = _context.LogsProjeto.AsQueryable();

        if (!string.IsNullOrEmpty(usuario))
            logs = logs.Where(l => l.IdUsuario.ToString() == usuario);

        if (!string.IsNullOrEmpty(operacao))
            logs = logs.Where(l => l.Operacao.Contains(operacao));

        if (data.HasValue)
            logs = logs.Where(l => l.DataOperacao.Date == data.Value.Date);

        var lista = await logs
            .OrderByDescending(l => l.DataOperacao)
            .Take(200)
            .ToListAsync();

        return View("~/Views/Home/LogsProjeto.cshtml", lista);
    }
}
