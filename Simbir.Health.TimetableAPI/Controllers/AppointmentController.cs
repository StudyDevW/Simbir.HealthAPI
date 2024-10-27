using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Simbir.Health.TimetableAPI.SDK.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Net;

namespace Simbir.Health.TimetableAPI.Controllers
{
    [Route("api/Appointment/")]
    [ApiController]
    public class AppointmentController : ControllerBase
    {
        private readonly IDatabaseService _database;
        private readonly HttpClient _httpClient;


        public AppointmentController(HttpClient httpClient, IDatabaseService database)
        {
            _httpClient = httpClient;
            _database = database;
        }

        /// <summary>
        /// Отменить запись на приём 
        /// </summary>
        /// <remarks>
        /// Нужна авторизация по accessToken'у
        /// 
        /// Нужно ввести id записи на прием
        /// 
        /// Доступ: <font color="red">Только администраторы, менеджеры, и записавшийся пользователь</font>
        /// 
        /// Пример авторизации:
        /// 
        ///     Bearer eyJhbGci...
        /// 
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
        [HttpDelete("{id}")]
        public async Task<IActionResult> AppointmentDelete(int id)
        {
            string bearer_key = Request.Headers["Authorization"];

            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, "http://simbir.health.accountapi:80/api/Authentication/Validate");

                request.Headers.Add("accessToken", bearer_key.Substring("Bearer ".Length));

                var response = await _httpClient.SendAsync(request);

                if (response.StatusCode == HttpStatusCode.Unauthorized)
                    return Unauthorized();

                if (response.StatusCode == HttpStatusCode.OK)
                {

                    var handler = new JwtSecurityTokenHandler();
                    var jwtToken = handler.ReadJwtToken(bearer_key.Substring("Bearer ".Length));

                    var userId = -1;

                    if (jwtToken != null)
                    {
                        userId = int.Parse(jwtToken.Claims.FirstOrDefault(o => o.Type == "Id").Value);
                    }

                    if (userId > -1)
                    {
                        await _database.DeleteAppoinment(id, userId);
                        return Ok();
                    }
                    else
                    {
                        return BadRequest("read_id_error");
                    }
                }

                return BadRequest();
            }
            catch (HttpRequestException ex)
            {
                return BadRequest();
            }
            catch (Exception ex)
            {
                return BadRequest();
            }

        }
    }
}
