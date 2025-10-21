using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SafeScribeAPI.Data;
using SafeScribeAPI.DTOs;
using SafeScribeAPI.Models;
using System.Security.Claims;

namespace SafeScribeAPI.Controllers
{
    /// <summary>
    /// Controlador responsável pelo CRUD de notas dentro da plataforma SafeScribe.
    /// 
    /// Contém operações de criação, consulta, atualização e exclusão de notas.
    /// A autorização e o acesso aos recursos dependem do papel (Role) do usuário autenticado.
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    [Authorize] // ✅ todas as rotas exigem autenticação
    public class NotesController : ControllerBase
    {
        private readonly AppDbContext _db;

        public NotesController(AppDbContext db)
        {
            _db = db;
        }

        /// <summary>
        /// 📝 Cria uma nova nota no sistema.
        /// </summary>
        /// <remarks>
        /// **Regras de acesso:**  
        /// - Apenas usuários com role **Editor** ou **Admin** podem criar notas.
        /// 
        /// A nota será associada automaticamente ao usuário autenticado (usando o ID presente no token JWT).
        /// 
        /// **Exemplo de requisição:**
        /// ```json
        /// {
        ///   "title": "Relatório Semanal",
        ///   "content": "Resumo das atividades da semana..."
        /// }
        /// ```
        /// </remarks>
        /// <param name="dto">Título e conteúdo da nota a ser criada.</param>
        /// <response code="200">Nota criada com sucesso.</response>
        /// <response code="403">Usuário sem permissão para criar notas.</response>
        [HttpPost]
        [Authorize(Roles = "Editor,Admin")]
        public async Task<IActionResult> Criar([FromBody] NoteCreateDto dto)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty);
            var note = new Note
            {
                Title = dto.Title,
                Content = dto.Content,
                UserId = userId
            };

            _db.Notes.Add(note);
            await _db.SaveChangesAsync();

            return Ok(note);
        }

        /// <summary>
        /// 📄 Obtém uma nota específica pelo ID.
        /// </summary>
        /// <remarks>
        /// **Regras de acesso:**  
        /// - **Admin:** pode acessar qualquer nota.  
        /// - **Editor/Leitor:** podem acessar **apenas suas próprias notas**.
        /// 
        /// **Exemplo de requisição:**
        /// ```
        /// GET /api/v1/notes/3fa85f64-5717-4562-b3fc-2c963f66afa6
        /// ```
        /// </remarks>
        /// <param name="id">ID da nota a ser consultada.</param>
        /// <response code="200">Nota encontrada e retornada.</response>
        /// <response code="403">Usuário sem permissão para acessar a nota.</response>
        /// <response code="404">Nota não encontrada.</response>
        [HttpGet("{id}")]
        public async Task<IActionResult> Obter(Guid id)
        {
            var note = await _db.Notes.FindAsync(id);
            if (note == null) return NotFound(new { error = "Nota não encontrada." });

            var userRole = User.FindFirstValue(ClaimTypes.Role);
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // 🔒 Leitores e Editores só podem acessar suas próprias notas
            if (userRole != "Admin" && note.UserId.ToString() != userId)
                return Forbid();

            return Ok(note);
        }

        /// <summary>
        /// ✏️ Atualiza o conteúdo de uma nota existente.
        /// </summary>
        /// <remarks>
        /// **Regras de acesso:**  
        /// - **Admin:** pode editar qualquer nota.  
        /// - **Editor:** pode editar apenas suas próprias notas.
        /// 
        /// **Exemplo de requisição:**
        /// ```json
        /// {
        ///   "title": "Relatório Atualizado",
        ///   "content": "Nova versão do relatório com dados atualizados..."
        /// }
        /// ```
        /// </remarks>
        /// <param name="id">ID da nota a ser atualizada.</param>
        /// <param name="dto">Novo título e conteúdo da nota.</param>
        /// <response code="200">Nota atualizada com sucesso.</response>
        /// <response code="403">Usuário não tem permissão para editar esta nota.</response>
        /// <response code="404">Nota não encontrada.</response>
        [HttpPut("{id}")]
        [Authorize(Roles = "Editor,Admin")]
        public async Task<IActionResult> Atualizar(Guid id, [FromBody] NoteUpdateDto dto)
        {
            var note = await _db.Notes.FindAsync(id);
            if (note == null) return NotFound(new { error = "Nota não encontrada." });

            var userRole = User.FindFirstValue(ClaimTypes.Role);
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // 🔒 Editores só podem editar suas próprias notas
            if (userRole != "Admin" && note.UserId.ToString() != userId)
                return Forbid();

            note.Title = dto.Title;
            note.Content = dto.Content;
            await _db.SaveChangesAsync();

            return Ok(new { message = "✅ Nota atualizada com sucesso.", note });
        }

        /// <summary>
        /// 🗑️ Exclui uma nota do sistema.
        /// </summary>
        /// <remarks>
        /// **Regras de acesso:**  
        /// - Apenas usuários com role **Admin** podem excluir notas.
        /// 
        /// **Exemplo de requisição:**
        /// ```
        /// DELETE /api/v1/notes/3fa85f64-5717-4562-b3fc-2c963f66afa6
        /// ```
        /// </remarks>
        /// <param name="id">ID da nota a ser excluída.</param>
        /// <response code="200">Nota removida com sucesso.</response>
        /// <response code="403">Usuário não tem permissão para excluir notas.</response>
        /// <response code="404">Nota não encontrada.</response>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Deletar(Guid id)
        {
            var note = await _db.Notes.FindAsync(id);
            if (note == null) return NotFound(new { error = "Nota não encontrada." });

            _db.Notes.Remove(note);
            await _db.SaveChangesAsync();

            return Ok(new { message = "✅ Nota removida com sucesso." });
        }
    }
}