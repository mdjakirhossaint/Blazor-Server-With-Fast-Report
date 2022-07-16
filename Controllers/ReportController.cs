using FastReport.Data;
using FastReport.Export.PdfSimple;
using FastReport.Web;
using FastReportBlazorServer.Data;
using Microsoft.AspNetCore.Mvc;

namespace FastReportBlazorServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public ReportController(IWebHostEnvironment webHostEnvironment, ApplicationDbContext context, IConfiguration configuration)
        {
            this._webHostEnvironment = webHostEnvironment;
            this._context = context;
            this._configuration = configuration;
        }
        [HttpGet("Index")]
        public IActionResult Index()
        {
            WebReport web = new WebReport();
            // Load the Fast Report
            var path = $"{this._webHostEnvironment.WebRootPath}\\Reports\\expanse3.frx";
            web.Report.Load(path);

            // Passing ConnectionString In Fast Report
            var mssqlDataConnection = new MsSqlDataConnection();
            mssqlDataConnection.ConnectionString = _configuration.GetConnectionString("DefaultConnection");
            var Conn = mssqlDataConnection.ConnectionString;
            web.Report.SetParameterValue("CONN", Conn);

            //web.Report.SetParameterValue("TblCategoryID", ID);

            var Categoryname = _context.Products.ToList();
            var catName = "Name";
            web.Report.SetParameterValue("CategoryName", catName);

            //Render The Report Pdf
            // prepare report
            web.Report.Prepare();
            //// save file in stream
            Stream stream = new MemoryStream();
            web.Report.Export(new PDFSimpleExport(), stream);
            stream.Position = 0;
            //// return stream in browser
            return File(stream, "application/zip", "report.pdf");
            //return web;
        }
    }
}
