using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Route("api/secure")]
[ApiController]
[Authorize] // Bu endpoint sadece JWT token sahibi kullanıcılar için açık
public class SecureController : ControllerBase
{
    [HttpGet]
    public IActionResult GetSecureData()
    {
        return Ok(new { message = "Bu korumalı bir sayfadır. JWT ile erişildi!" });
    }
}
