using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Simbir.Health.DocumentAPI.Model.Database.DTO;
using Simbir.Health.DocumentAPI.SDK.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Text.Json;

namespace Simbir.Health.DocumentAPI.Controllers
{
    [Route("api/History/")]
    [ApiController]
    public class HistoryController : ControllerBase
    {
        private readonly IDatabaseService _database;
        private readonly HttpClient _httpClient;

        public HistoryController(HttpClient httpClient, IDatabaseService database)
        {
            _httpClient = httpClient;
            _database = database;
        }

        /// <summary>
        /// Создание истории посещения и назначения
        /// </summary>
        /// <remarks>
        /// Нужна авторизация по accessToken'у
        /// 
        /// Доступ: <font color="red">Только администраторы и менеджеры и врачи</font>
        ///
        /// Пример авторизации:
        /// 
        ///     Bearer eyJhbGci...
        /// 
        /// Пример данного запроса:
        ///
        ///     POST
        ///     {
        ///         “date”: “2024-01-01T00:00:00Z”, 
        ///         “pacientId”: 1, 
        ///         “hospitalId”: 1, 
        ///         “doctorId”: 1, 
        ///         "room": "Кабинет...", 
        ///         “data”: “Информация”
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
        [HttpPost]
        public async Task<IActionResult> CreateHistory([FromBody] History_Create dtoObj)
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
                    List<string>? userRoles = new List<string>();

                    var handler = new JwtSecurityTokenHandler();
                    var jwtToken = handler.ReadJwtToken(bearer_key.Substring("Bearer ".Length));

                    var userId = -1;

                    if (jwtToken != null)
                    {
                        userRoles = JsonSerializer.Deserialize<List<string>>(
                            jwtToken.Claims.FirstOrDefault(o => o.Type == "Roles").Value
                        );

                        userId = int.Parse(jwtToken.Claims.FirstOrDefault(o => o.Type == "Id").Value);
                    }
                    else
                    {
                        return BadRequest("Read jwt error");
                    }

                    if (userRoles != null && (userRoles.Contains("Admin")
                        || userRoles.Contains("Manager")
                        || userRoles.Contains("Doctor")))
                    {
                        if (userRoles.Contains("User"))
                        {
                            if (userId > -1)
                            {
                                await _database.CreateHistory(userId, dtoObj);

                                return Ok();
                            }



                        }


                        //await _database.CreateRecordTimetable(dtoObj);


                    }
                    else if (userRoles == null)
                    {
                        return BadRequest("Roles not found");
                    }
                    else
                    {
                        return BadRequest("Administrators and Manager only");
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


        /// <summary>
        /// Обновление истории посещения и назначения
        /// </summary>
        /// <remarks>
        /// Нужна авторизация по accessToken'у
        /// 
        /// Доступ: <font color="red">Только администраторы и менеджеры и врачи</font>
        ///
        /// Пример авторизации:
        /// 
        ///     Bearer eyJhbGci...
        /// 
        /// Пример данного запроса:
        ///
        ///     PUT
        ///     {
        ///         “date”: “2024-01-01T00:00:00Z”, 
        ///         “pacientId”: 1, 
        ///         “hospitalId”: 1, 
        ///         “doctorId”: 1, 
        ///         "room": "Кабинет...", 
        ///         “data”: “Информация”
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
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateHistory(int id, [FromBody] History_Create dtoObj)
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
                    List<string>? userRoles = new List<string>();

                    var handler = new JwtSecurityTokenHandler();
                    var jwtToken = handler.ReadJwtToken(bearer_key.Substring("Bearer ".Length));

                    var userId = -1;

                    if (jwtToken != null)
                    {
                        userRoles = JsonSerializer.Deserialize<List<string>>(
                            jwtToken.Claims.FirstOrDefault(o => o.Type == "Roles").Value
                        );

                        userId = int.Parse(jwtToken.Claims.FirstOrDefault(o => o.Type == "Id").Value);
                    }
                    else
                    {
                        return BadRequest("Read jwt error");
                    }

                    if (userRoles != null && (userRoles.Contains("Admin")
                        || userRoles.Contains("Manager")
                        || userRoles.Contains("Doctor")))
                    {
                        if (userRoles.Contains("User"))
                        {
                            if (userId > -1)
                            {
                                await _database.UpdateHistory(id, dtoObj);

                                return Ok();
                            }



                        }


                        //await _database.CreateRecordTimetable(dtoObj);


                    }
                    else if (userRoles == null)
                    {
                        return BadRequest("Roles not found");
                    }
                    else
                    {
                        return BadRequest("Administrators and Manager only");
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

        /// <summary>
        /// Получение подробной информации о посещении и назначениях
        /// </summary>
        /// <remarks>
        /// Нужна авторизация по accessToken'у
        /// 
        /// Доступ: <font color="red">Только врачи и аккаунт, которому принадлежит история</font>
        ///
        /// Нужно ввести id истории
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
        [HttpGet("{id}")]
        public async Task<IActionResult> GetHistory(int id)
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
                    List<string>? userRoles = new List<string>();

                    var handler = new JwtSecurityTokenHandler();
                    var jwtToken = handler.ReadJwtToken(bearer_key.Substring("Bearer ".Length));

                    var userId = -1;

                    if (jwtToken != null)
                    {
                        userRoles = JsonSerializer.Deserialize<List<string>>(
                            jwtToken.Claims.FirstOrDefault(o => o.Type == "Roles").Value
                        );

                        userId = int.Parse(jwtToken.Claims.FirstOrDefault(o => o.Type == "Id").Value);
                    }
                    else
                    {
                        return BadRequest("Read jwt error");
                    }

                    if (userRoles != null && (userRoles.Contains("Admin")
                        || userRoles.Contains("Manager")
                        || userRoles.Contains("Doctor")))
                    {
                        if (userRoles.Contains("User"))
                        {
                            if (userId > -1)
                            {
                                if (_database.GetHistoryFromId(id, userId) != null)
                                {
                                    return Ok(_database.GetHistoryFromId(id, userId));
                                }
                            }
                        }
                        else
                        {
                            if (_database.GetHistoryFromId(id, -1) != null)
                            {
                                return Ok(_database.GetHistoryFromId(id, -1));
                            }
                        }

                        //await _database.CreateRecordTimetable(dtoObj);


                    }
                    else if (userRoles == null)
                    {
                        return BadRequest("Roles not found");
                    }
                    else
                    {
                        return BadRequest("Administrators and Manager only");
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

        /// <summary>
        /// Получение истории посещений и назначений аккаунта
        /// </summary>
        /// <remarks>
        /// Нужна авторизация по accessToken'у
        /// 
        /// Доступ: <font color="red">Только врачи и аккаунт, которому принадлежит история</font>
        ///
        /// Детали: <font color="orange">Возвращает записи где {pacientId}={id}</font>
        /// 
        /// Нужно ввести id пациента
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
        [HttpGet("Account/{id}")]
        public async Task<IActionResult> GetHistoryAccount(int id)
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
                    List<string>? userRoles = new List<string>();

                    var handler = new JwtSecurityTokenHandler();
                    var jwtToken = handler.ReadJwtToken(bearer_key.Substring("Bearer ".Length));

                    var userId = -1;

                    if (jwtToken != null)
                    {
                        userRoles = JsonSerializer.Deserialize<List<string>>(
                            jwtToken.Claims.FirstOrDefault(o => o.Type == "Roles").Value
                        );

                        userId = int.Parse(jwtToken.Claims.FirstOrDefault(o => o.Type == "Id").Value);
                    }
                    else
                    {
                        return BadRequest("Read jwt error");
                    }

                    if (userRoles != null && (userRoles.Contains("Admin")
                        || userRoles.Contains("Manager")
                        || userRoles.Contains("Doctor")))
                    {
                        if (userRoles.Contains("User"))
                        {
                            if (userId > -1)
                            {
                                if (_database.GetHistoryAccount(id, userId) != null)
                                {
                                    return Ok(_database.GetHistoryAccount(id, userId));
                                }
                            }
                        }
                        else
                        {
                            if (_database.GetHistoryAccount(id, -1) != null)
                            {
                                return Ok(_database.GetHistoryAccount(id, -1));
                            }
                        }

                        //await _database.CreateRecordTimetable(dtoObj);


                    }
                    else if (userRoles == null)
                    {
                        return BadRequest("Roles not found");
                    }
                    else
                    {
                        return BadRequest("Administrators and Manager only");
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
