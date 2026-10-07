using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Lab2.WebApi
{
    [ApiController]
    [Route("user")]
    public class UserController : ControllerBase
    {
        private readonly AppDbContext _db;
        public UserController(AppDbContext db) => _db = db;

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] User dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Login))
                return BadRequest(new { error = "Логин обязателен" });

            if (await _db.Users.AnyAsync(u => u.Login == dto.Login))
                return Conflict(new { error = "Логин уже занят" });

            dto.PassHash = BCrypt.Net.BCrypt.HashPassword(dto.PassHash);
            _db.Users.Add(dto);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = dto.Id },
                new { dto.Id, dto.Login });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var u = await _db.Users.FindAsync(id);
            if (u == null) return NotFound(new { error = "Не найден" });
            return Ok(new { u.Id, u.Login });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] User dto)
        {
            var u = await _db.Users.FindAsync(id);
            if (u == null) return NotFound();
            if (!string.IsNullOrWhiteSpace(dto.Login)) u.Login = dto.Login;
            if (!string.IsNullOrWhiteSpace(dto.PassHash))
                u.PassHash = BCrypt.Net.BCrypt.HashPassword(dto.PassHash);
            await _db.SaveChangesAsync();
            return Ok(new { message = "Обновлено" });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var u = await _db.Users.FindAsync(id);
            if (u == null) return NotFound();
            _db.Users.Remove(u);
            await _db.SaveChangesAsync();
            return Ok(new { message = "Удалено" });
        }
    }
}
