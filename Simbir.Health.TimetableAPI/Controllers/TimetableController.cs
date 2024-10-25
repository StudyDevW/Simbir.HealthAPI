using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Simbir.Health.TimetableAPI.Model.Database.DTO;
using Simbir.Health.TimetableAPI.SDK.Services;
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

        public TimetableController(HttpClient httpClient, IDatabaseService database)
        {
            _httpClient = httpClient;
            _database = database;
        }

        [Authorize(AuthenticationSchemes = "Asymmetric")]
        [HttpPost]
        public async Task<IActionResult> CreateRecord(Timetable_Create dtoObj)
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

    }
}
