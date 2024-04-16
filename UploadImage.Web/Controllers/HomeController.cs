using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using UploadImage.Data;
using System.Text.Json;
using UploadImage.Web.Models;
using System.Reflection;
using System.Diagnostics.Eventing.Reader;

namespace UploadImage.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        private string _connectionString = @"Data Source=.\sqlexpress; Initial Catalog=UploadImage; Integrated Security=True;";

        public HomeController(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Upload(IFormFile imageFile, string password)
        {
            var fileName = $"{Guid.NewGuid()}-{imageFile.FileName}";
            var fullImgPath = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", fileName);

            using FileStream fs = new FileStream(fullImgPath, FileMode.Create);
            imageFile.CopyTo(fs);

            var repo = new ImageRepository(_connectionString);
            int id = repo.Add(new Image
            {
                Password = password,
                ImagePath = fileName
            });

            UploadViewModel vm = new()
            {
                Id = id,
                Password = password,
                ImagePath = fullImgPath,
                Link = $"https://localhost:7240/home/viewimage?id={id}"
            };

            return View(vm);
        }



        [HttpGet]
        public IActionResult ViewImage(int id)
        {
            var repo = new ImageRepository(_connectionString);
            
            var imageIds = HttpContext.Session.Get<List<int>>("imageIds");
            if (imageIds == null)
            {
                imageIds = new();
            }

            ViewImageViewModel vm = new();

            if (TempData["incorrectPassword"] != null)
            {
                vm.IncorrectPassword = true;
            }

            if (imageIds.Contains(id))
            {
                vm.Unlocked = true;
                repo.IncrementView(id);
                HttpContext.Session.Set("imageIds", imageIds);
            }
           
            vm.Image = repo.GetImageById(id);
            return View(vm);
        }


        [HttpPost]
        public IActionResult ViewImage(int id, string password)
        {
            var repo = new ImageRepository(_connectionString);

            ViewImageViewModel vm = new();
            vm.Image = repo.GetImageById(id);

            if(password == null || password == vm.Image.Password)
            {
                vm.IncorrectPassword = false;
            }
            else
            {
                TempData["incorrectPassword"] = "Incorrect Password";
            }

            if(password == vm.Image.Password)
            {
                var imageIds = HttpContext.Session.Get<List<int>>("imageIds");
                if (imageIds == null)
                {
                    imageIds = new();
                }
                imageIds.Add(id);
                HttpContext.Session.Set("imageIds", imageIds);
                vm.Unlocked = true;
            }

            return Redirect($"/home/viewimage?id={id}");

        }



        #region
        ////add httppost???
        //public IActionResult ViewImage(int id, string password)
        //{
        //    var repo = new ImageRepository(_connectionString);

        //    var imageIds = HttpContext.Session.Get<List<int>>("imageIds");
        //    if (imageIds == null)
        //    {
        //        imageIds = new();
        //    }

        //    ViewImageViewModel vm = new();
        //    repo.IncrementView(id);
        //    vm.Image = repo.GetImageById(id);

        //    if (imageIds.Contains(id))
        //    {
        //        vm.Unlocked = true;
        //    }

        //    if (password == null || password == vm.Image.Password)
        //    {
        //        vm.IncorrectPassword = false;
        //    }
        //    else if (password != vm.Image.Password)
        //    {
        //        vm.IncorrectPassword = true;
        //    }

        //    if (password == vm.Image.Password)
        //    {
        //        vm.Unlocked = true;
        //        imageIds.Add(id);
        //        HttpContext.Session.Set("imageIds", imageIds);
        //    }

        //    return View(vm);
        #endregion

    }


}

