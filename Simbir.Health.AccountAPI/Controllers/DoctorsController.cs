using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Simbir.Health.AccountAPI.SDK.Services;
using System.ComponentModel.DataAnnotations;

namespace Simbir.Health.AccountAPI.Controllers
{
    [Route("api/Doctors/")]
    [ApiController]
    public class DoctorsController : ControllerBase
    {
        private readonly IDatabaseService _database;
        private readonly IJwtService _jwt;

        private readonly ICacheService _cache;

        public DoctorsController(IDatabaseService database, IJwtService jwt, ICacheService cache, IConfiguration configuration)
        {
            _database = database;
            _jwt = jwt;
            _cache = cache;
        }

        /// <summary>
        /// Получение списка докторов
        /// </summary>
        /// <remarks>
        /// Нужна авторизация по accessToken'у
        /// 
        /// Можно отфильтровать по имени, фамилии
        /// 
        /// Можно указать выборку
        /// 
        /// Пример фильтра:
        /// 
        ///     nameFilter: имя или фамилия
        /// 
        /// Пример выборки:
        ///     
        ///     from: начало выборки
        ///     count: размер выборки
        /// 
        /// Пример авторизации:
        /// 
        ///     Bearer eyJhbGci...
        /// 
        /// Информация по токенам:
        ///
        ///     Срок действия AT(accessToken) - 10 минут
        /// 
        ///     Срок действия RT(refreshToken) - 7 дней
        /// 
        ///     Алгоритм шифрования токенов RS512
        /// 
        /// Проверить токен можно на сайте <a href="https://jwt.io/" target="_blank">jwt.io</a>
        /// 
        /// </remarks>
        /// <returns></returns>
        [Authorize(AuthenticationSchemes = "Asymmetric")]
        [HttpGet]
        public async Task<IActionResult> GetDoctors([FromQuery] string nameFilter, [FromQuery] int from, [FromQuery] int count)
        {
            string bearer_key = Request.Headers["Authorization"];

            var validation = await _jwt.AccessTokenValidation(bearer_key);

            if (validation.TokenHasError())
            {
                return Unauthorized(validation.token_error.errorLog);
            }
            else if (validation.TokenHasSuccess())
            {
                return Ok(_database.GetAllDoctors(from, count, nameFilter));
            }

            return BadRequest();
        }

        /// <summary>
        /// Получение информации о докторе по Id
        /// </summary>
        /// <remarks>
        /// Нужна авторизация по accessToken'у
        /// 
        /// Нужно ввести id аккаунта
        /// 
        /// Пример авторизации:
        /// 
        ///     Bearer eyJhbGci...
        ///     
        /// Информация по токенам:
        ///
        ///     Срок действия AT(accessToken) - 10 минут
        /// 
        ///     Срок действия RT(refreshToken) - 7 дней
        /// 
        ///     Алгоритм шифрования токенов RS512
        /// 
        /// Проверить токен можно на сайте <a href="https://jwt.io/" target="_blank">jwt.io</a>
        /// 
        /// </remarks>
        /// <returns></returns>
        [Authorize(AuthenticationSchemes = "Asymmetric")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetDoctorFromId(int id)
        {
            try
            {
                var info = _database.InfoAccountDoctor(id);
                if (info != null)
                    return Ok(info);
                else
                    return NotFound("doctor_not_found");
            }
            catch (Exception e)
            {
                return NotFound();
            }
        }

    }
}
