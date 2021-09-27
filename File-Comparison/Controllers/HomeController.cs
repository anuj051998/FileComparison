using File_Comparison.Repositiry;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.IO;

namespace File_Comparison.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IFileRepo _repository;
        public HomeController(ILogger<HomeController> logger, IFileRepo _repo)
        {
            _logger = logger;
            _repository = _repo;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Privacy()
        {
            return View();
        }
        public IActionResult verifyPath(string path)
        {
            return Content(_repository.verifyPath(path));
        }
        [HttpPost]
        public IActionResult ReadDirectory(string folderOnePath, string folderTwoPath, string test = null)
        {
            ViewBag.fileNameOne = folderOnePath;
            ViewBag.fileNameTwo = folderTwoPath;
            return View();
        }
        //[HttpPost, ValidateInput(false)]
        [HttpPost]
        public IActionResult updateContent(string filePath, string content)
        {
           return Content(_repository.updateContent(filePath, content));
        }
        [HttpPost]
        public IActionResult Read(string folderOnePath, string folderTwoPath)
        {
            return Json(_repository.check(folderOnePath, folderTwoPath));
        }
        public string ReadFileContent(string fileName)
        {
            return _repository.readFileContents(fileName);
        }
        [HttpGet]
        public IActionResult ReadContents(string pathOne, string pathTwo)
        {
            ViewBag.PathOne = pathOne;
            ViewBag.PathTwo = pathTwo;
            return View();
        }
        [HttpPost]
        public IActionResult ShowFiles(string fileOne, string fileTwo)
        {
            return View();
        }
        [HttpGet]
        public IActionResult getCheckList()
        {
            return Content(_repository.getChecklist());
        }
        [HttpGet]
        public IActionResult GetList(string fileOne, string fileTwo)
        {
           return Content( _repository.fakeList(fileOne, fileTwo));
        }
        [HttpGet]
        public IActionResult Test(string path)
        {
            string[] li = Directory.GetFiles(path, "*", SearchOption.AllDirectories);
            return Content(li[0]);
        }
        //public string GetContents(string filePathOne, string filePathTwo)
        //{
        //    return "";
        //}
    }
}
