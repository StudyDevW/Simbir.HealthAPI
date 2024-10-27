using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client;
using Simbir.Health.TimetableAPI.Model.Database.DTO;
using Simbir.Health.TimetableAPI.SDK.Services;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Text.Json;

namespace Simbir.Health.TimetableAPI.Controllers
{
    [Route("api/Timetable/")]
    [ApiController]
    public class TimetableController : ControllerBase
    {
        private readonly IDatabaseService _database;
        private readonly HttpClient _httpClient;
        private readonly ICacheService _cache;


        public TimetableController(HttpClient httpClient, IDatabaseService database, ICacheService cache)
        {
            _httpClient = httpClient;
            _database = database;
            _cache = cache;
        }

        /// <summary>
        /// Создание новой записи в расписании
        /// </summary>
        /// <remarks>
        /// Нужна авторизация по accessToken'у
        /// 
        /// Доступ: <font color="red">Только администраторы и менеджеры</font>
        /// 
        /// - <font color="orange">
        ///    {from} и {to} - количество минут всегда кратно 30, секунды всегда 0.
        ///    (пример: “2024-04-25T11:30:00Z”, “2024-04-25T12:00:00Z”). {to} > {from}
        ///     Разница между { to } и {from} не должна превышать 12 часов. 
        ///   </font>
        /// 
        /// Пример авторизации:
        /// 
        ///     Bearer eyJhbGci...
        /// 
        /// Пример данного запроса:
        ///
        ///     POST
        ///     {
        ///        "hospitalId": id больницы(int),
        ///        "doctorId": id доктора(int),
        ///        "from": "2024-01-01T11:30:00Z",             
        ///        "to": "2024-01-01T12:00:00Z",
        ///        "room": "Кабинет ..." 
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
        public async Task<IActionResult> CreateRecord([FromBody] Timetable_Create dtoObj)
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

                    if (jwtToken != null)
                    {
                        userRoles = JsonSerializer.Deserialize<List<string>>(
                            jwtToken.Claims.FirstOrDefault(o => o.Type == "Roles").Value
                        );
                    }
                    else
                    {
                        return BadRequest("Read jwt error");
                    }

                    if (userRoles != null && (userRoles.Contains("Admin") || userRoles.Contains("Manager")))
                    {
                        await _database.CreateRecordTimetable(dtoObj);

                        return Ok();
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
        /// Обновление записи расписания
        /// </summary>
        /// <remarks>
        /// Нужна авторизация по accessToken'у
        /// 
        /// Нужно ввести id записи расписания
        /// 
        /// Доступ: <font color="red">Только администраторы и менеджеры</font>
        /// 
        /// - <font color="red">Нельзя изменить если есть записавшиеся на прием</font>
        ///
        /// 
        /// - <font color="orange">
        ///    {from} и {to} - количество минут всегда кратно 30, секунды всегда 0.
        ///    (пример: “2024-04-25T11:30:00Z”, “2024-04-25T12:00:00Z”). {to} > {from}
        ///     Разница между { to } и {from} не должна превышать 12 часов. 
        ///   </font>
        /// 
        /// Пример авторизации:
        /// 
        ///     Bearer eyJhbGci...
        /// 
        /// Пример данного запроса:
        ///
        ///     PUT
        ///     {
        ///        "hospitalId": новое id больницы(int),
        ///        "doctorId": новое id доктора(int),
        ///        "from": "2024-01-01T11:30:00Z",             
        ///        "to": "2024-01-01T12:00:00Z",
        ///        "room": "Новый кабинет ..." 
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
        public async Task<IActionResult> UpdateRecord(int id, [FromBody] Timetable_Create dtoObj)
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

                    if (jwtToken != null)
                    {
                        userRoles = JsonSerializer.Deserialize<List<string>>(
                            jwtToken.Claims.FirstOrDefault(o => o.Type == "Roles").Value
                        );
                    }
                    else
                    {
                        return BadRequest("Read jwt error");
                    }

                    if (userRoles != null && (userRoles.Contains("Admin") || userRoles.Contains("Manager")))
                    {
                        //должна быть проверка, записан ли кто то на прием или нет


                        await _database.UpdateRecordTimetable(id, dtoObj);

                        return Ok();
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
        /// Удаление записи расписания
        /// </summary>
        /// <remarks>
        /// Нужна авторизация по accessToken'у
        /// 
        /// Доступ: <font color="red">Только администраторы и менеджеры</font>
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
        public async Task<IActionResult> DeleteRecord(int id)
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

                    if (jwtToken != null)
                    {
                        userRoles = JsonSerializer.Deserialize<List<string>>(
                            jwtToken.Claims.FirstOrDefault(o => o.Type == "Roles").Value
                        );
                    }
                    else
                    {
                        return BadRequest("Read jwt error");
                    }

                    if (userRoles != null && (userRoles.Contains("Admin") || userRoles.Contains("Manager")))
                    {
                        await _database.DeleteRecordTimetable(id);

                        return Ok();
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
        /// Удаление записей расписания доктора
        /// </summary>
        /// <remarks>
        /// Нужна авторизация по accessToken'у
        /// 
        /// Нужно ввести id доктора
        /// 
        /// Доступ: <font color="red">Только администраторы и менеджеры</font>
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
        [HttpDelete("Doctor/{id}")]
        public async Task<IActionResult> DeleteRecordDoctor(int id)
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

                    if (jwtToken != null)
                    {
                        userRoles = JsonSerializer.Deserialize<List<string>>(
                            jwtToken.Claims.FirstOrDefault(o => o.Type == "Roles").Value
                        );
                    }
                    else
                    {
                        return BadRequest("Read jwt error");
                    }

                    if (userRoles != null && (userRoles.Contains("Admin") || userRoles.Contains("Manager")))
                    {
                        await _database.DeleteDoctorRecordTimetable(id);

                        return Ok();
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
        /// Удаление записей расписания больницы
        /// </summary>
        /// <remarks>
        /// Нужна авторизация по accessToken'у
        /// 
        /// Нужно ввести id больницы
        /// 
        /// Доступ: <font color="red">Только администраторы и менеджеры</font>
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
        [HttpDelete("Hospital/{id}")]
        public async Task<IActionResult> DeleteRecordHospital(int id)
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

                    if (jwtToken != null)
                    {
                        userRoles = JsonSerializer.Deserialize<List<string>>(
                            jwtToken.Claims.FirstOrDefault(o => o.Type == "Roles").Value
                        );
                    }
                    else
                    {
                        return BadRequest("Read jwt error");
                    }

                    if (userRoles != null && (userRoles.Contains("Admin") || userRoles.Contains("Manager")))
                    {
                        await _database.DeleteDoctorRecordTimetable(id);

                        return Ok();
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
        /// Получение расписания больницы по Id 
        /// </summary>
        /// <remarks>
        /// Нужна авторизация по accessToken'у
        /// 
        /// Нужно указать id больницы
        /// 
        /// Нужно указать выборку
        /// 
        /// Пример выборки:
        ///     
        ///     from: "2024-01-01T11:00:00Z"
        ///     to: "2024-01-01T12:30:00Z"
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
        [HttpGet("Hospital/{id}")]
        public async Task<IActionResult> GetRecordsHospital(int id, [FromHeader] DateTime from, [FromHeader] DateTime to)
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
                    return Ok(_database.GetAllHospitalsRecords(id, from, to));
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
        /// Получение расписания врача по Id
        /// </summary>
        /// <remarks>
        /// Нужна авторизация по accessToken'у
        /// 
        /// Нужно указать id врача
        /// 
        /// Нужно указать выборку
        /// 
        /// Пример выборки:
        ///     
        ///     from: "2024-01-01T11:00:00Z"
        ///     to: "2024-01-01T12:30:00Z"
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
        [HttpGet("Doctor/{id}")]
        public async Task<IActionResult> GetRecordsDoctor(int id, [FromHeader] DateTime from, [FromHeader] DateTime to)
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
                    return Ok(_database.GetAllDoctorRecords(id, from, to));
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
        /// Получение расписания кабинета больницы
        /// </summary>
        /// <remarks>
        /// Нужна авторизация по accessToken'у
        /// 
        /// Доступ: <font color="red">Только администраторы и менеджеры и врачи</font>
        /// 
        /// Нужно указать id больницы
        /// 
        /// Нужно указать room больницы
        /// 
        /// Нужно указать выборку
        /// 
        /// Пример выборки:
        ///     
        ///     from: "2024-01-01T11:00:00Z"
        ///     to: "2024-01-01T12:30:00Z"
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
        [HttpGet("Hospital/{id}/Room/{room}")]
        public async Task<IActionResult> GetRecordsDoctor(int id, string room, [FromHeader] DateTime from, [FromHeader] DateTime to)
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

                    if (jwtToken != null)
                    {
                        userRoles = JsonSerializer.Deserialize<List<string>>(
                            jwtToken.Claims.FirstOrDefault(o => o.Type == "Roles").Value
                        );
                    }
                    else
                    {
                        return BadRequest("Read jwt error");
                    }

                    if (userRoles != null && (userRoles.Contains("Admin") 
                        || userRoles.Contains("Manager") 
                        || userRoles.Contains("Doctor")))
                    {
                        return Ok(_database.GetRoomRecord(id, room, from, to));
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
        /// Получение свободных талонов на приём
        /// </summary>
        /// <remarks>
        /// Нужна авторизация по accessToken'у
        /// 
        /// Нужно указать id записи расписания
        /// 
        /// - <font color="orange">
        ///    Каждые 30 минут из записи расписания - это один талон. 
        ///    Если в сущности Timetable from=2024-04-25T11:00:00Z, to=2024-04-25T12:30:00Z, 
        ///    то запись доступна на: 2024-04-25T11:00:00Z, 2024-04-25T11:30:00Z, 2024-04-25T12:00:00Z.
        ///   </font>
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
        [HttpGet("{id}/Appointments")]
        public async Task<IActionResult> GetAppointments(int id/*timetable_id*/)
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
                    return Ok(_database.FreeAppointments(id));
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
        /// Записаться на приём
        /// </summary>
        /// <remarks>
        /// Нужна авторизация по accessToken'у
        /// 
        /// Нужно указать id записи расписания
        /// 
        /// Пример авторизации:
        /// 
        ///     Bearer eyJhbGci...
        /// 
        /// Пример данного запроса:
        ///
        ///     POST
        ///     {
        ///        "time": "2024-04-25T11:00:00Z"
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
        [HttpPost("{id}/Appointments")]
        public async Task<IActionResult> AppointmentWrite(int id, [FromBody] Appointments_Write dtoObj)
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
                        await _database.WriteAppointment(id, userId, dtoObj.time);
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
