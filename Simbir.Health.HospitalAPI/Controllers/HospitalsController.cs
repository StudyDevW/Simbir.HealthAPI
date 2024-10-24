using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Simbir.Health.HospitalAPI.Model.Database.DTO;
using Simbir.Health.HospitalAPI.SDK.Services;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Text.Json;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Model;

namespace Simbir.Health.HospitalAPI.Controllers
{
    [Route("api/Hospitals/")]
    [ApiController]
    public class HospitalsController : ControllerBase
    {
        private readonly IDatabaseService _database;
        private readonly ICacheService _cache;
        private readonly HttpClient _httpClient;

        public HospitalsController(HttpClient httpClient, IDatabaseService database, ICacheService cache)
        {
            _httpClient = httpClient;
            _database = database;
            _cache = cache;
        }

        [Authorize(AuthenticationSchemes = "Asymmetric")]
        [HttpGet]
        public async Task<IActionResult> GetHospitals([FromQuery] int from, [FromQuery] int count)
        {
            string bearer_key = Request.Headers["Authorization"];

            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, "http://simbir.health.accountapi:80/api/Authentication/Validate");

                request.Headers.Add("accessToken", bearer_key.Substring("Bearer ".Length));

                var response = await _httpClient.SendAsync(request);

                if (response.StatusCode == HttpStatusCode.Unauthorized)
                    return Unauthorized();

                if (response.Content.ReadAsStringAsync().Result == "valid")
                    return Ok(_database.GetAllHospitals(from, count));

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

        //Сделать
        [Authorize(AuthenticationSchemes = "Asymmetric")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetHospitalFromId(int id)
        {
            string bearer_key = Request.Headers["Authorization"];

            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, "http://simbir.health.accountapi:80/api/Authentication/Validate");

                request.Headers.Add("accessToken", bearer_key.Substring("Bearer ".Length));

                var response = await _httpClient.SendAsync(request);

                if (response.StatusCode == HttpStatusCode.Unauthorized)
                    return Unauthorized();

                if (response.Content.ReadAsStringAsync().Result == "valid")
                    return Ok(_database.GetHospitalFromId(id));

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

        //Сделать
        [Authorize(AuthenticationSchemes = "Asymmetric")]
        [HttpGet("{id}/Rooms")]
        public async Task<IActionResult> GetHospitalRoomsFromId(int id)
        {
            string bearer_key = Request.Headers["Authorization"];

            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, "http://simbir.health.accountapi:80/api/Authentication/Validate");

                request.Headers.Add("accessToken", bearer_key.Substring("Bearer ".Length));

                var response = await _httpClient.SendAsync(request);

                if (response.StatusCode == HttpStatusCode.Unauthorized)
                    return Unauthorized();

                if (response.Content.ReadAsStringAsync().Result == "valid")
                    return Ok(_database.GetHospitalRoomsFromId(id));

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


            return BadRequest();
        }

        [Authorize(AuthenticationSchemes = "Asymmetric")]
        [HttpPost]
        public async Task<IActionResult> CreateHospital([FromBody] Hospitals_Create dtoObj)
        {
            string bearer_key = Request.Headers["Authorization"];

            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, "http://simbir.health.accountapi:80/api/Authentication/Validate");

                request.Headers.Add("accessToken", bearer_key.Substring("Bearer ".Length));

                var response = await _httpClient.SendAsync(request);

                if (response.StatusCode == HttpStatusCode.Unauthorized)
                    return Unauthorized();

                if (response.Content.ReadAsStringAsync().Result.Equals("valid"))
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

                    if (userRoles != null && userRoles.Contains("Admin"))
                    {
                        await _database.CreateHospital(dtoObj);
                        return Ok();
                    }
                    else if (userRoles == null)
                    {
                        return BadRequest("Roles not found");
                    }
                    else
                    {
                        return BadRequest("Administrators only");
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

        //Сделать
        [Authorize(AuthenticationSchemes = "Asymmetric")]
        [HttpPut("{id}")]
        public async Task<IActionResult> ChangeHospitalFromId(int id, [FromBody] Hospitals_Create dtoObj)
        {
            string bearer_key = Request.Headers["Authorization"];

            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, "http://simbir.health.accountapi:80/api/Authentication/Validate");

                request.Headers.Add("accessToken", bearer_key.Substring("Bearer ".Length));

                var response = await _httpClient.SendAsync(request);

                if (response.StatusCode == HttpStatusCode.Unauthorized)
                    return Unauthorized();

                if (response.Content.ReadAsStringAsync().Result.Equals("valid"))
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

                    if (userRoles != null && userRoles.Contains("Admin"))
                    {
                        await _database.UpdateHospital(dtoObj, id);
                        return Ok();
                    }
                    else if (userRoles == null)
                    {
                        return BadRequest("Roles not found");
                    }
                    else
                    {
                        return BadRequest("Administrators only");
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

        //Сделать
        [Authorize(AuthenticationSchemes = "Asymmetric")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteHospitalFromId(int id)
        {
            string bearer_key = Request.Headers["Authorization"];

            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, "http://simbir.health.accountapi:80/api/Authentication/Validate");

                request.Headers.Add("accessToken", bearer_key.Substring("Bearer ".Length));

                var response = await _httpClient.SendAsync(request);

                if (response.StatusCode == HttpStatusCode.Unauthorized)
                    return Unauthorized();

                if (response.Content.ReadAsStringAsync().Result.Equals("valid"))
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

                    if (userRoles != null && userRoles.Contains("Admin"))
                    {
                        await _database.DeleteHospitalWithAdmin(id);
                        return Ok();
                    }
                    else if (userRoles == null)
                    {
                        return BadRequest("Roles not found");
                    }
                    else
                    {
                        return BadRequest("Administrators only");
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


        //[HttpGet("Test_Get_Messages")]
        //public async Task<IActionResult> GetMessagesTest()
        //{
        //    //try
        //    //{
        //    //    var messages = await _rabbitmq.GetMessages("test_queue");

        //    //    if (messages.messages_consumed != null)
        //    //    {
        //    //        return Ok(messages.messages_consumed);
        //    //    }

        //    //    return BadRequest();
        //    //}
        //    //catch (Exception e) { 

        //        return BadRequest();
        //    //}
        //}

    }
}
