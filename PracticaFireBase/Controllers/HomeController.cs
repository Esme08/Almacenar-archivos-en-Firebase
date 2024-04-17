using Firebase.Auth;
using Firebase.Database.Streaming;
using Firebase.Storage;
using Microsoft.AspNetCore.Mvc;
using PracticaFireBase.Models;
using System.Diagnostics;


namespace PracticaFireBase.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }



        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        [HttpPost]

        public async Task<IActionResult> SubirArchivo(IFormFile archivo)
        {
            //Leemos el archivo subido.
            Stream archivoSubir = archivo.OpenReadStream();

            //Configuramos la conexión hacia Firebase
            string email = "esmeraldajasmi746@gmail.com";
            string clave = "holi888";
            string ruta = "storagefiles-6f67f.appspot.com";
            string api_key = "AIzaSyBLS0X2csfQacIYJHsXYDhhKMI31WCx0WA";

            var auth = new FirebaseAuthProvider(new FirebaseConfig(api_key));
            var autentificarFireBase = await auth.SignInWithEmailAndPasswordAsync(email, clave);
            var cancellation = new CancellationTokenSource();
            var tokenUser = autentificarFireBase.FirebaseToken;

            var tareaCargarArchivo = new FirebaseStorage(ruta,
                                     new FirebaseStorageOptions
                                     {
                                         AuthTokenAsyncFactory = () => Task.FromResult(tokenUser),
                                         ThrowOnCancel = true
                                     }
                                     ).Child("Archivos")
                                     .Child(archivo.FileName)
                                     .PutAsync(archivoSubir, cancellation.Token);

            var urlArchivoCargado = await tareaCargarArchivo;


            return RedirectToAction("VerImagen", new {urlImagen = urlArchivoCargado});
        }

        [HttpGet]
        public ActionResult VerImagen(string urlImagen) 
        { 
        return View((object)urlImagen);
        }
    }
}
