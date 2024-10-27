using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Simbir.Health.AccountAPI.Model.Database.DTO;
using Simbir.Health.AccountAPI.SDK.Services;

namespace Simbir.Health.AccountAPI.Controllers
{
    [Route("api/Accounts/")]
    [ApiController]
    public class AccountsController : ControllerBase
    {

        private readonly IDatabaseService _database;
        private readonly IJwtService _jwt;

        private readonly ICacheService _cache;

        public AccountsController(IDatabaseService database, IJwtService jwt, ICacheService cache, IConfiguration configuration)
        {
            _database = database;
            _jwt = jwt;
            _cache = cache;
        }

        /// <summary>
        /// Получение данных о текущем аккаунте
        /// </summary>
        /// <remarks>
        /// Нужна авторизация по accessToken'у
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
        [HttpGet("Me")]
        public async Task<IActionResult> GetProfileOfUser()
        {
            string bearer_key = Request.Headers["Authorization"];

            var validation = await _jwt.AccessTokenValidation(bearer_key);

            if (validation.TokenHasError())
            {
                return Unauthorized();
            }
            else if (validation.TokenHasSuccess())
            {
                var info_user = _database.InfoAccounts(validation.token_success.Id);

                if (info_user != null)
                    return Ok(info_user);
                else
                    return BadRequest("account_not_found");
            }

            return BadRequest();
        }

        /// <summary>
        /// Обновление своего аккаунта
        /// </summary>
        /// <remarks>
        /// Нужна авторизация по accessToken'у
        /// 
        /// Пример авторизации:
        /// 
        ///     Bearer eyJhbGci...
        /// 
        /// Пример данного запроса:
        ///
        ///     PUT
        ///     {
        ///        "lastName": "Новая фамилия",
        ///        "firstName": "Новое имя",
        ///        "password": "Новый пароль"
        ///     }
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
        [HttpPut("Update")]
        public async Task<IActionResult> UpdateAccount([FromBody] Accounts_Update dtoObj)
        {

            string bearer_key = Request.Headers["Authorization"];

            var validation = await _jwt.AccessTokenValidation(bearer_key);

            if (validation.TokenHasError())
            {
                return Unauthorized();
            }
            else if (validation.TokenHasSuccess())
            {
                try
                {
                    await _database.UpdateAccount(dtoObj, validation.token_success.Id);
                    return Ok("account_updated");
                }
                catch (Exception e) {
                    return BadRequest();
                }
            }

            return BadRequest();
        }

        /// <summary>
        /// Получение списка всех аккаунтов
        /// </summary>
        /// <remarks>
        /// Нужна авторизация по accessToken'у
        /// 
        /// Можно указать выборку
        /// 
        /// Доступ: <font color="red">Только администраторы</font>
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
        public async Task<IActionResult> GetAll([FromQuery] int from, [FromQuery] int count)
        {
            string bearer_key = Request.Headers["Authorization"];

            var validation = await _jwt.AccessTokenValidation(bearer_key, "Admin");

            if (validation.TokenHasError())
            {
                return Unauthorized(validation.token_error.errorLog);
            }
            else if (validation.TokenHasSuccess())
            {
                return Ok(_database.GetAllAccounts(from, count));
            }


            return BadRequest();
        }

        /// <summary>
        /// Создание администратором нового аккаунта
        /// </summary>
        /// <remarks>
        /// Нужна авторизация по accessToken'у
        /// 
        /// Доступ: <font color="red">Только администраторы</font>
        /// 
        /// Пример авторизации:
        /// 
        ///     Bearer eyJhbGci...
        /// 
        /// Пример данного запроса:
        ///
        ///     POST
        ///     {
        ///        "lastName": "Фамилия",
        ///        "firstName": "Имя",
        ///        "username": "Никнейм",
        ///        "password": "Пароль",
        ///        "roles": [
        ///          "Роль"
        ///        ]
        ///     }
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
        [HttpPost]
        public async Task<IActionResult> CreateUserWithAdmin([FromBody] Accounts_CreateUser dtoObj)
        {
            string bearer_key = Request.Headers["Authorization"];

            var validation = await _jwt.AccessTokenValidation(bearer_key, "Admin");

            if (validation.TokenHasError())
            {
                return Unauthorized(validation.token_error.errorLog);
            }
            else if (validation.TokenHasSuccess())
            {
                try
                {
                    await _database.RegisterUserWithAdmin(dtoObj);
                    return Ok($"account_created");
                }
                catch (Exception e)
                {
                    return BadRequest();
                }
            }

            return BadRequest();
        }

        /// <summary>
        /// Изменение администратором аккаунта по id
        /// </summary>
        /// <remarks>
        /// Нужна авторизация по accessToken'у
        /// 
        /// Нужно ввести id аккаунта
        /// 
        /// Доступ: <font color="red">Только администраторы</font>
        /// 
        /// Пример авторизации:
        /// 
        ///     Bearer eyJhbGci...
        /// 
        /// Пример данного запроса:
        ///
        ///     PUT
        ///     {
        ///        "lastName": "Новая фамилия",
        ///        "firstName": "Новое имя",
        ///        "username": "Новый никнейм",
        ///        "password": "Новый пароль",
        ///        "roles": [
        ///          "Роль"
        ///        ]
        ///     }
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
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAccountWithAdmin(int id, [FromBody] Accounts_UpdateUser dtoObj)
        {
            string bearer_key = Request.Headers["Authorization"];

            var validation = await _jwt.AccessTokenValidation(bearer_key, "Admin");

            if (validation.TokenHasError())
            {
                return Unauthorized(validation.token_error.errorLog);
            }
            else if (validation.TokenHasSuccess())
            {
                try
                {
                    await _database.UpdateAccountWithAdmin(dtoObj, id);
                    return Ok($"account_changed");
                }
                catch (Exception e)
                {
                    return BadRequest();
                }
            }

            return BadRequest();
        }

        /// <summary>
        /// Мягкое удаление аккаунта по id 
        /// </summary>
        /// <remarks>
        /// Нужна авторизация по accessToken'у
        /// 
        /// Нужно ввести id аккаунта
        /// 
        /// Доступ: <font color="red">Только администраторы</font>
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
        [HttpDelete("{id}")] 
        public async Task<IActionResult> DeleteAccountWithAdmin(int id)
        {
            string bearer_key = Request.Headers["Authorization"];

            var validation = await _jwt.AccessTokenValidation(bearer_key, "Admin");

            if (validation.TokenHasError())
            {
                return Unauthorized(validation.token_error.errorLog);
            }
            else if (validation.TokenHasSuccess())
            {
                try
                {
                    await _database.DeleteAccountWithAdmin(id);
                    return Ok($"account_deleted");
                }
                catch (Exception e)
                {
                    return BadRequest();
                }
            }


            return BadRequest();
        }

    }
}
