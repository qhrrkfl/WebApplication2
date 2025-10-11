using Microsoft.AspNetCore.Mvc;
using System.Xml.Linq;

namespace DGRBWepAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DataController: ControllerBase
    {
        //https://localhost:{포트}/api/컨트롤러이름/메소드이름
        //https://localhost:{포트}/api/data/test 
        private readonly ILogger<DataController> _logger;

        public DataController(ILogger<DataController> logger)
        {
            _logger = logger;
        }
        #region TEST method 믿 사용법 예시 메소드
        [HttpGet("details")]  // GET /products/details/5
        public IActionResult Details([FromBody] string name)
        {
            return Content($"Product {name}");
        }


        [HttpPost("create")]       
        public IActionResult Create([FromBody] string name)
        {
            var result = new { message = $"Created {name}", success = true };
            return Ok(result);
        }

        [HttpGet("test/{id}")] 
        public IActionResult test(int id)
        {
            try
            {
                if (id <= 0)
                    throw new ArgumentException("Invalid ID.");

                // 정상 로직
                var result = new { message = $"Created {id}", success = true };
                _logger.LogInformation("Details({Id}) 호출됨", id); // 일반 로그
                return Ok(result);
            }
            catch (Exception ex)
            {
                // ⚠️ 예외 발생 시 Serilog error 로그로 기록됨
                _logger.LogError(ex, "Details({Id}) 처리 중 오류 발생", id);

                // 클라이언트 응답
                return Problem("Internal Server Error");
            }
        }
        #endregion
        



    }
}
